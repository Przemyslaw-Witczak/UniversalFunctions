using FirebirdSql.Data.FirebirdClient;
using System;
using System.IO;

namespace FbClientBaseNameSpace
{
    /// <summary>
    /// Klasa bazowa klientów/fasad do FirebirdSql.Data.FirebirdClient
    /// </summary>
    /// <remarks>Zawiera metody wspólne, niezależne od środowiska. Pobieranie wartości requestów, ustawianie zapytań i parametrów.</remarks>
    public abstract class FbAbstractClient
    {
        /// <summary>
        /// Zewnętrzna metoda logująca między innymi zapytania.
        /// </summary>
        public Action<string> DebugLog;

        /// <summary>
        /// Pobiera aktualny command w celu przypisania parametrów zapytania.
        /// </summary>
        /// <returns></returns>
        protected abstract FbCommand GetCurrentCommand();

        /// <summary>
        /// Pobiera aktualny response reader
        /// </summary>
        /// <returns></returns>
        public abstract FbDataReader GetCurrentResponse();

        /// <summary>
        /// Metoda abstrakcyjna, wymaga implementacji w celu logowania komunikatów błędów, lub wyświetlania okien komunikatów, zależnie od implementacji klasy pochodnej
        /// </summary>
        /// <param name="message"></param>
        protected abstract void ExceptionLogOrMessage(string message);

