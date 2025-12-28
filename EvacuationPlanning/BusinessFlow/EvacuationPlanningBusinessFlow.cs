using BusinessLogic;
using Models;
using Repository;
using System.Linq.Expressions;
using Helpers;
using System.Security.Cryptography.X509Certificates;

namespace BusinessFlow;

public class EvacuationPlanningBusinessFlow
{
    private readonly IUnitOfWork _unitOfWork;
    public EvacuationPlanningBusinessFlow(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // EVACUTION ZONE
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

    // VEHICLE
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

    //EVACUATION PLAN
    public async Task<List<EvacuationPlan>> ProcessEvacuationPlanAsync()
    {
        IEnumerable<EvacuationZone> evacuationZones = await getEvacuationZonesAsync();
        if (evacuationZones.Count <= 0)
        {
            // hadle here
        }

        IEnumerable<Vehicle> vehicles = await getAvailableVehiclesAsync();
        if (vehicles.Count <= 0)
        {
            // hadle here
        }

        List<EvacuationPlan> evacuationPlans = prepareDataAndCreateEvacuationPlans(evacuationZones, vehicles);
        await AddEvacuationPlanAsync(evacuationPlans);

        return evacuationPlans.ToList();
    }

    private async IEnumerable<EvacuationZone> getEvacuationZonesAsync()
    {
        return await _unitOfWork.EvacuationZones.GetAllAsync();
    }

    private async IEnumerable<Vehicle> getAvailableVehiclesAsync()
    {
        // filter unavailable vehicle
        // get in use vehicles from cache then create a predicate vehicle NOT in cache
        return await _unitOfWork.Vehicles.GetAllAsync();
    }

    private List<EvacuationPlan> prepareDataAndCreateEvacuationPlans(IEnumerable<EvacuationZone> sortedEvacuationZones, IEnumerable<Vehicle> vehicles)
    {
        sortedEvacuationZones = sortedEvacuationZones.OrderByDescending(o => o.UrgencyLevel);
        vehicles = vehicles.OrderByDescending(o => o.Capacity).ToList();

        List<EvacuationPlan> evacuationPlans = new List<EvacuationPlan>();
        foreach (EvacuationZone evacuationZone in sortedEvacuationZones)
        {
            Vehicle vehicle = EvacuationPlanBusinessLogic.FindAppropriateVehicle(evacuationZone.NumberOfPeople, vehicles);

            int remainingEvacuee = evacuationZone.NumberOfPeople - vehicle.Capacity;
            EvacuationPlan evacuationPlan = new EvacuationPlan()
            {
                ZoneID = evacuationZone.ZoneID,
                VehicleID = vehicle.VehicleID,
                NumberOfPeople = (remainingEvacuee <= 0) ? evacuationZone.NumberOfPeople : vehicle.Capacity,
                ETA = (int)vehicle.Distance * 60 / vehicle.Speed
            };
            evacuationPlans.Add(evacuationPlan);

            vehicles.RemoveAll(v => v.VehicleID == vehicle.VehicleID);
        }
        return evacuationPlans;
    }

    private async void AddEvacuationPlanAsync(List<EvacuationPlan> evacuationPlans)
    {
        await _unitOfWork.EvacuationPlans.AddRangeAsync(evacuationPlans);
        await _unitOfWork.SaveChangesAsync();
    }
}