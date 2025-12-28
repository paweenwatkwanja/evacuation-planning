using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("log")]
public class Log
{
    [Column("id")]
    public long Id { get; set; }

    [Column("zone_id")]
    public long ZoneID { get; set; }

    [Column("vehicle_id")]
    public long VehicleID { get; set; }

    [Column("total_evacuated")]
    public int TotalEvacuated { get; set; }

    [Column("remaining_people")]
    public int RemainingPeople { get; set; }

    [ForeignKey("ZoneID")]
    public EvacuationZone EvacuationZone { get; set; }

    [ForeignKey("VehicleID")]
    public Vehicle Vehicle { get; set; }
}