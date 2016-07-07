using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.NEAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject01.Util.DllTools
{
     [StructLayout(LayoutKind.Sequential)]
    struct FWTParam
	{
		public IntPtr input;
        public IntPtr output;
        public IntPtr temp;
        public int inputLength;

        public IntPtr h;
        public IntPtr g;
        public int filterLength;
	};

     [Serializable]
     abstract class BasicWavelet
    {
        public abstract double[] Scaling { get; }
        public abstract double[] Wavelet { get; }
        public int Digital { get { return Scaling.Length; } }
    }

    abstract class BasicDaubechiesWavelet : BasicWavelet
    {
        public override double[] Wavelet
        {
            get 
            {
                double[] wavelet = new double[Scaling.Length];
                double dir = -1;
                for(int i=0;i<wavelet.Length;i++)
                {

                    wavelet[i] = Scaling[Scaling.Length - 1 - i] * dir;
                    dir *= -1;
                }

                return wavelet;
            }
        }
    }
    [Serializable]
    class WaveletFunction : BasicWavelet
    {
        private double[] _scaling;
        private double[] _wavelet;

        public WaveletFunction(BasicWavelet wavelet)
        {
            _scaling = wavelet.Scaling;
            _wavelet = wavelet.Wavelet;
        }
        public override double[] Scaling
        {
            get { return _scaling; }
        }

        public override double[] Wavelet
        {
            get { return _wavelet; }
        }
    }
        class HaarWavelet : BasicWavelet
        {

            public override double[] Scaling
            {
                get
                {
                    return new double[] 
                    { 
                        1, 
                        1, 
                     };
                }
            }

            public override double[] Wavelet
            {
                get
                {
                    return new double[] 
                    { 
                        -1, 
                        1, 
                     };
                }
            }
        }
    class Daubechies8Wavelet : BasicDaubechiesWavelet
    {
        public override double[] Scaling
        {
            get 
            { 
                return new double[] 
                { 
                    0.32580343, 
                    1.01094572, 
                    0.89220014,
                    -0.03957503,
                    -0.26450717,
                    0.0436163,
                    0.0465036,
                    -0.01498699
                }; 
            }
        }
    }
#if true
    class Daubechies4Wavelet : BasicDaubechiesWavelet
    {
        public override double[] Scaling
        {
            get
            {
                return new double[] 
                { 
                    0.6830127, 
                    1.1830127, 
                    0.3169873,
                    -0.1830127
                };
            }
        }
    }
#else
    class Daubechies4Wavelet : BasicDaubechiesWavelet
    {
        public override double[] Scaling
        {
            get
            {
                return new double[] 
                { 
                    -0.1830127,
                    0.3169873,
                    0.6830127, 
                    1.1830127
                };
            }
        }
    }

#endif
    class Legendre6Wavelet : BasicDaubechiesWavelet
    {
        public override double[] Scaling
        {
            get
            {
                return new double[] 
                { 
                    0.348029119, 
                    0.19334951,
                    0.165728152,
                    0.165728152, 
                    0.19334951,
                    0.348029119
                };
            }
        }
    }
    class DllTools
    {
        private static double[] h = new double[]{.332670552950, .806891509311, .459877502118, -.135011020010,   
                    -.085441273882, .035226291882};
        private static double[] g = new double[]{.035226291882, .085441273882, -.135011020010, -.459877502118,  
                    .806891509311, -.332670552950};


        private static double[] haar_h = new double[] { 1,  1 };
        private static double[] haar_g = new double[] { -1, 1 };
        private static DllMemoryPoolCtrl _poolCtrl;
        
        static DllTools()
        {
            _poolCtrl = new DllMemoryPoolCtrl();
        }
        public static void FTW(double[] input, double[] output, double[] temp)
        {
            int level = 1;
            // Calculate level;
            while (true)
            {
                if (Math.Pow(2, level) > input.Length)
                    break;
                level++;
            }
            level--;

            lock (h)
            {
                if (false)
                {
                    FWTParam param;
                    // param.input = Marshal.UnsafeAddrOfPinnedArrayElement(input, 0);
                    // param.output = Marshal.UnsafeAddrOfPinnedArrayElement(output, 0);
                    // param.temp = Marshal.UnsafeAddrOfPinnedArrayElement(temp, 0);
                    param.inputLength = input.Length;
                    int len = Marshal.SizeOf(typeof(double)) * input.Length;
                    param.input = Marshal.AllocHGlobal(len);
                    Marshal.Copy(input, 0, param.input, len);

                    param.output = Marshal.AllocHGlobal(len);
                    Marshal.Copy(output, 0, param.output, len);

                    param.temp = Marshal.AllocHGlobal(len);
                    Marshal.Copy(temp, 0, param.temp, len);

                    param.h = Marshal.UnsafeAddrOfPinnedArrayElement(h, 0);
                    param.g = Marshal.UnsafeAddrOfPinnedArrayElement(g, 0);
                    param.filterLength = h.Length;

                    DllTools_DWT1D(ref param);

                    Marshal.FreeHGlobal(param.input);
                    Marshal.FreeHGlobal(param.output);
                    Marshal.FreeHGlobal(param.temp);
                }
                else
                {
                    FWTParam param;
                    param.input = Marshal.UnsafeAddrOfPinnedArrayElement(input, 0);
                    param.output = Marshal.UnsafeAddrOfPinnedArrayElement(output, 0);
                    param.temp = Marshal.UnsafeAddrOfPinnedArrayElement(temp, 0);
                    param.inputLength = input.Length;

                    param.h = Marshal.UnsafeAddrOfPinnedArrayElement(h, 0);
                    param.g = Marshal.UnsafeAddrOfPinnedArrayElement(g, 0);
                    param.filterLength = h.Length;

                    DllTools_DWT1D(ref param);
                }
            }
            return;
         }

        public static void FTW_2(double[] input, double[] output, double[] temp)
        {
            FWTParam param;
            param.temp = IntPtr.Zero;
            param.h = IntPtr.Zero;
            param.g = IntPtr.Zero;
            param.filterLength = 0;

#if false // non copy
            param.input = Marshal.UnsafeAddrOfPinnedArrayElement(input, 0);
            param.output = Marshal.UnsafeAddrOfPinnedArrayElement(output, 0);
            param.inputLength = input.Length;
            DllTools_DWT1D_V2(ref param);

#else
            param.inputLength = input.Length;
            int len = Marshal.SizeOf(typeof(double)) * input.Length;
            param.input = Marshal.AllocHGlobal(len);
            Marshal.Copy(input, 0, param.input, input.Length);

            param.output = Marshal.AllocHGlobal(len);

            DllTools_DWT1D_V2(ref param);
            Marshal.Copy(param.output, output, 0, output.Length);

            Marshal.FreeHGlobal(param.input);
            Marshal.FreeHGlobal(param.output);

#endif

        }
        public static void FTW_5(double[] input, double[] output)
        {
            FWTParam param;
            param.temp = IntPtr.Zero;
            param.h = IntPtr.Zero;
            param.g = IntPtr.Zero;
            param.filterLength = haar_h.Length;

            if(output.Length < input.Length*2)
            {
                throw (new Exception("Parameter error!"));
            }

            param.inputLength = input.Length;
            param.input = CreateBuffer(input);
            param.output = CreateBuffer(output);
            param.h = CreateBuffer(haar_h);
            param.g = CreateBuffer(haar_g);


            DllTools_DWT1D_V3(ref param);

            CopyBuffer(output, param.output);

            FreeBuffer(param.input);
            FreeBuffer(param.output);
            FreeBuffer(param.h);
            FreeBuffer(param.g);
        }
        public static void CalculateWavelet(double[] input, double[] output, BasicWavelet wavelet)
        {
            FWTParam param;
            param.temp = IntPtr.Zero;
            param.h = IntPtr.Zero;
            param.g = IntPtr.Zero;
            param.filterLength = wavelet.Digital;

            if (output.Length < input.Length * 2)
            {
                throw (new Exception("Parameter error!"));
            }

            param.inputLength = input.Length;
            param.input = CreateBuffer(input);
            param.output = CreateBuffer(output);
            param.h = CreateBuffer(wavelet.Scaling);
            param.g = CreateBuffer(wavelet.Wavelet);


            DllTools_DWT1D_V3(ref param);

            CopyBuffer(output, param.output);

            FreeBuffer(param.input);
            FreeBuffer(param.output);
            FreeBuffer(param.h);
            FreeBuffer(param.g);
        }
        public static void FTW_4(double[] input, double[] output, double[] temp)
        {
            FWTParam param;
            param.temp = IntPtr.Zero;
            param.h = IntPtr.Zero;
            param.g = IntPtr.Zero;
            param.filterLength = 0;

#if false // non copy
            param.input = Marshal.UnsafeAddrOfPinnedArrayElement(input, 0);
            param.output = Marshal.UnsafeAddrOfPinnedArrayElement(output, 0);
            param.inputLength = input.Length;
            DllTools_DWT1D_V2(ref param);

#else
            param.inputLength = input.Length;
            int len = Marshal.SizeOf(typeof(double)) * input.Length;
            MemoryObject inputMemory = _poolCtrl.Get(len);
            param.input = inputMemory.Ptr;
            Marshal.Copy(input, 0, param.input, input.Length);

            MemoryObject outputMemory = _poolCtrl.Get(len);
            param.output = outputMemory.Ptr;

            DllTools_DWT1D_V2(ref param);
            Marshal.Copy(param.output, output, 0, output.Length);

            _poolCtrl.Free(inputMemory);
            _poolCtrl.Free(outputMemory);


#endif

        }


        public static void FTW_3(double[] input, double[] output, double[] temp)
        {
            IntPtr signal, dataOut;
            signal = Marshal.UnsafeAddrOfPinnedArrayElement(input, 0);
            dataOut = Marshal.UnsafeAddrOfPinnedArrayElement(output, 0);

            OneDFwt(signal, (uint)input.Length, dataOut);
            
        }

        [DllImport("DllTools.dll", EntryPoint = "DllTools_DWT1D")]
        private static extern void DllTools_DWT1D(ref FWTParam param);

        [DllImport("DllTools.dll", EntryPoint = "DllTools_DWT1D_V2")]
        public static extern void DllTools_DWT1D_V2(ref FWTParam param);

        [DllImport("DllTools.dll", EntryPoint = "DllTools_DWT1D_V3")]
        public static extern void DllTools_DWT1D_V3(ref FWTParam param);

        [DllImport("dwtHaar1D.dll", EntryPoint = "OneDFwt")]
        private static extern void OneDFwt(IntPtr signal, uint slength, IntPtr output);

        private static IntPtr CreateBuffer(double[] buffer)
        {
            int len = Marshal.SizeOf(typeof(double)) * buffer.Length;
            IntPtr buf = Marshal.AllocHGlobal(len);
            Marshal.Copy(buffer, 0, buf, buffer.Length);
            return buf;
        }
        private static void CopyBuffer(double[] buffer, IntPtr ptr)
        {
            Marshal.Copy(ptr, buffer, 0, buffer.Length);
        }
        private static void FreeBuffer(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

    }

    class FWTCalculator
    {
        static private DllMemoryPoolCtrl _poolCtrl;
        static FWTCalculator()
        {
            _poolCtrl = new DllMemoryPoolCtrl();
        }


        private MemoryObject _inputMemory;
        private MemoryObject _outputMemory;
        public FWTCalculator(int dataSize)
        {
            _inputMemory = _poolCtrl.Get(dataSize);
            _outputMemory = _poolCtrl.Get(dataSize);
        }
        ~FWTCalculator()
        {
            _poolCtrl.Free(_inputMemory);
            _poolCtrl.Free(_outputMemory);
        }

        public void Compute(double[] input, double[] output, double[] temp)
        {
            FWTParam param;
            param.temp = IntPtr.Zero;
            param.h = IntPtr.Zero;
            param.g = IntPtr.Zero;
            param.filterLength = 0;

            param.inputLength = input.Length;
            int len = Marshal.SizeOf(typeof(double)) * input.Length;
            param.input = _inputMemory.Ptr;
            Marshal.Copy(input, 0, param.input, input.Length);

            param.output = _outputMemory.Ptr;

            DllTools.DllTools_DWT1D_V2(ref param);
            Marshal.Copy(param.output, output, 0, output.Length);

            _poolCtrl.Free(_inputMemory);
            _poolCtrl.Free(_outputMemory);
        }
    }

    class NetworkDllToolsV1 : IMLRegression
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Link
        {
            public int toNeuron;
            public int fromNeuron;
            public double weight;
        };
        [StructLayout(LayoutKind.Sequential)]
        struct NEATNetworkParm
        {
            public int linkCount;
            public IntPtr link;

            public int neuronCount;
            public int outputIndex;
            public IntPtr preActivation;
            public IntPtr postActivation;

            public int inputCount;
            public IntPtr input;

            public int outputCount;
            public IntPtr output;

            public int activationCycles;
        };

        [DllImport("DllTools.dll", EntryPoint = "DllTools_NEATNetwork")]
        private static extern void DllTools_NEATNetwork(ref NEATNetworkParm param);

        private NEATNetworkParm parm;
        private Link[] _linkArr;
        private double[] _inputBuffer;
        private double[] _outputBuffer;

        private double[] _preActivation;
        private double[] _postActivation;
        public NetworkDllToolsV1(Encog.Neural.NEAT.NEATNetwork network)
        {
            parm = new NEATNetworkParm();

            parm.linkCount = network.Links.Length;
            _linkArr = new Link[parm.linkCount];
            int index = 0;
            foreach(NEATLink l in network.Links)
            {
                _linkArr[index].fromNeuron = l.FromNeuron;
                _linkArr[index].toNeuron = l.ToNeuron;
                _linkArr[index].weight = l.Weight;
                index++;
            }
            parm.link = Marshal.UnsafeAddrOfPinnedArrayElement(_linkArr, 0);

            parm.neuronCount = network.PostActivation.Length;
            parm.outputIndex = network.OutputIndex;
            _preActivation = new double[parm.neuronCount];
            _postActivation = new double[parm.neuronCount];
            parm.preActivation = Marshal.UnsafeAddrOfPinnedArrayElement(_preActivation, 0);
            parm.postActivation = Marshal.UnsafeAddrOfPinnedArrayElement(_postActivation, 0);
          
            parm.inputCount = network.InputCount;
            _inputBuffer = new double[network.InputCount];
            parm.input = Marshal.UnsafeAddrOfPinnedArrayElement(_inputBuffer, 0);

            parm.outputCount = network.OutputCount;
            _outputBuffer = new double[network.OutputCount];
            parm.output = Marshal.UnsafeAddrOfPinnedArrayElement(_outputBuffer, 0);

            parm.activationCycles = network.ActivationCycles;

        }

        public IMLData Compute(IMLData input)
        {
            BasicMLData inputData = (BasicMLData)input;
            parm.input = Marshal.UnsafeAddrOfPinnedArrayElement(inputData.Data, 0);
            try
            {
                DllTools_NEATNetwork(ref parm);
            }catch(Exception e)
            {

            }
            BasicMLData outData = new BasicMLData(_outputBuffer, false);
            return outData;
        }

        public int InputCount
        {
            get { return parm.inputCount; }
        }

        public int OutputCount
        {
            get { return parm.outputCount; }
        }
    }
    class NetworkDllTools : IMLRegression
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Link
        {
            public int toNeuron;
            public int fromNeuron;
            public double weight;
        };
        [StructLayout(LayoutKind.Sequential)]
        struct NEATNetworkParm
        {
            public int linkCount;
            public IntPtr link;

            public int neuronCount;
            public int outputIndex;
            public IntPtr preActivation;
            public IntPtr postActivation;

            public int inputCount;
            public IntPtr input;

            public int outputCount;
            public IntPtr output;

            public int activationCycles;
        };

        [DllImport("DllTools.dll", EntryPoint = "DllTools_NEATNetwork", CallingConvention=CallingConvention.StdCall)]
        private static extern void DllTools_NEATNetwork(NEATNetworkParm param);

        private NEATNetworkParm parm;
        private Link[] _linkArr;
        private double[] _inputBuffer;
        private double[] _outputBuffer;
        private BasicMLData outData;
        public NetworkDllTools(Encog.Neural.NEAT.NEATNetwork network)
        {
            parm = new NEATNetworkParm();

            parm.linkCount = network.Links.Length;
            _linkArr = new Link[parm.linkCount];
            int index = 0;
            foreach (NEATLink l in network.Links)
            {
                _linkArr[index].fromNeuron = l.FromNeuron;
                _linkArr[index].toNeuron = l.ToNeuron;
                _linkArr[index].weight = l.Weight;
                index++;
            }
            // IntPtr pAddr = Marshal.UnsafeAddrOfPinnedArrayElement(_linkArr, 0);
            parm.link = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Link)) * parm.linkCount);
            IntPtr pAddr = parm.link;
            for (int i = 0; i < _linkArr.Length; i++)
            {
                Marshal.StructureToPtr(_linkArr[i], pAddr, false);
                pAddr += Marshal.SizeOf(typeof(Link));
            }

            parm.neuronCount = network.PostActivation.Length;
            parm.outputIndex = network.OutputIndex;
            parm.preActivation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.neuronCount);
            parm.postActivation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.neuronCount);

            parm.inputCount = network.InputCount;
            _inputBuffer = new double[network.InputCount];
            // parm.input = Marshal.UnsafeAddrOfPinnedArrayElement(_inputBuffer, 0);
            parm.input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.inputCount);

            parm.outputCount = network.OutputCount;
            _outputBuffer = new double[network.OutputCount];
            // parm.output = Marshal.UnsafeAddrOfPinnedArrayElement(_outputBuffer, 0);
            parm.output = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.outputCount);
            outData = new BasicMLData(_outputBuffer, false);

            parm.activationCycles = network.ActivationCycles;

        }
        ~NetworkDllTools()
        {
            Marshal.FreeHGlobal(parm.preActivation);
            Marshal.FreeHGlobal(parm.postActivation);
            Marshal.FreeHGlobal(parm.input);
            Marshal.FreeHGlobal(parm.output);
            Marshal.FreeHGlobal(parm.link);
        }

        public IMLData Compute(IMLData input)
        {
                BasicMLData inputData = (BasicMLData)input;
                // parm.link = Marshal.UnsafeAddrOfPinnedArrayElement(_linkArr, 0);
                // parm.input = Marshal.UnsafeAddrOfPinnedArrayElement(_inputBuffer, 0);
                // parm.output = _outputBuffer, 0);

                Marshal.Copy(inputData.Data, 0, parm.input, inputData.Data.Length);


                DllTools_NEATNetwork(parm);
  
                Marshal.Copy(parm.output, _outputBuffer, 0, _outputBuffer.Length);
            return outData;
        }

        public int InputCount
        {
            get { return parm.inputCount; }
        }

        public int OutputCount
        {
            get { return parm.outputCount; }
        }
    }
    class NetworkFloatDllTools : IMLRegression
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Link
        {
            public int toNeuron;
            public int fromNeuron;
            public float weight;
        };
        [StructLayout(LayoutKind.Sequential)]
        struct NEATNetworkParm
        {
            public int linkCount;
            public IntPtr link;

            public int neuronCount;
            public int outputIndex;
            public IntPtr preActivation;
            public IntPtr postActivation;

            public int inputCount;
            public IntPtr input;

            public int outputCount;
            public IntPtr output;

            public int activationCycles;
        };

        [DllImport("DllTools.dll", EntryPoint = "DllTools_NEATNetworkFloat", CallingConvention = CallingConvention.StdCall)]
        private static extern void DllTools_NEATNetworkFloat(NEATNetworkParm param);

        private NEATNetworkParm parm;
        private Link[] _linkArr;
        private double[] _outputBuffer;
        private BasicMLData outData;
        public NetworkFloatDllTools(Encog.Neural.NEAT.NEATNetwork network)
        {
            parm = new NEATNetworkParm();

            parm.linkCount = network.Links.Length;
            _linkArr = new Link[parm.linkCount];
            int index = 0;
            foreach (NEATLink l in network.Links)
            {
                _linkArr[index].fromNeuron = l.FromNeuron;
                _linkArr[index].toNeuron = l.ToNeuron;
                _linkArr[index].weight = (float)l.Weight;
                index++;
            }
            parm.link = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Link)) * parm.linkCount);
            IntPtr pAddr = parm.link;
            for (int i = 0; i < _linkArr.Length; i++)
            {
                Marshal.StructureToPtr(_linkArr[i], pAddr, false);
                pAddr += Marshal.SizeOf(typeof(Link));
            }

            parm.neuronCount = network.PostActivation.Length;
            parm.outputIndex = network.OutputIndex;
            parm.preActivation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * parm.neuronCount);
            parm.postActivation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * parm.neuronCount);

            parm.inputCount = network.InputCount;
            parm.input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.inputCount);

            parm.outputCount = network.OutputCount;
            _outputBuffer = new double[network.OutputCount];
            parm.output = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * parm.outputCount);
            outData = new BasicMLData(_outputBuffer, false);

            parm.activationCycles = network.ActivationCycles;

        }
        ~NetworkFloatDllTools()
        {
            Marshal.FreeHGlobal(parm.preActivation);
            Marshal.FreeHGlobal(parm.postActivation);
            Marshal.FreeHGlobal(parm.input);
            Marshal.FreeHGlobal(parm.output);
            Marshal.FreeHGlobal(parm.link);
        }

        public IMLData Compute(IMLData input)
        {
            BasicMLData inputData = (BasicMLData)input;
            // parm.link = Marshal.UnsafeAddrOfPinnedArrayElement(_linkArr, 0);
            // parm.input = Marshal.UnsafeAddrOfPinnedArrayElement(_inputBuffer, 0);
            // parm.output = _outputBuffer, 0);

            Marshal.Copy(inputData.Data, 0, parm.input, inputData.Data.Length);

            DllTools_NEATNetworkFloat(parm);

            Marshal.Copy(parm.output, _outputBuffer, 0, _outputBuffer.Length);
            return outData;
        }

        public int InputCount
        {
            get { return parm.inputCount; }
        }

        public int OutputCount
        {
            get { return parm.outputCount; }
        }
    }

}
