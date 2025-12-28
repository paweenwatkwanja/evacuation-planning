namespace BusinessLogic;

public static class EvacuationPlanBusinessLogic
{
      public Vehicle FindAppropriateVehicle(int numberOfPeople, List<Vehicle> vehicles)
    {
        Vehicle vehicle = new Vehicle();
        vehicle = vehicles.LastOrDefault(f => f.Capacity >= numberOfPeople);
        if (vehicle == null)
        {
            vehicle = vehicles.FirstOrDefault(f => f.Capacity < numberOfPeople);
        }

        List<Vehicle> vehiclesWithSameCapacity = vehicles.Where(w => w.Capacity == vehicle.Capacity);
        if (vehiclesWithSameCapacity.Count > 0)
        {
            vehicle = findClosestVehicle(vehiclesWithSameCapacity);
        }

        return vehicle;
    }

    private static Vehicle findClosestVehicle(List<Vehicle> vehicles)
    {
        foreach (Vehicle vehicle in vehicles)
        {
            vehicle.Distance = DistanceCalculator.CalculateDistance(
                vehicle.Latitude, vehicle.Longitude,
                evacuationZone.Latitude, evacuationZone.Longitude);
            vehicleWithDistance[vehicle.VehicleID] = distance;
        }

        return vehicles.OrderBy(o => o.Distance).FirstOrDefault();
    }
}