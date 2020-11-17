using System;
using System.Collections.Generic;

namespace NeuralNetwork.Data
{
    [Serializable]
    public class NetworkSaveData
    {
        public int ActivationFunctionType { get; set; }
        public long NeuronCurrentId { get; set; }
        public long ConnectionCurrentId { get; set; }
        public List<NeuronSaveData> Inputs { get; set; }
        public List<List<NeuronSaveData>> Hidden { get; set; }
        public List<NeuronSaveData> Output { get; set; }
        public List<ConnectionSaveData> Connections { get; set; }
    }

    [Serializable]
    public class NeuronSaveData
    {
        public long Id { get; set; }
    }

    [Serializable]
    public class ConnectionSaveData
    {
        public long Id { get; set; }
        public long InId { get; set; }
        public long OutId { get; set; }
        public float Weight { get; set; }
    }
}
