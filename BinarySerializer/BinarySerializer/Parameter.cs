using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializerNamespace
{
    public class Parameter : IParameter
    {
        public Parameter() : this("")
        {
            //NOP
            
        }

        public Parameter(string name)
        {
            ParameterTypeValue = ParameterValueType.UnSetValue;
            this.Name = name;
        }

        public string Name { get; set; }
        public ParameterValueType ParameterTypeValue { get; set; }

        private bool _asBool;
        private byte _asByte;
        private char _asChar;
        private DateTime _asTimeStamp;
        private decimal _asDecimal;
        private double _asDouble;
        private int _asInt;
        private String _asString;
        private object _asValue;
        private bool _isNull;

        public bool AsBool
        {
            get { return _asBool; }
            set
            {
                _asBool = value; 
                ParameterTypeValue = ParameterValueType.BooleanValue;                
            }
        }

        public byte AsByte
        {
            get { return _asByte; }
            set
            {
                _asByte = value;
                ParameterTypeValue = ParameterValueType.ByteValue;
            }
        }

        public char AsChar
        {
            get { return _asChar; }
            set
            {
                _asChar = value;
                ParameterTypeValue = ParameterValueType.CharValue;
            }
        }

        public DateTime AsTimeStamp
        {
            get { return _asTimeStamp; }
            set
            {
                _asTimeStamp = value;
                ParameterTypeValue = ParameterValueType.TimeStampValue;
            }
        }

        public DateTime AsDate
        {
            get { return _asTimeStamp; }
            set
            {
                _asTimeStamp = value;
                ParameterTypeValue = ParameterValueType.DateValue;
            }
        }

        public DateTime AsTime
        {
            get { return _asTimeStamp; }
            set
            {
                _asTimeStamp = value;
                ParameterTypeValue = ParameterValueType.TimeValue;
            }
        }

        public decimal AsDecimal
        {
            get { return _asDecimal; }
            set
            {
                _asDecimal = value;
                ParameterTypeValue = ParameterValueType.DecimalValue;
            }
        }

        public double AsDouble
        {
            get { return _asDouble; }
            set
            {
                _asDouble = value;
                ParameterTypeValue = ParameterValueType.DoubleValue;
            }
        }

        public int AsInt
        {
            get { return _asInt; }
            set
            {
                _asInt = value;
                ParameterTypeValue = ParameterValueType.IntegerValue;
            }
        }

        public string AsString
        {
            get { return _asString; }
            set
            {
                _asString = value;
                ParameterTypeValue = ParameterValueType.StringValue;
            }
        }

        public object AsValue
        {
            get { return _asValue; }
            set
            {
                _asValue = value;
                ParameterTypeValue = ParameterValueType.ObjectValue;
            }
        }

        public bool IsNull
        {
            get { return _isNull; }
            set
            {
                _isNull = value;
                ParameterTypeValue = ParameterValueType.UnSetValue;
            }
        }


        

        public string GetStringValue()
        {
            string returnedValue = string.Empty;

            switch (ParameterTypeValue)
            {
                case ParameterValueType.BooleanValue:
                    returnedValue = Convert.ToInt32(AsBool).ToString();
                    break;
                case ParameterValueType.ByteValue:
                    returnedValue = Convert.ToInt32(AsByte).ToString();
                    break;
                case ParameterValueType.CharValue:
                    returnedValue = AsChar.ToString();
                    break;
                case ParameterValueType.TimeStampValue:
                case ParameterValueType.DateValue:
                case ParameterValueType.TimeValue:
                    returnedValue = AsTimeStamp.ToString("o");
                    break;                                   
                case ParameterValueType.DecimalValue:
                    returnedValue = AsDecimal.ToString();
                    break;
                case ParameterValueType.DoubleValue:
                    returnedValue = AsDouble.ToString();
                    break;
                case ParameterValueType.IntegerValue:
                    returnedValue = AsInt.ToString();
                    break;
                case ParameterValueType.StringValue:
                    returnedValue = AsString;
                    break;                
                case ParameterValueType.UnSetValue:
                    break;                                                                                                    
                default:
                    returnedValue = AsValue.ToString();
                    break;
            }

            return returnedValue;
        }

        //private void SetValue(object value, ParameterValueType parameterValueType)
        //{
        //    if (value is string)
        //    {
        //        AsString = (string)(object)value;
        //        ParameterTypeValue = ParameterValueType.StringValue;
        //    }
        //    else if (value is int)
        //    {
        //        AsInt = (int)value;
        //        ParameterTypeValue = ParameterValueType.IntegerValue;
        //    }
        //    else if (value is bool)
        //    {
        //        AsBool = (bool)value;
        //        ParameterTypeValue = ParameterValueType.BooleanValue;
        //    }
        //    else if (value is decimal)
        //    {
        //        AsDecimal = (decimal)value;
        //        ParameterTypeValue = ParameterValueType.DecimalValue;
        //    }
        //    else if (value is double)
        //    {
        //        AsDouble = (double)value;
        //        ParameterTypeValue = ParameterValueType.DoubleValue;
        //    }
        //    else if (value is DateTime)
        //    {
        //        AsDate = (DateTime)value;
        //        ParameterTypeValue = ParameterValueType.TimeStampValue;
        //        AsTime = (DateTime)value;
        //        AsTimeStamp = (DateTime)value;
        //    }
        //    else if (value is byte)
        //    {
        //        AsByte = (byte)value;
        //        ParameterTypeValue = ParameterValueType.ByteValue;
        //    }
        //    else if (value is char)
        //    {
        //        AsChar = (char)value;
        //        ParameterTypeValue = ParameterValueType.CharValue;
        //    }
        //    else if (value == null)
        //    {
        //        IsNull = true;
        //        ParameterTypeValue = ParameterValueType.UnSetValue;
        //    }
        //}

        public void SetValueFromString(ParameterValueType parameterValueType, string value)
        {
            ParameterTypeValue = parameterValueType;
            switch (parameterValueType)
            {
                case ParameterValueType.UnSetValue:
                    break;
                case ParameterValueType.StringValue:
                    _asString = value;
                    break;
                case ParameterValueType.IntegerValue:
                    _asInt = Convert.ToInt32(value);
                    break;
                case ParameterValueType.BooleanValue:
                    _asBool = Convert.ToBoolean(Convert.ToInt16(value));
                    break;
                case ParameterValueType.DateValue:
                case ParameterValueType.TimeStampValue:
                case ParameterValueType.TimeValue:
                    _asTimeStamp = Convert.ToDateTime(value);
                    break;
                case ParameterValueType.DecimalValue:
                    _asDecimal = Convert.ToDecimal(value);
                    break;
                case ParameterValueType.DoubleValue:
                    _asDouble = Convert.ToDouble(value);
                    break;
                case ParameterValueType.ByteValue:
                    _asByte = Convert.ToByte(value);
                    break;
                case ParameterValueType.CharValue:
                    _asChar = Convert.ToChar(value);
                    break;                                    
                
                default:
                    _asValue = value;
                    break;
            }
        }

        

        
    }
}
