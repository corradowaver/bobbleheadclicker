using UnityEngine;

namespace Core.Math
{
    public static class BigUtility
    {
        public static BigNumber RandomRange(BigNumber min, BigNumber max)
        {
            float rand = Random.value;
            return Lerp(min,max, rand);
        }

        public static BigNumber Lerp(BigNumber a, BigNumber b, float t)
        {
            t = Mathf.Clamp(t, 0f, 1f);
            BigNumber bigT = new BigNumber(t.ToString("F"));
            return (BigNumber.One - bigT) * a + bigT * b;
        }

        public static BigNumber CeilTo(BigNumber val, int to)
        {
            float value = val % to;
            float round = Mathf.Ceil(value);
            float rez = round * to;
            return new BigNumber(rez.ToString("F"));
        }
        
        public static BigNumber FloorTo(BigNumber val, int to)
        {
            float value = val % to;
            float round = Mathf.Floor(value);
            float rez = round * to;
            return new BigNumber(rez.ToString("F"));
        }
        
        public static BigNumber RoundTo(BigNumber val, int to)
        {
            float value = val % to;
            float round = Mathf.Round(value);
            float rez = round * to;
            return new BigNumber(rez.ToString("F"));
        }
    }
}