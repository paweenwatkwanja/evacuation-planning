using BusinessLogic;
using Models;
using Repository;

namespace BusinessFlow;

public class EvacuationPlanningBusinessFlow
{
    private readonly IUnitOfWork _unitOfWork;
    public EvacuationPlanningBusinessFlow(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EvacuationZoneResponse>> ProcessEvacuationZonesAsync(List<EvacuationZoneRequest> requests)
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

        await _unitOfWork.EvacuationZones.AddRangeAsync(evacuationZones);
        await _unitOfWork.SaveChangesAsync();

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

    public async Task<List<VehicleResponse>> ProcessVehiclesAsync(List<VehicleRequest> requests)
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

        await _unitOfWork.Vehicles.AddRangeAsync(vehicles);
        await _unitOfWork.SaveChangesAsync();

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

    public void CreateEvacuationPlan()
    {
        // get evacuation zone

        // sort it by urgency level
        // if there are more than one, sort by distance
        // sorted by nunber of people
    }
}