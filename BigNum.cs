using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multidigit_arithmetic
{
    class BigNum
    {
        public ulong[] array = new ulong[0];

        public BigNum(int len)
        {
            array = new ulong[len];
        }

        public BigNum(string a)
        {
            string mass = a;

            while (mass.Length % 8 != 0)
            {
                mass = "0" + mass;
            }

            array = new ulong[mass.Length / 8];

            for (int i = 0; i < mass.Length; i += 8)
            {
                array[i / 8] = Convert.ToUInt64(mass.Substring(i, 8), 16);
            }
            Array.Reverse(array);
        }

        public override string ToString()
        {
            var result = string.Concat(array.Select(chunk => chunk.ToString("X").PadLeft(sizeof(ulong), '0')).Reverse()).TrimStart('0');
            if (result == "")
            {
                return "0";
            }
            else
            { return result; }
        }

    }
}
