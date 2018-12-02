using System;

namespace BinarySerializerNamespace
{
    public interface IParameter
    {
        bool AsBool { get; set; }
        byte AsByte { get; set; }
        char AsChar { get; set; }
        DateTime AsDate { get; set; }
        decimal AsDecimal { get; set; }
        double AsDouble { get; set; }
        int AsInt { get; set; }
        string AsString { get; set; }
        DateTime AsTime { get; set; }
        DateTime AsTimeStamp { get; set; }
        object AsValue { get; set; }
        bool IsNull { get; set; }
        string Name { get; set; }
        ParameterValueType ParameterTypeValue { get; set; }

        string GetStringValue();
        void SetValueFromString(ParameterValueType parameterValueType, string value);
    }
}