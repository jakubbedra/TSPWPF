namespace TspShared;

public class PmxSolver
{
    private static Random _r = new Random();

    private int _citiesNo;
    private int[] _parentA;
    private int[] _parentB;

    public PmxSolver(int[] parentA, int[] parentB, int citiesNo)
    {
        _parentA = parentA;
        _parentB = parentB;
        _citiesNo = citiesNo;
    }

    public List<int[]> Solve()
    {
        int subsetSize = _citiesNo / 2;
        int[] newA = DoPmx(_parentA, _parentB, subsetSize);
        int[] newB = DoPmx(_parentB, _parentA, subsetSize);
        List<int[]> tmp = new List<int[]>();
        tmp.Add(newA);
        tmp.Add(newB);
        return tmp;
    }

    private int[] DoPmx(int[] a, int[] b, int n)
    {
        int[] child = new int[_citiesNo];
        for (int i = 0; i < _citiesNo; i++)
            child[i] = -1;

        int ind = _r.Next(_citiesNo - n);
        for (int i = ind; i < ind + n; i++)
            child[i] = a[i];

        for (int i = ind; i < ind + n; i++)
        {
            int ind2 = IndexOf(child, _citiesNo, b[i]);
            if (ind2 == -1)
            {
                child[NextFreeIndex(child, b, i)] = b[i];
            }
        }

        for (int i = 0; i < _citiesNo; i++)
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
            ind = IndexOf(parent, _citiesNo, child[ind]);
            return NextFreeIndex(child, parent, ind);
        }
    }

    private int IndexOf(int[] arr, int n, int a)
    {
        for (int i = 0; i < n; i++)
            if (arr[i] == a)
                return i;
        return -1;
    }

    private bool IsTaken(int i, int start, int end)
    {
        return i >= start && i < end;
    }
}