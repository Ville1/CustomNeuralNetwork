using NeuralNetwork.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    public class Neuron
    {
        public enum NeuronType { Input, Hidden, Output }

        private static int currentId = 0;

        public int Id { get; private set; }
        public List<Connection> Connections { get; private set; }
        public float? Output { get; private set; }
        public Network Network { get; private set; }

        public NeuronType Type
        {
            get {
                if (!Connections.Any(x => x.Out.Id == Id) && Connections.Any(x => x.In.Id == Id))
                    return NeuronType.Input;
                if (Connections.Any(x => x.Out.Id == Id) && !Connections.Any(x => x.In.Id == Id))
                    return NeuronType.Output;
                return NeuronType.Hidden;
            }
        }

        public Neuron(Network network)
        {
            Id = currentId;
            currentId++;
            Connections = new List<Connection>();
            Output = null;
            Network = network;
        }

        public void Process(float? inputValue = null)
        {
            if (inputValue.HasValue && Type != NeuronType.Input) {
                throw new ArgumentException();
            }

            float sum = inputValue.HasValue ? inputValue.Value : 0.0f;
            foreach (Connection connection in Connections) {
                if (connection.Out.Id == Id) {
                    if (!connection.In.Output.HasValue) {
                        throw new Exception("Previous layer is not fully processed");
                    }
                    sum += connection.In.Output.Value * connection.Weight;
                }
            }

            Output = Network.ActivationFunction(sum);
        }

        public override string ToString()
        {
            return string.Format("Neuron #{0} ({1})", Id, Type);
        }
    }
}
