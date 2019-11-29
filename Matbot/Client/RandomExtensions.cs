using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot
{
    /// <summary>
    /// Basic extensions for Random.
    /// </summary>
    static class RandomExtensions
    {
        /// <summary>
        /// Generate random ulong value.
        /// </summary>
        /// <returns>Random ulong.</returns>
        public static ulong NextULong(this Random rand)
        {
            byte[] buff = new byte[8];
            rand.NextBytes(buff);

            return BitConverter.ToUInt64(buff, 0);
        }
    }
}
