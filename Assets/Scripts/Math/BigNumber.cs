using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using Utils;

namespace Core.Math
{
    [DataContract]
    public struct BigNumber
    {
        private const int TOP_TRESHOLD = 100000;
        private const long MAX_RANKABLE_VALUE = long.MaxValue / 10;

        private static readonly char[] _firstLetters =
        {
            'K', 'M', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'L', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z', 'A'
        };

        public static BigNumber Zero = new BigNumber("0");
        public static BigNumber One = new BigNumber("1");

        public BigNumber(long newValue, long newRank)
        {
            value = newValue;
            rank = newRank;

            CheckThreshold();
        }

        private static int LettersToPower(string letters)
        {
            if (letters.Length == 1)
            {
                return (_firstLetters.IndexOf(letters.First()) + 1) * 3;
            }

            int result = 0;
            int codeA = 65;
            for (int i = 0; i < letters.Length; i++)
            {
                int multiplier = (letters[i] - codeA + 1);
                int basePow = (int) Mathf.Pow(26, letters.Length - i - 1);
                result += multiplier * basePow;
            }

            return result * 3;
        }

        public BigNumber(int number)
        {
            rank = 0;
            value = number;
        }

        public BigNumber(string constructString)
        {
            try
            {
                if (!constructString.IsNullOrEmpty())
                {
                    if (constructString.Contains("E+") || constructString.Contains("E-"))
                    {
                        float parse = float.Parse(constructString);
                        constructString = parse.ToString("F");
                        throw new Exception($"Your string {constructString} in exponential format!");
                    }

                    int firstLetterIdx = constructString.IndexOfAny(_firstLetters);
                    if (firstLetterIdx != -1)
                    {
                        string letterPartOfString = constructString.Substring(firstLetterIdx);
                        int sumOfRanks = LettersToPower(letterPartOfString);
                        constructString = constructString.Substring(0, firstLetterIdx);

                        constructString = $"{constructString}e{sumOfRanks}";
                    }
                }


                string[] splitedString = constructString.Split('e');
                string valueString = splitedString[0];

                string rankString = "0";
                if (splitedString.Length > 1)
                {
                    rankString = splitedString[1];
                }

                rank = int.Parse(rankString);
                char[] separators = new char[] {'.', ','};
                if (valueString.IndexOfAny(separators) != -1)
                {
                    string[] splitedNum = constructString.Split(separators);
                    int decimalLength = splitedNum[1].Length;
                    long decimalValue = long.Parse(splitedNum[1]);

                    rank -= decimalLength;

                    value = (long) (long.Parse(splitedNum[0]) * System.Math.Pow(10, decimalLength));
                    value += decimalValue;
                }
                else
                {
                    var topLength = TOP_TRESHOLD.ToString().Length;

                    if (valueString.Length > topLength)
                    {
                        rank += valueString.Length - topLength;
                        valueString = valueString.Substring(0, topLength);
                    }
                    
                    value = long.Parse(valueString);
                }
            }
            catch (Exception e)
            {
                value = 0;
                rank = 0;
                Debug.LogError(e);
                Debug.LogError(constructString);
            }

            CheckThreshold();
        }

        public int ToInt32()
        {
            long res = value;

            var step = rank;
            while (step > 0)
            {
                if (res >= int.MaxValue)
                {
                    res = int.MaxValue;
                    break;
                }

                res *= 10;
                --step;
            }

            while (step < 0)
            {
                if (res == 0)
                {
                    break;
                }

                res /= 10;
                ++step;
            }

            return (int)res;
        }

        [DataMember] private long value;
        [DataMember] private long rank;

        public long Value => value;
        public long Rank => rank;

        private void CheckThreshold()
        {
            while (System.Math.Abs(value) > TOP_TRESHOLD)
            {
                value /= 10;
                rank++;
            }
        }

        public static BigNumber Floor(BigNumber num)
        {
            BigNumber result = num;

            while (result.rank < 0)
            {
                result.value /= 10;
                result.rank++;
            }

            return result;
        }

        public static BigNumber RoundLastDigit(BigNumber num)
        {
            BigNumber result = num;

            if (result.value % 10 > 4)
            {
                result.value += 10;
            }

            result.value /= 10;
            result.rank++;
            return result;
        }

        public static BigNumber Ceil(BigNumber num)
        {
            BigNumber result = num;

            while (result.rank < 0)
            {
                if (num % 10 > 0)
                {
                    num += 10;
                }

                result.value /= 10;
                result.rank++;
            }

            return result;
        }

        public static BigNumber Pow(BigNumber num, int pow)
        {
            BigNumber result = new BigNumber("1");
            if (pow < 0)
            {
                num = 1 / num;
                pow = -pow;
            }

            for (int i = 0; i < pow; i++)
            {
                result *= num;
            }

            return result;
        }

        public static BigNumber Min(BigNumber a, BigNumber b)
        {
            return a < b ? a : b;
        }

        public static BigNumber Max(BigNumber a, BigNumber b)
        {
            return a > b ? a : b;
        }

        public static BigNumber operator +(BigNumber a) => a;
        public static BigNumber operator -(BigNumber a) => new BigNumber(-a.value, a.rank);
        public static BigNumber operator +(BigNumber a, int b) => a + new BigNumber(b.ToString());

        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            if (a.rank == b.rank)
            {
                return new BigNumber(a.value + b.value, a.rank);
            }

            while ((a.value >= 0 ? a.value : -a.value) <= TOP_TRESHOLD / 10 && a.rank > -4)
            {
                a.value *= 10;
                a.rank--;
            }

