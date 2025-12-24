using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("location_coordinate")]
public class LocationCoordinate
{
    [Column("id")]
    public long Id { get; set; }

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }
}