namespace MVVMClasses.ViewModels
{
    /// <summary>
    /// Klasa bazowa viewModelu okna
    /// </summary>
    public abstract class ViewModelBase : ModelBase
    {

        /// <summary>
        /// Inicjalizacja formularza, tu inicjalizować Filtry
        /// </summary>
        public abstract void AkcjaStartowa();

        public ViewModelBase()
        {
            AkcjaStartowa();
        }
    }
}
