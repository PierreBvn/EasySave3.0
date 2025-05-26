using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave_2._0.Models
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public event EventHandler CanExecuteChanged;
        public RelayCommand(Action execute) => _execute = execute;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute();
    }
}
