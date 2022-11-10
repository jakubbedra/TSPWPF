using System;
using System.Collections.Generic;

namespace TspShared;

public class ThreeOptSolver
{
    private double[,] _cities;
    private int[] _currentBest;
    private int _n;

    private object pauseLock = new object();
    private bool _paused;

    public bool Paused
    {
        get { return _paused; }
        set
        {
            lock (pauseLock) _paused = value;
        }
    }

    private object stopLock = new object();
    private bool _stopped;

    public bool Stopped
    {
        get { return _stopped; }
        set
        {
            lock (stopLock) _stopped = value;
        }
    }


    public ThreeOptSolver(double[,] cities, int[] initial, int n)
    {
        Stopped = false;
        Paused = false;
        _cities = cities;
        _currentBest = initial;
        _n = n;
    }

    public TspResults Solve(int parallelId)
    {
        for (int i = 0; i < _n; i++)
        for (int j = i + 2; j < _n; j++)
        for (int k = j + 2; k < _n + (i > 0 ? 1 : 0); k++)
        {
            ReverseIfBetter(i, j, k);
        }

        return GetResults(parallelId);
    }

    private static readonly Object lockObj = new Object();

    public TspResults GetResults(int parallelId)
    {
        lock (lockObj)
        {
            return new TspResults()
            {
                ParallelId = parallelId,
                Route = _currentBest,
                TotalDistance = TspUtils.TotalTourDistance(_cities, _currentBest)
            };
        }
    }

    public void ReverseIfBetter(int i, int j, int k)
    {
        int x1 = i > 0 ? _currentBest[i - 1] : _currentBest[_n - 1];
        int x2 = _currentBest[i];
        int y1 = _currentBest[j - 1];
        int y2 = _currentBest[j];
        int z1 = _currentBest[k - 1];
        int z2 = _currentBest[k % _n];

        var d = TspUtils.Distance;

        double d0 = d(_cities, x1, x2) + d(_cities, y1, y2) + d(_cities, z1, z2);

        double d1 = d(_cities, x1, y1) + d(_cities, x2, y2) + d(_cities, z1, z2);
        double d2 = d(_cities, x1, x2) + d(_cities, y1, z1) + d(_cities, y2, z2);
        double d3 = d(_cities, x1, y2) + d(_cities, z1, x2) + d(_cities, y1, z2);
        double d4 = d(_cities, z2, x2) + d(_cities, y1, y2) + d(_cities, z1, x1);

        if (d0 > d1)
        {
            Array.Reverse(_currentBest, i, j - i);
        }
        else if (d0 > d2)
        {
            Array.Reverse(_currentBest, j, k - j);
        }
        else if (d0 > d4)
        {
            Array.Reverse(_currentBest, i, k - i);
        }
        else if (d0 > d3)
        {
            List<int> tmp = new List<int>();
            for (int l = j; l < k; l++)
                tmp.Add(_currentBest[l]);
            for (int l = i; l < j; l++)
                tmp.Add(_currentBest[l]);
            for (var l = 0; l < tmp.Count; l++)
                _currentBest[l + i] = tmp[l];
        }
    }
}