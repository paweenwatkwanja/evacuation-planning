using Models;

namespace BusinessLogic;

public static class VehicleBusinessLogic
{
    public static void ValidateVehicleRequest(VehicleRequest request)
    {
        Console.WriteLine("BusinessLogic");
        if (request == null)
        {
            throw new ArgumentException("Request cannot be null or empty");
        }

        if (string.IsNullOrEmpty(request.VehicleID))
        {
            throw new ArgumentException("VehicleID cannot be null or empty");
        }

        if (request.Capacity < 0)
        {
            throw new ArgumentException("Capacity cannot be negative");
        }

         if (string.IsNullOrEmpty(request.Type))
        {
            throw new ArgumentException("Type cannot be null or empty");
        }

        if (request.LocationCoordinates.Latitude < -90 || request.LocationCoordinates.Latitude > 90)
        {
            throw new ArgumentException("Latitude must be between -90 and 90");
        }

        if (request.LocationCoordinates.Longitude < -180 || request.LocationCoordinates.Longitude > 180)
        {
            throw new ArgumentException("Longitude must be between -180 and 180");
        }

         if (request.Speed < 0)
        {
            throw new ArgumentException("Speed cannot be negative");
        }
    }
}