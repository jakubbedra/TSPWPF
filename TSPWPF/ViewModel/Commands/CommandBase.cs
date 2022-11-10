using System;
using System.Windows.Input;

namespace TSPWPF.ViewModel.Commands;

public abstract class CommandBase : ICommand
{
    protected readonly MainViewModel _mainViewModel;

    public CommandBase(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    event EventHandler ICommand.CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
    
    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }
    
    public abstract void Execute(object? parameter);

    public event EventHandler? CanExecuteChanged;
}