using Models;
using Helpers;
using Exceptions;

namespace BusinessLogic;

public static class EvacuationPlanBusinessLogic
{
    public static Vehicle FindAppropriateVehicle(EvacuationZone evacuationZone, List<Vehicle> vehicles)
    {
        Vehicle vehicle = new Vehicle();
        vehicle = vehicles.LastOrDefault(f => f.Capacity >= evacuationZone.NumberOfPeople);
        if (vehicle == null)
        {
            vehicle = vehicles.FirstOrDefault(f => f.Capacity < evacuationZone.NumberOfPeople);
        }

        vehicles = vehicles.Where(w => w.Capacity == vehicle.Capacity).ToList();
        if (vehicles.Count > 0)
        {
            vehicle = FindClosestVehicle(evacuationZone, vehicles);
        }
        return vehicle;
    }

    public static Vehicle FindClosestVehicle(EvacuationZone evacuationZone, List<Vehicle> vehicles)
    {
        foreach (Vehicle vehicle in vehicles)
        {
            vehicle.Distance = DistanceCalculator.CalculateDistance(
                vehicle.Latitude, vehicle.Longitude,
                evacuationZone.Latitude, evacuationZone.Longitude);
        }

        return vehicles.Where(w => w.Distance <= 10).OrderBy(o => o.Distance).FirstOrDefault();
    }
}