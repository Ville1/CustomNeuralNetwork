using NeuralNetwork.Data;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class Analytics
    {
        public static TestResults Test(Network network, List<LearningData> data, bool bitTest = true, int decimals = 100)
        {
            TestResults testResults = new TestResults();
            foreach (LearningData learningData in data) {
                NetworkData output = network.Process(learningData.Input);
                bool success = true;
                for(int i = 0; i < output.RawValues.Count; i++) {
                    if (bitTest && output.BitValues[i] != learningData.ExpectedOutput.BitValues[i]) {
                        success = false;
                        break;
                    } else if(!bitTest) {
                        int intOutValue = (int)(output.RawValues[i] * decimals);
                        int intExpectedValue = (int)(learningData.ExpectedOutput.RawValues[i] * decimals);
                        if(intOutValue != intExpectedValue) {
                            success = false;
                            break;
                        }
                    }
                }
                if (success) {
                    testResults.SuccessfulTests++;
                } else {
                    testResults.FailedTests++;
                }
            }
            return testResults;
        }
    }
}
