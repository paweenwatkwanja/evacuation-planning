using System.Security.Cryptography.X509Certificates;

namespace Models;

public class EvacuationStatusUpdateRequest
{
    public int NumberOfEvacuee { get; set; }
    public string VehicleID { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}