namespace NeuralNetwork.Data
{
    public class LearningData
    {
        public NetworkData Input { get; set; }
        public NetworkData ExpectedOutput { get; set; }

        public LearningData(NetworkData input, NetworkData expectedOutput)
        {
            Input = input;
            ExpectedOutput = expectedOutput;
        }
    }
}
