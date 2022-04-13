using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculatrice.Commands
{
    public class CommandBase<T> : ICommand
    {
        public Action<T> ExecuterMethode;

        public Func<T,bool> CanExecuterMethode;

        public CommandBase(Action<T> executerMethode, Func<T,bool> canExecuterMethode)
        {
            ExecuterMethode = executerMethode;
            CanExecuterMethode = canExecuterMethode;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ExecuterMethode((T)parameter);
        }

        public event EventHandler? CanExecuteChanged;
    }
}
