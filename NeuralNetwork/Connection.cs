using NeuralNetwork.Data;

namespace NeuralNetwork
{
    public class Connection
    {
        public long Id { get; private set; }
        public Neuron In { get; private set; }
        public Neuron Out { get; private set; }
        public float Weight { get; set; }

        public Connection(Neuron inP, Neuron outP, float weight)
        {
            Id = inP.Network.ConnectionCurrentId;
            inP.Network.ConnectionCurrentId++;
            In = inP;
            Out = outP;
            In.Connections.Add(this);
            Out.Connections.Add(this);
            Weight = weight;
        }

        public Connection(Neuron inP, Neuron outP, ConnectionSaveData data)
        {
            Id = data.Id;
            In = inP;
            Out = outP;
            In.Connections.Add(this);
            Out.Connections.Add(this);
            Weight = data.Weight;
        }
    }
}
