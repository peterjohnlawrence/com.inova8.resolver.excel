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
	//const  bool NonIntegralValuesAllowed_NB = false;
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
    //const double max_discrete = 2^53; //9.00719925474099E+15;
	// 2^53 required for exact addition of 1 in hypergeometric routines
    //const double max_crit = 2^52; //4.5035996273705E+15;
	// 2^52 to make sure plenty of room for exact addition of 1 in crit routines
	//const double  nearly_zero = 9.99999983659714E-317;
	const double  cSmall = 5.562684646268E-309;
	// (smallest number before we start losing precision)/4
	//const double  t_nc_limit = 1.34078079299426E154;
	// just under 1/Sqr(cSmall)
	//const double   Log1p5 = 0.405465108108164;
	// 0.40546510810816438197801311546435 = Log(1.5)
	//const double logfbit0p5 = 0.0548141210519177;
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
	//const double  oneThird = 1.0 / 3.0;
		// 2^27
    //const double twoTo27 = 134217728.0;

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

    private static double AlterForIntegralChecks_Others(double Value)
    {
        double functionReturnValue = 0;
        if (NonIntegralValuesAllowed_Others)
        {
           // functionReturnValue = Convert.ToInt32(Value);
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
           // functionReturnValue = Value;
        }
        else
        {
            functionReturnValue = AlterForIntegralChecks_Others(Value);
        }
        return functionReturnValue;
    }

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

}
}
