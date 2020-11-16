namespace NeuralNetwork.Data
{
    public class LearningData
    {
        public Input Input { get; set; }
        public Output ExpectedOutput { get; set; }

        public LearningData(Input input, Output expectedOutput)
        {
            Input = input;
            ExpectedOutput = expectedOutput;
        }
    }
}
