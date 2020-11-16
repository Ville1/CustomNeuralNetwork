using System;

namespace NeuralNetwork.Utils
{
    public class MathFunctions
    {
        public static float Sigmoid(double value)
        {
            return (float)(1.0 / (1.0 + Math.Pow(Math.E, -value)));
        }

        public static float SigmoidDerivate(float value)
        {
            return value * (1.0f - value);
        }
    }
}
