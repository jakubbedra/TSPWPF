using System.Diagnostics;
using TspShared;

namespace TspThreads;

public class ThreadTspSolver : AbstractTspSolver
{
    public ThreadTspSolver(int tasksNo) : base(tasksNo)
    {
    }

    public List<TspResults> Phase1(List<TspResults> previousResults, double[,] cities, int citiesNo, long millis)
    {
        _currentPhase = 1;
        StopRequested = false;
        PauseRequested = false;
        Phase1Results.Clear();

        List<Thread> threads = new List<Thread>();
        int currentThreadId = 0;
        for (int i = 0; i < _tasksNo; i++)
        {
            Thread thread = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                int threadId = Interlocked.Increment(ref currentThreadId);
                sw.Start();
                TspResults tmp = previousResults[threadId - 1];
                PmxSolver solver = new PmxSolver(
                    cities, tmp.Route, TspUtils.Randomize(tmp.Route)
                );
                while (!StopRequested && sw.ElapsedMilliseconds < millis)
                {
                    while (!StopRequested && !PauseRequested && sw.ElapsedMilliseconds < millis)
                        solver.Solve();

                    sw.Stop();
                    Tuple<TspResults, TspResults> results = solver.GetResults(threadId);
                    if (!Phase1Results.TryAdd(threadId, results))
                        Phase1Results[threadId] = results;

                    while (PauseRequested && !StopRequested)
                        Thread.Sleep(1000);
                    sw.Start();
                }
            });
            thread.Start();
            threads.Add(thread);
        }
        
        threads.ForEach(t => t.Join());

        List<TspResults> resultsList = new List<TspResults>();
        foreach (var (key, value) in Phase1Results)
        {
            value.Item1.TotalDistance = TspUtils.TotalTourDistance(cities, value.Item1.Route);
            resultsList.Add(value.Item1);
            value.Item2.TotalDistance = TspUtils.TotalTourDistance(cities, value.Item2.Route);
            resultsList.Add(value.Item2);
        }

        return resultsList.OrderBy(r => r.TotalDistance).Take(_tasksNo).ToList();
    }

    public List<TspResults> Phase2(List<TspResults> previousResults, double[,] cities, int citiesNo, long millis)
    {
        _currentPhase = 2;
        Phase2Results.Clear();

        List<Thread> threads = new List<Thread>();
        int currentTaskId = 0;
        foreach (TspResults previous in previousResults)
        {
            Thread thread = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                int taskId = Interlocked.Increment(ref currentTaskId);
                int[] route = previous.Route;
                ThreeOptSolver solver = new ThreeOptSolver(
                    cities, route, route.Length
                );

                sw.Start();
                for (int i = 0; i < route.Length; i++)
                for (int j = i + 2; j < route.Length; j++)
                for (int k = j + 2; k < route.Length + (i > 0 ? 1 : 0); k++)
                {
                    solver.ReverseIfBetter(i, j, k);
                    if (_pauseRequested)
                    {
                        sw.Stop();
                        TspResults results = solver.GetResults(taskId);
                        if (!Phase2Results.TryAdd(taskId, results))
                            Phase2Results[taskId] = results;
                        while (_pauseRequested) Thread.Sleep(1000);
                        sw.Start();
                    }

                    if (_stopRequested || sw.ElapsedMilliseconds > millis)
                    {
                        sw.Stop();
                        TspResults results = solver.GetResults(taskId);
                        if (!Phase2Results.TryAdd(taskId, results))
                            Phase2Results[taskId] = results;
                        return;
                    }
                }

                TspResults result = solver.GetResults(taskId);
                if (!Phase2Results.TryAdd(taskId, result))
                    Phase2Results[taskId] = result;
                sw.Stop();
            });
            thread.Start();
            threads.Add(thread);
        }

        threads.ForEach(t => t.Join());
        
        while (Phase2Results.Count != _tasksNo)
        {
        }

        List<TspResults> resultsList = new List<TspResults>();
        foreach (var (key, value) in Phase2Results)
        {
            value.TotalDistance = TspUtils.TotalTourDistance(cities, value.Route);
            resultsList.Add(value);
        }

        return resultsList.OrderBy(r => r.TotalDistance).ToList();
    }
}