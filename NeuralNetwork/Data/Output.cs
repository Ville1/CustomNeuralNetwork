using NeuralNetwork.Utils;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Data
{
    public class Output : Data
    {
        private static readonly char INVALID_CHAR = '?';

        public Output(List<float> rawValues) : base(rawValues)
        { }

        public Output(char c) : base(Helper.ToBitList((int)Encoding.UTF8.GetBytes(c.ToString())[0], 8))
        { }

        public char ParseChar()
        {
            int utfCode = Helper.IntFromBitList(BitValues);
            if(utfCode < byte.MinValue || utfCode > byte.MaxValue) {
                return INVALID_CHAR;
            }
            return Encoding.UTF8.GetString(new byte[1] { (byte)utfCode })[0];
        }
    }
}
