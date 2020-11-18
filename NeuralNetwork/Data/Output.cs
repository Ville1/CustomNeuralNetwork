using NeuralNetwork.Utils;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Data
{
    public class Output : Data
    {
        private static readonly int BITS = 8;
        private static readonly char INVALID_CHAR = '?';

        public Output(List<float> rawValues) : base(rawValues)
        { }

        public Output(char c) : base(Helper.ToBits((int)Encoding.UTF8.GetBytes(c.ToString())[0], BITS))
        { }

        public Output(int i) : base(Helper.ToBits(i, BITS))
        { }

        public int ParseInt()
        {
            return Helper.IntFromBits(BitValues);
        }

        public char ParseChar()
        {
            int utfCode = Helper.IntFromBits(BitValues);
            if(utfCode < byte.MinValue || utfCode > byte.MaxValue) {
                return INVALID_CHAR;
            }
            return Encoding.UTF8.GetString(new byte[1] { (byte)utfCode })[0];
        }
    }
}
