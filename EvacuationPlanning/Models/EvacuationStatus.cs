using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models;

public class EvacuationStatus
{
    public string ZoneID { get; set; }
    public int TotalEvacuated { get; set; }
    public int RemainingPeople { get; set; }
    public long LastVehicleUsed { get; set; }

    [ForeignKey("LastVehicleUsed")]
    [JsonIgnore]
    public Vehicle Vehicle { get; set; }
}