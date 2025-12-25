namespace Models;

public class EvacuationZoneRequest
{
    public string ZoneID { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int NumberOfPeople { get; set; }
    public int UrgencyLevel { get; set; }
}