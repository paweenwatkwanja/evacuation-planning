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

    public List<EvacuationZoneResponse> ProcessEvacuationZones(List<EvacuationZoneRequest> requests)
    {
        List<EvacuationZone> evacuationZones = new List<EvacuationZone>();
        foreach (EvacuationZoneRequest request in requests)
        {
            EvacuationZoneBusinessLogic.ValidateEvacuationZoneRequest(request);
            EvacuationZone evacuationZone = new EvacuationZone()
            {
                ZoneID = request.ZoneID,
                Latitude = request.LocationCoordinates.Latitude,
                Longitude = request.LocationCoordinates.Longitude,
                NumberOfPeople = request.NumberOfPeople,
                UrgencyLevel = request.UrgencyLevel
            };
            evacuationZones.Add(evacuationZone);
        }

        _evacuationPlanningRepository.AddEvacautionZones(evacuationZones);

        List<EvacuationZoneResponse> responses = evacuationZones.Select(ez => new EvacuationZoneResponse
        {
            ZoneID = ez.ZoneID,
            LocationCoordinates = new LocationCoordinate()
            {
                Latitude = ez.Latitude,
                Longitude = ez.Longitude,
            },
            NumberOfPeople = ez.NumberOfPeople,
            UrgencyLevel = ez.UrgencyLevel
        }).ToList();

        return responses;
    }

    public List<VehicleResponse> ProcessVehicles(List<VehicleRequest> requests)
    {
        List<Vehicle> vehicles = new List<Vehicle>();
        foreach (VehicleRequest request in requests)
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
            vehicles.Add(vehicle);
        }

        _evacuationPlanningRepository.AddVehicles(vehicles);

        List<VehicleResponse> responses = vehicles.Select(v => new VehicleResponse
        {
            VehicleID = v.VehicleID,
            Capacity = v.Capacity,
            Type = v.Type,
            LocationCoordinates = new LocationCoordinate()
            {
                Latitude = v.Latitude,
                Longitude = v.Longitude,
            },
            Speed = v.Speed
        }).ToList();

        return responses;
    }
}