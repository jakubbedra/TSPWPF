namespace TspShared;

public class TspResults
{
    public int ParallelId { get; set; }
    public int[] Route { get; set; }
    public double TotalDistance { get; set; }
    public int Progress { get; set; }
    public int CurrentPhase { get; set; }
    public int CurrentEpoch { get; set; }
}