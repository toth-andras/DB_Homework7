using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework7.DatabaseModels;

[Table("events")]
public class Event
{
    [Key]
    [Column("event_id", TypeName = "char(7)")]
    public string EventId { get; set; }

    [Column("event_name", TypeName = "char(40)")]
    public string Name { get; set; }
    
    [Column("eventtype", TypeName = "char(20)")]
    public string EventType { get; set; }

    [ForeignKey("Olympic")]
    [Column("olympic_id", TypeName = "char(7)")]
    public string OlympicId { get; set; }
    public Olympic Olympic { get; set; }

    [Column("is_team_event")]
    public int IsTeamEvent { get; set; }
    
    [Column("num_players_in_team")]
    public int NumPlayersInTeam { get; set; }
    
    [Column("result_noted_in", TypeName = "char(100)")]
    public string ResultNotedIn { get; set; }
}
