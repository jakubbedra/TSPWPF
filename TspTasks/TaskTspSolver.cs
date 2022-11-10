using System.Diagnostics;
using TspShared;

namespace TspTasks;

public class TaskTspSolver : AbstractTspSolver
{
    public TaskTspSolver(int tasksNo) : base(tasksNo)
    { }

    public async Task<List<TspResults>> Phase1(List<TspResults> previousResults, double[,] cities, int citiesNo,
        long millis)
    {
        _currentPhase = 1;
        StopRequested = false;
        PauseRequested = false;
        Phase1Results.Clear();

        List<Task> tasks = new List<Task>();
        int currentTaskId = 0;
        for (int i = 0; i < _tasksNo; i++)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Stopwatch sw = new Stopwatch();
                int taskId = Interlocked.Increment(ref currentTaskId);
                sw.Start();
                TspResults tmp = previousResults[taskId - 1];
                PmxSolver solver = new PmxSolver(
                    cities, tmp.Route, TspUtils.Randomize(tmp.Route)
                );
                while (!StopRequested && sw.ElapsedMilliseconds < millis)
                {
                    while (!StopRequested && !PauseRequested && sw.ElapsedMilliseconds < millis)
                        solver.Solve();

                    sw.Stop();
                    Tuple<TspResults, TspResults> results = solver.GetResults(taskId);
                    if (!Phase1Results.TryAdd(taskId, results))
                        Phase1Results[taskId] = results;

                    while (PauseRequested && !StopRequested)
                        Thread.Sleep(1000);
                    sw.Start();
                }
            }, TaskCreationOptions.LongRunning);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

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

    public async Task<List<TspResults>> Phase2(List<TspResults> previousResults, double[,] cities, int citiesNo,
        long millis)
    {
        _currentPhase = 2;
        Phase2Results.Clear();

        List<Task> tasks = new List<Task>();
        int currentTaskId = 0;
        foreach (TspResults previous in previousResults)
        {
            Task task = Task.Factory.StartNew(() =>
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
            }, TaskCreationOptions.LongRunning);
            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray());
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