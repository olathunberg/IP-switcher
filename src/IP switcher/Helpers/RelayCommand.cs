using System.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;

namespace Deucalion.IP_Switcher
{
    public class RelayCommand : ICommand
    {
        #region Members
        readonly Func<Boolean> _canExecute;
        readonly Action _execute;
        private EventHandler _internalCanExecuteChanged;
        #endregion

        #region Constructors
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<Boolean> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region ICommand Members
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
        public bool CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(Object parameter)
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
            EventHandler eCanExecuteChanged = _internalCanExecuteChanged;
            if (eCanExecuteChanged != null)
                eCanExecuteChanged(this, EventArgs.Empty);
        }
        #endregion
    }
}
