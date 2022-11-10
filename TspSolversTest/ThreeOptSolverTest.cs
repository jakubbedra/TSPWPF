using TspShared;

namespace TspSolversTest;

public class ThreeOptSolverTest
{
    [Test]
    public void ThreeOptTest()
    {
        int[] a = { 2, 4, 0, 7, 1, 5, 6, 3, 8 };
        double[,] c =
        {
            { 36266.6667, 62550.0000 },
            { 34600.0000, 58633.3333 },
            { 51650.0000, 72300.0000 },
            { 37800.0000, 67683.3333 },
            { 35428.0556, 60174.1667 },
            { 34583.3333, 68550.0000 },
            { 27383.3333, 54766.6667 },
            { 34533.3333, 63166.6667 },
            { 23766.6667, 64683.3333 }
        };
        double initialD = TspUtils.TotalTourDistance(c, a);


        for (int i = 0; i < 100000; i++)
        {
            ThreeOptSolver solver = new ThreeOptSolver(c, a, a.Length);
            TspResults solve = solver.Solve(0);
            bool chuj = true;
            // checking if everything is there
            foreach (int j in a)
                if (!solve.Route.Contains(j))
                    chuj = false;

            double finalD = TspUtils.TotalTourDistance(c, a);

            Assert.That(chuj);
            Assert.That(finalD, Is.LessThanOrEqualTo(initialD));
            Console.WriteLine(finalD);
            a = solve.Route;
        }
    }
}