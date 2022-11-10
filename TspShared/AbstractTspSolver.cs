using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TspShared;

public abstract class AbstractTspSolver : ITspSolver
{
  
    protected object stopLock = new object();
    protected bool _stopRequested;

    protected short _currentPhase;

    public bool StopRequested
    {
        get { return _stopRequested; }
        set
        {
            lock (stopLock) _stopRequested = value;
        }
    }

    protected object pauseLock = new object();
    protected bool _pauseRequested;

    public bool PauseRequested
    {
        get { return _pauseRequested; }
        set
        {
            lock (pauseLock) _pauseRequested = value;
        }
    }

    public ConcurrentDictionary<int, Tuple<TspResults, TspResults>> Phase1Results { get; set; }
    public ConcurrentDictionary<int, TspResults> Phase2Results { get; set; }

    protected int _tasksNo;

    public AbstractTspSolver(int tasksNo)
    {
        _currentPhase = 0;
        _tasksNo = tasksNo;
        StopRequested = false;
        PauseRequested = false;
        Phase2Results = new ConcurrentDictionary<int, TspResults>();
        Phase1Results = new ConcurrentDictionary<int, Tuple<TspResults, TspResults>>();
    }

    public TspResults Stop()
    {
        StopRequested = true;
        if (_currentPhase == 1)
        {
            while (Phase1Results.Count != _tasksNo) Thread.Sleep(1000);
        }
        else
        {
            while (Phase2Results.Count != _tasksNo) Thread.Sleep(1000);
        }

        return TakeAndClearBestResult();
    }

    public TspResults Pause()
    {
        PauseRequested = true;
        if (_currentPhase == 1)
        {
            while (Phase1Results.Count != _tasksNo) Thread.Sleep(1000);
        }
        else
        {
            while (Phase2Results.Count != _tasksNo) Thread.Sleep(1000);
        }

        return TakeAndClearBestResult();
    }

    public void Unpause()
    {
        PauseRequested = false;
    }

    protected TspResults TakeAndClearBestResult()
    {
        List<TspResults> resultsList = new List<TspResults>();
        if (_currentPhase == 1)
        {
            foreach (var keyValue in Phase1Results)
            {
                resultsList.Add(keyValue.Value.Item1);
                resultsList.Add(keyValue.Value.Item2);
            }
        }
        else
        {
            foreach (var (key, value) in Phase2Results)
            {
                resultsList.Add(value);
            }
        }

        TspResults results = resultsList.OrderBy(r => r.TotalDistance).First();
        Phase1Results.Clear();
        return results;
    }
}