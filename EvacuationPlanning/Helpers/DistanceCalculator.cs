namespace Helpers;

public static class DistanceCalculator
{
    public static double CalculateDistance(double startLatitude, double startLongitude, double destLatitude, double destLongitude)
    {
        double distanceLatitude = (Math.PI / 180) * (destLatitude - startLatitude);
        double distanceLongitude = (Math.PI / 180) * (destLongitude - startLongitude);

        startLatitude = (Math.PI / 180) * (startLatitude);
        destLatitude = (Math.PI / 180) * (destLatitude);

        double a = Math.Pow(Math.Sin(distanceLatitude / 2), 2) +
                   Math.Pow(Math.Sin(distanceLongitude / 2), 2) *
                   Math.Cos(startLatitude) * Math.Cos(destLatitude);
        double radius = 6371;
        double c = 2 * Math.Asin(Math.Sqrt(a));
        
        return radius * c;
    }
}

