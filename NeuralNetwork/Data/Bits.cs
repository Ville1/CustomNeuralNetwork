using NeuralNetwork.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork.Data
{
    public class Bits : IEnumerable<bool>
    {
        public List<bool> RawValues { get; private set; }

        public Bits(List<bool> rawValues)
        {
            RawValues = Helper.CloneList(rawValues);
        }

        public Bits(string s)
        {
            RawValues = new List<bool>();
            for(int i = 0; i < s.Length; i++) {
                if(s[i] == '1') {
                    RawValues.Add(true);
                } else if(s[i] == '0') {
                    RawValues.Add(false);
                } else {
                    throw new ArgumentException("String must only contain ones and zeros");
                }
            }
        }

        public IEnumerator<bool> GetEnumerator()
        {
            return RawValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return RawValues.GetEnumerator();
        }

        public bool this[int index]
        {
            get {
                return RawValues[index];
            }
            set {
                RawValues[index] = value;
            }
        }

        public override string ToString()
        {
            return new string(RawValues.Select(x => x ? '1' : '0').ToArray());
        }
    }
}
