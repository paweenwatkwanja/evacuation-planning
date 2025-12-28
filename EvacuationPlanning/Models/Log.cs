using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("log")]
public class Log
{
    [Column("id")]
    public long Id { get; set; }

    [Column("evacuation_plan_id")]
    public long EvacuationPlanID { get; set; }
    
    [ForeignKey("EvacuationPlanID")]
    public EvacuationPlan EvacuationPlan { get; set; }

    [Column("vehicle_id")]
    public long VehicleID { get; set; }

    [ForeignKey("VehicleID")]
    public Vehicle Vehicle { get; set; }

    [Column("eta")]
    public double ETA { get; set; }

    [Column("is_evacuation_completed")]
    public bool IsEvacuationCompleted { get; set; }
}