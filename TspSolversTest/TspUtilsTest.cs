using TspShared;

namespace TspSolversTest;

public class TspUtilsTest
{
    [Test]
    public void SampleTest()
    {
        double[,] c =
        {
            {36266.6667, 62550.0000},
            {34600.0000, 58633.3333},
            {51650.0000, 72300.0000},
            {37800.0000, 67683.3333},
            {35428.0556, 60174.1667},
            {34583.3333, 68550.0000},
            {27383.3333, 54766.6667},
            {34533.3333, 63166.6667},
            {23766.6667, 64683.3333}
        };
        int[] sample = {2,1,6,0,4,5,7,3,8};
        double d = TspUtils.TotalTourDistance(c, sample);
        
        Console.WriteLine("dupa");
    }
}