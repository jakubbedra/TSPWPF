using TspShared;

namespace TspTasks;

public class TaskTspSolver : ITspSolver
{
    public async Task<List<int[]>> Phase1(double[,] cities, int citiesNo, int threadsNo, long millis)
    {
        int[] indices = new int[citiesNo];
        for (int i = 0; i < citiesNo; i++)
            indices[i] = i;
        PmxSolver solver = new PmxSolver(
            TspUtils.Randomize(indices), TspUtils.Randomize(indices),  citiesNo
        );
                
        return solver.Solve();
    }

    public async Task<int[]> Phase2(double[,] cities, int citiesNo, int threadsNo, long millis)
    {
        throw new NotImplementedException();
    }

    public int Cancel()
    {
        throw new NotImplementedException();
    }
}