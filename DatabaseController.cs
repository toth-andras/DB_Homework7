using Bogus;
using Bogus.DataSets;
using Homework7.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework7;

[ApiController]
public class DatabaseController : ControllerBase
{
    public readonly DatabaseContext _ctx;

    public DatabaseController(DatabaseContext ctx) => _ctx = ctx;
    
    /// <summary>
    /// Заполнить базу данных автоматически сгенерированными данными
    /// </summary>
    /// <param name="numberOfOlympics">Количество Олимпиад, для которых будут сгенерированы данные</param>
    [HttpPost("initialSeed")]
    public async Task SeedTheDatabase(int numberOfOlympics)
    {
        await Seed(numberOfOlympics);

        await _ctx.SaveChangesAsync();
    }

    [HttpGet("info2004")]
    public List<Info2004ListItem> GetInfoFor2004()
    {
        // Находим олимпиаду 2004 года
        var olympic = _ctx.Olympics.FirstOrDefault(x => x.Year == 2004);
        if (olympic is null)
        {
            return new List<Info2004ListItem>();
        }

        // Отбираем всех игроков, которые выиграли золото на олимпиаде 2004 года
        // (Один игрок может входить в выборку несколько раз, если выиграл несколько золотых медалей)
        var players = _ctx.Results
            .Where(x => x.Event.OlympicId == olympic.OlympicId && x.Medal == "Gold")
            .Include(result => result.Player);

        // Группируем записи об игроках по годам, для каждого года считаем кол-во медалей(равно кол-ву записей)
        // и кол-во игроков: для этого отбираем id, преобразуем полученный набор в HashSet (чтобы убрать дубликаты)
        // и считаем его размер
        return players
            .GroupBy(x => x.Player.BirthDate.Year)
            .Select(grouping => new Info2004ListItem(
                grouping.Key,
                grouping.Select(y => y.PlayerId).ToHashSet().Count, 
                grouping.Count()))
            .ToList();
    }

    [HttpGet("twoOrMoreGolds")]
    public List<string> TwoOrMoreGolds()
    {
        // 1) Отбираем результаты негрупповых событий
        // 2) группируем результаты по событию
        // 3) Отбираем группы, в которых есть хотя бы 2 золотые медали
        // 4) Берем id события для каждой из отобранных групп 
       return _ctx.Results
            .Where(x => x.Event.IsTeamEvent == 0)
            .GroupBy(x => x.EventId)
            .Where(x => x.Count(x => x.Medal == "Gold") > 1)
            .Select(x => x.First().Event.EventId)
            .ToList();
    }

    [HttpGet("atLeastOneMedal")]
    public List<AtLeastOneMedalListItem> AtLeastOneMedal()
    {
        return _ctx.Results
            .Where(x => x.Medal == "Gold" || x.Medal == "Silver" || x.Medal == "Bronze")
            .Include(result => result.Player)
            .Include(result => result.Event)
            .ToList()
            .DistinctBy(x => x.PlayerId) // Убираем дубликаты (когда один игрок выиграл несколько медалей)
            .ToList()
            .Select(x => new AtLeastOneMedalListItem(x.Player.Name, x.Event.OlympicId))
            .ToList();
    }

    [HttpGet("topVowelStartingNamesCountry")]
    public string TopVowelStartingNamesCountry()
    {
        var vowels = new HashSet<char> { 'A', 'E', 'I', 'O', 'U' };

        return _ctx.Players
            .Include(x => x.Country) // Необходимо для корректной работы ORM 
            .GroupBy(x => x.CountryId) // Группируем игроков по стране
            .AsEnumerable() // Необходимо для корректной работы ORM
            .MaxBy(x => x.Count(x => vowels.Contains(x.Name.First()))) // Выбираем группу с самым большим кол-вом игроков, чьи имена начинаются на гласную
            .First().Country.Name; // Возвращаем название страны
    }