            while ((b.value >= 0 ? b.value : -b.value) <= TOP_TRESHOLD / 10 && b.rank > -4)
            {
                b.value *= 10;
                b.rank--;
            }

            if (a.rank > b.rank)
            {
                long bValue = b.value;
                for (int i = 0; i < (a.rank - b.rank); ++i)
                {
                    bValue /= 10;
                }

                return new BigNumber(a.value + bValue, a.rank);
            }
            else
            {
                long aValue = a.value;
                for (int i = 0; i < (b.rank - a.rank); ++i)
                {
                    aValue /= 10;
                }

                return new BigNumber(aValue + b.value, b.rank);
            }
        }

        public static BigNumber operator -(BigNumber a, BigNumber b)
            => a + (-b);

        public static BigNumber operator *(BigNumber a, BigNumber b)
        {
            return new BigNumber(a.value * b.value, a.rank + b.rank);
        }


        public static BigNumber operator /(BigNumber a, BigNumber b)
        {
            if (b.value == 0)
            {
                throw new DivideByZeroException();
            }

            long value = a.value;
            long rank = a.rank - b.rank;

            float result = value / (float) b.value;
            while ((int) result < 10000 && rank > -4)
            {
                result *= 10;
                rank--;
            }

            BigNumber res = new BigNumber((int) result, rank);


            return res;
        }

        public static float operator %(BigNumber a, BigNumber b)
        {
            if (b.value == 0)
            {
                throw new DivideByZeroException();
            }

            long value = a.value;
            long rank = a.rank - b.rank;

            float result = value / (float) b.value;

            while (rank < 0)
            {
                result /= 10;
                ++rank;
            }

            while (rank > 0)
            {
                result *= 10;
                --rank;
            }

            return result;
        }

        public static BigNumber operator *(BigNumber a, int b)
        {
            BigNumber result = new BigNumber(a.value * b, a.rank);
            result.CheckThreshold();
            return result;
        }

        public static BigNumber operator *(int a, BigNumber b)
            => b * a;

        public static BigNumber operator *(BigNumber a, float b)
        {
            long rank = a.rank;

            while (b < 1000 && rank > -4)
            {
                b *= 10;
                rank--;
            }

            return new BigNumber((int) (a.value * b), rank);
        }

        public static implicit operator BigNumber(int value)
        {
            return new BigNumber(value.ToString());
        }

        public static BigNumber operator *(float a, BigNumber b)
            => b * a;

        public static bool operator ==(BigNumber a, BigNumber b)
        {
            return (a - b).value == 0;
        }

        public static bool operator !=(BigNumber a, BigNumber b) =>
            !(a == b);

        public static bool operator >(BigNumber a, BigNumber b) =>
            (a - b).value > 0;

        public static bool operator <(BigNumber a, BigNumber b) =>
            (a - b).value < 0;

        public static bool operator >=(BigNumber a, BigNumber b)
        {
            return (a - b).value >= 0;
        }

        public static bool operator >=(BigNumber a, int b)
        {
            return a >= new BigNumber(b.ToString());
        }

        public static bool operator <=(BigNumber a, int b)
        {
            return a <= new BigNumber(b.ToString());
        }

        public static bool operator <=(BigNumber a, BigNumber b)
        {
            return (a - b).value <= 0;
        }


        public override string ToString()
        {
            return ToString("{0}");
        }

        public string ToString(string floatFormat)
        {
            var tmpVal = value;
            var tmpRank = rank;
            while (tmpRank < 0)
            {
                tmpVal /= 10;
                tmpRank++;
            }

            if (tmpVal == 0) return "0";
            var valueCount = tmpVal.ToString().Length;
            var digitsNum = valueCount + tmpRank;

            if (valueCount < 3 && tmpRank == 0)
            {
                return string.Format(CultureInfo.InvariantCulture, floatFormat, tmpVal);
            }

            if (valueCount + tmpRank - 1 < 3 && tmpRank >= 0)
            {
                while (tmpRank > 0)
                {
                    tmpVal *= 10;
                    tmpRank--;
                }

                return string.Format(CultureInfo.InvariantCulture, floatFormat, tmpVal);
            }

            if (tmpRank < 0 && valueCount < -tmpRank)
            {
                float res = tmpVal;
                long numRank = tmpRank;

                while (numRank < 0)
                {
                    res /= 10;
                    numRank++;
                }

                return string.Format(CultureInfo.InvariantCulture, floatFormat, res);
            }

            var charValue = CreateCharValue(digitsNum - 1);

            digitsNum = digitsNum % 3;
            if (digitsNum == 0)
                digitsNum = 3;

            var tenNum = valueCount - digitsNum;
            var divider = (float) System.Math.Pow(10, tenNum);
            var numValue = (float) System.Math.Round(tmpVal / divider, 2);

            string numStr = string.Format(CultureInfo.InvariantCulture, floatFormat, numValue);
            return $"{numStr}{charValue}";
        }


        private string CreateCharValue(long digitsNum)
        {
            if (digitsNum <= 78)
            {
                long index = digitsNum < 3 ? 0 : (digitsNum / 3) - 1;
                return _firstLetters[index].ToString();
            }


            digitsNum = digitsNum / 3;
            var letters = "";

            while (digitsNum > 0)
            {
                var mod = (digitsNum - 1) % 26;
                letters = (char) (65 + mod) + letters;
                digitsNum = (digitsNum - mod) / 26;
            }

            return letters;
        }
    }
}
