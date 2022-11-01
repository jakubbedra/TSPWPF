using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TSPWPF.Model;
using TSPWPF.ViewModel.Commands;
using TSPWPF.ViewModel.Helper;

namespace TSPWPF.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    private string _filePath;

    public string FilePath
    {
        get { return _filePath; }
        set
        {
            _filePath = value;
            OnPropertyChanged();
        }
    }

    public double CanvasWidth { get; set; }
    public double CanvasHeight { get; set; }

    public List<City> Cities { get; set; }

    public ObservableCollection<CityViewModel> CitiesVM { get; set; }
    public ObservableCollection<PathViewModel> Paths { get; set; }

    public int ProcessOrTaskCount { get; set; }

    public int Phase1Seconds { get; set; }
    public int Phase2Seconds { get; set; }

    public ICommand LoadCitiesCommand { get; set; }

    public MainViewModel()
    {
        _filePath = "SampleText";
        CitiesVM = new ObservableCollection<CityViewModel>();
        Paths = new ObservableCollection<PathViewModel>();
        Cities = new List<City>();
        LoadCitiesCommand = new LoadFileCommand(this);
        CanvasHeight = 660;
        CanvasWidth = 720;
    }

    public void LoadCityList()
    {
        Cities.Clear();
        CitiesVM.Clear();
        Cities = TspFileLoader.CreateCitiesListFromFile(_filePath);

        double scaleX = CalculateScaleX();
        double scaleY = CalculateScaleY();
        double offsetX = CalculateOffsetX();
        double offsetY = CalculateOffsetY();

        foreach (City city in Cities)
        {
            CitiesVM.Add(
                new CityViewModel((city.X - offsetX) * scaleX + 5, (city.Y - offsetY) * scaleY + 5)
            );
        }

        for (var i = 1; i < CitiesVM.Count; i++)
        {
            Paths.Add(new PathViewModel(CitiesVM[i - 1], CitiesVM[i]));
        }

        Paths.Add(new PathViewModel(CitiesVM[0], CitiesVM[CitiesVM.Count - 1]));
    }

    private double CalculateScaleX()
    {
        double min = CalculateOffsetX();
        double max = Cities[0].X;
        foreach (City city in Cities)
        {
            if (city.X > max)
            {
                max = city.X;
            }
        }

        // (max - min) * x = canvasWidth
        // (max - min) = canvasWidth / x 
        // (max - min)/canvasWidth = 1 / x
        //canvasWidth / (max-min) = x 
        return CanvasWidth / (max - min);
    }

    private double CalculateScaleY()
    {
        double min = CalculateOffsetY();
        double max = Cities[0].Y;
        foreach (City city in Cities)
        {
            if (city.Y > max)
            {
                max = city.Y;
            }
        }

        return CanvasHeight / (max - min);
    }

    private double CalculateOffsetX()
    {
        double min = Cities[0].X;
        foreach (City city in Cities)
        {
            if (city.X < min)
            {
                min = city.X;
            }
        }

        return min;
    }

    private double CalculateOffsetY()
    {
        double min = Cities[0].Y;
        foreach (City city in Cities)
        {
            if (city.Y < min)
            {
                min = city.Y;
            }
        }

        return min;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}