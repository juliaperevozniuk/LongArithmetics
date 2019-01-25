using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multidigit_arithmetic
{
    class Operation
    {
        public static BigNum Zero = new BigNum(1);
        public static BigNum One = new BigNum("1");
        public static ulong carry = 0;
        public static ulong borrow = 0;
        public static ulong temp = 0;

        public static void Control(BigNum a, BigNum b, int maxlen)
        {
            Array.Resize(ref a.array, maxlen);
            Array.Resize(ref b.array, maxlen);
        }


        public int Compare(BigNum a, BigNum b)
        {
            var maxlen = Math.Max(a.array.Length, b.array.Length);
            Control(a, b, maxlen);
            for (int i = a.array.Length - 1; i > -1; i--)
            {
                if (a.array[i] > b.array[i]) return 1;
                if (a.array[i] < b.array[i]) return -1;
            }
            return 0;
        }


        public BigNum Addition(BigNum a, BigNum b)
        {
            var maxlen = Math.Max(a.array.Length, b.array.Length);
            Control(a, b, maxlen);
            carry = 0;
            var result = new BigNum(maxlen + 1);

            for (int i = 0; i < maxlen; i++)
            {
                ulong temp = a.array[i] + b.array[i] + carry;
                carry = temp >> 32;
                result.array[i] = temp & 0xffffffff;
            }

            result.array[a.array.Length] = carry;

            return result;
        }


        public BigNum Substraction(BigNum a, BigNum b)
        {
            if (Compare(a, b) == 0 || Compare(a, b) == -1) { return Zero; }

            var maxlen = Math.Max(a.array.Length, b.array.Length);
            Control(a, b, maxlen);

            var result = new BigNum(maxlen);

            for (int i = 0; i < maxlen; i++)
            {
                temp = a.array[i] - b.array[i] - borrow;
                result.array[i] = temp & 0xffffffff;
                borrow = (temp <= a.array[i]) ? 0ul : 1ul;
            }
            return result;
        }


        public static BigNum LongMulOneDigit(BigNum a, ulong b)
        {
            BigNum c = new BigNum(a.array.Length + 1);
            for (int i = 0; i < a.array.Length; i++)
            {
                temp = a.array[i] * b + carry;
                c.array[i] = temp & 0xffffffff;
                carry = temp >> 32;
            }
            c.array[a.array.Length] = carry;
            return c;
        }


        public BigNum ShiftDigitsToHigh(BigNum a, int i)
        { 
            BigNum result = new BigNum(a.array.Length + i);
            for (int k = 0; k < a.array.Length; k++)
            {
                result.array[k + i] = a.array[k]; 
            }
            return result;
        }


        public static BigNum RemoveHighZeros(BigNum c)
        {
            int i = c.array.Length - 1;
            while (c.array[i] == 0)
            {
                i--;
            }
            BigNum result = new BigNum(i + 1);
            Array.Copy(c.array, result.array, i + 1);
            return result;
        }


        public BigNum Multiplication(BigNum a, BigNum b)
        {
            var maxlen = Math.Max(a.array.Length, b.array.Length);
            Control(a,b, maxlen);

            var result = new BigNum(a.array.Length + b.array.Length);
            for (int i = 0; i < a.array.Length; i++)
            {
                carry = 0;
                for (int j = 0; j < b.array.Length; j++)
                {
                    ulong temp = result.array[i + j] + a.array[j] * b.array[i] + carry;
                    result.array[i + j] = temp & 0xFFFFFFFF;
                    carry = temp >> 32;
                }
                result.array[i + a.array.Length] = carry;
            }
            result = RemoveHighZeros(result);
            return result;
        }


        public BigNum ShiftBitsToHigh(BigNum a, int shift)
        {
            int t = shift / 32;
            int s = shift - t * 32;
            ulong n, carry = 0;

            BigNum result = new BigNum(a.array.Length + t + 1);

            for (int i = 0; i < a.array.Length; i++)
            {
                n = a.array[i];
                n = n << s;
                result.array[i + t] = (n & 0xFFFFFFFF) | carry;
                carry = (n & 0xFFFFFFFF00000000) >> 32;
            }
            result.array[a.array.Length + t] = carry;
            return result;
        }


        public static int HighNotZeroIndex(BigNum a)
        {
            for (var i = a.array.Length - 1; i >= 0; i--)
            {
                if (a.array[i] > 0) { return i; }
            }
            return 0;
        }


        public int Bitlen(BigNum a)
        {
            var bits = 0;
            var index = HighNotZeroIndex(a);
            var temp = a.array[index];
            while (temp > 0)
            {
                temp >>= 1;
                bits++;
            }
            return bits + 32 * index;
        }


        
        public BigNum[] LongDiv(BigNum a, BigNum b)
        {
            int k = Bitlen(b);
            int t = 0; 
            BigNum r = new BigNum(a.ToString());
            BigNum q = new BigNum(1);
            BigNum c = new BigNum(1);

            while(Compare(r, b) >= 0)
             {
                t = Bitlen(r);
                c = ShiftBitsToHigh(b, t - k);
                if(Compare(r, c) == -1)
                {
                    t--;
                    c = ShiftBitsToHigh(b, t - k);
                }
                r = Substraction(r, c);
                q = Addition(q, ShiftBitsToHigh(One, t - k));
             }

            BigNum[] result = new BigNum[] {q, r};
            return result;
        }
        

        public BigNum LongPowerWindow(BigNum a, BigNum b)
        {
            Dictionary<char, int> HexToDecimal = new Dictionary<char, int>
            {
                {'0', 0}, {'1', 1}, {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9}, {'A', 10}, {'B', 11}, {'C', 12}, {'D', 13}, {'E', 14}, {'F', 15}
            };
            var Bstring = b.ToString();

            var result = new BigNum("1");

            if (Compare(a, Zero) == 0) { return Zero; }
            if (Compare(b, Zero) == 0) { return One; }

            BigNum[] DegreesOfTwo = new BigNum[16];

            DegreesOfTwo[0] = new BigNum("1");
            DegreesOfTwo[1] = a;

            for (int k = 2; k < DegreesOfTwo.Length; k++)
            {
                DegreesOfTwo[k] = Multiplication(DegreesOfTwo[k - 1], a);
            }

            for(int i = 0; i < Bstring.Length; i++)
            {
                var x = Bstring[i];
                result = Multiplication(result, DegreesOfTwo[ HexToDecimal[ Bstring[ i]]]);
                if(i != Bstring.Length-1)
                {
                    for(int j = 1; j <= 4; j++)
                    {
                        result = Multiplication(result, result);
                    }
                }
            }
            return result;
        }
    }
}
