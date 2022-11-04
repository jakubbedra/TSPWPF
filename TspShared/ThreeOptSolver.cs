namespace TspShared;

public class ThreeOptSolver
{
    private double[,] _cities;
    private int[] _currentBest;
    private int _n;

    public ThreeOptSolver(double[,] cities, int[] initial, int n)
    {
        _cities = cities;
        _currentBest = initial;
        _n = n;
    }

    public int[] Solve()
    {
        dupa();
        return _currentBest;
    }

    private int dupa()
    {
        double delta = 0.0;
        for (int i = 0; i < _n; i++)
        {
            //int i1 = i;
            int x1 = _currentBest[i];
            int x2 = _currentBest[(i + 1) % _n];

            for (int j = i + 2 /* +i? */; j < _n - 2; j++)
            {
                //int j1 = (i1 + j) % _n;
                int y1 = _currentBest[j];
                int y2 = _currentBest[(j + 1) % _n];

                for (int k = j + 2; k < _n; k++)
                {
                    //int k1 = (i1 + k) % _n;
                    int z1 = _currentBest[k];
                    int z2 = _currentBest[(k + 1) % _n];

                    delta += ReverseIfBetter(i, j, k);
                    Console.WriteLine(delta);
                }
            }
        }

        return 0;
    }

    private double ReverseIfBetter(int i, int j, int k)
    {
        int x1 = _currentBest[i];
        int x2 = _currentBest[(i + 1) % _n];

        int y1 = _currentBest[j];
        int y2 = _currentBest[(j + 1) % _n];

        int z1 = _currentBest[k];
        int z2 = _currentBest[(k + 1) % _n];

        var d = TspUtils.Distance;

        double d0 = d(_cities, x1, x2) + d(_cities, y1, y2) + d(_cities, z1, z2);
        double d1 = d(_cities, x1, y1) + d(_cities, x2, y2) + d(_cities, z1, z2);
        double d2 = d(_cities, x1, x2) + d(_cities, y1, z1) + d(_cities, y2, z2);
        double d3 = d(_cities, x1, y2) + d(_cities, z1, x2) + d(_cities, y1, z2);
        double d4 = d(_cities, z2, x2) + d(_cities, y1, y2) + d(_cities, z1, x1);

        if (d0 > d1)
        {
            Array.Reverse(_currentBest, i, j - i);
            return d1 - d0;
        }
        else if (d0 > d2)
        {
            Array.Reverse(_currentBest, j, k - j);
            return d2 - d0;
        }
        else if (d0 > d4)
        {
            Array.Reverse(_currentBest, i, k - i);
            return d4 - d0;
        }
        else if (d0 > d3)
        {
            List<int> tmp = new List<int>();
            for (int l = j; l < k; l++)
                tmp.Add(_currentBest[l]);
            for(int l=i; l<j; l++)
                tmp.Add(_currentBest[l]);
            for (var l = 0; l < tmp.Count; l++)
                _currentBest[l + i] = tmp[l];
            return d3 - d0;
        }

        return 0.0;
    }
}