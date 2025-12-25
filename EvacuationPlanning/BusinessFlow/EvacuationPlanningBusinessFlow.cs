using BusinessLogic;
using Repository;
using Models;

namespace BusinessFlow;

public class EvacuationPlanningBusinessFlow
{
    private readonly EvacuationPlanningRepository _evacuationPlanningRepository;
    public EvacuationPlanningBusinessFlow(EvacuationPlanningRepository evacuationPlanningRepository)
    {
        _evacuationPlanningRepository = evacuationPlanningRepository;
    }

    public EvacuationZoneResponse ProcessEvacuationZone(EvacuationZoneRequest request)
    {
        Console.WriteLine("BusinessFlow");
        EvacuationZoneBusinessLogic.ValidateEvacuationZoneRequest(request);

        EvacuationZone evacuationZone = new EvacuationZone()
        {
            ZoneID = request.ZoneID,
            Latitude = request.LocationCoordinates.Latitude,
            Longitude = request.LocationCoordinates.Longitude,
            NumberOfPeople = request.NumberOfPeople,
            UrgencyLevel = request.UrgencyLevel
        };

        _evacuationPlanningRepository.AddEvacautionZone(evacuationZone);

        EvacuationZoneResponse response = new EvacuationZoneResponse()
        {
            ZoneID = request.ZoneID,
            LocationCoordinates = new LocationCoordinate()
            {
                Latitude = request.LocationCoordinates.Latitude,
                Longitude = request.LocationCoordinates.Longitude,
            },
            NumberOfPeople = request.NumberOfPeople,
            UrgencyLevel = request.UrgencyLevel
        };

        return response;
    }

    public VehicleResponse ProcessVehicle(VehicleRequest request)
    {
        VehicleBusinessLogic.ValidateVehicleRequest(request);

        Vehicle vehicle = new Vehicle()
        {
            VehicleID = request.VehicleID,
            Capacity = request.Capacity,
            Type = request.Type,
            Latitude = request.LocationCoordinates.Latitude,
            Longitude = request.LocationCoordinates.Longitude,
            Speed = request.Speed
        };

        _evacuationPlanningRepository.AddVehicle(vehicle);

        VehicleResponse response = new VehicleResponse()
        {
            VehicleID = request.VehicleID,
            Capacity = request.Capacity,
            Type = request.Type,
            LocationCoordinates = new LocationCoordinate()
            {
                Latitude = request.LocationCoordinates.Latitude,
                Longitude = request.LocationCoordinates.Longitude,
            },
            Speed = request.Speed
        };

        return response;
    }
}