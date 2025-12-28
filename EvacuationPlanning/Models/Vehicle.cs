using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("vehicle")]
public class Vehicle
{
    [Column("id")]
    public long Id { get; set; }

    [Column("vehicle_id")]
    public string VehicleID { get; set; }

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("type")]
    public string Type { get; set; }

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }

    [Column("speed")]
    public int Speed { get; set; }

    [NotMapped]
    public int Distance  { get; set; }
}