using NeuralNetwork.Utils;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Data
{
    public class Input : Data
    {
        public Input(List<float> rawValues) : base(rawValues)
        { }

        public Input(char c) : base(Helper.ToBitList((int)Encoding.UTF8.GetBytes(c.ToString())[0], 8))
        { }
    }
}
