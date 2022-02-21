using FirebirdSql.Data.FirebirdClient;
using System;
using System.IO;

namespace FbClientBase
{
    public abstract class FbReaderBase
    {
        protected FbDataReader Response;
        public Action<string> DebugLog;
        protected abstract void ExceptionLogOrMessage(string message);
        

        public int GetFieldIndex(String FieldName)
        {
            DebugLog("FbKlient__GetFieldIndex '" + FieldName + "'");
            try
            {
                int returned_value = Response.GetOrdinal(FieldName);
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
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to bool !");
                }

                return Response.GetBoolean(GetFieldIndex(FieldName));
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
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to byte !");
                }

                return Response.GetByte(GetFieldIndex(FieldName));
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
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to char !");
                }

                return Response.GetChar(GetFieldIndex(FieldName));
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
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to DateTime !");
                }

                return Response.GetDateTime(GetFieldIndex(FieldName));
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
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Decimal !");
                }

                return Response.GetDecimal(GetFieldIndex(FieldName));
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
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Double !");
                }

                return Response.GetDouble(GetFieldIndex(FieldName));
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
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Int16 !");
                }

                return Response.GetInt16(GetFieldIndex(FieldName));
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
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Int32 !");
                }

                return Response.GetInt32(GetFieldIndex(FieldName));
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
                if (Response.IsDBNull(GetFieldIndex(FieldName)))
                {
                    throw new Exception("'" + FieldName + "' has NULL value, can't convert to Int64 !");
                }

                return Response.GetInt64(GetFieldIndex(FieldName));
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
                return Response.GetString(GetFieldIndex(FieldName));
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
                return Response.GetValue(GetFieldIndex(FieldName));
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
                return Response.IsDBNull(GetFieldIndex(FieldName));
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
                int bufferSize = 100;                   // Size of the BLOB buffer.
                byte[] outbyte = new byte[bufferSize];  // The BLOB byte[] buffer to be filled by GetBytes.
                long retval;                            // The bytes returned from GetBytes.
                long startIndex = 0;                    // The starting position in the BLOB output.
                                                        // Reset the starting byte for the new BLOB.
                startIndex = 0;

                // Read the bytes into outbyte[] and retain the number of bytes returned.
                retval = Response.GetBytes(GetFieldIndex(FieldName), startIndex, outbyte, 0, bufferSize);

                // Continue reading and writing while there are bytes beyond the size of the buffer.
                while (retval == bufferSize)
                {
                    bw.Write(outbyte);
                    bw.Flush();

                    // Reposition the start index to the end of the last buffer and fill the buffer.
                    startIndex += bufferSize;
                    retval = Response.GetBytes(GetFieldIndex(FieldName), startIndex, outbyte, 0, bufferSize);
                }

                // Write the remaining buffer.
                bw.Write(outbyte, 0, (int)retval);
                bw.Flush();


                outbyte = null;
            }
        }
    }
}
