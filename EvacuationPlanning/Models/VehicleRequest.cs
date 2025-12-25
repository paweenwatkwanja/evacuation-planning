namespace Models;

public class VehicleRequest
{
    public string VehicleID { get; set; }
    public int Capacity { get; set; }
    public string Type { get; set; }
    public LocationCoordinate LocationCoordinates { get; set; }
    public int Speed { get; set; }
}