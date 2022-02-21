using FirebirdSql.Data.FirebirdClient;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace FbClientBase
{
    /// <summary>
    /// Klasa metod rozszerzających do FbCommand
    /// </summary>
    public static class FbCommandExtensions
    {
        /// <summary>
        /// Funkcja generująca log, z zapytaniem oraz parametrami przekazanymi do zapytania
        /// </summary>
        /// <param name="commandString">StringBuilder</param>
        /// <param name="command">Obiekt SQL Command</param>
        public static void GetCommandLog(this FbCommand command, StringBuilder commandString)
        {
            commandString.Append(Regex.Replace(command.CommandText, @"\s+", " ") + "|");
            commandString.Append(getParameterValues(command));
            commandString.Append(Environment.NewLine);
        }

        /// <summary>
        /// Metoda zwraca nazwy parametrów przekazanych do zapytania z wartościami
        /// </summary>
        /// <param name="command">Obiekt SQL Command</param>
        /// <returns>Łańcuch znaków</returns>
        private static string getParameterValues(FbCommand command)
        {
            var returnedValue = new StringBuilder();
            foreach (FbParameter param in command.Parameters)
            {
                if (param != null)
                {
                    if (param.Value != null && param.Value != DBNull.Value)
                        returnedValue.AppendFormat("{0}='{1}';", param.ParameterName,
                            param.Value.ToString().Length > 255
                                ? param.Value.ToString().Substring(0, 255)
                                : param.Value.ToString());
                    else
                        returnedValue.AppendFormat("'{0}'=NULL;", param.ParameterName);
                }


            }

            return returnedValue.ToString();
        }
    }
}
