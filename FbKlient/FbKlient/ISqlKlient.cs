using System;

namespace FbKlientNameSpace
{
    public interface ISqlKlient
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
        /// Dodanie treści zapytania do aktualnej komendy
        /// </summary>
        /// <param name="SqlString">Treść zapytania</param>
        void AddSQL(string SqlString);

        /// <summary>
        /// Konfiguracja połączenia do bazy danych
        /// </summary>
        void ConfigureDataBase();

        /// <summary>
        /// Zamknięcie połączenia do bazy danych
        /// </summary>
        void DataBaseClose();

        /// <summary>
        /// Zwolnienie obiektu
        /// </summary>
        void Dispose();

        /// <summary>
        /// Wykonanie zapytań ze zwracaniem rekordów
        /// </summary>
        void Execute();

        /// <summary>
        /// Wykonanie zapytanie bez zwracania rekordów
        /// </summary>
        void ExecuteNonQuery();


        bool GetBoolean(string FieldName);
        byte GetByte(string FieldName);
        char GetChar(string FieldName);
        DateTime GetDateTime(string FieldName);
        decimal GetDecimal(string FieldName);
        double GetDouble(string FieldName);
        short GetInt16(string FieldName);
        int GetInt32(string FieldName);
        long GetInt64(string FieldName);
        string GetString(string FieldName);
        object GetValue(string FieldName);
        bool IsDBNull(string FieldName);

        /// <summary>
        /// Przesunięcie wskaźnika na następny zwrócony rekord
        /// </summary>
        /// <returns></returns>
        bool Read();

        /// <summary>
        /// Ustawienie parametrowi wartości NULL
        /// </summary>
        /// <param name="paramName">Nazwa parametru</param>
        void SetNull(string paramName);
    }
}