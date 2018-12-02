using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MojeFunkcjeUniwersalneNameSpace
{
    /// <summary>
    /// Narzędzia do serializacji i deserializacji obiektów do/z pliku <c>.xml</c>
    /// </summary>
    /// <remarks>
    /// Należy pamiętać, aby klasy do serializacji opatrzone były atrybutem <c>Serializable</c>!
    /// </remarks>
    /// <author>
    /// Kamil Gadomski, PrimeSoft Polska.
    /// </author>
    public class XmlSerializator
    {
        /// <summary>
        /// Serializuj obiekt do pliku <c>.xml</c>
        /// </summary>
        /// <typeparam name="T">Typ obiektu do serializacji</typeparam>
        /// <param name="item">Obiekt serializacji</param>
        /// <param name="pathWithName">Ścieżka do pliku <c>.xml</c></param>
        public static void SerializeToXml<T>(T item, string pathWithName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            if (File.Exists(pathWithName))
            {
                using (Stream stream = new FileStream(pathWithName, FileMode.Truncate, FileAccess.Write, FileShare.None))
                {
                    xmlSerializer.Serialize(stream, item);
                }
            }
            else
            {
                using (Stream stream = new FileStream(pathWithName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    xmlSerializer.Serialize(stream, item);
                }
            }

        }

        /// <summary>
        /// Deserializuj plik <c>.xml</c> do obiektu
        /// </summary>
        /// <typeparam name="T">Typ obiektu do deserializacji</typeparam>
        /// <param name="pathWithName">Ścieżka do pliku <c>.xml</c></param>
        /// <returns>Obiekt zdeserializowany</returns>
        public static T DeserializeFromXML<T>(string pathWithName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T item;

            try
            {
                using (Stream stream = File.OpenRead(pathWithName))
                {
                    item = (T)xmlSerializer.Deserialize(stream);
                    return item;
                }
            }
            catch
            {
                return default(T);
            }

        }
    }
}
