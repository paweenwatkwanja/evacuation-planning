using Models;
using Helpers;

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
            vehicle = findClosestVehicle(evacuationZone, vehicles);
        }
        return vehicle;
    }

    private static Vehicle findClosestVehicle(EvacuationZone evacuationZone, List<Vehicle> vehicles)
    {
        foreach (Vehicle vehicle in vehicles)
        {
            vehicle.Distance = DistanceCalculator.CalculateDistance(
                vehicle.Latitude, vehicle.Longitude,
                evacuationZone.Latitude, evacuationZone.Longitude);
        }

        return vehicles.OrderBy(o => o.Distance).FirstOrDefault() ?? new Vehicle();
    }
}