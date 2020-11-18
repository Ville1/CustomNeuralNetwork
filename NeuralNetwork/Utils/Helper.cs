using NeuralNetwork.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Utils
{
    public class Helper
    {
        public static List<T> CloneList<T>(List<T> list)
        {
            List<T> clone = new List<T>();
            foreach (T item in list) {
                clone.Add(item);
            }
            return clone;
        }

        public static Bits ToBits(int integer, int bitsCount)
        {
            string bits = Convert.ToString(integer, 2).PadLeft(bitsCount, '0');
            if (bits.Length > bitsCount) {
                bits = bits.Substring(bits.Length - bitsCount);
            }
            List<bool> list = new List<bool>();
            for (int i = 0; i < bits.Length; i++) {
                list.Add(bits[i] == '1');
            }
            return new Bits(list);
        }

        public static int IntFromBits(Bits bits)
        {
            StringBuilder builder = new StringBuilder();
            foreach(bool bit in bits.RawValues) {
                builder.Append(bit ? "1" : "0");
            }
            return Convert.ToInt32(builder.ToString(), 2);
        }
    }
}
