using MyProject01.NeuroNetwork;
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
        public BasicDaubechiesWavelet()
        {
            for(int i=0;i<Scaling.Length;i++)
            {
                Scaling[i] /= Math.Sqrt(2);
            }
        }
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
                        1.0/Math.Sqrt(2), 
                        1.0/Math.Sqrt(2), 
                     };
                }
            }

            public override double[] Wavelet
            {
                get
                {
                    return new double[] 
                    { 
                        -1.0/Math.Sqrt(2), 
                        1.0/Math.Sqrt(2), 
                     };
                }
            }
        }
    class Daubechies8Wavelet : BasicDaubechiesWavelet
    {
        private double[] _scaling = new double[] 
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
        public override double[] Scaling
        {
            get 
            {
                return _scaling;
            }
        }
    }
    class Daubechies16Wavelet : BasicDaubechiesWavelet
    {
        private double[] _scaling = new double[] 
                { 
                    0.07695562, 
                    0.44246725, 
                    0.95548615,
                    0.82781653,
                    -0.02238574,
                    -0.40165863,
                    6.68194092e-4,
                    0.18207636,
                    -0.02456390,
                    -0.06235021,
                    0.01977216,
                    0.01236884,
                    -6.88771926e-3,
                    -5.54004549e-4,
                    9.55229711e-4,
                    -1.66137261e-4
                };
        public override double[] Scaling
        {
            get
            {
                return _scaling;
            }
        }
    }
    class Daubechies20Wavelet : BasicDaubechiesWavelet
    {
        private double[] _scaling = new double[] 
                { 
                    0.03771716,
                    0.26612218,
                    0.74557507,
                    0.97362811,
                    0.39763774,
                    -0.35333620,
                    -0.27710988,
                    0.18012745,
                    0.13160299,
                    -0.10096657,
                    -0.04165925,
                    0.04696981,
                    5.10043697e-3,
                    -0.01517900,
                    1.97332536e-3,
                    2.81768659e-3,
                    -9.69947840e-4,
                    -1.64709006e-4,
                    1.32354367e-4,
                    -1.875841e-5
                };
        public override double[] Scaling
        {
            get
            {
                return _scaling;
            }
        }
    }

#if true
    class Daubechies4Wavelet : BasicDaubechiesWavelet
    {
        private double[] _scaling = new double[] 
                { 
                    0.6830127, 
                    1.1830127, 
                    0.3169873,
                    -0.1830127
                };
        public override double[] Scaling
        {
            get
            {
                return _scaling;
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
        private double[] _scaling = new double[] 
                { 
                    63/256f, 
                    35/256f,
                    30/256f,
                    30/256f, 
                    35/256f,
                    63/256f
                };
        public override double[] Scaling
        {
            get
            {
                return _scaling;
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

}
