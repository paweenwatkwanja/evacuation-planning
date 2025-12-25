using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("evacuation_plan")]
public class EvacuationPlan
{
    [Column("id")]
    public long Id { get; set; }

    [Column("zone_id")]
    public long ZoneID { get; set; }

    [Column("vehicle_id")]
    public long VehicleID { get; set; }

    [Column("eta")]
    public int ETA { get; set; }

    [Column("number_of_people")]
    public int NumberOfPeople { get; set; }

    [ForeignKey("ZoneID")] 
    public EvacuationZone EcavuationZone { get; set; } 
    
    [ForeignKey("VehicleID")] 
    public Vehicle Vehicle { get; set; } 
}