    [HttpGet("groupMedalPerPopulation")]
    public List<Country> GroupMedalPerPopulation()
    {
        // Ищем олимпиаду 2000 года
        var olympic = _ctx.Olympics.FirstOrDefault(x => x.Year == 2000);
        if (olympic is null)
        {
            return new List<Country>();
        }

        // Для кажой страны подсчитываем кол-во групповых медалей
        var groupMedalPerCountry = _ctx.Results
            .Include(x => x.Player) // Необходимо для корректной работы ORM
            .Include(x => x.Event) // Необходимо для корректной работы ORM
            .Include(x => x.Event.Olympic) // Необходимо для корректной работы ORM
            .Include(x => x.Event.Olympic.Country) // Необходимо для корректной работы ORM
            .Where(x => x.Event.OlympicId == olympic.OlympicId) // Отбираем результаты групповых событий нужной олимпиады с медалями
            .Where(x => x.Event.IsTeamEvent == 1)
            .Where(x => x.Medal == "Gold" || x.Medal == "Silver" || x.Medal == "Bronze")
            .GroupBy(x => x.Player.Country.CountryId) // Группируем события по странам
            .ToDictionary(
                x => x.Key,
                x => x.Select(y => y.EventId).ToHashSet().Count); // С помощью ToHashSet считаем только "уникальные медали":
                                                                                           // из всех медалей за одно событие у команды считается одна

        return _ctx.Countries
            .AsEnumerable() // Необходимо для корректной работы ORM
            .OrderBy(x => groupMedalPerCountry.GetValueOrDefault(x.CountryId, 0) / x.Population) // Производим сортировку по отношению кол-ва медалей к населению
            .Take(5) // Берем первые 5
            .ToList();
    }
    
    
    // По-хорошему, этот код необходимо вынести в отдельный класс,
    // но для простоты проверки я решил рставить его здесь

    private async Task Seed(int numberOfOlympics)
    {
        var countries = CreateCountries(6);
        var players = CreatePlayers(numberOfOlympics * 5, countries);
        var olympics = CreateOlympics(numberOfOlympics, countries);
        var events = CreateEvents(olympics);
        var results = CreateResults(events, players);
        
        await _ctx.Countries.AddRangeAsync(countries);
        await _ctx.Players.AddRangeAsync(players);
        await _ctx.Olympics.AddRangeAsync(olympics);
        await _ctx.Events.AddRangeAsync(events);
        await _ctx.Results.AddRangeAsync(results);
    }

    private List<Country> CreateCountries(int countriesNumber)
    {
        HashSet<string> createdCountries = [];
        List<Country> countries = [];
        
        while (createdCountries.Count != countriesNumber)
        {
            var country = CreateCountry();

            if (createdCountries.Contains(country.CountryId))
            {
                continue;
            }

            createdCountries.Add(country.CountryId);
            countries.Add(country);
        }

        return countries;
        
        Country CreateCountry()
        {
            return new Faker<Country>()
                .CustomInstantiator(x => new Country()
                {
                    Name = new string(x.Address.Country().Take(40).ToArray()),
                    CountryId = x.Address.CountryCode(Iso3166Format.Alpha3),
                    AreaSqkm = x.Random.Int(min: 100, max: 17100000),
                    Population = x.Random.Int(min: 100, 1500000000)
                });
        }
    }

    private List<Player> CreatePlayers(int playersNumber, IReadOnlyList<Country> countries)
    {
        var ids = new HashSet<string>();
        while (ids.Count != playersNumber)
        {
            ids.Add(new Faker<string>().CustomInstantiator(x => x.Random.Long(1000000000, 9999999999).ToString()));
        }

        var rnd = new Random();
        return ids.Select(id =>
        {
            Player p = new Faker<Player>().CustomInstantiator(x => new Player()
            {
                Name = x.Name.FullName(),
                BirthDate = new DateTime(2000, rnd.Next(1, 12), rnd.Next(1, 28)).AddYears(- rnd.Next(16, 25)).ToUniversalTime(),
                Country = x.PickRandom<Country>(countries),
                PlayerId = id
            });

            return p;
        }).ToList();
    }

    private List<Olympic> CreateOlympics(int numberOfOlympics, IReadOnlyList<Country> countries)
    {

        List<int> years = [2000, 2004];
        while (years.Count != numberOfOlympics)
        {
            years.Add(years[^1] + 4);
        }
        var ids = new HashSet<string>();
        while (ids.Count != numberOfOlympics)
        {
            ids.Add(new Faker<string>().CustomInstantiator(x => x.Random.Long(1000000, 9999999).ToString()));
        }

        var index = 0;
        var rnd = new Random();
        return ids.Select(id =>
        {
            var startDate = new DateTime(years[index++], rnd.Next(1, 12), rnd.Next(1, 28));
            var endDate = startDate.AddDays(rnd.Next(20, 40));
            
            Olympic o = new Faker<Olympic>().CustomInstantiator(x => new Olympic()
            {
                OlympicId = id,
                Country = x.PickRandom<Country>(countries),
                City = x.Address.City(),
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate.ToUniversalTime(),
                Year = startDate.Year
            });
            return o;
        }).ToList();
    }

