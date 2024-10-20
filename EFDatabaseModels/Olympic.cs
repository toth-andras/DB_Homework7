using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework7.DatabaseModels;

[Table("olympics")]
public class Olympic
{
    [Key]
    [Column("olympic_id", TypeName = "char(7)")]
    public string OlympicId { get; set; }
    
    [ForeignKey("Country")]
    [Column("country_id", TypeName = "char(3)")]
    public string CountryId { get; set; }
    public Country Country { get; set; }
    
    [Column("city", TypeName = "char(50)")]
    public string City { get; set; }
    
    [Column("year")]
    public int Year { get; set; }
    
    [Column("startdate")]
    public DateTime StartDate { get; set; }
    
    [Column("enddate")]
    public DateTime EndDate { get; set; }
}
