
namespace com.inova8.resolver
{
    using System;
    internal class CovClass
    {
        private double[] pCov = null;
        private int n = 0;

        internal CovClass(int nReconciledVariables)
        {
            n = nReconciledVariables;
            pCov = new double[((nReconciledVariables + 1) * nReconciledVariables) / 2 + 1];
        }
        internal void InitializeCovariance( ResultsClass pResults )
        {
            int i = 0;

            for (i = 0; i < pResults.Count; i++)
            {
                ((ResultClass)pResults[i]).Value = ((ResultClass)pResults[i]).InitialValue.Value;
                this[i + 1, i + 1] = ((ResultClass)pResults[i]).InitialTolerance;
            }
        }
        //internal void InitializeSize(int nReconciledVariables)
        //{
        //    n = nReconciledVariables;
        //    pCov = new double[((nReconciledVariables + 1) * nReconciledVariables) / 2 + 1];
        //}
        //private double getValue(int i, int j)
        //{
        //    double getValue = 0.0;
        //    if (i <= j)
        //    {
        //        getValue = pCov[((((2 * n) - i) * (i - 1)) / 2) + j];
        //    }
        //    else
        //    {
        //        getValue = pCov[((((2 * n) - j) * (j - 1)) / 2) + i];
        //    }
        //    return getValue;
        //}

        //internal void setValue(int i, int j, double vValue)
        //{
        //    //Get upper triangular data
        //    if (i <= j)
        //    {
        //        pCov[((((2 * n) - i) * (i - 1)) / 2) + j] = vValue;
        //    }
        //    else
        //    {
        //        pCov[((((2 * n) - j) * (j - 1)) / 2) + i] = vValue;
        //    }
        //}

        //internal double getSquare(int i, int j)
        //{
        //    double Square = 0.0;
        //    int k = 0;
        //    int m = 0;
        //    double dSquare = 0.0;
        //    dSquare = 0.0;
        //    if (i > j)
        //    {
        //        m = i;
        //    }
        //    else
        //    {
        //        m = j;
        //    }
        //    for (k = m; k <= n; k++)
        //    {
        //        dSquare = dSquare + (this.getValue(i, k) * this.getValue(j, k));
        //    }
        //    Square = dSquare;
        //    return Square;
        //}
        //internal double getSquareRoot(int i, int j)
        //{
        //    double SquareRoot = 0.0;
        //    if (i > j)
        //    {
        //        SquareRoot = 0;
        //    }
        //    else
        //    {
        //        SquareRoot = this.getValue(i, j);
        //    }
        //    return SquareRoot;
        //}
        internal void Reset()
        {
            pCov = new double[((n + 1) * n) / 2 + 1];
        }
        internal double this[int i, int j]
        {
            get
            {
                if (i <= j)
                {
                    return pCov[((((2 * n) - i) * (i - 1)) / 2) + j];
                }
                else
                {
                    return pCov[((((2 * n) - j) * (j - 1)) / 2) + i];
                }
            }
            set 
            {
                if (i <= j)
                {
                    pCov[((((2 * n) - i) * (i - 1)) / 2) + j] = value;
                }
                else
                {
                    pCov[((((2 * n) - j) * (j - 1)) / 2) + i] = value;
                }
            }
        }
        internal double this[int i]
        {
            get
            {
                int k = 0;
                int m = i;
                double dSquare = 0.0;
                double getValue = 0.0;
                for (k = m; k <= n; k++)
                {
                    if (i <= k)
                    {
                        getValue = pCov[((((2 * n) - i) * (i - 1)) / 2) + k];
                    }
                    else
                    {
                        getValue = pCov[((((2 * n) - k) * (k - 1)) / 2) + i];
                    }

                    dSquare += getValue * getValue;
                    //dSquare += (this.getValue(i, k) * this.getValue(i, k));
                }
                return dSquare;
            }
        }
    }
}