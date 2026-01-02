using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models;

[Index(nameof(ZoneID), IsUnique = true)]
[Table("evacuation_zone")]
public class EvacuationZone
{
    [Column("id")]
    public long Id { get; set; }

    [Column("zone_id")]
    public string ZoneID { get; set; }

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }

    [Column("number_of_people")]
    public int NumberOfPeople { get; set; }

    [Column("urgency_level")]
    public int UrgencyLevel { get; set; }
}