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

        public static List<bool> ToBitList(int integer, int bitsCount)
        {
            string bits = Convert.ToString(integer, 2).PadLeft(bitsCount, '0');
            if (bits.Length > bitsCount) {
                bits = bits.Substring(bits.Length - bitsCount);
            }
            List<bool> list = new List<bool>();
            for (int i = 0; i < bits.Length; i++) {
                list.Add(bits[i] == '1');
            }
            return list;
        }

        public static int IntFromBitList(List<bool> list)
        {
            StringBuilder builder = new StringBuilder();
            foreach(bool bit in list) {
                builder.Append(bit ? "1" : "0");
            }
            return Convert.ToInt32(builder.ToString(), 2);
        }
    }
}