        #region Odczyt wartości pól z zapytania bazodanowego
        /// <summary>
        /// Metoda zwraca indeks pola w tablicy wszystkich odczytanych kolumn
        /// </summary>
        /// <param name="FieldName"></param>
        /// <returns>Indeks kolumny, jeżeli pole nie występuje w responsie, zwraca -1 </returns>
        public int GetFieldIndex(String FieldName)
        {
            DebugLog?.Invoke("FbKlient__GetFieldIndex '" + FieldName + "'");
            try
            {
                int returned_value = GetCurrentResponse().GetOrdinal(FieldName);
                if (returned_value < 0)
                {
                    ExceptionLogOrMessage("Can not find field " + FieldName);
                }

                return returned_value;
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error in GetFieldIndex '" + FieldName + "' " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Zwraca wartość bool odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public bool GetBoolean(String FieldName)
        {
            try
            {
                if (GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to bool !");
                }

                return GetCurrentResponse().GetBoolean(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting bool value from '" + FieldName + "' !\n" + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Zwraca wartość byte odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public byte GetByte(String FieldName)
        {
            try
            {
                if (GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to byte !");
                }

                return GetCurrentResponse().GetByte(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting byte value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Char odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public char GetChar(String FieldName)
        {
            try
            {
                if (GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to char !");
                }

                return GetCurrentResponse().GetChar(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting byte value from '" + FieldName + "' !\n" + ex.Message);
            }
            return Convert.ToChar(0);
        }

        /// <summary>
        /// Zwraca wartość DateTime odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public DateTime GetDateTime(String FieldName)
        {
            try
            {
                if (GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to DateTime !");
                }

                return GetCurrentResponse().GetDateTime(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting DateTime value from '" + FieldName + "' !\n" + ex.Message);
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Zwraca wartość Decimal odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public decimal GetDecimal(String FieldName)
        {
            try
            {
                if (GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Decimal !");
                }

                return GetCurrentResponse().GetDecimal(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting Decimal value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Double odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public double GetDouble(String FieldName)
        {
            try
            {
                if (GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Double !");
                }

                return GetCurrentResponse().GetDouble(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting Double value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Int16 odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public short GetInt16(String FieldName)
        {
            try
            {
                if (GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Int16 !");
                }

                return GetCurrentResponse().GetInt16(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting Int16 value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Int32 odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public int GetInt32(String FieldName)
        {
            try
            {
                if (GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Int32 !");
                }

                return GetCurrentResponse().GetInt32(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting Int32 value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość Int64 odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public long GetInt64(String FieldName)
        {
            try
            {
                if (GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Int64 !");
                }

                return GetCurrentResponse().GetInt64(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting Int64 value from '" + FieldName + "' !\n" + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Zwraca wartość String odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public string GetString(String FieldName)
        {
            try
            {
                //if (Response.IsDBNull(GetFieldIndex(FieldName)))
                //    throw new Exception("'" + FieldName + "' has NULL value, can't convert to string !");
                return GetCurrentResponse().GetString(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting string value from '" + FieldName + "' !\n" + ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// Zwraca wartość Object odczytaną z bazy danych
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Zwrócona wartość</returns>
        public object GetValue(String FieldName)
        {
            try
            {
                return GetCurrentResponse().GetValue(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while getting value from '" + FieldName + "' !\n" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Zwraca informację czy pole odczytane z bazy danych ma wartość NULL czy inną
        /// </summary>
        /// <param name="FieldName">Nazwa kolumny</param>
        /// <returns>Prawda/Fałsz</returns>
        public bool IsDBNull(String FieldName)
        {
            try
            {
                return GetCurrentResponse().IsDBNull(GetFieldIndex(FieldName));
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while checking if '" + FieldName + "' is null!\n" + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Metoda pobiera załącznik z bazy danych z parametru blob o podanej nazwie i zapisuje w podanej ścieżce
        /// </summary>
        /// <param name="FieldName">Nazwa parametru</param>
        /// <param name="FileName">Ścieżka do pliku gdzie zapisać</param>
        public void GetFile(String FieldName, String FileName)
        {

            try
            {
                //using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write)) // Writes the BLOB to a file
                Stream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);//) // Writes the BLOB to a file
                {
                    GetIntoStream(FieldName, ref fs);
                    fs.Close();
                }
            }
            catch
            {
                ExceptionLogOrMessage("Error while writing blob parameter " + FieldName + " with file " + FileName);
            }

        }

        /// <summary>
        /// Metoda pobiera parametr o podanej nazwie i zapisuje w strumieniu
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="fs"></param>
        public void GetIntoStream(string FieldName, ref Stream fs)
        {
            //using (BinaryWriter bw = new BinaryWriter(fs)) // Streams the BLOB to the FileStream object.
            BinaryWriter bw = new BinaryWriter(fs);
            {
                int bufferSize = 1024;                   // Size of the BLOB buffer.
                byte[] outbyte = new byte[bufferSize];  // The BLOB byte[] buffer to be filled by GetBytes.
                long retval;                            // The bytes returned from GetBytes.
                long startIndex = 0;                    // The starting position in the BLOB output.
                                                        // Reset the starting byte for the new BLOB.
                startIndex = 0;
                var currentResponse = GetCurrentResponse();
                var currentFieldIndex = GetFieldIndex(FieldName);
                // Read the bytes into outbyte[] and retain the number of bytes returned.
                retval = currentResponse.GetBytes(currentFieldIndex, startIndex, outbyte, 0, bufferSize);

                // Continue reading and writing while there are bytes beyond the size of the buffer.
                while (retval == bufferSize)
                {
                    bw.Write(outbyte);
                    bw.Flush();

                    // Reposition the start index to the end of the last buffer and fill the buffer.
                    startIndex += bufferSize;
                    retval = currentResponse.GetBytes(currentFieldIndex, startIndex, outbyte, 0, bufferSize);
                }

                // Write the remaining buffer.
                bw.Write(outbyte, 0, (int)retval);
                bw.Flush();


                outbyte = null;
            }
        }

        #endregion

        #region Ustawienie wartości parametrów
        /// <summary>
        /// Metoda do przekazywania wartości parametrów do zapytania
        /// </summary>
        /// <param name="paramName">Nazwa parametru poprzedzona, może być poprzedzona znakiem '@' lub ':'</param>
        /// <param name="Typ">Typ danych</param>
        /// <returns></returns>
        public FbParameter ParamByName(String paramName, FbDbType paramType)
        {
            DebugLog?.Invoke("FbKlient__ParamByName('" + paramName + "' as " + paramType.GetType().Name + ")");
            try
            {
                paramName = paramName.Replace(":", "@");

                if (!paramName.Contains("@"))
                {
                    paramName = "@" + paramName;
                }

                FbParameter returned_param = null;
                foreach (FbParameter param in GetCurrentCommand().Parameters)
                {
                    if (param.ParameterName.Equals(paramName))
                    {
                        returned_param = param;
                        returned_param.FbDbType = paramType;
                        break;
                    }
                }

                if (returned_param == null)
                {
                    returned_param = GetCurrentCommand().Parameters.Add(paramName, paramType);
                }

                DebugLog?.Invoke("FbKlient__ParamByName-Koniec");
                return returned_param;
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while setting parameter " + paramName + " " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Przypisuje parametrowi, wartość NULL
        /// </summary>
        /// <remarks>Tak naprawdę parametrowi przypisywany jest DbNull.Value</remarks>
        /// <param name="paramName">Nazwa parametru</param>
        public void SetNull(String paramName)
        {
            try
            {
                ParamByName(paramName, FbDbType.SmallInt).Value = DBNull.Value;
            }
            catch (Exception ex)
            {
                ExceptionLogOrMessage("Error while setting parameter " + paramName + " to null: " + ex.Message);
            }

        }
        #endregion

        #region Załączniki
        /// <summary>
        /// Dodaje plik jako obiekt blob
        /// </summary>
        /// <param name="ParamName">nazwa parametru zapytania</param>
        /// <param name="FileName">ścieżka do pliku</param>
        public void SetFile(String ParamName, String FileName)
        {
            try
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    SetStreamParameter(ParamName, fs);
                }
            }
            catch
            {
                ExceptionLogOrMessage($"Error while setting blob parameter {ParamName} with file {FileName} in SetFile");
            }
        }

        private void SetStreamParameter(string ParamName, Stream fs)
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] photo = br.ReadBytes((int)fs.Length);

                br.Close();

                fs.Close();

                GetCurrentCommand().Parameters.Add(ParamName, FbDbType.Binary, photo.Length).Value = photo;
            }
        }

        /// <summary>
        /// Dodaje strumień jako obiekt blob
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="memoryStream"></param>
        public void SetFromStream(string ParamName, Stream memoryStream)
        {
            try
            {
                memoryStream.Position = 0;
                SetStreamParameter(ParamName, memoryStream);
            }
            catch
            {
                ExceptionLogOrMessage($"Error while setting blob parameter {ParamName} in SetStream");
            }
        }
        #endregion
    }
}
