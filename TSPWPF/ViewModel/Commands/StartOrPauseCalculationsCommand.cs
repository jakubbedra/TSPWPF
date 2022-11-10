namespace TSPWPF.ViewModel.Commands;

public class StartOrPauseCalculationsCommand : CommandBase
{
    public StartOrPauseCalculationsCommand(MainViewModel mainViewModel) : base(mainViewModel)
    {
    }

    public override void Execute(object? parameter)
    {
        if (!_mainViewModel.CalculationsStarted)
        {
            _mainViewModel.StartCalculations();
        }
        else
        {
            _mainViewModel.SwitchPauseCalculations();
        }
    }
}