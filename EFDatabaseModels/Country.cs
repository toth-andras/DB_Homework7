using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework7.DatabaseModels;

[Table("countries")]
public class Country
{
    [Column("country_name", TypeName = "char(40)")]
    public string Name { get; set; }
    
    [Key]
    [Column("country_id", TypeName = "char(3)")]
    public string CountryId { get; set; }
    
    [Column("area_sqkm")]
    public int AreaSqkm { get; set; }
    
    [Column("population")]
    public int Population { get; set; }
}