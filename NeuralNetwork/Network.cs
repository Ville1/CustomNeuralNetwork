using NeuralNetwork.Data;
using NeuralNetwork.Utils;
using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class Network
    {
        private List<Neuron> inputs = new List<Neuron>();
        private List<List<Neuron>> hidden = new List<List<Neuron>>();
        private List<Neuron> outputs = new List<Neuron>();
        private List<Connection> connections = new List<Connection>();
        private float learningRate;
        private List<LearningData> learningData;
        private int learningDataRepeats;
        private int learningDataCycle;
        public ActivationFunctionType ActivationFunctionType { get; private set; }

        public Network(int inputCount, int hiddenWidth, int layers, int outputCount, float learningRate, ActivationFunctionType activationFunctionType)
        {
            this.learningRate = learningRate;
            ActivationFunctionType = activationFunctionType;
            learningData = null;
            learningDataRepeats = 0;
            learningDataCycle = 0;

            for (int i = 0; i < inputCount; i++) {
                inputs.Add(new Neuron(this));
            }

            for (int i = 0; i < layers; i++) {
                hidden.Add(new List<Neuron>());
                for (int j = 0; j < hiddenWidth; j++) {
                    Neuron neuron = new Neuron(this);
                    hidden[i].Add(neuron);
                }
            }
            hidden.Add(new List<Neuron>());
            for (int i = 0; i < outputCount; i++) {
                hidden[hidden.Count - 1].Add(new Neuron(this));
            }

            Random rng = new Random();
            foreach (Neuron input in inputs) {
                foreach (Neuron layer0 in hidden[0]) {
                    Connection connection = new Connection(input, layer0, rng.Next(100) * 0.01f);
                    connections.Add(connection);
                }
            }

            List<Neuron> previousLayer = null;
            foreach (List<Neuron> layer in hidden) {
                if (previousLayer != null) {
                    foreach (Neuron neuron in layer) {
                        foreach (Neuron previous in previousLayer) {
                            Connection connection = new Connection(previous, neuron, rng.Next(100) * 0.01f);
                            connections.Add(connection);
                        }
                    }
                }
                previousLayer = layer;
            }

            foreach (Neuron neuron in hidden[hidden.Count - 1]) {
                outputs.Add(neuron);
            }
            hidden.RemoveAt(hidden.Count - 1);
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

        public void Teach(List<float> inputValues, List<float> expectedOutputs)
        {
            if (inputValues.Count != inputs.Count) {
                throw new ArgumentException(string.Format("Invalid number of input values, {0} was given while {1} expected", inputValues.Count, inputs.Count));
            }
            if (expectedOutputs.Count != outputs.Count) {
                throw new ArgumentException(string.Format("Invalid number of output values, {0} was given while {1} expected", inputValues.Count, inputs.Count));
            }

            List<float> results = Process(inputValues).RawValues;

            float totalError = 0.0f;
            float delta = 0.0f;
            for (int i = 0; i < expectedOutputs.Count; i++) {
                totalError += (0.5f * (float)Math.Pow(expectedOutputs[i] - results[i], 2.0d));
                delta += (results[i] - expectedOutputs[i]);
            }
            if (totalError == 0.0f) {
                return;
            }

            //Calculate errors
            Dictionary<Neuron, float> errors = new Dictionary<Neuron, float>();
            for (int i = 0; i < outputs.Count; i++) {
                float error = (expectedOutputs[i] - outputs[i].Output.Value) * ActivationFunctionDerivate(outputs[i].Output.Value);
                errors.Add(outputs[i], error);
            }
            for (int layerIndex = hidden.Count - 1; layerIndex >= 0; layerIndex--) {
                List<Neuron> layer = hidden[layerIndex];
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
            foreach (Connection connection in connections) {
                connection.Weight = connection.Weight + (learningRate * errors[connection.Out] * connection.In.Output.Value);
            }
        }

        public Output Process(Input input)
        {
            return Process(input.RawValues);
        }

        public Output Process(float input)
        {
            return Process(new List<float>() { input });
        }

        public Output Process(List<float> inputValues)
        {
            if (inputValues.Count != inputs.Count) {
                throw new ArgumentException(string.Format("Invalid number of input values, {0} was given while {1} expected", inputValues.Count, inputs.Count));
            }
            foreach(float inputValue in inputValues) {
                if(inputValue < 0.0f || inputValue > 1.0f) {
                    throw new ArgumentException(string.Format("Invalid input value {0}, value must be between 0.0 and 1.0", inputValue));
                }
            }

            for (int i = 0; i < inputValues.Count; i++) {
                inputs[i].Process(inputValues[i]);
            }

            foreach (List<Neuron> layer in hidden) {
                foreach (Neuron neuron in layer) {
                    neuron.Process();
                }
            }

            List<float> result = new List<float>();
            foreach (Neuron neuron in outputs) {
                neuron.Process();
                result.Add(neuron.Output.Value);
            }

            return new Output(result);
        }
    }
}
