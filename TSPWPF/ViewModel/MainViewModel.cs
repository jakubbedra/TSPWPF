using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TspShared;
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
            OnPropertyChanged(nameof(FilePath));
        }
    }

    private double _canvasWidth;

    public double CanvasWidth
    {
        get { return _canvasWidth - 10; }
        set
        {
            _canvasWidth = value;
            OnPropertyChanged(nameof(CanvasWidth));
            RefreshCities();
        }
    }

    private double _canvasHeight { get; set; }

    public double CanvasHeight
    {
        get { return _canvasHeight - 10; }
        set
        {
            _canvasHeight = value;
            OnPropertyChanged(nameof(CanvasHeight));
            RefreshCities();
        }
    }

    private int _progress;

    public int Progress
    {
        get { return _progress; }
        set
        {
            _progress = value;
            OnPropertyChanged(nameof(Progress));
        }
    }

    private string _progressLabel;

    public string ProgressLabel
    {
        get { return _progressLabel; }
        set
        {
            _progressLabel = value;
            OnPropertyChanged(nameof(ProgressLabel));
        }
    }

    public List<City> Cities { get; set; }

    public ObservableCollection<CityViewModel> CitiesVM { get; set; }
    public ObservableCollection<PathViewModel> Paths { get; set; }
    public ObservableCollection<City> OrderedCities { get; set; }

    public ICommand LoadCitiesCommand { get; set; }
    public ICommand StartOrPauseCalculationsCommand { get; set; }
    public ICommand StopCalculationsCommand { get; set; }

    public InputData InputData { get; set; }

    private bool _calculationsStarted;

    public bool CalculationsStarted
    {
        get { return _calculationsStarted; }
        set
        {
            _calculationsStarted = value;
            OnPropertyChanged(nameof(CalculationsStarted));
        }
    }

    private bool _calculationsPaused;

    public bool CalculationsPaused
    {
        get { return _calculationsPaused; }
        set
        {
            _calculationsPaused = value;
            OnPropertyChanged(nameof(CalculationsPaused));
        }
    }

    private string _startButtonLabel;

    public string StartButtonLabel
    {
        get { return _startButtonLabel; }
        set
        {
            _startButtonLabel = value;
            OnPropertyChanged(nameof(StartButtonLabel));
        }
    }

    private string _optimalDistanceText;

    public string OptimalDistanceText
    {
        get { return _optimalDistanceText; }
        set
        {
            _optimalDistanceText = value;
            OnPropertyChanged(nameof(OptimalDistanceText));
        }
    }

    private RabbitDataSender _dataSender;

    private double _bestDistance;

    private MainWindow _window;

    public bool TasksChecked { get; set; }
    public bool ThreadsChecked { get; set; }

    public MainViewModel(MainWindow window)
    {
        TasksChecked = false;
        ThreadsChecked = false;
        OptimalDistanceText = "No optimal distance found yet.";
        _window = window;
        Progress = 0;
        ProgressLabel = "";
        StartButtonLabel = "Start";
        CalculationsStarted = false;
        CalculationsPaused = false;
        InputData = new InputData();
        _filePath = "SampleText";
        CitiesVM = new ObservableCollection<CityViewModel>();
        Paths = new ObservableCollection<PathViewModel>();
        OrderedCities = new ObservableCollection<City>();
        Cities = new List<City>();
        LoadCitiesCommand = new LoadFileCommand(this);
        StartOrPauseCalculationsCommand = new StartOrPauseCalculationsCommand(this);
        StopCalculationsCommand = new StopCalculationsCommand(this);
        _dataSender = new RabbitDataSender(this);
    }

    public void LoadCityList()
    {
        OptimalDistanceText = "No optimal distance found yet.";
        Cities.Clear();
        CitiesVM.Clear();
        Paths.Clear();
        OrderedCities.Clear();
        Cities = TspFileLoader.CreateCitiesListFromFile(_filePath);

        double scaleX = CalculateScaleX();
        double scaleY = CalculateScaleY();
        double offsetX = CalculateOffsetX();
        double offsetY = CalculateOffsetY();

        foreach (City city in Cities)
        {
            OrderedCities.Add(city);
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

    public void UpdateResults(TspResults results)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            if (results.TotalDistance < _bestDistance)
            {
                _bestDistance = results.TotalDistance;
                UpdateCities(results.Route);
                OptimalDistanceText =
                    $"Optimal distance: {string.Format("{0:0.000}", results.TotalDistance)} found by ({results.ParallelId}).";
            }

            if (results.Progress != 0)
            {
              Progress = results.Progress;
              ProgressLabel = $"Epoch: ({results.CurrentEpoch}) Phase: ({results.CurrentPhase})";
            }

            if (AreResultsFinal(results)) ResetGui();
        });
    }

    private bool AreResultsFinal(TspResults results)
    {
        return results.Progress == 100;
    }

    private void ResetGui()
    {
        _window.SwitchCloseButton(true);
        _calculationsPaused = false;
        _calculationsStarted = false;
        StartButtonLabel = "Start";
        ProgressLabel = "Finished.";
    }

    private void UpdateCities(int[] tour)
    {
        Paths.Clear();
        OrderedCities.Clear();
        for (var i = 1; i < CitiesVM.Count; i++)
        {
            OrderedCities.Add(Cities[tour[i - 1]]);
            Paths.Add(new PathViewModel(CitiesVM[tour[i - 1]], CitiesVM[tour[i]]));
        }

        OrderedCities.Add(Cities[tour[Cities.Count - 1]]);
        Paths.Add(new PathViewModel(CitiesVM[tour[0]], CitiesVM[tour[CitiesVM.Count - 1]]));
    }

    public void StartCalculations()
    {
        if (TasksChecked)
            Process.Start("..\\net6.0\\TspTasks.exe");
        else if (ThreadsChecked)
            Process.Start("..\\net6.0\\TspThreads.exe");
        else
            return;

        _window.SwitchCloseButton(false);
        Progress = 0;
        InputData.CitiesCount = Cities.Count;
        double[,] tmp = new double[Cities.Count, 2];
        for (int i = 0; i < Cities.Count; i++)
        {
            tmp[i, 0] = Cities[i].X;
            tmp[i, 1] = Cities[i].Y;
        }

        InputData.Cities = tmp;

        int[] tour = new int[CitiesVM.Count];
        for (int i = 0; i < tour.Length; i++)
            tour[i] = i;
        _bestDistance = TspUtils.TotalTourDistance(tmp, tour);
    }

    public void ChildProcessStarted()
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            _dataSender.SendStart(InputData);
            CalculationsStarted = true;
            StartButtonLabel = "Pause";
        });
    }

    // Pause or unpause
    public void SwitchPauseCalculations()
    {
        CalculationsPaused = !CalculationsPaused;
        _dataSender.SendSwitchPause(CalculationsPaused);
        StartButtonLabel = CalculationsPaused ? "Unpause" : "Pause";
    }

    // Stop Calculations
    public void StopCalculations()
    {
        _dataSender.SendEnd();
        ResetGui();
    }

    private void RefreshCities()
    {
        if (Cities.Count == 0) return;
        CitiesVM.Clear();
        Paths.Clear();
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