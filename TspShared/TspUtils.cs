namespace TspShared;

public class TspUtils
{
    private static Random _random = new Random();

    public static int[] Randomize(int[] indices)
    {
        return indices.OrderBy(x => _random.Next()).ToArray();
    }

    public static double Distance(double[,] cities, int a, int b)
    {
        double x = cities[a, 0] - cities[b, 0];
        double y = cities[a, 1] - cities[b, 1];
        return Math.Sqrt(x * x + y * y);
    }

    public static double TotalTourDistance(double[,] cities, int[] tour)
    {
        double ret = 0.0;
        for (int i = 0; i < tour.Length - 1; i++)
            ret += Distance(cities, tour[i], tour[i + 1]);
        ret += Distance(cities, tour[0], tour[tour.Length - 1]);
        return ret;
    }
}