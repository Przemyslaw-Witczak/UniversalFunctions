namespace ISqlClientCore
{
    public interface ISqlClient : ISqlSimpleClient
    {
        /// <summary>
        /// Liczba komend, zapytań do bazy danych, updat'ów, insertów
        /// </summary>
        int CommandsCount { get; /*private set;*/ }

        /// <summary>
        /// Indeks aktualnie dodawanego zapytania
        /// </summary>
        int QueryId { get; set; }

        /// <summary>
        /// Indeks aktualnie odczytywanego zapytania
        /// </summary>
        int ResponseId { get; set; }


        /// <summary>
        /// Konfiguracja połączenia do bazy danych
        /// </summary>
        void ConfigureDataBase();

    }
}