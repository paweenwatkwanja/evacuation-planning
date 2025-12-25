namespace Models;

public class EvacuationZoneResponse
{
    public string ZoneID { get; set; }
    public LocationCoordinate LocationCoordinate { get; set; }
    public int NumberOfPeople { get; set; }
    public int UrgencyLevel { get; set; }
}