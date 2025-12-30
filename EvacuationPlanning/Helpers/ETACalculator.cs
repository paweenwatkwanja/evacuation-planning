namespace Helpers;

public static class ETACalculator
{
    public static double CalculateETAInMinute(double distanceInKm, double speedInKmh)
    {
        return distanceInKm * (double)60 / (double)speedInKmh;
    }
}