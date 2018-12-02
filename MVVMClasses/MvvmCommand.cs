using System;
using System.Windows.Input;

namespace MVVMClasses
{
    /// <summary>
    /// Moje zdarzenie naciśnięcia przycisku z funkcją walidacji czy można zezwolić na wywołanie
    /// </summary>
    public class MvvmCommand : ICommand
    {
        /// <summary>
        /// Metoda uruchamiana do walidacji
        /// </summary>
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Metoda uruchamiana podczas kliknięcia przycisku
        /// </summary>
        private readonly Action<object> _executeAction;

        /// <summary>
        /// Konstruktor klasy niestandardowego polecenia, z opcją walidacji
        /// </summary>
        /// <param name="executeAction"></param>
        /// <param name="canExecute"></param>
        public MvvmCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            
            this._executeAction = executeAction;
            this._canExecute = canExecute;           
        }

        /// <summary>
        /// Konstruktor klasy niestandardowego polecenia, bez walidacji
        /// </summary>
        /// <param name="executeAction"></param>
        public MvvmCommand(Action<object> executeAction) : this(executeAction, DefaultCanExecute)
        {
            
        }

        /// <summary>
        /// Metoda domyślna dla konstruktora bez walidacji
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static bool DefaultCanExecute(object arg)
        {
            return true;
        }

        private event EventHandler CanExecuteChangedInternal;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this.CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this.CanExecuteChangedInternal -= value;
            }
        }


        public bool CanExecute(object parameter)
        {            
            return _canExecute != null && _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }
    }
}
