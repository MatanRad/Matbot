using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot
{
    static class RandomExtensions
    {
        public static ulong NextULong(this Random rand)
        {
            byte[] buff = new byte[8];
            rand.NextBytes(buff);

            return BitConverter.ToUInt64(buff, 0);
        }
    }
}
