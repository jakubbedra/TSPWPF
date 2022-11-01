namespace TSPWPF.ViewModel;

public class PathViewModel
{
    private readonly CityViewModel _cityA;
    private readonly CityViewModel _cityB;

    public double XA {get => _cityA.X;}
    public double YA {get => _cityA.Y;}
    public double XB {get => _cityB.X;}
    public double YB {get => _cityB.Y;}
    
    public PathViewModel(CityViewModel cityA, CityViewModel cityB)
    {
        _cityA = cityA;
        _cityB = cityB;
    }
}