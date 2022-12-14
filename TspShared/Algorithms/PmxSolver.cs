using System;
using System.Collections.Generic;

namespace TspShared;

public class PmxSolver
{
    private static Random _r = new Random();

    private double[,] _cities;
    private int[] _parentA;
    private int[] _parentB;

    public PmxSolver(double[,] cities, int[] parentA, int[] parentB)
    {
        _cities = cities;
        _parentA = parentA;
        _parentB = parentB;
    }

    public List<int[]> Solve()
    {
        int subsetSize = _parentA.Length / 2;
        int[] newA = DoPmx(_parentA, _parentB, subsetSize);
        int[] newB = DoPmx(_parentB, _parentA, subsetSize);
        List<int[]> tmp = new List<int[]>();
        tmp.Add(newA);
        tmp.Add(newB);
        _parentA = newA;
        _parentB = newB;
        return tmp;
    }
    
    public Tuple<TspResults, TspResults> GetResults(int parallelId) =>
        new Tuple<TspResults, TspResults>(
            new TspResults()
            {
                ParallelId = parallelId,
                Route = _parentA,
                TotalDistance = TspUtils.TotalTourDistance(_cities, _parentA)
            },
            new TspResults()
            {
                ParallelId = parallelId,
                Route = _parentB,
                TotalDistance = TspUtils.TotalTourDistance(_cities, _parentB)
            }
        );

    private int[] DoPmx(int[] a, int[] b, int n)
    {
        int[] child = new int[a.Length];
        for (int i = 0; i < child.Length; i++)
            child[i] = -1;

        int ind = _r.Next(a.Length - n);
        for (int i = ind; i < ind + n; i++)
            child[i] = a[i];

        for (int i = ind; i < ind + n; i++)
        {
            int ind2 = IndexOf(child, b[i]);
            if (ind2 == -1)
            {
                child[NextFreeIndex(child, b, i)] = b[i];
            }
        }

        for (int i = 0; i < child.Length; i++)
            if (child[i] == -1)
                child[i] = b[i];

        return child;
    }

    private int NextFreeIndex(int[] child, int[] parent, int ind)
    {
        if (child[ind] == -1)
        {
            return ind;
        }
        else
        {
            ind = IndexOf(parent, child[ind]);
            return NextFreeIndex(child, parent, ind);
        }
    }

    private int IndexOf(int[] arr, int a)
    {
        for (int i = 0; i < arr.Length; i++)
            if (arr[i] == a)
                return i;
        return -1;
    }
}