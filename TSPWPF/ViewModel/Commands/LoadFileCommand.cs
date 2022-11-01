namespace TSPWPF.ViewModel.Commands;

public class LoadFileCommand : CommandBase
{
    public LoadFileCommand(MainViewModel mainViewModel) : base(mainViewModel)
    {
    }

    public override void Execute(object? parameter)
    {
        _mainViewModel.LoadCityList();
    }
}