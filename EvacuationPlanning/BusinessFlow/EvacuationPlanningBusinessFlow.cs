using BusinessLogic;
using Models;
using Repository;
using System.Linq.Expressions;
using Helpers;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Services;

namespace BusinessFlow;

public class EvacuationPlanningBusinessFlow
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly RedisService _redisService;

    public EvacuationPlanningBusinessFlow(IUnitOfWork unitOfWork, RedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
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
        List<EvacuationZone> evacuationZones = await getEvacuationZonesAsync();
        if (evacuationZones.Count <= 0)
        {
            // hadle here
        }

        List<Vehicle> vehicles = await getAvailableVehiclesAsync();
        if (vehicles.Count <= 0)
        {
            // hadle here
        }

        _unitOfWork.BeginTransaction();
        List<EvacuationPlan> evacuationPlans = prepareDataAndCreateEvacuationPlans(evacuationZones, vehicles);
        await addEvacuationPlanAsync(evacuationPlans);
        List<EvacuationStatus> evacuationStatuses = convertEvacuationPlansToEvacuationStatuses(evacuationPlans);
        await addAndCacheEvacuationStatusesAsync(evacuationStatuses);
        List<Log> logs = convertEvacuationPlansToLogs(evacuationPlans);
        await addLogsAsync(logs);
        _unitOfWork.CommitTransaction();

        return evacuationPlans.ToList();
    }

    private async Task<List<EvacuationZone>> getEvacuationZonesAsync()
    {
        return await _unitOfWork.EvacuationZones.GetAllAsync();
    }

    private async Task<List<Vehicle>> getAvailableVehiclesAsync()
    {
        // filter unavailable vehicle
        // get in use vehicles from cache then create a predicate vehicle NOT in cache
        return await _unitOfWork.Vehicles.GetAllAsync();
    }

    private List<EvacuationPlan> prepareDataAndCreateEvacuationPlans(List<EvacuationZone> evacuationZones, List<Vehicle> vehicles)
    {
        List<EvacuationZone> sortedEvacuationZones = evacuationZones.OrderByDescending(o => o.UrgencyLevel).ToList();
        List<Vehicle> sortedVehicles = vehicles.OrderByDescending(o => o.Capacity).ToList();

        List<EvacuationPlan> evacuationPlans = new List<EvacuationPlan>();
        foreach (EvacuationZone evacuationZone in sortedEvacuationZones)
        {
            Vehicle vehicle = EvacuationPlanBusinessLogic.FindAppropriateVehicle(evacuationZone, sortedVehicles);

            int remainingPeople = evacuationZone.NumberOfPeople - vehicle.Capacity;
            EvacuationPlan evacuationPlan = new EvacuationPlan()
            {
                ZoneID = evacuationZone.Id,
                VehicleID = vehicle.Id,
                NumberOfPeople = (remainingPeople <= 0) ? evacuationZone.NumberOfPeople : vehicle.Capacity,
                ETA = vehicle.Distance * (double)60 / (double)vehicle.Speed,
                RemainingPeople = (remainingPeople > 0) ? remainingPeople : 0
            };
            evacuationPlans.Add(evacuationPlan);

            vehicles.RemoveAll(v => v.VehicleID == vehicle.VehicleID);
        }
        return evacuationPlans;
    }

    private async Task addEvacuationPlanAsync(List<EvacuationPlan> evacuationPlans)
    {
        await _unitOfWork.EvacuationPlans.AddRangeAsync(evacuationPlans);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task addAndCacheEvacuationStatusesAsync(List<EvacuationStatus> evacuationStatuses)
    {
        await _unitOfWork.EvacuationStatuses.AddRangeAsync(evacuationStatuses);
        await _unitOfWork.SaveChangesAsync();

        await _redisService.SetCacheAsync("EvacuationStatuses", evacuationStatuses);
    }
    
    private List<EvacuationStatus> convertEvacuationPlansToEvacuationStatuses(List<EvacuationPlan> evacuationPlans)
    {
        List<EvacuationStatus> evacuationStatuses = new List<EvacuationStatus>();
        foreach (EvacuationPlan plan in evacuationPlans)
        {
            EvacuationStatus log = new EvacuationStatus()
            {
                ZoneID = plan.EvacuationZone.Id,
                TotalEvacuated = 0,
                RemainingPeople = plan.EvacuationZone.NumberOfPeople,
                LastVehicleUsed = plan.Vehicle.Id
            };
            evacuationStatuses.Add(log);
        }
        return evacuationStatuses;
    }

    private List<Log> convertEvacuationPlansToLogs(List<EvacuationPlan> evacuationPlans)
    {
        List<Log> logs = new List<Log>();
        foreach (EvacuationPlan plan in evacuationPlans)
        {
            Log log = new Log()
            {
                VehicleID = plan.Vehicle.Id,
                ETA = plan.ETA,
                IsEvacuationCompleted = false
            };
            logs.Add(log);
        }
        return logs;
    }

    private async Task addLogsAsync(List<Log> logs)
    {
        await _unitOfWork.Logs.AddRangeAsync(logs);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<EvacuationStatus>> GetEvacuationStatusAsync()
    {
         List<EvacuationStatus> cachedEvacuationStatuses = await _redisService.GetCacheAsync<List<EvacuationStatus>>("EvacuationStatuses");
         if (cachedEvacuationStatuses != null)
         {
             Console.WriteLine("Evacuation statuses retrieved from cache.");
             return cachedEvacuationStatuses;
         }

        List<EvacuationStatus> evacuationStatuses = await _unitOfWork.EvacuationStatuses.GetAllAsync("EvacuationZone", "Vehicle");
        await _redisService.SetCacheAsync("EvacuationStatuses", evacuationStatuses);
        Console.WriteLine("Evacuation statuses retrieved from database and cached.");
        return evacuationStatuses;
    }

    public async Task UpdateEvacuationStatusAsync(int id, EvacuationStatusUpdateRequest request)
    {
        // validate request
        EvacuationStatus evacuationStatus = await _unitOfWork.EvacuationStatuses.FindOneAsync(p => p.Id == id);
        if (evacuationStatus == null)
        {
            // handle
        }

        Vehicle vehicle = await _unitOfWork.Vehicles.FindOneAsync(p => p.VehicleID == request.VehicleID);
        if (vehicle == null)
        {
            // locking mechanism here
            // handle
        }

        // UPDATE
        evacuationStatus.TotalEvacuated += request.NumberOfEvacuee;
        evacuationStatus.RemainingPeople -= request.NumberOfEvacuee;
        evacuationStatus.LastVehicleUsed = vehicle.Id;

        _unitOfWork.BeginTransaction();
        _unitOfWork.EvacuationStatuses.Update(evacuationStatus);

        // add cache here
        List<EvacuationStatus> allEvacuationStatuses = await _unitOfWork.EvacuationStatuses.GetAllAsync("EvacuationZone");
        await _redisService.SetCacheAsync("EvacuationStatuses", allEvacuationStatuses);

        // LOG
        EvacuationZone evacuationZone = await _unitOfWork.EvacuationZones.FindOneAsync(p => p.Id == evacuationStatus.ZoneID);
        if (evacuationZone == null)
        {
            // handle
        }
        double distance = DistanceCalculator.CalculateDistance(
                request.Latitude, request.Longitude,
                evacuationZone.Latitude, evacuationZone.Longitude);

        Log log = new Log()
        {
            VehicleID = vehicle.Id,
            ETA = distance * (double)60 / (double)vehicle.Speed,
            IsEvacuationCompleted = evacuationStatus.RemainingPeople <= 0
        };

        await _unitOfWork.Logs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
        _unitOfWork.CommitTransaction();
    }

     public async Task DeleteAllDataAsync()
    {
        await _unitOfWork.Logs.DeleteAllAsync();
        await _unitOfWork.EvacuationStatuses.DeleteAllAsync();
        await _unitOfWork.EvacuationPlans.DeleteAllAsync();
        await _unitOfWork.Vehicles.DeleteAllAsync();
        await _unitOfWork.EvacuationZones.DeleteAllAsync();
        await _unitOfWork.SaveChangesAsync();
    }
}