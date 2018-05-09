using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    /// <summary>
    /// Class for converting objects into different types.
    /// Useful for CommandVariation matching.
    /// </summary>
    class ClassConverter
    {
        /// <summary>
        /// Convert object into a given type.
        /// </summary>
        /// <param name="value">The object to be converted.</param>
        /// <param name="conversionType">Desired type.</param>
        /// <returns>Converted object of type 'conversionType'.</returns>
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
