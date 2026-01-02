using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class EvacuationPlanResponse
{
    public long Id { get; set; }
    public long ZoneID { get; set; }
    public long VehicleID { get; set; }
    public string ETA { get; set; }
    public int NumberOfPeople { get; set; }
    
    [ForeignKey("ZoneID")] 
    public EvacuationZone EvacuationZone { get; set; } 
    
    [ForeignKey("VehicleID")] 
    public Vehicle Vehicle { get; set; } 
}