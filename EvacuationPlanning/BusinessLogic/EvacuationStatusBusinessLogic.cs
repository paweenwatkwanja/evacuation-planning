namespace BusinessLogic;

using Models;

public static class EvacuationStatusBusinessLogic
{
    public static void ValidateEvacuationStatusRequest(EvacuationStatusUpdateRequest request)
    {
        if (request == null)
        {
            throw new ValidationException("Request cannot be null or empty");
        }

        if (request.NumberOfEvacuee < 0)
        {
            throw new ValidationException("NumberOfEvacuee cannot be negative");
        }
        
        if (string.IsNullOrEmpty(request.VehicleID))
        {
            throw new ValidationException("VehicleID cannot be null or empty");
        }   

        if (request.Latitude < -90 || request.Latitude > 90)
        {
            throw new ValidationException("Latitude must be between -90 and 90");
        }

        if (request.Longitude < -180 || request.Longitude > 180)
        {
            throw new ValidationException("Longitude must be between -180 and 180");
        }
    }
}