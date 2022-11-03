namespace TspShared;

public class ThreeOptSolver
{
    private double[][] _cities;
    private int[] _currentBest;
    private int _n;

    public ThreeOptSolver(double[][] cities, int[] initial, int n)
    {
        _cities = cities;
        _currentBest = initial;
        _n = n;
    }

    public int[] Solve()
    {
        return _currentBest;
    }

    private int dupa()
    {
        for (int i = 0; i < _n; i++)
        {
            for (int j = 0; j < _n; j++)
            {
                for (int k = 0; k < _n; k++)
                {
                    
                }
            }
        }
    }
}