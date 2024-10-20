using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Homework7.DatabaseModels;

[Table("results")]
[PrimaryKey(nameof(EventId), nameof(PlayerId))]
public class Result
{
    [ForeignKey("Event")]
    [Column("event_id", TypeName = "char(7)")]
    public string EventId { get; set; }
    public Event Event { get; set; }
    
    [ForeignKey("Player")]
    [Column("player_id", TypeName = "char(10)")]
    public string PlayerId { get; set; }
    public Player Player { get; set; }

    [Column("medal", TypeName = "char(7)")]
    public string Medal { get; set; }
    
    [Column("result", TypeName = "double precision")]
    public double ResultValue { get; set; }
}
