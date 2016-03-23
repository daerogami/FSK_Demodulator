using System;

namespace DSP4DotNET
{
    /// <summary>
    /// Imaginary class, Real + Image * i; i is the imaginary unit, i ^ 2 = -1, 
    /// Real real part, Image imaginary part, when the Image = 0, represents a 
    /// real number Real, when Real = 0 and Image is not 0 when pure imaginary number
    /// </summary>
    public class Complex
    {
        public double Real;
        public double Image;
        public double Modulus
        {
            get { return Math.Sqrt(Math.Pow(Real, 2) + Math.Pow(Image, 2)); }
        }
        public double Phase
        {
            get { return Math.Atan2(Image, Real); }
        }
        
        public Complex(double R, double I)
        {
            Real = R;
            Image = I;
        }

        public Complex()
        {
            Real = 0;
            Image = 0;
        }

        /// <summary>
        /// Seeking its complex conjugate is equal to the real part and the imaginary part of opposite number
        /// </summary>
        /// <returns></returns>
        public Complex ConjugateComplex()
        {
            return new Complex(Real, -Image);
        }

        /// <summary>
        /// Copied from one element
        /// </summary>
        /// <param name="other"></param>
        public void Copy(Complex other)
        {
            this.Image = other.Image;
            this.Real = other.Real;
        }

        /// <summary>
        /// addition
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Complex AddOperate(Complex other)
        {
            return new Complex(this.Real + other.Real, this.Image + other.Image);
        }

        /// <summary>
        /// multiplication
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Complex MuliplateOperate(Complex other)
        {
            return new Complex(this.Real * other.Real - this.Image * other.Image, this.Image * other.Real + this.Real * other.Image);
        }
    }
}