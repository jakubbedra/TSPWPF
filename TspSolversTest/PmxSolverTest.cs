using TspShared;

namespace TspSolversTest;

public class PmxSolverTest
{
    [Test]
    public void PmxTest()
    {
        int[] a = { 3, 5, 1, 8, 2, 6, 7, 4, 9 };
        int[] b = { 1, 6, 2, 7, 4, 8, 5, 9, 3 };
        PmxSolver solver = new PmxSolver(a, b);

        List<int[]> solve = solver.Solve();
        bool chuj = true;
        foreach (int i in a)
            if (!solve[0].Contains(i))
                chuj = false;
        foreach (int i in a)
            if (!solve[1].Contains(i))
                chuj = false;
        
        Assert.That(chuj);
    }
}