using System;
using System.Windows.Input;

namespace TSPWPF.ViewModel.Commands;

public class StopCalculationsCommand : CommandBase
{
    public StopCalculationsCommand(MainViewModel mainViewModel) : base(mainViewModel)
    {
    }

    public override bool CanExecute(object? parameter)
    {
        return _mainViewModel.CalculationsStarted;
    }
    
    public override void Execute(object? parameter)
    {
        _mainViewModel.StopCalculations();
    }
}