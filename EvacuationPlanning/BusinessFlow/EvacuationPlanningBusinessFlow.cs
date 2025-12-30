using BusinessLogic;
using Models;
using Repository;
using System.Linq.Expressions;
using Helpers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Services;
using Exceptions;
using StackExchange.Redis;
using System.Text.Json;

namespace BusinessFlow;

public class EvacuationPlanningBusinessFlow
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly RedisService _redisService;
    private readonly ILogger<EvacuationPlanningBusinessFlow> _logger;

    public EvacuationPlanningBusinessFlow(IUnitOfWork unitOfWork, RedisService redisService, ILogger<EvacuationPlanningBusinessFlow> logger)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
        _logger = logger;
    }

    // EVACUTION ZONE
    public async Task<List<EvacuationZoneResponse>> ProcessEvacuationZonesAsync(List<EvacuationZoneRequest> requests)
    {
        List<EvacuationZone> evacuationZones = convertRequestsToEvacuationZonesAndValidate(requests);

        await _unitOfWork.EvacuationZones.AddRangeAsync(evacuationZones);
        await _unitOfWork.SaveChangesAsync();

        List<EvacuationZoneResponse> responses = convertEvacuationZonesToResponses(evacuationZones);

        return responses;
    }

    private List<EvacuationZone> convertRequestsToEvacuationZonesAndValidate(List<EvacuationZoneRequest> requests)
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
        return evacuationZones;
    }

    private List<EvacuationZoneResponse> convertEvacuationZonesToResponses(List<EvacuationZone> evacuationZones)
    {
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
        List<Vehicle> vehicles = convertRequestsToVehiclesAndValidate(requests);

        await _unitOfWork.Vehicles.AddRangeAsync(vehicles);
        await _unitOfWork.SaveChangesAsync();

        List<VehicleResponse> responses = convertVehiclesToResponses(vehicles);

        return responses;
    }

    private List<Vehicle> convertRequestsToVehiclesAndValidate(List<VehicleRequest> requests)
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
                Speed = request.Speed,
                IsAvailable = true
            };
            vehicles.Add(vehicle);
        }
        return vehicles;
    }

    private List<VehicleResponse> convertVehiclesToResponses(List<Vehicle> vehicles)
    {
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
        List<EvacuationPlan> evacuationPlans = new List<EvacuationPlan>();

        List<EvacuationZone> evacuationZones = await getEvacuationZonesAsync();
        if (evacuationZones.Count <= 0)
        {
            _logger.LogInformation("No evacuation zones available to process evacuation plans.");
            return evacuationPlans;
        }

        List<Vehicle> vehicles = await getAvailableVehiclesAsync();
        if (vehicles.Count <= 0)
        {
            _logger.LogInformation("No vehicles available to process evacuation plans.");
            return evacuationPlans;
        }

        evacuationPlans = prepareDataAndCreateEvacuationPlans(evacuationZones, vehicles);
        await addEvacuationPlanAsync(evacuationPlans);

        List<long> vechicleIds = evacuationPlans.Select(ep => ep.Vehicle.Id).ToList();
        await updateVehicleAvailabilitAsync(vechicleIds, false);

        List<EvacuationStatus> evacuationStatuses = convertEvacuationPlansToEvacuationStatuses(evacuationPlans);
        await addAndCacheEvacuationStatusesAsync(evacuationStatuses);

        List<Log> logs = convertEvacuationPlansToLogs(evacuationPlans);
        await addLogsAsync(logs);

        return evacuationPlans.ToList();
    }

    private async Task<List<EvacuationZone>> getEvacuationZonesAsync()
    {
        return await _unitOfWork.EvacuationZones.GetAllAsync();
    }

    private async Task<List<Vehicle>> getAvailableVehiclesAsync()
    {
        return await _unitOfWork.Vehicles.FindAsync(p => p.IsAvailable);
    }

    private List<EvacuationPlan> prepareDataAndCreateEvacuationPlans(List<EvacuationZone> evacuationZones, List<Vehicle> vehicles)
    {
        List<EvacuationZone> sortedEvacuationZones = evacuationZones.OrderByDescending(o => o.UrgencyLevel).ToList();
        List<Vehicle> sortedVehicles = vehicles.OrderByDescending(o => o.Capacity).ToList();

        List<EvacuationPlan> evacuationPlans = new List<EvacuationPlan>();
        foreach (EvacuationZone evacuationZone in sortedEvacuationZones)
        {
            Vehicle vehicle = EvacuationPlanBusinessLogic.FindAppropriateVehicle(evacuationZone, sortedVehicles);
            if (vehicle == null)
            {
                _logger.LogInformation("No available vehicles within a reasonable distance for Evacuation Zone ID: " + evacuationZone.Id);
                continue;
            }

            int remainingPeople = evacuationZone.NumberOfPeople - vehicle.Capacity;
            EvacuationPlan evacuationPlan = new EvacuationPlan()
            {
                ZoneID = evacuationZone.Id,
                VehicleID = vehicle.Id,
                NumberOfPeople = (remainingPeople <= 0) ? evacuationZone.NumberOfPeople : vehicle.Capacity,
                ETA = ETACalculator.CalculateETAInMinute(vehicle.Distance, vehicle.Speed),
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

        HashEntry[] evacuationStatusHashSet = convertEvacuationStatusesToHashSet(evacuationStatuses);
        await _redisService.SetHastSetCacheAsync("EvacuationStatuses", evacuationStatusHashSet);
    }

    private HashEntry[] convertEvacuationStatusesToHashSet(List<EvacuationStatus> evacuationStatuses)
    {
        return evacuationStatuses.Select(status =>
           new HashEntry(
               status.EvacuationZone.ZoneID,
               JsonSerializer.Serialize(status)))
               .ToArray();
    }

    private async Task updateVehicleAvailabilitAsync(List<long> vehicleIds, bool isAvailable)
    {
        foreach (long vehicleId in vehicleIds)
        {
            Vehicle vehicle = await _unitOfWork.Vehicles.FindOneAsync(p => p.Id == vehicleId);
            if (vehicle != null)
            {
                vehicle.IsAvailable = isAvailable;
                _unitOfWork.Vehicles.Update(vehicle);
            }
        }
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

    // EVACUATION STATUS
    public async Task<List<EvacuationStatus>> GetEvacuationStatusesAsync()
    {
        List<EvacuationStatus> cachedEvacuationStatuses = await _redisService.GetHashSetCacheAsync<EvacuationStatus>("EvacuationStatuses");
        if (cachedEvacuationStatuses != null)
        {
            _logger.LogInformation("Evacuation statuses retrieved from cache.");
            return cachedEvacuationStatuses;
        }

        List<EvacuationStatus> evacuationStatuses = await _unitOfWork.EvacuationStatuses.GetAllAsync("EvacuationZone", "Vehicle");

        HashEntry[] evacuationStatusHashSet = convertEvacuationStatusesToHashSet(evacuationStatuses);
        await _redisService.SetHastSetCacheAsync("EvacuationStatuses", evacuationStatusHashSet);
        _logger.LogInformation("Evacuation statuses retrieved from database.");

        return evacuationStatuses;
    }

    // UPDATE EVACUATION STATUS
    public async Task UpdateEvacuationStatusAndRelatedDataAsync(int id, EvacuationStatusUpdateRequest request)
    {
        EvacuationStatusBusinessLogic.ValidateEvacuationStatusRequest(request);
        EvacuationStatus evacuationStatus = await getEvacuationStatusByIDAsync(id);
        if (evacuationStatus == null)
        {
            throw new NotFoundException($"EvacuationStatus with ID {id} not found.");
        }

        Vehicle vehicle = await getVehicleByVehicleIDAsync(request.VehicleID);
        if (vehicle == null)
        {
            throw new NotFoundException($"Vehicle with ID {request.VehicleID} not found.");
        }

        updateEvacuationStatus(request, evacuationStatus, vehicle);

        bool isEvacuationCompleted = evacuationStatus.RemainingPeople <= 0;
        updateVehicleAfterEvacuationStatusUpdate(vehicle, request, isEvacuationCompleted);

        await prepareLogDataAndAddLogAsync(evacuationStatus.ZoneID, request, vehicle, isEvacuationCompleted);
        await _unitOfWork.SaveChangesAsync();

        await updateCachedEvacuationStatusesAsync(evacuationStatus);
    }

    private async Task<EvacuationStatus> getEvacuationStatusByIDAsync(int id)
    {
        return await _unitOfWork.EvacuationStatuses.FindOneAsync(p => p.Id == id, null, "EvacuationZone");
    }

    private async Task<Vehicle> getVehicleByVehicleIDAsync(string vehicleID)
    {
        return await _unitOfWork.Vehicles.FindOneAsync(p => p.VehicleID == vehicleID);
    }

    private void updateEvacuationStatus(EvacuationStatusUpdateRequest request, EvacuationStatus evacuationStatus, Vehicle vehicle)
    {
        evacuationStatus.TotalEvacuated += request.NumberOfEvacuee;
        evacuationStatus.RemainingPeople -= request.NumberOfEvacuee;
        evacuationStatus.LastVehicleUsed = vehicle.Id;

        _unitOfWork.EvacuationStatuses.Update(evacuationStatus);
    }

    private async Task updateCachedEvacuationStatusesAsync(EvacuationStatus evacuationStatus)
    {
        HashEntry[] evacuationStatusHashSet = new HashEntry[]
        {
            new HashEntry(
                evacuationStatus.EvacuationZone.ZoneID,
                JsonSerializer.Serialize(evacuationStatus))
        };
        await _redisService.UpdateHashSetCacheAsync("EvacuationStatuses", evacuationStatusHashSet);
    }

    private async Task prepareLogDataAndAddLogAsync(long ZoneID, EvacuationStatusUpdateRequest request, Vehicle vehicle, bool isEvacuationCompleted)
    {
        EvacuationZone evacuationZone = await _unitOfWork.EvacuationZones.FindOneAsync(p => p.Id == ZoneID);
        if (evacuationZone == null)
        {
            throw new NotFoundException($"EvacuationZone with ID {ZoneID} not found.");
        }

        double distance = DistanceCalculator.CalculateDistance(
                request.Latitude, request.Longitude,
                evacuationZone.Latitude, evacuationZone.Longitude);

        List<Log> logs = new List<Log>() {  new Log()
        {
            VehicleID = vehicle.Id,
            ETA = ETACalculator.CalculateETAInMinute(distance, vehicle.Speed),
            IsEvacuationCompleted = isEvacuationCompleted
        }};

        await addLogsAsync(logs);
    }

    private void updateVehicleAfterEvacuationStatusUpdate(Vehicle vehicle, EvacuationStatusUpdateRequest request, bool isEvacuationCompleted)
    {
        if (isEvacuationCompleted)
        {
            vehicle.IsAvailable = true;
        }
        vehicle.Latitude = request.Latitude;
        vehicle.Longitude = request.Longitude;

        _unitOfWork.Vehicles.Update(vehicle);
    }

    // DELETE ALL DATA
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