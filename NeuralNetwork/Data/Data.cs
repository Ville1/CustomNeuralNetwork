using NeuralNetwork.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork.Data
{
    public class Data
    {
        public List<float> RawValues { get; set; }

        public Data(List<float> rawValues)
        {
            RawValues = Helper.CloneList(rawValues);
        }

        public Data(List<bool> rawValues)
        {
            RawValues = rawValues.Select(x => x ? 1.0f : 0.0f).ToList();
        }
        public Data(Bits bits)
        {
            RawValues = bits.RawValues.Select(x => x ? 1.0f : 0.0f).ToList();
        }

        public Bits BitValues
        {
            get {
                return new Bits(RawValues.Select(x => Math.Round(x) == 1.0d).ToList());
            }
        }
    }
}
