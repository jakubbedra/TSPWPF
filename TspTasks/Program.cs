using TspShared;
using TspTasks;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting task solver.");
        SolverDataTransferer solver = new TaskSolverDataTransferer();
        solver.Run();
    }
}