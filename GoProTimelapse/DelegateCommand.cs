using System;
using System.Windows.Input;

namespace journeyofcode.GoProTimelapse
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _start;
        private readonly Func<bool> _canStart;

        public DelegateCommand(Action start, Func<bool> canStart)
        {
            this._start = start;
            this._canStart = canStart;
        }
        public bool CanExecute(object parameter)
        {
            return this._canStart();
        }
        public void Execute(object parameter)
        {
            this._start();
        }
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}