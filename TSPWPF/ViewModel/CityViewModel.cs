using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TSPWPF.Model;

namespace TSPWPF.ViewModel;

public class CityViewModel : INotifyPropertyChanged
{
    //private readonly City _city;
//
    //private readonly double _offsetX;
    //private readonly double _offsetY;
    //private readonly double _scaleX;
    //private readonly double _scaleY;

    /**
     * Scaled down x and y
     */
    public double X { get; set; }

    public double Y { get; set; }

    public CityViewModel( /*City city, double offsetX, double offsetY, double scaleX, double scaleY*/ double x,
        double y)
    {
        //_city = city;
        //_offsetX = offsetX;
        //_offsetY = offsetY;
        //_scaleX = scaleX;
        //_scaleY = scaleY;
        X = x;
        Y = y;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}