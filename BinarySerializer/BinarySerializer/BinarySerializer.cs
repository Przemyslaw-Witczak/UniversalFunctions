using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BinarySerializerNamespace
{
    public class BinarySerializer
        {
        private IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            var attributeType = typeof(BinarySerializerAttribute);
            var allProperties = type.GetProperties();
            return allProperties.Where(prop => prop.GetCustomAttributes(attributeType, false).Any());
        }

        private IEnumerable<FieldInfo> GetFields(Type type)
        {
            var attributeType = typeof(BinarySerializerAttribute);
            var allFields = type.GetFields();
            return allFields.Where(prop => prop.GetCustomAttributes(attributeType, false).Any());
        }

        public void Serialize(object obj, Stream target)
        {
            ParameterSerializer ps = new ParameterSerializer();
            Stream sumStream = new MemoryStream();
            #region Właściwości
            var properties = GetProperties(obj.GetType());

            var attributeType = typeof(BinarySerializerAttribute);
            foreach (var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(obj, null);
                var attr = (BinarySerializerAttribute)propertyInfo.GetCustomAttributes(attributeType, false).First();

                Parameter propertyParameter = new Parameter(propertyInfo.Name);
                propertyParameter.AsValue = value;
                byte[] byteValue = MojeFunkcjeUniwersalneNameSpace.FunkcjeUniwersalne.GetBytes(value.ToString());

                sumStream.Write(byteValue, 0, value.ToString().Length);

                ps.Serialize(propertyParameter, target);
            }
            #endregion

            #region Pola
            //ToDo: Zaimplementować serializację pól
            #endregion

            #region Suma kontrolna
            string hashString = GetHashFromStream(sumStream);

            Parameter sumParameter = new Parameter(obj.GetType().Name);
            sumParameter.AsValue = hashString;

            ps.Serialize(sumParameter, target);
            #endregion
        }

        /// <summary>
        /// Metoda zwraca string na podstawie sumy MD5 ze strumienia przekazanego w parametrze
        /// </summary>
        /// <param name="sumStream">Strumień wejściowy</param>
        /// <returns>Łańcuch znaków zawierający wartość MD5 ze strumienia</returns>
        private static string GetHashFromStream(Stream sumStream)
        {
            int endStream = Convert.ToInt32(sumStream.Position);
            byte[] buffer = new byte[endStream];
            sumStream.Position = 0;
            sumStream.Read(buffer, 0, endStream);
            string streamString = MojeFunkcjeUniwersalneNameSpace.FunkcjeUniwersalne.Instance.GetString(buffer);
            string hashString = MojeFunkcjeUniwersalneNameSpace.FunkcjeUniwersalne.Instance.GetMd5Hash(streamString);
            return hashString;
        }

        public T Deserialize<T>(T instance, Stream source) where T : class, new()
        {
            List<Parameter> odczytaneParametry = new List<Parameter>();
            var properties = GetProperties(typeof(T));
            var obj = new T();

            //TODO: Tu poprawić odczytywanie uprawnienń           
            Stream sumStream = new MemoryStream();
            string readenHash = string.Empty;
            while (source.Position < source.Length)
            {                
                ParameterSerializer ps = new ParameterSerializer();
                Parameter readenParameter = ps.Deserialize(source);
                if (readenParameter.Name != obj.GetType().Name)
                {
                    byte[] byteValue = MojeFunkcjeUniwersalneNameSpace.FunkcjeUniwersalne.GetBytes(readenParameter.AsValue.ToString());
                    sumStream.Write(byteValue, 0, readenParameter.AsValue.ToString().ToString().Length);
                }
                else
                    readenHash = readenParameter.AsValue.ToString();
                odczytaneParametry.Add(readenParameter);
                
            }

            #region Właściwości
            foreach (PropertyInfo property in properties)
            {
                var readenParameter = odczytaneParametry.FirstOrDefault(param => param.Name == property.Name);
                if (readenParameter != null)
                    property.SetValue(instance, Convert.ChangeType(readenParameter.AsValue, property.PropertyType), null);
            }
            #endregion

            #region Pola
            //ToDo: Zaimplementować deserializację pól

            #endregion

            #region Suma kontrolna
            
            if (GetHashFromStream(sumStream) != readenHash)
                throw new Exception("Odczytana suma kontrolna klucza jest nieprawidłowa!");
            #endregion
            return obj;
        }


    }
}
