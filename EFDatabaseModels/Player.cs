using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework7.DatabaseModels;

[Table("players")]
public class Player
{
    [Key]
    [Column("player_id", TypeName = "char(10)")]
    public string PlayerId { get; set; }
    
    [ForeignKey("Country")]
    [Column("country_id", TypeName = "char(3)")]
    public string CountryId { get; set; }
    public Country Country { get; set; }

    [Column("player_name")]
    public string Name { get; set; }
    
    [Column("birthdate")]
    public DateTime BirthDate { get; set; }
}
