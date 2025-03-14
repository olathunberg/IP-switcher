using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TTech.IP_Switcher;

public class RelayCommand : ICommand
{
    readonly Func<bool> _canExecute;
    readonly Action _execute;
    private EventHandler _internalCanExecuteChanged;

    public RelayCommand(Action execute)
        : this(execute, null)
    {
    }

    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add
        {
            if (_canExecute != null)
            {
                _internalCanExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }
        }
        remove
        {

            if (_canExecute != null)
            {
                _internalCanExecuteChanged += value;
                CommandManager.RequerySuggested -= value;
            }
        }
    }

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public void Execute(object parameter)
    {
        _execute();
    }

    /// <summary>
    /// This method can be used to raise the CanExecuteChanged handler.
    /// This will force WPF to re-query the status of this command directly.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        if (_canExecute != null)
            OnCanExecuteChanged();
    }

    /// <summary>
    /// This method is used to walk the delegate chain and well WPF that
    /// our command execution status has changed.
    /// </summary>
    protected virtual void OnCanExecuteChanged()
    {
        _internalCanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
