using FirebirdSql.Data.FirebirdClient;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FbClientBase
{
    /// <summary>
    /// Klasa bazowa dla Fasady Klienta bazy Sql
    /// </summary>
    public class FbCommandFasade
    {
        public FbCommand Command;
        
        public FbCommandFasade(FbCommand command, Action<string> logDebug, Action<string> logException)
        {
            Command = command;        
            LogException = logException;
            LogDebug = logDebug;
        }
        Action<string> LogException;
        Action<string> LogDebug;                                

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
                LogException($"Error while setting blob parameter {ParamName} with file {FileName} in SetFile");
            }
        }

        private void SetStreamParameter(string ParamName, Stream fs)
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] photo = br.ReadBytes((int)fs.Length);

                br.Close();

                fs.Close();

                Command.Parameters.Add(ParamName, FbDbType.Binary, photo.Length).Value = photo;
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
                LogException($"Error while setting blob parameter {ParamName} in SetStream");
            }
        }
        #endregion

        #region Parametry
        /// <summary>
        /// Metoda do przekazywania wartości parametrów do zapytania
        /// </summary>
        /// <param name="paramName">Nazwa parametru poprzedzona, może być poprzedzona znakiem '@' lub ':'</param>
        /// <param name="Typ">Typ danych</param>
        /// <returns></returns>
        public FbParameter ParamByName(String paramName, FbDbType paramType)
        {
            LogDebug("FbKlient__ParamByName('" + paramName + "' as " + paramType.GetType().Name + ")");
            try
            {
                paramName = paramName.Replace(":", "@");

                if (!paramName.Contains("@"))
                {
                    paramName = "@" + paramName;
                }

                FbParameter returned_param = null;
                foreach (FbParameter param in Command.Parameters)
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
                    returned_param = Command.Parameters.Add(paramName, paramType);
                }

                LogDebug("FbKlient__ParamByName-Koniec");
                return returned_param;
            }
            catch (Exception ex)
            {
                LogException("Error while setting parameter " + paramName + " " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Przypisuje parametrowi, wartość NULL
        /// </summary>
        /// <param name="paramName"></param>
        public void SetNull(String paramName)
        {
            try
            {
                ParamByName(paramName, FbDbType.SmallInt).Value = DBNull.Value;
            }
            catch (Exception ex)
            {
                LogException("Error while setting parameter " + paramName + " to null: " + ex.Message);
            }

        }
        #endregion


    }
}
