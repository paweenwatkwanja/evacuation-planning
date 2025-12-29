using Models;

namespace BusinessLogic;

public static class EvacuationZoneBusinessLogic
{
    public static void ValidateEvacuationZoneRequest(EvacuationZoneRequest request)
    {
        if (request == null)
        {
            throw new ArgumentException("Request cannot be null or empty");
        }

        if (string.IsNullOrEmpty(request.ZoneID))
        {
            throw new ArgumentException("ZoneID cannot be null or empty");
        }

        if (request.LocationCoordinates.Latitude < -90 || request.LocationCoordinates.Latitude > 90)
        {
            throw new ArgumentException("Latitude must be between -90 and 90");
        }

        if (request.LocationCoordinates.Longitude < -180 || request.LocationCoordinates.Longitude > 180)
        {
            throw new ArgumentException("Longitude must be between -180 and 180");
        }

        if (request.NumberOfPeople < 0)
        {
            throw new ArgumentException("NumberOfPeople cannot be negative");
        }

        if (request.UrgencyLevel < 1 || request.UrgencyLevel > 5)
        {
            throw new ArgumentException("UrgencyLevel must be between 1 and 5");
        }
    }
}