using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    class ClassConverter
    {
        public static object ConvertToObj(object value, Type conversionType)
        {
            if (conversionType.IsEnum && (value.GetType().Equals(typeof(string)) || value.GetType().Equals(typeof(String))))
            {
                try
                {
                    return Enum.Parse(conversionType, (string)value);
                }
                catch (ArgumentException)
                {
                    throw new InvalidCastException("Cannot convert \"" + (string)value + "\" to enum: " + conversionType.Name);
                }
            }


            return Convert.ChangeType(value, conversionType);
        }
    }
}
