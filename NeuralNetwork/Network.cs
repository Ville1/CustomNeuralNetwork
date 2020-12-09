using NeuralNetwork.Data;
using NeuralNetwork.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace NeuralNetwork
{
    public class Network
    {
        public List<Neuron> Inputs { get; private set; }
        public List<List<Neuron>> Hidden { get; private set; }
        public List<Neuron> Outputs { get; private set; }
        public List<Connection> Connections { get; private set; }
        public ActivationFunctionType ActivationFunctionType { get; private set; }
        public long NeuronCurrentId { get { long value = neuronCurrentId; neuronCurrentId++; return value; } }
        public long ConnectionCurrentId { get { long value = connectionCurrentId; connectionCurrentId++; return value; } }

        private float learningRate;
        private List<LearningData> learningData;
        private int learningDataRepeats;
        private int learningDataCycle;
        private long neuronCurrentId;
        private long connectionCurrentId;

        private Network() { }

        public Network(int inputCount, int hiddenWidth, int layers, int outputCount, float learningRate, ActivationFunctionType activationFunctionType)
        {
            if(inputCount <= 0) {
                throw new ArgumentException("Invalid argument inputCount");
            }
            if (hiddenWidth <= 0) {
                throw new ArgumentException("Invalid argument hiddenWidth");
            }
            if (layers <= 0) {
                throw new ArgumentException("Invalid argument layers");
            }
            if (outputCount <= 0) {
                throw new ArgumentException("Invalid argument inputCount");
            }
            if (learningRate <= 0.0f) {
                throw new ArgumentException("Invalid argument learningRate");
            }

            this.learningRate = learningRate;
            ActivationFunctionType = activationFunctionType;
            learningData = null;
            learningDataRepeats = 0;
            learningDataCycle = 0;
            neuronCurrentId = 0;
            connectionCurrentId = 0;

            Inputs = new List<Neuron>();
            Hidden = new List<List<Neuron>>();
            Outputs = new List<Neuron>();
            Connections = new List<Connection>();

            for (int i = 0; i < inputCount; i++) {
                Inputs.Add(new Neuron(this));
            }

            for (int i = 0; i < layers; i++) {
                Hidden.Add(new List<Neuron>());
                for (int j = 0; j < hiddenWidth; j++) {
                    Neuron neuron = new Neuron(this);
                    Hidden[i].Add(neuron);
                }
            }
            Hidden.Add(new List<Neuron>());
            for (int i = 0; i < outputCount; i++) {
                Hidden[Hidden.Count - 1].Add(new Neuron(this));
            }

            Random rng = new Random();
            foreach (Neuron input in Inputs) {
                foreach (Neuron layer0 in Hidden[0]) {
                    Connection connection = new Connection(input, layer0, rng.Next(100) * 0.01f);
                    Connections.Add(connection);
                }
            }

            List<Neuron> previousLayer = null;
            foreach (List<Neuron> layer in Hidden) {
                if (previousLayer != null) {
                    foreach (Neuron neuron in layer) {
                        foreach (Neuron previous in previousLayer) {
                            Connection connection = new Connection(previous, neuron, rng.Next(100) * 0.01f);
                            Connections.Add(connection);
                        }
                    }
                }
                previousLayer = layer;
            }

            foreach (Neuron neuron in Hidden[Hidden.Count - 1]) {
                Outputs.Add(neuron);
            }
            Hidden.RemoveAt(Hidden.Count - 1);
        }

        public float ActivationFunction(float input)
        {
            switch (ActivationFunctionType) {
                case ActivationFunctionType.Sigmoid:
                    return MathFunctions.Sigmoid(input);
                default:
                    throw new Exception(string.Format("ActivationFunctionType {0} is missing implementation", ActivationFunctionType));
            }
        }

        public float ActivationFunctionDerivate(float input)
        {
            switch (ActivationFunctionType) {
                case ActivationFunctionType.Sigmoid:
                    return MathFunctions.SigmoidDerivate(input);
                default:
                    throw new Exception(string.Format("ActivationFunctionType {0} is missing implementation", ActivationFunctionType));
            }
        }

        public void StartTeaching(List<LearningData> data, int repeats)
        {
            if (repeats <= 0) {
                throw new ArgumentException("Invalid number of repeats");
            }
            learningData = data;
            learningDataRepeats = repeats;
            learningDataCycle = 0;
        }

        public float Teach()
        {
            if(learningData == null || learningDataRepeats <= 0) {
                throw new Exception("StartTeaching needs to be called before parameterless Teach is called");
            }
            if(learningDataCycle >= learningDataRepeats) {
                return 1.0f;
            }
            foreach(LearningData data in learningData) {
                Teach(data.Input.RawValues, data.ExpectedOutput.RawValues);
            }
            learningDataCycle++;
            return learningDataCycle / (float)learningDataRepeats;
        }

        public void Teach(List<LearningData> data, int repeats)
        {
            if(repeats <= 0) {
                throw new ArgumentException("Invalid number of repeats");
            }
            for(int i = 0; i < repeats; i++) {
                foreach(LearningData learningData in data) {
                    Teach(learningData.Input.RawValues, learningData.ExpectedOutput.RawValues);
                }
            }
        }

        public NetworkData Teach(LearningData data)
        {
            return Teach(data.Input.RawValues, data.ExpectedOutput.RawValues);
        }

        public NetworkData Teach(List<float> inputValues, List<float> expectedOutputs)
        {
            if (inputValues.Count != Inputs.Count) {
                throw new ArgumentException(string.Format("Invalid number of input values, {0} was given while {1} expected", inputValues.Count, Inputs.Count));
            }
            if (expectedOutputs.Count != Outputs.Count) {
                throw new ArgumentException(string.Format("Invalid number of output values, {0} was given while {1} expected", inputValues.Count, Inputs.Count));
            }

            List<float> results = Process(inputValues).RawValues;

            float totalError = 0.0f;
            float delta = 0.0f;
            for (int i = 0; i < expectedOutputs.Count; i++) {
                totalError += (0.5f * (float)Math.Pow(expectedOutputs[i] - results[i], 2.0d));
                delta += (results[i] - expectedOutputs[i]);
            }
            if (totalError == 0.0f) {
                return new NetworkData(results);
            }

            //Calculate errors
            Dictionary<Neuron, float> errors = new Dictionary<Neuron, float>();
            for (int i = 0; i < Outputs.Count; i++) {
                float error = (expectedOutputs[i] - Outputs[i].Output.Value) * ActivationFunctionDerivate(Outputs[i].Output.Value);
                errors.Add(Outputs[i], error);
            }
            for (int layerIndex = Hidden.Count - 1; layerIndex >= 0; layerIndex--) {
                List<Neuron> layer = Hidden[layerIndex];
                foreach (Neuron neuron in layer) {
                    float error = 0.0f;
                    foreach (Connection connection in neuron.Connections) {
                        if (connection.Out.Id != neuron.Id) {
                            error += connection.Weight * errors[connection.Out];
                        }
                    }
                    errors.Add(neuron, error * ActivationFunctionDerivate(neuron.Output.Value));
                }
            }

            //Update weights
            foreach (Connection connection in Connections) {
                connection.Weight = connection.Weight + (learningRate * errors[connection.Out] * connection.In.Output.Value);
            }

            return new NetworkData(results);
        }

        public NetworkData Process(NetworkData input)
        {
            return Process(input.RawValues);
        }

        public NetworkData Process(float input)
        {
            return Process(new List<float>() { input });
        }

        public NetworkData Process(List<float> inputValues)
        {
            if (inputValues.Count != Inputs.Count) {
                throw new ArgumentException(string.Format("Invalid number of input values, {0} was given while {1} expected", inputValues.Count, Inputs.Count));
            }
            foreach(float inputValue in inputValues) {
                if(inputValue < 0.0f || inputValue > 1.0f) {
                    throw new ArgumentException(string.Format("Invalid input value {0}, value must be between 0.0 and 1.0", inputValue));
                }
            }

            for (int i = 0; i < inputValues.Count; i++) {
                Inputs[i].Process(inputValues[i]);
            }

            foreach (List<Neuron> layer in Hidden) {
                foreach (Neuron neuron in layer) {
                    neuron.Process();
                }
            }

            List<float> result = new List<float>();
            foreach (Neuron neuron in Outputs) {
                neuron.Process();
                result.Add(neuron.Output.Value);
            }

            return new NetworkData(result);
        }

        public static void Save(Network network, string file)
        {
            NetworkSaveData data = new NetworkSaveData() {
                ActivationFunctionType = (int)network.ActivationFunctionType,
                NeuronCurrentId = network.NeuronCurrentId,
                ConnectionCurrentId = network.ConnectionCurrentId,
                Inputs = network.Inputs.Select(x => new NeuronSaveData() { Id = x.Id }).ToList(),
                Hidden = network.Hidden.Select(x => x.Select(y => new NeuronSaveData() { Id = y.Id }).ToList()).ToList(),
                Output = network.Outputs.Select(x => new NeuronSaveData() { Id = x.Id }).ToList(),
                Connections = network.Connections.Select(x => new ConnectionSaveData() { Id = x.Id, InId = x.In.Id, OutId = x.Out.Id, Weight = x.Weight }).ToList()
            };
            try {
                File.WriteAllText(file, JsonConvert.SerializeObject(data, Formatting.None));
            } catch (Exception exception) {
                //TODO: Logging?
                throw exception;
            }
        }

        public static Network Load(string file)
        {
            NetworkSaveData data = null;
            try {
                data = JsonConvert.DeserializeObject<NetworkSaveData>(File.ReadAllText(file));
            } catch (Exception exception) {
                //TODO: Logging?
                throw exception;
            }
            Network network = new Network();
            network.ActivationFunctionType = (ActivationFunctionType)data.ActivationFunctionType;
            network.neuronCurrentId = data.NeuronCurrentId;
            network.connectionCurrentId = data.ConnectionCurrentId;
            network.Inputs = data.Inputs.Select(x => new Neuron(network, x)).ToList();
            network.Hidden = data.Hidden.Select(x => x.Select(y => new Neuron(network, y)).ToList()).ToList();
            network.Outputs = data.Output.Select(x => new Neuron(network, x)).ToList();
            List<Neuron> all = network.Inputs.Select(x => x).ToList();
            foreach(List<Neuron> column in network.Hidden) {
                all.AddRange(column);
            }
            all.AddRange(network.Outputs);
            network.Connections = data.Connections.Select(x => new Connection(all.First(y => y.Id == x.InId), all.First(y => y.Id == x.OutId), x)).ToList();
            return network;
        }
    }
}
