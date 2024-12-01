using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataGridDynamicTest
{
    internal class Command : ICommand
    {
        private Action? _action;

        private Action<object>? _action2;

        public event EventHandler? CanExecuteChanged;


        public Command(Action action)
        {
            this._action = action;
        }

        public Command(Action<object> action)
        {
            this._action2 = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (_action != null)
            {
                _action?.Invoke();
            }

            _action2?.Invoke(parameter);
        }
    }
}
