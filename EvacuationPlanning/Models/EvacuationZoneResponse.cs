namespace Models;

public class EvacuationZoneResponse
{
    public long Id { get; set; }
    public string ZoneID { get; set; }
    public LocationCoordinate LocationCoordinates { get; set; }
    public int NumberOfPeople { get; set; }
    public int UrgencyLevel { get; set; }
}