    private List<Event> CreateEvents(IReadOnlyList<Olympic> olympics)
    {
        var rnd = new Random();
        
        return olympics.SelectMany(olympic =>
        {
            List<string> olympicSports =
            [
                "Archery",
                "Athletics",
                "Badminton",
                "Baseball",
                "Basketball",
                "Boxing",
                "Canoeing",
                "Cycling",
                "Diving",
                "Equestrian",
                "Fencing",
                "Football",
                "Golf",
                "Gymnastics",
                "Handball",
                "Hockey",
                "Judo",
                "Karate",
                "Rowing",
                "Rugby",
                "Sailing",
                "Shooting",
                "Skateboarding",
                "Surfing",
                "Swimming",
                "Table Tennis",
                "Taekwondo",
                "Tennis",
                "Triathlon",
                "Volleyball",
                "Water Polo",
                "Weightlifting",
                "Wrestling"
            ];

            
            List<string> eventIds = [];
            var eventNum = rnd.Next(3, 6);
            while (eventIds.Count != eventNum)
            {
                eventIds.Add(new Faker<string>().CustomInstantiator(x => x.Random.Long(1000000, 9999999).ToString()));
            }

            return eventIds.Select(id =>
            {
                Event e = new Faker<Event>().CustomInstantiator(x =>
                {
                    var isTeam = x.PickRandom(0, 1);
                    return new Event()
                    {
                        EventId = id,
                        EventType = x.PickRandom(olympicSports),
                        IsTeamEvent = isTeam,
                        Olympic = olympic,
                        Name = x.PickRandom("Final", "Semifinal", "Quarterfinal", "GroupCompetition"),
                        NumPlayersInTeam = isTeam == 0 ? 0 : x.Random.Int(2, 20),
                        ResultNotedIn = "results"
                    };
                });

                return e;
            });
        }).ToList();
    }

    private List<Result> CreateResults(IReadOnlyList<Event> events, IReadOnlyList<Player> players)
    {
        var playersPerCountry = players.GroupBy(x => x.CountryId).ToList();
        
        return events.SelectMany(sportEvent =>
        {
            if (sportEvent.IsTeamEvent == 0)
            {
                return CreatePersonalEventResults(sportEvent);
            }
            
            return CreateTeamEventResults(sportEvent);
        }).ToList();

        IEnumerable<Result> CreatePersonalEventResults(Event sportEvent)
        {
            // shuffle the players list
            List<Player> newPlayersList = [];
            foreach (var player in players)
            {
                if (new Random().Next(1000) % 2 == 0)
                {
                    newPlayersList.Add(player);
                }
                else
                {
                    newPlayersList.Insert(0, player);
                }
            }

            var index = 0;
            return Enumerable.Range(1, new Random().Next(3, 6)).Select(medal =>
            {
                Result r = new Faker<Result>().CustomInstantiator(x =>
                    new Result()
                    {
                        Event = sportEvent,
                        Player = newPlayersList[index++],
                        Medal = GetMedalByPlace(medal),
                        ResultValue = x.Random.Double()
                    });
                return r;
            });
        }
        
        IEnumerable<Result> CreateTeamEventResults(Event sportEvent)
        {
            List<IGrouping<string, Player>> newTeams = [];
            foreach (var team in newTeams)
            {
                if (new Random().Next(1000) % 2 == 0)
                {
                    newTeams.Add(team);
                }
                else
                {
                    newTeams.Insert(0, team);
                }
            }
            var index = 0;
            return Enumerable.Range(1, Math.Min(new Random().Next(3, 5), newTeams.Count)).SelectMany(medal =>
            {
                var command = newTeams[index++];

                return command.Select(member =>
                {
                    Result r = new Faker<Result>().CustomInstantiator(x =>
                        new Result()
                        {
                            Event = sportEvent,
                            Player = member,
                            Medal = GetMedalByPlace(medal),
                            ResultValue = x.Random.Double()
                        });

                    return r;
                });
            });
        }
        
        string? GetMedalByPlace(int res) => res switch
        {
            1 => "Gold",
            2 => "Silver",
            3 => "Bronze",

            _ => "NoMedal"
        };
    }

    public record Info2004ListItem(
        int Year,
        int PlayersBornThisYear,
        int GoldMedals);

    public record AtLeastOneMedalListItem(
        string PlayerName,
        string OlympicId);
    
    
}