using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models;

[Table("evacuation_status")]
public class EvacuationStatus
{
    [Column("id")]
    public long Id { get; set; }

    [Column("zone_id")]
    public long ZoneID { get; set; }

    [Column("total_evacuated")]
    public int TotalEvacuated { get; set; }

    [Column("remaining_people")]
    public int RemainingPeople { get; set; }

    [Column("last_vehicle_used")]
    public long LastVehicleUsed { get; set; }

    [ForeignKey("LastVehicleUsed")]
    [JsonIgnore]
    public Vehicle Vehicle { get; set; }

    [ForeignKey("ZoneID")]
    public EvacuationZone EvacuationZone { get; set; }

    [Column("is_evacuation_completed")]
    public bool IsEvacuationCompleted { get; set; }
}