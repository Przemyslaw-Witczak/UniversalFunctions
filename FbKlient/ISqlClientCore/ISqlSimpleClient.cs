using FirebirdSql.Data.FirebirdClient;
using System;

namespace ISqlClientCore
{
    public interface ISqlSimpleClient
    {
        /// <summary>
        /// Dodanie treści zapytania do aktualnej komendy
        /// </summary>
        /// <param name="SqlString">Treść zapytania</param>
        void AddSQL(string SqlString);
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

        /// <summary>
        /// Metoda do przekazywania wartości parametrów do zapytania
        /// </summary>
        /// <param name="paramName">Nazwa parametru poprzedzona, może być poprzedzona znakiem '@' lub ':'</param>
        /// <param name="Typ">Typ danych</param>
        /// <returns></returns>
        FbParameter ParamByName(String paramName, FbDbType paramType);
    }
}