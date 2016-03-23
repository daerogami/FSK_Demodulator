using System;

namespace DSP4DotNET
{
    /// <summary>
    /// Window function
    /// </summary>
    public class WindowFunction
    {
        /// <summary>
        /// Hamming Window
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="frameSize">window size</param>
        /// <returns></returns>
        public static double HammingWindow(int index, int frameSize)
        {
            return 0.54 - 0.46 * Math.Cos((2 * Math.PI * index) / (frameSize - 1));
        }

        /// <summary>
        /// Hanning
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="frameSize">window size</param>
        /// <returns></returns>
        public static double HannWindow(int index, int frameSize)
        {
            return 0.5 * (1 - Math.Cos((2 * Math.PI * index) / (frameSize - 1)));
        }

        /// <summary>
        /// Rectangular window
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="frameSize">window size</param>
        /// <returns></returns>
        public static double RectWindow(int index, int frameSize)
        {
            return 1;
        }

        /// <summary>
        /// Blackman Window
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="frameSize">window size</param>
        /// <returns></returns>
        public static double BlackmanWindow(int index, int frameSize)
        {
            return 0.42 - 0.5 * Math.Cos((2 * Math.PI * index) / (frameSize - 1)) + 0.08 * Math.Cos((4 * Math.PI * index) / (frameSize - 1));
        }

        /// <summary>
        /// Gaussian window
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="frameSize">window size</param>
        /// <param name="alpha">Coefficient of less than or equal to 0.5</param>
        /// <returns></returns>
        public static double GuaseWindow(int index, int frameSize, double alpha)
        {
            return Math.Pow(Math.E, (-1 / 2) * Math.Pow(((index - (frameSize - 1) / 2) / (alpha * (frameSize - 1) / 2)), 2));
        }

        /// <summary>
        /// Barlett Window
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="frameSize">window size</param>
        /// <returns></returns>
        public static double BartlettWindow(int index, int frameSize)
        {
            return 1.0 - Math.Abs(1.0 - 2 * index * Math.PI / (frameSize - 1));
        }

        /// <summary>
        /// Barlett-Hann Window
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="frameSize">window size</param>
        /// <returns></returns>
        public static double BartlettHannWindow(int index, int frameSize)
        {
            return 0.62 - 0.48 * Math.Abs((index / (frameSize - 1)) - 1 / 2) - 0.38 * Math.Cos((2 * Math.PI * index) / (frameSize - 1));
        }

        /// <summary>
        /// Blackman-Harris window
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="frameSize">window size</param>
        /// <returns></returns>
        public static double BlackmannHarrisWindow(int index, int frameSize)
        {
            return 0.35875 - (0.48829 * Math.Cos((2 * Math.PI * index) / (frameSize - 1))) + (0.14128 * Math.Cos((4 * Math.PI * index) / (frameSize - 1))) - (0.01168 * Math.Cos((6 * Math.PI * index) / (frameSize - 1)));
        }

        /// <summary>
        /// Kaiser Window
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="frameSize">window size</param>
        /// <param name="beta">Coefficient, generally 3 >= beta >= 9</param>
        /// <returns></returns>
        public static double KaiserWindow(int index, int frameSize, double beta)
        {
            double a = 0, w = 0, a2 = 0, b1 = 0, b2 = 0, beta1 = 0;
            b1 = FnZeroOrderBessel(beta);
            a = 2.0 * index / (double)(frameSize - 1) - 1;
            a2 = a * a;
            beta1 = beta * Math.Sqrt(1.0 - a2);
            b2 = FnZeroOrderBessel(beta1);
            w = b2 / b1;
            return w;
        }

        /// <summary>
        /// The first order modified Bessel function 0
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double FnZeroOrderBessel(double x)
        {
            double m_Result = 0;
            double d = 0, y = 0, d2 = 0;
            y = x / 2.0;
            d = 1.0d;
            m_Result = 1.0d;
            for (int i = 1; i <= 25; i++)
            {
                d = d * y / i;
                d2 = d * d;
                m_Result += d2;
                if (d2 < m_Result * 1.0e-8)
                {
                    break;
                }
            }
            return m_Result;
        }

        /// <summary>
        /// Hamming window sequence windowing filter
        /// </summary>
        /// <param name="data">Data Frame</param>
        /// <returns></returns>
        public static double[] FnUsingHammingWindow(double[] data)
        {
            double[] m_Result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                m_Result[i] = HammingWindow(i, data.Length) * data[i];
            }
            return m_Result;
        }

        /// <summary>
        /// Use Hanning
        /// </summary>
        /// <param name="data">Data Frame</param>
        /// <returns></returns>
        public static double[] FnUsingHannWindow(double[] data)
        {
            double[] m_Result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                m_Result[i] = HannWindow(i, data.Length) * data[i];
            }
            return m_Result;
        }

        /// <summary>
        /// Using a rectangular window
        /// </summary>
        /// <param name="data">Data Frame</param>
        /// <returns></returns>
        public static double[] FnUsingRectWindow(double[] data)
        {
            double[] m_Result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                m_Result[i] = RectWindow(i, data.Length) * data[i];
            }
            return m_Result;
        }

        /// <summary>
        /// Use Blackman window
        /// </summary>
        /// <param name="data">Data Frame</param>
        /// <returns></returns>
        public static double[] FnUsingBlackmanWindow(double[] data)
        {
            double[] m_Result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                m_Result[i] = BlackmanWindow(i, data.Length) * data[i];
            }
            return m_Result;
        }

        /// <summary>
        /// Use Gaussian window
        /// </summary>
        /// <param name="data">Data Frame</param>
        /// <param name="alpha">Filter coefficients, and generally less than or equal 0.5</param>
        /// <returns></returns>
        public static double[] FnUsingGuaseWindow(double[] data, double alpha = 0.5)
        {
            double[] m_Result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                m_Result[i] = GuaseWindow(i, data.Length, alpha) * data[i];
            }
            return m_Result;
        }

        /// <summary>
        /// Bartlett window that is using a triangular window
        /// </summary>
        /// <param name="data">Data Frame</param>
        /// <returns></returns>
        public static double[] FnUsingBartlettWindow(double[] data)
        {
            double[] m_Result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                m_Result[i] = BartlettWindow(i, data.Length);
            }
            return m_Result;
        }

        /// <summary>
        /// Use Bartlett Hann window
        /// </summary>
        /// <param name="data">Data Frame</param>
        /// <returns></returns>
        public static double[] FnUsingBartlettHannWindow(double[] data)
        {
            double[] m_Result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                m_Result[i] = BartlettHannWindow(i, data.Length);
            }
            return m_Result;
        }

        /// <summary>
        /// Use Blackman Harris window
        /// </summary>
        /// <param name="data">Data Frame</param>
        /// <returns></returns>
        public static double[] FnUsingBlackmannHarrisWindow(double[] data)
        {
            double[] m_Result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                m_Result[i] = BlackmannHarrisWindow(i, data.Length);
            }
            return m_Result;
        }

        /// <summary>
        /// Using Kaiser window
        /// </summary>
        /// <param name="data">Data Frame</param>
        /// <param name="beta"></param>
        /// <returns></returns>
        public static double[] FnUsingKaiserWindow(double[] data, double beta)
        {
            double[] m_Result = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                m_Result[i] = KaiserWindow(i, data.Length, beta);
            }
            return m_Result;
        }
    }
}