using System;
using System.Collections.Generic;
using NeuralNetwork;
using NeuralNetwork.Data;

namespace CustomNeuralNetwork
{
    class Program
    {
        private static readonly string APLHABET = "abcdefghijklmnopqrstuvwxyz";

        //Test app
        static void Main(string[] args)
        {
            Network network = new Network(8, 10, 2, 8, 1.0f, ActivationFunctionType.Sigmoid);

            Console.WriteLine("AI learns alphabet\n");
            Console.WriteLine("Fresh AI");

            Test(network);

            Console.WriteLine("Teaching...");
            int maxProgress = 10;
            int currentProgress = 0;
            List<LearningData> learningData = new List<LearningData>();

            for(int i = 0; i < APLHABET.Length; i++) {
                if(i == APLHABET.Length - 1) {
                    learningData.Add(new LearningData(new NetworkData(APLHABET[i]), new NetworkData(APLHABET[0])));
                } else {
                    learningData.Add(new LearningData(new NetworkData(APLHABET[i]), new NetworkData(APLHABET[i + 1])));
                }
            }

            network.StartTeaching(learningData, 10000);
            float progress = network.Teach();
            while (progress < 1.0f) {
                while(currentProgress < Math.Round(maxProgress * progress)) {
                    Console.Write("*");
                    currentProgress++;
                }
                progress = network.Teach();
            }


            Console.WriteLine(" Ready!");
            TestResults results = Analytics.Test(network, learningData);
            Console.WriteLine(string.Format("Success rate: {0}% ({1}/{2})", (int)(results.SuccessRate * 100.0f), results.SuccessfulTests, results.TotalTests));

            Test(network);

            //Network.Save(network, "C:\\Users\\Ville\\Documents\\Test\\test.json");

            /*Network fromFile = Network.Load("C:\\Users\\Ville\\Documents\\Test\\test.json");
            Test(fromFile);*/
        }

        private static void Test(Network network)
        {
            Console.WriteLine("\nPress a letter key and AI gives a letter after that one in alphabet");
            Console.WriteLine("Press any nonletter key to stop");

            bool running = true;
            while (running) {
                char keyChar = Console.ReadKey(true).KeyChar.ToString().ToLower()[0];
                if (APLHABET.Contains(keyChar)) {
                    NetworkData output = network.Process(new NetworkData(keyChar));
                    Console.WriteLine(string.Format("{0} -> {1}", keyChar, output.ParseChar()));
                } else {
                    running = false;
                }
            }
        }
    }
}
