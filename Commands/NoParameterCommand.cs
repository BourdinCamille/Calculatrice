using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculatrice.Commands
{
    public class NoParameterCommand : ICommand
    {
        public Action ExecuteDelegate;
        public Func<bool> CanExecuteDelegate;
        public event EventHandler? CanExecuteChanged;

        public NoParameterCommand(Action execute, Func<bool> canExecute)
        {
            ExecuteDelegate = execute;
            CanExecuteDelegate = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return CanExecuteDelegate();
        }

        public void Execute(object? parameter)
        {
            if (ExecuteDelegate != null)
            {
                ExecuteDelegate();
            }
        }
    }
}