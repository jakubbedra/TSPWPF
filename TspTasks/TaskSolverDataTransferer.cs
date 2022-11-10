using System.Collections.Concurrent;
using TspShared;

namespace TspTasks;

public class TaskSolverDataTransferer : SolverDataTransferer
{
    protected override void RunSolver()
    {
        Console.WriteLine("Starting calculations.");
        InputData data = _data;
        _solver = new TaskTspSolver(data.ParallelExecutionsCount);
        TaskTspSolver solver = _solver as TaskTspSolver;
        
        int[] indices = new int[data.CitiesCount];
        for (int i = 0; i < data.CitiesCount; i++)
            indices[i] = i;

        ConcurrentBag<TspResults> tmp = new ConcurrentBag<TspResults>();
        List<Task> tasks = new List<Task>();

        for (int i = 0; i < data.ParallelExecutionsCount; i++)
            tasks.Add(Task.Factory.StartNew(() =>
            {
                tmp.Add(new TspResults() { Route = TspUtils.Randomize(indices) });
            }));

        tasks.ForEach(t => t.Wait());
        List<TspResults> phase2Results = new List<TspResults>(tmp);

        int currentPhase = 0;
        int phaseCount = data.EpochsCount * 2;

        for (int i = 0; i < data.EpochsCount; i++)
        {
            Console.WriteLine($"Epoch: {i}; Phase 1 ");
            Task<List<TspResults>> phase1 =
                solver.Phase1(phase2Results, data.Cities, data.CitiesCount, data.Phase1Seconds * 1000);
            phase1.Wait();
            List<TspResults> tspResultsList = phase1.Result;
            TspResults phase1Results = tspResultsList.First();
            phase1Results.CurrentEpoch = i;
            phase1Results.CurrentPhase = 1;
            phase1Results.Progress = ++currentPhase * 100 / phaseCount;
            SendResults(_channel, tspResultsList.First());
            while (_solverPaused)
            {
            }

            if (_solverStopped) break;

            Console.WriteLine($"Epoch: {i}; Phase 2 ");
            Task<List<TspResults>> phase2 =
                solver.Phase2(tspResultsList, data.Cities, data.CitiesCount, data.Phase2Seconds * 1000);
            phase2.Wait();
            phase2Results = phase2.Result;
            TspResults finalResults = phase2Results.First();
            finalResults.CurrentEpoch = i;
            finalResults.CurrentPhase = 2;
            finalResults.Progress = ++currentPhase * 100 / phaseCount;
            SendResults(_channel, finalResults);
            while (_solverPaused)
            {
            }

            if (_solverStopped) break;
        }

        _solverStopped = true;
        Thread.Sleep(1000);
    }
}