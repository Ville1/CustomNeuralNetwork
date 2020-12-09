using NeuralNetwork.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.Data
{
    public class NetworkData
    {
        private static readonly int BITS = 8;
        private static readonly char INVALID_CHAR = '?';

        public List<float> RawValues { get; set; }

        public NetworkData(List<float> rawValues)
        {
            RawValues = Helper.CloneList(rawValues);
        }

        public NetworkData(List<bool> rawValues)
        {
            RawValues = rawValues.Select(x => x ? 1.0f : 0.0f).ToList();
        }

        public NetworkData(char c)
        {
            RawValues = Helper.ToBits(Encoding.UTF8.GetBytes(c.ToString())[0], BITS).RawValues.Select(x => x ? 1.0f : 0.0f).ToList();
        }

        public NetworkData(int i)
        {
            RawValues = Helper.ToBits(i, BITS).RawValues.Select(x => x ? 1.0f : 0.0f).ToList();
        }

        public NetworkData(Bits bits)
        {
            RawValues = bits.RawValues.Select(x => x ? 1.0f : 0.0f).ToList();
        }

        public Bits BitValues
        {
            get {
                return new Bits(RawValues.Select(x => Math.Round(x) == 1.0d).ToList());
            }
        }

        public int ParseInt()
        {
            return Helper.IntFromBits(BitValues);
        }

        public char ParseChar()
        {
            int utfCode = Helper.IntFromBits(BitValues);
            if (utfCode < byte.MinValue || utfCode > byte.MaxValue) {
                return INVALID_CHAR;
            }
            return Encoding.UTF8.GetString(new byte[1] { (byte)utfCode })[0];
        }
    }
}
