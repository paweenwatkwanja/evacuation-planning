namespace Models;

public class EvacuationZoneRequest
{
    public string ZoneID { get; set; }
    public LocationCoordinate LocationCoordinates { get; set; }
    public int NumberOfPeople { get; set; }
    public int UrgencyLevel { get; set; }
}