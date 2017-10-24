namespace com.inova8.resolver{

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

//// Copyright © Ian Smith 2002-2007
// http://members.aol.com/iandjmsmith/examples.xls
//// Version 3.3.3

public class StatisticsClass
{

	const  double ErrorValue = -9999999999999.9;
	const  bool NonIntegralValuesAllowed_df = false;
	// Are non-integral degrees of freedom for t, chi_square and f distributions allowed?
	const  bool NonIntegralValuesAllowed_NB = false;
	// Is "successes required" parameter for negative binomial allowed to be non-integral?

	const  bool NonIntegralValuesAllowed_Others = false;
	// Can Int function be applied to parameters like sample_size or is it a fault if
	// the parameter is non-integral?

	const  double nc_limit = 1000000.0;
	// Upper Limit for non-centrality parameters - as far as I know it's ok but slower
	//and slower up to 1e12. Above that I don't know.
	const  double lstpi = 0.918938533204673;
	// 0.9189385332046727417803297364 = ln(sqrt(2*Pi))
	const  double sumAcc = 5E-16;
	const  double cfSmall = 1E-14;
	const  double cfVSmall = 1E-15;
	const  double minLog1Value = -0.79149064;
	const  double OneOverSqrTwoPi = 0.398942280401433;
	// 0.39894228040143267793994605993438
	const double  scalefactor = 2^256; //1.15792089237316E+77;
	// 1.1579208923731619542357098500869e+77 = 2^256  ' used for rescaling
	//calcs w/o impacting accuracy, to avoid over/underflow
	const double  scalefactor2 = 2^-256;//8.63616855509444E-78;
	// 8.6361685550944446253863518628004e-78 = 2^-256
    const double max_discrete = 2^53; //9.00719925474099E+15;
	// 2^53 required for exact addition of 1 in hypergeometric routines
    const double max_crit = 2^52; //4.5035996273705E+15;
	// 2^52 to make sure plenty of room for exact addition of 1 in crit routines
	const double  nearly_zero = 9.99999983659714E-317;
	const double  cSmall = 5.562684646268E-309;
	// (smallest number before we start losing precision)/4
	const double  t_nc_limit = 1.34078079299426E154;
	// just under 1/Sqr(cSmall)
	const double   Log1p5 = 0.405465108108164;
	// 0.40546510810816438197801311546435 = Log(1.5)
	const double logfbit0p5 = 0.0548141210519177;
	// 0.054814121051917653896138702348386 = logfbit(0.5)

	//For logfbit functions
	// Stirling's series for ln(Gamma(x)), A046968/A046969
	const double  lfbc1 = 1.0 / 12.0;
	const double  lfbc2 = 1.0 / 30.0;
	// lfbc2 on are Sloane's ratio times 12
	const double  lfbc3 = 1.0 / 105.0;
	const double   lfbc4 = 1.0 / 140.0;
	const double  lfbc5 = 1.0 / 99.0;
	const double  lfbc6 = 691.0 / 30030.0;
	const double   lfbc7 = 1.0 / 13.0;
	const double  lfbc8 = 0.350686068964593;
	// Chosen to make logfbit(6) & logfbit(7) correct
	const double  lfbc9 = 1.67699982016711;
	// Chosen to make logfbit(6) & logfbit(7) correct

	//For invcnormal                             ' http://lib.stat.cmu.edu/apstat/241
		// 3.3871328727963666080
	const double  a0 = 3.38713287279637;
		// 133.14166789178437745
	const double  a1 = 133.141667891784;
		// 1971.5909503065514427
	const double  a2 = 1971.59095030655;
		// 13731.693765509461125
	const double   a3 = 13731.6937655095;
		// 45921.953931549871457
	const double  a4 = 45921.9539315499;
		// 67265.770927008700853
	const double  a5 = 67265.7709270087;
		// 33430.575583588128105
	const double  a6 = 33430.5755835881;
		// 2509.0809287301226727
	const double  a7 = 2509.08092873012;
		// 42.313330701600911252
	const double  b1 = 42.3133307016009;
		// 687.18700749205790830
	const double  b2 = 687.187007492058;
		// 5394.1960214247511077
	const double  b3 = 5394.19602142475;
		// 21213.794301586595867
	const double  b4 = 21213.7943015866;
		// 39307.895800092710610
	const double  b5 = 39307.8958000927;
		// 28729.085735721942674
	const double  b6 = 28729.0857357219;
		// 5226.4952788528545610
	const double  b7 = 5226.49527885285;
	////Coefficients for P not close to 0, 0.5 or 1.
		// 1.42343711074968357734
	const double  c0 = 1.42343711074968;
		// 4.63033784615654529590
	const double  c1 = 4.63033784615655;
		// 5.76949722146069140550
	const double  c2 = 5.76949722146069;
		// 3.64784832476320460504
	const double  c3 = 3.6478483247632;
		// 1.27045825245236838258
	const double  c4 = 1.27045825245237;
		// 0.241780725177450611770
	const double  c5 = 0.241780725177451;
		// 2.27238449892691845833E-02
	const double  c6 = 0.0227238449892692;
		// 7.74545014278341407640E-04
	const double  c7 = 0.000774545014278341;
		// 2.05319162663775882187
	const double  d1 = 2.05319162663776;
		// 1.67638483018380384940
	const double  d2 = 1.6763848301838;
		// 0.689767334985100004550
	const double  d3 = 0.6897673349851;
		// 0.148103976427480074590
	const double  d4 = 0.14810397642748;
		// 1.51986665636164571966E-02
	const double  d5 = 0.0151986665636165;
		// 5.47593808499534494600E-04
	const double  d6 = 0.000547593808499535;
		// 1.05075007164441684324E-09
	const double  d7 = 1.05075007164442E-09;
	////Coefficients for P near 0 or 1.
		// 6.65790464350110377720
	const double  e0 = 6.6579046435011;
		// 5.46378491116411436990
	const double  e1 = 5.46378491116411;
		// 1.78482653991729133580
	const double  e2 = 1.78482653991729;
		// 0.296560571828504891230
	const double  e3 = 0.296560571828505;
		// 2.65321895265761230930E-02
	const double  e4 = 0.0265321895265761;
		// 1.24266094738807843860E-03
	const double  e5 = 0.00124266094738808;
		// 2.71155556874348757815E-05
	const double  e6 = 2.71155556874349E-05;
		// 2.01033439929228813265E-07
	const double  e7 = 2.01033439929229E-07;
		// 0.599832206555887937690
	const double  f1 = 0.599832206555888;
		// 0.136929880922735805310
	const double  f2 = 0.136929880922736;
		// 1.48753612908506148525E-02
	const double  f3 = 0.0148753612908506;
		// 7.86869131145613259100E-04
	const double  f4 = 0.000786869131145613;
		// 1.84631831751005468180E-05
	const double  f5 = 1.84631831751005E-05;
		// 1.42151175831644588870E-07
	const double  f6 = 1.42151175831645E-07;
		// 2.04426310338993978564E-15
	const double  f7 = 2.04426310338994E-15;

	//For poissapprox                            ' Stirling's series for Gamma(x), A001163/A001164
	const double  coef15 = 1.0 / 12.0;
	const double  coef25 = 1.0 / 288.0;
	const double  coef35 = -139.0 / 51840.0;
	const double  coef45 = -571.0 / 2488320.0;
	const double  coef55 = 163879.0 / 209018880.0;
	const double  coef65 = 5246819.0 / 75246796800.0;
	const double  coef75 = -534703531.0 / 902961561600.0;
		// Ramanujan's series for Gamma(x+1,x)-Gamma(x+1)/2, A065973
	const double  coef1 = 2.0 / 3.0;
		// cf. http://www.whim.org/nebula/math/gammaratio.html
	const double  coef2 = -4.0 / 135.0;
	const double  coef3 = 8.0 / 2835.0;
	const double  coef4 = 16.0 / 8505.0;
	const double  coef5 = -8992.0 / 12629925.0;
	const double  coef6 = -334144.0 / 492567075.0;
	const double  coef7 = 698752.0 / 1477701225.0;

	const double  coef8 = 23349012224.0 / 39565450299375.0;
	const double  twoThirds = 2.0 / 3.0;
	const double  twoFifths = 2.0 / 5.0;
	const double  twoSevenths = 2.0 / 7.0;
	const double  twoNinths = 2.0 / 9.0;
	const double  twoElevenths = 2.0 / 11.0;

	const double  twoThirteenths = 2.0 / 13.0;
	//For binapprox
	const double  oneThird = 1.0 / 3.0;
		// 2^27
    const double twoTo27 = 134217728.0;

	//For lngammaexpansion
		// 0.5772156649015328606065120901
	const double eulers_const = 0.577215664901533;

    private static double Min(double x, double y)
    {
        double functionReturnValue = 0;
        if (x < y)
        {
            functionReturnValue = x;
        }
        else
        {
            functionReturnValue = y;
        }
        return functionReturnValue;
    }
    private static double max(double x, double y)
    {
        double functionReturnValue = 0;
        if (x > y)
        {
            functionReturnValue = x;
        }
        else
        {
            functionReturnValue = y;
        }
        return functionReturnValue;
    }

    private static double expm1(double x)
    {
        double functionReturnValue = 0;
        //// Accurate calculation of exp(x)-1, particularly for small x.
        //// Uses a variation of the standard continued fraction for tanh(x) see A&S 4.5.70.
        if ((Math.Abs(x) < 2))
        {
            double a1 = 0;
            double a2 = 0;
            double b1 = 0;
            double b2 = 0;
            double c1 = 0;
            double x2 = 0;
            a1 = 24.0;
            b1 = 2.0 * (12.0 - x * (6.0 - x));
            x2 = x * x * 0.25;
            a2 = 8.0 * (15.0 + x2);
            b2 = 120.0 - x * (60.0 - x * (12.0 - x));
            c1 = 7.0;


            while (((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfSmall * b1 * a2))))
            {
                a1 = c1 * a2 + x2 * a1;
                b1 = c1 * b2 + x2 * b1;
                c1 = c1 + 2.0;

                a2 = c1 * a1 + x2 * a2;
                b2 = c1 * b1 + x2 * b2;
                c1 = c1 + 2.0;
                if ((b2 > scalefactor))
                {
                    a1 = a1 * scalefactor2;
                    b1 = b1 * scalefactor2;
                    a2 = a2 * scalefactor2;
                    b2 = b2 * scalefactor2;
                }
            }

            functionReturnValue = x * a2 / b2;
        }
        else
        {
            functionReturnValue = Math.Exp(x) - 1.0;
        }
        return functionReturnValue;

    }

    private static double logcf(double x, double i, double D)
    {
        //// Continued fraction for calculation of 1/i + x/(i+d) + x*x/(i+2*d) + x*x*x/(i+3d) + ...
        double a1 = 0;
        double a2 = 0;
        double b1 = 0;
        double b2 = 0;
        double c1 = 0;
        double c2 = 0;
        double c3 = 0;
        double c4 = 0;
        c1 = 2.0 * D;
        c2 = i + D;
        c4 = c2 + D;
        a1 = c2;
        b1 = i * (c2 - i * x);
        b2 = D * D * x;
        a2 = c4 * c2 - b2;
        b2 = c4 * b1 - i * b2;


        while (((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfVSmall * b1 * a2))))
        {
            c3 = c2 * c2 * x;
            c2 = c2 + D;
            c4 = c4 + D;
            a1 = c4 * a2 - c3 * a1;
            b1 = c4 * b2 - c3 * b1;

            c3 = c1 * c1 * x;
            c1 = c1 + D;
            c4 = c4 + D;
            a2 = c4 * a1 - c3 * a2;
            b2 = c4 * b1 - c3 * b2;
            if ((b2 > scalefactor))
            {
                a1 = a1 * scalefactor2;
                b1 = b1 * scalefactor2;
                a2 = a2 * scalefactor2;
                b2 = b2 * scalefactor2;
            }
            else if ((b2 < scalefactor2))
            {
                a1 = a1 * scalefactor;
                b1 = b1 * scalefactor;
                a2 = a2 * scalefactor;
                b2 = b2 * scalefactor;
            }
        }
        return a2 / b2;
    }

    private static double log0(double x)
    {
        double functionReturnValue = 0;
        ////Accurate calculation of log(1+x), particularly for small x.
        double term = 0;
        if ((Math.Abs(x) > 0.5))
        {
            functionReturnValue = Math.Log(1.0 + x);
        }
        else
        {
            term = x / (2.0 + x);
            functionReturnValue = 2.0 * term * logcf(term * term, 1.0, 2.0);
        }
        return functionReturnValue;
    }

    private static double log1(double x)
    {
        double functionReturnValue = 0;
        ////Accurate calculation of log(1+x)-x, particularly for small x.
        double term = 0;
        double y = 0;
        if ((Math.Abs(x) < 0.01))
        {
            term = x / (2.0 + x);
            y = term * term;
            functionReturnValue = term * ((((2.0 / 9.0 * y + 2.0 / 7.0) * y + 0.4) * y + 2.0 / 3.0) * y - x);
        }
        else if ((x < minLog1Value | x > 1.0))
        {
            functionReturnValue = Math.Log(1.0 + x) - x;
        }
        else
        {
            term = x / (2.0 + x);
            y = term * term;
            functionReturnValue = term * (2.0 * y * logcf(y, 3.0, 2.0) - x);
        }
        return functionReturnValue;
    }

    private static double logfbitdif(double x)
    {
        ////Calculation of logfbit(x)-logfbit(1+x). x must be > -1.
        double y = 0;
        double y2 = 0;
        y = 1.0 / (2.0 * x + 3.0);
        y2 = y * y;
        return y2 * logcf(y2, 3.0, 2.0);
    }

    private static double logfbit(double x)
    {
        double functionReturnValue = 0;
        ////Error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
        ////Are we ever concerned about the relative error involved in this function? I don't think so.
        double x1 = 0;
        double x2 = 0;
        double x3 = 0;
        if ((x >= 10000000000.0))
        {
            functionReturnValue = lfbc1 / (x + 1.0);
            // Abramowitz & Stegun's series 6.1.41
        }
        else if ((x >= 6.0))
        {
            x1 = x + 1.0;
            x2 = 1.0 / (x1 * x1);
            x3 = x2 * (lfbc6 - x2 * (lfbc7 - x2 * (lfbc8 - x2 * lfbc9)));
            x3 = x2 * (lfbc4 - x2 * (lfbc5 - x3));
            x3 = x2 * (lfbc2 - x2 * (lfbc3 - x3));
            functionReturnValue = lfbc1 * (1.0 - x3) / x1;
        }
        else if ((x == 5.0))
        {
            functionReturnValue = 0.0138761288230707;
            // 1.3876128823070747998745727023763E-02  ' calculated to give exact factorials
        }
        else if ((x == 4.0))
        {
            functionReturnValue = 0.0166446911898212;
            // 1.6644691189821192163194865373593E-02
        }
        else if ((x == 3.0))
        {
            functionReturnValue = 0.0207906721037651;
            // 2.0790672103765093111522771767849E-02
        }
        else if ((x == 2.0))
        {
            functionReturnValue = 0.0276779256849983;
            // 2.7677925684998339148789292746245E-02
        }
        else if ((x == 1.0))
        {
            functionReturnValue = 0.0413406959554093;
            // 4.1340695955409294093822081407118E-02
        }
        else if ((x == 0.0))
        {
            functionReturnValue = 0.0810614667953273;
            // 8.1061466795327258219670263594382E-02
        }
        else if ((x > -1.0))
        {
            x1 = x;
            x2 = 0.0;
            while ((x1 < 6.0))
            {
                x2 = x2 + logfbitdif(x1);
                x1 = x1 + 1.0;
            }

            functionReturnValue = x2 + logfbit(x1);
        }
        else
        {
            functionReturnValue = 1E308;
        }
        return functionReturnValue;
    }

    private static double logdif(double pr, double prob)
    {
        double functionReturnValue = 0;
        double temp = 0;
        temp = (pr - prob) / prob;
        if (Math.Abs(temp) >= 0.5)
        {
            functionReturnValue = Math.Log(pr / prob);
        }
        else
        {
            functionReturnValue = log0(temp);
        }
        return functionReturnValue;
    }

    private static double cnormal(double x)
    {
        double functionReturnValue = 0;
        ////Probability that a normal variate <= x
        double acc = 0;
        double x2 = 0;
        double D = 0;
        double term = 0;
        double a1 = 0;
        double a2 = 0;
        double b1 = 0;
        double b2 = 0;
        double c1 = 0;
        double c2 = 0;
        double c3 = 0;

        if ((Math.Abs(x) < 1.5))
        {
            acc = 0.0;
            x2 = x * x;
            term = 1.0;
            D = 3.0;


            while ((term > sumAcc * acc))
            {
                D = D + 2.0;
                term = term * x2 / D;
                acc = acc + term;

            }

            acc = 1.0 + x2 / 3.0 * (1.0 + acc);
            functionReturnValue = 0.5 + Math.Exp(-x * x * 0.5 - lstpi) * x * acc;
        }
        else if ((Math.Abs(x) > 40.0))
        {
            if ((x > 0.0))
            {
                functionReturnValue = 1.0;
            }
            else
            {
                functionReturnValue = 0.0;
            }
        }
        else
        {
            x2 = x * x;
            a1 = 2.0;
            b1 = x2 + 5.0;
            c2 = x2 + 9.0;
            a2 = a1 * c2;
            b2 = b1 * c2 - 12.0;
            c1 = 5.0;
            c2 = c2 + 4.0;


            while (((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfVSmall * b1 * a2))))
            {
                c3 = c1 * (c1 + 1.0);
                a1 = c2 * a2 - c3 * a1;
                b1 = c2 * b2 - c3 * b1;
                c1 = c1 + 2.0;
                c2 = c2 + 4.0;
                c3 = c1 * (c1 + 1.0);
                a2 = c2 * a1 - c3 * a2;
                b2 = c2 * b1 - c3 * b2;
                c1 = c1 + 2.0;
                c2 = c2 + 4.0;
                if ((b2 > scalefactor))
                {
                    a1 = a1 * scalefactor2;
                    b1 = b1 * scalefactor2;
                    a2 = a2 * scalefactor2;
                    b2 = b2 * scalefactor2;
                }

            }

            if ((x > 0.0))
            {
                functionReturnValue = 1.0 - Math.Exp(-x * x * 0.5 - lstpi) * x / (x2 + 1.0 - a2 / b2);
            }
            else
            {
                functionReturnValue = -Math.Exp(-x * x * 0.5 - lstpi) * x / (x2 + 1.0 - a2 / b2);
            }

        }
        return functionReturnValue;
    }

    private static double invcnormal(double p)
    {
        ////Inverse of cnormal from AS241.
        ////Require p to be strictly in the range 0..1

        double PPND16 = 0;
        double Q = 0;
        double r = 0;
        Q = p - 0.5;
        if ((Math.Abs(Q) <= 0.425))
        {
            r = 0.180625 - Q * Q;
            PPND16 = Q * (((((((a7 * r + a6) * r + a5) * r + a4) * r + a3) * r + a2) * r + a1) * r + a0) / (((((((b7 * r + b6) * r + b5) * r + b4) * r + b3) * r + b2) * r + b1) * r + 1.0);
        }
        else
        {
            if ((Q < 0.0))
            {
                r = p;
            }
            else
            {
                r = 1.0 - p;
            }
            r = Math.Sqrt(-Math.Log(r));
            if ((r <= 5.0))
            {
                r = r - 1.6;
                PPND16 = (((((((c7 * r + c6) * r + c5) * r + c4) * r + c3) * r + c2) * r + c1) * r + c0) / (((((((d7 * r + d6) * r + d5) * r + d4) * r + d3) * r + d2) * r + d1) * r + 1.0);
            }
            else
            {
                r = r - 5.0;
                PPND16 = (((((((e7 * r + e6) * r + e5) * r + e4) * r + e3) * r + e2) * r + e1) * r + e0) / (((((((f7 * r + f6) * r + f5) * r + f4) * r + f3) * r + f2) * r + f1) * r + 1.0);
            }
            if ((Q < 0.0))
            {
                PPND16 = -PPND16;
            }
        }
        return PPND16;
    }

    //private static  bool even(double x)
    //{
    //    return (Convert.ToInt32(x / 2.0) * 2.0 == x);
    //}

    //private static  double tdistexp(double p, double Q, double logqk2, double k, ref double tdistDensity)
    //{
    //    ////Special transformation of t-distribution useful for BinApprox.
    //    ////Note approxtdistDens only used by binApprox if k > 100 or so.
    //    double sum = 0;
    //    double aki = 0;
    //    double Ai = 0;
    //    double term = 0;
    //    double q1 = 0;
    //    double q8 = 0;
    //    double c1 = 0;
    //    double c2 = 0;
    //    double a1 = 0;
    //    double a2 = 0;
    //    double b1 = 0;
    //    double b2 = 0;
    //    double cadd = 0;
    //    double Result = 0;
    //    double approxtdistDens = 0;

    //    if ((even(k))) {
    //        approxtdistDens = Math.Exp(logqk2 + logfbit(k - 1.0) - 2.0 * logfbit(k * 0.5 - 1.0) - lstpi);
    //    } else {
    //        approxtdistDens = Math.Exp(logqk2 + k * log1(1.0 / k) + 2.0 * logfbit((k - 1.0) * 0.5) - logfbit(k - 1.0) - lstpi);
    //    }

    //    if ((k * p < 4.0 * Q)) {
    //        sum = 0.0;
    //        aki = k + 1.0;
    //        Ai = 3.0;
    //        term = 1.0;


    //        while ((term > sumAcc * sum)) {
    //            Ai = Ai + 2.0;
    //            aki = aki + 2.0;
    //            term = term * aki * p / Ai;
    //            sum = sum + term;

    //        }

    //        sum = 1.0 + (k + 1.0) * p * (1.0 + sum) / 3.0;
    //        Result = 0.5 - approxtdistDens * sum * Math.Sqrt(k * p);
    //    } else if (approxtdistDens == 0.0) {
    //        Result = 0.0;
    //    } else {
    //        q1 = 2.0 * (1.0 + Q);
    //        q8 = 8.0 * Q;
    //        a1 = 1.0;
    //        b1 = (k - 3.0) * p + 7.0;
    //        c1 = -20.0 * Q;
    //        a2 = (k - 5.0) * p + 11.0;
    //        b2 = a2 * b1 + c1;
    //        cadd = -30.0 * Q;
    //        c1 = -42.0 * Q;
    //        c2 = (k - 7.0) * p + 15.0;


    //        while (((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfVSmall * b1 * a2)))) {
    //            a1 = c2 * a2 + c1 * a1;
    //            b1 = c2 * b2 + c1 * b1;
    //            c1 = c1 + cadd;
    //            cadd = cadd - q8;
    //            c2 = c2 + q1;
    //            a2 = c2 * a1 + c1 * a2;
    //            b2 = c2 * b1 + c1 * b2;
    //            c1 = c1 + cadd;
    //            cadd = cadd - q8;
    //            c2 = c2 + q1;
    //            if ((Math.Abs(b2) > scalefactor)) {
    //                a1 = a1 * scalefactor2;
    //                b1 = b1 * scalefactor2;
    //                a2 = a2 * scalefactor2;
    //                b2 = b2 * scalefactor2;
    //            } else if ((Math.Abs(b2) < scalefactor2)) {
    //                a1 = a1 * scalefactor;
    //                b1 = b1 * scalefactor;
    //                a2 = a2 * scalefactor;
    //                b2 = b2 * scalefactor;
    //            }
    //        }

    //        Result = approxtdistDens * (1.0 - Q / ((k - 1.0) * p + 3.0 - 6.0 * Q * a2 / b2)) / Math.Sqrt(k * p);
    //    }
    //    tdistDensity = approxtdistDens * Math.Sqrt(Q);
    //    return Result;
    //}

    //private static  double tdist(double x, double k, double tdistDensity)
    //{
    //    double functionReturnValue = 0;
    //    ////Probability that variate from t-distribution with k degress of freedom <= x
    //    double x2 = 0;
    //    double k2 = 0;
    //    double logterm = 0;
    //    double a = 0;
    //    double r = 0;

    //    if (Math.Abs(x) >= Min(1.0, k)) {
    //        k2 = k / x;
    //        x2 = x + k2;
    //        k2 = k2 / x2;
    //        x2 = x / x2;
    //    } else {
    //        x2 = x * x;
    //        k2 = k + x2;
    //        x2 = x2 / k2;
    //        k2 = k / k2;
    //    }
    //    if ((k > 1E+30)) {
    //        functionReturnValue = cnormal(x);
    //        tdistDensity = Math.Exp(-x * x / 2.0);
    //    } else {
    //        if ((k2 < cSmall)) {
    //            logterm = k * 0.5 * (Math.Log(k) - 2.0 * Math.Log(Math.Abs(x)));
    //        } else if ((Math.Abs(x2) < 0.5)) {
    //            logterm = k * 0.5 * log0(-x2);
    //        } else {
    //            logterm = k * 0.5 * Math.Log(k2);
    //        }
    //        if ((k >= 1.0)) {
    //            if ((x < 0.0)) {
    //                functionReturnValue = tdistexp(x2, k2, logterm, k, ref tdistDensity);
    //            } else {
    //                functionReturnValue = 1.0 - tdistexp(x2, k2, logterm, k, ref tdistDensity);
    //            }
    //            return functionReturnValue;
    //        }
    //        a = k / 2.0;
    //        tdistDensity = Math.Exp(0.5 + (1.0 + 1.0 / k) * logterm + a * log0(-0.5 / (a + 1.0)) + logfbit(a - 0.5) - logfbit(a)) * Math.Sqrt(a / ((1.0 + a))) * OneOverSqrTwoPi;
    //        if ((k2 < cSmall)) {
    //            r = (a + 1.0) * log1(a / 1.5) + (logfbit(a + 0.5) - logfbit0p5) - lngammaexpansion(a);
    //            r = r + a * ((a - 0.5) / 1.5 + Log1p5 + (Math.Log(k) - 2.0 * Math.Log(Math.Abs(x))));
    //            r = Math.Exp(r) * (0.25 / (a + 0.5));
    //            if (x < 0.0) {
    //                functionReturnValue = r;
    //            } else {
    //                functionReturnValue = 1.0 - r;
    //            }
    //        } else if ((x < 0.0)) {
    //            if (x2 < k2) {
    //                functionReturnValue = 0.5 * compbeta(x2, 0.5, a);
    //            } else {
    //                functionReturnValue = 0.5 * BETA(k2, a, 0.5);
    //            }
    //        } else {
    //            if (x2 < k2) {
    //                functionReturnValue = 0.5 * (1.0 + BETA(x2, 0.5, a));
    //            } else {
    //                functionReturnValue = 0.5 * (1.0 + compbeta(k2, a, 0.5));
    //            }
    //        }
    //    }
    //    return functionReturnValue;
    //}

    //private static  bool BetterThanTailApprox(double prob, double df)
    //{
    //    bool functionReturnValue = false;
    //    if (df <= 2) {
    //        functionReturnValue = prob > 0.25 * Math.Exp((1.0 - df) * 1.78514841051368);
    //    } else if (df <= 5) {
    //        functionReturnValue = prob > 0.045 * Math.Exp((2.0 - df) * 1.30400766847605);
    //    } else if (df <= 20) {
    //        functionReturnValue = prob > 0.0009 * Math.Exp((5.0 - df) * 0.921034037197618);
    //    } else {
    //        functionReturnValue = prob > 9E-10 * Math.Exp((20.0 - df) * 0.690775527898214);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double invtdist(double prob, double df)
    //{
    //    double functionReturnValue = 0;
    //    ////Inverse of tdist
    //    ////Require prob to be in the range 0..1 df should be positive
    //    double xn = 0;
    //    double xn2 = 0;
    //    double tp = 0;
    //    double tpDif = 0;
    //    double tprob = 0;
    //    double a = 0;
    //    double pr = 0;
    //    double lpr = 0;
    //    double small = 0;
    //    double smalllpr = 0;
    //    double tdistDensity = 0;
    //    if (prob > 0.5) {
    //        pr = 1.0 - prob;
    //    } else {
    //        pr = prob;
    //    }
    //    lpr = -Math.Log(pr);
    //    small = 1E-14;
    //    smalllpr = small * lpr * pr;
    //    if (pr >= 0.5 | df >= 1.0 & BetterThanTailApprox(pr, df)) {
    //        //// Will divide by 0 if tp so small that tdistDensity underflows. Not a problem if prob > cSmall
    //        xn = invcnormal(pr);
    //        xn2 = xn * xn;
    //        ////Initial approximation is given in http://digital.library.adelaide.edu.au/coll/special//fisher/281.pdf. The modified NR correction then gets it right.
    //        tp = (((((27.0 * xn2 + 339.0) * xn2 + 930.0) * xn2 - 1782.0) * xn2 - 765.0) * xn2 + 17955.0) / (368640.0 * df);
    //        tp = (tp + ((((79.0 * xn2 + 776.0) * xn2 + 1482.0) * xn2 - 1920.0) * xn2 - 945.0) / 92160.0) / df;
    //        tp = (tp + (((3.0 * xn2 + 19.0) * xn2 + 17.0) * xn2 - 15.0) / 384.0) / df;
    //        tp = (tp + ((5.0 * xn2 + 16) * xn2 + 3.0) / 96.0) / df;
    //        tp = (tp + (xn2 + 1.0) / 4.0) / df;
    //        tp = xn * (1.0 + tp);
    //        tprob = 0.0;
    //        tpDif = 1.0 + Math.Abs(tp);
    //    } else if (df < 1.0) {
    //        a = df / 2.0;
    //        tp = (a + 1.0) * log1(a / 1.5) + (logfbit(a + 0.5) - logfbit0p5) - lngammaexpansion(a);
    //        tp = ((a - 0.5) / 1.5 + Log1p5 + Math.Log(df)) / 2.0 + (tp - Math.Log(4.0 * pr * (a + 0.5))) / df;
    //        tp = -Math.Exp(tp);
    //        tprob = tdist(tp, df, tdistDensity);
    //        if (tdistDensity < nearly_zero) {
    //            tpDif = 0.0;
    //        } else {
    //            tpDif = (tprob / tdistDensity) * log0((tprob - pr) / pr);
    //            tp = tp - tpDif;
    //        }
    //    } else {
    //        tp = tdist(0, df, tdistDensity);
    //        //Marginally quicker to get tdistDensity for integral df
    //        tp = Math.Exp(-Math.Log(Math.Sqrt(df) * pr / tdistDensity) / df);
    //        if (df >= 2) {
    //            tp = -Math.Sqrt(df * (tp * tp - 1.0));
    //        } else {
    //            tp = -Math.Sqrt(df) * Math.Sqrt(tp - 1.0) * Math.Sqrt(tp + 1.0);
    //        }
    //        tpDif = tp / df;
    //        tpDif = -log0((0.5 - 1.0 / (df + 2)) / (1.0 + tpDif * tp)) * (tpDif + 1.0 / tp);
    //        tp = tp - tpDif;
    //        tprob = tdist(tp, df, tdistDensity);
    //        if (tdistDensity < nearly_zero) {
    //            tpDif = 0.0;
    //        } else {
    //            tpDif = (tprob / tdistDensity) * log0((tprob - pr) / pr);
    //            tp = tp - tpDif;
    //        }
    //    }
    //    while ((Math.Abs(tprob - pr) > smalllpr & Math.Abs(tpDif) > small * (1.0 + Math.Abs(tp)))) {
    //        tprob = tdist(tp, df, tdistDensity);
    //        tpDif = (tprob / tdistDensity) * log0((tprob - pr) / pr);
    //        tp = tp - tpDif;
    //    }
    //    functionReturnValue = tp;
    //    if (prob > 0.5)
    //        functionReturnValue = -functionReturnValue;
    //    return functionReturnValue;
    //}

    private static double poissonTerm(double i, double N, double diffFromMean, double logAdd)
    {
        double functionReturnValue = 0;
        ////Probability that poisson variate with mean n has value i (diffFromMean = n-i)
        double c2 = 0;
        double c3 = 0;
        double logpoissonTerm = 0;
        double c1 = 0;

        if (((i <= -1.0) | (N < 0.0)))
        {
            if ((i == 0.0))
            {
                functionReturnValue = Math.Exp(logAdd);
            }
            else
            {
                functionReturnValue = 0.0;
            }
        }
        else if (((i < 0.0) & (N == 0.0)))
        {
            functionReturnValue = ErrorValue;
        }
        else
        {
            c3 = i;
            c2 = c3 + 1.0;
            c1 = (diffFromMean - 1.0) / c2;

            if ((c1 < minLog1Value))
            {
                if ((i == 0.0))
                {
                    logpoissonTerm = -N;
                    functionReturnValue = Math.Exp(logpoissonTerm + logAdd);
                }
                else if ((N == 0.0))
                {
                    functionReturnValue = 0.0;
                }
                else
                {
                    logpoissonTerm = (c3 * Math.Log(N / c2) - (diffFromMean - 1.0)) - logfbit(c3);
                    functionReturnValue = Math.Exp(logpoissonTerm + logAdd) / Math.Sqrt(c2) * OneOverSqrTwoPi;
                }
            }
            else
            {
                logpoissonTerm = c3 * log1(c1) - c1 - logfbit(c3);
                functionReturnValue = Math.Exp(logpoissonTerm + logAdd) / Math.Sqrt(c2) * OneOverSqrTwoPi;
            }
        }
        return functionReturnValue;
    }

    private static double poisson1(double i, double N, double diffFromMean)
    {
        double functionReturnValue = 0;
        ////Probability that poisson variate with mean n has value <= i (diffFromMean = n-i)
        ////For negative values of i (used for calculating the cumlative gamma distribution) there's a really nasty interpretation!
        ////1-gamma(n,i) is calculated as poisson1(-i,n,0) since we need an accurate version of i rather than i-1.
        ////Uses a simplified version of Legendre's continued fraction.
        double prob = 0;
        bool exact = false;
        if (((i >= 0.0) & (N <= 0.0)))
        {
            exact = true;
            prob = 1.0;
        }
        else if (((i > -1.0) & (N <= 0.0)))
        {
            exact = true;
            prob = 0.0;
        }
        else if (((i > -1.0) & (i < 0.0)))
        {
            i = -i;
            exact = false;
            prob = poissonTerm(i, N, N - i, 0.0) * i / N;
            i = i - 1.0;
            diffFromMean = N - i;
        }
        else
        {
            exact = ((i <= -1.0) | (N < 0.0));
            prob = poissonTerm(i, N, diffFromMean, 0.0);
        }
        if ((exact | prob == 0.0))
        {
            functionReturnValue = prob;
            return functionReturnValue;
        }

        double a1 = 0;
        double a2 = 0;
        double b1 = 0;
        double b2 = 0;
        double c1 = 0;
        double c2 = 0;
        double c3 = 0;
        double c4 = 0;
        double cfValue = 0;
        long njj = 0;
        long Numb = 0;
        long sumAlways = 0;
        long sumFactor = 0;
        sumAlways = 0;
        sumFactor = 6;
        a1 = 0.0;
        if ((i > sumAlways))
        {
            Numb = Convert.ToInt32(sumFactor * Math.Exp(Math.Log(N) / 3));
            Numb = (long)max(0, Convert.ToInt32(Numb - diffFromMean));
            if ((Numb > i))
            {
                Numb = Convert.ToInt32(i);
            }
        }
        else
        {
            Numb = (long)max(0, Convert.ToInt32(i));
        }

        b1 = 1.0;
        a2 = i - Numb;
        b2 = diffFromMean + (Numb + 1.0);
        c1 = 0.0;
        c2 = a2;
        c4 = b2;
        if (c2 < 0.0)
        {
            cfValue = cfVSmall;
        }
        else
        {
            cfValue = cfSmall;
        }

        while (((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfValue * b1 * a2))))
        {
            c1 = c1 + 1.0;
            c2 = c2 - 1.0;
            c3 = c1 * c2;
            c4 = c4 + 2.0;
            a1 = c4 * a2 + c3 * a1;
            b1 = c4 * b2 + c3 * b1;
            c1 = c1 + 1.0;
            c2 = c2 - 1.0;
            c3 = c1 * c2;
            c4 = c4 + 2.0;
            a2 = c4 * a1 + c3 * a2;
            b2 = c4 * b1 + c3 * b2;
            if ((b2 > scalefactor))
            {
                a1 = a1 * scalefactor2;
                b1 = b1 * scalefactor2;
                a2 = a2 * scalefactor2;
                b2 = b2 * scalefactor2;
            }
            if (c2 < 0.0 & cfValue > cfVSmall)
            {
                cfValue = cfVSmall;
            }
        }

        a1 = a2 / b2;

        c1 = i - Numb + 1.0;
        for (njj = 1; njj <= Numb; njj++)
        {
            a1 = (1.0 + a1) * (c1 / N);
            c1 = c1 + 1.0;
        }

        functionReturnValue = (1.0 + a1) * prob;
        return functionReturnValue;
    }

    private static double poisson2(double i, double N, double diffFromMean)
    {
        double functionReturnValue = 0;
        ////Probability that poisson variate with mean n has value >= i (diffFromMean = n-i)
        double prob = 0;
        bool exact = false;
        if (((i <= 0.0) & (N <= 0.0)))
        {
            exact = true;
            prob = 1.0;
        }
        else
        {
            exact = false;
            prob = poissonTerm(i, N, diffFromMean, 0.0);
        }
        if ((exact | prob == 0.0))
        {
            functionReturnValue = prob;
            return functionReturnValue;
        }

        double a1 = 0;
        double a2 = 0;
        double b1 = 0;
        double b2 = 0;
        double c1 = 0;
        double c2 = 0;
        long njj = 0;
        long Numb = 0;
        const long sumFactor = 6;
        Numb = Convert.ToInt32(sumFactor * Math.Exp(Math.Log(N) / 3));
        Numb = (long)max(0, Convert.ToInt32(diffFromMean + Numb));

        a1 = 0.0;
        b1 = 1.0;
        a2 = N;
        b2 = (Numb + 1.0) - diffFromMean;
        c1 = 0.0;
        c2 = b2;


        while (((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfSmall * b1 * a2))))
        {
            c1 = c1 + N;
            c2 = c2 + 1.0;
            a1 = c2 * a2 + c1 * a1;
            b1 = c2 * b2 + c1 * b1;
            c1 = c1 + N;
            c2 = c2 + 1.0;
            a2 = c2 * a1 + c1 * a2;
            b2 = c2 * b1 + c1 * b2;
            if ((b2 > scalefactor))
            {
                a1 = a1 * scalefactor2;
                b1 = b1 * scalefactor2;
                a2 = a2 * scalefactor2;
                b2 = b2 * scalefactor2;
            }
        }

        a1 = a2 / b2;

        c1 = i + Numb;
        for (njj = 1; njj <= Numb; njj++)
        {
            a1 = (1.0 + a1) * (N / c1);
            c1 = c1 - 1.0;
        }

        functionReturnValue = (1.0 + a1) * prob;
        return functionReturnValue;

    }

    private static double poissonApprox(double j, double diffFromMean, bool comp)
    {
        double functionReturnValue = 0;
        ////Asymptotic expansion to calculate the probability that poisson variate has value <= j (diffFromMean = mean-j). If comp then calulate 1-probability.
        ////cf. http://members.aol.com/iandjmsmith/PoissonApprox.htm
        double pt = 0;
        double s2pt = 0;
        double res1 = 0;
        double res2 = 0;
        double elfb = 0;
        double term = 0;
        double ig2 = 0;
        double ig3 = 0;
        double ig4 = 0;
        double ig5 = 0;
        double ig6 = 0;
        double ig7 = 0;
        double ig8 = 0;
        double ig05 = 0;
        double ig25 = 0;
        double ig35 = 0;
        double ig45 = 0;
        double ig55 = 0;
        double ig65 = 0;
        double ig75 = 0;

        pt = -log1(diffFromMean / j);
        s2pt = Math.Sqrt(2.0 * j * pt);

        ig2 = 1.0 / j + pt;
        term = pt * pt * 0.5;
        ig3 = ig2 / j + term;
        term = term * pt / 3.0;
        ig4 = ig3 / j + term;
        term = term * pt / 4.0;
        ig5 = ig4 / j + term;
        term = term * pt / 5.0;
        ig6 = ig5 / j + term;
        term = term * pt / 6.0;
        ig7 = ig6 / j + term;
        term = term * pt / 7.0;
        ig8 = ig7 / j + term;

        ig05 = cnormal(-s2pt);
        term = pt * twoThirds;
        ig25 = 1.0 / j + term;
        term = term * pt * twoFifths;
        ig35 = ig25 / j + term;
        term = term * pt * twoSevenths;
        ig45 = ig35 / j + term;
        term = term * pt * twoNinths;
        ig55 = ig45 / j + term;
        term = term * pt * twoElevenths;
        ig65 = ig55 / j + term;
        term = term * pt * twoThirteenths;
        ig75 = ig65 / j + term;

        elfb = ((((((coef75 / j + coef65) / j + coef55) / j + coef45) / j + coef35) / j + coef25) / j + coef15) + j;
        res1 = (((((((ig8 * coef8 + ig7 * coef7) + ig6 * coef6) + ig5 * coef5) + ig4 * coef4) + ig3 * coef3) + ig2 * coef2) + coef1) * Math.Sqrt(j);
        res2 = ((((((ig75 * coef75 + ig65 * coef65) + ig55 * coef55) + ig45 * coef45) + ig35 * coef35) + ig25 * coef25) + coef15) * s2pt;

        if ((comp))
        {
            if ((diffFromMean < 0.0))
            {
                functionReturnValue = ig05 - (res1 - res2) * Math.Exp(-j * pt - lstpi) / elfb;
            }
            else
            {
                functionReturnValue = (1.0 - ig05) - (res1 + res2) * Math.Exp(-j * pt - lstpi) / elfb;
            }
        }
        else if ((diffFromMean < 0.0))
        {
            functionReturnValue = (1.0 - ig05) + (res1 - res2) * Math.Exp(-j * pt - lstpi) / elfb;
        }
        else
        {
            functionReturnValue = ig05 + (res1 + res2) * Math.Exp(-j * pt - lstpi) / elfb;
        }
        return functionReturnValue;
    }

    private static double cpoisson(double k, double Lambda, double dfm)
    {
        double functionReturnValue = 0;
        ////Probability that poisson variate with mean lambda has value <= k (diffFromMean = lambda-k) calculated by various methods.
        if (((k >= 21.0) & (Math.Abs(dfm) < (0.3 * k))))
        {
            functionReturnValue = poissonApprox(k, dfm, false);
        }
        else if (((Lambda > k) & (Lambda >= 1.0)))
        {
            functionReturnValue = poisson1(k, Lambda, dfm);
        }
        else
        {
            functionReturnValue = 1.0 - poisson2(k + 1.0, Lambda, dfm - 1.0);
        }
        return functionReturnValue;
    }

    private static double comppoisson(double k, double Lambda, double dfm)
    {
        double functionReturnValue = 0;
        ////Probability that poisson variate with mean lambda has value > k (diffFromMean = lambda-k) calculated by various methods.
        if (((k >= 21.0) & (Math.Abs(dfm) < (0.3 * k))))
        {
            functionReturnValue = poissonApprox(k, dfm, true);
        }
        else if (((Lambda > k) & (Lambda >= 1.0)))
        {
            functionReturnValue = 1.0 - poisson1(k, Lambda, dfm);
        }
        else
        {
            functionReturnValue = poisson2(k + 1.0, Lambda, dfm - 1.0);
        }
        return functionReturnValue;
    }

    private static double invpoisson(double k, double prob)
    {
        double functionReturnValue = 0;
        ////Inverse of poisson. Calculates mean such that poisson(k,mean,mean-k)=prob.
        ////Require prob to be in the range 0..1, k should be -1/2 or non-negative
        if ((k == 0.0))
        {
            functionReturnValue = -Math.Log(prob + 9.99988867182683E-321);
        }
        else if ((prob > 0.5))
        {
            functionReturnValue = invcomppoisson(k, 1.0 - prob);
        }
        else
        {
            double temp2 = 0;
            double xp = 0;
            double dfm = 0;
            double Q = 0;
            double qdif = 0;
            double lpr = 0;
            double small = 0;
            double smalllpr = 0;
            lpr = -Math.Log(prob);
            small = 1E-14;
            smalllpr = small * lpr * prob;
            xp = invcnormal(prob);
            dfm = 0.5 * xp * (xp - Math.Sqrt(4.0 * k + xp * xp));
            Q = -1.0;
            qdif = -dfm;
            if (Math.Abs(qdif) < 1.0)
            {
                qdif = 1.0;
            }
            else if ((k > 1E50))
            {
                functionReturnValue = k;
                return functionReturnValue;
            }
            while (((Math.Abs(Q - prob) > smalllpr) & (Math.Abs(qdif) > (1.0 + Math.Abs(dfm)) * small)))
            {
                Q = cpoisson(k, k + dfm, dfm);
                if ((Q == 0.0))
                {
                    qdif = qdif / 2.0;
                    dfm = dfm + qdif;
                    Q = -1.0;
                }
                else
                {
                    temp2 = poissonTerm(k, k + dfm, dfm, 0.0);
                    if ((temp2 == 0.0))
                    {
                        qdif = qdif / 2.0;
                        dfm = dfm + qdif;
                        Q = -1.0;
                    }
                    else
                    {
                        qdif = -2.0 * Q * logdif(Q, prob) / (1.0 + Math.Sqrt(Math.Log(prob) / Math.Log(Q))) / temp2;
                        if ((qdif > k + dfm))
                        {
                            qdif = dfm / 2.0;
                            dfm = dfm - qdif;
                            Q = -1.0;
                        }
                        else
                        {
                            dfm = dfm - qdif;
                        }
                    }
                }
            }
            functionReturnValue = k + dfm;
        }
        return functionReturnValue;
    }

    private static  double invcomppoisson(double k, double prob)
    {
        double functionReturnValue = 0;
        ////Inverse of comppoisson. Calculates mean such that comppoisson(k,mean,mean-k)=prob.
        ////Require prob to be in the range 0..1, k should be -1/2 or non-negative
        if ((prob > 0.5)) {
            functionReturnValue = invpoisson(k, 1.0 - prob);
        } else if ((k == 0.0)) {
            functionReturnValue = -log0(-prob);
        } else {
            double temp2 = 0;
            double xp = 0;
            double dfm = 0;
            double Q = 0;
            double qdif = 0;
            double Lambda = 0;
            bool qdifset = false;
            double lpr = 0;
            double small = 0;
            double smalllpr = 0;
            lpr = -Math.Log(prob);
            small = 1E-14;
            smalllpr = small * lpr * prob;
            xp = invcnormal(prob);
            dfm = 0.5 * xp * (xp + Math.Sqrt(4.0 * k + xp * xp));
            Lambda = k + dfm;
            if (((Lambda < 1.0) & (k < 40.0))) {
                Lambda = Math.Exp(Math.Log(prob / poissonTerm(k + 1.0, 1.0, -k, 0.0)) / (k + 1.0));
                dfm = Lambda - k;
            } else if ((k > 1E50)) {
                functionReturnValue = Lambda;
                return functionReturnValue;
            }
            Q = -1.0;
            qdif = Lambda;
            qdifset = false;
            while (((Math.Abs(Q - prob) > smalllpr) & (Math.Abs(qdif) > Min(Lambda, Math.Abs(dfm)) * small))) {
                Q = comppoisson(k, Lambda, dfm);
                if ((Q == 0.0)) {
                    if (qdifset) {
                        qdif = qdif / 2.0;
                        dfm = dfm + qdif;
                        Lambda = Lambda + qdif;
                    } else {
                        Lambda = 2.0 * Lambda;
                        qdif = qdif * 2.0;
                        dfm = Lambda - k;
                    }
                    Q = -1.0;
                } else {
                    temp2 = poissonTerm(k, Lambda, dfm, 0.0);
                    if ((temp2 == 0.0)) {
                        if (qdifset) {
                            qdif = qdif / 2.0;
                            dfm = dfm + qdif;
                            Lambda = Lambda + qdif;
                        } else {
                            Lambda = 2.0 * Lambda;
                            qdif = qdif * 2.0;
                            dfm = Lambda - k;
                        }
                        Q = -1.0;
                    } else {
                        qdif = 2.0 * Q * logdif(Q, prob) / (1.0 + Math.Sqrt(Math.Log(prob) / Math.Log(Q))) / temp2;
                        if ((qdif > Lambda)) {
                            Lambda = Lambda / 10.0;
                            qdif = dfm;
                            dfm = Lambda - k;
                            qdif = qdif - dfm;
                            Q = -1.0;
                        } else {
                            Lambda = Lambda - qdif;
                            dfm = dfm - qdif;
                        }
                        qdifset = true;
                    }
                }
                if ((Math.Abs(dfm) > Lambda)) {
                    dfm = Lambda - k;
                } else {
                    Lambda = k + dfm;
                }
            }
            functionReturnValue = Lambda;
       }
       return functionReturnValue;
    }

    //private static  double binomialTerm(double i, double j, double p, double Q, double diffFromMean, double logAdd)
    //{
    //    double functionReturnValue = 0;
    //    ////Probability that binomial variate with sample size i+j and event prob p (=1-q) has value i (diffFromMean = (i+j)*p-i)
    //    double c1 = 0;
    //    double c2 = 0;
    //    double c3 = 0;
    //    double c4 = 0;
    //    double c5 = 0;
    //    double c6 = 0;
    //    double ps = 0;
    //    double logbinomialTerm = 0;
    //    double dfm = 0;
    //    if (((i == 0.0) & (j <= 0.0))) {
    //        functionReturnValue = Math.Exp(logAdd);
    //    } else if (((i <= -1.0) | (j < 0.0))) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        c1 = (i + 1.0) + j;
    //        if ((p < Q)) {
    //            c2 = i;
    //            c3 = j;
    //            ps = p;
    //            dfm = diffFromMean;
    //        } else {
    //            c3 = i;
    //            c2 = j;
    //            ps = Q;
    //            dfm = -diffFromMean;
    //        }

    //        c5 = (dfm - (1.0 - ps)) / (c2 + 1.0);
    //        c6 = -(dfm + ps) / (c3 + 1.0);

    //        if ((c5 < minLog1Value)) {
    //            if ((c2 == 0.0)) {
    //                logbinomialTerm = c3 * log0(-ps);
    //                functionReturnValue = Math.Exp(logbinomialTerm + logAdd);
    //            } else if (((ps == 0.0) & (c2 > 0.0))) {
    //                functionReturnValue = 0.0;
    //            } else {
    //                c4 = logfbit(i + j) - logfbit(i) - logfbit(j);
    //                logbinomialTerm = c4 + c2 * (Math.Log((ps * c1) / (c2 + 1.0)) - c5) - c5 + c3 * log1(c6) - c6;
    //                functionReturnValue = Math.Exp(logbinomialTerm + logAdd) * Math.Sqrt(c1 / ((c2 + 1.0) * (c3 + 1.0))) * OneOverSqrTwoPi;
    //            }
    //        } else {
    //            c4 = logfbit(i + j) - logfbit(i) - logfbit(j);
    //            logbinomialTerm = c4 + (c2 * log1(c5) - c5) + (c3 * log1(c6) - c6);
    //            functionReturnValue = Math.Exp(logbinomialTerm + logAdd) * Math.Sqrt((c1 / (c3 + 1.0)) / ((c2 + 1.0))) * OneOverSqrTwoPi;
    //        }
    //    }
    //    return functionReturnValue;
    //}

    //private static  double binomialcf(double ii, double jj, double pp, double qq, double diffFromMean, bool comp)
    //{
    //    double functionReturnValue = 0;
    //    ////Probability that binomial variate with sample size ii+jj and event prob pp (=1-qq) has value <=i (diffFromMean = (ii+jj)*pp-ii). If comp the returns 1 - probability.
    //    double prob = 0;
    //    double p = 0;
    //    double Q = 0;
    //    double a1 = 0;
    //    double a2 = 0;
    //    double b1 = 0;
    //    double b2 = 0;
    //    double c1 = 0;
    //    double c2 = 0;
    //    double c3 = 0;
    //    double c4 = 0;
    //    double n1 = 0;
    //    double q1 = 0;
    //    double dfm = 0;
    //    double i = 0;
    //    double j = 0;
    //    double ni = 0;
    //    double nj = 0;
    //    double Numb = 0;
    //    double ip1 = 0;
    //    double cfValue = 0;
    //    bool swapped = false;
    //    bool exact = false;

    //    if (((ii > -1.0) & (ii < 0.0))) {
    //        ip1 = -ii;
    //        ii = ip1 - 1.0;
    //    } else {
    //        ip1 = ii + 1.0;
    //    }
    //    n1 = (ii + 3.0) + jj;
    //    if (ii < 0.0) {
    //        cfValue = cfVSmall;
    //        swapped = false;
    //    } else if (pp > qq) {
    //        cfValue = cfSmall;
    //        swapped = n1 * qq >= jj + 1.0;
    //    } else {
    //        cfValue = cfSmall;
    //        swapped = n1 * pp <= ii + 2.0;
    //    }
    //    if (!swapped) {
    //        i = ii;
    //        j = jj;
    //        p = pp;
    //        Q = qq;
    //        dfm = diffFromMean;
    //    } else {
    //        j = ip1;
    //        ip1 = jj;
    //        i = jj - 1.0;
    //        p = qq;
    //        Q = pp;
    //        dfm = 1.0 - diffFromMean;
    //    }
    //    if (((i > -1.0) & ((j <= 0.0) | (p == 0.0)))) {
    //        exact = true;
    //        prob = 1.0;
    //    } else if (((i > -1.0) & (i < 0.0) | (i == -1.0) & (ip1 > 0.0))) {
    //        exact = false;
    //        prob = binomialTerm(ip1, j, p, Q, (ip1 + j) * p - ip1, 0.0) * ip1 / ((ip1 + j) * p);
    //        dfm = (i + j) * p - i;
    //    } else {
    //        exact = ((i == 0.0) & (j <= 0.0)) | ((i <= -1.0) | (j < 0.0));
    //        prob = binomialTerm(i, j, p, Q, dfm, 0.0);
    //    }
    //    if ((exact) | (prob == 0.0)) {
    //        if ((swapped == comp)) {
    //            functionReturnValue = prob;
    //        } else {
    //            functionReturnValue = 1.0 - prob;
    //        }
    //        return functionReturnValue;
    //    }

    //    long sumAlways = 0;
    //    long sumFactor = 0;
    //    sumAlways = 0;
    //    sumFactor = 6;
    //    a1 = 0.0;
    //    if ((i > sumAlways)) {
    //        Numb = Convert.ToInt32(sumFactor * Math.Sqrt(p + 0.5) * Math.Exp(Math.Log(n1 * p * Q) / 3));
    //        Numb = Convert.ToInt32(Numb - dfm);
    //        if ((Numb > i)) {
    //            Numb = Convert.ToInt32(i);
    //        }
    //    } else {
    //        Numb = Convert.ToInt32(i);
    //    }
    //    if ((Numb < 0.0)) {
    //        Numb = 0.0;
    //    }

    //    b1 = 1.0;
    //    q1 = Q + 1.0;
    //    a2 = (i - Numb) * Q;
    //    b2 = dfm + Numb + 1.0;
    //    c1 = 0.0;
    //    c2 = a2;
    //    c4 = b2;

    //    while (((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfValue * b1 * a2)))) {
    //        c1 = c1 + 1.0;
    //        c2 = c2 - Q;
    //        c3 = c1 * c2;
    //        c4 = c4 + q1;
    //        a1 = c4 * a2 + c3 * a1;
    //        b1 = c4 * b2 + c3 * b1;
    //        c1 = c1 + 1.0;
    //        c2 = c2 - Q;
    //        c3 = c1 * c2;
    //        c4 = c4 + q1;
    //        a2 = c4 * a1 + c3 * a2;
    //        b2 = c4 * b1 + c3 * b2;
    //        if ((Math.Abs(b2) > scalefactor)) {
    //            a1 = a1 * scalefactor2;
    //            b1 = b1 * scalefactor2;
    //            a2 = a2 * scalefactor2;
    //            b2 = b2 * scalefactor2;
    //        } else if ((Math.Abs(b2) < scalefactor2)) {
    //            a1 = a1 * scalefactor;
    //            b1 = b1 * scalefactor;
    //            a2 = a2 * scalefactor;
    //            b2 = b2 * scalefactor;
    //        }
    //        if (c2 < 0.0 & cfValue > cfVSmall) {
    //            cfValue = cfVSmall;
    //        }
    //    }
    //    a1 = a2 / b2;

    //    ni = (i - Numb + 1.0) * Q;
    //    nj = (j + Numb) * p;
    //    while ((Numb > 0.0)) {
    //        a1 = (1.0 + a1) * (ni / nj);
    //        ni = ni + Q;
    //        nj = nj - p;
    //        Numb = Numb - 1.0;
    //    }

    //    a1 = (1.0 + a1) * prob;
    //    if ((swapped == comp)) {
    //        functionReturnValue = a1;
    //    } else {
    //        functionReturnValue = 1.0 - a1;
    //    }
    //    return functionReturnValue;

    //}

    //private static  double binApprox(double a, double B, double diffFromMean, bool comp)
    //{
    //    double functionReturnValue = 0;
    //    ////Asymptotic expansion to calculate the probability that binomial variate has value <= a (diffFromMean = (a+b)*p-a). If comp then calulate 1-probability.
    //    ////cf. http://members.aol.com/iandjmsmith/BinomialApprox.htm
    //    double N = 0;
    //    double n1 = 0;
    //    double pq1 = 0;
    //    double mfac = 0;
    //    double Res = 0;
    //    double tp = 0;
    //    double lval = 0;
    //    double lvv = 0;
    //    double temp = 0;
    //    double ib05 = 0;
    //    double ib15 = 0;
    //    double ib25 = 0;
    //    double ib35 = 0;
    //    double ib45 = 0;
    //    double ib55 = 0;
    //    double ib65 = 0;
    //    double ib2 = 0;
    //    double ib3 = 0;
    //    double ib4 = 0;
    //    double ib5 = 0;
    //    double ib6 = 0;
    //    double ib7 = 0;
    //    double elfb = 0;
    //    double coef15 = 0;
    //    double coef25 = 0;
    //    double coef35 = 0;
    //    double coef45 = 0;
    //    double coef55 = 0;
    //    double coef65 = 0;
    //    double coef2 = 0;
    //    double coef3 = 0;
    //    double coef4 = 0;
    //    double coef5 = 0;
    //    double coef6 = 0;
    //    double coef7 = 0;
    //    double tdistDensity = 0;
    //    double approxtdistDens = 0;

    //    N = a + B;
    //    n1 = N + 1.0;
    //    lvv = (B + diffFromMean) / n1 - diffFromMean;
    //    lval = (a * log1(lvv / a) + B * log1(-lvv / B)) / N;
    //    tp = -expm1(lval);

    //    pq1 = (a / N) * (B / N);

    //    coef15 = (-17.0 * pq1 + 2.0) / 24.0;
    //    coef25 = ((-503.0 * pq1 + 76.0) * pq1 + 4.0) / 1152.0;
    //    coef35 = (((-315733.0 * pq1 + 53310.0) * pq1 + 8196.0) * pq1 - 1112.0) / 414720.0;
    //    coef45 = (4059192.0 + pq1 * (15386296.0 - 85262251.0 * pq1));
    //    coef45 = (-9136.0 + pq1 * (-697376 + pq1 * coef45)) / 39813120.0;
    //    coef55 = (3904584040.0 + pq1 * (10438368262.0 - 55253161559.0 * pq1));
    //    coef55 = (5244128.0 + pq1 * (-43679536.0 + pq1 * (-703410640.0 + pq1 * coef55))) / 6688604160.0;
    //    coef65 = (-3242780782432.0 + pq1 * (18320560326516.0 + pq1 * (38020748623980.0 - 194479285104469.0 * pq1)));
    //    coef65 = (335796416.0 + pq1 * (61701376704.0 + pq1 * (-433635420336.0 + pq1 * coef65))) / 4815794995200.0;
    //    elfb = (((((coef65 / ((N + 6.5) * pq1) + coef55) / ((N + 5.5) * pq1) + coef45) / ((N + 4.5) * pq1) + coef35) / ((N + 3.5) * pq1) + coef25) / ((N + 2.5) * pq1) + coef15) / ((N + 1.5) * pq1) + 1.0;

    //    coef2 = (-pq1 - 2.0) / 135.0;
    //    coef3 = ((-44.0 * pq1 - 86.0) * pq1 + 4.0) / 2835.0;
    //    coef4 = (((-404.0 * pq1 - 786.0) * pq1 + 48.0) * pq1 + 8.0) / 8505.0;
    //    coef5 = (((((-2421272.0 * pq1 - 4721524.0) * pq1 + 302244.0) * pq1) + 118160.0) * pq1 - 4496.0) / 12629925.0;
    //    coef6 = ((((((-473759128.0 * pq1 - 928767700.0) * pq1 + 57300188.0) * pq1) + 38704888.0) * pq1 - 1870064.0) * pq1 - 167072.0) / 492567075.0;
    //    coef7 = (((((((-8530742848.0 * pq1 - 16836643200.0) * pq1 + 954602040.0) * pq1) + 990295352.0) * pq1 - 44963088.0) * pq1 - 11596512.0) * pq1 + 349376.0) / 1477701225.0;

    //    ib05 = tdistexp(tp, 1.0 - tp, n1 * lval, 2.0 * n1, ref tdistDensity);
    //    mfac = n1 * tp;
    //    ib15 = Math.Sqrt(2.0 * mfac);

    //    if ((mfac > 1E+50)) {
    //        ib2 = (1.0 + mfac) / (N + 2.0);
    //        mfac = mfac * tp / 2.0;
    //        ib3 = (ib2 + mfac) / (N + 3.0);
    //        mfac = mfac * tp / 3.0;
    //        ib4 = (ib3 + mfac) / (N + 4.0);
    //        mfac = mfac * tp / 4.0;
    //        ib5 = (ib4 + mfac) / (N + 5.0);
    //        mfac = mfac * tp / 5.0;
    //        ib6 = (ib5 + mfac) / (N + 6.0);
    //        mfac = mfac * tp / 6.0;
    //        ib7 = (ib6 + mfac) / (N + 7.0);
    //        Res = (ib2 * coef2 + (ib3 * coef3 + (ib4 * coef4 + (ib5 * coef5 + (ib6 * coef6 + ib7 * coef7 / pq1) / pq1) / pq1) / pq1) / pq1) / pq1;

    //        mfac = (N + 1.5) * tp * twoThirds;
    //        ib25 = (1.0 + mfac) / (N + 2.5);
    //        mfac = mfac * tp * twoFifths;
    //        ib35 = (ib25 + mfac) / (N + 3.5);
    //        mfac = mfac * tp * twoSevenths;
    //        ib45 = (ib35 + mfac) / (N + 4.5);
    //        mfac = mfac * tp * twoNinths;
    //        ib55 = (ib45 + mfac) / (N + 5.5);
    //        mfac = mfac * tp * twoElevenths;
    //        ib65 = (ib55 + mfac) / (N + 6.5);
    //        temp = (((((coef65 * ib65 / pq1 + coef55 * ib55) / pq1 + coef45 * ib45) / pq1 + coef35 * ib35) / pq1 + coef25 * ib25) / pq1 + coef15);
    //    } else {
    //        ib2 = 1.0 + mfac;
    //        mfac = mfac * (N + 2.0) * tp / 2.0;
    //        ib3 = ib2 + mfac;
    //        mfac = mfac * (N + 3.0) * tp / 3.0;
    //        ib4 = ib3 + mfac;
    //        mfac = mfac * (N + 4.0) * tp / 4.0;
    //        ib5 = ib4 + mfac;
    //        mfac = mfac * (N + 5.0) * tp / 5.0;
    //        ib6 = ib5 + mfac;
    //        mfac = mfac * (N + 6.0) * tp / 6.0;
    //        ib7 = ib6 + mfac;
    //        Res = (ib2 * coef2 + (ib3 * coef3 + (ib4 * coef4 + (ib5 * coef5 + (ib6 * coef6 + ib7 * coef7 / ((N + 7.0) * pq1)) / ((N + 6.0) * pq1)) / ((N + 5.0) * pq1)) / ((N + 4.0) * pq1)) / ((N + 3.0) * pq1)) / ((N + 2.0) * pq1);

    //        mfac = (N + 1.5) * tp * twoThirds;
    //        ib25 = 1.0 + mfac;
    //        mfac = mfac * (N + 2.5) * tp * twoFifths;
    //        ib35 = ib25 + mfac;
    //        mfac = mfac * (N + 3.5) * tp * twoSevenths;
    //        ib45 = ib35 + mfac;
    //        mfac = mfac * (N + 4.5) * tp * twoNinths;
    //        ib55 = ib45 + mfac;
    //        mfac = mfac * (N + 5.5) * tp * twoElevenths;
    //        ib65 = ib55 + mfac;
    //        temp = (((((coef65 * ib65 / ((N + 6.5) * pq1) + coef55 * ib55) / ((N + 5.5) * pq1) + coef45 * ib45) / ((N + 4.5) * pq1) + coef35 * ib35) / ((N + 3.5) * pq1) + coef25 * ib25) / ((N + 2.5) * pq1) + coef15);
    //    }

    //    approxtdistDens = tdistDensity / Math.Sqrt(1.0 - tp);
    //    temp = ib15 * temp / ((N + 1.5) * pq1);
    //    Res = (oneThird + Res) * 2.0 * (a - B) / (N * Math.Sqrt(n1 * pq1));
    //    if ((comp)) {
    //        if ((lvv > 0.0)) {
    //            functionReturnValue = ib05 - (Res - temp) * approxtdistDens / elfb;
    //        } else {
    //            functionReturnValue = (1.0 - ib05) - (Res + temp) * approxtdistDens / elfb;
    //        }
    //    } else if ((lvv > 0.0)) {
    //        functionReturnValue = (1.0 - ib05) + (Res - temp) * approxtdistDens / elfb;
    //    } else {
    //        functionReturnValue = ib05 + (Res + temp) * approxtdistDens / elfb;
    //    }
    //    return functionReturnValue;
    //}

    //private static  double binomial(double ii, double jj, double pp, double qq, double diffFromMean)
    //{
    //    double functionReturnValue = 0;
    //    ////Probability that binomial variate with sample size ii+jj and event prob pp (=1-qq) has value <=i (diffFromMean = (ii+jj)*pp-ii).
    //    double mij = 0;
    //    mij = Min(ii, jj);
    //    if (((mij > 50.0) & (Math.Abs(diffFromMean) < (0.1 * mij)))) {
    //        functionReturnValue = binApprox(jj - 1.0, ii, diffFromMean, false);
    //    } else {
    //        functionReturnValue = binomialcf(ii, jj, pp, qq, diffFromMean, false);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double compbinomial(double ii, double jj, double pp, double qq, double diffFromMean)
    //{
    //    double functionReturnValue = 0;
    //    ////Probability that binomial variate with sample size ii+jj and event prob pp (=1-qq) has value >i (diffFromMean = (ii+jj)*pp-ii).
    //    double mij = 0;
    //    mij = Min(ii, jj);
    //    if (((mij > 50.0) & (Math.Abs(diffFromMean) < (0.1 * mij)))) {
    //        functionReturnValue = binApprox(jj - 1.0, ii, diffFromMean, true);
    //    } else {
    //        functionReturnValue = binomialcf(ii, jj, pp, qq, diffFromMean, true);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double invbinom(double k, double M, double prob, ref double oneMinusP)
    //{
    //    ////Inverse of binomial. Delivers event probability p (q held in oneMinusP in case required) so that binomial(k,m,p,oneMinusp,dfm) = prob.
    //    ////Note that dfm is calculated accurately but never made available outside of this routine.
    //    ////Require prob to be in the range 0..1, m should be positive and k should be >= 0
    //    double temp1 = 0;
    //    double temp2 = 0;
    //    if ((prob > 0.5)) {
    //        temp2 = invcompbinom(k, M, 1.0 - prob, ref oneMinusP);
    //    } else {
    //        temp1 = invcompbinom(M - 1.0, k + 1.0, prob, ref oneMinusP);
    //        temp2 = oneMinusP;
    //        oneMinusP = temp1;
    //    }
    //    return temp2;
    //}

    //private static  double invcompbinom(double k, double M, double prob, ref double oneMinusP)
    //{
    //    double functionReturnValue = 0;
    //    ////Inverse of compbinomial. Delivers event probability p (q held in oneMinusP in case required) so that compbinomial(k,m,p,oneMinusp,dfm) = prob.
    //    ////Note that dfm is calculated accurately but never made available outside of this routine.
    //    ////Require prob to be in the range 0..1, m should be positive and k should be >= -0.5
    //    double xp = 0;
    //    double xp2 = 0;
    //    double dfm = 0;
    //    double N = 0;
    //    double p = 0;
    //    double Q = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double temp2 = 0;
    //    double Result = 0;
    //    double lpr = 0;
    //    double small = 0;
    //    double smalllpr = 0;
    //    double nminpq = 0;
    //    Result = -1.0;
    //    N = k + M;
    //    if ((prob > 0.5)) {
    //        Result = invbinom(k, M, 1.0 - prob, ref oneMinusP);
    //    } else if ((k == 0.0)) {
    //        Result = log0(-prob) / N;
    //        if ((Math.Abs(Result) < 1.0)) {
    //            Result = -expm1(Result);
    //            oneMinusP = 1.0 - Result;
    //        } else {
    //            oneMinusP = Math.Exp(Result);
    //            Result = 1.0 - oneMinusP;
    //        }
    //    } else if ((M == 1.0)) {
    //        Result = Math.Log(prob) / N;
    //        if ((Math.Abs(Result) < 1.0)) {
    //            oneMinusP = -expm1(Result);
    //            Result = 1.0 - oneMinusP;
    //        } else {
    //            Result = Math.Exp(Result);
    //            oneMinusP = 1.0 - Result;
    //        }
    //    } else {
    //        pr = -1.0;
    //        xp = invcnormal(prob);
    //        xp2 = xp * xp;
    //        temp = 2.0 * xp * Math.Sqrt(k * (M / N) + xp2 / 4.0);
    //        xp2 = xp2 / N;
    //        dfm = (xp2 * (M - k) + temp) / (2.0 * (1.0 + xp2));
    //        if ((k + dfm < 0.0)) {
    //            dfm = -k;
    //        }
    //        Q = (M - dfm) / N;
    //        p = (k + dfm) / N;
    //        dif = -dfm / N;
    //        if ((dif == 0.0)) {
    //            dif = 1.0;
    //        } else if (Min(k, M) > 1E+50) {
    //            oneMinusP = Q;
    //            functionReturnValue = p;
    //            return functionReturnValue;
    //        }
    //        lpr = -Math.Log(prob);
    //        small = 4E-14;
    //        smalllpr = small * lpr * prob;
    //        nminpq = N * Min(p, Q);
    //        while (((Math.Abs(pr - prob) > smalllpr) & (N * Math.Abs(dif) > Min(Math.Abs(dfm), nminpq) * small))) {
    //            pr = compbinomial(k, M, p, Q, dfm);
    //            ///*Should not be happenning often */
    //            if ((pr < nearly_zero)) {
    //                dif = dif / 2.0;
    //                dfm = dfm + N * dif;
    //                p = p + dif;
    //                Q = Q - dif;
    //                pr = -1.0;
    //            } else {
    //                temp2 = binomialTerm(k, M, p, Q, dfm, 0.0) * M / Q;
    //                ///*Should not be happenning often */
    //                if ((temp2 < nearly_zero)) {
    //                    dif = dif / 2.0;
    //                    dfm = dfm + N * dif;
    //                    p = p + dif;
    //                    Q = Q - dif;
    //                    pr = -1.0;
    //                } else {
    //                    dif = 2.0 * pr * logdif(pr, prob) / (1.0 + Math.Sqrt(Math.Log(prob) / Math.Log(pr))) / temp2;
    //                    ///*not v. good */
    //                    if ((Q + dif <= 0.0)) {
    //                        dif = -0.9999 * Q;
    //                        dfm = dfm - N * dif;
    //                        p = p - dif;
    //                        Q = Q + dif;
    //                        pr = -1.0;
    //                    ///*v. good */
    //                    } else if ((p - dif <= 0.0)) {
    //                        temp = Math.Exp(Math.Log(prob / pr) / (k + 1.0));
    //                        dif = p;
    //                        p = temp * p;
    //                        dif = p - dif;
    //                        dfm = N * p - k;
    //                        Q = 1.0 - p;
    //                        pr = -1.0;
    //                    } else {
    //                        dfm = dfm - N * dif;
    //                        p = p - dif;
    //                        Q = Q + dif;
    //                    }
    //                }
    //            }
    //        }
    //        Result = p;
    //        oneMinusP = Q;
    //    }
    //    functionReturnValue = Result;
    //    return functionReturnValue;
    //}

    //private static  double abMinuscd(double a, double B, double C, double D)
    //{
    //    double functionReturnValue = 0;
    //    double a1 = 0;
    //    double b1 = 0;
    //    double c1 = 0;
    //    double d1 = 0;
    //    double a2 = 0;
    //    double b2 = 0;
    //    double c2 = 0;
    //    double d2 = 0;
    //    double r1 = 0;
    //    double r2 = 0;
    //    double r3 = 0;
    //    a2 = Convert.ToInt32(a / twoTo27) * twoTo27;
    //    a1 = a - a2;
    //    b2 = Convert.ToInt32(B / twoTo27) * twoTo27;
    //    b1 = B - b2;
    //    c2 = Convert.ToInt32(C / twoTo27) * twoTo27;
    //    c1 = C - c2;
    //    d2 = Convert.ToInt32(D / twoTo27) * twoTo27;
    //    d1 = D - d2;
    //    r1 = a1 * b1 - c1 * d1;
    //    r2 = (a2 * b1 - c1 * d2) + (a1 * b2 - c2 * d1);
    //    r3 = a2 * b2 - c2 * d2;
    //    if ((r3 < 0.0) == (r2 < 0.0)) {
    //        functionReturnValue = r3 + (r2 + r1);
    //    } else {
    //        functionReturnValue = (r3 + r2) + r1;
    //    }
    //    return functionReturnValue;
    //}

    //private static  double aTimes2Powerb(double a, int B)
    //{
    //    if (B > 709) {
    //        a = (a * scalefactor) * scalefactor;
    //        B = B - 512;
    //    } else if (B < -709) {
    //        a = (a * scalefactor2) * scalefactor2;
    //        B = B + 512;
    //    }
    //    return a * Math.Pow((2.0), B);
    //}

    //private static  double GeneralabMinuscd(double a, double B, double C, double D)
    //{
    //    double functionReturnValue = 0;
    //    double S = 0;
    //    double ca = 0;
    //    double cb = 0;
    //    double CC = 0;
    //    double cd = 0;
    //    int l2 = 0;
    //    int pa = 0;
    //    int pb = 0;
    //    int pc = 0;
    //    int pd = 0;
    //    S = a * B - C * D;
    //    if (a <= 0.0 | B <= 0.0 | C <= 0.0 | D <= 0.0) {
    //        functionReturnValue = S;
    //        return functionReturnValue;
    //    } else if (S < 0.0) {
    //        functionReturnValue = -GeneralabMinuscd(C, D, a, B);
    //        return functionReturnValue;
    //    }
    //    l2 = Convert.ToInt32(Math.Log(a) / Math.Log(2));
    //    pa = 51 - l2;
    //    ca = aTimes2Powerb(a, pa);
    //    l2 = Convert.ToInt32(Math.Log(B) / Math.Log(2));
    //    pb = 51 - l2;
    //    cb = aTimes2Powerb(B, pb);
    //    l2 = Convert.ToInt32(Math.Log(C) / Math.Log(2));
    //    pc = 51 - l2;
    //    CC = aTimes2Powerb(C, pc);
    //    pd = pa + pb - pc;
    //    cd = aTimes2Powerb(D, pd);
    //    functionReturnValue = aTimes2Powerb(abMinuscd(ca, cb, CC, cd), -(pa + pb));
    //    return functionReturnValue;
    //}

    //private static  double hypergeometricTerm(double Ai, double aji, double aki, double amkji)
    //{
    //    double functionReturnValue = 0;
    //    //// Probability that hypergeometric variate from a population with total type Is of aki+ai, total type IIs of amkji+aji, has ai type Is and aji type IIs selected.
    //    double aj = 0;
    //    double am = 0;
    //    double ak = 0;
    //    double amj = 0;
    //    double amk = 0;
    //    double cjkmi = 0;
    //    double ai1 = 0;
    //    double aj1 = 0;
    //    double ak1 = 0;
    //    double am1 = 0;
    //    double aki1 = 0;
    //    double aji1 = 0;
    //    double amk1 = 0;
    //    double amj1 = 0;
    //    double amkji1 = 0;
    //    double c1 = 0;
    //    double c3 = 0;
    //    double c4 = 0;
    //    double c5 = 0;
    //    double loghypergeometricTerm = 0;

    //    ak = aki + Ai;
    //    amk = amkji + aji;
    //    aj = aji + Ai;
    //    am = amk + ak;
    //    amj = amkji + aki;
    //    if ((am > max_discrete)) {
    //        functionReturnValue = ErrorValue;
    //        return functionReturnValue;
    //    }
    //    if (((Ai == 0.0) & ((aji <= 0.0) | (aki <= 0.0) | (amj < 0.0) | (amk < 0.0)))) {
    //        functionReturnValue = 1.0;
    //    } else if (((Ai > 0.0) & (Min(aki, aji) == 0.0) & (max(amj, amk) == 0.0))) {
    //        functionReturnValue = 1.0;
    //    } else if (((Ai >= 0.0) & (amkji > -1.0) & (aki > -1.0) & (aji >= 0.0))) {
    //        c1 = logfbit(amkji) + logfbit(aki) + logfbit(aji) + logfbit(am) + logfbit(Ai);
    //        c1 = logfbit(amk) + logfbit(ak) + logfbit(aj) + logfbit(amj) - c1;
    //        ai1 = Ai + 1.0;
    //        aj1 = aj + 1.0;
    //        ak1 = ak + 1.0;
    //        am1 = am + 1.0;
    //        aki1 = aki + 1.0;
    //        aji1 = aji + 1.0;
    //        amk1 = amk + 1.0;
    //        amj1 = amj + 1.0;
    //        amkji1 = amkji + 1.0;
    //        cjkmi = GeneralabMinuscd(aji, aki, Ai, amkji);
    //        c5 = (cjkmi - Ai) / (amkji1 * am1);
    //        if ((c5 < minLog1Value)) {
    //            c3 = amkji * (Math.Log((amj1 * amk1) / (amkji1 * am1)) - c5) - c5;
    //        } else {
    //            c3 = amkji * log1(c5) - c5;
    //        }

    //        c5 = (-cjkmi - aji) / (aki1 * am1);
    //        if ((c5 < minLog1Value)) {
    //            c4 = aki * (Math.Log((ak1 * amj1) / (aki1 * am1)) - c5) - c5;
    //        } else {
    //            c4 = aki * log1(c5) - c5;
    //        }

    //        c3 = c3 + c4;
    //        c5 = (-cjkmi - aki) / (aji1 * am1);
    //        if ((c5 < minLog1Value)) {
    //            c4 = aji * (Math.Log((aj1 * amk1) / (aji1 * am1)) - c5) - c5;
    //        } else {
    //            c4 = aji * log1(c5) - c5;
    //        }

    //        c3 = c3 + c4;
    //        c5 = (cjkmi - amkji) / (ai1 * am1);
    //        if ((c5 < minLog1Value)) {
    //            c4 = Ai * (Math.Log((aj1 * ak1) / (ai1 * am1)) - c5) - c5;
    //        } else {
    //            c4 = Ai * log1(c5) - c5;
    //        }

    //        c3 = c3 + c4;
    //        loghypergeometricTerm = (c1 + 1.0 / am1) + c3;

    //        functionReturnValue = Math.Exp(loghypergeometricTerm) * Math.Sqrt((amk1 * ak1) * (aj1 * amj1) / ((amkji1 * aki1 * aji1) * (am1 * ai1))) * OneOverSqrTwoPi;
    //    } else {
    //        functionReturnValue = 0.0;
    //    }
    //    return functionReturnValue;

    //}

    //private static  double hypergeometric(double Ai, double aji, double aki, double amkji, bool comp, ref double ha1, ref double hprob, ref bool hswap)
    //{
    //    double functionReturnValue = 0;
    //    //// Probability that hypergeometric variate from a population with total type Is of aki+ai, total type IIs of amkji+aji, has up to ai type Is selected in a sample of size aji+ai.
    //    double prob = 0;
    //    double a1 = 0;
    //    double a2 = 0;
    //    double b1 = 0;
    //    double b2 = 0;
    //    double an = 0;
    //    double bn = 0;
    //    double bnAdd = 0;
    //    double c1 = 0;
    //    double c2 = 0;
    //    double c3 = 0;
    //    double c4 = 0;
    //    double i = 0;
    //    double ji = 0;
    //    double ki = 0;
    //    double mkji = 0;
    //    double njj = 0;
    //    double Numb = 0;
    //    double maxSums = 0;
    //    bool swapped = false;
    //    double ip1 = 0;
    //    bool must_do_cf = false;
    //    bool allIntegral = false;
    //    bool exact = false;
    //    if ((amkji > -1.0) & (amkji < 0.0)) {
    //        ip1 = -amkji;
    //        mkji = ip1 - 1.0;
    //        allIntegral = false;
    //    } else {
    //        ip1 = amkji + 1.0;
    //        mkji = amkji;
    //        allIntegral = Ai == Convert.ToInt32(Ai) & aji == Convert.ToInt32(aji) & aki == Convert.ToInt32(aki) & mkji == Convert.ToInt32(mkji);
    //    }

    //    if (allIntegral) {
    //        swapped = (Ai + 0.5) * (mkji + 0.5) >= (aki - 0.5) * (aji - 0.5);
    //    } else if (Ai < 100.0 & Ai == Convert.ToInt32(Ai) | mkji < 0.0) {
    //        swapped = (Ai + 0.5) * (mkji + 0.5) >= aki * aji + 1000.0;
    //    } else if (Ai < 1.0) {
    //        swapped = (Ai + 0.5) * (mkji + 0.5) >= aki * aji;
    //    } else if (aji < 1.0 | aki < 1.0 | (Ai < 1.0 & Ai > 0.0)) {
    //        swapped = false;
    //    } else {
    //        swapped = (Ai + 0.5) * (mkji + 0.5) >= (aki - 0.5) * (aji - 0.5);
    //    }
    //    if (!swapped) {
    //        i = Ai;
    //        ji = aji;
    //        ki = aki;
    //    } else {
    //        i = aji - 1.0;
    //        ji = Ai + 1.0;
    //        ki = ip1;
    //        ip1 = aki;
    //        mkji = aki - 1.0;
    //    }
    //    c2 = ji + i;
    //    c4 = mkji + ki + c2;
    //    if ((c4 > max_discrete)) {
    //        functionReturnValue = ErrorValue;
    //        return functionReturnValue;
    //    }
    //    if (((i >= 0.0) & (ji <= 0.0) | (ki <= 0.0) | (ip1 + ki <= 0.0) | (ip1 + ji <= 0.0))) {
    //        exact = true;
    //        if ((i >= 0.0)) {
    //            prob = 1.0;
    //        } else {
    //            prob = 0.0;
    //        }
    //    } else if ((ip1 > 0.0) & (ip1 < 1.0)) {
    //        exact = false;
    //        prob = hypergeometricTerm(i, ji, ki, ip1) * (ip1 * (c4 + 1.0)) / ((ki + ip1) * (ji + ip1));
    //    } else {
    //        exact = ((i == 0.0) & ((ji <= 0.0) | (ki <= 0.0) | (mkji + ki < 0.0) | (mkji + ji < 0.0))) | ((i > 0.0) & (Min(ki, ji) == 0.0) & (max(mkji + ki, mkji + ji) == 0.0));
    //        prob = hypergeometricTerm(i, ji, ki, mkji);
    //    }
    //    hprob = prob;
    //    hswap = swapped;
    //    ha1 = 0.0;

    //    if ((exact) | (prob == 0.0)) {
    //        if ((swapped == comp)) {
    //            functionReturnValue = prob;
    //        } else {
    //            functionReturnValue = 1.0 - prob;
    //        }
    //        return functionReturnValue;
    //    }

    //    a1 = 0.0;
    //    long sumAlways = 0;
    //    long sumFactor = 0;
    //    sumAlways = 0;
    //    sumFactor = 10;

    //    if (i < mkji) {
    //        must_do_cf = i != Convert.ToInt32(i);
    //        maxSums = Convert.ToInt32(i);
    //    } else {
    //        must_do_cf = mkji != Convert.ToInt32(mkji);
    //        maxSums = Convert.ToInt32(max(mkji, 0.0));
    //    }
    //    if (must_do_cf) {
    //        sumAlways = 0;
    //        sumFactor = 5;
    //    } else {
    //        sumAlways = 20;
    //        sumFactor = 10;
    //    }
    //    if ((maxSums > sumAlways | must_do_cf)) {
    //        Numb = Convert.ToInt32(sumFactor / c4 * Math.Exp(Math.Log((ki + i) * (ji + i) * (ip1 + ji) * (ip1 + ki)) / 3.0));
    //        Numb = Convert.ToInt32(i - (ki + i) * (ji + i) / c4 + Numb);
    //        if ((Numb < 0.0)) {
    //            Numb = 0.0;
    //        } else if (Numb > maxSums) {
    //            Numb = maxSums;
    //        }
    //    } else {
    //        Numb = maxSums;
    //    }

    //    if ((2.0 * Numb <= maxSums | must_do_cf)) {
    //        b1 = 1.0;
    //        c1 = 0.0;
    //        c2 = i - Numb;
    //        c3 = mkji - Numb;
    //        a2 = c2 * c3;
    //        c3 = c3 - 1.0;
    //        b2 = GeneralabMinuscd(ki + Numb + 1.0, ji + Numb + 1.0, c2 - 1.0, c3);
    //        bn = b2;
    //        bnAdd = c3 + c4 + c2 - 2.0;
    //        while ((b2 > 0.0 & (Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfVSmall * b1 * a2)))) {
    //            c1 = c1 + 1.0;
    //            c2 = c2 - 1.0;
    //            an = (c1 * c2) * (c3 * c4);
    //            c3 = c3 - 1.0;
    //            c4 = c4 - 1.0;
    //            bn = bn + bnAdd;
    //            bnAdd = bnAdd - 4.0;
    //            a1 = bn * a2 + an * a1;
    //            b1 = bn * b2 + an * b1;
    //            if ((b1 > scalefactor)) {
    //                a1 = a1 * scalefactor2;
    //                b1 = b1 * scalefactor2;
    //                a2 = a2 * scalefactor2;
    //                b2 = b2 * scalefactor2;
    //            }
    //            c1 = c1 + 1.0;
    //            c2 = c2 - 1.0;
    //            an = (c1 * c2) * (c3 * c4);
    //            c3 = c3 - 1.0;
    //            c4 = c4 - 1.0;
    //            bn = bn + bnAdd;
    //            bnAdd = bnAdd - 4.0;
    //            a2 = bn * a1 + an * a2;
    //            b2 = bn * b1 + an * b2;
    //            if ((b2 > scalefactor)) {
    //                a1 = a1 * scalefactor2;
    //                b1 = b1 * scalefactor2;
    //                a2 = a2 * scalefactor2;
    //                b2 = b2 * scalefactor2;
    //            }
    //        }
    //        if (b1 < 0.0 | b2 < 0.0) {
    //            functionReturnValue = ErrorValue;
    //            return functionReturnValue;
    //        } else {
    //            a1 = a2 / b2;
    //        }
    //    } else {
    //        Numb = maxSums;
    //    }

    //    c1 = i - Numb + 1.0;
    //    c2 = mkji - Numb + 1.0;
    //    c3 = ki + Numb;
    //    c4 = ji + Numb;
    //    for (njj = 1; njj <= Numb; njj++) {
    //        a1 = (1.0 + a1) * ((c1 * c2) / (c3 * c4));
    //        c1 = c1 + 1.0;
    //        c2 = c2 + 1.0;
    //        c3 = c3 - 1.0;
    //        c4 = c4 - 1.0;
    //    }

    //    ha1 = a1;
    //    a1 = (1.0 + a1) * prob;
    //    if ((swapped == comp)) {
    //        functionReturnValue = a1;
    //    } else {
    //        if (a1 > 0.99) {
    //            functionReturnValue = ErrorValue;
    //        } else {
    //            functionReturnValue = 1.0 - a1;
    //        }
    //    }
    //    return functionReturnValue;
    //}

    private static double compgfunc(double x, double a)
    {
        ////Calculates a*x(1/(a+1) - x/2*(1/(a+2) - x/3*(1/(a+3) - ...)))
        ////Mainly for calculating the complement of gamma(x,a) for small a and x <= 1.
        ////a should be close to 0, x >= 0 & x <=1
        double term = 0;
        double D = 0;
        double sum = 0;
        term = x;
        D = 2.0;
        sum = term / (a + 1.0);
        while ((Math.Abs(term) > Math.Abs(sum * sumAcc)))
        {
            term = -term * x / D;
            sum = sum + term / (a + D);
            D = D + 1.0;
        }
        return a * sum;
    }

    private static double lngammaexpansion(double a)
    {
        ////Calculates log(gamma(a+1)) accurately for for small a (0 < a & a < 0.5).
        ////Uses Abramowitz & Stegun's series 6.1.33
        ////Mainly for calculating the complement of gamma(x,a) for small a and x <= 1.
        ////
        //double[] coeffs = null;
        // "Variant" rather than  "coefs(40) as Double"  to permit use of Array assignment
        //// for i < 40 coeffs[i] holds (zeta(i+2)-1)/(i+2), coeffs[40] holds (zeta(i+2)-1)
        double[] coeffs = {
            0.322467033424113,
            0.0673523010531981,
            0.0205808084277845,
            0.00738555102867399,
            0.00289051033074152,
            0.00119275391170326,
            0.000509669524743042,
            0.000223154758453579,
            9.94575127818085E-05,
            4.49262367381331E-05,
            2.05072127756707E-05,
            9.4394882752684E-06,
            4.37486678990749E-06,
            2.03921575380137E-06,
            9.55141213040742E-07,
            4.49246919876457E-07,
            2.12071848055547E-07,
            1.00432248239681E-07,
            4.76981016936398E-08,
            2.27110946089432E-08,
            1.0838659214897E-08,
            5.18347504197005E-09,
            2.48367454380248E-09,
            1.19214014058609E-09,
            5.73136724167886E-10,
            2.75952288512423E-10,
            1.33047643742445E-10,
            6.4229645638381E-11,
            3.10442477473223E-11,
            1.50213840807541E-11,
            7.27597448023908E-12,
            3.52774247657592E-12,
            1.71199179055962E-12,
            8.31538584142029E-13,
            4.04220052528944E-13,
            1.96647563109662E-13,
            9.57363038783856E-14,
            4.66407602642837E-14,
            2.27373696006597E-14,
            1.10913994708345E-14,
            2.27373684582465E-13
        };
        double lgam = 0;
        int i = 0;
        lgam = coeffs[40] * logcf(-a / 2.0, 42.0, 1.0);
        for (i = 39; i >= 0; i += -1)
        {
            lgam = (coeffs[i] - a * lgam);
        }
        return (a * lgam - eulers_const) * a - log1(a);
    }

    //private static  double incgamma(double x, double a, bool comp)
    //{
    //    double functionReturnValue = 0;
    //    ////Calculates gamma-cdf for small a (complementary gamma-cdf if comp).
    //    double r = 0;
    //    r = a * Math.Log(x) - lngammaexpansion(a);
    //    if ((comp)) {
    //        r = -expm1(r);
    //        functionReturnValue = r + compgfunc(x, a) * (1.0 - r);
    //    } else {
    //        functionReturnValue = Math.Exp(r) * (1.0 - compgfunc(x, a));
    //    }
    //    return functionReturnValue;
    //}

    private static double invincgamma(double a, double prob, bool comp)
    {
        double functionReturnValue = 0;
        ////Calculates inverse of gamma for small a (inverse of complementary gamma if comp).
        double ga = 0;
        double x = 0;
        double deriv = 0;
        double Z = 0;
        double W = 0;
        double dif = 0;
        double pr = 0;
        double lpr = 0;
        double small = 0;
        double smalllpr = 0;
        if ((prob > 0.5))
        {
            functionReturnValue = invincgamma(a, 1.0 - prob, !comp);
            return functionReturnValue;
        }
        lpr = -Math.Log(prob);
        small = 1E-14;
        smalllpr = small * lpr * prob;
        if ((comp))
        {
            ga = -expm1(lngammaexpansion(a));
            x = -Math.Log(prob * (1.0 - ga) / a);
            if ((x < 0.5))
            {
                pr = Math.Exp(log0(-(ga + prob * (1.0 - ga))) / a);
                if ((x < pr))
                {
                    x = pr;
                }
            }
            dif = x;
            pr = -1.0;
            while (((Math.Abs(pr - prob) > smalllpr) & (Math.Abs(dif) > small * max(cSmall, x))))
            {
                deriv = poissonTerm(a, x, x - a, 0.0) * a;
                //value of derivative is actually deriv/x but it can overflow when x is denormal...
                if ((x > 1.0))
                {
                    pr = poisson1(-a, x, 0.0);
                }
                else
                {
                    Z = compgfunc(x, a);
                    W = -expm1(a * Math.Log(x));
                    W = Z + W * (1.0 - Z);
                    pr = (W - ga) / (1.0 - ga);
                }
                dif = x * (pr / deriv) * logdif(pr, prob);
                //...so multiply by x in slightly different order
                x = x + dif;
                if ((x < 0.0))
                {
                    functionReturnValue = 0.0;
                    return functionReturnValue;
                }
            }
        }
        else
        {
            ga = Math.Exp(lngammaexpansion(a));
            x = Math.Log(prob * ga);
            if ((x < -711.0 * a))
            {
                functionReturnValue = 0.0;
                return functionReturnValue;
            }
            x = Math.Exp(x / a);
            Z = 1.0 - compgfunc(x, a);
            deriv = poissonTerm(a, x, x - a, 0.0) * a / x;
            pr = prob * Z;
            dif = (pr / deriv) * logdif(pr, prob);
            x = x - dif;
            while (((Math.Abs(pr - prob) > smalllpr) & (Math.Abs(dif) > small * max(cSmall, x))))
            {
                deriv = poissonTerm(a, x, x - a, 0.0) * a / x;
                if ((x > 1.0))
                {
                    pr = 1.0 - poisson1(-a, x, 0.0);
                }
                else
                {
                    pr = (1.0 - compgfunc(x, a)) * Math.Exp(a * Math.Log(x)) / ga;
                }
                dif = (pr / deriv) * logdif(pr, prob);
                x = x - dif;
            }
        }
        functionReturnValue = x;
        return functionReturnValue;
    }

    //private static  double GAMMA(double N, double a)
    //{
    //    double functionReturnValue = 0;
    //    //Assumes n > 0 & a >= 0.  Only called by (comp)gamma_nc with a = 0.
    //    if ((a == 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if (((a < 1.0) & (N < 1.0))) {
    //        functionReturnValue = incgamma(N, a, false);
    //    } else if ((a >= 1.0)) {
    //        functionReturnValue = comppoisson(a - 1.0, N, N - a + 1.0);
    //    } else {
    //        functionReturnValue = 1.0 - poisson1(-a, N, 0.0);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double compgamma(double N, double a)
    //{
    //    double functionReturnValue = 0;
    //    //Assumes n > 0 & a >= 0. Only called by (comp)gamma_nc with a = 0.
    //    if ((a == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if (((a < 1.0) & (N < 1.0))) {
    //        functionReturnValue = incgamma(N, a, true);
    //    } else if ((a >= 1.0)) {
    //        functionReturnValue = cpoisson(a - 1.0, N, N - a + 1.0);
    //    } else {
    //        functionReturnValue = poisson1(-a, N, 0.0);
    //    }
    //    return functionReturnValue;
    //}

    private static double invgamma(double a, double prob)
    {
        double functionReturnValue = 0;
        ////Inverse of gamma(x,a)
        if ((a >= 1.0))
        {
            functionReturnValue = invcomppoisson(a - 1.0, prob);
        }
        else
        {
            functionReturnValue = invincgamma(a, prob, false);
        }
        return functionReturnValue;
    }

    //private static  double invcompgamma(double a, double prob)
    //{
    //    double functionReturnValue = 0;
    //    ////Inverse of compgamma(x,a)
    //    if ((a >= 1.0)) {
    //        functionReturnValue = invpoisson(a - 1.0, prob);
    //    } else {
    //        functionReturnValue = invincgamma(a, prob, true);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double logfbit1dif(double x)
    //{
    //    //// Calculation of logfbit1(x)-logfbit1(1+x).
    //    return (logfbitdif(x) - 0.25 / ((x + 1.0) * (x + 2.0))) / (x + 1.5);
    //}

    //private static  double logfbit1(double x)
    //{
    //    double functionReturnValue = 0;
    //    //// Derivative of error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
    //    double x1 = 0;
    //    double x2 = 0;
    //    if ((x >= 10000000000.0)) {
    //        functionReturnValue = -lfbc1 * (Math.Pow((x + 1.0), -2));
    //    } else if ((x >= 6.0)) {
    //        double x3 = 0;
    //        x1 = x + 1.0;
    //        x2 = 1.0 / (x1 * x1);
    //        x3 = (11.0 * lfbc6 - x2 * (13.0 * lfbc7 - x2 * (15.0 * lfbc8 - x2 * 17.0 * lfbc9)));
    //        x3 = (5.0 * lfbc3 - x2 * (7.0 * lfbc4 - x2 * (9.0 * lfbc5 - x2 * x3)));
    //        x3 = x2 * (3.0 * lfbc2 - x2 * x3);
    //        functionReturnValue = -lfbc1 * (1.0 - x3) * x2;
    //    } else if ((x > -1.0)) {
    //        x1 = x;
    //        x2 = 0.0;
    //        while ((x1 < 6.0)) {
    //            x2 = x2 + logfbit1dif(x1);
    //            x1 = x1 + 1.0;
    //        }
    //        functionReturnValue = x2 + logfbit1(x1);
    //    } else {
    //        functionReturnValue = -1E+308;
    //    }
    //    return functionReturnValue;
    //}

    //private static  double logfbit3dif(double x)
    //{
    //    //// Calculation of logfbit3(x)-logfbit3(1+x).
    //    return -(2.0 * x + 3.0) * Math.Pow(((x + 1.0) * (x + 2.0)), -3);
    //}

    //private static  double logfbit3(double x)
    //{
    //    double functionReturnValue = 0;
    //    //// Third derivative of error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
    //    double x1 = 0;
    //    double x2 = 0;
    //    if ((x >= 10000000000.0)) {
    //        functionReturnValue = -0.5 * Math.Pow((x + 1.0), -4);
    //    } else if ((x >= 6.0)) {
    //        double x3 = 0;
    //        x1 = x + 1.0;
    //        x2 = 1.0 / (x1 * x1);
    //        x3 = x2 * (4080.0 * lfbc8 - x2 * 5814.0 * lfbc9);
    //        x3 = x2 * (1716.0 * lfbc6 - x2 * (2730.0 * lfbc7 - x3));
    //        x3 = x2 * (504.0 * lfbc4 - x2 * (990.0 * lfbc5 - x3));
    //        x3 = x2 * (60.0 * lfbc2 - x2 * (210.0 * lfbc3 - x3));
    //        functionReturnValue = -lfbc1 * (6.0 - x3) * x2 * x2;
    //    } else if ((x > -1.0)) {
    //        x1 = x;
    //        x2 = 0.0;
    //        while ((x1 < 6.0)) {
    //            x2 = x2 + logfbit3dif(x1);
    //            x1 = x1 + 1.0;
    //        }
    //        functionReturnValue = x2 + logfbit3(x1);
    //    } else {
    //        functionReturnValue = -1E+308;
    //    }
    //    return functionReturnValue;
    //}

    //private static  double logfbit5dif(double x)
    //{
    //    //// Calculation of logfbit5(x)-logfbit5(1+x).
    //    return -6.0 * (2.0 * x + 3.0) * ((5.0 * x + 15.0) * x + 12.0) * Math.Pow(((x + 1.0) * (x + 2.0)), -5);
    //}

    //private static  double logfbit5(double x)
    //{
    //    double functionReturnValue = 0;
    //    //// Fifth derivative of error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
    //    double x1 = 0;
    //    double x2 = 0;
    //    if ((x >= 10000000000.0)) {
    //        functionReturnValue = -10.0 * Math.Pow((x + 1.0), -6);
    //    } else if ((x >= 6.0)) {
    //        double x3 = 0;
    //        x1 = x + 1.0;
    //        x2 = 1.0 / (x1 * x1);
    //        x3 = x2 * (1395360.0 * lfbc8 - x2 * 2441880.0 * lfbc9);
    //        x3 = x2 * (360360.0 * lfbc6 - x2 * (742560.0 * lfbc7 - x3));
    //        x3 = x2 * (55440.0 * lfbc4 - x2 * (154440.0 * lfbc5 - x3));
    //        x3 = x2 * (2520.0 * lfbc2 - x2 * (15120.0 * lfbc3 - x3));
    //        functionReturnValue = -lfbc1 * (120.0 - x3) * x2 * x2 * x2;
    //    } else if ((x > -1.0)) {
    //        x1 = x;
    //        x2 = 0.0;
    //        while ((x1 < 6.0)) {
    //            x2 = x2 + logfbit5dif(x1);
    //            x1 = x1 + 1.0;
    //        }
    //        functionReturnValue = x2 + logfbit5(x1);
    //    } else {
    //        functionReturnValue = -1E+308;
    //    }
    //    return functionReturnValue;
    //}

    //private static  double logfbit7dif(double x)
    //{
    //    //// Calculation of logfbit7(x)-logfbit7(1+x).
    //    return -120.0 * (2.0 * x + 3.0) * ((((14.0 * x + 84.0) * x + 196.0) * x + 210.0) * x + 87.0) * Math.Pow(((x + 1.0) * (x + 2.0)), -7);
    //}

    //private static  double logfbit7(double x)
    //{
    //    double functionReturnValue = 0;
    //    //// Seventh derivative of error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
    //    double x1 = 0;
    //    double x2 = 0;
    //    if ((x >= 10000000000.0)) {
    //        functionReturnValue = -420.0 * Math.Pow((x + 1.0), -8);
    //    } else if ((x >= 6.0)) {
    //        double x3 = 0;
    //        x1 = x + 1.0;
    //        x2 = 1.0 / (x1 * x1);
    //        x3 = x2 * (586051200.0 * lfbc8 - x2 * 1235591280.0 * lfbc9);
    //        x3 = x2 * (98017920.0 * lfbc6 - x2 * (253955520.0 * lfbc7 - x3));
    //        x3 = x2 * (8648640.0 * lfbc4 - x2 * (32432400.0 * lfbc5 - x3));
    //        x3 = x2 * (181440.0 * lfbc2 - x2 * (1663200.0 * lfbc3 - x3));
    //        functionReturnValue = -lfbc1 * (5040.0 - x3) * x2 * x2 * x2 * x2;
    //    } else if ((x > -1.0)) {
    //        x1 = x;
    //        x2 = 0.0;
    //        while ((x1 < 6.0)) {
    //            x2 = x2 + logfbit7dif(x1);
    //            x1 = x1 + 1.0;
    //        }
    //        functionReturnValue = x2 + logfbit7(x1);
    //    } else {
    //        functionReturnValue = -1E+308;
    //    }
    //    return functionReturnValue;
    //}

    //private static  double lfbaccdif(double a, double B)
    //{
    //    double functionReturnValue = 0;
    //    //// This is now always reasonably accurate, although it is not always required to be so when called from incbeta.
    //    if ((a > 0.03 * (a + B + 1.0))) {
    //        functionReturnValue = logfbit(a + B) - logfbit(B);
    //    } else {
    //        double a2 = 0;
    //        double ab2 = 0;
    //        a2 = a * a;
    //        ab2 = a / 2.0 + B;
    //        functionReturnValue = a * (logfbit1(ab2) + a2 / 24.0 * (logfbit3(ab2) + a2 / 80.0 * (logfbit5(ab2) + a2 / 168.0 * logfbit7(ab2))));
    //    }
    //    return functionReturnValue;
    //}

    //private static  double compbfunc(double x, double a, double B)
    //{
    //    //// Calculates a*(b-1)*x(1/(a+1) - (b-2)*x/2*(1/(a+2) - (b-3)*x/3*(1/(a+3) - ...)))
    //    //// Mainly for calculating the complement of BETA(x,a,b) for small a and b*x < 1.
    //    //// a should be close to 0, x >= 0 & x <=1 & b*x < 1
    //    double term = 0;
    //    double D = 0;
    //    double sum = 0;
    //    term = x;
    //    D = 2.0;
    //    sum = term / (a + 1.0);
    //    while ((Math.Abs(term) > Math.Abs(sum * sumAcc))) {
    //        term = -term * (B - D) * x / D;
    //        sum = sum + term / (a + D);
    //        D = D + 1.0;
    //    }
    //    return a * (B - 1.0) * sum;
    //}

    //private static  double incbeta(double x, double a, double B, bool comp)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculates BETA for small a (complementary BETA if comp).
    //    double r = 0;
    //    if ((x > 0.5)) {
    //        functionReturnValue = incbeta(1.0 - x, B, a, !comp);
    //    } else {
    //        r = (a + B + 0.5) * log1(a / (1.0 + B)) + a * ((a - 0.5) / (1.0 + B) + Math.Log((1.0 + B) * x)) - lngammaexpansion(a);
    //        if ((comp)) {
    //            r = -expm1(r + lfbaccdif(a, B));
    //            r = r + compbfunc(x, a, B) * (1.0 - r);
    //            r = r + (a / (a + B)) * (1.0 - r);
    //        } else {
    //            r = Math.Exp(r + (logfbit(a + B) - logfbit(B))) * (1.0 - compbfunc(x, a, B)) * (B / (a + B));
    //        }
    //        functionReturnValue = r;
    //    }
    //    return functionReturnValue;
    //}

    //private static  double BETA(double x, double a, double B)
    //{
    //    double functionReturnValue = 0;
    //    //Assumes x >= 0 & a >= 0 & b >= 0. Only called with a = 0 or b = 0 by (comp)BETA_nc
    //    if ((a == 0.0 & B == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((a == 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((B == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x >= 1.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((a < 1.0 & B < 1.0)) {
    //        functionReturnValue = incbeta(x, a, B, false);
    //    } else if ((a < 1.0 & (1.0 + B) * x <= 1.0)) {
    //        functionReturnValue = incbeta(x, a, B, false);
    //    } else if ((B < 1.0 & a <= (1.0 + a) * x)) {
    //        functionReturnValue = incbeta(1.0 - x, B, a, true);
    //    } else if ((a < 1.0)) {
    //        functionReturnValue = compbinomial(-a, B, x, 1.0 - x, 0.0);
    //    } else if ((B < 1.0)) {
    //        functionReturnValue = binomial(-B, a, 1.0 - x, x, 0.0);
    //    } else {
    //        functionReturnValue = compbinomial(a - 1.0, B, x, 1.0 - x, (a + B - 1.0) * x - a + 1.0);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double compbeta(double x, double a, double B)
    //{
    //    double functionReturnValue = 0;
    //    //Assumes x >= 0 & a >= 0 & b >= 0. Only called with a = 0 or b = 0 by (comp)BETA_nc
    //    if ((a == 0.0 & B == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((a == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((B == 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((x >= 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((a < 1.0 & B < 1.0)) {
    //        functionReturnValue = incbeta(x, a, B, true);
    //    } else if ((a < 1.0 & (1.0 + B) * x <= 1.0)) {
    //        functionReturnValue = incbeta(x, a, B, true);
    //    } else if ((B < 1.0 & a <= (1.0 + a) * x)) {
    //        functionReturnValue = incbeta(1.0 - x, B, a, false);
    //    } else if ((a < 1.0)) {
    //        functionReturnValue = binomial(-a, B, x, 1.0 - x, 0.0);
    //    } else if ((B < 1.0)) {
    //        functionReturnValue = compbinomial(-B, a, 1.0 - x, x, 0.0);
    //    } else {
    //        functionReturnValue = binomial(a - 1.0, B, x, 1.0 - x, (a + B - 1.0) * x - a + 1.0);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double invincbeta(double a, double B, double prob, bool comp, ref double oneMinusP)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculates inverse of BETA for small a (inverse of complementary BETA if comp).
    //    double r = 0;
    //    double rb = 0;
    //    double x = 0;
    //    double OneOverDeriv = 0;
    //    double dif = 0;
    //    double pr = 0;
    //    double mnab = 0;
    //    double aplusbOvermxab = 0;
    //    double lpr = 0;
    //    double small = 0;
    //    double smalllpr = 0;
    //    if ((!comp & prob > B / (a + B))) {
    //        functionReturnValue = invincbeta(a, B, 1.0 - prob, !comp, ref oneMinusP);
    //        return functionReturnValue;
    //    } else if ((comp & prob > a / (a + B) & prob > 0.1)) {
    //        functionReturnValue = invincbeta(a, B, 1.0 - prob, !comp, ref oneMinusP);
    //        return functionReturnValue;
    //    }
    //    lpr = max(-Math.Log(prob), 1.0);
    //    small = 1E-14;
    //    smalllpr = small * lpr * prob;
    //    if (a >= B) {
    //        mnab = B;
    //        aplusbOvermxab = (a + B) / a;
    //    } else {
    //        mnab = a;
    //        aplusbOvermxab = (a + B) / B;
    //    }
    //    if ((comp)) {
    //        r = (a + B + 0.5) * log1(a / (1.0 + B)) + a * (a - 0.5) / (1.0 + B) + lfbaccdif(a, B) - lngammaexpansion(a);
    //        r = -expm1(r);
    //        r = r + (a / (a + B)) * (1.0 - r);
    //        if ((B < 1.0)) {
    //            rb = (a + B + 0.5) * log1(B / (1.0 + a)) + B * (B - 0.5) / (1.0 + a) + (logfbit(a + B) - logfbit(a)) - lngammaexpansion(B);
    //            rb = Math.Exp(rb) * (a / (a + B));
    //            oneMinusP = Math.Log(prob / rb) / B;
    //            if ((oneMinusP < 0.0)) {
    //                oneMinusP = Math.Exp(oneMinusP) / (1.0 + a);
    //            } else {
    //                oneMinusP = 0.5;
    //            }
    //            if ((oneMinusP == 0.0)) {
    //                functionReturnValue = 1.0;
    //                return functionReturnValue;
    //            } else if ((oneMinusP > 0.5)) {
    //                oneMinusP = 0.5;
    //            }
    //            x = 1.0 - oneMinusP;
    //            pr = rb * (1.0 - compbfunc(oneMinusP, B, a)) * Math.Exp(B * Math.Log((1 + a) * oneMinusP));
    //            OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0) * mnab);
    //            dif = OneOverDeriv * pr * logdif(pr, prob);
    //            oneMinusP = oneMinusP - dif;
    //            x = 1.0 - oneMinusP;
    //            if ((oneMinusP <= 0.0)) {
    //                oneMinusP = 0.0;
    //                functionReturnValue = 1.0;
    //                return functionReturnValue;
    //            } else if ((x < 0.25)) {
    //                x = Math.Exp(log0((r - prob) / (1.0 - r)) / a) / (B + 1.0);
    //                oneMinusP = 1.0 - x;
    //                if ((x == 0.0)) {
    //                    functionReturnValue = 0.0;
    //                    return functionReturnValue;
    //                }
    //                pr = compbfunc(x, a, B) * (1.0 - prob);
    //                OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0) * mnab);
    //                dif = OneOverDeriv * (prob + pr) * log0(pr / prob);
    //                x = x + dif;
    //                if ((x <= 0.0)) {
    //                    oneMinusP = 1.0;
    //                    functionReturnValue = 0.0;
    //                    return functionReturnValue;
    //                }
    //                oneMinusP = 1.0 - x;
    //            }
    //        } else {
    //            pr = Math.Exp(log0((r - prob) / (1.0 - r)) / a) / (B + 1.0);
    //            x = Math.Log(B * prob / (a * (1.0 - r) * B * Math.Exp(a * Math.Log(1 + B)))) / B;
    //            if ((Math.Abs(x) < 0.5)) {
    //                x = -expm1(x);
    //                oneMinusP = 1.0 - x;
    //            } else {
    //                oneMinusP = Math.Exp(x);
    //                x = 1.0 - oneMinusP;
    //                if ((oneMinusP == 0.0)) {
    //                    functionReturnValue = x;
    //                    return functionReturnValue;
    //                }
    //            }
    //            if (pr > x & pr < 1.0) {
    //                x = pr;
    //                oneMinusP = 1.0 - x;
    //            }
    //        }
    //        dif = Min(x, oneMinusP);
    //        pr = -1.0;
    //        while (((Math.Abs(pr - prob) > smalllpr) & (Math.Abs(dif) > small * max(cSmall, Min(x, oneMinusP))))) {
    //            if ((B < 1.0 & x > 0.5)) {
    //                pr = rb * (1.0 - compbfunc(oneMinusP, B, a)) * Math.Exp(B * Math.Log((1.0 + a) * oneMinusP));
    //            } else if (((1.0 + B) * x > 1.0)) {
    //                pr = binomial(-a, B, x, oneMinusP, 0.0);
    //            } else {
    //                pr = r + compbfunc(x, a, B) * (1.0 - r);
    //                pr = pr - expm1(a * Math.Log((1.0 + B) * x)) * (1.0 - pr);
    //            }
    //            OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0) * mnab);
    //            dif = OneOverDeriv * pr * logdif(pr, prob);
    //            if ((x > 0.5)) {
    //                oneMinusP = oneMinusP - dif;
    //                x = 1.0 - oneMinusP;
    //                if ((oneMinusP <= 0.0)) {
    //                    oneMinusP = 0.0;
    //                    functionReturnValue = 1.0;
    //                    return functionReturnValue;
    //                }
    //            } else {
    //                x = x + dif;
    //                oneMinusP = 1.0 - x;
    //                if ((x <= 0.0)) {
    //                    oneMinusP = 1.0;
    //                    functionReturnValue = 0.0;
    //                    return functionReturnValue;
    //                }
    //            }
    //        }
    //    } else {
    //        r = (a + B + 0.5) * log1(a / (1.0 + B)) + a * (a - 0.5) / (1.0 + B) + (logfbit(a + B) - logfbit(B)) - lngammaexpansion(a);
    //        r = Math.Exp(r) * (B / (a + B));
    //        x = logdif(prob, r);
    //        if ((x < -711.0 * a)) {
    //            x = 0.0;
    //        } else {
    //            x = Math.Exp(x / a) / (1.0 + B);
    //        }
    //        if ((x == 0.0)) {
    //            oneMinusP = 1.0;
    //            functionReturnValue = x;
    //            return functionReturnValue;
    //        } else if ((x >= 0.5)) {
    //            x = 0.5;
    //        }
    //        oneMinusP = 1.0 - x;
    //        pr = r * (1.0 - compbfunc(x, a, B)) * Math.Exp(a * Math.Log((1.0 + B) * x));
    //        OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0) * mnab);
    //        dif = OneOverDeriv * pr * logdif(pr, prob);
    //        x = x - dif;
    //        oneMinusP = oneMinusP + dif;
    //        while (((Math.Abs(pr - prob) > smalllpr) & (Math.Abs(dif) > small * max(cSmall, Min(x, oneMinusP))))) {
    //            if (((1.0 + B) * x > 1.0)) {
    //                pr = compbinomial(-a, B, x, oneMinusP, 0.0);
    //            } else if ((x > 0.5)) {
    //                pr = incbeta(oneMinusP, B, a, !comp);
    //            } else {
    //                pr = r * (1.0 - compbfunc(x, a, B)) * Math.Exp(a * Math.Log((1.0 + B) * x));
    //            }
    //            OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0) * mnab);
    //            dif = OneOverDeriv * pr * logdif(pr, prob);
    //            if (x < 0.5) {
    //                x = x - dif;
    //                oneMinusP = 1.0 - x;
    //            } else {
    //                oneMinusP = oneMinusP + dif;
    //                x = 1.0 - oneMinusP;
    //            }
    //        }
    //    }
    //    functionReturnValue = x;
    //    return functionReturnValue;
    //}

    //private static  double invbeta(double a, double B, double prob, ref double oneMinusP)
    //{
    //    double functionReturnValue = 0;
    //    double swap = 0;
    //    if ((prob == 0.0)) {
    //        oneMinusP = 1.0;
    //        functionReturnValue = 0.0;
    //    } else if ((prob == 1.0)) {
    //        oneMinusP = 0.0;
    //        functionReturnValue = 1.0;
    //    } else if ((a == B & prob == 0.5)) {
    //        oneMinusP = 0.5;
    //        functionReturnValue = 0.5;
    //    } else if ((a < B & B < 1.0)) {
    //        functionReturnValue = invincbeta(a, B, prob, false, ref oneMinusP);
    //    } else if ((B < a & a < 1.0)) {
    //        swap = invincbeta(B, a, prob, true, ref oneMinusP);
    //        functionReturnValue = oneMinusP;
    //        oneMinusP = swap;
    //    } else if ((a < 1.0)) {
    //        functionReturnValue = invincbeta(a, B, prob, false, ref oneMinusP);
    //    } else if ((B < 1.0)) {
    //        swap = invincbeta(B, a, prob, true, ref oneMinusP);
    //        functionReturnValue = oneMinusP;
    //        oneMinusP = swap;
    //    } else {
    //        functionReturnValue = invcompbinom(a - 1.0, B, prob, ref oneMinusP);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double invcompbeta(double a, double B, double prob, ref double oneMinusP)
    //{
    //    double functionReturnValue = 0;
    //    double swap = 0;
    //    if ((prob == 0.0)) {
    //        oneMinusP = 0.0;
    //        functionReturnValue = 1.0;
    //    } else if ((prob == 1.0)) {
    //        oneMinusP = 1.0;
    //        functionReturnValue = 0.0;
    //    } else if ((a == B & prob == 0.5)) {
    //        oneMinusP = 0.5;
    //        functionReturnValue = 0.5;
    //    } else if ((a < B & B < 1.0)) {
    //        functionReturnValue = invincbeta(a, B, prob, true, ref oneMinusP);
    //    } else if ((B < a & a < 1.0)) {
    //        swap = invincbeta(B, a, prob, false, ref oneMinusP);
    //        functionReturnValue = oneMinusP;
    //        oneMinusP = swap;
    //    } else if ((a < 1.0)) {
    //        functionReturnValue = invincbeta(a, B, prob, true, ref oneMinusP);
    //    } else if ((B < 1.0)) {
    //        swap = invincbeta(B, a, prob, false, ref oneMinusP);
    //        functionReturnValue = oneMinusP;
    //        oneMinusP = swap;
    //    } else {
    //        functionReturnValue = invbinom(a - 1.0, B, prob, ref oneMinusP);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double critpoiss(double Mean, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that Pr(poisson(mean,i)) >= cprob and  Pr(poisson(mean,i-1)) < cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critcomppoiss(Mean, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double dfm = 0;
    //    double i = 0;
    //    dfm = invcnormal(cprob) * Math.Sqrt(Mean);
    //    i = Convert.ToInt32(Mean + dfm + 0.5);
    //    while ((true)) {
    //        i = Convert.ToInt32(i);
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        }
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        dfm = Mean - i;
    //        pr = cpoisson(i, Mean, dfm);
    //        tpr = 0;
    //        if ((pr >= cprob)) {
    //            if ((i == 0.0)) {
    //                functionReturnValue = i;
    //                return functionReturnValue;
    //            }
    //            tpr = poissonTerm(i, Mean, dfm, 0.0);
    //            pr = pr - tpr;
    //            if ((pr < cprob)) {
    //                functionReturnValue = i;
    //                return functionReturnValue;
    //            }

    //            i = i - 1.0;
    //            double temp = 0;
    //            double temp2 = 0;
    //            temp = (pr - cprob) / tpr;
    //            if ((temp > 10)) {
    //                temp = Convert.ToInt32(temp + 0.5);
    //                i = i - temp;
    //                temp2 = poissonTerm(i, Mean, Mean - i, 0.0);
    //                i = i - temp * (tpr - temp2) / (2 * temp2);
    //            } else {
    //                tpr = tpr * (i + 1.0) / Mean;
    //                pr = pr - tpr;
    //                if ((pr < cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                i = i - 1.0;
    //                if ((i == 0.0)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                temp2 = (pr - cprob) / tpr;
    //                if ((temp2 < temp - 0.9)) {
    //                    while ((pr >= cprob)) {
    //                        tpr = tpr * (i + 1.0) / Mean;
    //                        pr = pr - tpr;
    //                        i = i - 1.0;
    //                    }
    //                    functionReturnValue = i + 1.0;
    //                    return functionReturnValue;
    //                } else {
    //                    temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log((i + 1.0) / Mean) + 0.5);
    //                    i = i - temp;
    //                    if ((i < 0.0)) {
    //                        i = 0.0;
    //                    }
    //                    temp2 = poissonTerm(i, Mean, Mean - i, 0.0);
    //                    if ((temp2 > nearly_zero)) {
    //                        temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log((i + 1.0) / Mean);
    //                        i = i - temp;
    //                    }
    //                }
    //            }
    //        } else {
    //            while (((tpr < cSmall) & (pr < cprob))) {
    //                i = i + 1.0;
    //                dfm = dfm - 1.0;
    //                tpr = poissonTerm(i, Mean, dfm, 0.0);
    //                pr = pr + tpr;
    //            }
    //            while ((pr < cprob)) {
    //                i = i + 1.0;
    //                tpr = tpr * Mean / i;
    //                pr = pr + tpr;
    //            }
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critcomppoiss(double Mean, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that 1-Pr(poisson(mean,i)) > cprob and  1-Pr(poisson(mean,i-1)) <= cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critpoiss(Mean, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double dfm = 0;
    //    double i = 0;
    //    dfm = invcnormal(cprob) * Math.Sqrt(Mean);
    //    i = Convert.ToInt32(Mean - dfm + 0.5);
    //    while ((true)) {
    //        i = Convert.ToInt32(i);
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        dfm = Mean - i;
    //        pr = comppoisson(i, Mean, dfm);
    //        tpr = 0;
    //        if ((pr > cprob)) {
    //            i = i + 1.0;
    //            dfm = dfm - 1.0;
    //            tpr = poissonTerm(i, Mean, dfm, 0.0);
    //            if ((pr < (1.00001) * tpr)) {
    //                while ((tpr > cprob)) {
    //                    i = i + 1.0;
    //                    tpr = tpr * Mean / i;
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr <= cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                double temp = 0;
    //                double temp2 = 0;
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i + temp;
    //                    temp2 = poissonTerm(i, Mean, Mean - i, 0.0);
    //                    i = i + temp * (tpr - temp2) / (2.0 * temp2);
    //                } else if ((pr / tpr > 1E-05)) {
    //                    i = i + 1.0;
    //                    tpr = tpr * Mean / i;
    //                    pr = pr - tpr;
    //                    if ((pr <= cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr > cprob)) {
    //                            i = i + 1.0;
    //                            tpr = tpr * Mean / i;
    //                            pr = pr - tpr;
    //                        }
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Math.Log(cprob / pr) / Math.Log(Mean / i);
    //                        temp = Convert.ToInt32((Math.Log(cprob / pr) - temp * Math.Log(i / (temp + i))) / Math.Log(Mean / i) + 0.5);
    //                        i = i + temp;
    //                        temp2 = poissonTerm(i, Mean, Mean - i, 0.0);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(Mean / i);
    //                            i = i + temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else {
    //            while (((tpr < cSmall) & (pr <= cprob))) {
    //                tpr = poissonTerm(i, Mean, dfm, 0.0);
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //                dfm = dfm + 1.0;
    //            }
    //            while ((pr <= cprob)) {
    //                tpr = tpr * (i + 1.0) / Mean;
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //            }
    //            functionReturnValue = i + 1.0;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critbinomial(double N, double eprob, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that Pr(binomial(n,eprob,i)) >= cprob and  Pr(binomial(n,eprob,i-1)) < cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critcompbinomial(N, eprob, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double dfm = 0;
    //    double i = 0;
    //    dfm = invcnormal(cprob) * Math.Sqrt(N * eprob * (1.0 - eprob));
    //    i = N * eprob + dfm;
    //    while ((true)) {
    //        i = Convert.ToInt32(i);
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        } else if ((i > N)) {
    //            i = N;
    //        }
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        dfm = N * eprob - i;
    //        pr = binomial(i, N - i, eprob, 1.0 - eprob, dfm);
    //        tpr = 0.0;
    //        if ((pr >= cprob)) {
    //            if ((i == 0.0)) {
    //                functionReturnValue = i;
    //                return functionReturnValue;
    //            }
    //            tpr = binomialTerm(i, N - i, eprob, 1.0 - eprob, dfm, 0.0);
    //            if ((pr < (1.00001) * tpr)) {
    //                tpr = tpr * ((i + 1.0) * (1.0 - eprob)) / ((N - i) * eprob);
    //                i = i - 1.0;
    //                while ((tpr >= cprob)) {
    //                    tpr = tpr * ((i + 1.0) * (1.0 - eprob)) / ((N - i) * eprob);
    //                    i = i - 1;
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr < cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                i = i - 1.0;
    //                if ((i == 0.0)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                double temp = 0;
    //                double temp2 = 0;
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10.0)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i - temp;
    //                    temp2 = binomialTerm(i, N - i, eprob, 1.0 - eprob, N * eprob - i, 0.0);
    //                    i = i - temp * (tpr - temp2) / (2.0 * temp2);
    //                } else {
    //                    tpr = tpr * ((i + 1.0) * (1.0 - eprob)) / ((N - i) * eprob);
    //                    pr = pr - tpr;
    //                    if ((pr < cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    i = i - 1.0;
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr >= cprob)) {
    //                            tpr = tpr * ((i + 1.0) * (1.0 - eprob)) / ((N - i) * eprob);
    //                            pr = pr - tpr;
    //                            i = i - 1.0;
    //                        }
    //                        functionReturnValue = i + 1.0;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log(((i + 1.0) * (1.0 - eprob)) / ((N - i) * eprob)) + 0.5);
    //                        i = i - temp;
    //                        if ((i < 0.0)) {
    //                            i = 0.0;
    //                        }
    //                        temp2 = binomialTerm(i, N - i, eprob, 1.0 - eprob, N * eprob - i, 0.0);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((i + 1.0) * (1.0 - eprob)) / ((N - i) * eprob));
    //                            i = i - temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else {
    //            while (((tpr < cSmall) & (pr < cprob))) {
    //                i = i + 1.0;
    //                dfm = dfm - 1.0;
    //                tpr = binomialTerm(i, N - i, eprob, 1.0 - eprob, dfm, 0.0);
    //                pr = pr + tpr;
    //            }
    //            while ((pr < cprob)) {
    //                i = i + 1.0;
    //                tpr = tpr * ((N - i + 1.0) * eprob) / (i * (1.0 - eprob));
    //                pr = pr + tpr;
    //            }
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critcompbinomial(double N, double eprob, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that 1-Pr(binomial(n,eprob,i)) > cprob and  1-Pr(binomial(n,eprob,i-1)) <= cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critbinomial(N, eprob, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double dfm = 0;
    //    double i = 0;
    //    dfm = invcnormal(cprob) * Math.Sqrt(N * eprob * (1.0 - eprob));
    //    i = N * eprob - dfm;
    //    while ((true)) {
    //        i = Convert.ToInt32(i);
    //        if ((i < 0.0)) {
    //            i = 0;
    //        } else if ((i > N)) {
    //            i = N;
    //        }
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        dfm = N * eprob - i;
    //        pr = compbinomial(i, N - i, eprob, 1.0 - eprob, dfm);
    //        tpr = 0.0;
    //        if ((pr > cprob)) {
    //            i = i + 1.0;
    //            dfm = dfm - 1.0;
    //            tpr = binomialTerm(i, N - i, eprob, 1.0 - eprob, dfm, 0.0);
    //            if ((pr < (1.00001) * tpr)) {
    //                while ((tpr > cprob)) {
    //                    i = i + 1.0;
    //                    tpr = tpr * ((N - i + 1.0) * eprob) / (i * (1.0 - eprob));
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr <= cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                double temp = 0;
    //                double temp2 = 0;
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10.0)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i + temp;
    //                    temp2 = binomialTerm(i, N - i, eprob, 1.0 - eprob, N * eprob - i, 0.0);
    //                    i = i + temp * (tpr - temp2) / (2.0 * temp2);
    //                } else {
    //                    i = i + 1.0;
    //                    tpr = tpr * ((N - i + 1.0) * eprob) / (i * (1.0 - eprob));
    //                    pr = pr - tpr;
    //                    if ((pr <= cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr > cprob)) {
    //                            i = i + 1.0;
    //                            tpr = tpr * ((N - i + 1.0) * eprob) / (i * (1.0 - eprob));
    //                            pr = pr - tpr;
    //                        }
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log(((N - i + 1.0) * eprob) / (i * (1.0 - eprob))) + 0.5);
    //                        i = i + temp;
    //                        if ((i > N)) {
    //                            i = N;
    //                        }
    //                        temp2 = binomialTerm(i, N - i, eprob, 1.0 - eprob, N * eprob - i, 0.0);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((N - i + 1.0) * eprob) / (i * (1.0 - eprob)));
    //                            i = i + temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else {
    //            while (((tpr < cSmall) & (pr <= cprob))) {
    //                tpr = binomialTerm(i, N - i, eprob, 1.0 - eprob, dfm, 0.0);
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //                dfm = dfm + 1.0;
    //            }
    //            while ((pr <= cprob)) {
    //                tpr = tpr * ((i + 1.0) * (1.0 - eprob)) / ((N - i) * eprob);
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //            }
    //            functionReturnValue = i + 1.0;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double crithyperg(double j, double k, double M, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that Pr(hypergeometric(i,j,k,m)) >= cprob and  Pr(hypergeometric(i-1,j,k,m)) < cprob
    //    double ha1 = 0;
    //    double hprob = 0;
    //    bool hswap = false;
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critcomphyperg(j, k, M, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double i = 0;
    //    i = j * k / M + invcnormal(cprob) * Math.Sqrt(j * k * (M - j) * (M - k) / (M * M * (M - 1.0)));
    //    double MX = 0;
    //    double mn = 0;
    //    MX = Min(j, k);
    //    mn = max(0, j + k - M);
    //    while ((true)) {
    //        if ((i < mn)) {
    //            i = mn;
    //        } else if ((i > MX)) {
    //            i = MX;
    //        }
    //        i = Convert.ToInt32(i + 0.5);
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        pr = hypergeometric(i, j - i, k - i, M - k - j + i, false, ref ha1, ref hprob, ref hswap);
    //        tpr = 0;
    //        if ((pr >= cprob)) {
    //            if ((i == mn)) {
    //                functionReturnValue = mn;
    //                return functionReturnValue;
    //            }
    //            tpr = hypergeometricTerm(i, j - i, k - i, M - k - j + i);
    //            if ((pr < (1 + 1E-05) * tpr)) {
    //                tpr = tpr * ((i + 1.0) * (M - j - k + i + 1.0)) / ((k - i) * (j - i));
    //                i = i - 1.0;
    //                while ((tpr > cprob)) {
    //                    tpr = tpr * ((i + 1.0) * (M - j - k + i + 1.0)) / ((k - i) * (j - i));
    //                    i = i - 1.0;
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr < cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                i = i - 1.0;
    //                if ((i == mn)) {
    //                    functionReturnValue = mn;
    //                    return functionReturnValue;
    //                }
    //                double temp = 0;
    //                double temp2 = 0;
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i - temp;
    //                    temp2 = hypergeometricTerm(i, j - i, k - i, M - k - j + i);
    //                    i = i - temp * (tpr - temp2) / (2.0 * temp2);
    //                } else {
    //                    tpr = tpr * ((i + 1.0) * (M - j - k + i + 1.0)) / ((k - i) * (j - i));
    //                    pr = pr - tpr;
    //                    if ((pr < cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    i = i - 1.0;
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr >= cprob)) {
    //                            tpr = tpr * ((i + 1.0) * (M - j - k + i + 1.0)) / ((k - i) * (j - i));
    //                            pr = pr - tpr;
    //                            i = i - 1.0;
    //                        }
    //                        functionReturnValue = i + 1.0;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log(((i + 1.0) * (M - j - k + i + 1.0)) / ((k - i) * (j - i))) + 0.5);
    //                        i = i - temp;
    //                        temp2 = hypergeometricTerm(i, j - i, k - i, M - k - j + i);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((i + 1.0) * (M - j - k + i + 1.0)) / ((k - i) * (j - i)));
    //                            i = i - temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else {
    //            while (((tpr < cSmall) & (pr < cprob))) {
    //                i = i + 1.0;
    //                tpr = hypergeometricTerm(i, j - i, k - i, M - k - j + i);
    //                pr = pr + tpr;
    //            }
    //            while ((pr < cprob)) {
    //                i = i + 1.0;
    //                tpr = tpr * ((k - i + 1.0) * (j - i + 1.0)) / (i * (M - j - k + i));
    //                pr = pr + tpr;
    //            }
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critcomphyperg(double j, double k, double M, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that 1-Pr(hypergeometric(i,j,k,m)) > cprob and  1-Pr(hypergeometric(i-1,j,k,m)) <= cprob
    //    double ha1 = 0;
    //    double hprob = 0;
    //    bool hswap = false;
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = crithyperg(j, k, M, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double i = 0;
    //    i = j * k / M - invcnormal(cprob) * Math.Sqrt(j * k * (M - j) * (M - k) / (M * M * (M - 1.0)));
    //    double MX = 0;
    //    double mn = 0;
    //    MX = Min(j, k);
    //    mn = max(0, j + k - M);
    //    while ((true)) {
    //        if ((i < mn)) {
    //            i = mn;
    //        } else if ((i > MX)) {
    //            i = MX;
    //        }
    //        i = Convert.ToInt32(i + 0.5);
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        pr = hypergeometric(i, j - i, k - i, M - k - j + i, true, ref ha1, ref hprob, ref hswap);
    //        tpr = 0.0;
    //        if ((pr > cprob)) {
    //            i = i + 1.0;
    //            tpr = hypergeometricTerm(i, j - i, k - i, M - k - j + i);
    //            if ((pr < (1.0 + 1E-05) * tpr)) {
    //                while ((tpr > cprob)) {
    //                    i = i + 1;
    //                    tpr = tpr * ((k - i + 1.0) * (j - i + 1.0)) / (i * (M - j - k + i));
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr <= cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                double temp = 0;
    //                double temp2 = 0;
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i + temp;
    //                    temp2 = hypergeometricTerm(i, j - i, k - i, M - k - j + i);
    //                    i = i + temp * (tpr - temp2) / (2.0 * temp2);
    //                } else {
    //                    i = i + 1.0;
    //                    tpr = tpr * ((k - i + 1.0) * (j - i + 1.0)) / (i * (M - j - k + i));
    //                    pr = pr - tpr;
    //                    if ((pr <= cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr > cprob)) {
    //                            i = i + 1.0;
    //                            tpr = tpr * ((k - i + 1.0) * (j - i + 1.0)) / (i * (M - j - k + i));
    //                            pr = pr - tpr;
    //                        }
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log(((k - i + 1.0) * (j - i + 1.0)) / (i * (M - j - k + i))) + 0.5);
    //                        i = i + temp;
    //                        temp2 = hypergeometricTerm(i, j - i, k, M - k);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((k - i + 1.0) * (j - i + 1.0)) / (i * (M - j - k + i)));
    //                            i = i + temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else {
    //            while (((tpr < cSmall) & (pr <= cprob))) {
    //                tpr = hypergeometricTerm(i, j - i, k - i, M - k - j + i);
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //            }
    //            while ((pr <= cprob)) {
    //                tpr = tpr * ((i + 1.0) * (M - j - k + i + 1.0)) / ((k - i) * (j - i));
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //            }
    //            functionReturnValue = i + 1.0;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critnegbinom(double N, double eprob, double fprob, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that Pr(negbinomial(n,eprob,i)) >= cprob and  Pr(negbinomial(n,eprob,i-1)) < cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critcompnegbinom(N, eprob, fprob, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double dfm = 0;
    //    double i = 0;
    //    i = invgamma(N * fprob, cprob) / eprob;
    //    while ((true)) {
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        }
    //        i = Convert.ToInt32(i);
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        if (eprob <= fprob) {
    //            pr = BETA(eprob, N, i + 1.0);
    //        } else {
    //            pr = compbeta(fprob, i + 1.0, N);
    //        }
    //        tpr = 0.0;
    //        if ((pr >= cprob)) {
    //            if ((i == 0.0)) {
    //                functionReturnValue = i;
    //                return functionReturnValue;
    //            }
    //            if (eprob <= fprob) {
    //                dfm = N - (N + i) * eprob;
    //            } else {
    //                dfm = (N + i) * fprob - i;
    //            }
    //            tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0);
    //            if ((pr < (1 + 1E-05) * tpr)) {
    //                tpr = tpr * (i + 1.0) / ((N + i) * fprob);
    //                i = i - 1.0;
    //                while ((tpr > cprob)) {
    //                    tpr = tpr * (i + 1.0) / ((N + i) * fprob);
    //                    i = i - 1.0;
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr < cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                i = i - 1.0;
    //                if ((i == 0.0)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                double temp = 0;
    //                double temp2 = 0;
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10.0)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i - temp;
    //                    if (eprob <= fprob) {
    //                        dfm = N - (N + i) * eprob;
    //                    } else {
    //                        dfm = (N + i) * fprob - i;
    //                    }
    //                    temp2 = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0);
    //                    i = i - temp * (tpr - temp2) / (2.0 * temp2);
    //                } else {
    //                    tpr = tpr * (i + 1.0) / ((N + i) * fprob);
    //                    pr = pr - tpr;
    //                    if ((pr < cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    i = i - 1.0;
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr >= cprob)) {
    //                            tpr = tpr * (i + 1.0) / ((N + i) * fprob);
    //                            pr = pr - tpr;
    //                            i = i - 1.0;
    //                        }
    //                        functionReturnValue = i + 1.0;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log((i + 1.0) / ((N + i) * fprob)) + 0.5);
    //                        i = i - temp;
    //                        if (eprob <= fprob) {
    //                            dfm = N - (N + i) * eprob;
    //                        } else {
    //                            dfm = (N + i) * fprob - i;
    //                        }
    //                        temp2 = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log((i + 1.0) / ((N + i) * fprob));
    //                            i = i - temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else {
    //            while (((tpr < cSmall) & (pr < cprob))) {
    //                i = i + 1.0;
    //                if (eprob <= fprob) {
    //                    dfm = N - (N + i) * eprob;
    //                } else {
    //                    dfm = (N + i) * fprob - i;
    //                }
    //                tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0);
    //                pr = pr + tpr;
    //            }
    //            while ((pr < cprob)) {
    //                i = i + 1.0;
    //                tpr = tpr * ((N + i - 1.0) * fprob) / i;
    //                pr = pr + tpr;
    //            }
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critcompnegbinom(double N, double eprob, double fprob, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that 1-Pr(negbinomial(n,eprob,i)) > cprob and  1-Pr(negbinomial(n,eprob,i-1)) <= cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critnegbinom(N, eprob, fprob, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double dfm = 0;
    //    double i = 0;
    //    i = invcompgamma(N * fprob, cprob) / eprob;
    //    while ((true)) {
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        }
    //        i = Convert.ToInt32(i);
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        if (eprob <= fprob) {
    //            pr = compbeta(eprob, N, i + 1.0);
    //        } else {
    //            pr = BETA(fprob, i + 1.0, N);
    //        }
    //        if ((pr > cprob)) {
    //            i = i + 1.0;
    //            if (eprob <= fprob) {
    //                dfm = N - (N + i) * eprob;
    //            } else {
    //                dfm = (N + i) * fprob - i;
    //            }
    //            tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0);
    //            if ((pr < (1.00001) * tpr)) {
    //                while ((tpr > cprob)) {
    //                    i = i + 1.0;
    //                    tpr = tpr * ((N + i - 1.0) * fprob) / i;
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr <= cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                } else if ((tpr < 1E-15 * pr)) {
    //                    if ((tpr < cSmall)) {
    //                        functionReturnValue = i;
    //                    } else {
    //                        functionReturnValue = i + Convert.ToInt32((pr - cprob) / tpr);
    //                    }
    //                    return functionReturnValue;
    //                }
    //                double temp = 0;
    //                double temp2 = 0;
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10.0)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i + temp;
    //                    if (eprob <= fprob) {
    //                        dfm = N - (N + i) * eprob;
    //                    } else {
    //                        dfm = (N + i) * fprob - i;
    //                    }
    //                    temp2 = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0);
    //                    i = i + temp * (tpr - temp2) / (2.0 * temp2);
    //                } else {
    //                    i = i + 1.0;
    //                    tpr = tpr * ((N + i - 1.0) * fprob) / i;
    //                    pr = pr - tpr;
    //                    if ((pr <= cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr > cprob)) {
    //                            i = i + 1.0;
    //                            tpr = tpr * ((N + i - 1.0) * fprob) / i;
    //                            pr = pr - tpr;
    //                        }
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log(((N + i - 1.0) * fprob) / i) + 0.5);
    //                        i = i + temp;
    //                        if (eprob <= fprob) {
    //                            dfm = N - (N + i) * eprob;
    //                        } else {
    //                            dfm = (N + i) * fprob - i;
    //                        }
    //                        temp2 = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((N + i - 1.0) * fprob) / i);
    //                            i = i + temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else {
    //            if (eprob <= fprob) {
    //                dfm = N - (N + i) * eprob;
    //            } else {
    //                dfm = (N + i) * fprob - i;
    //            }
    //            tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0);
    //            if ((tpr < 1E-15 * pr)) {
    //                if ((tpr < cSmall)) {
    //                    functionReturnValue = i;
    //                } else {
    //                    functionReturnValue = i - Convert.ToInt32((cprob - pr) / tpr);
    //                }
    //                return functionReturnValue;
    //            }
    //            while (((tpr < cSmall) & (pr <= cprob))) {
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //                if (eprob <= fprob) {
    //                    dfm = N - (N + i) * eprob;
    //                } else {
    //                    dfm = (N + i) * fprob - i;
    //                }
    //                tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0);
    //            }
    //            while ((pr <= cprob)) {
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //                if (i < 0.0) {
    //                    functionReturnValue = 0.0;
    //                    return functionReturnValue;
    //                }
    //                tpr = tpr * (i + 1.0) / ((N + i) * fprob);
    //            }
    //            functionReturnValue = i + 1.0;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critneghyperg(double j, double k, double M, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that Pr(neghypergeometric(i,j,k,m)) >= cprob and  Pr(neghypergeometric(i-1,j,k,m)) < cprob
    //    double ha1 = 0;
    //    double hprob = 0;
    //    bool hswap = false;
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critcompneghyperg(j, k, M, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double i = 0;
    //    double temp = 0;
    //    double temp2 = 0;
    //    double oneMinusP = 0;
    //    pr = (M - k) / M;
    //    i = invbeta(j * pr, pr * (k - j + 1.0), cprob, ref oneMinusP) * (M - k);
    //    while ((true)) {
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        } else if ((i > M - k)) {
    //            i = M - k;
    //        }
    //        i = Convert.ToInt32(i + 0.5);
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        pr = hypergeometric(i, j, M - k - i, k - j, false, ref ha1, ref hprob, ref hswap);
    //        tpr = 0.0;
    //        if ((pr >= cprob)) {
    //            if ((i == 0.0)) {
    //                functionReturnValue = 0.0;
    //                return functionReturnValue;
    //            }
    //            tpr = hypergeometricTerm(j - 1.0, i, k - j + 1.0, M - k - i) * (k - j + 1.0) / (M - j - i + 1.0);
    //            if ((pr < (1.0 + 1E-05) * tpr)) {
    //                tpr = tpr * ((i + 1.0) * (M - j - i)) / ((j + i) * (M - i - k));
    //                i = i - 1.0;
    //                while ((tpr > cprob)) {
    //                    tpr = tpr * ((i + 1.0) * (M - j - i)) / ((j + i) * (M - i - k));
    //                    i = i - 1.0;
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr < cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                i = i - 1.0;

    //                if ((i == 0.0)) {
    //                    functionReturnValue = 0.0;
    //                    return functionReturnValue;
    //                }
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i - temp;
    //                    temp2 = hypergeometricTerm(j - 1.0, i, k - j + 1.0, M - k - i) * (k - j + 1.0) / (M - j - i + 1.0);
    //                    i = i - temp * (tpr - temp2) / (2 * temp2);
    //                } else {
    //                    tpr = tpr * ((i + 1.0) * (M - j - i)) / ((j + i) * (M - i - k));
    //                    pr = pr - tpr;
    //                    if ((pr < cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    i = i - 1.0;
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr >= cprob)) {
    //                            tpr = tpr * ((i + 1.0) * (M - j - i)) / ((j + i) * (M - i - k));
    //                            pr = pr - tpr;
    //                            i = i - 1.0;
    //                        }
    //                        functionReturnValue = i + 1.0;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log(((i + 1.0) * (M - j - i)) / ((j + i) * (M - i - k))) + 0.5);
    //                        i = i - temp;
    //                        temp2 = hypergeometricTerm(j - 1.0, i, k - j + 1.0, M - k - i) * (k - j + 1.0) / (M - j - i + 1.0);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((i + 1.0) * (M - j - i)) / ((j + i) * (M - i - k)));
    //                            i = i - temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else {
    //            while (((tpr < cSmall) & (pr < cprob))) {
    //                i = i + 1.0;
    //                tpr = hypergeometricTerm(j - 1.0, i, k - j + 1.0, M - k - i) * (k - j + 1.0) / (M - j - i + 1.0);
    //                pr = pr + tpr;
    //            }
    //            while ((pr < cprob)) {
    //                i = i + 1.0;
    //                tpr = tpr * ((j + i - 1.0) * (M - i - k + 1.0)) / (i * (M - j - i + 1.0));
    //                pr = pr + tpr;
    //            }
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critcompneghyperg(double j, double k, double M, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that 1-Pr(neghypergeometric(i,j,k,m)) > cprob and  1-Pr(neghypergeometric(i-1,j,k,m)) <= cprob
    //    double ha1 = 0;
    //    double hprob = 0;
    //    bool hswap = false;
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critneghyperg(j, k, M, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double i = 0;
    //    double temp = 0;
    //    double temp2 = 0;
    //    double oneMinusP = 0;
    //    pr = (M - k) / M;
    //    i = invcompbeta(j * pr, pr * (k - j + 1.0), cprob, ref oneMinusP) * (M - k);
    //    while ((true)) {
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        } else if ((i > M - k)) {
    //            i = M - k;
    //        }
    //        i = Convert.ToInt32(i + 0.5);
    //        if ((i >= max_crit)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        pr = hypergeometric(i, j, M - k - i, k - j, true, ref ha1, ref hprob, ref hswap);
    //        tpr = 0.0;
    //        if ((pr > cprob)) {
    //            i = i + 1.0;
    //            tpr = hypergeometricTerm(j - 1.0, i, k - j + 1.0, M - k - i) * (k - j + 1.0) / (M - j - i + 1.0);
    //            if ((pr < (1 + 1E-05) * tpr)) {
    //                while ((tpr > cprob)) {
    //                    i = i + 1.0;
    //                    temp = M - j - i + 1.0;
    //                    if (temp == 0.0)
    //                        break; // TODO: might not be correct. Was : Exit Do
    //                    tpr = tpr * ((j + i - 1.0) * (M - i - k + 1.0)) / (i * temp);
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr <= cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10.0)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i + temp;
    //                    temp2 = hypergeometricTerm(j - 1.0, i, k - j + 1.0, M - k - i) * (k - j + 1.0) / (M - j - i + 1.0);
    //                    i = i + temp * (tpr - temp2) / (2 * temp2);
    //                } else {
    //                    i = i + 1.0;
    //                    tpr = tpr * ((j + i - 1.0) * (M - i - k + 1.0)) / (i * (M - j - i + 1.0));
    //                    pr = pr - tpr;
    //                    if ((pr <= cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr > cprob)) {
    //                            i = i + 1.0;
    //                            tpr = tpr * ((j + i - 1.0) * (M - i - k + 1.0)) / (i * (M - j - i + 1.0));
    //                            pr = pr - tpr;
    //                        }
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log(((j + i - 1.0) * (M - i - k + 1.0)) / (i * (M - j - i + 1.0))) + 0.5);
    //                        i = i + temp;
    //                        temp2 = hypergeometricTerm(j - 1.0, i, k - j + 1.0, M - k - i) * (k - j + 1.0) / (M - j - i + 1.0);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((j + i - 1.0) * (M - i - k + 1.0)) / (i * (M - j - i + 1.0)));
    //                            i = i + temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else {
    //            while (((tpr < cSmall) & (pr <= cprob))) {
    //                tpr = hypergeometricTerm(j - 1.0, i, k - j + 1.0, M - k - i) * (k - j + 1.0) / (M - j - i + 1.0);
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //            }
    //            while ((pr <= cprob)) {
    //                tpr = tpr * ((i + 1.0) * (M - j - i)) / ((j + i) * (M - i - k));
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //            }
    //            functionReturnValue = i + 1.0;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    private static double AlterForIntegralChecks_Others(double Value)
    {
        double functionReturnValue = 0;
        if (NonIntegralValuesAllowed_Others)
        {
            functionReturnValue = Convert.ToInt32(Value);
        }
        else if (Value != Convert.ToInt32(Value))
        {
            functionReturnValue = ErrorValue;
        }
        else
        {
            functionReturnValue = Value;
        }
        return functionReturnValue;
    }

    private static double AlterForIntegralChecks_df(double Value)
    {
        double functionReturnValue = 0;
        if (NonIntegralValuesAllowed_df)
        {
            functionReturnValue = Value;
        }
        else
        {
            functionReturnValue = AlterForIntegralChecks_Others(Value);
        }
        return functionReturnValue;
    }

    //private static  double AlterForIntegralChecks_NB(double Value)
    //{
    //    double functionReturnValue = 0;
    //    if (NonIntegralValuesAllowed_NB) {
    //        functionReturnValue = Value;
    //    } else {
    //        functionReturnValue = AlterForIntegralChecks_Others(Value);
    //    }
    //    return functionReturnValue;
    //}

    private static double GetRidOfMinusZeroes(double x)
    {
        double functionReturnValue = 0;
        if (x == 0.0)
        {
            functionReturnValue = 0.0;
        }
        else
        {
            functionReturnValue = x;
        }
        return functionReturnValue;
    }

    //protected internal static double pmf_geometric(double failures, double success_prob)
    //{
    //    double functionReturnValue = 0;
    //    failures = AlterForIntegralChecks_Others(failures);
    //    if ((success_prob < 0.0 | success_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if (failures < 0.0) {
    //        functionReturnValue = 0.0;
    //    } else if (success_prob == 1.0) {
    //        if (failures == 0.0) {
    //            functionReturnValue = 1.0;
    //        } else {
    //            functionReturnValue = 0.0;
    //        }
    //    } else {
    //        functionReturnValue = success_prob * Math.Exp(failures * log0(-success_prob));
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_geometric(double failures, double success_prob)
    //{
    //    double functionReturnValue = 0;
    //    failures = Convert.ToInt32(failures);
    //    if ((success_prob < 0.0 | success_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if (failures < 0.0) {
    //        functionReturnValue = 0.0;
    //    } else if (success_prob == 1.0) {
    //        if (failures >= 0.0) {
    //            functionReturnValue = 1.0;
    //        } else {
    //            functionReturnValue = 0.0;
    //        }
    //    } else {
    //        functionReturnValue = -expm1((failures + 1.0) * log0(-success_prob));
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_geometric(double failures, double success_prob)
    //{
    //    double functionReturnValue = 0;
    //    failures = Convert.ToInt32(failures);
    //    if ((success_prob < 0.0 | success_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if (failures < 0.0) {
    //        functionReturnValue = 1.0;
    //    } else if (success_prob == 1.0) {
    //        if (failures >= 0.0) {
    //            functionReturnValue = 0.0;
    //        } else {
    //            functionReturnValue = 1.0;
    //        }
    //    } else {
    //        functionReturnValue = Math.Exp((failures + 1.0) * log0(-success_prob));
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double crit_geometric(double success_prob, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    if ((success_prob <= 0.0 | success_prob > 1.0 | crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((success_prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = Convert.ToInt32(log0(-crit_prob) / log0(-success_prob) - 1.0);
    //        if (-expm1((functionReturnValue + 1.0) * log0(-success_prob)) < crit_prob) {
    //            functionReturnValue = functionReturnValue + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_crit_geometric(double success_prob, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    if ((success_prob <= 0.0 | success_prob > 1.0 | crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((success_prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = Convert.ToInt32(Math.Log(crit_prob) / log0(-success_prob) - 1.0);
    //        if (Math.Exp((functionReturnValue + 1.0) * log0(-success_prob)) > crit_prob) {
    //            functionReturnValue = functionReturnValue + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double lcb_geometric(double failures, double prob)
    //{
    //    double functionReturnValue = 0;
    //    failures = AlterForIntegralChecks_Others(failures);
    //    if ((prob < 0.0 | prob > 1.0 | failures < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        functionReturnValue = -expm1(log0(-prob) / (failures + 1.0));
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ucb_geometric(double failures, double prob)
    //{
    //    double functionReturnValue = 0;
    //    failures = AlterForIntegralChecks_Others(failures);
    //    if ((prob < 0.0 | prob > 1.0 | failures < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | failures == 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = -expm1(Math.Log(prob) / failures);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pmf_negbinomial(double failures, double success_prob, double successes_reqd)
    //{
    //    double functionReturnValue = 0;
    //    double Q = 0;
    //    double dfm = 0;
    //    failures = AlterForIntegralChecks_Others(failures);
    //    successes_reqd = AlterForIntegralChecks_NB(successes_reqd);
    //    if ((success_prob < 0.0 | success_prob > 1.0 | successes_reqd <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((successes_reqd + failures > 0.0)) {
    //        Q = 1.0 - success_prob;
    //        if (success_prob <= Q) {
    //            dfm = successes_reqd - (successes_reqd + failures) * success_prob;
    //        } else {
    //            dfm = (successes_reqd + failures) * Q - failures;
    //        }
    //        functionReturnValue = successes_reqd / (successes_reqd + failures) * binomialTerm(failures, successes_reqd, Q, success_prob, dfm, 0.0);
    //    } else if ((failures != 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = 1.0;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_negbinomial(double failures, double success_prob, double successes_reqd)
    //{
    //    double functionReturnValue = 0;
    //    double Q = 0;
    //    failures = Convert.ToInt32(failures);
    //    successes_reqd = AlterForIntegralChecks_NB(successes_reqd);
    //    if ((success_prob < 0.0 | success_prob > 1.0 | successes_reqd <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        Q = 1.0 - success_prob;
    //        if (Q < success_prob) {
    //            functionReturnValue = compbeta(Q, failures + 1, successes_reqd);
    //        } else {
    //            functionReturnValue = BETA(success_prob, successes_reqd, failures + 1);
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_negbinomial(double failures, double success_prob, double successes_reqd)
    //{
    //    double functionReturnValue = 0;
    //    double Q = 0;
    //    failures = Convert.ToInt32(failures);
    //    successes_reqd = AlterForIntegralChecks_NB(successes_reqd);
    //    if ((success_prob < 0.0 | success_prob > 1.0 | successes_reqd <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        Q = 1.0 - success_prob;
    //        if (Q < success_prob) {
    //            functionReturnValue = BETA(Q, failures + 1, successes_reqd);
    //        } else {
    //            functionReturnValue = compbeta(success_prob, successes_reqd, failures + 1);
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double crit_negbinomial(double success_prob, double successes_reqd, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    successes_reqd = AlterForIntegralChecks_NB(successes_reqd);
    //    if ((success_prob <= 0.0 | success_prob > 1.0 | successes_reqd <= 0.0 | crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((success_prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double i = 0;
    //        double pr = 0;
    //        functionReturnValue = critnegbinom(successes_reqd, success_prob, 1.0 - success_prob, crit_prob);
    //        i = functionReturnValue;
    //        pr = cdf_negbinomial(i, success_prob, successes_reqd);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr > crit_prob)) {
    //            i = i - 1.0;
    //            pr = cdf_negbinomial(i, success_prob, successes_reqd);
    //            if ((pr >= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_crit_negbinomial(double success_prob, double successes_reqd, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    successes_reqd = AlterForIntegralChecks_NB(successes_reqd);
    //    if ((success_prob <= 0.0 | success_prob > 1.0 | successes_reqd <= 0.0 | crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((success_prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double i = 0;
    //        double pr = 0;
    //        functionReturnValue = critcompnegbinom(successes_reqd, success_prob, 1.0 - success_prob, crit_prob);
    //        i = functionReturnValue;
    //        pr = comp_cdf_negbinomial(i, success_prob, successes_reqd);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr < crit_prob)) {
    //            i = i - 1.0;
    //            pr = comp_cdf_negbinomial(i, success_prob, successes_reqd);
    //            if ((pr <= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double lcb_negbinomial(double failures, double successes_reqd, double prob)
    //{
    //    double functionReturnValue = 0;
    //    failures = AlterForIntegralChecks_Others(failures);
    //    successes_reqd = AlterForIntegralChecks_NB(successes_reqd);
    //    if ((prob < 0.0 | prob > 1.0 | failures < 0.0 | successes_reqd <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        double oneMinusP = 0;
    //        functionReturnValue = invbeta(successes_reqd, failures + 1, prob, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ucb_negbinomial(double failures, double successes_reqd, double prob)
    //{
    //    double functionReturnValue = 0;
    //    failures = AlterForIntegralChecks_Others(failures);
    //    successes_reqd = AlterForIntegralChecks_NB(successes_reqd);
    //    if ((prob < 0.0 | prob > 1.0 | failures < 0.0 | successes_reqd <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | failures == 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double oneMinusP = 0;
    //        functionReturnValue = invcompbeta(successes_reqd, failures, prob, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pmf_binomial(double SAMPLE_SIZE, double successes, double success_prob)
    //{
    //    double functionReturnValue = 0;
    //    double Q = 0;
    //    double dfm = 0;
    //    successes = AlterForIntegralChecks_Others(successes);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((success_prob < 0.0 | success_prob > 1.0 | SAMPLE_SIZE < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        Q = 1.0 - success_prob;
    //        if (success_prob <= Q) {
    //            dfm = SAMPLE_SIZE * success_prob - successes;
    //        } else {
    //            dfm = (SAMPLE_SIZE - successes) - SAMPLE_SIZE * Q;
    //        }
    //        functionReturnValue = binomialTerm(successes, SAMPLE_SIZE - successes, success_prob, Q, dfm, 0.0);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_binomial(double SAMPLE_SIZE, double successes, double success_prob)
    //{
    //    double functionReturnValue = 0;
    //    double Q = 0;
    //    double dfm = 0;
    //    successes = Convert.ToInt32(successes);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((success_prob < 0.0 | success_prob > 1.0 | SAMPLE_SIZE < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        Q = 1.0 - success_prob;
    //        if (success_prob <= Q) {
    //            dfm = SAMPLE_SIZE * success_prob - successes;
    //        } else {
    //            dfm = (SAMPLE_SIZE - successes) - SAMPLE_SIZE * Q;
    //        }
    //        functionReturnValue = binomial(successes, SAMPLE_SIZE - successes, success_prob, Q, dfm);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_binomial(double SAMPLE_SIZE, double successes, double success_prob)
    //{
    //    double functionReturnValue = 0;
    //    double Q = 0;
    //    double dfm = 0;
    //    successes = Convert.ToInt32(successes);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((success_prob < 0.0 | success_prob > 1.0 | SAMPLE_SIZE < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        Q = 1.0 - success_prob;
    //        if (success_prob <= Q) {
    //            dfm = SAMPLE_SIZE * success_prob - successes;
    //        } else {
    //            dfm = (SAMPLE_SIZE - successes) - SAMPLE_SIZE * Q;
    //        }
    //        functionReturnValue = compbinomial(successes, SAMPLE_SIZE - successes, success_prob, Q, dfm);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double crit_binomial(double SAMPLE_SIZE, double success_prob, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((success_prob < 0.0 | success_prob > 1.0 | SAMPLE_SIZE < 0.0 | crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((success_prob == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((crit_prob == 1.0 | success_prob == 1.0)) {
    //        functionReturnValue = SAMPLE_SIZE;
    //    } else {
    //        double pr = 0;
    //        double i = 0;
    //        functionReturnValue = critbinomial(SAMPLE_SIZE, success_prob, crit_prob);
    //        i = functionReturnValue;
    //        pr = cdf_binomial(SAMPLE_SIZE, i, success_prob);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr > crit_prob)) {
    //            i = i - 1.0;
    //            pr = cdf_binomial(SAMPLE_SIZE, i, success_prob);
    //            if ((pr >= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_crit_binomial(double SAMPLE_SIZE, double success_prob, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((success_prob < 0.0 | success_prob > 1.0 | SAMPLE_SIZE < 0.0 | crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0 | success_prob == 1.0)) {
    //        functionReturnValue = SAMPLE_SIZE;
    //    } else if ((success_prob == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double pr = 0;
    //        double i = 0;
    //        functionReturnValue = critcompbinomial(SAMPLE_SIZE, success_prob, crit_prob);
    //        i = functionReturnValue;
    //        pr = comp_cdf_binomial(SAMPLE_SIZE, i, success_prob);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr < crit_prob)) {
    //            i = i - 1.0;
    //            pr = comp_cdf_binomial(SAMPLE_SIZE, i, success_prob);
    //            if ((pr <= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double lcb_binomial(double SAMPLE_SIZE, double successes, double prob)
    //{
    //    double functionReturnValue = 0;
    //    successes = AlterForIntegralChecks_Others(successes);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((prob < 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((SAMPLE_SIZE < successes | successes < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | successes == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        double oneMinusP = 0;
    //        functionReturnValue = invcompbinom(successes - 1.0, SAMPLE_SIZE - successes + 1.0, prob, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ucb_binomial(double SAMPLE_SIZE, double successes, double prob)
    //{
    //    double functionReturnValue = 0;
    //    successes = AlterForIntegralChecks_Others(successes);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((prob < 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((SAMPLE_SIZE < successes | successes < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | successes == SAMPLE_SIZE)) {
    //        functionReturnValue = 1.0;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double oneMinusP = 0;
    //        functionReturnValue = invbinom(successes, SAMPLE_SIZE - successes, prob, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pmf_poisson(double Mean, double i)
    //{
    //    double functionReturnValue = 0;
    //    i = AlterForIntegralChecks_Others(i);
    //    if ((Mean < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((i < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = poissonTerm(i, Mean, Mean - i, 0.0);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_poisson(double Mean, double i)
    //{
    //    double functionReturnValue = 0;
    //    i = Convert.ToInt32(i);
    //    if ((Mean < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((i < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = cpoisson(i, Mean, Mean - i);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_poisson(double Mean, double i)
    //{
    //    double functionReturnValue = 0;
    //    i = Convert.ToInt32(i);
    //    if ((Mean < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((i < 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        functionReturnValue = comppoisson(i, Mean, Mean - i);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double crit_poisson(double Mean, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    if ((crit_prob < 0.0 | crit_prob > 1.0 | Mean < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((Mean == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double pr = 0;
    //        functionReturnValue = critpoiss(Mean, crit_prob);
    //        pr = cpoisson(functionReturnValue, Mean, Mean - functionReturnValue);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr > crit_prob)) {
    //            functionReturnValue = functionReturnValue - 1.0;
    //            pr = cpoisson(functionReturnValue, Mean, Mean - functionReturnValue);
    //            if ((pr < crit_prob)) {
    //                functionReturnValue = functionReturnValue + 1.0;
    //            }
    //        } else {
    //            functionReturnValue = functionReturnValue + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_crit_poisson(double Mean, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    if ((crit_prob < 0.0 | crit_prob > 1.0 | Mean < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((Mean == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double pr = 0;
    //        functionReturnValue = critcomppoiss(Mean, crit_prob);
    //        pr = comppoisson(functionReturnValue, Mean, Mean - functionReturnValue);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr < crit_prob)) {
    //            functionReturnValue = functionReturnValue - 1.0;
    //            pr = comppoisson(functionReturnValue, Mean, Mean - functionReturnValue);
    //            if ((pr > crit_prob)) {
    //                functionReturnValue = functionReturnValue + 1.0;
    //            }
    //        } else {
    //            functionReturnValue = functionReturnValue + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double lcb_poisson(double i, double prob)
    //{
    //    double functionReturnValue = 0;
    //    i = AlterForIntegralChecks_Others(i);
    //    if ((prob < 0.0 | prob > 1.0 | i < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | i == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = invcomppoisson(i - 1.0, prob);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ucb_poisson(double i, double prob)
    //{
    //    double functionReturnValue = 0;
    //    i = AlterForIntegralChecks_Others(i);
    //    if ((prob <= 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((i < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = invpoisson(i, prob);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pmf_hypergeometric(double type1s, double SAMPLE_SIZE, double tot_type1, double POP_SIZE)
    //{
    //    double functionReturnValue = 0;
    //    type1s = AlterForIntegralChecks_Others(type1s);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((SAMPLE_SIZE < 0.0 | tot_type1 < 0.0 | SAMPLE_SIZE > POP_SIZE | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = hypergeometricTerm(type1s, SAMPLE_SIZE - type1s, tot_type1 - type1s, POP_SIZE - tot_type1 - SAMPLE_SIZE + type1s);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_hypergeometric(double type1s, double SAMPLE_SIZE, double tot_type1, double POP_SIZE)
    //{
    //    double functionReturnValue = 0;
    //    type1s = Convert.ToInt32(type1s);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((SAMPLE_SIZE < 0.0 | tot_type1 < 0.0 | SAMPLE_SIZE > POP_SIZE | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double ha1 = 0;
    //        double hprob = 0;
    //        bool hswap = false;
    //        functionReturnValue = hypergeometric(type1s, SAMPLE_SIZE - type1s, tot_type1 - type1s, POP_SIZE - tot_type1 - SAMPLE_SIZE + type1s, false, ref ha1, ref hprob, ref hswap);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_hypergeometric(double type1s, double SAMPLE_SIZE, double tot_type1, double POP_SIZE)
    //{
    //    double functionReturnValue = 0;
    //    type1s = Convert.ToInt32(type1s);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((SAMPLE_SIZE < 0.0 | tot_type1 < 0.0 | SAMPLE_SIZE > POP_SIZE | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double ha1 = 0;
    //        double hprob = 0;
    //        bool hswap = false;
    //        functionReturnValue = hypergeometric(type1s, SAMPLE_SIZE - type1s, tot_type1 - type1s, POP_SIZE - tot_type1 - SAMPLE_SIZE + type1s, true, ref ha1, ref hprob, ref hswap);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double crit_hypergeometric(double SAMPLE_SIZE, double tot_type1, double POP_SIZE, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((SAMPLE_SIZE < 0.0 | tot_type1 < 0.0 | SAMPLE_SIZE > POP_SIZE | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((SAMPLE_SIZE == 0.0 | tot_type1 == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((SAMPLE_SIZE == POP_SIZE | tot_type1 == POP_SIZE)) {
    //        functionReturnValue = Min(SAMPLE_SIZE, tot_type1);
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = Min(SAMPLE_SIZE, tot_type1);
    //    } else {
    //        double ha1 = 0;
    //        double hprob = 0;
    //        bool hswap = false;
    //        double i = 0;
    //        double pr = 0;
    //        functionReturnValue = crithyperg(SAMPLE_SIZE, tot_type1, POP_SIZE, crit_prob);
    //        i = functionReturnValue;
    //        pr = hypergeometric(i, SAMPLE_SIZE - i, tot_type1 - i, POP_SIZE - tot_type1 - SAMPLE_SIZE + i, false, ref ha1, ref hprob, ref hswap);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr > crit_prob)) {
    //            i = i - 1.0;
    //            pr = hypergeometric(i, SAMPLE_SIZE - i, tot_type1 - i, POP_SIZE - tot_type1 - SAMPLE_SIZE + i, false, ref ha1, ref hprob, ref hswap);
    //            if ((pr >= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_crit_hypergeometric(double SAMPLE_SIZE, double tot_type1, double POP_SIZE, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((SAMPLE_SIZE < 0.0 | tot_type1 < 0.0 | SAMPLE_SIZE > POP_SIZE | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((SAMPLE_SIZE == 0.0 | tot_type1 == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((SAMPLE_SIZE == POP_SIZE | tot_type1 == POP_SIZE)) {
    //        functionReturnValue = Min(SAMPLE_SIZE, tot_type1);
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = Min(SAMPLE_SIZE, tot_type1);
    //    } else {
    //        double ha1 = 0;
    //        double hprob = 0;
    //        bool hswap = false;
    //        double i = 0;
    //        double pr = 0;
    //        functionReturnValue = critcomphyperg(SAMPLE_SIZE, tot_type1, POP_SIZE, crit_prob);
    //        i = functionReturnValue;
    //        pr = hypergeometric(i, SAMPLE_SIZE - i, tot_type1 - i, POP_SIZE - tot_type1 - SAMPLE_SIZE + i, true, ref ha1, ref hprob, ref hswap);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr < crit_prob)) {
    //            i = i - 1.0;
    //            pr = hypergeometric(i, SAMPLE_SIZE - i, tot_type1 - i, POP_SIZE - tot_type1 - SAMPLE_SIZE + i, true, ref ha1, ref hprob, ref hswap);
    //            if ((pr <= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double lcb_hypergeometric(double type1s, double SAMPLE_SIZE, double POP_SIZE, double prob)
    //{
    //    double functionReturnValue = 0;
    //    type1s = AlterForIntegralChecks_Others(type1s);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((prob < 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((type1s < 0.0 | type1s > SAMPLE_SIZE | SAMPLE_SIZE > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | type1s == 0.0 | POP_SIZE == SAMPLE_SIZE)) {
    //        functionReturnValue = type1s;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = POP_SIZE - (SAMPLE_SIZE - type1s);
    //    } else if ((prob < 0.5)) {
    //        functionReturnValue = critneghyperg(type1s, SAMPLE_SIZE, POP_SIZE, prob * (1.000000000001)) + type1s;
    //    } else {
    //        functionReturnValue = critcompneghyperg(type1s, SAMPLE_SIZE, POP_SIZE, (1.0 - prob) * (1.0 - 1E-12)) + type1s;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ucb_hypergeometric(double type1s, double SAMPLE_SIZE, double POP_SIZE, double prob)
    //{
    //    double functionReturnValue = 0;
    //    type1s = AlterForIntegralChecks_Others(type1s);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((prob < 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((type1s < 0.0 | type1s > SAMPLE_SIZE | SAMPLE_SIZE > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | type1s == SAMPLE_SIZE | POP_SIZE == SAMPLE_SIZE)) {
    //        functionReturnValue = POP_SIZE - (SAMPLE_SIZE - type1s);
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = type1s;
    //    } else if ((prob < 0.5)) {
    //        functionReturnValue = critcompneghyperg(type1s + 1.0, SAMPLE_SIZE, POP_SIZE, prob * (1.0 - 1E-12)) + type1s;
    //    } else {
    //        functionReturnValue = critneghyperg(type1s + 1.0, SAMPLE_SIZE, POP_SIZE, (1.0 - prob) * (1.000000000001)) + type1s;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pmf_neghypergeometric(double type2s, double type1s_reqd, double tot_type1, double POP_SIZE)
    //{
    //    double functionReturnValue = 0;
    //    type2s = AlterForIntegralChecks_Others(type2s);
    //    type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((type1s_reqd <= 0.0 | tot_type1 < type1s_reqd | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((type2s < 0.0 | tot_type1 + type2s > POP_SIZE)) {
    //        if (type2s == 0.0) {
    //            functionReturnValue = 1.0;
    //        } else {
    //            functionReturnValue = 0.0;
    //        }
    //    } else {
    //        functionReturnValue = hypergeometricTerm(type1s_reqd - 1.0, type2s, tot_type1 - type1s_reqd + 1.0, POP_SIZE - tot_type1 - type2s) * (tot_type1 - type1s_reqd + 1.0) / (POP_SIZE - type1s_reqd - type2s + 1.0);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_neghypergeometric(double type2s, double type1s_reqd, double tot_type1, double POP_SIZE)
    //{
    //    double functionReturnValue = 0;
    //    type2s = Convert.ToInt32(type2s);
    //    type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((type1s_reqd <= 0.0 | tot_type1 < type1s_reqd | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((tot_type1 + type2s > POP_SIZE)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        double ha1 = 0;
    //        double hprob = 0;
    //        bool hswap = false;
    //        functionReturnValue = hypergeometric(type2s, type1s_reqd, POP_SIZE - tot_type1 - type2s, tot_type1 - type1s_reqd, false, ref ha1, ref hprob, ref hswap);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_neghypergeometric(double type2s, double type1s_reqd, double tot_type1, double POP_SIZE)
    //{
    //    double functionReturnValue = 0;
    //    type2s = Convert.ToInt32(type2s);
    //    type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((type1s_reqd <= 0.0 | tot_type1 < type1s_reqd | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((tot_type1 + type2s > POP_SIZE)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double ha1 = 0;
    //        double hprob = 0;
    //        bool hswap = false;
    //        functionReturnValue = hypergeometric(type2s, type1s_reqd, POP_SIZE - tot_type1 - type2s, tot_type1 - type1s_reqd, true, ref ha1, ref hprob, ref hswap);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double crit_neghypergeometric(double type1s_reqd, double tot_type1, double POP_SIZE, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((type1s_reqd < 0.0 | tot_type1 < type1s_reqd | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((POP_SIZE == tot_type1)) {
    //        functionReturnValue = 0.0;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = POP_SIZE - tot_type1;
    //    } else {
    //        double ha1 = 0;
    //        double hprob = 0;
    //        bool hswap = false;
    //        double i = 0;
    //        double pr = 0;
    //        functionReturnValue = critneghyperg(type1s_reqd, tot_type1, POP_SIZE, crit_prob);
    //        i = functionReturnValue;
    //        pr = hypergeometric(i, type1s_reqd, POP_SIZE - tot_type1 - i, tot_type1 - type1s_reqd, false, ref ha1, ref hprob, ref hswap);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr > crit_prob)) {
    //            i = i - 1.0;
    //            pr = hypergeometric(i, type1s_reqd, POP_SIZE - tot_type1 - i, tot_type1 - type1s_reqd, false, ref ha1, ref hprob, ref hswap);
    //            if ((pr >= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_crit_neghypergeometric(double type1s_reqd, double tot_type1, double POP_SIZE, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd);
    //    tot_type1 = AlterForIntegralChecks_Others(tot_type1);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((type1s_reqd <= 0.0 | tot_type1 < type1s_reqd | tot_type1 > POP_SIZE)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0 | POP_SIZE == tot_type1)) {
    //        functionReturnValue = POP_SIZE - tot_type1;
    //    } else {
    //        double ha1 = 0;
    //        double hprob = 0;
    //        bool hswap = false;
    //        double i = 0;
    //        double pr = 0;
    //        functionReturnValue = critcompneghyperg(type1s_reqd, tot_type1, POP_SIZE, crit_prob);
    //        i = functionReturnValue;
    //        pr = hypergeometric(i, type1s_reqd, POP_SIZE - tot_type1 - i, tot_type1 - type1s_reqd, true, ref ha1, ref hprob, ref hswap);
    //        if ((pr == crit_prob)) {
    //        } else if ((pr < crit_prob)) {
    //            i = i - 1.0;
    //            pr = hypergeometric(i, type1s_reqd, POP_SIZE - tot_type1 - i, tot_type1 - type1s_reqd, true, ref ha1, ref hprob, ref hswap);
    //            if ((pr <= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double lcb_neghypergeometric(double type2s, double type1s_reqd, double POP_SIZE, double prob)
    //{
    //    double functionReturnValue = 0;
    //    type2s = AlterForIntegralChecks_Others(type2s);
    //    type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((prob < 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((type1s_reqd <= 0.0 | type1s_reqd > POP_SIZE | type2s > POP_SIZE - type1s_reqd)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | POP_SIZE == type2s + type1s_reqd)) {
    //        functionReturnValue = type1s_reqd;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = POP_SIZE - type2s;
    //    } else if ((prob < 0.5)) {
    //        functionReturnValue = critneghyperg(type1s_reqd, type2s + type1s_reqd, POP_SIZE, prob * (1.000000000001)) + type1s_reqd;
    //    } else {
    //        functionReturnValue = critcompneghyperg(type1s_reqd, type2s + type1s_reqd, POP_SIZE, (1.0 - prob) * (1.0 - 1E-12)) + type1s_reqd;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ucb_neghypergeometric(double type2s, double type1s_reqd, double POP_SIZE, double prob)
    //{
    //    double functionReturnValue = 0;
    //    type2s = AlterForIntegralChecks_Others(type2s);
    //    type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd);
    //    POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE);
    //    if ((prob < 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((type1s_reqd <= 0.0 | type1s_reqd > POP_SIZE | type2s > POP_SIZE - type1s_reqd)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | type2s == 0.0 | POP_SIZE == type2s + type1s_reqd)) {
    //        functionReturnValue = POP_SIZE - type2s;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = type1s_reqd;
    //    } else if ((prob < 0.5)) {
    //        functionReturnValue = critcompneghyperg(type1s_reqd, type2s + type1s_reqd - 1.0, POP_SIZE, prob * (1.0 - 1E-12)) + type1s_reqd - 1.0;
    //    } else {
    //        functionReturnValue = critneghyperg(type1s_reqd, type2s + type1s_reqd - 1.0, POP_SIZE, (1.0 - prob) * (1.000000000001)) + type1s_reqd - 1.0;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_exponential(double x, double Lambda)
    //{
    //    double functionReturnValue = 0;
    //    if ((Lambda <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = Math.Exp(-Lambda * x + Math.Log(Lambda));
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_exponential(double x, double Lambda)
    //{
    //    double functionReturnValue = 0;
    //    if ((Lambda <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = -expm1(-Lambda * x);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_exponential(double x, double Lambda)
    //{
    //    double functionReturnValue = 0;
    //    if ((Lambda <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        functionReturnValue = Math.Exp(-Lambda * x);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_exponential(double prob, double Lambda)
    //{
    //    double functionReturnValue = 0;
    //    if ((Lambda <= 0.0 | prob < 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = -log0(-prob) / Lambda;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_exponential(double prob, double Lambda)
    //{
    //    double functionReturnValue = 0;
    //    if ((Lambda <= 0.0 | prob <= 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = -Math.Log(prob) / Lambda;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_normal(double x)
    //{
    //    double functionReturnValue = 0;
    //    if ((Math.Abs(x) < 40.0)) {
    //        functionReturnValue = Math.Exp(-x * x * 0.5 - lstpi);
    //    } else {
    //        functionReturnValue = 0.0;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_normal(double x)
    //{
    //    double functionReturnValue = 0;
    //    functionReturnValue = cnormal(x);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    protected internal static double inv_normal(double prob)
    {
        double functionReturnValue = 0;
        if ((prob <= 0.0 | prob >= 1.0))
        {
            functionReturnValue = ErrorValue;
        }
        else
        {
            functionReturnValue = invcnormal(prob);
        }
        functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
        return functionReturnValue;
    }

    //protected internal static double pdf_chi_sq(double x, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    functionReturnValue = pdf_gamma(x, df / 2.0, 2.0);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_chi_sq(double x, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((df <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = GAMMA(x / 2.0, df / 2.0);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_chi_sq(double x, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((df <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        functionReturnValue = compgamma(x / 2.0, df / 2.0);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    protected internal static double inv_chi_sq(double prob, double df)
    {
        double functionReturnValue = 0;
        df = AlterForIntegralChecks_df(df);
        if ((df <= 0.0 | prob < 0.0 | prob >= 1.0))
        {
            functionReturnValue = ErrorValue;
        }
        else if ((prob == 0.0))
        {
            functionReturnValue = 0.0;
        }
        else
        {
            functionReturnValue = 2.0 * invgamma(df / 2.0, prob);
        }
        functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
        return functionReturnValue;
    }

    //protected internal static double comp_inv_chi_sq(double prob, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((df <= 0.0 | prob <= 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = 2.0 * invcompgamma(df / 2.0, prob);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_gamma(double x, double shape_param, double scale_param)
    //{
    //    double functionReturnValue = 0;
    //    double xs = 0;
    //    if ((shape_param <= 0.0 | scale_param <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0)) {
    //        if ((shape_param < 1.0)) {
    //            functionReturnValue = ErrorValue;
    //        } else if ((shape_param == 1.0)) {
    //            functionReturnValue = 1.0 / scale_param;
    //        } else {
    //            functionReturnValue = 0.0;
    //        }
    //    } else {
    //        xs = x / scale_param;
    //        functionReturnValue = poissonTerm(shape_param, xs, xs - shape_param, Math.Log(shape_param) - Math.Log(x));
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_gamma(double x, double shape_param, double scale_param)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param <= 0.0 | scale_param <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = GAMMA(x / scale_param, shape_param);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_gamma(double x, double shape_param, double scale_param)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param <= 0.0 | scale_param <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        functionReturnValue = compgamma(x / scale_param, shape_param);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_gamma(double prob, double shape_param, double scale_param)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param <= 0.0 | scale_param <= 0.0 | prob < 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = scale_param * invgamma(shape_param, prob);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_gamma(double prob, double shape_param, double scale_param)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param <= 0.0 | scale_param <= 0.0 | prob <= 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = scale_param * invcompgamma(shape_param, prob);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //private static  double pdftdist(double x, double k)
    //{
    //    double functionReturnValue = 0;
    //    ////Probability density for a variate from t-distribution with k degress of freedom
    //    double x2 = 0;
    //    double k2 = 0;
    //    double logterm = 0;
    //    if ((k <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((k > 1E+30)) {
    //        functionReturnValue = pdf_normal(x);
    //    } else {
    //        if (Math.Abs(x) >= Min(1.0, k)) {
    //            k2 = k / x;
    //            x2 = x + k2;
    //            k2 = k2 / x2;
    //            x2 = x / x2;
    //        } else {
    //            x2 = x * x;
    //            k2 = k + x2;
    //            x2 = x2 / k2;
    //            k2 = k / k2;
    //        }
    //        if ((k2 < cSmall)) {
    //            logterm = Math.Log(k) - 2.0 * Math.Log(Math.Abs(x));
    //        } else if ((Math.Abs(x2) < 0.5)) {
    //            logterm = log0(-x2);
    //        } else {
    //            logterm = Math.Log(k2);
    //        }
    //        x2 = k * 0.5;
    //        functionReturnValue = Math.Exp(0.5 + (k + 1.0) * 0.5 * logterm + x2 * log0(-0.5 / (x2 + 1)) + logfbit(x2 - 0.5) - logfbit(x2)) * Math.Sqrt(x2 / ((1.0 + x2))) * OneOverSqrTwoPi;
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_tdist(double x, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    functionReturnValue = pdftdist(x, df);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_tdist(double x, double df)
    //{
    //    double functionReturnValue = 0;
    //    double tdistDensity = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((df <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = tdist(x, df, tdistDensity);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_tdist(double prob, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((df <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob <= 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = invtdist(prob, df);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_fdist(double x, double df1, double df2)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 & df1 > 2.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 & df1 < 2.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x == 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        double p = 0;
    //        double Q = 0;
    //        if (x > 1.0) {
    //            Q = df2 / x;
    //            p = Q + df1;
    //            Q = Q / p;
    //            p = df1 / p;
    //        } else {
    //            p = df1 * x;
    //            Q = df2 + p;
    //            p = p / Q;
    //            Q = df2 / Q;
    //        }
    //        //If p < cSmall And x <> 0# Or q < cSmall Then
    //        //   pdf_fdist = ErrorValue
    //        //   Exit Function
    //        //End If
    //        df2 = df2 / 2.0;
    //        df1 = df1 / 2.0;
    //        if ((df1 >= 1.0)) {
    //            df1 = df1 - 1.0;
    //            functionReturnValue = binomialTerm(df1, df2, p, Q, df2 * p - df1 * Q, Math.Log((df1 + 1.0) * Q));
    //        } else {
    //            functionReturnValue = df1 * df1 * Q / (p * (df1 + df2)) * binomialTerm(df1, df2, p, Q, df2 * p - df1 * Q, 0.0);
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_fdist(double x, double df1, double df2)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double p = 0;
    //        double Q = 0;
    //        if (x > 1.0) {
    //            Q = df2 / x;
    //            p = Q + df1;
    //            Q = Q / p;
    //            p = df1 / p;
    //        } else {
    //            p = df1 * x;
    //            Q = df2 + p;
    //            p = p / Q;
    //            Q = df2 / Q;
    //        }
    //        //If p < cSmall And x <> 0# Or q < cSmall Then
    //        //   cdf_fdist = ErrorValue
    //        //   Exit Function
    //        //End If
    //        df2 = df2 / 2.0;
    //        df1 = df1 / 2.0;
    //        if ((p < 0.5)) {
    //            functionReturnValue = BETA(p, df1, df2);
    //        } else {
    //            functionReturnValue = compbeta(Q, df2, df1);
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_fdist(double x, double df1, double df2)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        double p = 0;
    //        double Q = 0;
    //        if (x > 1.0) {
    //            Q = df2 / x;
    //            p = Q + df1;
    //            Q = Q / p;
    //            p = df1 / p;
    //        } else {
    //            p = df1 * x;
    //            Q = df2 + p;
    //            p = p / Q;
    //            Q = df2 / Q;
    //        }
    //        //If p < cSmall And x <> 0# Or q < cSmall Then
    //        //   comp_cdf_fdist = ErrorValue
    //        //   Exit Function
    //        //End If
    //        df2 = df2 / 2.0;
    //        df1 = df1 / 2.0;
    //        if ((p < 0.5)) {
    //            functionReturnValue = compbeta(p, df1, df2);
    //        } else {
    //            functionReturnValue = BETA(Q, df2, df1);
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_fdist(double prob, double df1, double df2)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0 | prob < 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double temp = 0;
    //        double oneMinusP = 0;
    //        df1 = df1 / 2.0;
    //        df2 = df2 / 2.0;
    //        temp = invbeta(df1, df2, prob, ref oneMinusP);
    //        functionReturnValue = df2 * temp / (df1 * oneMinusP);
    //        //If oneMinusP < cSmall Then inv_fdist = ErrorValue
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_fdist(double prob, double df1, double df2)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0 | prob <= 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double temp = 0;
    //        double oneMinusP = 0;
    //        df1 = df1 / 2.0;
    //        df2 = df2 / 2.0;
    //        temp = invcompbeta(df1, df2, prob, ref oneMinusP);
    //        functionReturnValue = df2 * temp / (df1 * oneMinusP);
    //        //If oneMinusP < cSmall Then comp_inv_fdist = ErrorValue
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_BETA(double x, double shape_param1, double shape_param2)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param1 <= 0.0 | shape_param2 <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0 | x > 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 & shape_param1 < 1.0 | x == 1.0 & shape_param2 < 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x == 0.0 & shape_param1 == 1.0)) {
    //        functionReturnValue = shape_param2;
    //    } else if ((x == 1.0 & shape_param2 == 1.0)) {
    //        functionReturnValue = shape_param1;
    //    } else if (((x == 0.0) | (x == 1.0))) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double MX = 0;
    //        double mn = 0;
    //        MX = max(shape_param1, shape_param2);
    //        mn = Min(shape_param1, shape_param2);
    //        functionReturnValue = binomialTerm(shape_param1, shape_param2, x, 1.0 - x, (shape_param1 + shape_param2) * x - shape_param1, Math.Log(MX / (mn + MX)) + Math.Log(mn) - Math.Log(x * (1.0 - x)));
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_BETA(double x, double shape_param1, double shape_param2)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param1 <= 0.0 | shape_param2 <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x >= 1.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        functionReturnValue = BETA(x, shape_param1, shape_param2);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_BETA(double x, double shape_param1, double shape_param2)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param1 <= 0.0 | shape_param2 <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((x >= 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = compbeta(x, shape_param1, shape_param2);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_BETA(double prob, double shape_param1, double shape_param2)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param1 <= 0.0 | shape_param2 <= 0.0 | prob < 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double oneMinusP = 0;
    //        functionReturnValue = invbeta(shape_param1, shape_param2, prob, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_BETA(double prob, double shape_param1, double shape_param2)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param1 <= 0.0 | shape_param2 <= 0.0 | prob < 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double oneMinusP = 0;
    //        functionReturnValue = invcompbeta(shape_param1, shape_param2, prob, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //private static  double gamma_nc1(double x, double a, double nc, ref double nc_derivative)
    //{
    //    double functionReturnValue = 0;
    //    double aa = 0;
    //    double bb = 0;
    //    double nc_dtemp = 0;
    //    double N = 0;
    //    double p = 0;
    //    double W = 0;
    //    double S = 0;
    //    double ps = 0;
    //    double Result = 0;
    //    double term = 0;
    //    double ptx = 0;
    //    double ptnc = 0;
    //    N = a + Math.Sqrt(Math.Pow(a, 2) + 4.0 * nc * x);
    //    if (N > 0.0)
    //        N = Convert.ToInt32(2.0 * nc * x / N);
    //    aa = N + a;
    //    bb = N;
    //    ptnc = poissonTerm(N, nc, nc - N, 0.0);
    //    ptx = poissonTerm(aa, x, x - aa, 0.0);
    //    aa = aa + 1.0;
    //    bb = bb + 1.0;
    //    p = nc / bb;
    //    ps = p;
    //    nc_derivative = ps;
    //    S = x / aa;
    //    W = p;
    //    term = S * W;
    //    Result = term;
    //    if (ptx > 0.0) {
    //        while ((((term > 1E-15 * Result) & (p > 1E-16 * W)) | (ps > 1E-16 * nc_derivative))) {
    //            aa = aa + 1.0;
    //            bb = bb + 1.0;
    //            p = nc / bb * p;
    //            ps = p * S;
    //            nc_derivative = nc_derivative + ps;
    //            S = x / aa * S;
    //            W = W + p;
    //            term = S * W;
    //            Result = Result + term;
    //        }
    //        W = W * ptnc;
    //    } else {
    //        W = comppoisson(N, nc, nc - N);
    //    }
    //    functionReturnValue = Result * ptx * ptnc + comppoisson(a + bb, x, (x - a) - bb) * W;
    //    ps = 1.0;
    //    nc_dtemp = 0.0;
    //    aa = N + a;
    //    bb = N;
    //    p = 1.0;
    //    S = ptx;
    //    W = GAMMA(x, aa);
    //    term = p * W;
    //    Result = term;
    //    while (bb > 0.0 & ((term > 1E-15 * Result) | (ps > 1E-16 * nc_dtemp))) {
    //        S = aa / x * S;
    //        ps = p * S;
    //        nc_dtemp = nc_dtemp + ps;
    //        p = bb / nc * p;
    //        W = W + S;
    //        term = p * W;
    //        Result = Result + term;
    //        aa = aa - 1.0;
    //        bb = bb - 1.0;
    //    }
    //    if (bb == 0.0)
    //        aa = a;
    //    if (N > 0.0) {
    //        nc_dtemp = nc_derivative * ptx + nc_dtemp + p * aa / x * S;
    //    } else {
    //        nc_dtemp = poissonTerm(aa, x, x - aa, Math.Log(nc_derivative * x + aa) - Math.Log(x));
    //    }
    //    functionReturnValue = functionReturnValue + Result * ptnc + cpoisson(bb - 1.0, nc, nc - bb + 1.0) * W;
    //    if (nc_dtemp == 0.0) {
    //        nc_derivative = 0.0;
    //    } else {
    //        nc_derivative = poissonTerm(N, nc, nc - N, Math.Log(nc_dtemp));
    //    }
    //    return functionReturnValue;
    //}

    //private static  double comp_gamma_nc1(double x, double a, double nc, ref double nc_derivative)
    //{
    //    double functionReturnValue = 0;
    //    double aa = 0;
    //    double bb = 0;
    //    double nc_dtemp = 0;
    //    double N = 0;
    //    double p = 0;
    //    double W = 0;
    //    double S = 0;
    //    double ps = 0;
    //    double Result = 0;
    //    double term = 0;
    //    double ptx = 0;
    //    double ptnc = 0;
    //    N = a + Math.Sqrt(Math.Pow(a, 2) + 4.0 * nc * x);
    //    if (N > 0.0)
    //        N = Convert.ToInt32(2.0 * nc * x / N);
    //    aa = N + a;
    //    bb = N;
    //    ptnc = poissonTerm(N, nc, nc - N, 0.0);
    //    ptx = poissonTerm(aa, x, x - aa, 0.0);
    //    S = 1.0;
    //    ps = 1.0;
    //    nc_dtemp = 0.0;
    //    p = 1.0;
    //    W = p;
    //    term = 1.0;
    //    Result = 0.0;
    //    if (ptx > 0.0) {
    //        while (bb > 0.0 & (((term > 1E-15 * Result) & (p > 1E-16 * W)) | (ps > 1E-16 * nc_dtemp))) {
    //            S = aa / x * S;
    //            ps = p * S;
    //            nc_dtemp = nc_dtemp + ps;
    //            p = bb / nc * p;
    //            term = S * W;
    //            Result = Result + term;
    //            W = W + p;
    //            aa = aa - 1.0;
    //            bb = bb - 1.0;
    //        }
    //        W = W * ptnc;
    //    } else {
    //        W = cpoisson(N, nc, nc - N);
    //    }
    //    if (bb == 0.0)
    //        aa = a;
    //    if (N > 0.0) {
    //        nc_dtemp = (nc_dtemp + p * aa / x * S) * ptx;
    //    } else if (aa == 0 & x > 0) {
    //        nc_dtemp = 0.0;
    //    } else {
    //        nc_dtemp = poissonTerm(aa, x, x - aa, Math.Log(aa) - Math.Log(x));
    //    }
    //    functionReturnValue = Result * ptx * ptnc + compgamma(x, aa) * W;
    //    aa = N + a;
    //    bb = N;
    //    ps = 1.0;
    //    nc_derivative = 0.0;
    //    p = 1.0;
    //    S = ptx;
    //    W = compgamma(x, aa);
    //    term = 0.0;
    //    Result = term;
    //    do {
    //        W = W + S;
    //        aa = aa + 1.0;
    //        bb = bb + 1.0;
    //        p = nc / bb * p;
    //        ps = p * S;
    //        nc_derivative = nc_derivative + ps;
    //        S = x / aa * S;
    //        term = p * W;
    //        Result = Result + term;
    //    } while ((((term > 1E-15 * Result) & (S > 1E-16 * W)) | (ps > 1E-16 * nc_derivative)));
    //    functionReturnValue = functionReturnValue + Result * ptnc + comppoisson(bb, nc, nc - bb) * W;
    //    nc_dtemp = nc_derivative + nc_dtemp;
    //    if (nc_dtemp == 0.0) {
    //        nc_derivative = 0.0;
    //    } else {
    //        nc_derivative = poissonTerm(N, nc, nc - N, Math.Log(nc_dtemp));
    //    }
    //    return functionReturnValue;
    //}

    //private static  double inv_gamma_nc1(double prob, double a, double nc)
    //{
    //    double functionReturnValue = 0;
    //    //Uses approx in A&S 26.4.27 for to get initial estimate the modified NR to improve it.
    //    double x = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double hi = 0;
    //    double lo = 0;
    //    double nc_derivative = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = comp_inv_gamma_nc1(1.0 - prob, a, nc);
    //        return functionReturnValue;
    //    }

    //    lo = 0.0;
    //    hi = 1E+308;
    //    pr = Math.Exp(-nc);
    //    if (pr > prob) {
    //        if (2.0 * prob > pr) {
    //            x = comp_inv_gamma((pr - prob) / pr, a + cSmall, 1.0);
    //        } else {
    //            x = inv_gamma(prob / pr, a + cSmall, 1.0);
    //        }
    //        if (x < cSmall) {
    //            x = cSmall;
    //            pr = gamma_nc1(x, a, nc, ref nc_derivative);
    //            if (pr > prob) {
    //                functionReturnValue = 0.0;
    //                return functionReturnValue;
    //            }
    //        }
    //    } else {
    //        x = inv_gamma(prob, (a + nc) / (1.0 + nc / (a + nc)), 1.0);
    //        x = x * (1.0 + nc / (a + nc));
    //    }
    //    dif = x;
    //    do {
    //        pr = gamma_nc1(x, a, nc, ref nc_derivative);
    //        if (pr < 3E-308 & nc_derivative == 0.0) {
    //            lo = x;
    //            dif = dif / 2.0;
    //            x = x - dif;
    //        } else if (nc_derivative == 0.0) {
    //            hi = x;
    //            dif = dif / 2.0;
    //            x = x - dif;
    //        } else {
    //            if (pr < prob) {
    //                lo = x;
    //            } else {
    //                hi = x;
    //            }
    //            dif = -(pr / nc_derivative) * logdif(pr, prob);
    //            if (x + dif < lo) {
    //                dif = (lo - x) / 2.0;
    //            } else if (x + dif > hi) {
    //                dif = (hi - x) / 2.0;
    //            }
    //            x = x + dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(x) * 1E-10)));
    //    functionReturnValue = x;
    //    return functionReturnValue;
    //}

    //private static  double comp_inv_gamma_nc1(double prob, double a, double nc)
    //{
    //    double functionReturnValue = 0;
    //    //Uses approx in A&S 26.4.27 for to get initial estimate the modified NR to improve it.
    //    double x = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double hi = 0;
    //    double lo = 0;
    //    double nc_derivative = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = inv_gamma_nc1(1.0 - prob, a, nc);
    //        return functionReturnValue;
    //    }

    //    lo = 0.0;
    //    hi = 1E+308;
    //    pr = Math.Exp(-nc);
    //    if (pr > prob) {
    //        x = comp_inv_gamma(prob / pr, a + cSmall, 1.0);
    //        // Is this as small as x could be?
    //    } else {
    //        x = comp_inv_gamma(prob, (a + nc) / (1.0 + nc / (a + nc)), 1.0);
    //        x = x * (1.0 + nc / (a + nc));
    //    }
    //    if (x < cSmall)
    //        x = cSmall;
    //    dif = x;
    //    do {
    //        pr = comp_gamma_nc1(x, a, nc, ref nc_derivative);
    //        if (pr < 3E-308 & nc_derivative == 0.0) {
    //            hi = x;
    //            dif = dif / 2.0;
    //            x = x - dif;
    //        } else if (nc_derivative == 0.0) {
    //            lo = x;
    //            dif = dif / 2.0;
    //            x = x - dif;
    //        } else {
    //            if (pr < prob) {
    //                hi = x;
    //            } else {
    //                lo = x;
    //            }
    //            dif = (pr / nc_derivative) * logdif(pr, prob);
    //            if (x + dif < lo) {
    //                dif = (lo - x) / 2.0;
    //            } else if (x + dif > hi) {
    //                dif = (hi - x) / 2.0;
    //            }
    //            x = x + dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(x) * 1E-10)));
    //    functionReturnValue = x;
    //    return functionReturnValue;
    //}

    //private static  double ncp_gamma_nc1(double prob, double x, double a)
    //{
    //    double functionReturnValue = 0;
    //    //Uses Normal approx for difference of 2 poisson distributed variables  to get initial estimate the modified NR to improve it.
    //    double ncp = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double deriv = 0;
    //    double B = 0;
    //    double sqarg = 0;
    //    bool checked_nc_limit = false;
    //    bool checked_0_limit = false;
    //    double hi = 0;
    //    double lo = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = comp_ncp_gamma_nc1(1.0 - prob, x, a);
    //        return functionReturnValue;
    //    }

    //    lo = 0.0;
    //    hi = nc_limit;
    //    checked_0_limit = false;
    //    checked_nc_limit = false;
    //    temp = Math.Pow(inv_normal(prob), 2);
    //    B = 2.0 * (x - a) + temp;
    //    sqarg = Math.Pow(B, 2) - 4 * (Math.Pow((x - a), 2) - temp * x);
    //    if (sqarg < 0) {
    //        ncp = B / 2;
    //    } else {
    //        ncp = (B + Math.Sqrt(sqarg)) / 2;
    //    }
    //    ncp = max(0.0, Min(ncp, nc_limit));
    //    if (ncp == 0.0) {
    //        pr = cdf_gamma_nc(x, a, 0.0);
    //        if (pr < prob) {
    //            if ((inv_gamma(prob, a, 1) <= x)) {
    //                functionReturnValue = 0.0;
    //            } else {
    //                functionReturnValue = ErrorValue;
    //            }
    //            return functionReturnValue;
    //        } else {
    //            checked_0_limit = true;
    //        }
    //    } else if (ncp == nc_limit) {
    //        pr = cdf_gamma_nc(x, a, ncp);
    //        if (pr > prob) {
    //            functionReturnValue = ErrorValue;
    //            return functionReturnValue;
    //        } else {
    //            checked_nc_limit = true;
    //        }
    //    }
    //    dif = ncp;
    //    do {
    //        pr = cdf_gamma_nc(x, a, ncp);
    //        //Debug.Print ncp, pr, prob
    //        deriv = pdf_gamma_nc(x, a + 1.0, ncp);
    //        if (pr < 3E-308 & deriv == 0.0) {
    //            hi = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else if (deriv == 0.0) {
    //            lo = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else {
    //            if (pr < prob) {
    //                hi = ncp;
    //            } else {
    //                lo = ncp;
    //            }
    //            dif = (pr / deriv) * logdif(pr, prob);
    //            if (ncp + dif < lo) {
    //                dif = (lo - ncp) / 2.0;
    //                if (!checked_0_limit & (lo == 0.0)) {
    //                    temp = cdf_gamma_nc(x, a, lo);
    //                    if (temp < prob) {
    //                        if ((inv_gamma(prob, a, 1) <= x)) {
    //                            functionReturnValue = 0.0;
    //                        } else {
    //                            functionReturnValue = ErrorValue;
    //                        }
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_0_limit = true;
    //                    }
    //                }
    //            } else if (ncp + dif > hi) {
    //                dif = (hi - ncp) / 2.0;
    //                if (!checked_nc_limit & (hi == nc_limit)) {
    //                    pr = cdf_gamma_nc(x, a, hi);
    //                    if (pr > prob) {
    //                        functionReturnValue = ErrorValue;
    //                        return functionReturnValue;
    //                    } else {
    //                        ncp = hi;
    //                        deriv = pdf_gamma_nc(x, a + 1.0, ncp);
    //                        dif = (pr / deriv) * logdif(pr, prob);
    //                        if (ncp + dif < lo) {
    //                            dif = (lo - ncp) / 2.0;
    //                        }
    //                        checked_nc_limit = true;
    //                    }
    //                }
    //            }
    //            ncp = ncp + dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(ncp) * 1E-10)));
    //    functionReturnValue = ncp;
    //    return functionReturnValue;
    //    //Debug.Print "ncp_gamma_nc1", ncp_gamma_nc1
    //}

    //private static  double comp_ncp_gamma_nc1(double prob, double x, double a)
    //{
    //    double functionReturnValue = 0;
    //    //Uses Normal approx for difference of 2 poisson distributed variables  to get initial estimate the modified NR to improve it.
    //    double ncp = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double deriv = 0;
    //    double B = 0;
    //    double sqarg = 0;
    //    bool checked_nc_limit = false;
    //    bool checked_0_limit = false;
    //    double hi = 0;
    //    double lo = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = ncp_gamma_nc1(1.0 - prob, x, a);
    //        return functionReturnValue;
    //    }

    //    lo = 0.0;
    //    hi = nc_limit;
    //    checked_0_limit = false;
    //    checked_nc_limit = false;
    //    temp = Math.Pow(inv_normal(prob), 2);
    //    B = 2.0 * (x - a) + temp;
    //    sqarg = Math.Pow(B, 2) - 4 * (Math.Pow((x - a), 2) - temp * x);
    //    if (sqarg < 0) {
    //        ncp = B / 2;
    //    } else {
    //        ncp = (B - Math.Sqrt(sqarg)) / 2;
    //    }
    //    ncp = max(0.0, ncp);
    //    if (ncp <= 1.0) {
    //        pr = comp_cdf_gamma_nc(x, a, 0.0);
    //        if (pr > prob) {
    //            if ((comp_inv_gamma(prob, a, 1) <= x)) {
    //                functionReturnValue = 0.0;
    //            } else {
    //                functionReturnValue = ErrorValue;
    //            }
    //            return functionReturnValue;
    //        } else {
    //            checked_0_limit = true;
    //        }
    //        deriv = pdf_gamma_nc(x, a + 1.0, ncp);
    //        if (deriv == 0.0) {
    //            ncp = nc_limit;
    //        } else if (a < 1) {
    //            ncp = (prob - pr) / deriv;
    //            if (ncp >= nc_limit) {
    //                ncp = -(pr / deriv) * logdif(pr, prob);
    //            }
    //        } else {
    //            ncp = -(pr / deriv) * logdif(pr, prob);
    //        }
    //    }
    //    ncp = Min(ncp, nc_limit);
    //    if (ncp == nc_limit) {
    //        pr = comp_cdf_gamma_nc(x, a, ncp);
    //        if (pr < prob) {
    //            functionReturnValue = ErrorValue;
    //            return functionReturnValue;
    //        } else {
    //            deriv = pdf_gamma_nc(x, a + 1.0, ncp);
    //            dif = -(pr / deriv) * logdif(pr, prob);
    //            if (ncp + dif < lo) {
    //                dif = (lo - ncp) / 2.0;
    //            }
    //            checked_nc_limit = true;
    //        }
    //    }
    //    dif = ncp;
    //    do {
    //        pr = comp_cdf_gamma_nc(x, a, ncp);
    //        //Debug.Print ncp, pr, prob
    //        deriv = pdf_gamma_nc(x, a + 1.0, ncp);
    //        if (pr < 3E-308 & deriv == 0.0) {
    //            lo = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp + dif;
    //        } else if (deriv == 0.0) {
    //            hi = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else {
    //            if (pr < prob) {
    //                lo = ncp;
    //            } else {
    //                hi = ncp;
    //            }
    //            dif = -(pr / deriv) * logdif(pr, prob);
    //            if (ncp + dif < lo) {
    //                dif = (lo - ncp) / 2.0;
    //                if (!checked_0_limit & (lo == 0.0)) {
    //                    temp = comp_cdf_gamma_nc(x, a, lo);
    //                    if (temp > prob) {
    //                        if ((comp_inv_gamma(prob, a, 1) <= x)) {
    //                            functionReturnValue = 0.0;
    //                        } else {
    //                            functionReturnValue = ErrorValue;
    //                        }
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_0_limit = true;
    //                    }
    //                }
    //            } else if (ncp + dif > hi) {
    //                if (!checked_nc_limit & (hi == nc_limit)) {
    //                    ncp = hi;
    //                    pr = comp_cdf_gamma_nc(x, a, ncp);
    //                    if (pr < prob) {
    //                        functionReturnValue = ErrorValue;
    //                        return functionReturnValue;
    //                    } else {
    //                        deriv = pdf_gamma_nc(x, a + 1.0, ncp);
    //                        dif = -(pr / deriv) * logdif(pr, prob);
    //                        if (ncp + dif < lo) {
    //                            dif = (lo - ncp) / 2.0;
    //                        }
    //                        checked_nc_limit = true;
    //                    }
    //                } else {
    //                    dif = (hi - ncp) / 2.0;
    //                }
    //            }
    //            ncp = ncp + dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(ncp) * 1E-10)));
    //    functionReturnValue = ncp;
    //    return functionReturnValue;
    //    //Debug.Print "comp_ncp_gamma_nc1", comp_ncp_gamma_nc1
    //}

    //protected internal static double pdf_gamma_nc(double x, double shape_param, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculate pdf of noncentral gamma
    //    double nc_derivative = 0;
    //    if ((shape_param < 0.0) | (nc_param < 0.0) | (nc_param > nc_limit)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((shape_param == 0.0 & nc_param == 0.0 & x > 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 | nc_param == 0.0)) {
    //        functionReturnValue = Math.Exp(-nc_param) * pdf_gamma(x, shape_param, 1.0);
    //    } else if (shape_param >= 1.0) {
    //        if (x >= nc_param) {
    //            if ((x < 1.0 | x <= shape_param + nc_param)) {
    //                functionReturnValue = gamma_nc1(x, shape_param, nc_param, ref nc_derivative);
    //            } else {
    //                functionReturnValue = comp_gamma_nc1(x, shape_param, nc_param, ref nc_derivative);
    //            }
    //            functionReturnValue = nc_derivative;
    //        } else {
    //            if ((nc_param < 1.0 | nc_param <= shape_param + x)) {
    //                functionReturnValue = gamma_nc1(nc_param, shape_param, x, ref nc_derivative);
    //            } else {
    //                functionReturnValue = comp_gamma_nc1(nc_param, shape_param, x, ref nc_derivative);
    //            }
    //            if (nc_derivative == 0.0) {
    //                functionReturnValue = 0.0;
    //            } else {
    //                functionReturnValue = Math.Exp(Math.Log(nc_derivative) + (shape_param - 1.0) * (Math.Log(x) - Math.Log(nc_param)));
    //            }
    //        }
    //    } else {
    //        if (x < nc_param) {
    //            if ((x < 1.0 | x <= shape_param + nc_param)) {
    //                functionReturnValue = gamma_nc1(x, shape_param, nc_param, ref nc_derivative);
    //            } else {
    //                functionReturnValue = comp_gamma_nc1(x, shape_param, nc_param, ref nc_derivative);
    //            }
    //            functionReturnValue = nc_derivative;
    //        } else {
    //            if ((nc_param < 1.0 | nc_param <= shape_param + x)) {
    //                functionReturnValue = gamma_nc1(nc_param, shape_param, x, ref nc_derivative);
    //            } else {
    //                functionReturnValue = comp_gamma_nc1(nc_param, shape_param, x, ref nc_derivative);
    //            }
    //            if (nc_derivative == 0.0) {
    //                functionReturnValue = 0.0;
    //            } else {
    //                functionReturnValue = Math.Exp(Math.Log(nc_derivative) + (shape_param - 1.0) * (Math.Log(x) - Math.Log(nc_param)));
    //            }
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_gamma_nc(double x, double shape_param, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculate cdf of noncentral gamma
    //    double nc_derivative = 0;
    //    if ((shape_param < 0.0) | (nc_param < 0.0) | (nc_param > nc_limit)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 & shape_param == 0.0)) {
    //        functionReturnValue = Math.Exp(-nc_param);
    //    // limit as shape_param+nc_param->0 is degenerate point mass at zero
    //    } else if ((shape_param + nc_param == 0.0)) {
    //        functionReturnValue = 1.0;
    //        // if fix central gamma, then works for degenerate poisson
    //    } else if ((x == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((nc_param == 0.0)) {
    //        functionReturnValue = GAMMA(x, shape_param);
    //    //ElseIf (shape_param = 0#) Then              ' extends Ruben (1974) and Cohen (1988) recurrence
    //    //   cdf_gamma_nc = ((x + shape_param + 2#) * gamma_nc1(x, shape_param + 2#, nc_param) + (nc_param - shape_param - 2#) * gamma_nc1(x, shape_param + 4#, nc_param) - nc_param * gamma_nc1(x, shape_param + 6#, nc_param)) / x
    //    } else if ((x < 1.0 | x <= shape_param + nc_param)) {
    //        functionReturnValue = gamma_nc1(x, shape_param, nc_param, ref nc_derivative);
    //    } else {
    //        functionReturnValue = 1.0 - comp_gamma_nc1(x, shape_param, nc_param, ref nc_derivative);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_gamma_nc(double x, double shape_param, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculate 1-cdf of noncentral gamma
    //    double nc_derivative = 0;
    //    if ((shape_param < 0.0) | (nc_param < 0.0) | (nc_param > nc_limit)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((x == 0.0 & shape_param == 0.0)) {
    //        functionReturnValue = -expm1(-nc_param);
    //    // limit as shape_param+nc_param->0 is degenerate point mass at zero
    //    } else if ((shape_param + nc_param == 0.0)) {
    //        functionReturnValue = 0.0;
    //        // if fix central gamma, then works for degenerate poisson
    //    } else if ((x == 0.0)) {
    //        functionReturnValue = 1;
    //    } else if ((nc_param == 0.0)) {
    //        functionReturnValue = compgamma(x, shape_param);
    //    //ElseIf (shape_param = 0#) Then              ' extends Ruben (1974) and Cohen (1988) recurrence
    //    //   comp_cdf_gamma_nc = ((x + shape_param + 2#) * comp_gamma_nc1(x, shape_param + 2#, nc_param) + (nc_param - shape_param - 2#) * comp_gamma_nc1(x, shape_param + 4#, nc_param) - nc_param * comp_gamma_nc1(x, shape_param + 6#, nc_param)) / x
    //    } else if ((x < 1.0 | x >= shape_param + nc_param)) {
    //        functionReturnValue = comp_gamma_nc1(x, shape_param, nc_param, ref nc_derivative);
    //    } else {
    //        functionReturnValue = 1.0 - gamma_nc1(x, shape_param, nc_param, ref nc_derivative);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_gamma_nc(double prob, double shape_param, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param < 0.0 | nc_param < 0.0 | nc_param > nc_limit | prob < 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | shape_param == 0.0 & prob <= Math.Exp(-nc_param))) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = inv_gamma_nc1(prob, shape_param, nc_param);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_gamma_nc(double prob, double shape_param, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param < 0.0 | nc_param < 0.0 | nc_param > nc_limit | prob <= 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 1.0 | shape_param == 0.0 & prob >= -expm1(-nc_param))) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = comp_inv_gamma_nc1(prob, shape_param, nc_param);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ncp_gamma_nc(double prob, double x, double shape_param)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param < 0.0 | x < 0.0 | prob <= 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x == 0.0 & shape_param == 0.0)) {
    //        functionReturnValue = -Math.Log(prob);
    //    } else if ((shape_param == 0.0 & prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 | prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = ncp_gamma_nc1(prob, x, shape_param);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_ncp_gamma_nc(double prob, double x, double shape_param)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param < 0.0 | x < 0.0 | prob < 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x == 0.0 & shape_param == 0.0)) {
    //        functionReturnValue = -log0(-prob);
    //    } else if ((shape_param == 0.0 & prob == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 | prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = comp_ncp_gamma_nc1(prob, x, shape_param);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_Chi2_nc(double x, double df, double nc)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculate pdf of noncentral chi-square
    //    df = AlterForIntegralChecks_df(df);
    //    functionReturnValue = 0.5 * pdf_gamma_nc(x / 2.0, df / 2.0, nc / 2.0);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_Chi2_nc(double x, double df, double nc)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculate cdf of noncentral chi-square
    //    ////   parametrized per Johnson & Kotz, SAS, etc. so that cdf_Chi2_nc(x,df,nc) = cdf_gamma_nc(x/2,df/2,nc/2)
    //    ////   If Xi ~ N(Di,1) independent, then sum(Xi,i=1..n) ~ Chi2_nc(n,nc) with nc=sum(Di,i=1..n)
    //    ////   Note that Knusel, Graybill, etc. use a different noncentrality parameter lambda=nc/2
    //    df = AlterForIntegralChecks_df(df);
    //    functionReturnValue = cdf_gamma_nc(x / 2.0, df / 2.0, nc / 2.0);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_Chi2_nc(double x, double df, double nc)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculate 1-cdf of noncentral chi-square
    //    ////   parametrized per Johnson & Kotz, SAS, etc. so that cdf_Chi2_nc(x,df,nc) = cdf_gamma_nc(x/2,df/2,nc/2)
    //    ////   If Xi ~ N(Di,1) independent, then sum(Xi,i=1..n) ~ Chi2_nc(n,nc) with nc=sum(Di,i=1..n)
    //    ////   Note that Knusel, Graybill, etc. use a different noncentrality parameter lambda=nc/2
    //    df = AlterForIntegralChecks_df(df);
    //    functionReturnValue = comp_cdf_gamma_nc(x / 2.0, df / 2.0, nc / 2.0);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_Chi2_nc(double prob, double df, double nc)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    functionReturnValue = 2.0 * inv_gamma_nc(prob, df / 2.0, nc / 2.0);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_Chi2_nc(double prob, double df, double nc)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    functionReturnValue = 2.0 * comp_inv_gamma_nc(prob, df / 2.0, nc / 2.0);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ncp_Chi2_nc(double prob, double x, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    functionReturnValue = 2.0 * ncp_gamma_nc(prob, x / 2.0, df / 2.0);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_ncp_Chi2_nc(double prob, double x, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    functionReturnValue = 2.0 * comp_ncp_gamma_nc(prob, x / 2.0, df / 2.0);
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //private static  double BETA_nc1(double x, double y, double a, double B, double nc, ref double nc_derivative)
    //{
    //    double functionReturnValue = 0;
    //    //y is 1-x but held accurately to avoid possible cancellation errors
    //    double aa = 0;
    //    double bb = 0;
    //    double nc_dtemp = 0;
    //    double N = 0;
    //    double p = 0;
    //    double W = 0;
    //    double S = 0;
    //    double ps = 0;
    //    double Result = 0;
    //    double term = 0;
    //    double ptx = 0;
    //    double ptnc = 0;
    //    aa = a - nc * x * (a + B);
    //    bb = (x * nc - 1.0) - a;
    //    if ((bb < 0.0)) {
    //        N = bb - Math.Sqrt(Math.Pow(bb, 2) - 4.0 * aa);
    //        N = Convert.ToInt32(2.0 * aa / N);
    //    } else {
    //        N = Convert.ToInt32((bb + Math.Sqrt(Math.Pow(bb, 2) - 4.0 * aa)) / 2.0);
    //    }
    //    if (N < 0.0) {
    //        N = 0.0;
    //    }
    //    aa = N + a;
    //    bb = N;
    //    ptnc = poissonTerm(N, nc, nc - N, 0.0);
    //    ptx = B * binomialTerm(aa, B, x, y, B * x - aa * y, 0.0);
    //    //  (aa + b)*(I(x, aa, b) - I(x, aa + 1, b))
    //    aa = aa + 1.0;
    //    bb = bb + 1.0;
    //    p = nc / bb;
    //    ps = p;
    //    nc_derivative = ps;
    //    S = x / aa;
    //    // (I(x, aa, b) - I(x, aa + 1, b)) / ptx
    //    W = p;
    //    term = S * W;
    //    Result = term;
    //    if (ptx > 0) {
    //        while ((((term > 1E-15 * Result) & (p > 1E-16 * W)) | (ps > 1E-16 * nc_derivative))) {
    //            S = (aa + B) * S;
    //            aa = aa + 1.0;
    //            bb = bb + 1.0;
    //            p = nc / bb * p;
    //            ps = p * S;
    //            nc_derivative = nc_derivative + ps;
    //            S = x / aa * S;
    //            // (I(x, aa, b) - I(x, aa + 1, b)) / ptx
    //            W = W + p;
    //            term = S * W;
    //            Result = Result + term;
    //        }
    //        W = W * ptnc;
    //    } else {
    //        W = comppoisson(N, nc, nc - N);
    //    }
    //    if (x > y) {
    //        S = compbeta(y, B, a + (bb + 1.0));
    //    } else {
    //        S = BETA(x, a + (bb + 1.0), B);
    //    }
    //    functionReturnValue = Result * ptx * ptnc + S * W;
    //    ps = 1.0;
    //    nc_dtemp = 0.0;
    //    aa = N + a;
    //    bb = N;
    //    p = 1.0;
    //    S = ptx / (aa + B);
    //    // I(x, aa, b) - I(x, aa + 1, b)
    //    if (x > y) {
    //        W = compbeta(y, B, aa);
    //        // I(x, aa, b)
    //    } else {
    //        W = BETA(x, aa, B);
    //        // I(x, aa, b)
    //    }
    //    term = p * W;
    //    Result = term;
    //    while (bb > 0.0 & (((term > 1E-15 * Result) & (S > 1E-16 * W)) | (ps > 1E-16 * nc_dtemp))) {
    //        S = aa / x * S;
    //        ps = p * S;
    //        nc_dtemp = nc_dtemp + ps;
    //        p = bb / nc * p;
    //        aa = aa - 1.0;
    //        bb = bb - 1.0;
    //        if (bb == 0.0)
    //            aa = a;
    //        S = S / (aa + B);
    //        // I(x, aa, b) - I(x, aa + 1, b)
    //        W = W + S;
    //        // I(x, aa, b)
    //        term = p * W;
    //        Result = Result + term;
    //    }
    //    if (N > 0.0) {
    //        nc_dtemp = nc_derivative * ptx + nc_dtemp + p * aa / x * S;
    //    } else if (B == 0.0) {
    //        nc_dtemp = 0.0;
    //    } else {
    //        nc_dtemp = binomialTerm(aa, B, x, y, B * x - aa * y, Math.Log(B) + Math.Log((nc_derivative + aa / (x * (aa + B)))));
    //    }
    //    nc_dtemp = nc_dtemp / y;
    //    functionReturnValue = functionReturnValue + Result * ptnc + cpoisson(bb - 1.0, nc, nc - bb + 1.0) * W;
    //    if (nc_dtemp == 0.0) {
    //        nc_derivative = 0.0;
    //    } else {
    //        nc_derivative = poissonTerm(N, nc, nc - N, Math.Log(nc_dtemp));
    //    }
    //    return functionReturnValue;
    //}

    //private static  double comp_BETA_nc1(double x, double y, double a, double B, double nc, ref double nc_derivative)
    //{
    //    double functionReturnValue = 0;
    //    //y is 1-x but held accurately to avoid possible cancellation errors
    //    double aa = 0;
    //    double bb = 0;
    //    double nc_dtemp = 0;
    //    double N = 0;
    //    double p = 0;
    //    double W = 0;
    //    double S = 0;
    //    double ps = 0;
    //    double Result = 0;
    //    double term = 0;
    //    double ptx = 0;
    //    double ptnc = 0;
    //    aa = a - nc * x * (a + B);
    //    bb = (x * nc - 1.0) - a;
    //    if ((bb < 0.0)) {
    //        N = bb - Math.Sqrt(Math.Pow(bb, 2) - 4.0 * aa);
    //        N = Convert.ToInt32(2.0 * aa / N);
    //    } else {
    //        N = Convert.ToInt32((bb + Math.Sqrt(Math.Pow(bb, 2) - 4.0 * aa)) / 2);
    //    }
    //    if (N < 0.0) {
    //        N = 0.0;
    //    }
    //    aa = N + a;
    //    bb = N;
    //    ptnc = poissonTerm(N, nc, nc - N, 0.0);
    //    ptx = B / (aa + B) * binomialTerm(aa, B, x, y, B * x - aa * y, 0.0);
    //    //(1 - I(x, aa + 1, b)) - (1 - I(x, aa, b))
    //    ps = 1.0;
    //    nc_dtemp = 0.0;
    //    p = 1.0;
    //    S = 1.0;
    //    W = p;
    //    term = 1.0;
    //    Result = 0.0;
    //    if (ptx > 0) {
    //        while (bb > 0.0 & (((term > 1E-15 * Result) & (p > 1E-16 * W)) | (ps > 1E-16 * nc_dtemp))) {
    //            S = aa / x * S;
    //            ps = p * S;
    //            nc_dtemp = nc_dtemp + ps;
    //            p = bb / nc * p;
    //            aa = aa - 1.0;
    //            bb = bb - 1.0;
    //            if (bb == 0.0)
    //                aa = a;
    //            S = S / (aa + B);
    //            // (1 - I(x, aa + 1, b)) - (1 - I(x, aa + 1, b))
    //            term = S * W;
    //            Result = Result + term;
    //            W = W + p;
    //        }
    //        W = W * ptnc;
    //    } else {
    //        W = cpoisson(N, nc, nc - N);
    //    }
    //    if (N > 0.0) {
    //        nc_dtemp = (nc_dtemp + p * aa / x * S) * ptx;
    //    } else if (a == 0.0 | B == 0.0) {
    //        nc_dtemp = 0.0;
    //    } else {
    //        nc_dtemp = binomialTerm(aa, B, x, y, B * x - aa * y, Math.Log(B) + Math.Log(aa / (x * (aa + B))));
    //    }
    //    if (x > y) {
    //        S = BETA(y, B, aa);
    //    } else {
    //        S = compbeta(x, aa, B);
    //    }
    //    functionReturnValue = Result * ptx * ptnc + S * W;
    //    aa = N + a;
    //    bb = N;
    //    p = 1.0;
    //    nc_derivative = 0.0;
    //    S = ptx;
    //    if (x > y) {
    //        W = BETA(y, B, aa);
    //        //  1 - I(x, aa, b)
    //    } else {
    //        W = compbeta(x, aa, B);
    //        // 1 - I(x, aa, b)
    //    }
    //    term = 0.0;
    //    Result = term;
    //    do {
    //        W = W + S;
    //        // 1 - I(x, aa, b)
    //        S = (aa + B) * S;
    //        aa = aa + 1.0;
    //        bb = bb + 1.0;
    //        p = nc / bb * p;
    //        ps = p * S;
    //        nc_derivative = nc_derivative + ps;
    //        S = x / aa * S;
    //        // (1 - I(x, aa + 1, b)) - (1 - I(x, aa, b))
    //        term = p * W;
    //        Result = Result + term;
    //    } while ((((term > 1E-15 * Result) & (S > 1E-16 * W)) | (ps > 1E-16 * nc_derivative)));
    //    nc_dtemp = (nc_derivative + nc_dtemp) / y;
    //    functionReturnValue = functionReturnValue + Result * ptnc + comppoisson(bb, nc, nc - bb) * W;
    //    if (nc_dtemp == 0.0) {
    //        nc_derivative = 0.0;
    //    } else {
    //        nc_derivative = poissonTerm(N, nc, nc - N, Math.Log(nc_dtemp));
    //    }
    //    return functionReturnValue;
    //}

    //private static  double inv_BETA_nc1(double prob, double a, double B, double nc, ref double oneMinusP)
    //{
    //    double functionReturnValue = 0;
    //    //Uses approx in A&S 26.6.26 for to get initial estimate the modified NR to improve it.
    //    double x = 0;
    //    double y = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double hip = 0;
    //    double lop = 0;
    //    double hix = 0;
    //    double lox = 0;
    //    double nc_derivative = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = comp_inv_BETA_nc1(1.0 - prob, a, B, nc, ref oneMinusP);
    //        return functionReturnValue;
    //    }

    //    lop = 0.0;
    //    hip = 1.0;
    //    lox = 0.0;
    //    hix = 1.0;
    //    pr = Math.Exp(-nc);
    //    if (pr > prob) {
    //        if (2.0 * prob > pr) {
    //            x = invcompbeta(a + cSmall, B, (pr - prob) / pr, ref oneMinusP);
    //        } else {
    //            x = invbeta(a + cSmall, B, prob / pr, ref oneMinusP);
    //        }
    //        if (x == 0.0) {
    //            functionReturnValue = 0.0;
    //            return functionReturnValue;
    //        } else {
    //            temp = oneMinusP;
    //            y = invbeta(Math.Pow((a + nc), 2) / (a + 2.0 * nc), B, prob, ref oneMinusP);
    //            oneMinusP = (a + nc) * oneMinusP / (a + nc * (1.0 + y));
    //            if (temp > oneMinusP) {
    //                oneMinusP = temp;
    //            } else {
    //                x = (a + 2.0 * nc) * y / (a + nc * (1.0 + y));
    //            }
    //        }
    //    } else {
    //        y = invbeta(Math.Pow((a + nc), 2) / (a + 2.0 * nc), B, prob, ref oneMinusP);
    //        x = (a + 2.0 * nc) * y / (a + nc * (1.0 + y));
    //        oneMinusP = (a + nc) * oneMinusP / (a + nc * (1.0 + y));
    //        if (oneMinusP < cSmall) {
    //            oneMinusP = cSmall;
    //            pr = BETA_nc1(x, oneMinusP, a, B, nc, ref nc_derivative);
    //            if (pr < prob) {
    //                functionReturnValue = 1.0;
    //                oneMinusP = 0.0;
    //                return functionReturnValue;
    //            }
    //        }
    //    }
    //    do {
    //        pr = BETA_nc1(x, oneMinusP, a, B, nc, ref nc_derivative);
    //        if (pr < 3E-308 & nc_derivative == 0.0) {
    //            hip = oneMinusP;
    //            lox = x;
    //            dif = dif / 2.0;
    //            x = x - dif;
    //            oneMinusP = oneMinusP + dif;
    //        } else if (nc_derivative == 0.0) {
    //            lop = oneMinusP;
    //            hix = x;
    //            dif = dif / 2.0;
    //            x = x - dif;
    //            oneMinusP = oneMinusP + dif;
    //        } else {
    //            if (pr < prob) {
    //                hip = oneMinusP;
    //                lox = x;
    //            } else {
    //                lop = oneMinusP;
    //                hix = x;
    //            }
    //            dif = -(pr / nc_derivative) * logdif(pr, prob);
    //            if (x > oneMinusP) {
    //                if (oneMinusP - dif < lop) {
    //                    dif = (oneMinusP - lop) * 0.9;
    //                } else if (oneMinusP - dif > hip) {
    //                    dif = (oneMinusP - hip) * 0.9;
    //                }
    //            } else if (x + dif < lox) {
    //                dif = (lox - x) * 0.9;
    //            } else if (x + dif > hix) {
    //                dif = (hix - x) * 0.9;
    //            }
    //            x = x + dif;
    //            oneMinusP = oneMinusP - dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(Min(x, oneMinusP)) * 1E-10)));
    //    functionReturnValue = x;
    //    return functionReturnValue;
    //}

    //private static  double comp_inv_BETA_nc1(double prob, double a, double B, double nc, ref double oneMinusP)
    //{
    //    double functionReturnValue = 0;
    //    //Uses approx in A&S 26.6.26 for to get initial estimate the modified NR to improve it.
    //    double x = 0;
    //    double y = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double hip = 0;
    //    double lop = 0;
    //    double hix = 0;
    //    double lox = 0;
    //    double nc_derivative = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = inv_BETA_nc1(1.0 - prob, a, B, nc, ref oneMinusP);
    //        return functionReturnValue;
    //    }

    //    lop = 0.0;
    //    hip = 1.0;
    //    lox = 0.0;
    //    hix = 1.0;
    //    pr = Math.Exp(-nc);
    //    if (pr > prob) {
    //        if (2.0 * prob > pr) {
    //            x = invbeta(a + cSmall, B, (pr - prob) / pr, ref oneMinusP);
    //        } else {
    //            x = invcompbeta(a + cSmall, B, prob / pr, ref oneMinusP);
    //        }
    //        if (oneMinusP < cSmall) {
    //            oneMinusP = cSmall;
    //            pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, ref nc_derivative);
    //            if (pr > prob) {
    //                functionReturnValue = 1.0;
    //                oneMinusP = 0.0;
    //                return functionReturnValue;
    //            }
    //        } else {
    //            temp = oneMinusP;
    //            y = invcompbeta(Math.Pow((a + nc), 2) / (a + 2.0 * nc), B, prob, ref oneMinusP);
    //            oneMinusP = (a + nc) * oneMinusP / (a + nc * (1.0 + y));
    //            if (temp < oneMinusP) {
    //                oneMinusP = temp;
    //            } else {
    //                x = (a + 2.0 * nc) * y / (a + nc * (1.0 + y));
    //            }
    //            if (oneMinusP < cSmall) {
    //                oneMinusP = cSmall;
    //                pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, ref nc_derivative);
    //                if (pr > prob) {
    //                    functionReturnValue = 1.0;
    //                    oneMinusP = 0.0;
    //                    return functionReturnValue;
    //                }
    //            } else if (x < cSmall) {
    //                x = cSmall;
    //                pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, ref nc_derivative);
    //                if (pr < prob) {
    //                    functionReturnValue = 0.0;
    //                    oneMinusP = 1.0;
    //                    return functionReturnValue;
    //                }
    //            }
    //        }
    //    } else {
    //        y = invcompbeta(Math.Pow((a + nc), 2) / (a + 2.0 * nc), B, prob, ref oneMinusP);
    //        x = (a + 2.0 * nc) * y / (a + nc * (1.0 + y));
    //        oneMinusP = (a + nc) * oneMinusP / (a + nc * (1.0 + y));
    //        if (oneMinusP < cSmall) {
    //            oneMinusP = cSmall;
    //            pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, ref nc_derivative);
    //            if (pr > prob) {
    //                functionReturnValue = 1.0;
    //                oneMinusP = 0.0;
    //                return functionReturnValue;
    //            }
    //        } else if (x < cSmall) {
    //            x = cSmall;
    //            pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, ref nc_derivative);
    //            if (pr < prob) {
    //                functionReturnValue = 0.0;
    //                oneMinusP = 1.0;
    //                return functionReturnValue;
    //            }
    //        }
    //    }
    //    dif = x;
    //    do {
    //        pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, ref nc_derivative);
    //        if (pr < 3E-308 & nc_derivative == 0.0) {
    //            lop = oneMinusP;
    //            hix = x;
    //            dif = dif / 2.0;
    //            x = x - dif;
    //            oneMinusP = oneMinusP + dif;
    //        } else if (nc_derivative == 0.0) {
    //            hip = oneMinusP;
    //            lox = x;
    //            dif = dif / 2.0;
    //            x = x - dif;
    //            oneMinusP = oneMinusP + dif;
    //        } else {
    //            if (pr < prob) {
    //                lop = oneMinusP;
    //                hix = x;
    //            } else {
    //                hip = oneMinusP;
    //                lox = x;
    //            }
    //            dif = (pr / nc_derivative) * logdif(pr, prob);
    //            if (x > oneMinusP) {
    //                if (oneMinusP - dif < lop) {
    //                    dif = (oneMinusP - lop) * 0.9;
    //                } else if (oneMinusP - dif > hip) {
    //                    dif = (oneMinusP - hip) * 0.9;
    //                }
    //            } else if (x + dif < lox) {
    //                dif = (lox - x) * 0.9;
    //            } else if (x + dif > hix) {
    //                dif = (hix - x) * 0.9;
    //            }
    //            x = x + dif;
    //            oneMinusP = oneMinusP - dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(Min(x, oneMinusP)) * 1E-10)));
    //    functionReturnValue = x;
    //    return functionReturnValue;
    //}

    //private static  double invBetaLessThanX(double prob, double x, double y, double a, double B)
    //{
    //    double functionReturnValue = 0;
    //    double oneMinusP = 0;
    //    if (x >= y) {
    //        if (invcompbeta(B, a, prob, ref oneMinusP) >= y * (1.0 - 1E-15)) {
    //            functionReturnValue = 0.0;
    //        } else {
    //            functionReturnValue = ErrorValue;
    //        }
    //    } else if (invbeta(a, B, prob, ref oneMinusP) <= x * (1.0 + 1E-15)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = ErrorValue;
    //    }
    //    return functionReturnValue;
    //}

    //private static  double compInvBetaLessThanX(double prob, double x, double y, double a, double B)
    //{
    //    double functionReturnValue = 0;
    //    double oneMinusP = 0;
    //    if (x >= y) {
    //        if (invbeta(B, a, prob, ref oneMinusP) >= y * (1.0 - 1E-15)) {
    //            functionReturnValue = 0.0;
    //        } else {
    //            functionReturnValue = ErrorValue;
    //        }
    //    } else if (invcompbeta(a, B, prob, ref oneMinusP) <= x * (1.0 + 1E-15)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = ErrorValue;
    //    }
    //    return functionReturnValue;
    //}

    //private static  double ncp_BETA_nc1(double prob, double x, double y, double a, double B)
    //{
    //    double functionReturnValue = 0;
    //    //Uses Normal approx for difference of 2 a Negative Binomial and a poisson distributed variable to get initial estimate the modified NR to improve it.
    //    double ncp = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double deriv = 0;
    //    double C = 0;
    //    double D = 0;
    //    double E = 0;
    //    double sqarg = 0;
    //    bool checked_nc_limit = false;
    //    bool checked_0_limit = false;
    //    double hi = 0;
    //    double lo = 0;
    //    double nc_derivative = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = comp_ncp_BETA_nc1(1.0 - prob, x, y, a, B);
    //        return functionReturnValue;
    //    }

    //    lo = 0.0;
    //    hi = nc_limit;
    //    checked_0_limit = false;
    //    checked_nc_limit = false;
    //    temp = Math.Pow(inv_normal(prob), 2);
    //    C = B * x / y;
    //    D = temp - 2.0 * (a - C);
    //    if (D < 2 * nc_limit) {
    //        E = Math.Pow((C - a), 2) - temp * C / y;
    //        sqarg = Math.Pow(D, 2) - 4 * E;
    //        if (sqarg < 0) {
    //            ncp = D / 2;
    //        } else {
    //            ncp = (D + Math.Sqrt(sqarg)) / 2;
    //        }
    //    } else {
    //        ncp = nc_limit;
    //    }
    //    ncp = Min(max(0.0, ncp), nc_limit);
    //    if (x > y) {
    //        pr = compbeta(y * (1 + ncp / (ncp + a)) / (1 + ncp / (ncp + a) * y), B, a + Math.Pow(ncp, 2) / (2 * ncp + a));
    //    } else {
    //        pr = BETA(x / (1 + ncp / (ncp + a) * y), a + Math.Pow(ncp, 2) / (2 * ncp + a), B);
    //    }
    //    //Debug.Print "ncp_BETA_nc1 ncp1 ", ncp, pr
    //    if (ncp == 0.0) {
    //        if (pr < prob) {
    //            functionReturnValue = invBetaLessThanX(prob, x, y, a, B);
    //            return functionReturnValue;
    //        } else {
    //            checked_0_limit = true;
    //        }
    //    }
    //    temp = Min(max(0.0, invcompgamma(B * x, prob) / y - a), nc_limit);
    //    if (temp == ncp) {
    //        C = pr;
    //    } else if (x > y) {
    //        C = compbeta(y * (1 + temp / (temp + a)) / (1 + temp / (temp + a) * y), B, a + Math.Pow(temp, 2) / (2 * temp + a));
    //    } else {
    //        C = BETA(x / (1 + temp / (temp + a) * y), a + Math.Pow(temp, 2) / (2 * temp + a), B);
    //    }
    //    //Debug.Print "ncp_BETA_nc1 ncp2 ", temp, c
    //    if (temp == 0.0) {
    //        if (C < prob) {
    //            functionReturnValue = invBetaLessThanX(prob, x, y, a, B);
    //            return functionReturnValue;
    //        } else {
    //            checked_0_limit = true;
    //        }
    //    }
    //    if (pr * C == 0.0) {
    //        ncp = Min(ncp, temp);
    //        pr = max(pr, C);
    //        if (pr == 0.0) {
    //            C = compbeta(y, B, a);
    //            if (C < prob) {
    //                functionReturnValue = invBetaLessThanX(prob, x, y, a, B);
    //                return functionReturnValue;
    //            } else {
    //                checked_0_limit = true;
    //            }
    //        }
    //    } else if (Math.Abs(Math.Log(pr / prob)) > Math.Abs(Math.Log(C / prob))) {
    //        ncp = temp;
    //        pr = C;
    //    }
    //    if (ncp == 0.0) {
    //        if (B > 1 + 1E-06) {
    //            deriv = BETA_nc1(x, y, a + 1.0, B - 1.0, ncp, ref nc_derivative);
    //            deriv = nc_derivative * Math.Pow(y, 2) / (B - 1.0);
    //        } else {
    //            deriv = pr - BETA_nc1(x, y, a + 1.0, B, ncp, ref nc_derivative);
    //        }
    //        if (deriv == 0.0) {
    //            ncp = nc_limit;
    //        } else {
    //            ncp = (pr - prob) / deriv;
    //            if (ncp >= nc_limit) {
    //                ncp = (pr / deriv) * logdif(pr, prob);
    //            }
    //        }
    //    } else {
    //        if (ncp == nc_limit) {
    //            if (pr > prob) {
    //                functionReturnValue = ErrorValue;
    //                return functionReturnValue;
    //            } else {
    //                checked_nc_limit = true;
    //            }
    //        }
    //        if (pr > 0) {
    //            temp = ncp * 0.999999;
    //            //Use numerical derivative on approximation since cheap compared to evaluating non-central BETA
    //            if (x > y) {
    //                C = compbeta(y * (1.0 + temp / (temp + a)) / (1 + temp / (temp + a) * y), B, a + Math.Pow(temp, 2) / (2 * temp + a));
    //            } else {
    //                C = BETA(x / (1 + temp / (temp + a) * y), a + Math.Pow(temp, 2) / (2 * temp + a), B);
    //            }
    //            if (pr != C) {
    //                dif = (1E-06 * ncp * pr / (pr - C)) * logdif(pr, prob);
    //                if (ncp - dif < 0.0) {
    //                    ncp = ncp / 2.0;
    //                } else if (ncp - dif > nc_limit) {
    //                    ncp = (ncp + nc_limit) / 2.0;
    //                } else {
    //                    ncp = ncp - dif;
    //                }
    //            }
    //        } else {
    //            ncp = ncp / 2.0;
    //        }
    //    }
    //    dif = ncp;
    //    do {
    //        pr = BETA_nc1(x, y, a, B, ncp, ref nc_derivative);
    //        //Debug.Print ncp, pr, prob
    //        if (B > 1 + 1E-06) {
    //            deriv = BETA_nc1(x, y, a + 1.0, B - 1.0, ncp, ref nc_derivative);
    //            deriv = nc_derivative * Math.Pow(y, 2) / (B - 1.0);
    //        } else {
    //            deriv = pr - BETA_nc1(x, y, a + 1.0, B, ncp, ref nc_derivative);
    //        }
    //        if (pr < 3E-308 & deriv == 0.0) {
    //            hi = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else if (deriv == 0.0) {
    //            lo = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else {
    //            if (pr < prob) {
    //                hi = ncp;
    //            } else {
    //                lo = ncp;
    //            }
    //            dif = (pr / deriv) * logdif(pr, prob);
    //            if (ncp + dif < lo) {
    //                dif = (lo - ncp) / 2.0;
    //                if (!checked_0_limit & (lo == 0.0)) {
    //                    temp = cdf_BETA_nc(x, a, B, lo);
    //                    if (temp < prob) {
    //                        functionReturnValue = invBetaLessThanX(prob, x, y, a, B);
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_0_limit = true;
    //                    }
    //                }
    //            } else if (ncp + dif > hi) {
    //                dif = (hi - ncp) / 2.0;
    //                if (!checked_nc_limit & (hi == nc_limit)) {
    //                    temp = cdf_BETA_nc(x, a, B, hi);
    //                    if (temp > prob) {
    //                        functionReturnValue = ErrorValue;
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_nc_limit = true;
    //                    }
    //                }
    //            }
    //            ncp = ncp + dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(ncp) * 1E-10)));
    //    functionReturnValue = ncp;
    //    return functionReturnValue;
    //    //Debug.Print "ncp_BETA_nc1", ncp_BETA_nc1
    //}

    //private static  double comp_ncp_BETA_nc1(double prob, double x, double y, double a, double B)
    //{
    //    double functionReturnValue = 0;
    //    //Uses Normal approx for difference of 2 a Negative Binomial and a poisson distributed variable to get initial estimate the modified NR to improve it.
    //    double ncp = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double deriv = 0;
    //    double C = 0;
    //    double D = 0;
    //    double E = 0;
    //    double sqarg = 0;
    //    bool checked_nc_limit = false;
    //    bool checked_0_limit = false;
    //    double hi = 0;
    //    double lo = 0;
    //    double nc_derivative = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = ncp_BETA_nc1(1.0 - prob, x, y, a, B);
    //        return functionReturnValue;
    //    }

    //    lo = 0.0;
    //    hi = nc_limit;
    //    checked_0_limit = false;
    //    checked_nc_limit = false;
    //    temp = Math.Pow(inv_normal(prob), 2);
    //    C = B * x / y;
    //    D = temp - 2.0 * (a - C);
    //    if (D < 4 * nc_limit) {
    //        sqarg = Math.Pow(D, 2) - 4 * E;
    //        if (sqarg < 0) {
    //            ncp = D / 2;
    //        } else {
    //            ncp = (D - Math.Sqrt(sqarg)) / 2;
    //        }
    //    } else {
    //        ncp = 0.0;
    //    }
    //    ncp = Min(max(0.0, ncp), nc_limit);
    //    if (x > y) {
    //        pr = BETA(y * (1 + ncp / (ncp + a)) / (1 + ncp / (ncp + a) * y), B, a + Math.Pow(ncp, 2) / (2 * ncp + a));
    //    } else {
    //        pr = compbeta(x / (1 + ncp / (ncp + a) * y), a + Math.Pow(ncp, 2) / (2 * ncp + a), B);
    //    }
    //    //Debug.Print "comp_ncp_BETA_nc1 ncp1 ", ncp, pr
    //    if (ncp == 0.0) {
    //        if (pr > prob) {
    //            functionReturnValue = compInvBetaLessThanX(prob, x, y, a, B);
    //            return functionReturnValue;
    //        } else {
    //            checked_0_limit = true;
    //        }
    //    }
    //    temp = Min(max(0.0, invgamma(B * x, prob) / y - a), nc_limit);
    //    if (temp == ncp) {
    //        C = pr;
    //    } else if (x > y) {
    //        C = BETA(y * (1 + temp / (temp + a)) / (1 + temp / (temp + a) * y), B, a + Math.Pow(temp, 2) / (2 * temp + a));
    //    } else {
    //        C = compbeta(x / (1 + temp / (temp + a) * y), a + Math.Pow(temp, 2) / (2 * temp + a), B);
    //    }
    //    //Debug.Print "comp_ncp_BETA_nc1 ncp2 ", temp, c
    //    if (temp == 0.0) {
    //        if (C > prob) {
    //            functionReturnValue = compInvBetaLessThanX(prob, x, y, a, B);
    //            return functionReturnValue;
    //        } else {
    //            checked_0_limit = true;
    //        }
    //    }
    //    if (pr * C == 0.0) {
    //        ncp = max(ncp, temp);
    //        pr = max(pr, C);
    //    } else if (Math.Abs(Math.Log(pr / prob)) > Math.Abs(Math.Log(C / prob))) {
    //        ncp = temp;
    //        pr = C;
    //    }
    //    if (ncp == 0.0) {
    //        if (pr > prob) {
    //            functionReturnValue = compInvBetaLessThanX(prob, x, y, a, B);
    //            return functionReturnValue;
    //        } else {
    //            if (B > 1 + 1E-06) {
    //                deriv = BETA_nc1(x, y, a + 1.0, B - 1.0, 0.0, ref nc_derivative);
    //                deriv = nc_derivative * Math.Pow(y, 2) / (B - 1.0);
    //            } else {
    //                deriv = comp_BETA_nc1(x, y, a + 1.0, B, 0.0, ref nc_derivative) - pr;
    //            }
    //            if (deriv == 0.0) {
    //                ncp = nc_limit;
    //            } else {
    //                ncp = (prob - pr) / deriv;
    //                if (ncp >= nc_limit) {
    //                    ncp = -(pr / deriv) * logdif(pr, prob);
    //                }
    //            }
    //            checked_0_limit = true;
    //        }
    //    } else {
    //        if (ncp == nc_limit) {
    //            if (pr < prob) {
    //                functionReturnValue = ErrorValue;
    //                return functionReturnValue;
    //            } else {
    //                checked_nc_limit = true;
    //            }
    //        }
    //        if (pr > 0) {
    //            temp = ncp * 0.999999;
    //            //Use numerical derivative on approximation since cheap compared to evaluating non-central BETA
    //            if (x > y) {
    //                C = BETA(y * (1.0 + temp / (temp + a)) / (1 + temp / (temp + a) * y), B, a + Math.Pow(temp, 2) / (2 * temp + a));
    //            } else {
    //                C = compbeta(x / (1 + temp / (temp + a) * y), a + Math.Pow(temp, 2) / (2 * temp + a), B);
    //            }
    //            if (pr != C) {
    //                dif = -(1E-06 * ncp * pr / (pr - C)) * logdif(pr, prob);
    //                if (ncp + dif < 0) {
    //                    ncp = ncp / 2;
    //                } else if (ncp + dif > nc_limit) {
    //                    ncp = (ncp + nc_limit) / 2;
    //                } else {
    //                    ncp = ncp + dif;
    //                }
    //            }
    //        } else {
    //            ncp = (nc_limit + ncp) / 2.0;
    //        }
    //    }
    //    dif = ncp;
    //    do {
    //        pr = comp_BETA_nc1(x, y, a, B, ncp, ref nc_derivative);
    //        //Debug.Print ncp, pr, prob
    //        if (B > 1 + 1E-06) {
    //            deriv = BETA_nc1(x, y, a + 1.0, B - 1.0, ncp, ref nc_derivative);
    //            deriv = nc_derivative * Math.Pow(y, 2) / (B - 1.0);
    //        } else {
    //            deriv = comp_BETA_nc1(x, y, a + 1.0, B, ncp, ref nc_derivative) - pr;
    //        }
    //        if (pr < 3E-308 & deriv == 0.0) {
    //            lo = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp + dif;
    //        } else if (deriv == 0.0) {
    //            hi = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else {
    //            if (pr < prob) {
    //                lo = ncp;
    //            } else {
    //                hi = ncp;
    //            }
    //            dif = -(pr / deriv) * logdif(pr, prob);
    //            if (ncp + dif < lo) {
    //                dif = (lo - ncp) / 2.0;
    //                if (!checked_0_limit & (lo == 0.0)) {
    //                    temp = comp_cdf_BETA_nc(x, a, B, lo);
    //                    if (temp > prob) {
    //                        functionReturnValue = compInvBetaLessThanX(prob, x, y, a, B);
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_0_limit = true;
    //                    }
    //                }
    //            } else if (ncp + dif > hi) {
    //                dif = (hi - ncp) / 2.0;
    //                if (!checked_nc_limit & (hi == nc_limit)) {
    //                    temp = comp_cdf_BETA_nc(x, a, B, hi);
    //                    if (temp < prob) {
    //                        functionReturnValue = ErrorValue;
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_nc_limit = true;
    //                    }
    //                }
    //            }
    //            ncp = ncp + dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(ncp) * 1E-10)));
    //    functionReturnValue = ncp;
    //    return functionReturnValue;
    //    //Debug.Print "comp_ncp_BETA_nc1", comp_ncp_BETA_nc1
    //}

    //protected internal static double pdf_BETA_nc(double x, double shape_param1, double shape_param2, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param1 < 0.0) | (shape_param2 < 0.0) | (nc_param < 0.0) | (nc_param > nc_limit) | ((shape_param1 == 0.0) & (shape_param2 == 0.0))) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0 | x > 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 | nc_param == 0.0)) {
    //        functionReturnValue = Math.Exp(-nc_param) * pdf_BETA(x, shape_param1, shape_param2);
    //    } else if ((x == 1.0 & shape_param2 == 1.0)) {
    //        functionReturnValue = shape_param1 + nc_param;
    //    } else if ((x == 1.0)) {
    //        functionReturnValue = pdf_BETA(x, shape_param1, shape_param2);
    //    } else {
    //        double nc_derivative = 0;
    //        if ((shape_param1 < 1.0 | x * shape_param2 <= (1.0 - x) * (shape_param1 + nc_param))) {
    //            functionReturnValue = BETA_nc1(x, 1.0 - x, shape_param1, shape_param2, nc_param, ref nc_derivative);
    //        } else {
    //            functionReturnValue = comp_BETA_nc1(x, 1.0 - x, shape_param1, shape_param2, nc_param, ref nc_derivative);
    //        }
    //        functionReturnValue = nc_derivative;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_BETA_nc(double x, double shape_param1, double shape_param2, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    double nc_derivative = 0;
    //    if ((shape_param1 < 0.0) | (shape_param2 < 0.0) | (nc_param < 0.0) | (nc_param > nc_limit) | ((shape_param1 == 0.0) & (shape_param2 == 0.0))) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x >= 1.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((x == 0.0 & shape_param1 == 0.0)) {
    //        functionReturnValue = Math.Exp(-nc_param);
    //    } else if ((x == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((nc_param == 0.0)) {
    //        functionReturnValue = BETA(x, shape_param1, shape_param2);
    //    } else if ((shape_param1 < 1.0 | x * shape_param2 <= (1.0 - x) * (shape_param1 + nc_param))) {
    //        functionReturnValue = BETA_nc1(x, 1.0 - x, shape_param1, shape_param2, nc_param, ref nc_derivative);
    //    } else {
    //        functionReturnValue = 1.0 - comp_BETA_nc1(x, 1.0 - x, shape_param1, shape_param2, nc_param, ref nc_derivative);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_BETA_nc(double x, double shape_param1, double shape_param2, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    double nc_derivative = 0;
    //    if ((shape_param1 < 0.0) | (shape_param2 < 0.0) | (nc_param < 0.0) | (nc_param > nc_limit) | ((shape_param1 == 0.0) & (shape_param2 == 0.0))) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((x >= 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 & shape_param1 == 0.0)) {
    //        functionReturnValue = -expm1(-nc_param);
    //    } else if ((x == 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((nc_param == 0.0)) {
    //        functionReturnValue = compbeta(x, shape_param1, shape_param2);
    //    } else if ((shape_param1 < 1.0 | x * shape_param2 >= (1.0 - x) * (shape_param1 + nc_param))) {
    //        functionReturnValue = comp_BETA_nc1(x, 1.0 - x, shape_param1, shape_param2, nc_param, ref nc_derivative);
    //    } else {
    //        functionReturnValue = 1.0 - BETA_nc1(x, 1.0 - x, shape_param1, shape_param2, nc_param, ref nc_derivative);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_BETA_nc(double prob, double shape_param1, double shape_param2, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    double oneMinusP = 0;
    //    if ((shape_param1 < 0.0) | (shape_param2 <= 0.0) | (nc_param < 0.0) | (nc_param > nc_limit) | (prob < 0.0) | (prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0 | shape_param1 == 0.0 & prob <= Math.Exp(-nc_param))) {
    //        functionReturnValue = 0.0;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((nc_param == 0.0)) {
    //        functionReturnValue = invbeta(shape_param1, shape_param2, prob, ref oneMinusP);
    //    } else {
    //        functionReturnValue = inv_BETA_nc1(prob, shape_param1, shape_param2, nc_param, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_BETA_nc(double prob, double shape_param1, double shape_param2, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    double oneMinusP = 0;
    //    if ((shape_param1 < 0.0) | (shape_param2 <= 0.0) | (nc_param < 0.0) | (nc_param > nc_limit) | (prob < 0.0) | (prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 1.0 | shape_param1 == 0.0 & prob >= -expm1(-nc_param))) {
    //        functionReturnValue = 0.0;
    //    } else if ((prob == 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((nc_param == 0.0)) {
    //        functionReturnValue = invcompbeta(shape_param1, shape_param2, prob, ref oneMinusP);
    //    } else {
    //        functionReturnValue = comp_inv_BETA_nc1(prob, shape_param1, shape_param2, nc_param, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ncp_BETA_nc(double prob, double x, double shape_param1, double shape_param2)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param1 < 0.0) | (shape_param2 <= 0.0) | (x < 0.0) | (x >= 1.0) | (prob <= 0.0) | (prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x == 0.0 & shape_param1 == 0.0)) {
    //        functionReturnValue = -Math.Log(prob);
    //    } else if ((x == 0.0 | prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = ncp_BETA_nc1(prob, x, 1.0 - x, shape_param1, shape_param2);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_ncp_BETA_nc(double prob, double x, double shape_param1, double shape_param2)
    //{
    //    double functionReturnValue = 0;
    //    if ((shape_param1 < 0.0) | (shape_param2 <= 0.0) | (x < 0.0) | (x >= 1.0) | (prob < 0.0) | (prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x == 0.0 & shape_param1 == 0.0)) {
    //        functionReturnValue = -log0(-prob);
    //    } else if ((x == 0.0 | prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = comp_ncp_BETA_nc1(prob, x, 1.0 - x, shape_param1, shape_param2);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_fdist_nc(double x, double df1, double df2, double nc)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0 | (nc < 0.0) | (nc > 2.0 * nc_limit))) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((x == 0.0 | nc == 0.0)) {
    //        functionReturnValue = Math.Exp(-nc / 2.0) * pdf_fdist(x, df1, df2);
    //    } else {
    //        double p = 0;
    //        double Q = 0;
    //        double nc_derivative = 0;
    //        if (x > 1.0) {
    //            Q = df2 / x;
    //            p = Q + df1;
    //            Q = Q / p;
    //            p = df1 / p;
    //        } else {
    //            p = df1 * x;
    //            Q = df2 + p;
    //            p = p / Q;
    //            Q = df2 / Q;
    //        }
    //        if ((df1 < 1.0 | p * df2 <= Q * (df1 + nc))) {
    //            functionReturnValue = BETA_nc1(p, Q, df1 / 2.0, df2 / 2.0, nc / 2.0, ref nc_derivative);
    //        } else {
    //            functionReturnValue = comp_BETA_nc1(p, Q, df1 / 2.0, df2 / 2.0, nc / 2.0, ref nc_derivative);
    //        }
    //        functionReturnValue = (nc_derivative * Q) * (df1 * Q / df2);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_fdist_nc(double x, double df1, double df2, double nc)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0 | (nc < 0.0) | (nc > 2.0 * nc_limit))) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double p = 0;
    //        double Q = 0;
    //        double nc_derivative = 0;
    //        if (x > 1.0) {
    //            Q = df2 / x;
    //            p = Q + df1;
    //            Q = Q / p;
    //            p = df1 / p;
    //        } else {
    //            p = df1 * x;
    //            Q = df2 + p;
    //            p = p / Q;
    //            Q = df2 / Q;
    //        }
    //        //If p < cSmall And x <> 0# Or q < cSmall Then
    //        //   cdf_fdist_nc = ErrorValue
    //        //   Exit Function
    //        //End If
    //        df2 = df2 / 2.0;
    //        df1 = df1 / 2.0;
    //        nc = nc / 2.0;
    //        if ((nc == 0.0 & p <= Q)) {
    //            functionReturnValue = BETA(p, df1, df2);
    //        } else if ((nc == 0.0)) {
    //            functionReturnValue = compbeta(Q, df2, df1);
    //        } else if ((df1 < 1.0 | p * df2 <= Q * (df1 + nc))) {
    //            functionReturnValue = BETA_nc1(p, Q, df1, df2, nc, ref nc_derivative);
    //        } else {
    //            functionReturnValue = 1.0 - comp_BETA_nc1(p, Q, df1, df2, nc, ref nc_derivative);
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_fdist_nc(double x, double df1, double df2, double nc)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0 | (nc < 0.0) | (nc > 2.0 * nc_limit))) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x <= 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        double p = 0;
    //        double Q = 0;
    //        double nc_derivative = 0;
    //        if (x > 1.0) {
    //            Q = df2 / x;
    //            p = Q + df1;
    //            Q = Q / p;
    //            p = df1 / p;
    //        } else {
    //            p = df1 * x;
    //            Q = df2 + p;
    //            p = p / Q;
    //            Q = df2 / Q;
    //        }
    //        //If p < cSmall And x <> 0# Or q < cSmall Then
    //        //   comp_cdf_fdist_nc = ErrorValue
    //        //   Exit Function
    //        //End If
    //        df2 = df2 / 2.0;
    //        df1 = df1 / 2.0;
    //        nc = nc / 2.0;
    //        if ((nc == 0.0 & p <= Q)) {
    //            functionReturnValue = compbeta(p, df1, df2);
    //        } else if ((nc == 0.0)) {
    //            functionReturnValue = BETA(Q, df2, df1);
    //        } else if ((df1 < 1.0 | p * df2 >= Q * (df1 + nc))) {
    //            functionReturnValue = comp_BETA_nc1(p, Q, df1, df2, nc, ref nc_derivative);
    //        } else {
    //            functionReturnValue = 1.0 - BETA_nc1(p, Q, df1, df2, nc, ref nc_derivative);
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_fdist_nc(double prob, double df1, double df2, double nc)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0 | (nc < 0.0) | (nc > 2.0 * nc_limit) | prob < 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double temp = 0;
    //        double oneMinusP = 0;
    //        df1 = df1 / 2.0;
    //        df2 = df2 / 2.0;
    //        if (nc == 0.0) {
    //            temp = invbeta(df1, df2, prob, ref oneMinusP);
    //        } else {
    //            temp = inv_BETA_nc1(prob, df1, df2, nc / 2.0, ref oneMinusP);
    //        }
    //        functionReturnValue = df2 * temp / (df1 * oneMinusP);
    //        //If oneMinusP < cSmall Then inv_fdist_nc = ErrorValue
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_fdist_nc(double prob, double df1, double df2, double nc)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0 | df2 <= 0.0 | (nc < 0.0) | (nc > 2.0 * nc_limit) | prob <= 0.0 | prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        double temp = 0;
    //        double oneMinusP = 0;
    //        df1 = df1 / 2.0;
    //        df2 = df2 / 2.0;
    //        if (nc == 0.0) {
    //            temp = invcompbeta(df1, df2, prob, ref oneMinusP);
    //        } else {
    //            temp = comp_inv_BETA_nc1(prob, df1, df2, nc / 2.0, ref oneMinusP);
    //        }
    //        functionReturnValue = df2 * temp / (df1 * oneMinusP);
    //        //If oneMinusP < cSmall Then comp_inv_fdist_nc = ErrorValue
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ncp_fdist_nc(double prob, double x, double df1, double df2)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0) | (df2 <= 0.0) | (x <= 0.0) | (prob <= 0.0) | (prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double p = 0;
    //        double Q = 0;
    //        if (x > 1.0) {
    //            Q = df2 / x;
    //            p = Q + df1;
    //            Q = Q / p;
    //            p = df1 / p;
    //        } else {
    //            p = df1 * x;
    //            Q = df2 + p;
    //            p = p / Q;
    //            Q = df2 / Q;
    //        }
    //        df2 = df2 / 2.0;
    //        df1 = df1 / 2.0;
    //        functionReturnValue = ncp_BETA_nc1(prob, p, Q, df1, df2) * 2.0;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_ncp_fdist_nc(double prob, double x, double df1, double df2)
    //{
    //    double functionReturnValue = 0;
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    if ((df1 <= 0.0) | (df2 <= 0.0) | (x <= 0.0) | (prob <= 0.0) | (prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double p = 0;
    //        double Q = 0;
    //        if (x > 1.0) {
    //            Q = df2 / x;
    //            p = Q + df1;
    //            Q = Q / p;
    //            p = df1 / p;
    //        } else {
    //            p = df1 * x;
    //            Q = df2 + p;
    //            p = p / Q;
    //            Q = df2 / Q;
    //        }
    //        df1 = df1 / 2.0;
    //        df2 = df2 / 2.0;
    //        functionReturnValue = comp_ncp_BETA_nc1(prob, p, Q, df1, df2) * 2.0;
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //private static  double t_nc1(double T, double df, double nct, ref double nc_derivative)
    //{
    //    double functionReturnValue = 0;
    //    //y is 1-x but held accurately to avoid possible cancellation errors
    //    //nc_derivative holds t * derivative
    //    double aa = 0;
    //    double bb = 0;
    //    double nc_dtemp = 0;
    //    double N = 0;
    //    double p = 0;
    //    double Q = 0;
    //    double W = 0;
    //    double V = 0;
    //    double r = 0;
    //    double S = 0;
    //    double ps = 0;
    //    double result1 = 0;
    //    double result2 = 0;
    //    double term1 = 0;
    //    double term2 = 0;
    //    double ptnc = 0;
    //    double qtnc = 0;
    //    double ptx = 0;
    //    double qtx = 0;
    //    double a = 0;
    //    double B = 0;
    //    double x = 0;
    //    double y = 0;
    //    double nc = 0;
    //    double save_result1 = 0;
    //    double save_result2 = 0;
    //    double phi = 0;
    //    double vScale = 0;
    //    phi = cnormal(-Math.Abs(nct));
    //    a = 0.5;
    //    B = df / 2.0;
    //    if (Math.Abs(T) >= Min(1.0, df)) {
    //        y = df / T;
    //        x = T + y;
    //        y = y / x;
    //        x = T / x;
    //    } else {
    //        x = T * T;
    //        y = df + x;
    //        x = x / y;
    //        y = df / y;
    //    }
    //    if (y < cSmall) {
    //        functionReturnValue = ErrorValue;
    //        return functionReturnValue;
    //    }
    //    nc = nct * nct / 2.0;
    //    aa = a - nc * x * (a + B);
    //    bb = (x * nc - 1.0) - a;
    //    if ((bb < 0.0)) {
    //        N = bb - Math.Sqrt(Math.Pow(bb, 2) - 4.0 * aa);
    //        N = Convert.ToInt32(2.0 * aa / N);
    //    } else {
    //        N = Convert.ToInt32((bb + Math.Sqrt(Math.Pow(bb, 2) - 4.0 * aa)) / 2.0);
    //    }
    //    if (N < 0.0) {
    //        N = 0.0;
    //    }
    //    aa = N + a;
    //    bb = N + 0.5;
    //    qtnc = poissonTerm(bb, nc, nc - bb, 0.0);
    //    bb = N;
    //    ptnc = poissonTerm(bb, nc, nc - bb, 0.0);
    //    ptx = binomialTerm(aa, B, x, y, B * x - aa * y, 0.0) / (aa + B);
    //    //(I(x, aa, b) - I(x, aa+1, b))/b
    //    qtx = binomialTerm(aa + 0.5, B, x, y, B * x - (aa + 0.5) * y, 0.0) / (aa + B + 0.5);
    //    //(I(x, aa+1/2, b) - I(x, aa+3/2, b))/b
    //    if (B > 1.0) {
    //        ptx = B * ptx;
    //        qtx = B * qtx;
    //    }
    //    vScale = max(ptx, qtx);
    //    if (ptx == vScale) {
    //        S = 1.0;
    //    } else {
    //        S = ptx / vScale;
    //    }
    //    if (qtx == vScale) {
    //        r = 1.0;
    //    } else {
    //        r = qtx / vScale;
    //    }
    //    S = (aa + B) * S;
    //    r = (aa + B + 0.5) * r;
    //    aa = aa + 1.0;
    //    bb = bb + 1.0;
    //    p = nc / bb * ptnc;
    //    Q = nc / (bb + 0.5) * qtnc;
    //    ps = p * S + Q * r;
    //    nc_derivative = ps;
    //    S = x / aa * S;
    //    // I(x, aa, b) - I(x, aa+1, b)
    //    r = x / (aa + 0.5) * r;
    //    // I(x, aa+1/2, b) - I(x, aa+3/2, b)
    //    W = p;
    //    V = Q;
    //    term1 = S * W;
    //    term2 = r * V;
    //    result1 = term1;
    //    result2 = term2;
    //    while (((((term1 + term2) > 1E-15 * (result1 + result2)) & (p > 1E-16 * W)) | (ps > 1E-16 * nc_derivative))) {
    //        S = (aa + B) * S;
    //        r = (aa + B + 0.5) * r;
    //        aa = aa + 1.0;
    //        bb = bb + 1.0;
    //        p = nc / bb * p;
    //        Q = nc / (bb + 0.5) * Q;
    //        ps = p * S + Q * r;
    //        nc_derivative = nc_derivative + ps;
    //        S = x / aa * S;
    //        // I(x, aa, b) - I(x, aa+1, b)
    //        r = x / (aa + 0.5) * r;
    //        // I(x, aa+1/2, b) - I(x, aa+3/2, b)
    //        W = W + p;
    //        V = V + Q;
    //        term1 = S * W;
    //        term2 = r * V;
    //        result1 = result1 + term1;
    //        result2 = result2 + term2;
    //    }
    //    if (x > y) {
    //        S = compbeta(y, B, a + (bb + 1.0));
    //        r = compbeta(y, B, a + (bb + 1.5));
    //    } else {
    //        S = BETA(x, a + (bb + 1.0), B);
    //        r = BETA(x, a + (bb + 1.5), B);
    //    }
    //    nc_derivative = x * nc_derivative * vScale;
    //    if (B <= 1.0)
    //        vScale = vScale * B;
    //    save_result1 = result1 * vScale + S * W;
    //    save_result2 = result2 * vScale + r * V;

    //    ps = 1.0;
    //    nc_dtemp = 0.0;
    //    aa = N + a;
    //    bb = N;
    //    vScale = max(ptnc, qtnc);
    //    if (ptnc == vScale) {
    //        p = 1.0;
    //    } else {
    //        p = ptnc / vScale;
    //    }
    //    if (qtnc == vScale) {
    //        Q = 1.0;
    //    } else {
    //        Q = qtnc / vScale;
    //    }
    //    S = ptx;
    //    // I(x, aa, b) - I(x, aa+1, b)
    //    r = qtx;
    //    // I(x, aa+1/2, b) - I(x, aa+3/2, b)
    //    if (x > y) {
    //        W = compbeta(y, B, aa);
    //        // I(x, aa, b)
    //        V = compbeta(y, B, aa + 0.5);
    //        // I(x, aa+1/2, b)
    //    } else {
    //        W = BETA(x, aa, B);
    //        // I(x, aa, b)
    //        V = BETA(x, aa + 0.5, B);
    //        // I(x, aa+1/2, b)
    //    }
    //    term1 = p * W;
    //    term2 = Q * V;
    //    result1 = term1;
    //    result2 = term2;
    //    while (bb > 0.0 & ((((term1 + term2) > 1E-15 * (result1 + result2)) & (S > 1E-16 * W)) | (ps > 1E-16 * nc_dtemp))) {
    //        S = aa / x * S;
    //        r = (aa + 0.5) / x * r;
    //        ps = p * S + Q * r;
    //        nc_dtemp = nc_dtemp + ps;
    //        p = bb / nc * p;
    //        Q = (bb + 0.5) / nc * Q;
    //        aa = aa - 1.0;
    //        bb = bb - 1.0;
    //        if (bb == 0.0)
    //            aa = a;
    //        S = S / (aa + B);
    //        // I(x, aa, b) - I(x, aa+1, b)
    //        r = r / (aa + B + 0.5);
    //        // I(x, aa+1/2, b) - I(x, aa+3/2, b)
    //        if (B > 1.0) {
    //            W = W + S;
    //            // I(x, aa, b)
    //            V = V + r;
    //            // I(x, aa+0.5, b)
    //        } else {
    //            W = W + B * S;
    //            V = V + B * r;
    //        }
    //        term1 = p * W;
    //        term2 = Q * V;
    //        result1 = result1 + term1;
    //        result2 = result2 + term2;
    //    }
    //    nc_dtemp = x * nc_dtemp + p * aa * S + Q * (aa + 0.5) * r;
    //    p = cpoisson(bb - 1.0, nc, nc - bb + 1.0);
    //    Q = cpoisson(bb - 0.5, nc, nc - bb + 0.5) - 2.0 * phi;
    //    result1 = save_result1 + result1 * vScale + p * W;
    //    result2 = save_result2 + result2 * vScale + Q * V;
    //    if (T > 0.0) {
    //        functionReturnValue = phi + 0.5 * (result1 + result2);
    //        nc_derivative = nc_derivative + nc_dtemp * vScale;
    //    } else {
    //        functionReturnValue = phi - 0.5 * (result1 - result2);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double comp_t_nc1(double T, double df, double nct, ref double nc_derivative)
    //{
    //    double functionReturnValue = 0;
    //    //y is 1-x but held accurately to avoid possible cancellation errors
    //    //nc_derivative holds t * derivative
    //    double aa = 0;
    //    double bb = 0;
    //    double nc_dtemp = 0;
    //    double N = 0;
    //    double p = 0;
    //    double Q = 0;
    //    double W = 0;
    //    double V = 0;
    //    double r = 0;
    //    double S = 0;
    //    double ps = 0;
    //    double result1 = 0;
    //    double result2 = 0;
    //    double term1 = 0;
    //    double term2 = 0;
    //    double ptnc = 0;
    //    double qtnc = 0;
    //    double ptx = 0;
    //    double qtx = 0;
    //    double a = 0;
    //    double B = 0;
    //    double x = 0;
    //    double y = 0;
    //    double nc = 0;
    //    double save_result1 = 0;
    //    double save_result2 = 0;
    //    double vScale = 0;
    //    a = 0.5;
    //    B = df / 2.0;
    //    if (Math.Abs(T) >= Min(1.0, df)) {
    //        y = df / T;
    //        x = T + y;
    //        y = y / x;
    //        x = T / x;
    //    } else {
    //        x = T * T;
    //        y = df + x;
    //        x = x / y;
    //        y = df / y;
    //    }
    //    if (y < cSmall) {
    //        functionReturnValue = ErrorValue;
    //        return functionReturnValue;
    //    }
    //    nc = nct * nct / 2.0;
    //    aa = a - nc * x * (a + B);
    //    bb = (x * nc - 1.0) - a;
    //    if ((bb < 0.0)) {
    //        N = bb - Math.Sqrt(Math.Pow(bb, 2) - 4.0 * aa);
    //        N = Convert.ToInt32(2.0 * aa / N);
    //    } else {
    //        N = Convert.ToInt32((bb + Math.Sqrt(Math.Pow(bb, 2) - 4.0 * aa)) / 2);
    //    }
    //    if (N < 0.0) {
    //        N = 0.0;
    //    }
    //    aa = N + a;
    //    bb = N + 0.5;
    //    qtnc = poissonTerm(bb, nc, nc - bb, 0.0);
    //    bb = N;
    //    ptnc = poissonTerm(bb, nc, nc - bb, 0.0);
    //    ptx = binomialTerm(aa, B, x, y, B * x - aa * y, 0.0) / (aa + B);
    //    //((1 - I(x, aa+1, b)) - (1 - I(x, aa, b)))/b
    //    qtx = binomialTerm(aa + 0.5, B, x, y, B * x - (aa + 0.5) * y, 0.0) / (aa + B + 0.5);
    //    //((1 - I(x, aa+3/2, b)) - (1 - I(x, aa+1/2, b)))/b
    //    if (B > 1.0) {
    //        ptx = B * ptx;
    //        qtx = B * qtx;
    //    }
    //    vScale = max(ptnc, qtnc);
    //    if (ptnc == vScale) {
    //        p = 1.0;
    //    } else {
    //        p = ptnc / vScale;
    //    }
    //    if (qtnc == vScale) {
    //        Q = 1.0;
    //    } else {
    //        Q = qtnc / vScale;
    //    }
    //    nc_derivative = 0.0;
    //    S = ptx;
    //    r = qtx;
    //    if (x > y) {
    //        V = BETA(y, B, aa + 0.5);
    //        //  1 - I(x, aa+1/2, b)
    //        W = BETA(y, B, aa);
    //        //  1 - I(x, aa, b)
    //    } else {
    //        V = compbeta(x, aa + 0.5, B);
    //        // 1 - I(x, aa+1/2, b)
    //        W = compbeta(x, aa, B);
    //        // 1 - I(x, aa, b)
    //    }
    //    term1 = 0.0;
    //    term2 = 0.0;
    //    result1 = term1;
    //    result2 = term2;
    //    do {
    //        if (B > 1.0) {
    //            W = W + S;
    //            // 1 - I(x, aa, b)
    //            V = V + r;
    //            // 1 - I(x, aa+1/2, b)
    //        } else {
    //            W = W + B * S;
    //            V = V + B * r;
    //        }
    //        S = (aa + B) * S;
    //        r = (aa + B + 0.5) * r;
    //        aa = aa + 1.0;
    //        bb = bb + 1.0;
    //        p = nc / bb * p;
    //        Q = nc / (bb + 0.5) * Q;
    //        ps = p * S + Q * r;
    //        nc_derivative = nc_derivative + ps;
    //        S = x / aa * S;
    //        // (1 - I(x, aa+1, b)) - (1 - I(x, aa, b))
    //        r = x / (aa + 0.5) * r;
    //        // (1 - I(x, aa+3/2, b)) - (1 - I(x, aa+1/2, b))
    //        term1 = p * W;
    //        term2 = Q * V;
    //        result1 = result1 + term1;
    //        result2 = result2 + term2;
    //    } while (((((term1 + term2) > 1E-15 * (result1 + result2)) & (S > 1E-16 * W)) | (ps > 1E-16 * nc_derivative)));
    //    p = comppoisson(bb, nc, nc - bb);
    //    bb = bb + 0.5;
    //    Q = comppoisson(bb, nc, nc - bb);
    //    nc_derivative = x * nc_derivative * vScale;
    //    save_result1 = result1 * vScale + p * W;
    //    save_result2 = result2 * vScale + Q * V;
    //    ps = 1.0;
    //    nc_dtemp = 0.0;
    //    aa = N + a;
    //    bb = N;
    //    p = ptnc;
    //    Q = qtnc;
    //    vScale = max(ptx, qtx);
    //    if (ptx == vScale) {
    //        S = 1.0;
    //    } else {
    //        S = ptx / vScale;
    //    }
    //    if (qtx == vScale) {
    //        r = 1.0;
    //    } else {
    //        r = qtx / vScale;
    //    }
    //    W = p;
    //    V = Q;
    //    term1 = 1.0;
    //    term2 = 1.0;
    //    result1 = 0.0;
    //    result2 = 0.0;
    //    while (bb > 0.0 & ((((term1 + term2) > 1E-15 * (result1 + result2)) & (p > 1E-16 * W)) | (ps > 1E-16 * nc_dtemp))) {
    //        r = (aa + 0.5) / x * r;
    //        S = aa / x * S;
    //        ps = p * S + Q * r;
    //        nc_dtemp = nc_dtemp + ps;
    //        p = bb / nc * p;
    //        Q = (bb + 0.5) / nc * Q;
    //        aa = aa - 1.0;
    //        bb = bb - 1.0;
    //        if (bb == 0.0)
    //            aa = a;
    //        r = r / (aa + B + 0.5);
    //        // (1 - I(x, aa+3/2, b)) - (1 - I(x, aa+1/2, b))
    //        S = S / (aa + B);
    //        // (1 - I(x, aa + 1, b)) - (1 - I(x, aa, b))
    //        term1 = S * W;
    //        term2 = r * V;
    //        result1 = result1 + term1;
    //        result2 = result2 + term2;
    //        W = W + p;
    //        V = V + Q;
    //    }
    //    nc_dtemp = (x * nc_dtemp + p * aa * S + Q * (aa + 0.5) * r) * vScale;
    //    if (x > y) {
    //        r = BETA(y, B, a + (bb + 0.5));
    //        S = BETA(y, B, a + bb);
    //    } else {
    //        r = compbeta(x, a + (bb + 0.5), B);
    //        S = compbeta(x, a + bb, B);
    //    }
    //    if (B <= 1.0)
    //        vScale = vScale * B;
    //    result1 = save_result1 + result1 * vScale + S * W;
    //    result2 = save_result2 + result2 * vScale + r * V;
    //    if (T > 0.0) {
    //        functionReturnValue = 0.5 * (result1 + result2);
    //        nc_derivative = nc_derivative + nc_dtemp;
    //    } else {
    //        functionReturnValue = 1.0 - 0.5 * (result1 - result2);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double inv_t_nc1(double prob, double df, double nc, ref double oneMinusP)
    //{
    //    double functionReturnValue = 0;
    //    //Uses approximations in A&S 26.6.26 and 26.7.10 for to get initial estimate, the modified NR to improve it.
    //    double x = 0;
    //    double y = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double nc_BETA_param = 0;
    //    double hix = 0;
    //    double lox = 0;
    //    double test = 0;
    //    double nc_derivative = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = comp_inv_t_nc1(1.0 - prob, df, nc, ref oneMinusP);
    //        return functionReturnValue;
    //    }
    //    nc_BETA_param = Math.Pow(nc, 2) / 2.0;
    //    lox = 0.0;
    //    hix = t_nc_limit * Math.Sqrt(df);
    //    pr = Math.Exp(-nc_BETA_param);
    //    if (pr > prob) {
    //        if (2.0 * prob > pr) {
    //            x = invcompbeta(0.5, df / 2.0, (pr - prob) / pr, ref oneMinusP);
    //        } else {
    //            x = invbeta(0.5, df / 2.0, prob / pr, ref oneMinusP);
    //        }
    //        if (x == 0.0) {
    //            functionReturnValue = 0.0;
    //            return functionReturnValue;
    //        } else {
    //            temp = oneMinusP;
    //            y = invbeta(Math.Pow((0.5 + nc_BETA_param), 2) / (0.5 + 2.0 * nc_BETA_param), df / 2.0, prob, ref oneMinusP);
    //            oneMinusP = (0.5 + nc_BETA_param) * oneMinusP / (0.5 + nc_BETA_param * (1.0 + y));
    //            if (temp > oneMinusP) {
    //                oneMinusP = temp;
    //            } else {
    //                x = (0.5 + 2.0 * nc_BETA_param) * y / (0.5 + nc_BETA_param * (1.0 + y));
    //            }
    //            if (oneMinusP < cSmall) {
    //                pr = t_nc1(hix, df, nc, ref nc_derivative);
    //                if (pr < prob) {
    //                    functionReturnValue = ErrorValue;
    //                    oneMinusP = 0.0;
    //                    return functionReturnValue;
    //                }
    //                oneMinusP = 4.0 * cSmall;
    //            }
    //        }
    //    } else {
    //        y = invbeta(Math.Pow((0.5 + nc_BETA_param), 2) / (0.5 + 2.0 * nc_BETA_param), df / 2.0, prob, ref oneMinusP);
    //        x = (0.5 + 2.0 * nc_BETA_param) * y / (0.5 + nc_BETA_param * (1 + y));
    //        oneMinusP = (0.5 + nc_BETA_param) * oneMinusP / (0.5 + nc_BETA_param * (1.0 + y));
    //        if (oneMinusP < cSmall) {
    //            pr = t_nc1(hix, df, nc, ref nc_derivative);
    //            if (pr < prob) {
    //                functionReturnValue = ErrorValue;
    //                oneMinusP = 0.0;
    //                return functionReturnValue;
    //            }
    //            oneMinusP = 4.0 * cSmall;
    //        }
    //    }
    //    test = Math.Sqrt(df * x) / Math.Sqrt(oneMinusP);
    //    do {
    //        pr = t_nc1(test, df, nc, ref nc_derivative);
    //        if (pr < prob) {
    //            lox = test;
    //        } else {
    //            hix = test;
    //        }
    //        if (nc_derivative == 0.0) {
    //            if (pr < prob) {
    //                dif = (hix - lox) / 2.0;
    //            } else {
    //                dif = (lox - hix) / 2.0;
    //            }
    //        } else {
    //            dif = -(pr * test / nc_derivative) * logdif(pr, prob);
    //            if (df < 2.0)
    //                dif = 2.0 * dif / df;
    //            if (test + dif < lox) {
    //                if (lox == 0) {
    //                    dif = (lox - test) * 0.9999999999;
    //                } else {
    //                    dif = (lox - test) * 0.9;
    //                }
    //            } else if (test + dif > hix) {
    //                dif = (hix - test) * 0.9;
    //            }
    //        }
    //        test = test + dif;
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > test * 1E-10)));
    //    functionReturnValue = test;
    //    return functionReturnValue;
    //}

    //private static  double comp_inv_t_nc1(double prob, double df, double nc, ref double oneMinusP)
    //{
    //    double functionReturnValue = 0;
    //    //Uses approximations in A&S 26.6.26 and 26.7.10 for to get initial estimate, the modified NR to improve it.
    //    double x = 0;
    //    double y = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double nc_BETA_param = 0;
    //    double hix = 0;
    //    double lox = 0;
    //    double test = 0;
    //    double nc_derivative = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = inv_t_nc1(1.0 - prob, df, nc, ref oneMinusP);
    //        return functionReturnValue;
    //    }
    //    nc_BETA_param = Math.Pow(nc, 2) / 2.0;
    //    lox = 0.0;
    //    hix = t_nc_limit * Math.Sqrt(df);
    //    pr = Math.Exp(-nc_BETA_param);
    //    if (pr > prob) {
    //        if (2.0 * prob > pr) {
    //            x = invbeta(0.5, df / 2.0, (pr - prob) / pr, ref oneMinusP);
    //        } else {
    //            x = invcompbeta(0.5, df / 2.0, prob / pr, ref oneMinusP);
    //        }
    //        if (oneMinusP < cSmall) {
    //            pr = comp_t_nc1(hix, df, nc, ref nc_derivative);
    //            if (pr > prob) {
    //                functionReturnValue = ErrorValue;
    //                oneMinusP = 0.0;
    //                return functionReturnValue;
    //            }
    //            oneMinusP = 4.0 * cSmall;
    //        } else {
    //            temp = oneMinusP;
    //            y = invcompbeta(Math.Pow((0.5 + nc_BETA_param), 2) / (0.5 + 2.0 * nc_BETA_param), df / 2.0, prob, ref oneMinusP);
    //            oneMinusP = (0.5 + nc_BETA_param) * oneMinusP / (0.5 + nc_BETA_param * (1.0 + y));
    //            if (temp < oneMinusP) {
    //                oneMinusP = temp;
    //            } else {
    //                x = (0.5 + 2.0 * nc_BETA_param) * y / (0.5 + nc_BETA_param * (1.0 + y));
    //            }
    //            if (oneMinusP < cSmall) {
    //                pr = comp_t_nc1(hix, df, nc, ref nc_derivative);
    //                if (pr > prob) {
    //                    functionReturnValue = ErrorValue;
    //                    oneMinusP = 0.0;
    //                    return functionReturnValue;
    //                }
    //                oneMinusP = 4.0 * cSmall;
    //            }
    //        }
    //    } else {
    //        y = invcompbeta(Math.Pow((0.5 + nc_BETA_param), 2) / (0.5 + 2.0 * nc_BETA_param), df / 2.0, prob, ref oneMinusP);
    //        x = (0.5 + 2.0 * nc_BETA_param) * y / (0.5 + nc_BETA_param * (1.0 + y));
    //        oneMinusP = (0.5 + nc_BETA_param) * oneMinusP / (0.5 + nc_BETA_param * (1.0 + y));
    //        if (oneMinusP < cSmall) {
    //            pr = comp_t_nc1(hix, df, nc, ref nc_derivative);
    //            if (pr > prob) {
    //                functionReturnValue = ErrorValue;
    //                oneMinusP = 0.0;
    //                return functionReturnValue;
    //            }
    //            oneMinusP = 4.0 * cSmall;
    //        }
    //    }
    //    test = Math.Sqrt(df * x) / Math.Sqrt(oneMinusP);
    //    dif = test;
    //    do {
    //        pr = comp_t_nc1(test, df, nc, ref nc_derivative);
    //        if (pr < prob) {
    //            hix = test;
    //        } else {
    //            lox = test;
    //        }
    //        if (nc_derivative == 0.0) {
    //            if (pr < prob) {
    //                dif = (lox - hix) / 2.0;
    //            } else {
    //                dif = (hix - lox) / 2.0;
    //            }
    //        } else {
    //            dif = (pr * test / nc_derivative) * logdif(pr, prob);
    //            if (df < 2.0)
    //                dif = 2.0 * dif / df;
    //            if (test + dif < lox) {
    //                dif = (lox - test) * 0.9;
    //            } else if (test + dif > hix) {
    //                dif = (hix - test) * 0.9;
    //            }
    //        }
    //        test = test + dif;
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > test * 1E-10)));
    //    functionReturnValue = test;
    //    return functionReturnValue;
    //}

    //private static  double ncp_t_nc1(double prob, double T, double df)
    //{
    //    double functionReturnValue = 0;
    //    //Uses Normal approx for non-central t (A&S 26.7.10) to get initial estimate the modified NR to improve it.
    //    double ncp = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double deriv = 0;
    //    bool checked_tnc_limit = false;
    //    bool checked_0_limit = false;
    //    double hi = 0;
    //    double lo = 0;
    //    double tnc_limit = 0;
    //    double x = 0;
    //    double y = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = comp_ncp_t_nc1(1.0 - prob, T, df);
    //        return functionReturnValue;
    //    }

    //    lo = 0.0;
    //    tnc_limit = Math.Sqrt(2.0 * nc_limit);
    //    hi = tnc_limit;
    //    checked_0_limit = false;
    //    checked_tnc_limit = false;
    //    if (T >= Min(1.0, df)) {
    //        y = df / T;
    //        x = T + y;
    //        y = y / x;
    //        x = T / x;
    //    } else {
    //        x = T * T;
    //        y = df + x;
    //        x = x / y;
    //        y = df / y;
    //    }
    //    temp = -inv_normal(prob);
    //    if (T > df) {
    //        ncp = T * (1.0 - 0.25 / df) + temp * Math.Sqrt(T) * Math.Sqrt((1.0 / T + 0.5 * T / df));
    //    } else {
    //        ncp = T * (1.0 - 0.25 / df) + temp * Math.Sqrt((1.0 + (0.5 * T / df) * T));
    //    }
    //    ncp = max(temp, ncp);
    //    //Debug.Print "ncp_estimate1", ncp
    //    //I think we can put more accurate bounds on when this will not deliver a sensible answer
    //    if (x > 1E-200) {
    //        temp = invcompgamma(0.5 * x * df, prob) / y - 0.5;
    //        if (temp > 0) {
    //            temp = Math.Sqrt(2.0 * temp);
    //            if (temp > ncp) {
    //                ncp = temp;
    //            }
    //        }
    //    }
    //    //Debug.Print "ncp_estimate2", ncp
    //    ncp = Min(ncp, tnc_limit);
    //    if (ncp == tnc_limit) {
    //        pr = cdf_t_nc(T, df, ncp);
    //        if (pr > prob) {
    //            functionReturnValue = ErrorValue;
    //            return functionReturnValue;
    //        } else {
    //            checked_tnc_limit = true;
    //        }
    //    }
    //    dif = ncp;
    //    do {
    //        pr = cdf_t_nc(T, df, ncp);
    //        //Debug.Print ncp, pr, prob
    //        if (ncp > 1) {
    //            deriv = cdf_t_nc(T, df, ncp * (1 - 1E-06));
    //            deriv = 1000000.0 * (deriv - pr) / ncp;
    //        } else if (ncp > 1E-06) {
    //            deriv = cdf_t_nc(T, df, ncp + 1E-06);
    //            deriv = 1000000.0 * (pr - deriv);
    //        } else if (x < y) {
    //            deriv = comp_cdf_BETA(x, 1, df / 2) * OneOverSqrTwoPi;
    //        } else {
    //            deriv = cdf_BETA(y, df / 2, 1) * OneOverSqrTwoPi;
    //        }
    //        if (pr < 3E-308 & deriv == 0.0) {
    //            hi = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else if (deriv == 0.0) {
    //            lo = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else {
    //            if (pr < prob) {
    //                hi = ncp;
    //            } else {
    //                lo = ncp;
    //            }
    //            dif = (pr / deriv) * logdif(pr, prob);
    //            if (ncp + dif < lo) {
    //                dif = (lo - ncp) / 2.0;
    //                if (!checked_0_limit & (lo == 0.0)) {
    //                    temp = cdf_t_nc(T, df, lo);
    //                    if (temp < prob) {
    //                        if (invtdist(prob, df) <= T) {
    //                            functionReturnValue = 0.0;
    //                        } else {
    //                            functionReturnValue = ErrorValue;
    //                        }
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_0_limit = true;
    //                    }
    //                    dif = dif * 1.99999999;
    //                }
    //            } else if (ncp + dif > hi) {
    //                dif = (hi - ncp) / 2.0;
    //                if (!checked_tnc_limit & (hi == tnc_limit)) {
    //                    temp = cdf_t_nc(T, df, hi);
    //                    if (temp > prob) {
    //                        functionReturnValue = ErrorValue;
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_tnc_limit = true;
    //                    }
    //                    dif = dif * 1.99999999;
    //                }
    //            }
    //            ncp = ncp + dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(ncp) * 1E-10)));
    //    functionReturnValue = ncp;
    //    return functionReturnValue;
    //    //Debug.Print "ncp_t_nc1", ncp_t_nc1
    //}

    //private static  double comp_ncp_t_nc1(double prob, double T, double df)
    //{
    //    double functionReturnValue = 0;
    //    //Uses Normal approx for non-central t (A&S 26.7.10) to get initial estimate the modified NR to improve it.
    //    double ncp = 0;
    //    double pr = 0;
    //    double dif = 0;
    //    double temp = 0;
    //    double temp1 = 0;
    //    double temp2 = 0;
    //    double deriv = 0;
    //    bool checked_tnc_limit = false;
    //    bool checked_0_limit = false;
    //    double hi = 0;
    //    double lo = 0;
    //    double tnc_limit = 0;
    //    double x = 0;
    //    double y = 0;
    //    if ((prob > 0.5)) {
    //        functionReturnValue = ncp_t_nc1(1.0 - prob, T, df);
    //        return functionReturnValue;
    //    }

    //    lo = 0.0;
    //    tnc_limit = Math.Sqrt(2.0 * nc_limit);
    //    hi = tnc_limit;
    //    checked_0_limit = false;
    //    checked_tnc_limit = false;
    //    if (T >= Min(1.0, df)) {
    //        y = df / T;
    //        x = T + y;
    //        y = y / x;
    //        x = T / x;
    //    } else {
    //        x = T * T;
    //        y = df + x;
    //        x = x / y;
    //        y = df / y;
    //    }
    //    temp = -inv_normal(prob);
    //    temp1 = T * (1.0 - 0.25 / df);
    //    if (T > df) {
    //        temp2 = temp * Math.Sqrt(T) * Math.Sqrt((1.0 / T + 0.5 * T / df));
    //    } else {
    //        temp2 = temp * Math.Sqrt((1.0 + (0.5 * T / df) * T));
    //    }
    //    ncp = max(temp, temp1 + temp2);
    //    //Debug.Print "comp_ncp ncp estimate1", ncp
    //    //I think we can put more accurate bounds on when this will not deliver a sensible answer
    //    if (x > 1E-200) {
    //        temp = invcompgamma(0.5 * x * df, prob) / y - 0.5;
    //        if (temp > 0) {
    //            temp = Math.Sqrt(2.0 * temp);
    //            if (temp > ncp) {
    //                temp = invgamma(0.5 * x * df, prob) / y - 0.5;
    //                if (temp > 0) {
    //                    ncp = Math.Sqrt(2.0 * temp);
    //                } else {
    //                    ncp = 0;
    //                }
    //            } else {
    //                ncp = temp1 - temp2;
    //            }
    //        } else {
    //            ncp = temp1 - temp2;
    //        }
    //    } else {
    //        ncp = temp1 - temp2;
    //    }
    //    ncp = Min(max(0.0, ncp), tnc_limit);
    //    if (ncp == 0.0) {
    //        pr = comp_cdf_t_nc(T, df, 0.0);
    //        if (pr > prob) {
    //            if (-invtdist(prob, df) <= T) {
    //                functionReturnValue = 0.0;
    //            } else {
    //                functionReturnValue = ErrorValue;
    //            }
    //            return functionReturnValue;
    //        } else if (Math.Abs(pr - prob) <= -prob * 1E-14 * Math.Log(pr)) {
    //            functionReturnValue = 0.0;
    //            return functionReturnValue;
    //        } else {
    //            checked_0_limit = true;
    //        }
    //        if (x < y) {
    //            deriv = -comp_cdf_BETA(x, 1, 0.5 * df) * OneOverSqrTwoPi;
    //        } else {
    //            deriv = -cdf_BETA(y, 0.5 * df, 1) * OneOverSqrTwoPi;
    //        }
    //        if (deriv == 0.0) {
    //            ncp = tnc_limit;
    //        } else {
    //            ncp = (pr - prob) / deriv;
    //            if (ncp >= tnc_limit) {
    //                ncp = (pr / deriv) * logdif(pr, prob);
    //                //If these two are miles apart then best to take invgamma estimate if > 0
    //            }
    //        }
    //    }
    //    ncp = Min(ncp, tnc_limit);
    //    if (ncp == tnc_limit) {
    //        pr = comp_cdf_t_nc(T, df, ncp);
    //        if (pr < prob) {
    //            functionReturnValue = ErrorValue;
    //            return functionReturnValue;
    //        } else {
    //            checked_tnc_limit = true;
    //        }
    //    }
    //    dif = ncp;
    //    do {
    //        pr = comp_cdf_t_nc(T, df, ncp);
    //        //Debug.Print ncp, pr, prob
    //        if (ncp > 1) {
    //            deriv = comp_cdf_t_nc(T, df, ncp * (1 - 1E-06));
    //            deriv = 1000000.0 * (pr - deriv) / ncp;
    //        } else if (ncp > 1E-06) {
    //            deriv = comp_cdf_t_nc(T, df, ncp + 1E-06);
    //            deriv = 1000000.0 * (deriv - pr);
    //        } else if (x < y) {
    //            deriv = comp_cdf_BETA(x, 1, 0.5 * df) * OneOverSqrTwoPi;
    //        } else {
    //            deriv = cdf_BETA(y, 0.5 * df, 1) * OneOverSqrTwoPi;
    //        }
    //        if (pr < 3E-308 & deriv == 0.0) {
    //            lo = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else if (deriv == 0.0) {
    //            hi = ncp;
    //            dif = dif / 2.0;
    //            ncp = ncp - dif;
    //        } else {
    //            if (pr > prob) {
    //                hi = ncp;
    //            } else {
    //                lo = ncp;
    //            }
    //            dif = -(pr / deriv) * logdif(pr, prob);
    //            if (ncp + dif < lo) {
    //                dif = (lo - ncp) / 2.0;
    //                if (!checked_0_limit & (lo == 0.0)) {
    //                    temp = comp_cdf_t_nc(T, df, lo);
    //                    if (temp > prob) {
    //                        if (-invtdist(prob, df) <= T) {
    //                            functionReturnValue = 0.0;
    //                        } else {
    //                            functionReturnValue = ErrorValue;
    //                        }
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_0_limit = true;
    //                    }
    //                    dif = dif * 1.99999999;
    //                }
    //            } else if (ncp + dif > hi) {
    //                dif = (hi - ncp) / 2.0;
    //                if (!checked_tnc_limit & (hi == tnc_limit)) {
    //                    temp = comp_cdf_t_nc(T, df, hi);
    //                    if (temp < prob) {
    //                        functionReturnValue = ErrorValue;
    //                        return functionReturnValue;
    //                    } else {
    //                        checked_tnc_limit = true;
    //                    }
    //                    dif = dif * 1.99999999;
    //                }
    //            }
    //            ncp = ncp + dif;
    //        }
    //    } while (((Math.Abs(pr - prob) > prob * 1E-14) & (Math.Abs(dif) > Math.Abs(ncp) * 1E-10)));
    //    functionReturnValue = ncp;
    //    return functionReturnValue;
    //    //Debug.Print "comp_ncp_t_nc1", comp_ncp_t_nc1
    //}

    //protected internal static double pdf_t_nc(double x, double df, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculate pdf of noncentral t
    //    //// Deliberately set not to calculate when x and nc_param have opposite signs as the algorithm used is prone to cancellation error in these circumstances.
    //    //// The user can access t_nc1,comp_t_nc1 directly and check on the accuracy of the results, if required
    //    double nc_derivative = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((x < 0.0) & (nc_param <= 0.0)) {
    //        functionReturnValue = pdf_t_nc(-x, df, -nc_param);
    //    } else if ((df <= 0.0) | (nc_param < 0.0) | (nc_param > Math.Sqrt(2.0 * nc_limit))) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x == 0.0 | nc_param == 0.0)) {
    //        functionReturnValue = Math.Exp(-Math.Pow(nc_param, 2) / 2) * pdftdist(x, df);
    //    } else {
    //        if ((df < 1.0 | x < 1.0 | x <= nc_param)) {
    //            functionReturnValue = t_nc1(x, df, nc_param, ref nc_derivative);
    //        } else {
    //            functionReturnValue = comp_t_nc1(x, df, nc_param, ref nc_derivative);
    //        }
    //        if (nc_derivative < cSmall) {
    //            functionReturnValue = Math.Exp(-Math.Pow(nc_param, 2) / 2) * pdftdist(x, df);
    //        } else if (df > 2.0) {
    //            functionReturnValue = nc_derivative / x;
    //        } else {
    //            functionReturnValue = nc_derivative * (df / (2.0 * x));
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_t_nc(double x, double df, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculate cdf of noncentral t
    //    //// Deliberately set not to calculate when x and nc_param have opposite signs as the algorithm used is prone to cancellation error in these circumstances.
    //    //// The user can access t_nc1,comp_t_nc1 directly and check on the accuracy of the results, if required
    //    double tdistDensity = 0;
    //    double nc_derivative = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((nc_param == 0.0)) {
    //        functionReturnValue = tdist(x, df, tdistDensity);
    //    } else if ((x <= 0.0) & (nc_param < 0.0)) {
    //        functionReturnValue = comp_cdf_t_nc(-x, df, -nc_param);
    //    } else if ((df <= 0.0) | (nc_param < 0.0) | (nc_param > Math.Sqrt(2.0 * nc_limit))) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((df < 1.0 | x < 1.0 | x <= nc_param)) {
    //        functionReturnValue = t_nc1(x, df, nc_param, ref nc_derivative);
    //    } else {
    //        functionReturnValue = 1.0 - comp_t_nc1(x, df, nc_param, ref nc_derivative);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_t_nc(double x, double df, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    //// Calculate 1-cdf of noncentral t
    //    //// Deliberately set not to calculate when x and nc_param have opposite signs as the algorithm used is prone to cancellation error in these circumstances.
    //    //// The user can access t_nc1,comp_t_nc1 directly and check on the accuracy of the results, if required
    //    double tdistDensity = 0;
    //    double nc_derivative = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((nc_param == 0.0)) {
    //        functionReturnValue = tdist(-x, df, tdistDensity);
    //    } else if ((x <= 0.0) & (nc_param < 0.0)) {
    //        functionReturnValue = cdf_t_nc(-x, df, -nc_param);
    //    } else if ((df <= 0.0) | (nc_param < 0.0) | (nc_param > Math.Sqrt(2.0 * nc_limit))) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((x < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((df < 1.0 | x < 1.0 | x >= nc_param)) {
    //        functionReturnValue = comp_t_nc1(x, df, nc_param, ref nc_derivative);
    //    } else {
    //        functionReturnValue = 1.0 - t_nc1(x, df, nc_param, ref nc_derivative);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double inv_t_nc(double prob, double df, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((nc_param == 0.0)) {
    //        functionReturnValue = invtdist(prob, df);
    //    } else if ((nc_param < 0.0)) {
    //        functionReturnValue = -comp_inv_t_nc(prob, df, -nc_param);
    //    } else if ((df <= 0.0 | nc_param > Math.Sqrt(2.0 * nc_limit) | prob <= 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((invcnormal(prob) < -nc_param)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double oneMinusP = 0;
    //        functionReturnValue = inv_t_nc1(prob, df, nc_param, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_t_nc(double prob, double df, double nc_param)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((nc_param == 0.0)) {
    //        functionReturnValue = -invtdist(prob, df);
    //    } else if ((nc_param < 0.0)) {
    //        functionReturnValue = -inv_t_nc(prob, df, -nc_param);
    //    } else if ((df <= 0.0 | nc_param > Math.Sqrt(2.0 * nc_limit) | prob <= 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((invcnormal(prob) > nc_param)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double oneMinusP = 0;
    //        functionReturnValue = comp_inv_t_nc1(prob, df, nc_param, ref oneMinusP);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double ncp_t_nc(double prob, double x, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((x == 0.0 & prob > 0.5)) {
    //        functionReturnValue = -invcnormal(prob);
    //    } else if ((x < 0)) {
    //        functionReturnValue = -comp_ncp_t_nc(prob, -x, df);
    //    } else if ((df <= 0.0 | prob <= 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = ncp_t_nc1(prob, x, df);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_ncp_t_nc(double prob, double x, double df)
    //{
    //    double functionReturnValue = 0;
    //    df = AlterForIntegralChecks_df(df);
    //    if ((x == 0.0)) {
    //        functionReturnValue = invcnormal(prob);
    //    } else if ((x < 0)) {
    //        functionReturnValue = -ncp_t_nc(prob, -x, df);
    //    } else if ((df <= 0.0 | prob <= 0.0 | prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = comp_ncp_t_nc1(prob, x, df);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pmf_GammaPoisson(double i, double gamma_shape, double gamma_scale)
    //{
    //    double functionReturnValue = 0;
    //    double p = 0;
    //    double Q = 0;
    //    double dfm = 0;
    //    Q = gamma_scale / (1.0 + gamma_scale);
    //    p = 1.0 / (1.0 + gamma_scale);
    //    i = AlterForIntegralChecks_Others(i);
    //    if ((gamma_shape <= 0.0 | gamma_scale <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((i < 0.0)) {
    //        functionReturnValue = 0;
    //    } else {
    //        if (p < Q) {
    //            dfm = gamma_shape - (gamma_shape + i) * p;
    //        } else {
    //            dfm = (gamma_shape + i) * Q - i;
    //        }
    //        functionReturnValue = (gamma_shape / (gamma_shape + i)) * binomialTerm(i, gamma_shape, Q, p, dfm, 0.0);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_GammaPoisson(double i, double gamma_shape, double gamma_scale)
    //{
    //    double functionReturnValue = 0;
    //    double p = 0;
    //    double Q = 0;
    //    Q = gamma_scale / (1.0 + gamma_scale);
    //    p = 1.0 / (1.0 + gamma_scale);
    //    i = Convert.ToInt32(i);
    //    if ((gamma_shape <= 0.0 | gamma_scale <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((i < 0.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((p <= Q)) {
    //        functionReturnValue = BETA(p, gamma_shape, i + 1.0);
    //    } else {
    //        functionReturnValue = compbeta(Q, i + 1.0, gamma_shape);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_GammaPoisson(double i, double gamma_shape, double gamma_scale)
    //{
    //    double functionReturnValue = 0;
    //    double p = 0;
    //    double Q = 0;
    //    Q = gamma_scale / (1.0 + gamma_scale);
    //    p = 1.0 / (1.0 + gamma_scale);
    //    i = Convert.ToInt32(i);
    //    if ((gamma_shape <= 0.0 | gamma_scale <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((i < 0.0)) {
    //        functionReturnValue = 1.0;
    //    } else if ((p <= Q)) {
    //        functionReturnValue = compbeta(p, gamma_shape, i + 1.0);
    //    } else {
    //        functionReturnValue = BETA(Q, i + 1.0, gamma_shape);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double crit_GammaPoisson(double gamma_shape, double gamma_scale, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    double p = 0;
    //    double Q = 0;
    //    Q = gamma_scale / (1.0 + gamma_scale);
    //    p = 1.0 / (1.0 + gamma_scale);
    //    if ((gamma_shape < 0.0 | gamma_scale < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob < 0.0 | crit_prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double i = 0;
    //        double pr = 0;
    //        functionReturnValue = critnegbinom(gamma_shape, p, Q, crit_prob);
    //        i = functionReturnValue;
    //        if (p <= Q) {
    //            pr = BETA(p, gamma_shape, i + 1.0);
    //        } else {
    //            pr = compbeta(Q, i + 1.0, gamma_shape);
    //        }
    //        if ((pr == crit_prob)) {
    //        } else if ((pr > crit_prob)) {
    //            i = i - 1.0;
    //            if (p <= Q) {
    //                pr = BETA(p, gamma_shape, i + 1.0);
    //            } else {
    //                pr = compbeta(Q, i + 1.0, gamma_shape);
    //            }
    //            if ((pr >= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_crit_GammaPoisson(double gamma_shape, double gamma_scale, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    double p = 0;
    //    double Q = 0;
    //    Q = gamma_scale / (1.0 + gamma_scale);
    //    p = 1.0 / (1.0 + gamma_scale);
    //    if ((gamma_shape < 0.0 | gamma_scale < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob <= 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        double i = 0;
    //        double pr = 0;
    //        functionReturnValue = critcompnegbinom(gamma_shape, p, Q, crit_prob);
    //        i = functionReturnValue;
    //        if (p <= Q) {
    //            pr = compbeta(p, gamma_shape, i + 1.0);
    //        } else {
    //            pr = BETA(Q, i + 1.0, gamma_shape);
    //        }
    //        if ((pr == crit_prob)) {
    //        } else if ((pr < crit_prob)) {
    //            i = i - 1.0;
    //            if (p <= Q) {
    //                pr = compbeta(p, gamma_shape, i + 1.0);
    //            } else {
    //                pr = BETA(Q, i + 1.0, gamma_shape);
    //            }
    //            if ((pr <= crit_prob)) {
    //                functionReturnValue = i;
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //        }
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //private static  double PBB(double i, double ssmi, double BETA_shape1, double BETA_shape2)
    //{
    //    return (BETA_shape1 / (i + BETA_shape1)) * (BETA_shape2 / (BETA_shape1 + BETA_shape2)) * ((i + ssmi + BETA_shape1 + BETA_shape2) / (ssmi + BETA_shape2)) * hypergeometricTerm(i, ssmi, BETA_shape1, BETA_shape2);
    //}

    //protected internal static double pmf_BetaNegativeBinomial(double i, double r, double BETA_shape1, double BETA_shape2)
    //{
    //    double functionReturnValue = 0;
    //    i = AlterForIntegralChecks_Others(i);
    //    if ((r <= 0.0 | BETA_shape1 <= 0.0 | BETA_shape2 <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if (i < 0) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = (BETA_shape2 / (BETA_shape1 + BETA_shape2)) * (r / (BETA_shape1 + r)) * BETA_shape1 * (i + BETA_shape1 + r + BETA_shape2) / ((i + r) * (i + BETA_shape2)) * hypergeometricTerm(i, r, BETA_shape2, BETA_shape1);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //private static  double CBNB0(double i, double r, double BETA_shape1, double BETA_shape2, double toBeAdded)
    //{
    //    double functionReturnValue = 0;
    //    double ha1 = 0;
    //    double hprob = 0;
    //    bool hswap = false;
    //    double mrb2 = 0;
    //    double other = 0;
    //    double temp = 0;
    //    if ((r < 2.0 | BETA_shape2 < 2.0)) {
    //        //Assumption here that i is integral or greater than 4.
    //        mrb2 = max(r, BETA_shape2);
    //        other = Min(r, BETA_shape2);
    //        functionReturnValue = PBB(i, other, mrb2, BETA_shape1);
    //        if (i == 0.0)
    //            return functionReturnValue;
    //        functionReturnValue = functionReturnValue * (1.0 + i * (other + BETA_shape1) / (((i - 1.0) + mrb2) * (other + 1.0)));
    //        if (i == 1.0)
    //            return functionReturnValue;
    //        i = i - 2.0;
    //        other = other + 2.0;
    //        temp = PBB(i, mrb2, other, BETA_shape1);
    //        if (i == 0.0) {
    //            functionReturnValue = functionReturnValue + temp;
    //            return functionReturnValue;
    //        }
    //        functionReturnValue = functionReturnValue + temp * (1.0 + i * (mrb2 + BETA_shape1) / (((i - 1.0) + other) * (mrb2 + 1.0)));
    //        if (i == 1.0)
    //            return functionReturnValue;
    //        i = i - 2.0;
    //        mrb2 = mrb2 + 2.0;
    //        functionReturnValue = functionReturnValue + CBNB0(i, mrb2, BETA_shape1, other, functionReturnValue);
    //    } else if ((BETA_shape1 < 1.0)) {
    //        mrb2 = max(r, BETA_shape2);
    //        other = Min(r, BETA_shape2);
    //        functionReturnValue = hypergeometric(i, mrb2 - 1.0, other, BETA_shape1, false, ref ha1, ref hprob, ref hswap);
    //        if (hswap) {
    //            temp = PBB(mrb2 - 1.0, BETA_shape1, i + 1.0, other);
    //            if ((toBeAdded + (functionReturnValue - temp)) < 0.1 * (toBeAdded + (functionReturnValue + temp))) {
    //                functionReturnValue = CBNB2(i, mrb2, BETA_shape1, other);
    //            } else {
    //                functionReturnValue = functionReturnValue - temp;
    //            }
    //        } else if (ha1 < -0.9 * BETA_shape1 / (BETA_shape1 + other)) {
    //            functionReturnValue = ErrorValue;
    //        } else {
    //            functionReturnValue = hprob * (BETA_shape1 / (BETA_shape1 + other) + ha1);
    //        }
    //    } else {
    //        functionReturnValue = hypergeometric(i, r, BETA_shape2, BETA_shape1 - 1.0, false, ref ha1, ref hprob, ref hswap);
    //    }
    //    return functionReturnValue;
    //}

    //private static  double CBNB2(double i, double r, double BETA_shape1, double BETA_shape2)
    //{
    //    double functionReturnValue = 0;
    //    double j = 0;
    //    double ss = 0;
    //    double bs2 = 0;
    //    double temp = 0;
    //    double d1 = 0;
    //    double d2 = 0;
    //    double d_count = 0;
    //    double pbbval = 0;
    //    //In general may be a good idea to take Min(i, BETA_shape1) down to just above 0 and then work on Max(i, BETA_shape1)
    //    ss = Min(r, BETA_shape2);
    //    bs2 = max(r, BETA_shape2);
    //    r = ss;
    //    BETA_shape2 = bs2;
    //    d1 = (i + 0.5) * (BETA_shape1 + 0.5) - (bs2 - 1.5) * (ss - 0.5);
    //    if (d1 < 0.0) {
    //        functionReturnValue = CBNB0(i, ss, BETA_shape1, bs2, 0.0);
    //        return functionReturnValue;
    //    }
    //    d1 = Convert.ToInt32(d1 / (bs2 + BETA_shape1)) + 1.0;
    //    if (ss + d1 > bs2)
    //        d1 = Convert.ToInt32(bs2 - ss);
    //    ss = ss + d1;
    //    j = i - d1;
    //    d2 = (j + 0.5) * (BETA_shape1 + 0.5) - (bs2 - 1.5) * (ss - 0.5);
    //    if (d2 < 0.0) {
    //        d2 = 2.0;
    //    } else {
    //        //Could make this smaller
    //        d2 = Convert.ToInt32(Math.Sqrt(d2)) + 2.0;
    //    }
    //    if (2.0 * d2 > i) {
    //        d2 = Convert.ToInt32(i / 2.0);
    //    }
    //    pbbval = PBB(i, r, BETA_shape2, BETA_shape1);
    //    ss = ss + d2;
    //    bs2 = bs2 + d2;
    //    j = j - 2.0 * d2;
    //    functionReturnValue = CBNB0(j, ss, BETA_shape1, bs2, 0.0);
    //    temp = 1.0;
    //    d_count = d2 - 2.0;
    //    j = j + 1.0;
    //    while (d_count >= 0.0) {
    //        j = j + 1.0;
    //        bs2 = BETA_shape2 + d_count;
    //        d_count = d_count - 1.0;
    //        temp = 1.0 + (j * (bs2 + BETA_shape1) / ((j + ss - 1.0) * (bs2 + 1.0))) * temp;
    //    }
    //    j = i - d2 - d1;
    //    temp = (ss * (j + bs2)) / (bs2 * (j + ss)) * temp;
    //    d_count = d1 + d2 - 1.0;
    //    while (d_count >= 0) {
    //        j = j + 1.0;
    //        ss = r + d_count;
    //        d_count = d_count - 1.0;
    //        temp = 1.0 + (j * (ss + BETA_shape1) / ((j + bs2 - 1.0) * (ss + 1.0))) * temp;
    //    }
    //    functionReturnValue = functionReturnValue + temp * pbbval;
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_BetaNegativeBinomial(double i, double r, double BETA_shape1, double BETA_shape2)
    //{
    //    double functionReturnValue = 0;
    //    i = Convert.ToInt32(i);
    //    if ((r <= 0.0 | BETA_shape1 <= 0.0 | BETA_shape2 <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if (i < 0) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = CBNB0(i, r, BETA_shape1, BETA_shape2, 0.0);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_BetaNegativeBinomial(double i, double r, double BETA_shape1, double BETA_shape2)
    //{
    //    double functionReturnValue = 0;
    //    double ha1 = 0;
    //    double hprob = 0;
    //    bool hswap = false;
    //    double mrb2 = 0;
    //    double other = 0;
    //    double temp = 0;
    //    double CTEMP = 0;
    //    i = Convert.ToInt32(i);
    //    mrb2 = max(r, BETA_shape2);
    //    other = Min(r, BETA_shape2);
    //    if ((other <= 0.0 | BETA_shape1 <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if (i < 0.0) {
    //        functionReturnValue = 1.0;
    //    } else if ((mrb2 > 100.0)) {
    //        functionReturnValue = CBNB0(mrb2 - 1.0, i + 1.0, other, BETA_shape1, 0.0);
    //    } else if ((other < 1.0 | BETA_shape1 < 1.0)) {
    //        if ((i + BETA_shape1) < 60.0) {
    //            i = i + 1.0;
    //            temp = pmf_BetaNegativeBinomial(i, r, BETA_shape1, BETA_shape2);
    //            CTEMP = temp;
    //            while ((i + BETA_shape1) < 60.0) {
    //                temp = temp * (i + r) * (i + BETA_shape2) / ((i + 1.0) * (i + BETA_shape1 + r + BETA_shape2));
    //                CTEMP = CTEMP + temp;
    //                i = i + 1.0;
    //            }
    //        } else {
    //            CTEMP = 0.0;
    //        }
    //        if (other >= 1.0) {
    //            functionReturnValue = hypergeometric(mrb2, i, BETA_shape1, other - 1.0, false, ref ha1, ref hprob, ref hswap);
    //        } else {
    //            functionReturnValue = hypergeometric(mrb2, i, BETA_shape1, -other, false, ref ha1, ref hprob, ref hswap);
    //        }
    //        if (hswap) {
    //            temp = PBB(i, mrb2, other, BETA_shape1);
    //            //N.B. subtraction of PBB term can be done exactly from hypergeometric one if hswap false
    //            if (temp > 0.9 * functionReturnValue) {
    //                functionReturnValue = ErrorValue;
    //            } else {
    //                functionReturnValue = (functionReturnValue - temp) + CTEMP;
    //            }
    //        } else {
    //            if (ha1 < -0.9 * mrb2 / (BETA_shape1 + mrb2)) {
    //                functionReturnValue = ErrorValue;
    //            } else {
    //                functionReturnValue = hprob * (mrb2 / (BETA_shape1 + mrb2) + ha1) + CTEMP;
    //            }
    //        }
    //    } else {
    //        functionReturnValue = hypergeometric(i, r, BETA_shape2, BETA_shape1 - 1.0, true, ref ha1, ref hprob, ref hswap);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //private static  double critbetanegbinomial(double a, double B, double r, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that Pr(betanegbinomial(i,r,a,b)) >= cprob and  Pr(betanegbinomial(i-1,r,a,b)) < cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critcompbetanegbinomial(a, B, r, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double i = 0;
    //    double temp = 0;
    //    double temp2 = 0;
    //    double oneMinusP = 0;
    //    if (B > r) {
    //        i = B;
    //        B = r;
    //        r = i;
    //    }
    //    if ((a < 10.0 | B < 10.0)) {
    //        if (r < a & a < 1.0) {
    //            pr = cprob * a / r;
    //        } else {
    //            pr = cprob;
    //        }
    //        i = invcompbeta(a, B, pr, ref oneMinusP);
    //    } else {
    //        pr = r / (r + a + B - 1.0);
    //        i = invcompbeta(a * pr, B * pr, cprob, ref oneMinusP);
    //    }
    //    if (i == 0.0) {
    //        i = max_crit / 2.0;
    //    } else {
    //        i = r * (oneMinusP / i);
    //        if (i >= max_crit) {
    //            i = max_crit - 1.0;
    //        }
    //    }
    //    while ((true)) {
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        }
    //        i = Convert.ToInt32(i + 0.5);
    //        if ((i >= max_crit)) {
    //            functionReturnValue = ErrorValue;
    //            return functionReturnValue;
    //        }
    //        pr = CBNB0(i, r, a, B, 0.0);
    //        tpr = 0.0;
    //        if ((pr > cprob * (1 + cfSmall))) {
    //            if ((i == 0.0)) {
    //                functionReturnValue = 0.0;
    //                return functionReturnValue;
    //            }
    //            tpr = pmf_BetaNegativeBinomial(i, r, a, B);
    //            if ((pr < (1.0 + 1E-05) * tpr)) {
    //                tpr = tpr * (((i + 1.0) * (i + a + r + B)) / ((i + r) * (i + B)));
    //                i = i - 1.0;
    //                while ((tpr > cprob)) {
    //                    tpr = tpr * (((i + 1.0) * (i + a + r + B)) / ((i + r) * (i + B)));
    //                    i = i - 1.0;
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr < cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                i = i - 1.0;
    //                if ((i == 0.0)) {
    //                    functionReturnValue = 0.0;
    //                    return functionReturnValue;
    //                }
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10.0)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    if (temp > i) {
    //                        i = i / 10.0;
    //                    } else {
    //                        i = Convert.ToInt32(i - temp);
    //                        temp2 = pmf_BetaNegativeBinomial(i, r, a, B);
    //                        i = i - temp * (tpr - temp2) / (2.0 * temp2);
    //                    }
    //                } else {
    //                    tpr = tpr * (((i + 1.0) * (i + a + r + B)) / ((i + r) * (i + B)));
    //                    pr = pr - tpr;
    //                    if ((pr < cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    i = i - 1.0;
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr >= cprob)) {
    //                            tpr = tpr * (((i + 1.0) * (i + a + r + B)) / ((i + r) * (i + B)));
    //                            pr = pr - tpr;
    //                            i = i - 1.0;
    //                        }
    //                        functionReturnValue = i + 1.0;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log((((i + 1.0) * (i + a + r + B)) / ((i + r) * (i + B)))) + 0.5);
    //                        i = i - temp;
    //                        temp2 = pmf_BetaNegativeBinomial(i, r, a, B);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log((((i + 1.0) * (i + a + r + B)) / ((i + r) * (i + B))));
    //                            i = i - temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else if (((1.0 + cfSmall) * pr < cprob)) {
    //            while (((tpr < cSmall) & (pr < cprob))) {
    //                i = i + 1.0;
    //                tpr = pmf_BetaNegativeBinomial(i, r, a, B);
    //                pr = pr + tpr;
    //            }
    //            temp = (cprob - pr) / tpr;
    //            if (temp <= 0.0) {
    //                functionReturnValue = i;
    //                return functionReturnValue;
    //            } else if (temp < 10.0) {
    //                while ((pr < cprob)) {
    //                    i = i + 1.0;
    //                    tpr = tpr * (((i + r - 1.0) * (i + B - 1.0)) / (i * (i + a + r + B - 1.0)));
    //                    pr = pr + tpr;
    //                }
    //                functionReturnValue = i;
    //                return functionReturnValue;
    //            } else if (i + temp > max_crit) {
    //                functionReturnValue = ErrorValue;
    //                return functionReturnValue;
    //            } else {
    //                i = Convert.ToInt32(i + temp);
    //                temp2 = pmf_BetaNegativeBinomial(i, r, a, B);
    //                if (temp2 > 0.0)
    //                    i = i + temp * (tpr - temp2) / (2.0 * temp2);
    //            }
    //        } else {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critcompbetanegbinomial(double a, double B, double r, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that 1-Pr(betanegbinomial(i,r,a,b)) > cprob and  1-Pr(betanegbinomial(i-1,r,a,b)) <= cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critbetanegbinomial(a, B, r, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double i = 0;
    //    double temp = 0;
    //    double temp2 = 0;
    //    double oneMinusP = 0;
    //    if (B > r) {
    //        i = B;
    //        B = r;
    //        r = i;
    //    }
    //    if ((a < 10.0 | B < 10.0)) {
    //        if (r < a & a < 1.0) {
    //            pr = cprob * a / r;
    //        } else {
    //            pr = cprob;
    //        }
    //        i = invbeta(a, B, pr, ref oneMinusP);
    //    } else {
    //        pr = r / (r + a + B - 1.0);
    //        i = invbeta(a * pr, B * pr, cprob, ref oneMinusP);
    //    }
    //    if (i == 0.0) {
    //        i = max_crit / 2.0;
    //    } else {
    //        i = r * (oneMinusP / i);
    //        if (i >= max_crit) {
    //            i = max_crit - 1.0;
    //        }
    //    }
    //    while ((true)) {
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        }
    //        i = Convert.ToInt32(i + 0.5);
    //        if ((i >= max_crit)) {
    //            functionReturnValue = ErrorValue;
    //            return functionReturnValue;
    //        }
    //        pr = comp_cdf_BetaNegativeBinomial(i, r, a, B);
    //        tpr = 0.0;
    //        if ((pr > cprob * (1 + cfSmall))) {
    //            i = i + 1.0;
    //            tpr = pmf_BetaNegativeBinomial(i, r, a, B);
    //            if ((pr < (1 + 1E-05) * tpr)) {
    //                while ((tpr > cprob)) {
    //                    i = i + 1.0;
    //                    tpr = tpr * (((i + r - 1.0) * (i + B - 1.0)) / (i * (i + a + r + B - 1.0)));
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr <= cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10.0)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i + temp;
    //                    temp2 = pmf_BetaNegativeBinomial(i, r, a, B);
    //                    i = i + temp * (tpr - temp2) / (2.0 * temp2);
    //                } else {
    //                    i = i + 1.0;
    //                    tpr = tpr * (((i + r - 1.0) * (i + B - 1.0)) / (i * (i + a + r + B - 1.0)));
    //                    pr = pr - tpr;
    //                    if ((pr <= cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr > cprob)) {
    //                            i = i + 1.0;
    //                            tpr = tpr * (((i + r - 1.0) * (i + B - 1.0)) / (i * (i + a + r + B - 1.0)));
    //                            pr = pr - tpr;
    //                        }
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log((((i + r - 1.0) * (i + B - 1.0)) / (i * (i + a + r + B - 1.0)))) + 0.5);
    //                        i = i + temp;
    //                        temp2 = pmf_BetaNegativeBinomial(i, r, a, B);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log((((i + r - 1.0) * (i + B - 1.0)) / (i * (i + a + r + B - 1.0))));
    //                            i = i + temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else if (((1.0 + cfSmall) * pr < cprob)) {
    //            while (((tpr < cSmall) & (pr <= cprob))) {
    //                tpr = pmf_BetaNegativeBinomial(i, r, a, B);
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //            }
    //            temp = (cprob - pr) / tpr;
    //            if (temp <= 0.0) {
    //                functionReturnValue = i + 1.0;
    //                return functionReturnValue;
    //            } else if (temp < 100.0 | i < 1000.0) {
    //                while ((pr <= cprob)) {
    //                    tpr = tpr * (((i + 1.0) * (i + a + r + B)) / ((i + r) * (i + B)));
    //                    pr = pr + tpr;
    //                    i = i - 1.0;
    //                }
    //                functionReturnValue = i + 1.0;
    //                return functionReturnValue;
    //            } else if (temp > i) {
    //                i = i / 10.0;
    //            } else {
    //                i = Convert.ToInt32(i - temp);
    //                temp2 = pmf_BetaNegativeBinomial(i, r, a, B);
    //                if (temp2 > 0.0)
    //                    i = i - temp * (tpr - temp2) / (2.0 * temp2);
    //            }
    //        } else {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //protected internal static double crit_BetaNegativeBinomial(double r, double BETA_shape1, double BETA_shape2, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    if ((BETA_shape1 <= 0.0 | BETA_shape2 <= 0.0 | r <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob < 0.0 | crit_prob >= 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else {
    //        functionReturnValue = critbetanegbinomial(BETA_shape1, BETA_shape2, r, crit_prob);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_crit_BetaNegativeBinomial(double r, double BETA_shape1, double BETA_shape2, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    if ((BETA_shape1 <= 0.0 | BETA_shape2 <= 0.0 | r <= 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob <= 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = critcompbetanegbinomial(BETA_shape1, BETA_shape2, r, crit_prob);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pmf_BetaBinomial(double i, double SAMPLE_SIZE, double BETA_shape1, double BETA_shape2)
    //{
    //    double functionReturnValue = 0;
    //    i = AlterForIntegralChecks_Others(i);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((BETA_shape1 <= 0.0 | BETA_shape2 <= 0.0 | SAMPLE_SIZE < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if (i < 0 | i > SAMPLE_SIZE) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = (BETA_shape1 / (i + BETA_shape1)) * (BETA_shape2 / (BETA_shape1 + BETA_shape2)) * ((SAMPLE_SIZE + BETA_shape1 + BETA_shape2) / (SAMPLE_SIZE - i + BETA_shape2)) * hypergeometricTerm(i, SAMPLE_SIZE - i, BETA_shape1, BETA_shape2);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_BetaBinomial(double i, double SAMPLE_SIZE, double BETA_shape1, double BETA_shape2)
    //{
    //    double functionReturnValue = 0;
    //    i = Convert.ToInt32(i);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((BETA_shape1 <= 0.0 | BETA_shape2 <= 0.0 | SAMPLE_SIZE < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if (i < 0.0) {
    //        functionReturnValue = 0.0;
    //    } else if (i >= SAMPLE_SIZE) {
    //        functionReturnValue = 1.0;
    //    } else {
    //        functionReturnValue = CBNB0(i, SAMPLE_SIZE - i, BETA_shape2, BETA_shape1, 0.0);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_BetaBinomial(double i, double SAMPLE_SIZE, double BETA_shape1, double BETA_shape2)
    //{
    //    double functionReturnValue = 0;
    //    i = Convert.ToInt32(i);
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((BETA_shape1 <= 0.0 | BETA_shape2 <= 0.0 | SAMPLE_SIZE < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if (i < 0.0) {
    //        functionReturnValue = 1.0;
    //    } else if (i >= SAMPLE_SIZE) {
    //        functionReturnValue = 0.0;
    //    } else {
    //        functionReturnValue = CBNB0(SAMPLE_SIZE - i - 1.0, i + 1.0, BETA_shape1, BETA_shape2, 0.0);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //private static  double critbetabinomial(double a, double B, double ss, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that Pr(betabinomial(i,ss,a,b)) >= cprob and  Pr(betabinomial(i-1,ss,a,b)) < cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critcompbetabinomial(a, B, ss, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double i = 0;
    //    double temp = 0;
    //    double temp2 = 0;
    //    double oneMinusP = 0;
    //    if ((a + B < 1.0)) {
    //        i = invbeta(a, B, cprob, ref oneMinusP) * ss;
    //    } else {
    //        pr = ss / (ss + a + B - 1.0);
    //        i = invbeta(a * pr, B * pr, cprob, ref oneMinusP) * ss;
    //    }
    //    while ((true)) {
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        } else if ((i > ss)) {
    //            i = ss;
    //        }
    //        i = Convert.ToInt32(i + 0.5);
    //        if ((i >= max_discrete)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        pr = cdf_BetaBinomial(i, ss, a, B);
    //        tpr = 0.0;
    //        if ((pr >= cprob * (1 + cfSmall))) {
    //            if ((i == 0.0)) {
    //                functionReturnValue = 0.0;
    //                return functionReturnValue;
    //            }
    //            tpr = pmf_BetaBinomial(i, ss, a, B);
    //            if ((pr < (1.0 + 1E-05) * tpr)) {
    //                tpr = tpr * ((i + 1.0) * (ss + B - i - 1.0)) / ((a + i) * (ss - i));
    //                i = i - 1.0;
    //                while ((tpr > cprob)) {
    //                    tpr = tpr * ((i + 1.0) * (ss + B - i - 1.0)) / ((a + i) * (ss - i));
    //                    i = i - 1.0;
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr < cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                i = i - 1.0;
    //                if ((i == 0.0)) {
    //                    functionReturnValue = 0.0;
    //                    return functionReturnValue;
    //                }
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i - temp;
    //                    temp2 = pmf_BetaBinomial(i, ss, a, B);
    //                    i = i - temp * (tpr - temp2) / (2.0 * temp2);
    //                } else {
    //                    tpr = tpr * ((i + 1.0) * (ss + B - i - 1.0)) / ((a + i) * (ss - i));
    //                    pr = pr - tpr;
    //                    if ((pr < cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    i = i - 1.0;
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr >= cprob)) {
    //                            tpr = tpr * ((i + 1.0) * (ss + B - i - 1.0)) / ((a + i) * (ss - i));
    //                            pr = pr - tpr;
    //                            i = i - 1.0;
    //                        }
    //                        functionReturnValue = i + 1.0;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log(((i + 1.0) * (ss + B - i - 1.0)) / ((a + i) * (ss - i))) + 0.5);
    //                        i = i - temp;
    //                        temp2 = pmf_BetaBinomial(i, ss, a, B);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((i + 1.0) * (ss + B - i - 1.0)) / ((a + i) * (ss - i)));
    //                            i = i - temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else if (((1.0 + cfSmall) * pr < cprob)) {
    //            while (((tpr < cSmall) & (pr < cprob))) {
    //                i = i + 1.0;
    //                tpr = pmf_BetaBinomial(i, ss, a, B);
    //                pr = pr + tpr;
    //            }
    //            temp = (cprob - pr) / tpr;
    //            if (temp <= 0.0) {
    //                functionReturnValue = i;
    //                return functionReturnValue;
    //            } else if (temp < 10.0) {
    //                while ((pr < cprob)) {
    //                    i = i + 1.0;
    //                    tpr = tpr * ((a + i - 1.0) * (ss - i + 1.0)) / (i * (ss + B - i));
    //                    pr = pr + tpr;
    //                }
    //                functionReturnValue = i;
    //                return functionReturnValue;
    //            } else if (temp > 4E+15) {
    //                i = 4E+15;
    //            } else {
    //                i = Convert.ToInt32(i + temp);
    //                temp2 = pmf_BetaBinomial(i, ss, a, B);
    //                if (temp2 > 0.0)
    //                    i = i + temp * (tpr - temp2) / (2.0 * temp2);
    //            }
    //        } else {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //private static  double critcompbetabinomial(double a, double B, double ss, double cprob)
    //{
    //    double functionReturnValue = 0;
    //    ////i such that 1-Pr(betabinomial(i,ss,a,b)) > cprob and  1-Pr(betabinomial(i-1,ss,a,b)) <= cprob
    //    if ((cprob > 0.5)) {
    //        functionReturnValue = critbetabinomial(a, B, ss, 1.0 - cprob);
    //        return functionReturnValue;
    //    }
    //    double pr = 0;
    //    double tpr = 0;
    //    double i = 0;
    //    double temp = 0;
    //    double temp2 = 0;
    //    double oneMinusP = 0;
    //    if ((a + B < 1.0)) {
    //        i = invcompbeta(a, B, cprob, ref oneMinusP) * ss;
    //    } else {
    //        pr = ss / (ss + a + B - 1.0);
    //        i = invcompbeta(a * pr, B * pr, cprob, ref oneMinusP) * ss;
    //    }
    //    while ((true)) {
    //        if ((i < 0.0)) {
    //            i = 0.0;
    //        } else if ((i > ss)) {
    //            i = ss;
    //        }
    //        i = Convert.ToInt32(i + 0.5);
    //        if ((i >= max_discrete)) {
    //            functionReturnValue = i;
    //            return functionReturnValue;
    //        }
    //        pr = comp_cdf_BetaBinomial(i, ss, a, B);
    //        tpr = 0.0;
    //        if ((pr >= cprob * (1 + cfSmall))) {
    //            i = i + 1.0;
    //            tpr = pmf_BetaBinomial(i, ss, a, B);
    //            if ((pr < (1 + 1E-05) * tpr)) {
    //                while ((tpr > cprob)) {
    //                    i = i + 1.0;
    //                    temp = ss + B - i;
    //                    if (temp == 0.0)
    //                        break; // TODO: might not be correct. Was : Exit Do
    //                    tpr = tpr * ((a + i - 1.0) * (ss - i + 1.0)) / (i * temp);
    //                }
    //            } else {
    //                pr = pr - tpr;
    //                if ((pr <= cprob)) {
    //                    functionReturnValue = i;
    //                    return functionReturnValue;
    //                }
    //                temp = (pr - cprob) / tpr;
    //                if ((temp > 10.0)) {
    //                    temp = Convert.ToInt32(temp + 0.5);
    //                    i = i + temp;
    //                    temp2 = pmf_BetaBinomial(i, ss, a, B);
    //                    i = i + temp * (tpr - temp2) / (2.0 * temp2);
    //                } else {
    //                    i = i + 1.0;
    //                    tpr = tpr * ((a + i - 1.0) * (ss - i + 1.0)) / (i * (ss + B - i));
    //                    pr = pr - tpr;
    //                    if ((pr <= cprob)) {
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    }
    //                    temp2 = (pr - cprob) / tpr;
    //                    if ((temp2 < temp - 0.9)) {
    //                        while ((pr > cprob)) {
    //                            i = i + 1.0;
    //                            tpr = tpr * ((a + i - 1.0) * (ss - i + 1.0)) / (i * (ss + B - i));
    //                            pr = pr - tpr;
    //                        }
    //                        functionReturnValue = i;
    //                        return functionReturnValue;
    //                    } else {
    //                        temp = Convert.ToInt32(Math.Log(cprob / pr) / Math.Log(((a + i - 1.0) * (ss - i + 1.0)) / (i * (ss + B - i))) + 0.5);
    //                        i = i + temp;
    //                        temp2 = pmf_BetaBinomial(i, ss, a, B);
    //                        if ((temp2 > nearly_zero)) {
    //                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((a + i - 1.0) * (ss - i + 1.0)) / (i * (ss + B - i)));
    //                            i = i + temp;
    //                        }
    //                    }
    //                }
    //            }
    //        } else if (((1.0 + cfSmall) * pr < cprob)) {
    //            while (((tpr < cSmall) & (pr <= cprob))) {
    //                tpr = pmf_BetaBinomial(i, ss, a, B);
    //                pr = pr + tpr;
    //                i = i - 1.0;
    //            }
    //            temp = (cprob - pr) / tpr;
    //            if (temp <= 0.0) {
    //                functionReturnValue = i + 1.0;
    //                return functionReturnValue;
    //            } else if (temp < 100.0 | i < 1000.0) {
    //                while ((pr <= cprob)) {
    //                    tpr = tpr * ((i + 1.0) * (ss + B - i - 1.0)) / ((a + i) * (ss - i));
    //                    pr = pr + tpr;
    //                    i = i - 1.0;
    //                }
    //                functionReturnValue = i + 1.0;
    //                return functionReturnValue;
    //            } else if (temp > i) {
    //                i = i / 10.0;
    //            } else {
    //                i = Convert.ToInt32(i - temp);
    //                temp2 = pmf_BetaNegativeBinomial(i, ss, a, B);
    //                if (temp2 > 0.0)
    //                    i = i - temp * (tpr - temp2) / (2.0 * temp2);
    //            }
    //        } else {
    //            functionReturnValue = i + 1.0;
    //            return functionReturnValue;
    //        }
    //    }
    //}

    //protected internal static double crit_BetaBinomial(double SAMPLE_SIZE, double BETA_shape1, double BETA_shape2, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((BETA_shape1 <= 0.0 | BETA_shape2 <= 0.0 | SAMPLE_SIZE < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((SAMPLE_SIZE == 0.0 | crit_prob == 1.0)) {
    //        functionReturnValue = SAMPLE_SIZE;
    //    } else {
    //        functionReturnValue = critbetabinomial(BETA_shape1, BETA_shape2, SAMPLE_SIZE, crit_prob);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double comp_crit_BetaBinomial(double SAMPLE_SIZE, double BETA_shape1, double BETA_shape2, double crit_prob)
    //{
    //    double functionReturnValue = 0;
    //    SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE);
    //    if ((BETA_shape1 <= 0.0 | BETA_shape2 <= 0.0 | SAMPLE_SIZE < 0.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob < 0.0 | crit_prob > 1.0)) {
    //        functionReturnValue = ErrorValue;
    //    } else if ((crit_prob == 1.0)) {
    //        functionReturnValue = 0.0;
    //    } else if ((SAMPLE_SIZE == 0.0 | crit_prob == 0.0)) {
    //        functionReturnValue = SAMPLE_SIZE;
    //    } else {
    //        functionReturnValue = critcompbetabinomial(BETA_shape1, BETA_shape2, SAMPLE_SIZE, crit_prob);
    //    }
    //    functionReturnValue = GetRidOfMinusZeroes(functionReturnValue);
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_normal_os(double x, double N = 1, double r = -1)
    //{
    //    double functionReturnValue = 0;
    //    // pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid N(0,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    if (x <= 0) {
    //        functionReturnValue = pdf_BETA(cnormal(x), n1 + r, -r) * pdf_normal(x);
    //    } else {
    //        functionReturnValue = pdf_BETA(cnormal(-x), -r, n1 + r) * pdf_normal(-x);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_normal_os(double x, double N = 1, double r = -1)
    //{
    //    double functionReturnValue = 0;
    //    // cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid N(0,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    if (x <= 0) {
    //        functionReturnValue = cdf_BETA(cnormal(x), n1 + r, -r);
    //    } else {
    //        functionReturnValue = comp_cdf_BETA(cnormal(-x), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_normal_os(double x, double N = 1, double r = -1)
    //{
    //    double functionReturnValue = 0;
    //    // 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid N(0,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    if (x <= 0) {
    //        functionReturnValue = comp_cdf_BETA(cnormal(x), n1 + r, -r);
    //    } else {
    //        functionReturnValue = cdf_BETA(cnormal(-x), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double inv_normal_os(double p, double N = 1, double r = -1)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of cdf_normal_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    // accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invbeta(n1 + r, -r, p, ref oneMinusxp);
    //    if (Math.Abs(xp - 0.5) < 1E-14 & xp != 0.5)
    //        if (cdf_BETA(0.5, n1 + r, -r) == p){functionReturnValue = 0;return functionReturnValue;}
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_normal(xp);
    //    } else {
    //        functionReturnValue = -inv_normal(oneMinusxp);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_normal_os(double p, double N = 1, double r = -1)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of comp_cdf_normal_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invcompbeta(n1 + r, -r, p, ref oneMinusxp);
    //    if (Math.Abs(xp - 0.5) < 1E-14 & xp != 0.5)
    //        if (comp_cdf_BETA(0.5, n1 + r, -r) == p){functionReturnValue = 0;return functionReturnValue;}
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_normal(xp);
    //    } else {
    //        functionReturnValue = -inv_normal(oneMinusxp);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_gamma_os(double x, double shape_param, double N = 1, double r = -1, double scale_param = 1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_gamma_nc(x / scale_param, shape_param, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = pdf_BETA(p, n1 + r, -r) * pdf_gamma_nc(x / scale_param, shape_param, nc_param) / scale_param;
    //    } else {
    //        functionReturnValue = pdf_BETA(comp_cdf_gamma_nc(x / scale_param, shape_param, nc_param), -r, n1 + r) * pdf_gamma_nc(x / scale_param, shape_param, nc_param) / scale_param;
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_gamma_os(double x, double shape_param, double N = 1, double r = -1, double scale_param = 1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_gamma_nc(x / scale_param, shape_param, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = cdf_BETA(p, n1 + r, -r);
    //    } else {
    //        functionReturnValue = comp_cdf_BETA(comp_cdf_gamma_nc(x / scale_param, shape_param, nc_param), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_gamma_os(double x, double shape_param, double N = 1, double r = -1, double scale_param = 1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_gamma_nc(x / scale_param, shape_param, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = comp_cdf_BETA(p, n1 + r, -r);
    //    } else {
    //        functionReturnValue = cdf_BETA(comp_cdf_gamma_nc(x / scale_param, shape_param, nc_param), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double inv_gamma_os(double p, double shape_param, double N = 1, double r = -1, double scale_param = 1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of cdf_gamma_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    // accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invbeta(n1 + r, -r, p, ref oneMinusxp);
    //    // avoid truncation error by working with xp <= 0.5
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_gamma_nc(xp, shape_param, nc_param) * scale_param;
    //    } else {
    //        functionReturnValue = comp_inv_gamma_nc(oneMinusxp, shape_param, nc_param) * scale_param;
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_gamma_os(double p, double shape_param, double N = 1, double r = -1, double scale_param = 1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of comp_cdf_gamma_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invcompbeta(n1 + r, -r, p, ref oneMinusxp);
    //    // avoid truncation error by working with xp <= 0.5
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_gamma_nc(xp, shape_param, nc_param) * scale_param;
    //    } else {
    //        functionReturnValue = comp_inv_gamma_nc(oneMinusxp, shape_param, nc_param) * scale_param;
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_chi2_os(double x, double df, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid chi2(df) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df = AlterForIntegralChecks_df(df);
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_Chi2_nc(x, df, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = pdf_BETA(p, n1 + r, -r) * pdf_Chi2_nc(x, df, nc_param);
    //    } else {
    //        functionReturnValue = pdf_BETA(comp_cdf_Chi2_nc(x, df, nc_param), -r, n1 + r) * pdf_Chi2_nc(x, df, nc_param);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_chi2_os(double x, double df, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid chi2(df) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df = AlterForIntegralChecks_df(df);
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_Chi2_nc(x, df, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = cdf_BETA(p, n1 + r, -r);
    //    } else {
    //        functionReturnValue = comp_cdf_BETA(comp_cdf_Chi2_nc(x, df, nc_param), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_chi2_os(double x, double df, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid chi2(df) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df = AlterForIntegralChecks_df(df);
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_Chi2_nc(x, df, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = comp_cdf_BETA(p, n1 + r, -r);
    //    } else {
    //        functionReturnValue = cdf_BETA(comp_cdf_Chi2_nc(x, df, nc_param), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double inv_chi2_os(double p, double df, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of cdf_chi2_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    // accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df = AlterForIntegralChecks_df(df);
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invbeta(n1 + r, -r, p, ref oneMinusxp);
    //    // avoid truncation error by working with xp <= 0.5
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_Chi2_nc(xp, df, nc_param);
    //    } else {
    //        functionReturnValue = comp_inv_Chi2_nc(oneMinusxp, df, nc_param);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_chi2_os(double p, double df, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of comp_cdf_chi2_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df = AlterForIntegralChecks_df(df);
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invcompbeta(n1 + r, -r, p, ref oneMinusxp);
    //    // avoid truncation error by working with xp <= 0.5
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_Chi2_nc(xp, df, nc_param);
    //    } else {
    //        functionReturnValue = comp_inv_Chi2_nc(oneMinusxp, df, nc_param);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_F_os(double x, double df1, double df2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_fdist_nc(x, df1, df2, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = pdf_BETA(p, n1 + r, -r) * pdf_fdist_nc(x, df1, df2, nc_param);
    //    } else {
    //        functionReturnValue = pdf_BETA(comp_cdf_fdist_nc(x, df1, df2, nc_param), -r, n1 + r) * pdf_fdist_nc(x, df1, df2, nc_param);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_F_os(double x, double df1, double df2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_fdist_nc(x, df1, df2, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = cdf_BETA(p, n1 + r, -r);
    //    } else {
    //        functionReturnValue = comp_cdf_BETA(comp_cdf_fdist_nc(x, df1, df2, nc_param), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_F_os(double x, double df1, double df2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_fdist_nc(x, df1, df2, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = comp_cdf_BETA(p, n1 + r, -r);
    //    } else {
    //        functionReturnValue = cdf_BETA(comp_cdf_fdist_nc(x, df1, df2, nc_param), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double inv_F_os(double p, double df1, double df2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of cdf_F_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    // accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invbeta(n1 + r, -r, p, ref oneMinusxp);
    //    // avoid truncation error by working with xp <= 0.5
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_fdist_nc(xp, df1, df2, nc_param);
    //    } else {
    //        functionReturnValue = comp_inv_fdist_nc(oneMinusxp, df1, df2, nc_param);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_F_os(double p, double df1, double df2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of comp_cdf_F_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    df1 = AlterForIntegralChecks_df(df1);
    //    df2 = AlterForIntegralChecks_df(df2);
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invcompbeta(n1 + r, -r, p, ref oneMinusxp);
    //    // avoid truncation error by working with xp <= 0.5
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_fdist_nc(xp, df1, df2, nc_param);
    //    } else {
    //        functionReturnValue = comp_inv_fdist_nc(oneMinusxp, df1, df2, nc_param);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double pdf_BETA_os(double x, double shape_param1, double shape_param2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_BETA_nc(x, shape_param1, shape_param2, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = pdf_BETA(p, n1 + r, -r) * pdf_BETA_nc(x, shape_param1, shape_param2, nc_param);
    //    } else {
    //        functionReturnValue = pdf_BETA(comp_cdf_BETA_nc(x, shape_param1, shape_param2, nc_param), -r, n1 + r) * pdf_BETA_nc(x, shape_param1, shape_param2, nc_param);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double cdf_BETA_os(double x, double shape_param1, double shape_param2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_BETA_nc(x, shape_param1, shape_param2, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = cdf_BETA(p, n1 + r, -r);
    //    } else {
    //        functionReturnValue = comp_cdf_BETA(comp_cdf_BETA_nc(x, shape_param1, shape_param2, nc_param), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_cdf_BETA_os(double x, double shape_param1, double shape_param2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double p = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    p = cdf_BETA_nc(x, shape_param1, shape_param2, nc_param);
    //    // avoid truncation error by working with p <= 0.5
    //    if (p <= 0.5) {
    //        functionReturnValue = comp_cdf_BETA(p, n1 + r, -r);
    //    } else {
    //        functionReturnValue = cdf_BETA(comp_cdf_BETA_nc(x, shape_param1, shape_param2, nc_param), -r, n1 + r);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double inv_BETA_os(double p, double shape_param1, double shape_param2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of cdf_BETA_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    // accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invbeta(n1 + r, -r, p, ref oneMinusxp);
    //    // avoid truncation error by working with xp <= 0.5
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_BETA_nc(xp, shape_param1, shape_param2, nc_param);
    //    } else {
    //        functionReturnValue = comp_inv_BETA_nc(oneMinusxp, shape_param1, shape_param2, nc_param);
    //    }
    //    return functionReturnValue;
    //}

    //protected internal static double comp_inv_BETA_os(double p, double shape_param1, double shape_param2, double N = 1, double r = -1, double nc_param = 0)
    //{
    //    double functionReturnValue = 0;
    //    // inverse of comp_cdf_BETA_os
    //    // based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
    //    double oneMinusxp = 0;
    //    N = AlterForIntegralChecks_Others(N);
    //    r = AlterForIntegralChecks_Others(r);
    //    if (N < 1 | Math.Abs(r) > N | r == 0){functionReturnValue = ErrorValue;return functionReturnValue;}
    //    double xp = 0;
    //    double n1 = 0;
    //    n1 = N + 1;
    //    if (r > 0)
    //        r = r - n1;
    //    xp = invcompbeta(n1 + r, -r, p, ref oneMinusxp);
    //    // avoid truncation error by working with xp <= 0.5
    //    if (xp <= 0.5) {
    //        functionReturnValue = inv_BETA_nc(xp, shape_param1, shape_param2, nc_param);
    //    } else {
    //        functionReturnValue = comp_inv_BETA_nc(oneMinusxp, shape_param1, shape_param2, nc_param);
    //    }
    //    return functionReturnValue;
    //}
}
}
