using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Domains.Sequence
{
    class SequenceStep
    {
        public double[] Input;
        public double[] Output;
        public double Weight;
        public SequenceStep(double[] input, double[] output, double weight)
        {
            Input = input;
            Output = output;
            Weight = weight;
        }

        public override string ToString()
        {
            string str = "";
            foreach (float f in Input)
                str += f.ToString("f3") + " ";
            str += "| ";

            foreach (float f in Output)
                str += f.ToString("f3") + " ";
            str += "; ";

            str += "weight=" + Weight.ToString("f6");

            return str;
        }
    }

    class SequenceTest : List<SequenceStep>
    {
        public string Name;
        public double MaxError
        {
            get
            {
                double sum = 0.0;
                foreach(SequenceStep step in this)
                    sum += step.Weight * step.Output.Length;
                return sum;
            }
        }
        public override string ToString()
        {
            string str = "~~~" + Name;
            foreach (SequenceStep step in this)
                str += step.ToString() + "\r\n";

            return str;
        }
    }


    class SequenceEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        const double StopFitness = 0.99;
        private ulong _evalCount;
        private bool _stopConditionSatisfied;
        private double _maxError;


        private List<SequenceTest> _testList;

        public SequenceEvaluator()
        {
            _testList = CreateTests();

            foreach (SequenceTest test in _testList)
                _maxError += test.MaxError;
        }

        #region IPhenomeEvaluator<IBlackBox>

        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            double errorSum = 0;
            double fitness = 0;
            _evalCount++;
            double[] realOutput = new double[_testList[0][0].Output.Length];

            foreach(SequenceTest test in _testList)
            {
                phenome.ResetState();
                for (int i = 0; i < test.Count; i++)
                {
                    SequenceStep step = test[i];
                    phenome.InputSignalArray.CopyFrom(step.Input, 0);
                    phenome.Activate();

                    phenome.OutputSignalArray.CopyTo(realOutput, 0);
                    errorSum += EvaluatError(step.Output, realOutput) * step.Weight;
                }
            }

            fitness = 1.0 - errorSum / _maxError;

            if (fitness > StopFitness)
                _stopConditionSatisfied = true;

            return new FitnessInfo(fitness, fitness);
        }

        public void Reset()
        {
        }

        #endregion

        private List<SequenceTest> CreateTests()
        {
            int bitLen = 4;
            int len = (int)Math.Pow(2,bitLen);
            byte bitTest = 0;
            List<SequenceTest>  testList = new List<SequenceTest>();
            for(int i=0;i<len;i++)
            {
                SequenceTest test = new SequenceTest();
                double[] output = new double[bitLen];
                for (int j = bitLen-1; j>=0; j--)
                {
                    if((bitTest & (0x1<<j)) != 0)
                    {
                        test.Add(new SequenceStep(new double[] { 1, 0, 1 }, new double[] { 0, 0, 0, 0 }, 5.0));
                        test.Add(new SequenceStep(new double[] { 0, 0, 0 }, new double[] { 0, 0, 0, 0 }, 5.0));
                        output[bitLen-1-j] = 1;
                    }
                    else
                    {
                        test.Add(new SequenceStep(new double[] { 1, 0, 0 }, new double[] { 0, 0, 0, 0 }, 5.0));
                        test.Add(new SequenceStep(new double[] { 0, 0, 0 }, new double[] { 0, 0, 0, 0 }, 5.0));
                        output[bitLen - 1 - j] = 0;
                    }
                }

                test.Add(new SequenceStep(new double[] { 0, 1, 0 }, output, 50));
                testList.Add(test);
                bitTest++;
            }

            return testList;
        }
        private double EvaluatError(double[] expected, double[] actual)
        {
            double errorSum = 0;
            double error;
            for(int i=0;i<actual.Length;i++)
            {
                error = Math.Abs(actual[i] - expected[i]);
                if( error < 0.05)
                    error = 0;
                errorSum += error;
            }
            return errorSum;
        }
    }
}
