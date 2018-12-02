using System;
using System.IO;
using System.Text;

namespace FilesServer.Package
{
    /// <summary>
    /// Rodzaj pakietu
    /// </summary>
    public enum NetPackageCommandType
    {
        /// <summary>
        /// Nie ustawiono trybu komunikacji, throw Exception podczas serializacji danych
        /// </summary>
        NotDefined = 0,
        /// <summary>
        /// Pobieranie pliku do klienta
        /// </summary>
        Download = 1,
        /// <summary>
        /// Zapis pliku na serwerze
        /// </summary>
        Upload = 2,
        /// <summary>
        /// Usunięcie pliku z serwera
        /// </summary>
        Delete = 3
    }
    
    /// <summary>
    /// Pakiet Transmisyjny
    /// </summary>
    public class NetPackage
    {
        /// <summary>
        /// Długość pliku danych przesyłanego zaraz za pakietem
        /// </summary>
        public long PackageLength;               //4 bajty //obejmuje tylko długość danych
        
        /// <summary>
        /// Rodzaj pakietu
        /// </summary> 
        public NetPackageCommandType Command;   //1 bajty

        /// <summary>
        /// Metoda serializująca pakiet do strumienia, wersja 2
        /// </summary>
        /// <returns>Strumień danych</returns>
        internal Stream Serialize2()
        {
            if (Command == NetPackageCommandType.NotDefined)
                throw new Exception("Nie zdefiniowano trybu komunikacji z serwerem !");
            MemoryStream stream = new MemoryStream();
            //długość pakietu danych binarnych 4 bajty
            stream.Write(BitConverter.GetBytes(PackageLength), 0, sizeof(Int64));
            //rodzaj komendy 1 bajt
            stream.Write(BitConverter.GetBytes(Convert.ToByte(Command)), 0, sizeof(byte));
            //string czy dane binarne 1 bit
            stream.Write(BitConverter.GetBytes(IsBinaryData), 0, sizeof(bool));
            //długość łańcucha danych string oraz łańcuch danych nazwy pliku
            serializeString(stream, Path.GetFileName(FileName));
            serializeString(stream, Path.GetFileName(FileNameWTimeStamp));
            if (!IsBinaryData)
                serializeString(stream, StringData);            

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Fizyczna nazwa pliku, konieczna tylko podczas dodawania pliku !
        /// </summary>
        public string FileName;                 //dynamic, poprzedzony długością


        /// <summary>
        /// Nazwę pliku z pieczątką czasową, wymagana, niezbędna do usuwania, pobierania
        /// </summary>
        public string FileNameWTimeStamp;

        /// <summary>
        /// Czy pakiet zawiera dane binarne czy łańcuch znaków
        /// </summary>
        public bool IsBinaryData;               //1 bit
     
        private string stringDataValue;
        public string StringData
        {
            get
            {
                return stringDataValue;
            }
            set
            {
                stringDataValue = value.Trim();
                IsBinaryData = string.IsNullOrEmpty(stringDataValue);
            }
        }

        /// <summary>
        /// Konstruktor pakietu danych
        /// </summary>
        public NetPackage()
        {
            PackageLength = 0;
            Command = NetPackageCommandType.NotDefined;            
            FileName = string.Empty;
            IsBinaryData = true;
            StringData = string.Empty;
        }


        /// <summary>
        /// Metoda deserializująca pakiet, wersja 2. Nie serializuje pliku
        /// </summary>
        /// <param name="stream"></param>
        public void Deserialize2(Stream stream)
        {
            //stream.Position = 0;
            BinaryReader binaryReader = new BinaryReader(stream);
            //if (stream.Length == 0)
            //    throw new Exception("Strumień danych jest pusty, nie można zdeserializować obiektu!");
            //długość pakietu danych binarnych 8 bajtów
            PackageLength = binaryReader.ReadInt64();

            //rodzaj komendy 1 bajt
            Command = (NetPackageCommandType)binaryReader.ReadByte();
            //string czy dane binarne 1 bit
            IsBinaryData = binaryReader.ReadBoolean();
            //długość łańcucha danych string oraz łańcuch danych nazwy pliku
            FileName = deserializeString(stream);
            FileNameWTimeStamp = deserializeString(stream);

            if (!IsBinaryData)
            {
                StringData = deserializeString(stream);
            }            
        }


        /// <summary>
        /// Serializuje łańcuch znaków, do strumienia
        /// </summary>
        /// <param name="stream">Łańcuch znaków do zserializowania</param>
        /// <param name="value">Strumień danych</param>
        private void serializeString(Stream stream, string value)
        {
            int str_length = 0;
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
                str_length = value.Length;
            }

            stream.Write(BitConverter.GetBytes(str_length), 0, sizeof(int));

            if (str_length>0)
            {
                byte[] buffer = Encoding.GetEncoding("windows-1250").GetBytes(value);
                stream.Write(buffer, 0, buffer.Length);
            }
            
        }

        /// <summary>
        /// Deserializuje łańcuch znaków ze strumienia, zwraca łańcuch
        /// </summary>
        /// <param name="stream">Strumień</param>
        /// <returns>Wynikowy łańcuch znaków</returns>
        private string deserializeString(Stream stream)
        {
            string returned_value = string.Empty;
            BinaryReader binaryReader = new BinaryReader(stream);
        
            int str_length = binaryReader.ReadInt32();

            if (str_length > 0)
            {
                byte[] buffer = null;
                buffer = binaryReader.ReadBytes(str_length);

                returned_value = Encoding.GetEncoding("windows-1250").GetString(buffer);                
            }

            return returned_value;
        }
    }
}
