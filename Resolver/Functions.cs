
namespace Resolver
{
    using System;
    public class Functions
    {
        /// <summary>' Constants rounded for 21 decimals. </summary>
        //protected internal static double M_E = 2.71828182845905;
        //protected internal static double M_LOG2E = 1.44269504088896;
        //protected internal static double M_LOG10E = 0.434294481903252;
        //protected internal static double M_LN2 = 0.693147180559945;
        //protected internal static double M_LN10 = 2.30258509299405;
        //protected internal static double M_PI = 3.14159265358979;
        //protected internal static double M_PI_2 = 1.5707963267949;
        //protected internal static double M_PI_4 = 0.785398163397448;
        //protected internal static double M_1_PI = 0.318309886183791;
        //protected internal static double M_2_PI = 0.636619772367581;
        protected internal static double M_1_SQRTPI = 0.564189583547756;
        protected internal static double M_2_SQRTPI = 1.12837916709551;
        protected internal static double M_SQRT2 = 1.4142135623731;
        //protected internal static double M_SQRT_2 = 0.707106781186548;
        protected internal static double REC_ETA = 0.0001;
        //protected internal static double REC_SMALLVALUE = 1e-20;
        protected internal static double REC_LARGEVALUE = 100000000.0;
        protected internal static double REC_ACCURACY = 0.001;

        /// <summary> --------------------------------------------------------------------
        /// 
        /// FUNCTION NAME: LnFactorial
        /// 
        /// PARAMETERS   :
        /// 
        /// GLOBAL       :
        /// 
        /// 
        /// STATUS RETURN:
        /// 
        /// TESTED DATE  :
        /// 
        /// AUTHOR       : PJL
        /// 
        /// 
        /// DESCRIPTION  : Series approximation routine for ln(x!).
        /// Accuracy better than 6 places for x>=3
        /// Accuracy better than 12 places for x>=10
        /// Advantage is that very large values of argument
        /// can be used without fear of overflow
        /// 
        /// ln(x!) = (x+.5).ln(x)
        /// 
        /// - x +  1  -   1    +    1    -    1
        /// ---   -----     -----     -----
        /// 12x   360x^3   1260x^5   1680x^7
        /// 
        /// + 0.918938533205
        /// </summary>
        /// 

