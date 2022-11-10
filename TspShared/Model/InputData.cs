namespace TspShared;

public class InputData
{
    public double[,]? Cities { get; set; }
    public int CitiesCount { get; set; }
    public int Phase1Seconds { get; set; }
    public int Phase2Seconds { get; set; }
    public int ParallelExecutionsCount { get; set; }
    public int EpochsCount { get; set; }
}