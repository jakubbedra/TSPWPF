using System.Collections.Concurrent;
using TspShared;

namespace TspThreads;

public class ThreadSolverDataTransferer : SolverDataTransferer
{
    protected override void RunSolver()
    {
        Console.WriteLine("Starting calculations.");
        InputData data = _data;
        _solver = new ThreadTspSolver(data.ParallelExecutionsCount);
        ThreadTspSolver solver = _solver as ThreadTspSolver;

        int[] indices = new int[data.CitiesCount];

        for (int i = 0; i < data.CitiesCount; i++)
            indices[i] = i;

        ConcurrentBag<TspResults> tmp = new ConcurrentBag<TspResults>();
        List<Thread> threads = new List<Thread>();

        for (int i = 0; i < data.ParallelExecutionsCount; i++)
            threads.Add(new Thread(() =>
            {
                tmp.Add(new TspResults() { Route = TspUtils.Randomize(indices) });
            }));
        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());
        List<TspResults> phase2Results = new List<TspResults>(tmp);

        int currentPhase = 0;
        int phaseCount = data.EpochsCount * 2;

        for (int i = 0; i < data.EpochsCount; i++)
        {
            Console.WriteLine($"Epoch: {i}; Phase 1 ");
            List<TspResults> tspResultsList =
                solver.Phase1(phase2Results, data.Cities, data.CitiesCount, data.Phase1Seconds * 1000);
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
            phase2Results =
                solver.Phase2(tspResultsList, data.Cities, data.CitiesCount, data.Phase2Seconds * 1000);
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