        protected static internal Double NormSInv(Double p)
        {
            const double a1 = -39.6968302866538;
            const double a2 = 220.946098424521;
            const double a3 = -275.928510446969;
            const double a4 = 138.357751867269;
            const double a5 = -30.6647980661472;
            const double a6 = 2.50662827745924;
            const double b1 = -54.4760987982241;
            const double b2 = 161.585836858041;
            const double b3 = -155.698979859887;
            const double b4 = 66.8013118877197;
            const double b5 = -13.2806815528857;
            const double c1 = -0.00778489400243029;
            const double c2 = -0.322396458041136;
            const double c3 = -2.40075827716184;
            const double c4 = -2.54973253934373;
            const double c5 = 4.37466414146497;
            const double c6 = 2.93816398269878;
            const double d1 = 0.00778469570904146;
            const double d2 = 0.32246712907004;
            const double d3 = 2.445134137143;
            const double d4 = 3.75440866190742;
            const double p_low = 0.02425;
            const double p_high = 1 - p_low;
            Double q;
            Double r;
            Double pNormSInv;
            if ((p < 0) || (p > 1))
            {
                //Err.Raise(vbObjectError, , "NormSInv: Argument out of range.");
                pNormSInv = 0;
            }
            else if (p < p_low)
            {
                q = Math.Sqrt(-2 * Math.Log(p));
                pNormSInv = (((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / ((((d1 * q + d2) * q + d3) * q + d4) * q + 1);
            }
            else if (p <= p_high)
            {
                q = p - 0.5;
                r = q * q;
                pNormSInv = (((((a1 * r + a2) * r + a3) * r + a4) * r + a5) * r + a6) * q / (((((b1 * r + b2) * r + b3) * r + b4) * r + b5) * r + 1);
            }
            else
            {
                q = Math.Sqrt(-2 * Math.Log(1 - p));
                pNormSInv = -(((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / ((((d1 * q + d2) * q + d3) * q + d4) * q + 1);
            }
            return pNormSInv;
        }
        protected internal static double LnFactorial(double dX)
        {
            double LnFactorial = 0.0;
            //double dX1 = 0.0;
            double dLnFactorial = 0.0;

            if (dX < (-0.5 - REC_ETA))
            {

                LnFactorial = REC_LARGEVALUE;
                return LnFactorial;
            }
            else if ((dX > (-0.5 - REC_ETA)) && (dX < (-0.5 + REC_ETA)))
            {
                LnFactorial = -System.Math.Log(M_1_SQRTPI);
                return LnFactorial;
            }
            else if ((dX > -REC_ETA) && (dX < REC_ETA))
            {
                LnFactorial = 0;
                return LnFactorial;
            }
            else if ((dX > (0.5 - REC_ETA)) && (dX < (0.5 + REC_ETA)))
            {

                LnFactorial = -System.Math.Log(M_2_SQRTPI);
                return LnFactorial;
            }
            else
            {
                //      dX1 = 1# / (dX * dX)
                //      dLnFactorial = (dX + 0.5) * Log(dX) - dX * _
                //                      (1# - dX1 * (1# - dX1 * (1# - dX1 * (1# - _
                //                      dX1 / 1.33333333) / 3.5) / 30#) / 12#) _
                //                      + 0.918938533205
                dLnFactorial = (((((((dX + 0.5) * System.Math.Log(dX)) - dX) + (1 / (12 * dX))) - (1 / System.Math.Pow((360 * dX), 3))) + (1 / System.Math.Pow((1260 * dX), 5))) - (1 / System.Math.Pow((1680 * dX), 7))) + 0.918938533205;

                LnFactorial = dLnFactorial;
                return LnFactorial;
            }
        }

        /// <summary> --------------------------------------------------------------------
        /// 
        /// FUNCTION NAME: ChiSquareDensity
        /// 
        /// PARAMETERS   :
        /// 
        /// GLOBAL       :
        /// 
        /// 
        /// STATUS RETURN:
        /// 
        /// TESTED DATE  :
        /// 
        /// AUTHOR       : PJL
        /// 
        /// 
        /// DESCRIPTION  : Takes a given degree of freedom, DegreeFreedom,
        /// and value dValue and calculates the Chi-square
        /// density distribution function dY.
        /// 
        /// exp( -x/2 ).x^(M/2-1)
        /// p(x) =    ---------------------
        /// 2^(M/2).Gamma(M/2)
        /// </summary>
        protected internal static double ChiSquareDensity(int nDegreeFreedom, double dValue)
        {
            double ChiSquareDensity = 0.0;
            double dChi = 0.0;
            double dLnValue = 0.0;

            if (nDegreeFreedom < 1)
            {
                ChiSquareDensity = 0.0;
                return ChiSquareDensity;
            }
            if (System.Math.Abs(dValue) < REC_ETA)
            {
                dLnValue = -REC_LARGEVALUE;
            }
            else
            {
                dLnValue = System.Math.Log(dValue);
            }

            dChi = ((((-dValue) / 2) + (((nDegreeFreedom / 2) - 1) * dLnValue)) - ((nDegreeFreedom / 2) * System.Math.Log(2))) - Functions.LnFactorial(((nDegreeFreedom / 2) - 1));
            ChiSquareDensity = System.Math.Exp(dChi);
            return ChiSquareDensity;
        }

        /// <summary> --------------------------------------------------------------------
        /// 
        /// FUNCTION NAME: ChiSquareProbability
        /// 
        /// PARAMETERS   :
        /// 
        /// GLOBAL       :
        /// 
        /// 
        /// STATUS RETURN:
        /// 
        /// TESTED DATE  :
        /// 
        /// AUTHOR       : PJL
        /// 
        /// 
        /// DESCRIPTION  : Integrates the ChiSquareDensity function using
        /// the approximation:
        /// 2.x                      x^k
        /// P(x) = ---.p(x).{ 1 + sum{ ------------------ }}
        /// M                  (M+2).(M+4)...(M+2k)
        /// </summary>
        protected internal static double ChiSquareProbability(int nDegreeFreedom, double dValue)
        {
            double ChiSquareProbability = 0.0;
            double dChiProb = 0.0;
            double dM = 0.0;
            double dXk = 0.0;

            dChiProb = 1.0;

            if (nDegreeFreedom < 1)
            {

                ChiSquareProbability = 0.0;
                return ChiSquareProbability;
            }

            dM = nDegreeFreedom + 2.0;
            dXk = dValue / dM;

            do
            {
                dChiProb = dChiProb + dXk;
                dM = dM + 2.0;
                dXk = (dXk * dValue) / dM;
            }
            while (dXk > REC_ACCURACY);

            dChiProb = (dChiProb * 2.0 * dValue * Functions.ChiSquareDensity(nDegreeFreedom, dValue)) / nDegreeFreedom;
            ChiSquareProbability = dChiProb;
            return ChiSquareProbability;
        }

        /// <summary> --------------------------------------------------------------------
        /// 
        /// FUNCTION NAME: ArcChiSquare
        /// 
        /// PARAMETERS   :
        /// 
        /// GLOBAL       :
        /// 
        /// 
        /// STATUS RETURN:
        /// 
        /// TESTED DATE  :
        /// 
        /// AUTHOR       : PJL
        /// 
        /// 
        /// DESCRIPTION  : Calculates the ordinate value of the Chi square
        /// distribution for a given tail area and degree of
        /// freedom.
        /// Uses binary halving to solve the equation:
        /// dBeta = 1.0 - ArcChiSquare ( dOrdinate )
        /// </summary>
        protected internal static double ArcChiSquare( double dBeta, int nDegreeFreedom)
        {
            double ArcChiSquare = 0.0;
            double dOrdinate = 0.0;
            double dDeltaOrdinate = 0.0;

            dDeltaOrdinate = 1.0;

            if (nDegreeFreedom < 1)
            {

                ArcChiSquare = REC_LARGEVALUE;
                return ArcChiSquare;
            }

            if (dBeta < REC_ETA)
            {
                ArcChiSquare = REC_LARGEVALUE;
                return ArcChiSquare;
            }
            else if (dBeta > (1.0 - REC_ETA))
            {
                ArcChiSquare = 0.0;
                return ArcChiSquare;
            }
            else
            {
                dOrdinate = nDegreeFreedom;

                do
                {
                    if (dDeltaOrdinate > 0.0)
                    {
                        if ((1.0 - Functions.ChiSquareProbability(nDegreeFreedom, dOrdinate)) > dBeta)
                        {
                            dOrdinate = dOrdinate + dDeltaOrdinate;
                        }
                        else
                        {
                            dDeltaOrdinate = dDeltaOrdinate * (-0.5);
                        }
                    }
                    else
                    {
                        if ((1.0 - Functions.ChiSquareProbability(nDegreeFreedom, dOrdinate)) < dBeta)
                        {
                            dOrdinate = dOrdinate + dDeltaOrdinate;
                        }
                        else
                        {
                            dDeltaOrdinate = dDeltaOrdinate * (-0.5);
                        }
                    }
                }
                while (System.Math.Abs(dDeltaOrdinate) > REC_ACCURACY);
                ArcChiSquare = dOrdinate;
            }
            return ArcChiSquare;
        }

        /// <summary> --------------------------------------------------------------------
        /// 
        /// FUNCTION NAME: Erf
        /// 
        /// PARAMETERS   :
        /// 
        /// GLOBAL       :
        /// 
        /// 
        /// STATUS RETURN:
        /// 
        /// TESTED DATE  :
        /// 
        /// AUTHOR       : PJL
        /// 
        /// 
        /// DESCRIPTION  : Asymptotic series expansion of the integral of
        /// 2 exp ( - x ^ 2 ) / sqrt( PI )
        /// - the normalised error function.
        /// This program determines the values of the above
        /// integrand using an asymptotic series which is
        /// evaluated to the level of maximum accuracy
        /// 
        /// exp(-x^2)
        /// x > 3         erf(x) = 1 - ---------
        /// x sqrt(pi)
        /// 
        /// 1       1.3       1.3.5
        /// { 1 - ---- + -------- - --------
        /// 2x^2   (2x^2)^2   (2x^2)^3
        /// 
        /// 2
        /// x <= 3       erf(x) =  --------
        /// sqrt(pi)
        /// 
        /// x^3    x^5    x^7
        /// { x - ---- + ---- - ----
        /// 3.1!   5.2!   7.3!
        /// </summary>
        protected internal static double Erf(double dValue)
        {
            double Erf = 0.0;
            double dC1 = 0.0;
            double dC2 = 0.0;
            double dC0 = 0.0;
            double dR = 0.0;
            double dK = 0.0;
            double dError = 0.0;
            double dE = 0.0;
            double dErf = 0.0;
            double dN = 0.0;

            dValue = System.Math.Abs(dValue / M_SQRT2);
            if (dValue > 3.0)
            {
                dErf = 1.0;
                dN = 1.0;
                dC2 = 1.0 / (2.0 * dValue * dValue);
                dC0 = dC2;

                do
                {
                    dErf = dErf - dC2;
                    dC1 = dC2;
                    dN = dN + 2.0;
                    dC2 = (-dC1) * dN * dC0;
                }
                while (System.Math.Abs(dC2) < System.Math.Abs(dC1));

                dN = (dN + 1.0) / 2.0;
                dE = System.Math.Exp(((-dValue) * dValue)) / dValue / M_1_SQRTPI;
                dErf = 1.0 - (dErf * dE);
                dError = dE * dC2;
                Erf = dErf;
                return Erf;
            }
            else if (dValue > 0.0)
            {
                dErf = 0.0;
                dR = 1.0;
                dC1 = dValue;
                dK = 1.0;
                dN = 0.0;

                do
                {
                    dE = dC1 / (dK * dR);
                    dErf = dErf + dE;
                    dC1 = dC1 * (-dValue) * dValue;
                    dK = dK + 2.0;
                    dN = dN + 1.0;
                    dR = dR * dN;
                }
                while (System.Math.Abs(dE) > REC_ACCURACY);

                dErf = dErf * M_2_SQRTPI;
                Erf = dErf;
                return Erf;
            }
            else
            {

                Erf = 0.0;
                return Erf;
            }
        }

        /// <summary> --------------------------------------------------------------------
        /// 
        /// FUNCTION NAME: ArcErf
        /// 
        /// PARAMETERS   :
        /// 
        /// GLOBAL       :
        /// 
        /// 
        /// STATUS RETURN:
        /// 
        /// TESTED DATE  :
        /// 
        /// AUTHOR       : PJL
        /// 
        /// 
        /// DESCRIPTION  : Calculates the ordinate value of the unit normal
        /// distribution for a given tail area.
        /// Uses binary halving to solve the equation:
        /// dBeta = 1.0 - Erf ( dOrdinate )
        /// </summary>
        protected internal static double ArcErf(double dBeta)
        {
            double ArcErf = 0.0;
            double dOrdinate = 0.0;
            double dDeltaOrdinate = 0.0;
            dOrdinate = 0.0;
            dDeltaOrdinate = 1.0;

            if (dBeta >= 1.0)
            {
                ArcErf = 0.0;
                return ArcErf;
            }
            else if (dBeta <= 0.0)
            {
                ArcErf = REC_LARGEVALUE;
                return ArcErf;
            }
            else
            {
                do
                {
                    if (dDeltaOrdinate > 0.0)
                    {
                        if ((1.0 - Functions.Erf(dOrdinate)) > dBeta)
                        {
                            dOrdinate = dOrdinate + dDeltaOrdinate;
                        }
                        else
                        {
                            dDeltaOrdinate = dDeltaOrdinate * (-0.5);
                        }
                    }
                    else
                    {
                        if ((1.0 - Functions.Erf(dOrdinate)) < dBeta)
                        {
                            dOrdinate = dOrdinate + dDeltaOrdinate;
                        }
                        else
                        {
                            dDeltaOrdinate = dDeltaOrdinate * (-0.5);
                        }
                    }
                }
                while (System.Math.Abs(dDeltaOrdinate) > REC_ACCURACY);
                ArcErf = dOrdinate;
                return ArcErf;
            }
        }
    }
}