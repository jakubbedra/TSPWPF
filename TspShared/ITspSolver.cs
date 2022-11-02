namespace TspShared;

public interface ITspSolver
{
    /**
     * Calculates (sub) optimal TSP route with PmxSolver
     */
    public Task<List<int[]>> Phase1(double[,] cities, int citiesNo, int threadsNo, long millis);

    /**
     * Calculates (sub) optimal TSP route with ThreeOptSolver
     */
    public Task<int[]>  Phase2(double[,] cities, int citiesNo, int threadsNo, long millis);

    /**
     * Cancels the calculations
     */
    public int Cancel();
}