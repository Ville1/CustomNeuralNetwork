namespace NeuralNetwork
{
    public class Connection
    {
        private static int currentId = 0;

        public int Id { get; private set; }
        public Neuron In { get; private set; }
        public Neuron Out { get; private set; }
        public float Weight { get; set; }

        public Connection(Neuron inP, Neuron outP, float weight)
        {
            Id = currentId;
            currentId++;
            In = inP;
            Out = outP;
            In.Connections.Add(this);
            Out.Connections.Add(this);
            Weight = weight;
        }
    }
}
