using TspShared;
using TspThreads;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting thread solver.");
        SolverDataTransferer solver = new ThreadSolverDataTransferer();
        solver.Run();
    }
}