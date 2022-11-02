namespace TspShared;

public class TspUtils
{
    private static Random _random = new Random();

    public static int[] Randomize(int[] indices)
    {
        return indices.OrderBy(x => _random.Next()).ToArray();
    }
    
}