using NeuralNetwork.Utils;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Data
{
    public class Input : Data
    {
        private static readonly int BITS = 8;

        public Input(List<float> rawValues) : base(rawValues)
        { }

        public Input(char c) : base(Helper.ToBits((int)Encoding.UTF8.GetBytes(c.ToString())[0], BITS))
        { }

        public Input(int i) : base(Helper.ToBits(i, BITS))
        { }
    }
}
