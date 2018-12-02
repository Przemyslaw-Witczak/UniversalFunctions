using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializerNamespace
{

    /// <summary>
    /// Klasa serializująca i deserializująca parametry dziedziczące po interfejsie IParameter
    /// </summary>
    public class ParameterSerializer
    {
        public void Serialize(IParameter parameter, Stream target)
        {
            string etap = string.Empty;
            try
            {
                int str_length = 1;
                string value = parameter.GetStringValue();

                etap = "Długość wartości parametru";
                str_length = value.Length;
                target.Write(BitConverter.GetBytes(str_length), 0, sizeof(int));

                etap = "Rodzaj parametru";
                int objectType = (int)parameter.ParameterTypeValue;
                target.Write(BitConverter.GetBytes(objectType), 0, sizeof(int));

                etap = "Długość nazwy parametru";
                int length = parameter.Name.Length;
                target.Write(BitConverter.GetBytes(length), 0, sizeof(int));

                etap = "Nazwa parametru";
                byte[] buffer = Encoding.GetEncoding("windows-1250").GetBytes(parameter.Name);
                target.Write(buffer, 0, buffer.Length);

                etap = "Wartość parametru";
                if (str_length > 0)
                {
                    buffer = Encoding.GetEncoding("windows-1250").GetBytes(value);
                    target.Write(buffer, 0, buffer.Length);                    
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Error in ParameterSerializer::Serialize('{parameter?.Name}' value type:'{parameter?.ParameterTypeValue.ToString()}' step:'{etap}', {ex.Message}");
            }
            
        }
        

        public Parameter Deserialize(Stream stream)
        {
            Parameter returnedParameter = new Parameter();
            BinaryReader binaryReader = new BinaryReader(stream);
            byte[] buffer = null;
            string etap = string.Empty;
            try
            {
                etap = "Długość wartości parametru";
                int str_length = binaryReader.ReadInt32();

                etap = "Rodzaj parametru";
                int intValue = binaryReader.ReadInt32();
                returnedParameter.ParameterTypeValue = (ParameterValueType)intValue;//binaryReader.ReadInt32();

                etap = "Długość nazwy parametru";
                int nameLength = 0;
                nameLength = binaryReader.ReadInt32();

                etap = "Nazwa parametru";
                buffer = binaryReader.ReadBytes(nameLength);
                returnedParameter.Name = Encoding.GetEncoding("windows-1250").GetString(buffer);

                etap = "Wartość parametru";
                if (str_length > 0)
                {
                    buffer = binaryReader.ReadBytes(str_length);

                    string strValue = Encoding.GetEncoding("windows-1250").GetString(buffer);
                    returnedParameter.SetValueFromString(returnedParameter.ParameterTypeValue, strValue);
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Error in ParameterSerializer::Deserialize(Parameter name'{returnedParameter?.Name}' value type:'{returnedParameter?.ParameterTypeValue.ToString()}' step:'{etap}', {ex.Message}");
            }
            return returnedParameter;
        }
    }
}
