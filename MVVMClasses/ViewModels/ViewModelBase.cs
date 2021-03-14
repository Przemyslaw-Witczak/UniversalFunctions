namespace MVVMClasses.ViewModels
{
    /// <summary>
    /// Klasa bazowa viewModelu okna
    /// </summary>
    public abstract class ViewModelBase : ModelBase
    {
        /// <summary>
        /// Inicjalizacja formularza
        /// </summary>
        public abstract void AkcjaStartowa();

        public ViewModelBase()
        {
            AkcjaStartowa();
        }
    }
}
