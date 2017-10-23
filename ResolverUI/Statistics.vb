'// Copyright © Ian Smith 2002-2007
' http://members.aol.com/iandjmsmith/examples.xls
'// Version 3.3.3

Option Explicit
Public Class statistics

    Const ErrorValue = -9999999999999.9
Const NonIntegralValuesAllowed_df = False
' Are non-integral degrees of freedom for t, chi_square and f distributions allowed?
Const NonIntegralValuesAllowed_NB = False
' Is "successes required" parameter for negative binomial allowed to be non-integral?

Const NonIntegralValuesAllowed_Others = False
' Can Int function be applied to parameters like sample_size or is it a fault if
' the parameter is non-integral?

Const nc_limit = 1000000#
' Upper Limit for non-centrality parameters - as far as I know it's ok but slower
'and slower up to 1e12. Above that I don't know.
Const lstpi = 0.918938533204673
' 0.9189385332046727417803297364 = ln(sqrt(2*Pi))
Const sumAcc = 5E-16
Const cfSmall = 0.00000000000001
Const cfVSmall = 0.000000000000001
Const minLog1Value = -0.79149064
Const OneOverSqrTwoPi = 0.398942280401433
' 0.39894228040143267793994605993438
Const scalefactor = 1.15792089237316E+77
' 1.1579208923731619542357098500869e+77 = 2^256  ' used for rescaling
'calcs w/o impacting accuracy, to avoid over/underflow
Const scalefactor2 = 8.63616855509444E-78
' 8.6361685550944446253863518628004e-78 = 2^-256
Const max_discrete = 9.00719925474099E+15
' 2^53 required for exact addition of 1 in hypergeometric routines
Const max_crit = 4.5035996273705E+15
' 2^52 to make sure plenty of room for exact addition of 1 in crit routines
Const nearly_zero = 9.99999983659714E-317
Const cSmall = 5.562684646268E-309
' (smallest number before we start losing precision)/4
Const t_nc_limit = 1.34078079299426E+154
' just under 1/Sqr(cSmall)
Const Log1p5 = 0.405465108108164
' 0.40546510810816438197801311546435 = Log(1.5)
Const logfbit0p5 = 5.48141210519177E-02
' 0.054814121051917653896138702348386 = logfbit(0.5)

'For logfbit functions
' Stirling's series for ln(Gamma(x)), A046968/A046969
Const lfbc1 = 1# / 12#
Const lfbc2 = 1# / 30#
' lfbc2 on are Sloane's ratio times 12
Const lfbc3 = 1# / 105#
Const lfbc4 = 1# / 140#
Const lfbc5 = 1# / 99#
Const lfbc6 = 691# / 30030#
Const lfbc7 = 1# / 13#
Const lfbc8 = 0.350686068964593
' Chosen to make logfbit(6) & logfbit(7) correct
Const lfbc9 = 1.67699982016711
' Chosen to make logfbit(6) & logfbit(7) correct

'For invcnormal                             ' http://lib.stat.cmu.edu/apstat/241
Const a0 = 3.38713287279637                 ' 3.3871328727963666080
Const a1 = 133.141667891784                 ' 133.14166789178437745
Const a2 = 1971.59095030655                 ' 1971.5909503065514427
Const a3 = 13731.6937655095                 ' 13731.693765509461125
Const a4 = 45921.9539315499                 ' 45921.953931549871457
Const a5 = 67265.7709270087                 ' 67265.770927008700853
Const a6 = 33430.5755835881                 ' 33430.575583588128105
Const a7 = 2509.08092873012                 ' 2509.0809287301226727
Const b1 = 42.3133307016009                 ' 42.313330701600911252
Const b2 = 687.187007492058                 ' 687.18700749205790830
Const b3 = 5394.19602142475                 ' 5394.1960214247511077
Const b4 = 21213.7943015866                 ' 21213.794301586595867
Const b5 = 39307.8958000927                 ' 39307.895800092710610
Const b6 = 28729.0857357219                 ' 28729.085735721942674
Const b7 = 5226.49527885285                 ' 5226.4952788528545610
'//Coefficients for P not close to 0, 0.5 or 1.
Const c0 = 1.42343711074968                 ' 1.42343711074968357734
Const c1 = 4.63033784615655                 ' 4.63033784615654529590
Const c2 = 5.76949722146069                 ' 5.76949722146069140550
Const c3 = 3.6478483247632                  ' 3.64784832476320460504
Const c4 = 1.27045825245237                 ' 1.27045825245236838258
Const c5 = 0.241780725177451                ' 0.241780725177450611770
Const c6 = 2.27238449892692E-02             ' 2.27238449892691845833E-02
Const c7 = 7.74545014278341E-04             ' 7.74545014278341407640E-04
Const d1 = 2.05319162663776                 ' 2.05319162663775882187
Const d2 = 1.6763848301838                  ' 1.67638483018380384940
Const d3 = 0.6897673349851                  ' 0.689767334985100004550
Const d4 = 0.14810397642748                 ' 0.148103976427480074590
Const d5 = 1.51986665636165E-02             ' 1.51986665636164571966E-02
Const d6 = 5.47593808499535E-04             ' 5.47593808499534494600E-04
Const d7 = 1.05075007164442E-09             ' 1.05075007164441684324E-09
'//Coefficients for P near 0 or 1.
Const e0 = 6.6579046435011                  ' 6.65790464350110377720
Const e1 = 5.46378491116411                 ' 5.46378491116411436990
Const e2 = 1.78482653991729                 ' 1.78482653991729133580
Const e3 = 0.296560571828505                ' 0.296560571828504891230
Const e4 = 2.65321895265761E-02             ' 2.65321895265761230930E-02
Const e5 = 1.24266094738808E-03             ' 1.24266094738807843860E-03
Const e6 = 2.71155556874349E-05             ' 2.71155556874348757815E-05
Const e7 = 2.01033439929229E-07             ' 2.01033439929228813265E-07
Const f1 = 0.599832206555888                ' 0.599832206555887937690
Const f2 = 0.136929880922736                ' 0.136929880922735805310
Const f3 = 1.48753612908506E-02             ' 1.48753612908506148525E-02
Const f4 = 7.86869131145613E-04             ' 7.86869131145613259100E-04
Const f5 = 1.84631831751005E-05             ' 1.84631831751005468180E-05
Const f6 = 1.42151175831645E-07             ' 1.42151175831644588870E-07
Const f7 = 2.04426310338994E-15             ' 2.04426310338993978564E-15

'For poissapprox                            ' Stirling's series for Gamma(x), A001163/A001164
Const coef15 = 1# / 12#
Const coef25 = 1# / 288#
Const coef35 = -139# / 51840#
Const coef45 = -571# / 2488320#
Const coef55 = 163879# / 209018880#
Const coef65 = 5246819# / 75246796800#
Const coef75 = -534703531# / 902961561600#
Const coef1 = 2# / 3#                        ' Ramanujan's series for Gamma(x+1,x)-Gamma(x+1)/2, A065973
Const coef2 = -4# / 135#                     ' cf. http://www.whim.org/nebula/math/gammaratio.html
Const coef3 = 8# / 2835#
Const coef4 = 16# / 8505#
Const coef5 = -8992# / 12629925#
Const coef6 = -334144# / 492567075#
Const coef7 = 698752# / 1477701225#
Const coef8 = 23349012224# / 39565450299375#

Const twoThirds = 2# / 3#
Const twoFifths = 2# / 5#
Const twoSevenths = 2# / 7#
Const twoNinths = 2# / 9#
Const twoElevenths = 2# / 11#
Const twoThirteenths = 2# / 13#

'For binapprox
Const oneThird = 1# / 3#
Const twoTo27 = 134217728#                   ' 2^27

'For lngammaexpansion
Const eulers_const = 0.577215664901533      ' 0.5772156649015328606065120901

Private Function Min(ByVal x As Double, ByVal y As Double) As Double
   If x < y Then
      Min = x
   Else
      Min = y
   End If
End Function
Private Function max(ByVal x As Double, ByVal y As Double) As Double
   If x > y Then
      max = x
   Else
      max = y
   End If
End Function

Private Function expm1(ByVal x As Double) As Double
'// Accurate calculation of exp(x)-1, particularly for small x.
'// Uses a variation of the standard continued fraction for tanh(x) see A&S 4.5.70.
        If (Math.Abs(x) < 2) Then
            Dim a1 As Double, a2 As Double, b1 As Double, b2 As Double, c1 As Double, x2 As Double
            a1 = 24.0#
            b1 = 2.0# * (12.0# - x * (6.0# - x))
            x2 = x * x * 0.25
            a2 = 8.0# * (15.0# + x2)
            b2 = 120.0# - x * (60.0# - x * (12.0# - x))
            c1 = 7.0#

            While ((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfSmall * b1 * a2)))

                a1 = c1 * a2 + x2 * a1
                b1 = c1 * b2 + x2 * b1
                c1 = c1 + 2.0#

                a2 = c1 * a1 + x2 * a2
                b2 = c1 * b1 + x2 * b2
                c1 = c1 + 2.0#
                If (b2 > scalefactor) Then
                    a1 = a1 * scalefactor2
                    b1 = b1 * scalefactor2
                    a2 = a2 * scalefactor2
                    b2 = b2 * scalefactor2
                End If
            End While

            expm1 = x * a2 / b2
        Else
            expm1 = Math.Exp(x) - 1.0#
        End If

End Function

Private Function logcf(ByVal x As Double, ByVal i As Double, ByVal D As Double) As Double
'// Continued fraction for calculation of 1/i + x/(i+d) + x*x/(i+2*d) + x*x*x/(i+3d) + ...
Dim a1 As Double, a2 As Double, b1 As Double, b2 As Double, c1 As Double, c2 As Double, c3 As Double, c4 As Double
     c1 = 2# * D
     c2 = i + D
     c4 = c2 + D
     a1 = c2
     b1 = i * (c2 - i * x)
     b2 = D * D * x
     a2 = c4 * c2 - b2
     b2 = c4 * b1 - i * b2

        While ((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfVSmall * b1 * a2)))

            c3 = c2 * c2 * x
            c2 = c2 + D
            c4 = c4 + D
            a1 = c4 * a2 - c3 * a1
            b1 = c4 * b2 - c3 * b1

            c3 = c1 * c1 * x
            c1 = c1 + D
            c4 = c4 + D
            a2 = c4 * a1 - c3 * a2
            b2 = c4 * b1 - c3 * b2
            If (b2 > scalefactor) Then
                a1 = a1 * scalefactor2
                b1 = b1 * scalefactor2
                a2 = a2 * scalefactor2
                b2 = b2 * scalefactor2
            ElseIf (b2 < scalefactor2) Then
                a1 = a1 * scalefactor
                b1 = b1 * scalefactor
                a2 = a2 * scalefactor
                b2 = b2 * scalefactor
            End If
        End While
     logcf = a2 / b2
End Function

Private Function log0(ByVal x As Double) As Double
'//Accurate calculation of log(1+x), particularly for small x.
   Dim term As Double
        If (Math.Abs(x) > 0.5) Then
            log0 = Math.Log(1.0# + x)
        Else
            term = x / (2.0# + x)
            log0 = 2.0# * term * logcf(term * term, 1.0#, 2.0#)
        End If
End Function

Private Function log1(ByVal x As Double) As Double
'//Accurate calculation of log(1+x)-x, particularly for small x.
   Dim term As Double, y  As Double
        If (Math.Abs(x) < 0.01) Then
            term = x / (2.0# + x)
            y = term * term
            log1 = term * ((((2.0# / 9.0# * y + 2.0# / 7.0#) * y + 0.4) * y + 2.0# / 3.0#) * y - x)
        ElseIf (x < minLog1Value Or x > 1.0#) Then
            log1 = Math.Log(1.0# + x) - x
        Else
            term = x / (2.0# + x)
            y = term * term
            log1 = term * (2.0# * y * logcf(y, 3.0#, 2.0#) - x)
        End If
End Function

Private Function logfbitdif(ByVal x As Double) As Double
'//Calculation of logfbit(x)-logfbit(1+x). x must be > -1.
  Dim y As Double, y2 As Double
  y = 1# / (2# * x + 3#)
  y2 = y * y
  logfbitdif = y2 * logcf(y2, 3#, 2#)
End Function

Private Function logfbit(ByVal x As Double) As Double
'//Error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
'//Are we ever concerned about the relative error involved in this function? I don't think so.
  Dim x1 As Double, x2 As Double, x3 As Double
  If (x >= 10000000000#) Then
     logfbit = lfbc1 / (x + 1#)
  ElseIf (x >= 6#) Then                      ' Abramowitz & Stegun's series 6.1.41
     x1 = x + 1#
     x2 = 1# / (x1 * x1)
     x3 = x2 * (lfbc6 - x2 * (lfbc7 - x2 * (lfbc8 - x2 * lfbc9)))
     x3 = x2 * (lfbc4 - x2 * (lfbc5 - x3))
     x3 = x2 * (lfbc2 - x2 * (lfbc3 - x3))
     logfbit = lfbc1 * (1# - x3) / x1
  ElseIf (x = 5#) Then
     logfbit = 1.38761288230707E-02                         ' 1.3876128823070747998745727023763E-02  ' calculated to give exact factorials
  ElseIf (x = 4#) Then
     logfbit = 1.66446911898212E-02                         ' 1.6644691189821192163194865373593E-02
  ElseIf (x = 3#) Then
     logfbit = 2.07906721037651E-02                         ' 2.0790672103765093111522771767849E-02
  ElseIf (x = 2#) Then
     logfbit = 2.76779256849983E-02                         ' 2.7677925684998339148789292746245E-02
  ElseIf (x = 1#) Then
     logfbit = 4.13406959554093E-02                         ' 4.1340695955409294093822081407118E-02
  ElseIf (x = 0#) Then
     logfbit = 8.10614667953273E-02                         ' 8.1061466795327258219670263594382E-02
  ElseIf (x > -1#) Then
     x1 = x
     x2 = 0#
     While (x1 < 6#)
        x2 = x2 + logfbitdif(x1)
        x1 = x1 + 1#
            End While

     logfbit = x2 + logfbit(x1)
  Else
     logfbit = 1E+308
  End If
End Function

Private Function logdif(ByVal pr As Double, ByVal prob As Double) As Double
   Dim temp As Double
   temp = (pr - prob) / prob
        If Math.Abs(temp) >= 0.5 Then
            logdif = Math.Log(pr / prob)
        Else
            logdif = log0(temp)
        End If
End Function

Private Function cnormal(ByVal x As Double) As Double
'//Probability that a normal variate <= x
  Dim acc As Double, x2 As Double, D As Double, term As Double, a1 As Double, a2 As Double, b1 As Double, b2 As Double, c1 As Double, c2 As Double, c3 As Double

        If (Math.Abs(x) < 1.5) Then
            acc = 0.0#
            x2 = x * x
            term = 1.0#
            D = 3.0#

            While (term > sumAcc * acc)

                D = D + 2.0#
                term = term * x2 / D
                acc = acc + term

            End While

            acc = 1.0# + x2 / 3.0# * (1.0# + acc)
            cnormal = 0.5 + Math.Exp(-x * x * 0.5 - lstpi) * x * acc
        ElseIf (Math.Abs(x) > 40.0#) Then
            If (x > 0.0#) Then
                cnormal = 1.0#
            Else
                cnormal = 0.0#
            End If
        Else
            x2 = x * x
            a1 = 2.0#
            b1 = x2 + 5.0#
            c2 = x2 + 9.0#
            a2 = a1 * c2
            b2 = b1 * c2 - 12.0#
            c1 = 5.0#
            c2 = c2 + 4.0#

            While ((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfVSmall * b1 * a2)))

                c3 = c1 * (c1 + 1.0#)
                a1 = c2 * a2 - c3 * a1
                b1 = c2 * b2 - c3 * b1
                c1 = c1 + 2.0#
                c2 = c2 + 4.0#
                c3 = c1 * (c1 + 1.0#)
                a2 = c2 * a1 - c3 * a2
                b2 = c2 * b1 - c3 * b2
                c1 = c1 + 2.0#
                c2 = c2 + 4.0#
                If (b2 > scalefactor) Then
                    a1 = a1 * scalefactor2
                    b1 = b1 * scalefactor2
                    a2 = a2 * scalefactor2
                    b2 = b2 * scalefactor2
                End If

            End While

            If (x > 0.0#) Then
                cnormal = 1.0# - Math.Exp(-x * x * 0.5 - lstpi) * x / (x2 + 1.0# - a2 / b2)
            Else
                cnormal = -Math.Exp(-x * x * 0.5 - lstpi) * x / (x2 + 1.0# - a2 / b2)
            End If

        End If
End Function

Private Function invcnormal(ByVal p As Double) As Double
'//Inverse of cnormal from AS241.
'//Require p to be strictly in the range 0..1

   Dim PPND16 As Double, Q As Double, r As Double
   Q = p - 0.5
        If (Math.Abs(Q) <= 0.425) Then
            r = 0.180625 - Q * Q
            PPND16 = Q * (((((((a7 * r + a6) * r + a5) * r + a4) * r + a3) * r + a2) * r + a1) * r + a0) / (((((((b7 * r + b6) * r + b5) * r + b4) * r + b3) * r + b2) * r + b1) * r + 1.0#)
        Else
            If (Q < 0.0#) Then
                r = p
            Else
                r = 1.0# - p
            End If
            r = Math.Sqrt(-Math.Log(r))
            If (r <= 5.0#) Then
                r = r - 1.6
                PPND16 = (((((((c7 * r + c6) * r + c5) * r + c4) * r + c3) * r + c2) * r + c1) * r + c0) / (((((((d7 * r + d6) * r + d5) * r + d4) * r + d3) * r + d2) * r + d1) * r + 1.0#)
            Else
                r = r - 5.0#
                PPND16 = (((((((e7 * r + e6) * r + e5) * r + e4) * r + e3) * r + e2) * r + e1) * r + e0) / (((((((f7 * r + f6) * r + f5) * r + f4) * r + f3) * r + f2) * r + f1) * r + 1.0#)
            End If
            If (Q < 0.0#) Then
                PPND16 = -PPND16
            End If
        End If
   invcnormal = PPND16
End Function

Private Function even(ByVal x As Double) As Boolean
   even = (Int(x / 2#) * 2# = x)
End Function

Private Function tdistexp(ByVal p As Double, ByVal Q As Double, ByVal logqk2 As Double, ByVal k As Double, ByRef tdistDensity As Double) As Double
'//Special transformation of t-distribution useful for BinApprox.
'//Note approxtdistDens only used by binApprox if k > 100 or so.
   Dim sum As Double, aki As Double, Ai As Double, term As Double, q1 As Double, q8 As Double
   Dim c1 As Double, c2 As Double, a1 As Double, a2 As Double, b1 As Double, b2 As Double, cadd As Double
   Dim Result As Double, approxtdistDens As Double

   If (even(k)) Then
            approxtdistDens = Math.Exp(logqk2 + logfbit(k - 1.0#) - 2.0# * logfbit(k * 0.5 - 1.0#) - lstpi)
   Else
            approxtdistDens = Math.Exp(logqk2 + k * log1(1.0# / k) + 2.0# * logfbit((k - 1.0#) * 0.5) - logfbit(k - 1.0#) - lstpi)
   End If

   If (k * p < 4# * Q) Then
     sum = 0#
     aki = k + 1#
     Ai = 3#
     term = 1#

     While (term > sumAcc * sum)

        Ai = Ai + 2#
        aki = aki + 2#
        term = term * aki * p / Ai
        sum = sum + term

            End While

     sum = 1# + (k + 1#) * p * (1# + sum) / 3#
            Result = 0.5 - approxtdistDens * sum * Math.Sqrt(k * p)
        ElseIf approxtdistDens = 0.0# Then
            Result = 0.0#
        Else
            q1 = 2.0# * (1.0# + Q)
            q8 = 8.0# * Q
            a1 = 1.0#
            b1 = (k - 3.0#) * p + 7.0#
            c1 = -20.0# * Q
            a2 = (k - 5.0#) * p + 11.0#
            b2 = a2 * b1 + c1
            cadd = -30.0# * Q
            c1 = -42.0# * Q
            c2 = (k - 7.0#) * p + 15.0#

            While ((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfVSmall * b1 * a2)))

                a1 = c2 * a2 + c1 * a1
                b1 = c2 * b2 + c1 * b1
                c1 = c1 + cadd
                cadd = cadd - q8
                c2 = c2 + q1
                a2 = c2 * a1 + c1 * a2
                b2 = c2 * b1 + c1 * b2
                c1 = c1 + cadd
                cadd = cadd - q8
                c2 = c2 + q1
                If (Math.Abs(b2) > scalefactor) Then
                    a1 = a1 * scalefactor2
                    b1 = b1 * scalefactor2
                    a2 = a2 * scalefactor2
                    b2 = b2 * scalefactor2
                ElseIf (Math.Abs(b2) < scalefactor2) Then
                    a1 = a1 * scalefactor
                    b1 = b1 * scalefactor
                    a2 = a2 * scalefactor
                    b2 = b2 * scalefactor
                End If
            End While

            Result = approxtdistDens * (1.0# - Q / ((k - 1.0#) * p + 3.0# - 6.0# * Q * a2 / b2)) / Math.Sqrt(k * p)
        End If
        tdistDensity = approxtdistDens * Math.Sqrt(Q)
        tdistexp = Result
    End Function

    Private Function tdist(ByVal x As Double, ByVal k As Double, ByVal tdistDensity As Double) As Double
        '//Probability that variate from t-distribution with k degress of freedom <= x
        Dim x2 As Double, k2 As Double, logterm As Double, a As Double, r As Double

        If Math.Abs(x) >= Min(1.0#, k) Then
            k2 = k / x
            x2 = x + k2
            k2 = k2 / x2
            x2 = x / x2
        Else
            x2 = x * x
            k2 = k + x2
            x2 = x2 / k2
            k2 = k / k2
        End If
        If (k > 1.0E+30) Then
            tdist = cnormal(x)
            tdistDensity = Math.Exp(-x * x / 2.0#)
        Else
            If (k2 < cSmall) Then
                logterm = k * 0.5 * (Math.Log(k) - 2.0# * Math.Log(Math.Abs(x)))
            ElseIf (Math.Abs(x2) < 0.5) Then
                logterm = k * 0.5 * log0(-x2)
            Else
                logterm = k * 0.5 * Math.Log(k2)
            End If
            If (k >= 1.0#) Then
                If (x < 0.0#) Then
                    tdist = tdistexp(x2, k2, logterm, k, tdistDensity)
                Else
                    tdist = 1.0# - tdistexp(x2, k2, logterm, k, tdistDensity)
                End If
                Exit Function
            End If
            a = k / 2.0#
            tdistDensity = Math.Exp(0.5 + (1.0# + 1.0# / k) * logterm + a * log0(-0.5 / (a + 1.0#)) + logfbit(a - 0.5) - logfbit(a)) * Math.Sqrt(a / ((1.0# + a))) * OneOverSqrTwoPi
            If (k2 < cSmall) Then
                r = (a + 1.0#) * log1(a / 1.5) + (logfbit(a + 0.5) - logfbit0p5) - lngammaexpansion(a)
                r = r + a * ((a - 0.5) / 1.5 + Log1p5 + (Math.Log(k) - 2.0# * Math.Log(Math.Abs(x))))
                r = Math.Exp(r) * (0.25 / (a + 0.5))
                If x < 0.0# Then
                    tdist = r
                Else
                    tdist = 1.0# - r
                End If
            ElseIf (x < 0.0#) Then
                If x2 < k2 Then
                    tdist = 0.5 * compbeta(x2, 0.5, a)
                Else
                    tdist = 0.5 * BETA(k2, a, 0.5)
                End If
            Else
                If x2 < k2 Then
                    tdist = 0.5 * (1.0# + BETA(x2, 0.5, a))
                Else
                    tdist = 0.5 * (1.0# + compbeta(k2, a, 0.5))
                End If
            End If
        End If
    End Function

    Private Function BetterThanTailApprox(ByVal prob As Double, ByVal df As Double) As Boolean
        If df <= 2 Then
            BetterThanTailApprox = prob > 0.25 * Math.Exp((1.0# - df) * 1.78514841051368)
        ElseIf df <= 5 Then
            BetterThanTailApprox = prob > 0.045 * Math.Exp((2.0# - df) * 1.30400766847605)
        ElseIf df <= 20 Then
            BetterThanTailApprox = prob > 0.0009 * Math.Exp((5.0# - df) * 0.921034037197618)
        Else
            BetterThanTailApprox = prob > 0.0000000009 * Math.Exp((20.0# - df) * 0.690775527898214)
        End If
    End Function

    Private Function invtdist(ByVal prob As Double, ByVal df As Double) As Double
        '//Inverse of tdist
        '//Require prob to be in the range 0..1 df should be positive
        Dim xn As Double, xn2 As Double, tp As Double, tpDif As Double, tprob As Double, a As Double, pr As Double, lpr As Double, small As Double, smalllpr As Double, tdistDensity As Double
        If prob > 0.5 Then
            pr = 1.0# - prob
        Else
            pr = prob
        End If
        lpr = -Math.Log(pr)
        small = 0.00000000000001
        smalllpr = small * lpr * pr
        If pr >= 0.5 Or df >= 1.0# And BetterThanTailApprox(pr, df) Then
            '// Will divide by 0 if tp so small that tdistDensity underflows. Not a problem if prob > cSmall
            xn = invcnormal(pr)
            xn2 = xn * xn
            '//Initial approximation is given in http://digital.library.adelaide.edu.au/coll/special//fisher/281.pdf. The modified NR correction then gets it right.
            tp = (((((27.0# * xn2 + 339.0#) * xn2 + 930.0#) * xn2 - 1782.0#) * xn2 - 765.0#) * xn2 + 17955.0#) / (368640.0# * df)
            tp = (tp + ((((79.0# * xn2 + 776.0#) * xn2 + 1482.0#) * xn2 - 1920.0#) * xn2 - 945.0#) / 92160.0#) / df
            tp = (tp + (((3.0# * xn2 + 19.0#) * xn2 + 17.0#) * xn2 - 15.0#) / 384.0#) / df
            tp = (tp + ((5.0# * xn2 + 16) * xn2 + 3.0#) / 96.0#) / df
            tp = (tp + (xn2 + 1.0#) / 4.0#) / df
            tp = xn * (1.0# + tp)
            tprob = 0.0#
            tpDif = 1.0# + Math.Abs(tp)
        ElseIf df < 1.0# Then
            a = df / 2.0#
            tp = (a + 1.0#) * log1(a / 1.5) + (logfbit(a + 0.5) - logfbit0p5) - lngammaexpansion(a)
            tp = ((a - 0.5) / 1.5 + Log1p5 + Math.Log(df)) / 2.0# + (tp - Math.Log(4.0# * pr * (a + 0.5))) / df
            tp = -Math.Exp(tp)
            tprob = tdist(tp, df, tdistDensity)
            If tdistDensity < nearly_zero Then
                tpDif = 0.0#
            Else
                tpDif = (tprob / tdistDensity) * log0((tprob - pr) / pr)
                tp = tp - tpDif
            End If
        Else
            tp = tdist(0, df, tdistDensity) 'Marginally quicker to get tdistDensity for integral df
            tp = Math.Exp(-Math.Log(Math.Sqrt(df) * pr / tdistDensity) / df)
            If df >= 2 Then
                tp = -Math.Sqrt(df * (tp * tp - 1.0#))
            Else
                tp = -Math.Sqrt(df) * Math.Sqrt(tp - 1.0#) * Math.Sqrt(tp + 1.0#)
            End If
            tpDif = tp / df
            tpDif = -log0((0.5 - 1.0# / (df + 2)) / (1.0# + tpDif * tp)) * (tpDif + 1.0# / tp)
            tp = tp - tpDif
            tprob = tdist(tp, df, tdistDensity)
            If tdistDensity < nearly_zero Then
                tpDif = 0.0#
            Else
                tpDif = (tprob / tdistDensity) * log0((tprob - pr) / pr)
                tp = tp - tpDif
            End If
        End If
        While (Math.Abs(tprob - pr) > smalllpr And Math.Abs(tpDif) > small * (1.0# + Math.Abs(tp)))
            tprob = tdist(tp, df, tdistDensity)
            tpDif = (tprob / tdistDensity) * log0((tprob - pr) / pr)
            tp = tp - tpDif
        End While
        invtdist = tp
        If prob > 0.5 Then invtdist = -invtdist
    End Function

    Private Function poissonTerm(ByVal i As Double, ByVal N As Double, ByVal diffFromMean As Double, ByVal logAdd As Double) As Double
        '//Probability that poisson variate with mean n has value i (diffFromMean = n-i)
        Dim c2 As Double, c3 As Double
        Dim logpoissonTerm As Double, c1 As Double

        If ((i <= -1.0#) Or (N < 0.0#)) Then
            If (i = 0.0#) Then
                poissonTerm = Math.Exp(logAdd)
            Else
                poissonTerm = 0.0#
            End If
        ElseIf ((i < 0.0#) And (N = 0.0#)) Then
            poissonTerm = ErrorValue
        Else
            c3 = i
            c2 = c3 + 1.0#
            c1 = (diffFromMean - 1.0#) / c2

            If (c1 < minLog1Value) Then
                If (i = 0.0#) Then
                    logpoissonTerm = -N
                    poissonTerm = Math.Exp(logpoissonTerm + logAdd)
                ElseIf (N = 0.0#) Then
                    poissonTerm = 0.0#
                Else
                    logpoissonTerm = (c3 * Math.Log(N / c2) - (diffFromMean - 1.0#)) - logfbit(c3)
                    poissonTerm = Math.Exp(logpoissonTerm + logAdd) / Math.Sqrt(c2) * OneOverSqrTwoPi
                End If
            Else
                logpoissonTerm = c3 * log1(c1) - c1 - logfbit(c3)
                poissonTerm = Math.Exp(logpoissonTerm + logAdd) / Math.Sqrt(c2) * OneOverSqrTwoPi
            End If
        End If
    End Function

    Private Function poisson1(ByVal i As Double, ByVal N As Double, ByVal diffFromMean As Double) As Double
        '//Probability that poisson variate with mean n has value <= i (diffFromMean = n-i)
        '//For negative values of i (used for calculating the cumlative gamma distribution) there's a really nasty interpretation!
        '//1-gamma(n,i) is calculated as poisson1(-i,n,0) since we need an accurate version of i rather than i-1.
        '//Uses a simplified version of Legendre's continued fraction.
        Dim prob As Double, exact As Boolean
        If ((i >= 0.0#) And (N <= 0.0#)) Then
            exact = True
            prob = 1.0#
        ElseIf ((i > -1.0#) And (N <= 0.0#)) Then
            exact = True
            prob = 0.0#
        ElseIf ((i > -1.0#) And (i < 0.0#)) Then
            i = -i
            exact = False
            prob = poissonTerm(i, N, N - i, 0.0#) * i / N
            i = i - 1.0#
            diffFromMean = N - i
        Else
            exact = ((i <= -1.0#) Or (N < 0.0#))
            prob = poissonTerm(i, N, diffFromMean, 0.0#)
        End If
        If (exact Or prob = 0.0#) Then
            poisson1 = prob
            Exit Function
        End If

        Dim a1 As Double, a2 As Double, b1 As Double, b2 As Double, c1 As Double, c2 As Double, c3 As Double, c4 As Double, cfValue As Double
        Dim njj As Long, Numb As Long
        Dim sumAlways As Long, sumFactor As Long
        sumAlways = 0
        sumFactor = 6
        a1 = 0.0#
        If (i > sumAlways) Then
            Numb = Int(sumFactor * Math.Exp(Math.Log(N) / 3))
            Numb = max(0, Int(Numb - diffFromMean))
            If (Numb > i) Then
                Numb = Int(i)
            End If
        Else
            Numb = max(0, Int(i))
        End If

        b1 = 1.0#
        a2 = i - Numb
        b2 = diffFromMean + (Numb + 1.0#)
        c1 = 0.0#
        c2 = a2
        c4 = b2
        If c2 < 0.0# Then
            cfValue = cfVSmall
        Else
            cfValue = cfSmall
        End If
        While ((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfValue * b1 * a2)))

            c1 = c1 + 1.0#
            c2 = c2 - 1.0#
            c3 = c1 * c2
            c4 = c4 + 2.0#
            a1 = c4 * a2 + c3 * a1
            b1 = c4 * b2 + c3 * b1
            c1 = c1 + 1.0#
            c2 = c2 - 1.0#
            c3 = c1 * c2
            c4 = c4 + 2.0#
            a2 = c4 * a1 + c3 * a2
            b2 = c4 * b1 + c3 * b2
            If (b2 > scalefactor) Then
                a1 = a1 * scalefactor2
                b1 = b1 * scalefactor2
                a2 = a2 * scalefactor2
                b2 = b2 * scalefactor2
            End If
            If c2 < 0.0# And cfValue > cfVSmall Then
                cfValue = cfVSmall
            End If
        End While

        a1 = a2 / b2

        c1 = i - Numb + 1.0#
        For njj = 1 To Numb
            a1 = (1.0# + a1) * (c1 / N)
            c1 = c1 + 1.0#
        Next njj

        poisson1 = (1.0# + a1) * prob
    End Function

    Private Function poisson2(ByVal i As Double, ByVal N As Double, ByVal diffFromMean As Double) As Double
        '//Probability that poisson variate with mean n has value >= i (diffFromMean = n-i)
        Dim prob As Double, exact As Boolean
        If ((i <= 0.0#) And (N <= 0.0#)) Then
            exact = True
            prob = 1.0#
        Else
            exact = False
            prob = poissonTerm(i, N, diffFromMean, 0.0#)
        End If
        If (exact Or prob = 0.0#) Then
            poisson2 = prob
            Exit Function
        End If

        Dim a1 As Double, a2 As Double, b1 As Double, b2 As Double, c1 As Double, c2 As Double
        Dim njj As Long, Numb As Long
        Const sumFactor = 6
        Numb = Int(sumFactor * Math.Exp(Math.Log(N) / 3))
        Numb = max(0, Int(diffFromMean + Numb))

        a1 = 0.0#
        b1 = 1.0#
        a2 = N
        b2 = (Numb + 1.0#) - diffFromMean
        c1 = 0.0#
        c2 = b2

        While ((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfSmall * b1 * a2)))

            c1 = c1 + N
            c2 = c2 + 1.0#
            a1 = c2 * a2 + c1 * a1
            b1 = c2 * b2 + c1 * b1
            c1 = c1 + N
            c2 = c2 + 1.0#
            a2 = c2 * a1 + c1 * a2
            b2 = c2 * b1 + c1 * b2
            If (b2 > scalefactor) Then
                a1 = a1 * scalefactor2
                b1 = b1 * scalefactor2
                a2 = a2 * scalefactor2
                b2 = b2 * scalefactor2
            End If
        End While

        a1 = a2 / b2

        c1 = i + Numb
        For njj = 1 To Numb
            a1 = (1.0# + a1) * (N / c1)
            c1 = c1 - 1.0#
        Next

        poisson2 = (1.0# + a1) * prob

    End Function

    Private Function poissonApprox(ByVal j As Double, ByVal diffFromMean As Double, ByVal comp As Boolean) As Double
        '//Asymptotic expansion to calculate the probability that poisson variate has value <= j (diffFromMean = mean-j). If comp then calulate 1-probability.
        '//cf. http://members.aol.com/iandjmsmith/PoissonApprox.htm
        Dim pt As Double, s2pt As Double, res1 As Double, res2 As Double, elfb As Double, term As Double
        Dim ig2 As Double, ig3 As Double, ig4 As Double, ig5 As Double, ig6 As Double, ig7 As Double, ig8 As Double
        Dim ig05 As Double, ig25 As Double, ig35 As Double, ig45 As Double, ig55 As Double, ig65 As Double, ig75 As Double

        pt = -log1(diffFromMean / j)
        s2pt = Math.Sqrt(2.0# * j * pt)

        ig2 = 1.0# / j + pt
        term = pt * pt * 0.5
        ig3 = ig2 / j + term
        term = term * pt / 3.0#
        ig4 = ig3 / j + term
        term = term * pt / 4.0#
        ig5 = ig4 / j + term
        term = term * pt / 5.0#
        ig6 = ig5 / j + term
        term = term * pt / 6.0#
        ig7 = ig6 / j + term
        term = term * pt / 7.0#
        ig8 = ig7 / j + term

        ig05 = cnormal(-s2pt)
        term = pt * twoThirds
        ig25 = 1.0# / j + term
        term = term * pt * twoFifths
        ig35 = ig25 / j + term
        term = term * pt * twoSevenths
        ig45 = ig35 / j + term
        term = term * pt * twoNinths
        ig55 = ig45 / j + term
        term = term * pt * twoElevenths
        ig65 = ig55 / j + term
        term = term * pt * twoThirteenths
        ig75 = ig65 / j + term

        elfb = ((((((coef75 / j + coef65) / j + coef55) / j + coef45) / j + coef35) / j + coef25) / j + coef15) + j
        res1 = (((((((ig8 * coef8 + ig7 * coef7) + ig6 * coef6) + ig5 * coef5) + ig4 * coef4) + ig3 * coef3) + ig2 * coef2) + coef1) * Math.Sqrt(j)
        res2 = ((((((ig75 * coef75 + ig65 * coef65) + ig55 * coef55) + ig45 * coef45) + ig35 * coef35) + ig25 * coef25) + coef15) * s2pt

        If (comp) Then
            If (diffFromMean < 0.0#) Then
                poissonApprox = ig05 - (res1 - res2) * Math.Exp(-j * pt - lstpi) / elfb
            Else
                poissonApprox = (1.0# - ig05) - (res1 + res2) * Math.Exp(-j * pt - lstpi) / elfb
            End If
        ElseIf (diffFromMean < 0.0#) Then
            poissonApprox = (1.0# - ig05) + (res1 - res2) * Math.Exp(-j * pt - lstpi) / elfb
        Else
            poissonApprox = ig05 + (res1 + res2) * Math.Exp(-j * pt - lstpi) / elfb
        End If
    End Function

    Private Function cpoisson(ByVal k As Double, ByVal Lambda As Double, ByVal dfm As Double) As Double
        '//Probability that poisson variate with mean lambda has value <= k (diffFromMean = lambda-k) calculated by various methods.
        If ((k >= 21.0#) And (Math.Abs(dfm) < (0.3 * k))) Then
            cpoisson = poissonApprox(k, dfm, False)
        ElseIf ((Lambda > k) And (Lambda >= 1.0#)) Then
            cpoisson = poisson1(k, Lambda, dfm)
        Else
            cpoisson = 1.0# - poisson2(k + 1.0#, Lambda, dfm - 1.0#)
        End If
    End Function

    Private Function comppoisson(ByVal k As Double, ByVal Lambda As Double, ByVal dfm As Double) As Double
        '//Probability that poisson variate with mean lambda has value > k (diffFromMean = lambda-k) calculated by various methods.
        If ((k >= 21.0#) And (Math.Abs(dfm) < (0.3 * k))) Then
            comppoisson = poissonApprox(k, dfm, True)
        ElseIf ((Lambda > k) And (Lambda >= 1.0#)) Then
            comppoisson = 1.0# - poisson1(k, Lambda, dfm)
        Else
            comppoisson = poisson2(k + 1.0#, Lambda, dfm - 1.0#)
        End If
    End Function

    Private Function invpoisson(ByVal k As Double, ByVal prob As Double) As Double
        '//Inverse of poisson. Calculates mean such that poisson(k,mean,mean-k)=prob.
        '//Require prob to be in the range 0..1, k should be -1/2 or non-negative
        If (k = 0.0#) Then
            invpoisson = -Math.Log(prob + 9.99988867182683E-321)
        ElseIf (prob > 0.5) Then
            invpoisson = invcomppoisson(k, 1.0# - prob)
        Else '/*if (k > 0#)*/ then
            Dim temp2 As Double, xp As Double, dfm As Double, Q As Double, qdif As Double, lpr As Double, small As Double, smalllpr As Double
            lpr = -Math.Log(prob)
            small = 0.00000000000001
            smalllpr = small * lpr * prob
            xp = invcnormal(prob)
            dfm = 0.5 * xp * (xp - Math.Sqrt(4.0# * k + xp * xp))
            Q = -1.0#
            qdif = -dfm
            If Math.Abs(qdif) < 1.0# Then
                qdif = 1.0#
            ElseIf (k > 1.0E+50) Then
                invpoisson = k
                Exit Function
            End If
            While ((Math.Abs(Q - prob) > smalllpr) And (Math.Abs(qdif) > (1.0# + Math.Abs(dfm)) * small))
                Q = cpoisson(k, k + dfm, dfm)
                If (Q = 0.0#) Then
                    qdif = qdif / 2.0#
                    dfm = dfm + qdif
                    Q = -1.0#
                Else
                    temp2 = poissonTerm(k, k + dfm, dfm, 0.0#)
                    If (temp2 = 0.0#) Then
                        qdif = qdif / 2.0#
                        dfm = dfm + qdif
                        Q = -1.0#
                    Else
                        qdif = -2.0# * Q * logdif(Q, prob) / (1.0# + Math.Sqrt(Math.Log(prob) / Math.Log(Q))) / temp2
                        If (qdif > k + dfm) Then
                            qdif = dfm / 2.0#
                            dfm = dfm - qdif
                            Q = -1.0#
                        Else
                            dfm = dfm - qdif
                        End If
                    End If
                End If
            End While
            invpoisson = k + dfm
        End If
    End Function

    Private Function invcomppoisson(ByVal k As Double, ByVal prob As Double) As Double
        '//Inverse of comppoisson. Calculates mean such that comppoisson(k,mean,mean-k)=prob.
        '//Require prob to be in the range 0..1, k should be -1/2 or non-negative
        If (prob > 0.5) Then
            invcomppoisson = invpoisson(k, 1.0# - prob)
        ElseIf (k = 0.0#) Then
            invcomppoisson = -log0(-prob)
        Else '/*if (k > 0#)*/ then
            Dim temp2 As Double, xp As Double, dfm As Double, Q As Double, qdif As Double, Lambda As Double, qdifset As Boolean, lpr As Double, small As Double, smalllpr As Double
            lpr = -Math.Log(prob)
            small = 0.00000000000001
            smalllpr = small * lpr * prob
            xp = invcnormal(prob)
            dfm = 0.5 * xp * (xp + Math.Sqrt(4.0# * k + xp * xp))
            Lambda = k + dfm
            If ((Lambda < 1.0#) And (k < 40.0#)) Then
                Lambda = Math.Exp(Math.Log(prob / poissonTerm(k + 1.0#, 1.0#, -k, 0.0#)) / (k + 1.0#))
                dfm = Lambda - k
            ElseIf (k > 1.0E+50) Then
                invcomppoisson = Lambda
                Exit Function
            End If
            Q = -1.0#
            qdif = Lambda
            qdifset = False
            While ((Math.Abs(Q - prob) > smalllpr) And (Math.Abs(qdif) > Min(Lambda, Math.Abs(dfm)) * small))
                Q = comppoisson(k, Lambda, dfm)
                If (Q = 0.0#) Then
                    If qdifset Then
                        qdif = qdif / 2.0#
                        dfm = dfm + qdif
                        Lambda = Lambda + qdif
                    Else
                        Lambda = 2.0# * Lambda
                        qdif = qdif * 2.0#
                        dfm = Lambda - k
                    End If
                    Q = -1.0#
                Else
                    temp2 = poissonTerm(k, Lambda, dfm, 0.0#)
                    If (temp2 = 0.0#) Then
                        If qdifset Then
                            qdif = qdif / 2.0#
                            dfm = dfm + qdif
                            Lambda = Lambda + qdif
                        Else
                            Lambda = 2.0# * Lambda
                            qdif = qdif * 2.0#
                            dfm = Lambda - k
                        End If
                        Q = -1.0#
                    Else
                        qdif = 2.0# * Q * logdif(Q, prob) / (1.0# + Math.Sqrt(Math.Log(prob) / Math.Log(Q))) / temp2
                        If (qdif > Lambda) Then
                            Lambda = Lambda / 10.0#
                            qdif = dfm
                            dfm = Lambda - k
                            qdif = qdif - dfm
                            Q = -1.0#
                        Else
                            Lambda = Lambda - qdif
                            dfm = dfm - qdif
                        End If
                        qdifset = True
                    End If
                End If
                If (Math.Abs(dfm) > Lambda) Then
                    dfm = Lambda - k
                Else
                    Lambda = k + dfm
                End If
            End While
            invcomppoisson = Lambda
        End If
    End Function

    Private Function binomialTerm(ByVal i As Double, ByVal j As Double, ByVal p As Double, ByVal Q As Double, ByVal diffFromMean As Double, ByVal logAdd As Double) As Double
        '//Probability that binomial variate with sample size i+j and event prob p (=1-q) has value i (diffFromMean = (i+j)*p-i)
        Dim c1 As Double, c2 As Double, c3 As Double
        Dim c4 As Double, c5 As Double, c6 As Double, ps As Double, logbinomialTerm As Double, dfm As Double
        If ((i = 0.0#) And (j <= 0.0#)) Then
            binomialTerm = Math.Exp(logAdd)
        ElseIf ((i <= -1.0#) Or (j < 0.0#)) Then
            binomialTerm = 0.0#
        Else
            c1 = (i + 1.0#) + j
            If (p < Q) Then
                c2 = i
                c3 = j
                ps = p
                dfm = diffFromMean
            Else
                c3 = i
                c2 = j
                ps = Q
                dfm = -diffFromMean
            End If

            c5 = (dfm - (1.0# - ps)) / (c2 + 1.0#)
            c6 = -(dfm + ps) / (c3 + 1.0#)

            If (c5 < minLog1Value) Then
                If (c2 = 0.0#) Then
                    logbinomialTerm = c3 * log0(-ps)
                    binomialTerm = Math.Exp(logbinomialTerm + logAdd)
                ElseIf ((ps = 0.0#) And (c2 > 0.0#)) Then
                    binomialTerm = 0.0#
                Else
                    c4 = logfbit(i + j) - logfbit(i) - logfbit(j)
                    logbinomialTerm = c4 + c2 * (Math.Log((ps * c1) / (c2 + 1.0#)) - c5) - c5 + c3 * log1(c6) - c6
                    binomialTerm = Math.Exp(logbinomialTerm + logAdd) * Math.Sqrt(c1 / ((c2 + 1.0#) * (c3 + 1.0#))) * OneOverSqrTwoPi
                End If
            Else
                c4 = logfbit(i + j) - logfbit(i) - logfbit(j)
                logbinomialTerm = c4 + (c2 * log1(c5) - c5) + (c3 * log1(c6) - c6)
                binomialTerm = Math.Exp(logbinomialTerm + logAdd) * Math.Sqrt((c1 / (c3 + 1.0#)) / ((c2 + 1.0#))) * OneOverSqrTwoPi
            End If
        End If
    End Function

    Private Function binomialcf(ByVal ii As Double, ByVal jj As Double, ByVal pp As Double, ByVal qq As Double, ByVal diffFromMean As Double, ByVal comp As Boolean) As Double
        '//Probability that binomial variate with sample size ii+jj and event prob pp (=1-qq) has value <=i (diffFromMean = (ii+jj)*pp-ii). If comp the returns 1 - probability.
        Dim prob As Double, p As Double, Q As Double, a1 As Double, a2 As Double, b1 As Double, b2 As Double
        Dim c1 As Double, c2 As Double, c3 As Double, c4 As Double, n1 As Double, q1 As Double, dfm As Double
        Dim i As Double, j As Double, ni As Double, nj As Double, Numb As Double, ip1 As Double, cfValue As Double
        Dim swapped As Boolean, exact As Boolean

        If ((ii > -1.0#) And (ii < 0.0#)) Then
            ip1 = -ii
            ii = ip1 - 1.0#
        Else
            ip1 = ii + 1.0#
        End If
        n1 = (ii + 3.0#) + jj
        If ii < 0.0# Then
            cfValue = cfVSmall
            swapped = False
        ElseIf pp > qq Then
            cfValue = cfSmall
            swapped = n1 * qq >= jj + 1.0#
        Else
            cfValue = cfSmall
            swapped = n1 * pp <= ii + 2.0#
        End If
        If Not swapped Then
            i = ii
            j = jj
            p = pp
            Q = qq
            dfm = diffFromMean
        Else
            j = ip1
            ip1 = jj
            i = jj - 1.0#
            p = qq
            Q = pp
            dfm = 1.0# - diffFromMean
        End If
        If ((i > -1.0#) And ((j <= 0.0#) Or (p = 0.0#))) Then
            exact = True
            prob = 1.0#
        ElseIf ((i > -1.0#) And (i < 0.0#) Or (i = -1.0#) And (ip1 > 0.0#)) Then
            exact = False
            prob = binomialTerm(ip1, j, p, Q, (ip1 + j) * p - ip1, 0.0#) * ip1 / ((ip1 + j) * p)
            dfm = (i + j) * p - i
        Else
            exact = ((i = 0.0#) And (j <= 0.0#)) Or ((i <= -1.0#) Or (j < 0.0#))
            prob = binomialTerm(i, j, p, Q, dfm, 0.0#)
        End If
        If (exact) Or (prob = 0.0#) Then
            If (swapped = comp) Then
                binomialcf = prob
            Else
                binomialcf = 1.0# - prob
            End If
            Exit Function
        End If

        Dim sumAlways As Long, sumFactor As Long
        sumAlways = 0
        sumFactor = 6
        a1 = 0.0#
        If (i > sumAlways) Then
            Numb = Int(sumFactor * Math.Sqrt(p + 0.5) * Math.Exp(Math.Log(n1 * p * Q) / 3))
            Numb = Int(Numb - dfm)
            If (Numb > i) Then
                Numb = Int(i)
            End If
        Else
            Numb = Int(i)
        End If
        If (Numb < 0.0#) Then
            Numb = 0.0#
        End If

        b1 = 1.0#
        q1 = Q + 1.0#
        a2 = (i - Numb) * Q
        b2 = dfm + Numb + 1.0#
        c1 = 0.0#
        c2 = a2
        c4 = b2
        While ((Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfValue * b1 * a2)))

            c1 = c1 + 1.0#
            c2 = c2 - Q
            c3 = c1 * c2
            c4 = c4 + q1
            a1 = c4 * a2 + c3 * a1
            b1 = c4 * b2 + c3 * b1
            c1 = c1 + 1.0#
            c2 = c2 - Q
            c3 = c1 * c2
            c4 = c4 + q1
            a2 = c4 * a1 + c3 * a2
            b2 = c4 * b1 + c3 * b2
            If (Math.Abs(b2) > scalefactor) Then
                a1 = a1 * scalefactor2
                b1 = b1 * scalefactor2
                a2 = a2 * scalefactor2
                b2 = b2 * scalefactor2
            ElseIf (Math.Abs(b2) < scalefactor2) Then
                a1 = a1 * scalefactor
                b1 = b1 * scalefactor
                a2 = a2 * scalefactor
                b2 = b2 * scalefactor
            End If
            If c2 < 0.0# And cfValue > cfVSmall Then
                cfValue = cfVSmall
            End If
        End While
        a1 = a2 / b2

        ni = (i - Numb + 1.0#) * Q
        nj = (j + Numb) * p
        While (Numb > 0.0#)
            a1 = (1.0# + a1) * (ni / nj)
            ni = ni + Q
            nj = nj - p
            Numb = Numb - 1.0#
        End While

        a1 = (1.0# + a1) * prob
        If (swapped = comp) Then
            binomialcf = a1
        Else
            binomialcf = 1.0# - a1
        End If

    End Function

    Private Function binApprox(ByVal a As Double, ByVal B As Double, ByVal diffFromMean As Double, ByVal comp As Boolean) As Double
        '//Asymptotic expansion to calculate the probability that binomial variate has value <= a (diffFromMean = (a+b)*p-a). If comp then calulate 1-probability.
        '//cf. http://members.aol.com/iandjmsmith/BinomialApprox.htm
        Dim N As Double, n1 As Double
        Dim pq1 As Double, mfac As Double, Res As Double, tp As Double, lval As Double, lvv As Double, temp As Double
        Dim ib05 As Double, ib15 As Double, ib25 As Double, ib35 As Double, ib45 As Double, ib55 As Double, ib65 As Double
        Dim ib2 As Double, ib3 As Double, ib4 As Double, ib5 As Double, ib6 As Double, ib7 As Double
        Dim elfb As Double, coef15 As Double, coef25 As Double, coef35 As Double, coef45 As Double, coef55 As Double, coef65 As Double
        Dim coef2 As Double, coef3 As Double, coef4 As Double, coef5 As Double, coef6 As Double, coef7 As Double
        Dim tdistDensity As Double, approxtdistDens As Double

        N = a + B
        n1 = N + 1.0#
        lvv = (B + diffFromMean) / n1 - diffFromMean
        lval = (a * log1(lvv / a) + B * log1(-lvv / B)) / N
        tp = -expm1(lval)

        pq1 = (a / N) * (B / N)

        coef15 = (-17.0# * pq1 + 2.0#) / 24.0#
        coef25 = ((-503.0# * pq1 + 76.0#) * pq1 + 4.0#) / 1152.0#
        coef35 = (((-315733.0# * pq1 + 53310.0#) * pq1 + 8196.0#) * pq1 - 1112.0#) / 414720.0#
        coef45 = (4059192.0# + pq1 * (15386296.0# - 85262251.0# * pq1))
        coef45 = (-9136.0# + pq1 * (-697376 + pq1 * coef45)) / 39813120.0#
        coef55 = (3904584040.0# + pq1 * (10438368262.0# - 55253161559.0# * pq1))
        coef55 = (5244128.0# + pq1 * (-43679536.0# + pq1 * (-703410640.0# + pq1 * coef55))) / 6688604160.0#
        coef65 = (-3242780782432.0# + pq1 * (18320560326516.0# + pq1 * (38020748623980.0# - 194479285104469.0# * pq1)))
        coef65 = (335796416.0# + pq1 * (61701376704.0# + pq1 * (-433635420336.0# + pq1 * coef65))) / 4815794995200.0#
        elfb = (((((coef65 / ((N + 6.5) * pq1) + coef55) / ((N + 5.5) * pq1) + coef45) / ((N + 4.5) * pq1) + coef35) / ((N + 3.5) * pq1) + coef25) / ((N + 2.5) * pq1) + coef15) / ((N + 1.5) * pq1) + 1.0#

        coef2 = (-pq1 - 2.0#) / 135.0#
        coef3 = ((-44.0# * pq1 - 86.0#) * pq1 + 4.0#) / 2835.0#
        coef4 = (((-404.0# * pq1 - 786.0#) * pq1 + 48.0#) * pq1 + 8.0#) / 8505.0#
        coef5 = (((((-2421272.0# * pq1 - 4721524.0#) * pq1 + 302244.0#) * pq1) + 118160.0#) * pq1 - 4496.0#) / 12629925.0#
        coef6 = ((((((-473759128.0# * pq1 - 928767700.0#) * pq1 + 57300188.0#) * pq1) + 38704888.0#) * pq1 - 1870064.0#) * pq1 - 167072.0#) / 492567075.0#
        coef7 = (((((((-8530742848.0# * pq1 - 16836643200.0#) * pq1 + 954602040.0#) * pq1) + 990295352.0#) * pq1 - 44963088.0#) * pq1 - 11596512.0#) * pq1 + 349376.0#) / 1477701225.0#

        ib05 = tdistexp(tp, 1.0# - tp, n1 * lval, 2.0# * n1, tdistDensity)
        mfac = n1 * tp
        ib15 = Math.Sqrt(2.0# * mfac)

        If (mfac > 1.0E+50) Then
            ib2 = (1.0# + mfac) / (N + 2.0#)
            mfac = mfac * tp / 2.0#
            ib3 = (ib2 + mfac) / (N + 3.0#)
            mfac = mfac * tp / 3.0#
            ib4 = (ib3 + mfac) / (N + 4.0#)
            mfac = mfac * tp / 4.0#
            ib5 = (ib4 + mfac) / (N + 5.0#)
            mfac = mfac * tp / 5.0#
            ib6 = (ib5 + mfac) / (N + 6.0#)
            mfac = mfac * tp / 6.0#
            ib7 = (ib6 + mfac) / (N + 7.0#)
            Res = (ib2 * coef2 + (ib3 * coef3 + (ib4 * coef4 + (ib5 * coef5 + (ib6 * coef6 + ib7 * coef7 / pq1) / pq1) / pq1) / pq1) / pq1) / pq1

            mfac = (N + 1.5) * tp * twoThirds
            ib25 = (1.0# + mfac) / (N + 2.5)
            mfac = mfac * tp * twoFifths
            ib35 = (ib25 + mfac) / (N + 3.5)
            mfac = mfac * tp * twoSevenths
            ib45 = (ib35 + mfac) / (N + 4.5)
            mfac = mfac * tp * twoNinths
            ib55 = (ib45 + mfac) / (N + 5.5)
            mfac = mfac * tp * twoElevenths
            ib65 = (ib55 + mfac) / (N + 6.5)
            temp = (((((coef65 * ib65 / pq1 + coef55 * ib55) / pq1 + coef45 * ib45) / pq1 + coef35 * ib35) / pq1 + coef25 * ib25) / pq1 + coef15)
        Else
            ib2 = 1.0# + mfac
            mfac = mfac * (N + 2.0#) * tp / 2.0#
            ib3 = ib2 + mfac
            mfac = mfac * (N + 3.0#) * tp / 3.0#
            ib4 = ib3 + mfac
            mfac = mfac * (N + 4.0#) * tp / 4.0#
            ib5 = ib4 + mfac
            mfac = mfac * (N + 5.0#) * tp / 5.0#
            ib6 = ib5 + mfac
            mfac = mfac * (N + 6.0#) * tp / 6.0#
            ib7 = ib6 + mfac
            Res = (ib2 * coef2 + (ib3 * coef3 + (ib4 * coef4 + (ib5 * coef5 + (ib6 * coef6 + ib7 * coef7 / ((N + 7.0#) * pq1)) / ((N + 6.0#) * pq1)) / ((N + 5.0#) * pq1)) / ((N + 4.0#) * pq1)) / ((N + 3.0#) * pq1)) / ((N + 2.0#) * pq1)

            mfac = (N + 1.5) * tp * twoThirds
            ib25 = 1.0# + mfac
            mfac = mfac * (N + 2.5) * tp * twoFifths
            ib35 = ib25 + mfac
            mfac = mfac * (N + 3.5) * tp * twoSevenths
            ib45 = ib35 + mfac
            mfac = mfac * (N + 4.5) * tp * twoNinths
            ib55 = ib45 + mfac
            mfac = mfac * (N + 5.5) * tp * twoElevenths
            ib65 = ib55 + mfac
            temp = (((((coef65 * ib65 / ((N + 6.5) * pq1) + coef55 * ib55) / ((N + 5.5) * pq1) + coef45 * ib45) / ((N + 4.5) * pq1) + coef35 * ib35) / ((N + 3.5) * pq1) + coef25 * ib25) / ((N + 2.5) * pq1) + coef15)
        End If

        approxtdistDens = tdistDensity / Math.Sqrt(1.0# - tp)
        temp = ib15 * temp / ((N + 1.5) * pq1)
        Res = (oneThird + Res) * 2.0# * (a - B) / (N * Math.Sqrt(n1 * pq1))
        If (comp) Then
            If (lvv > 0.0#) Then
                binApprox = ib05 - (Res - temp) * approxtdistDens / elfb
            Else
                binApprox = (1.0# - ib05) - (Res + temp) * approxtdistDens / elfb
            End If
        ElseIf (lvv > 0.0#) Then
            binApprox = (1.0# - ib05) + (Res - temp) * approxtdistDens / elfb
        Else
            binApprox = ib05 + (Res + temp) * approxtdistDens / elfb
        End If
    End Function

    Private Function binomial(ByVal ii As Double, ByVal jj As Double, ByVal pp As Double, ByVal qq As Double, ByVal diffFromMean As Double) As Double
        '//Probability that binomial variate with sample size ii+jj and event prob pp (=1-qq) has value <=i (diffFromMean = (ii+jj)*pp-ii).
        Dim mij As Double
        mij = Min(ii, jj)
        If ((mij > 50.0#) And (Math.Abs(diffFromMean) < (0.1 * mij))) Then
            binomial = binApprox(jj - 1.0#, ii, diffFromMean, False)
        Else
            binomial = binomialcf(ii, jj, pp, qq, diffFromMean, False)
        End If
    End Function

    Private Function compbinomial(ByVal ii As Double, ByVal jj As Double, ByVal pp As Double, ByVal qq As Double, ByVal diffFromMean As Double) As Double
        '//Probability that binomial variate with sample size ii+jj and event prob pp (=1-qq) has value >i (diffFromMean = (ii+jj)*pp-ii).
        Dim mij As Double
        mij = Min(ii, jj)
        If ((mij > 50.0#) And (Math.Abs(diffFromMean) < (0.1 * mij))) Then
            compbinomial = binApprox(jj - 1.0#, ii, diffFromMean, True)
        Else
            compbinomial = binomialcf(ii, jj, pp, qq, diffFromMean, True)
        End If
    End Function

    Private Function invbinom(ByVal k As Double, ByVal M As Double, ByVal prob As Double, ByRef oneMinusP As Double) As Double
        '//Inverse of binomial. Delivers event probability p (q held in oneMinusP in case required) so that binomial(k,m,p,oneMinusp,dfm) = prob.
        '//Note that dfm is calculated accurately but never made available outside of this routine.
        '//Require prob to be in the range 0..1, m should be positive and k should be >= 0
        Dim temp1 As Double, temp2 As Double
        If (prob > 0.5) Then
            temp2 = invcompbinom(k, M, 1.0# - prob, oneMinusP)
        Else
            temp1 = invcompbinom(M - 1.0#, k + 1.0#, prob, oneMinusP)
            temp2 = oneMinusP
            oneMinusP = temp1
        End If
        invbinom = temp2
    End Function

    Private Function invcompbinom(ByVal k As Double, ByVal M As Double, ByVal prob As Double, ByRef oneMinusP As Double) As Double
        '//Inverse of compbinomial. Delivers event probability p (q held in oneMinusP in case required) so that compbinomial(k,m,p,oneMinusp,dfm) = prob.
        '//Note that dfm is calculated accurately but never made available outside of this routine.
        '//Require prob to be in the range 0..1, m should be positive and k should be >= -0.5
        Dim xp As Double, xp2 As Double, dfm As Double, N As Double, p As Double, Q As Double, pr As Double, dif As Double, temp As Double, temp2 As Double, Result As Double, lpr As Double, small As Double, smalllpr As Double, nminpq As Double
        Result = -1.0#
        N = k + M
        If (prob > 0.5) Then
            Result = invbinom(k, M, 1.0# - prob, oneMinusP)
        ElseIf (k = 0.0#) Then
            Result = log0(-prob) / N
            If (Math.Abs(Result) < 1.0#) Then
                Result = -expm1(Result)
                oneMinusP = 1.0# - Result
            Else
                oneMinusP = Math.Exp(Result)
                Result = 1.0# - oneMinusP
            End If
        ElseIf (M = 1.0#) Then
            Result = Math.Log(prob) / N
            If (Math.Abs(Result) < 1.0#) Then
                oneMinusP = -expm1(Result)
                Result = 1.0# - oneMinusP
            Else
                Result = Math.Exp(Result)
                oneMinusP = 1.0# - Result
            End If
        Else
            pr = -1.0#
            xp = invcnormal(prob)
            xp2 = xp * xp
            temp = 2.0# * xp * Math.Sqrt(k * (M / N) + xp2 / 4.0#)
            xp2 = xp2 / N
            dfm = (xp2 * (M - k) + temp) / (2.0# * (1.0# + xp2))
            If (k + dfm < 0.0#) Then
                dfm = -k
            End If
            Q = (M - dfm) / N
            p = (k + dfm) / N
            dif = -dfm / N
            If (dif = 0.0#) Then
                dif = 1.0#
            ElseIf Min(k, M) > 1.0E+50 Then
                oneMinusP = Q
                invcompbinom = p
                Exit Function
            End If
            lpr = -Math.Log(prob)
            small = 0.00000000000004
            smalllpr = small * lpr * prob
            nminpq = N * Min(p, Q)
            While ((Math.Abs(pr - prob) > smalllpr) And (N * Math.Abs(dif) > Min(Math.Abs(dfm), nminpq) * small))
                pr = compbinomial(k, M, p, Q, dfm)
                If (pr < nearly_zero) Then '/*Should not be happenning often */
                    dif = dif / 2.0#
                    dfm = dfm + N * dif
                    p = p + dif
                    Q = Q - dif
                    pr = -1.0#
                Else
                    temp2 = binomialTerm(k, M, p, Q, dfm, 0.0#) * M / Q
                    If (temp2 < nearly_zero) Then '/*Should not be happenning often */
                        dif = dif / 2.0#
                        dfm = dfm + N * dif
                        p = p + dif
                        Q = Q - dif
                        pr = -1.0#
                    Else
                        dif = 2.0# * pr * logdif(pr, prob) / (1.0# + Math.Sqrt(Math.Log(prob) / Math.Log(pr))) / temp2
                        If (Q + dif <= 0.0#) Then '/*not v. good */
                            dif = -0.9999 * Q
                            dfm = dfm - N * dif
                            p = p - dif
                            Q = Q + dif
                            pr = -1.0#
                        ElseIf (p - dif <= 0.0#) Then '/*v. good */
                            temp = Math.Exp(Math.Log(prob / pr) / (k + 1.0#))
                            dif = p
                            p = temp * p
                            dif = p - dif
                            dfm = N * p - k
                            Q = 1.0# - p
                            pr = -1.0#
                        Else
                            dfm = dfm - N * dif
                            p = p - dif
                            Q = Q + dif
                        End If
                    End If
                End If
            End While
            Result = p
            oneMinusP = Q
        End If
        invcompbinom = Result
    End Function

    Private Function abMinuscd(ByVal a As Double, ByVal B As Double, ByVal C As Double, ByVal D As Double) As Double
        Dim a1 As Double, b1 As Double, c1 As Double, d1 As Double, a2 As Double, b2 As Double, c2 As Double, d2 As Double, r1 As Double, r2 As Double, r3 As Double
        a2 = Int(a / twoTo27) * twoTo27
        a1 = a - a2
        b2 = Int(B / twoTo27) * twoTo27
        b1 = B - b2
        c2 = Int(C / twoTo27) * twoTo27
        c1 = C - c2
        d2 = Int(D / twoTo27) * twoTo27
        d1 = D - d2
        r1 = a1 * b1 - c1 * d1
        r2 = (a2 * b1 - c1 * d2) + (a1 * b2 - c2 * d1)
        r3 = a2 * b2 - c2 * d2
        If (r3 < 0.0#) = (r2 < 0.0#) Then
            abMinuscd = r3 + (r2 + r1)
        Else
            abMinuscd = (r3 + r2) + r1
        End If
    End Function

    Private Function aTimes2Powerb(ByVal a As Double, ByVal B As Integer) As Double
        If B > 709 Then
            a = (a * scalefactor) * scalefactor
            B = B - 512
        ElseIf B < -709 Then
            a = (a * scalefactor2) * scalefactor2
            B = B + 512
        End If
        aTimes2Powerb = a * (2.0#) ^ B
    End Function

    Private Function GeneralabMinuscd(ByVal a As Double, ByVal B As Double, ByVal C As Double, ByVal D As Double) As Double
        Dim S As Double, ca As Double, cb As Double, CC As Double, cd As Double
        Dim l2 As Integer, pa As Integer, pb As Integer, pc As Integer, pd As Integer
        S = a * B - C * D
        If a <= 0.0# Or B <= 0.0# Or C <= 0.0# Or D <= 0.0# Then
            GeneralabMinuscd = S
            Exit Function
        ElseIf S < 0.0# Then
            GeneralabMinuscd = -GeneralabMinuscd(C, D, a, B)
            Exit Function
        End If
        l2 = Int(Math.Log(a) / Math.Log(2))
        pa = 51 - l2
        ca = aTimes2Powerb(a, pa)
        l2 = Int(Math.Log(B) / Math.Log(2))
        pb = 51 - l2
        cb = aTimes2Powerb(B, pb)
        l2 = Int(Math.Log(C) / Math.Log(2))
        pc = 51 - l2
        CC = aTimes2Powerb(C, pc)
        pd = pa + pb - pc
        cd = aTimes2Powerb(D, pd)
        GeneralabMinuscd = aTimes2Powerb(abMinuscd(ca, cb, CC, cd), -(pa + pb))
    End Function

    Private Function hypergeometricTerm(ByVal Ai As Double, ByVal aji As Double, ByVal aki As Double, ByVal amkji As Double) As Double
        '// Probability that hypergeometric variate from a population with total type Is of aki+ai, total type IIs of amkji+aji, has ai type Is and aji type IIs selected.
        Dim aj As Double, am As Double, ak As Double, amj As Double, amk As Double
        Dim cjkmi As Double, ai1 As Double, aj1 As Double, ak1 As Double, am1 As Double, aki1 As Double, aji1 As Double, amk1 As Double, amj1 As Double, amkji1 As Double
        Dim c1 As Double, c3 As Double, c4 As Double, c5 As Double, loghypergeometricTerm As Double

        ak = aki + Ai
        amk = amkji + aji
        aj = aji + Ai
        am = amk + ak
        amj = amkji + aki
        If (am > max_discrete) Then
            hypergeometricTerm = ErrorValue
            Exit Function
        End If
        If ((Ai = 0.0#) And ((aji <= 0.0#) Or (aki <= 0.0#) Or (amj < 0.0#) Or (amk < 0.0#))) Then
            hypergeometricTerm = 1.0#
        ElseIf ((Ai > 0.0#) And (Min(aki, aji) = 0.0#) And (max(amj, amk) = 0.0#)) Then
            hypergeometricTerm = 1.0#
        ElseIf ((Ai >= 0.0#) And (amkji > -1.0#) And (aki > -1.0#) And (aji >= 0.0#)) Then
            c1 = logfbit(amkji) + logfbit(aki) + logfbit(aji) + logfbit(am) + logfbit(Ai)
            c1 = logfbit(amk) + logfbit(ak) + logfbit(aj) + logfbit(amj) - c1
            ai1 = Ai + 1.0#
            aj1 = aj + 1.0#
            ak1 = ak + 1.0#
            am1 = am + 1.0#
            aki1 = aki + 1.0#
            aji1 = aji + 1.0#
            amk1 = amk + 1.0#
            amj1 = amj + 1.0#
            amkji1 = amkji + 1.0#
            cjkmi = GeneralabMinuscd(aji, aki, Ai, amkji)
            c5 = (cjkmi - Ai) / (amkji1 * am1)
            If (c5 < minLog1Value) Then
                c3 = amkji * (Math.Log((amj1 * amk1) / (amkji1 * am1)) - c5) - c5
            Else
                c3 = amkji * log1(c5) - c5
            End If

            c5 = (-cjkmi - aji) / (aki1 * am1)
            If (c5 < minLog1Value) Then
                c4 = aki * (Math.Log((ak1 * amj1) / (aki1 * am1)) - c5) - c5
            Else
                c4 = aki * log1(c5) - c5
            End If

            c3 = c3 + c4
            c5 = (-cjkmi - aki) / (aji1 * am1)
            If (c5 < minLog1Value) Then
                c4 = aji * (Math.Log((aj1 * amk1) / (aji1 * am1)) - c5) - c5
            Else
                c4 = aji * log1(c5) - c5
            End If

            c3 = c3 + c4
            c5 = (cjkmi - amkji) / (ai1 * am1)
            If (c5 < minLog1Value) Then
                c4 = Ai * (Math.Log((aj1 * ak1) / (ai1 * am1)) - c5) - c5
            Else
                c4 = Ai * log1(c5) - c5
            End If

            c3 = c3 + c4
            loghypergeometricTerm = (c1 + 1.0# / am1) + c3

            hypergeometricTerm = Math.Exp(loghypergeometricTerm) * Math.Sqrt((amk1 * ak1) * (aj1 * amj1) / ((amkji1 * aki1 * aji1) * (am1 * ai1))) * OneOverSqrTwoPi
        Else
            hypergeometricTerm = 0.0#
        End If

    End Function

    Private Function hypergeometric(ByVal Ai As Double, ByVal aji As Double, ByVal aki As Double, ByVal amkji As Double, ByVal comp As Boolean, ByRef ha1 As Double, ByRef hprob As Double, ByRef hswap As Boolean) As Double
        '// Probability that hypergeometric variate from a population with total type Is of aki+ai, total type IIs of amkji+aji, has up to ai type Is selected in a sample of size aji+ai.
        Dim prob As Double
        Dim a1 As Double, a2 As Double, b1 As Double, b2 As Double, an As Double, bn As Double, bnAdd As Double
        Dim c1 As Double, c2 As Double, c3 As Double, c4 As Double
        Dim i As Double, ji As Double, ki As Double, mkji As Double, njj As Double, Numb As Double, maxSums As Double, swapped As Boolean
        Dim ip1 As Double, must_do_cf As Boolean, allIntegral As Boolean, exact As Boolean
        If (amkji > -1.0#) And (amkji < 0.0#) Then
            ip1 = -amkji
            mkji = ip1 - 1.0#
            allIntegral = False
        Else
            ip1 = amkji + 1.0#
            mkji = amkji
            allIntegral = Ai = Int(Ai) And aji = Int(aji) And aki = Int(aki) And mkji = Int(mkji)
        End If

        If allIntegral Then
            swapped = (Ai + 0.5) * (mkji + 0.5) >= (aki - 0.5) * (aji - 0.5)
        ElseIf Ai < 100.0# And Ai = Int(Ai) Or mkji < 0.0# Then
            swapped = (Ai + 0.5) * (mkji + 0.5) >= aki * aji + 1000.0#
        ElseIf Ai < 1.0# Then
            swapped = (Ai + 0.5) * (mkji + 0.5) >= aki * aji
        ElseIf aji < 1.0# Or aki < 1.0# Or (Ai < 1.0# And Ai > 0.0#) Then
            swapped = False
        Else
            swapped = (Ai + 0.5) * (mkji + 0.5) >= (aki - 0.5) * (aji - 0.5)
        End If
        If Not swapped Then
            i = Ai
            ji = aji
            ki = aki
        Else
            i = aji - 1.0#
            ji = Ai + 1.0#
            ki = ip1
            ip1 = aki
            mkji = aki - 1.0#
        End If
        c2 = ji + i
        c4 = mkji + ki + c2
        If (c4 > max_discrete) Then
            hypergeometric = ErrorValue
            Exit Function
        End If
        If ((i >= 0.0#) And (ji <= 0.0#) Or (ki <= 0.0#) Or (ip1 + ki <= 0.0#) Or (ip1 + ji <= 0.0#)) Then
            exact = True
            If (i >= 0.0#) Then
                prob = 1.0#
            Else
                prob = 0.0#
            End If
        ElseIf (ip1 > 0.0#) And (ip1 < 1.0#) Then
            exact = False
            prob = hypergeometricTerm(i, ji, ki, ip1) * (ip1 * (c4 + 1.0#)) / ((ki + ip1) * (ji + ip1))
        Else
            exact = ((i = 0.0#) And ((ji <= 0.0#) Or (ki <= 0.0#) Or (mkji + ki < 0.0#) Or (mkji + ji < 0.0#))) Or ((i > 0.0#) And (Min(ki, ji) = 0.0#) And (max(mkji + ki, mkji + ji) = 0.0#))
            prob = hypergeometricTerm(i, ji, ki, mkji)
        End If
        hprob = prob
        hswap = swapped
        ha1 = 0.0#

        If (exact) Or (prob = 0.0#) Then
            If (swapped = comp) Then
                hypergeometric = prob
            Else
                hypergeometric = 1.0# - prob
            End If
            Exit Function
        End If

        a1 = 0.0#
        Dim sumAlways As Long, sumFactor As Long
        sumAlways = 0.0#
        sumFactor = 10.0#

        If i < mkji Then
            must_do_cf = i <> Int(i)
            maxSums = Int(i)
        Else
            must_do_cf = mkji <> Int(mkji)
            maxSums = Int(max(mkji, 0.0#))
        End If
        If must_do_cf Then
            sumAlways = 0.0#
            sumFactor = 5.0#
        Else
            sumAlways = 20.0#
            sumFactor = 10.0#
        End If
        If (maxSums > sumAlways Or must_do_cf) Then
            Numb = Int(sumFactor / c4 * Math.Exp(Math.Log((ki + i) * (ji + i) * (ip1 + ji) * (ip1 + ki)) / 3.0#))
            Numb = Int(i - (ki + i) * (ji + i) / c4 + Numb)
            If (Numb < 0.0#) Then
                Numb = 0.0#
            ElseIf Numb > maxSums Then
                Numb = maxSums
            End If
        Else
            Numb = maxSums
        End If

        If (2.0# * Numb <= maxSums Or must_do_cf) Then
            b1 = 1.0#
            c1 = 0.0#
            c2 = i - Numb
            c3 = mkji - Numb
            a2 = c2 * c3
            c3 = c3 - 1.0#
            b2 = GeneralabMinuscd(ki + Numb + 1.0#, ji + Numb + 1.0#, c2 - 1.0#, c3)
            bn = b2
            bnAdd = c3 + c4 + c2 - 2.0#
            While (b2 > 0.0# And (Math.Abs(a2 * b1 - a1 * b2) > Math.Abs(cfVSmall * b1 * a2)))
                c1 = c1 + 1.0#
                c2 = c2 - 1.0#
                an = (c1 * c2) * (c3 * c4)
                c3 = c3 - 1.0#
                c4 = c4 - 1.0#
                bn = bn + bnAdd
                bnAdd = bnAdd - 4.0#
                a1 = bn * a2 + an * a1
                b1 = bn * b2 + an * b1
                If (b1 > scalefactor) Then
                    a1 = a1 * scalefactor2
                    b1 = b1 * scalefactor2
                    a2 = a2 * scalefactor2
                    b2 = b2 * scalefactor2
                End If
                c1 = c1 + 1.0#
                c2 = c2 - 1.0#
                an = (c1 * c2) * (c3 * c4)
                c3 = c3 - 1.0#
                c4 = c4 - 1.0#
                bn = bn + bnAdd
                bnAdd = bnAdd - 4.0#
                a2 = bn * a1 + an * a2
                b2 = bn * b1 + an * b2
                If (b2 > scalefactor) Then
                    a1 = a1 * scalefactor2
                    b1 = b1 * scalefactor2
                    a2 = a2 * scalefactor2
                    b2 = b2 * scalefactor2
                End If
            End While
            If b1 < 0.0# Or b2 < 0.0# Then
                hypergeometric = ErrorValue
                Exit Function
            Else
                a1 = a2 / b2
            End If
        Else
            Numb = maxSums
        End If

        c1 = i - Numb + 1.0#
        c2 = mkji - Numb + 1.0#
        c3 = ki + Numb
        c4 = ji + Numb
        For njj = 1 To Numb
            a1 = (1.0# + a1) * ((c1 * c2) / (c3 * c4))
            c1 = c1 + 1.0#
            c2 = c2 + 1.0#
            c3 = c3 - 1.0#
            c4 = c4 - 1.0#
        Next njj

        ha1 = a1
        a1 = (1.0# + a1) * prob
        If (swapped = comp) Then
            hypergeometric = a1
        Else
            If a1 > 0.99 Then
                hypergeometric = ErrorValue
            Else
                hypergeometric = 1.0# - a1
            End If
        End If
    End Function

    Private Function compgfunc(ByVal x As Double, ByVal a As Double) As Double
        '//Calculates a*x(1/(a+1) - x/2*(1/(a+2) - x/3*(1/(a+3) - ...)))
        '//Mainly for calculating the complement of gamma(x,a) for small a and x <= 1.
        '//a should be close to 0, x >= 0 & x <=1
        Dim term As Double, D As Double, sum As Double
        term = x
        D = 2.0#
        sum = term / (a + 1.0#)
        While (Math.Abs(term) > Math.Abs(sum * sumAcc))
            term = -term * x / D
            sum = sum + term / (a + D)
            D = D + 1.0#
        End While
        compgfunc = a * sum
    End Function

    Private Function lngammaexpansion(ByVal a As Double) As Double
        '//Calculates log(gamma(a+1)) accurately for for small a (0 < a & a < 0.5).
        '//Uses Abramowitz & Stegun's series 6.1.33
        '//Mainly for calculating the complement of gamma(x,a) for small a and x <= 1.
        '//
        'Dim coeffs() As Double                       ' "Variant" rather than  "coefs(40) as Double"  to permit use of Array assignment
        '// for i < 40 coeffs[i] holds (zeta(i+2)-1)/(i+2), coeffs[40] holds (zeta(i+2)-1)
        Dim coeffs() As Double = { _
        0.322467033424113, 0.0673523010531981, 0.0205808084277845, 0.00738555102867399, _
        0.00289051033074152, 0.00119275391170326, 0.000509669524743042, 0.000223154758453579, _
        0.0000994575127818085, 0.0000449262367381331, 0.0000205072127756707, 0.0000094394882752684, _
        0.00000437486678990749, 0.00000203921575380137, 0.000000955141213040742, 0.000000449246919876457, _
        0.000000212071848055547, 0.000000100432248239681, 0.0000000476981016936398, 0.0000000227110946089432, _
        0.000000010838659214897, 0.00000000518347504197005, 0.00000000248367454380248, 0.00000000119214014058609, _
        0.000000000573136724167886, 0.000000000275952288512423, 0.000000000133047643742445, 0.000000000064229645638381, _
        0.0000000000310442477473223, 0.0000000000150213840807541, 0.00000000000727597448023908, 0.00000000000352774247657592, _
        0.00000000000171199179055962, 0.000000000000831538584142029, 0.000000000000404220052528944, 0.000000000000196647563109662, _
        0.0000000000000957363038783856, 0.0000000000000466407602642837, 0.0000000000000227373696006597, 0.0000000000000110913994708345, _
        0.000000000000227373684582465}
        Dim lgam As Double
        Dim i As Integer
        lgam = coeffs(40) * logcf(-a / 2.0#, 42.0#, 1.0#)
        For i = 39 To 0 Step -1
            lgam = (coeffs(i) - a * lgam)
        Next i
        lngammaexpansion = (a * lgam - eulers_const) * a - log1(a)
    End Function

    Private Function incgamma(ByVal x As Double, ByVal a As Double, ByVal comp As Boolean) As Double
        '//Calculates gamma-cdf for small a (complementary gamma-cdf if comp).
        Dim r As Double
        r = a * Math.Log(x) - lngammaexpansion(a)
        If (comp) Then
            r = -expm1(r)
            incgamma = r + compgfunc(x, a) * (1.0# - r)
        Else
            incgamma = Math.Exp(r) * (1.0# - compgfunc(x, a))
        End If
    End Function

    Private Function invincgamma(ByVal a As Double, ByVal prob As Double, ByVal comp As Boolean) As Double
        '//Calculates inverse of gamma for small a (inverse of complementary gamma if comp).
        Dim ga As Double, x As Double, deriv As Double, Z As Double, W As Double, dif As Double, pr As Double, lpr As Double, small As Double, smalllpr As Double
        If (prob > 0.5) Then
            invincgamma = invincgamma(a, 1.0# - prob, Not comp)
            Exit Function
        End If
        lpr = -Math.Log(prob)
        small = 0.00000000000001
        smalllpr = small * lpr * prob
        If (comp) Then
            ga = -expm1(lngammaexpansion(a))
            x = -Math.Log(prob * (1.0# - ga) / a)
            If (x < 0.5) Then
                pr = Math.Exp(log0(-(ga + prob * (1.0# - ga))) / a)
                If (x < pr) Then
                    x = pr
                End If
            End If
            dif = x
            pr = -1.0#
            While ((Math.Abs(pr - prob) > smalllpr) And (Math.Abs(dif) > small * max(cSmall, x)))
                deriv = poissonTerm(a, x, x - a, 0.0#) * a             'value of derivative is actually deriv/x but it can overflow when x is denormal...
                If (x > 1.0#) Then
                    pr = poisson1(-a, x, 0.0#)
                Else
                    Z = compgfunc(x, a)
                    W = -expm1(a * Math.Log(x))
                    W = Z + W * (1.0# - Z)
                    pr = (W - ga) / (1.0# - ga)
                End If
                dif = x * (pr / deriv) * logdif(pr, prob) '...so multiply by x in slightly different order
                x = x + dif
                If (x < 0.0#) Then
                    invincgamma = 0.0#
                    Exit Function
                End If
            End While
        Else
            ga = Math.Exp(lngammaexpansion(a))
            x = Math.Log(prob * ga)
            If (x < -711.0# * a) Then
                invincgamma = 0.0#
                Exit Function
            End If
            x = Math.Exp(x / a)
            Z = 1.0# - compgfunc(x, a)
            deriv = poissonTerm(a, x, x - a, 0.0#) * a / x
            pr = prob * Z
            dif = (pr / deriv) * logdif(pr, prob)
            x = x - dif
            While ((Math.Abs(pr - prob) > smalllpr) And (Math.Abs(dif) > small * max(cSmall, x)))
                deriv = poissonTerm(a, x, x - a, 0.0#) * a / x
                If (x > 1.0#) Then
                    pr = 1.0# - poisson1(-a, x, 0.0#)
                Else
                    pr = (1.0# - compgfunc(x, a)) * Math.Exp(a * Math.Log(x)) / ga
                End If
                dif = (pr / deriv) * logdif(pr, prob)
                x = x - dif
            End While
        End If
        invincgamma = x
    End Function

    Private Function GAMMA(ByVal N As Double, ByVal a As Double) As Double
        'Assumes n > 0 & a >= 0.  Only called by (comp)gamma_nc with a = 0.
        If (a = 0.0#) Then
            GAMMA = 1.0#
        ElseIf ((a < 1.0#) And (N < 1.0#)) Then
            GAMMA = incgamma(N, a, False)
        ElseIf (a >= 1.0#) Then
            GAMMA = comppoisson(a - 1.0#, N, N - a + 1.0#)
        Else
            GAMMA = 1.0# - poisson1(-a, N, 0.0#)
        End If
    End Function

    Private Function compgamma(ByVal N As Double, ByVal a As Double) As Double
        'Assumes n > 0 & a >= 0. Only called by (comp)gamma_nc with a = 0.
        If (a = 0.0#) Then
            compgamma = 0.0#
        ElseIf ((a < 1.0#) And (N < 1.0#)) Then
            compgamma = incgamma(N, a, True)
        ElseIf (a >= 1.0#) Then
            compgamma = cpoisson(a - 1.0#, N, N - a + 1.0#)
        Else
            compgamma = poisson1(-a, N, 0.0#)
        End If
    End Function

    Private Function invgamma(ByVal a As Double, ByVal prob As Double) As Double
        '//Inverse of gamma(x,a)
        If (a >= 1.0#) Then
            invgamma = invcomppoisson(a - 1.0#, prob)
        Else
            invgamma = invincgamma(a, prob, False)
        End If
    End Function

    Private Function invcompgamma(ByVal a As Double, ByVal prob As Double) As Double
        '//Inverse of compgamma(x,a)
        If (a >= 1.0#) Then
            invcompgamma = invpoisson(a - 1.0#, prob)
        Else
            invcompgamma = invincgamma(a, prob, True)
        End If
    End Function

    Private Function logfbit1dif(ByVal x As Double) As Double
        '// Calculation of logfbit1(x)-logfbit1(1+x).
        logfbit1dif = (logfbitdif(x) - 0.25 / ((x + 1.0#) * (x + 2.0#))) / (x + 1.5)
    End Function

    Private Function logfbit1(ByVal x As Double) As Double
        '// Derivative of error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
        Dim x1 As Double, x2 As Double
        If (x >= 10000000000.0) Then
            logfbit1 = -lfbc1 * (Math.Pow((x + 1.0), -2))
        ElseIf (x >= 6.0#) Then
            Dim x3 As Double
            x1 = x + 1.0#
            x2 = 1.0# / (x1 * x1)
            x3 = (11.0# * lfbc6 - x2 * (13.0# * lfbc7 - x2 * (15.0# * lfbc8 - x2 * 17.0# * lfbc9)))
            x3 = (5.0# * lfbc3 - x2 * (7.0# * lfbc4 - x2 * (9.0# * lfbc5 - x2 * x3)))
            x3 = x2 * (3.0# * lfbc2 - x2 * x3)
            logfbit1 = -lfbc1 * (1.0# - x3) * x2
        ElseIf (x > -1.0#) Then
            x1 = x
            x2 = 0.0#
            While (x1 < 6.0#)
                x2 = x2 + logfbit1dif(x1)
                x1 = x1 + 1.0#
            End While
            logfbit1 = x2 + logfbit1(x1)
        Else
            logfbit1 = -1.0E+308
        End If
    End Function

    Private Function logfbit3dif(ByVal x As Double) As Double
        '// Calculation of logfbit3(x)-logfbit3(1+x).
        logfbit3dif = -(2.0# * x + 3.0#) * Math.Pow(((x + 1.0#) * (x + 2.0#)), -3)
    End Function

    Private Function logfbit3(ByVal x As Double) As Double
        '// Third derivative of error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
        Dim x1 As Double, x2 As Double
        If (x >= 10000000000.0#) Then
            logfbit3 = -0.5 * Math.Pow((x + 1.0#), -4)
        ElseIf (x >= 6.0#) Then
            Dim x3 As Double
            x1 = x + 1.0#
            x2 = 1.0# / (x1 * x1)
            x3 = x2 * (4080.0# * lfbc8 - x2 * 5814.0# * lfbc9)
            x3 = x2 * (1716.0# * lfbc6 - x2 * (2730.0# * lfbc7 - x3))
            x3 = x2 * (504.0# * lfbc4 - x2 * (990.0# * lfbc5 - x3))
            x3 = x2 * (60.0# * lfbc2 - x2 * (210.0# * lfbc3 - x3))
            logfbit3 = -lfbc1 * (6.0# - x3) * x2 * x2
        ElseIf (x > -1.0#) Then
            x1 = x
            x2 = 0.0#
            While (x1 < 6.0#)
                x2 = x2 + logfbit3dif(x1)
                x1 = x1 + 1.0#
            End While
            logfbit3 = x2 + logfbit3(x1)
        Else
            logfbit3 = -1.0E+308
        End If
    End Function

    Private Function logfbit5dif(ByVal x As Double) As Double
        '// Calculation of logfbit5(x)-logfbit5(1+x).
        logfbit5dif = -6.0# * (2.0# * x + 3.0#) * ((5.0# * x + 15.0#) * x + 12.0#) * Math.Pow(((x + 1.0#) * (x + 2.0#)), -5)
    End Function

    Private Function logfbit5(ByVal x As Double) As Double
        '// Fifth derivative of error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
        Dim x1 As Double, x2 As Double
        If (x >= 10000000000.0#) Then
            logfbit5 = -10.0# * Math.Pow((x + 1.0#), -6)
        ElseIf (x >= 6.0#) Then
            Dim x3 As Double
            x1 = x + 1.0#
            x2 = 1.0# / (x1 * x1)
            x3 = x2 * (1395360.0# * lfbc8 - x2 * 2441880.0# * lfbc9)
            x3 = x2 * (360360.0# * lfbc6 - x2 * (742560.0# * lfbc7 - x3))
            x3 = x2 * (55440.0# * lfbc4 - x2 * (154440.0# * lfbc5 - x3))
            x3 = x2 * (2520.0# * lfbc2 - x2 * (15120.0# * lfbc3 - x3))
            logfbit5 = -lfbc1 * (120.0# - x3) * x2 * x2 * x2
        ElseIf (x > -1.0#) Then
            x1 = x
            x2 = 0.0#
            While (x1 < 6.0#)
                x2 = x2 + logfbit5dif(x1)
                x1 = x1 + 1.0#
            End While
            logfbit5 = x2 + logfbit5(x1)
        Else
            logfbit5 = -1.0E+308
        End If
    End Function

    Private Function logfbit7dif(ByVal x As Double) As Double
        '// Calculation of logfbit7(x)-logfbit7(1+x).
        logfbit7dif = -120.0# * (2.0# * x + 3.0#) * ((((14.0# * x + 84.0#) * x + 196.0#) * x + 210.0#) * x + 87.0#) * Math.Pow(((x + 1.0#) * (x + 2.0#)), -7)
    End Function

    Private Function logfbit7(ByVal x As Double) As Double
        '// Seventh derivative of error part of Stirling's formula where log(x!) = log(sqrt(twopi))+(x+0.5)*log(x+1)-(x+1)+logfbit(x).
        Dim x1 As Double, x2 As Double
        If (x >= 10000000000.0#) Then
            logfbit7 = -420.0# * Math.Pow((x + 1.0#), -8)
        ElseIf (x >= 6.0#) Then
            Dim x3 As Double
            x1 = x + 1.0#
            x2 = 1.0# / (x1 * x1)
            x3 = x2 * (586051200.0# * lfbc8 - x2 * 1235591280.0# * lfbc9)
            x3 = x2 * (98017920.0# * lfbc6 - x2 * (253955520.0# * lfbc7 - x3))
            x3 = x2 * (8648640.0# * lfbc4 - x2 * (32432400.0# * lfbc5 - x3))
            x3 = x2 * (181440.0# * lfbc2 - x2 * (1663200.0# * lfbc3 - x3))
            logfbit7 = -lfbc1 * (5040.0# - x3) * x2 * x2 * x2 * x2
        ElseIf (x > -1.0#) Then
            x1 = x
            x2 = 0.0#
            While (x1 < 6.0#)
                x2 = x2 + logfbit7dif(x1)
                x1 = x1 + 1.0#
            End While
            logfbit7 = x2 + logfbit7(x1)
        Else
            logfbit7 = -1.0E+308
        End If
    End Function

    Private Function lfbaccdif(ByVal a As Double, ByVal B As Double) As Double
        '// This is now always reasonably accurate, although it is not always required to be so when called from incbeta.
        If (a > 0.03 * (a + B + 1.0#)) Then
            lfbaccdif = logfbit(a + B) - logfbit(B)
        Else
            Dim a2 As Double, ab2 As Double
            a2 = a * a
            ab2 = a / 2.0# + B
            lfbaccdif = a * (logfbit1(ab2) + a2 / 24.0# * (logfbit3(ab2) + a2 / 80.0# * (logfbit5(ab2) + a2 / 168.0# * logfbit7(ab2))))
        End If
    End Function

    Private Function compbfunc(ByVal x As Double, ByVal a As Double, ByVal B As Double) As Double
        '// Calculates a*(b-1)*x(1/(a+1) - (b-2)*x/2*(1/(a+2) - (b-3)*x/3*(1/(a+3) - ...)))
        '// Mainly for calculating the complement of BETA(x,a,b) for small a and b*x < 1.
        '// a should be close to 0, x >= 0 & x <=1 & b*x < 1
        Dim term As Double, D As Double, sum As Double
        term = x
        D = 2.0#
        sum = term / (a + 1.0#)
        While (Math.Abs(term) > Math.Abs(sum * sumAcc))
            term = -term * (B - D) * x / D
            sum = sum + term / (a + D)
            D = D + 1.0#
        End While
        compbfunc = a * (B - 1.0#) * sum
    End Function

    Private Function incbeta(ByVal x As Double, ByVal a As Double, ByVal B As Double, ByVal comp As Boolean) As Double
        '// Calculates BETA for small a (complementary BETA if comp).
        Dim r As Double
        If (x > 0.5) Then
            incbeta = incbeta(1.0# - x, B, a, Not comp)
        Else
            r = (a + B + 0.5) * log1(a / (1.0# + B)) + a * ((a - 0.5) / (1.0# + B) + Math.Log((1.0# + B) * x)) - lngammaexpansion(a)
            If (comp) Then
                r = -expm1(r + lfbaccdif(a, B))
                r = r + compbfunc(x, a, B) * (1.0# - r)
                r = r + (a / (a + B)) * (1.0# - r)
            Else
                r = Math.Exp(r + (logfbit(a + B) - logfbit(B))) * (1.0# - compbfunc(x, a, B)) * (B / (a + B))
            End If
            incbeta = r
        End If
    End Function

    Private Function BETA(ByVal x As Double, ByVal a As Double, ByVal B As Double) As Double
        'Assumes x >= 0 & a >= 0 & b >= 0. Only called with a = 0 or b = 0 by (comp)BETA_nc
        If (a = 0.0# And B = 0.0#) Then
            BETA = ErrorValue
        ElseIf (a = 0.0#) Then
            BETA = 1.0#
        ElseIf (B = 0.0#) Then
            BETA = 0.0#
        ElseIf (x <= 0.0#) Then
            BETA = 0.0#
        ElseIf (x >= 1.0#) Then
            BETA = 1.0#
        ElseIf (a < 1.0# And B < 1.0#) Then
            BETA = incbeta(x, a, B, False)
        ElseIf (a < 1.0# And (1.0# + B) * x <= 1.0#) Then
            BETA = incbeta(x, a, B, False)
        ElseIf (B < 1.0# And a <= (1.0# + a) * x) Then
            BETA = incbeta(1.0# - x, B, a, True)
        ElseIf (a < 1.0#) Then
            BETA = compbinomial(-a, B, x, 1.0# - x, 0.0#)
        ElseIf (B < 1.0#) Then
            BETA = binomial(-B, a, 1.0# - x, x, 0.0#)
        Else
            BETA = compbinomial(a - 1.0#, B, x, 1.0# - x, (a + B - 1.0#) * x - a + 1.0#)
        End If
    End Function

    Private Function compbeta(ByVal x As Double, ByVal a As Double, ByVal B As Double) As Double
        'Assumes x >= 0 & a >= 0 & b >= 0. Only called with a = 0 or b = 0 by (comp)BETA_nc
        If (a = 0.0# And B = 0.0#) Then
            compbeta = ErrorValue
        ElseIf (a = 0.0#) Then
            compbeta = 0.0#
        ElseIf (B = 0.0#) Then
            compbeta = 1.0#
        ElseIf (x <= 0.0#) Then
            compbeta = 1.0#
        ElseIf (x >= 1.0#) Then
            compbeta = 0.0#
        ElseIf (a < 1.0# And B < 1.0#) Then
            compbeta = incbeta(x, a, B, True)
        ElseIf (a < 1.0# And (1.0# + B) * x <= 1.0#) Then
            compbeta = incbeta(x, a, B, True)
        ElseIf (B < 1.0# And a <= (1.0# + a) * x) Then
            compbeta = incbeta(1.0# - x, B, a, False)
        ElseIf (a < 1.0#) Then
            compbeta = binomial(-a, B, x, 1.0# - x, 0.0#)
        ElseIf (B < 1.0#) Then
            compbeta = compbinomial(-B, a, 1.0# - x, x, 0.0#)
        Else
            compbeta = binomial(a - 1.0#, B, x, 1.0# - x, (a + B - 1.0#) * x - a + 1.0#)
        End If
    End Function

    Private Function invincbeta(ByVal a As Double, ByVal B As Double, ByVal prob As Double, ByVal comp As Boolean, ByRef oneMinusP As Double) As Double
        '// Calculates inverse of BETA for small a (inverse of complementary BETA if comp).
        Dim r As Double, rb As Double, x As Double, OneOverDeriv As Double, dif As Double, pr As Double, mnab As Double, aplusbOvermxab As Double, lpr As Double, small As Double, smalllpr As Double
        If (Not comp And prob > B / (a + B)) Then
            invincbeta = invincbeta(a, B, 1.0# - prob, Not comp, oneMinusP)
            Exit Function
        ElseIf (comp And prob > a / (a + B) And prob > 0.1) Then
            invincbeta = invincbeta(a, B, 1.0# - prob, Not comp, oneMinusP)
            Exit Function
        End If
        lpr = max(-Math.Log(prob), 1.0#)
        small = 0.00000000000001
        smalllpr = small * lpr * prob
        If a >= B Then
            mnab = B
            aplusbOvermxab = (a + B) / a
        Else
            mnab = a
            aplusbOvermxab = (a + B) / B
        End If
        If (comp) Then
            r = (a + B + 0.5) * log1(a / (1.0# + B)) + a * (a - 0.5) / (1.0# + B) + lfbaccdif(a, B) - lngammaexpansion(a)
            r = -expm1(r)
            r = r + (a / (a + B)) * (1.0# - r)
            If (B < 1.0#) Then
                rb = (a + B + 0.5) * log1(B / (1.0# + a)) + B * (B - 0.5) / (1.0# + a) + (logfbit(a + B) - logfbit(a)) - lngammaexpansion(B)
                rb = Math.Exp(rb) * (a / (a + B))
                oneMinusP = Math.Log(prob / rb) / B
                If (oneMinusP < 0.0#) Then
                    oneMinusP = Math.Exp(oneMinusP) / (1.0# + a)
                Else
                    oneMinusP = 0.5
                End If
                If (oneMinusP = 0.0#) Then
                    invincbeta = 1.0#
                    Exit Function
                ElseIf (oneMinusP > 0.5) Then
                    oneMinusP = 0.5
                End If
                x = 1.0# - oneMinusP
                pr = rb * (1.0# - compbfunc(oneMinusP, B, a)) * Math.Exp(B * Math.Log((1 + a) * oneMinusP))
                OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0#) * mnab)
                dif = OneOverDeriv * pr * logdif(pr, prob)
                oneMinusP = oneMinusP - dif
                x = 1.0# - oneMinusP
                If (oneMinusP <= 0.0#) Then
                    oneMinusP = 0.0#
                    invincbeta = 1.0#
                    Exit Function
                ElseIf (x < 0.25) Then
                    x = Math.Exp(log0((r - prob) / (1.0# - r)) / a) / (B + 1.0#)
                    oneMinusP = 1.0# - x
                    If (x = 0.0#) Then
                        invincbeta = 0.0#
                        Exit Function
                    End If
                    pr = compbfunc(x, a, B) * (1.0# - prob)
                    OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0#) * mnab)
                    dif = OneOverDeriv * (prob + pr) * log0(pr / prob)
                    x = x + dif
                    If (x <= 0.0#) Then
                        oneMinusP = 1.0#
                        invincbeta = 0.0#
                        Exit Function
                    End If
                    oneMinusP = 1.0# - x
                End If
            Else
                pr = Math.Exp(log0((r - prob) / (1.0# - r)) / a) / (B + 1.0#)
                x = Math.Log(B * prob / (a * (1.0# - r) * B * Math.Exp(a * Math.Log(1 + B)))) / B
                If (Math.Abs(x) < 0.5) Then
                    x = -expm1(x)
                    oneMinusP = 1.0# - x
                Else
                    oneMinusP = Math.Exp(x)
                    x = 1.0# - oneMinusP
                    If (oneMinusP = 0.0#) Then
                        invincbeta = x
                        Exit Function
                    End If
                End If
                If pr > x And pr < 1.0# Then
                    x = pr
                    oneMinusP = 1.0# - x
                End If
            End If
            dif = Min(x, oneMinusP)
            pr = -1.0#
            While ((Math.Abs(pr - prob) > smalllpr) And (Math.Abs(dif) > small * max(cSmall, Min(x, oneMinusP))))
                If (B < 1.0# And x > 0.5) Then
                    pr = rb * (1.0# - compbfunc(oneMinusP, B, a)) * Math.Exp(B * Math.Log((1.0# + a) * oneMinusP))
                ElseIf ((1.0# + B) * x > 1.0#) Then
                    pr = binomial(-a, B, x, oneMinusP, 0.0#)
                Else
                    pr = r + compbfunc(x, a, B) * (1.0# - r)
                    pr = pr - expm1(a * Math.Log((1.0# + B) * x)) * (1.0# - pr)
                End If
                OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0#) * mnab)
                dif = OneOverDeriv * pr * logdif(pr, prob)
                If (x > 0.5) Then
                    oneMinusP = oneMinusP - dif
                    x = 1.0# - oneMinusP
                    If (oneMinusP <= 0.0#) Then
                        oneMinusP = 0.0#
                        invincbeta = 1.0#
                        Exit Function
                    End If
                Else
                    x = x + dif
                    oneMinusP = 1.0# - x
                    If (x <= 0.0#) Then
                        oneMinusP = 1.0#
                        invincbeta = 0.0#
                        Exit Function
                    End If
                End If
            End While
        Else
            r = (a + B + 0.5) * log1(a / (1.0# + B)) + a * (a - 0.5) / (1.0# + B) + (logfbit(a + B) - logfbit(B)) - lngammaexpansion(a)
            r = Math.Exp(r) * (B / (a + B))
            x = logdif(prob, r)
            If (x < -711.0# * a) Then
                x = 0.0#
            Else
                x = Math.Exp(x / a) / (1.0# + B)
            End If
            If (x = 0.0#) Then
                oneMinusP = 1.0#
                invincbeta = x
                Exit Function
            ElseIf (x >= 0.5) Then
                x = 0.5
            End If
            oneMinusP = 1.0# - x
            pr = r * (1.0# - compbfunc(x, a, B)) * Math.Exp(a * Math.Log((1.0# + B) * x))
            OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0#) * mnab)
            dif = OneOverDeriv * pr * logdif(pr, prob)
            x = x - dif
            oneMinusP = oneMinusP + dif
            While ((Math.Abs(pr - prob) > smalllpr) And (Math.Abs(dif) > small * max(cSmall, Min(x, oneMinusP))))
                If ((1.0# + B) * x > 1.0#) Then
                    pr = compbinomial(-a, B, x, oneMinusP, 0.0#)
                ElseIf (x > 0.5) Then
                    pr = incbeta(oneMinusP, B, a, Not comp)
                Else
                    pr = r * (1.0# - compbfunc(x, a, B)) * Math.Exp(a * Math.Log((1.0# + B) * x))
                End If
                OneOverDeriv = (aplusbOvermxab * x * oneMinusP) / (binomialTerm(a, B, x, oneMinusP, (a + B) * x - a, 0.0#) * mnab)
                dif = OneOverDeriv * pr * logdif(pr, prob)
                If x < 0.5 Then
                    x = x - dif
                    oneMinusP = 1.0# - x
                Else
                    oneMinusP = oneMinusP + dif
                    x = 1.0# - oneMinusP
                End If
            End While
        End If
        invincbeta = x
    End Function

    Private Function invbeta(ByVal a As Double, ByVal B As Double, ByVal prob As Double, ByRef oneMinusP As Double) As Double
        Dim swap As Double
        If (prob = 0.0#) Then
            oneMinusP = 1.0#
            invbeta = 0.0#
        ElseIf (prob = 1.0#) Then
            oneMinusP = 0.0#
            invbeta = 1.0#
        ElseIf (a = B And prob = 0.5) Then
            oneMinusP = 0.5
            invbeta = 0.5
        ElseIf (a < B And B < 1.0#) Then
            invbeta = invincbeta(a, B, prob, False, oneMinusP)
        ElseIf (B < a And a < 1.0#) Then
            swap = invincbeta(B, a, prob, True, oneMinusP)
            invbeta = oneMinusP
            oneMinusP = swap
        ElseIf (a < 1.0#) Then
            invbeta = invincbeta(a, B, prob, False, oneMinusP)
        ElseIf (B < 1.0#) Then
            swap = invincbeta(B, a, prob, True, oneMinusP)
            invbeta = oneMinusP
            oneMinusP = swap
        Else
            invbeta = invcompbinom(a - 1.0#, B, prob, oneMinusP)
        End If
    End Function

    Private Function invcompbeta(ByVal a As Double, ByVal B As Double, ByVal prob As Double, ByRef oneMinusP As Double) As Double
        Dim swap As Double
        If (prob = 0.0#) Then
            oneMinusP = 0.0#
            invcompbeta = 1.0#
        ElseIf (prob = 1.0#) Then
            oneMinusP = 1.0#
            invcompbeta = 0.0#
        ElseIf (a = B And prob = 0.5) Then
            oneMinusP = 0.5
            invcompbeta = 0.5
        ElseIf (a < B And B < 1.0#) Then
            invcompbeta = invincbeta(a, B, prob, True, oneMinusP)
        ElseIf (B < a And a < 1.0#) Then
            swap = invincbeta(B, a, prob, False, oneMinusP)
            invcompbeta = oneMinusP
            oneMinusP = swap
        ElseIf (a < 1.0#) Then
            invcompbeta = invincbeta(a, B, prob, True, oneMinusP)
        ElseIf (B < 1.0#) Then
            swap = invincbeta(B, a, prob, False, oneMinusP)
            invcompbeta = oneMinusP
            oneMinusP = swap
        Else
            invcompbeta = invbinom(a - 1.0#, B, prob, oneMinusP)
        End If
    End Function

    Private Function critpoiss(ByVal Mean As Double, ByVal cprob As Double) As Double
        '//i such that Pr(poisson(mean,i)) >= cprob and  Pr(poisson(mean,i-1)) < cprob
        If (cprob > 0.5) Then
            critpoiss = critcomppoiss(Mean, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double, dfm As Double
        Dim i As Double
        dfm = invcnormal(cprob) * Math.Sqrt(Mean)
        i = Int(Mean + dfm + 0.5)
        While (True)
            i = Int(i)
            If (i < 0.0#) Then
                i = 0.0#
            End If
            If (i >= max_crit) Then
                critpoiss = i
                Exit Function
            End If
            dfm = Mean - i
            pr = cpoisson(i, Mean, dfm)
            tpr = 0
            If (pr >= cprob) Then
                If (i = 0.0#) Then
                    critpoiss = i
                    Exit Function
                End If
                tpr = poissonTerm(i, Mean, dfm, 0.0#)
                pr = pr - tpr
                If (pr < cprob) Then
                    critpoiss = i
                    Exit Function
                End If

                i = i - 1.0#
                Dim temp As Double, temp2 As Double
                temp = (pr - cprob) / tpr
                If (temp > 10) Then
                    temp = Int(temp + 0.5)
                    i = i - temp
                    temp2 = poissonTerm(i, Mean, Mean - i, 0.0#)
                    i = i - temp * (tpr - temp2) / (2 * temp2)
                Else
                    tpr = tpr * (i + 1.0#) / Mean
                    pr = pr - tpr
                    If (pr < cprob) Then
                        critpoiss = i
                        Exit Function
                    End If
                    i = i - 1.0#
                    If (i = 0.0#) Then
                        critpoiss = i
                        Exit Function
                    End If
                    temp2 = (pr - cprob) / tpr
                    If (temp2 < temp - 0.9) Then
                        While (pr >= cprob)
                            tpr = tpr * (i + 1.0#) / Mean
                            pr = pr - tpr
                            i = i - 1.0#
                        End While
                        critpoiss = i + 1.0#
                        Exit Function
                    Else
                        temp = Int(Math.Log(cprob / pr) / Math.Log((i + 1.0#) / Mean) + 0.5)
                        i = i - temp
                        If (i < 0.0#) Then
                            i = 0.0#
                        End If
                        temp2 = poissonTerm(i, Mean, Mean - i, 0.0#)
                        If (temp2 > nearly_zero) Then
                            temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log((i + 1.0#) / Mean)
                            i = i - temp
                        End If
                    End If
                End If
            Else
                While ((tpr < cSmall) And (pr < cprob))
                    i = i + 1.0#
                    dfm = dfm - 1.0#
                    tpr = poissonTerm(i, Mean, dfm, 0.0#)
                    pr = pr + tpr
                End While
                While (pr < cprob)
                    i = i + 1.0#
                    tpr = tpr * Mean / i
                    pr = pr + tpr
                End While
                critpoiss = i
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critcomppoiss(ByVal Mean As Double, ByVal cprob As Double) As Double
        '//i such that 1-Pr(poisson(mean,i)) > cprob and  1-Pr(poisson(mean,i-1)) <= cprob
        If (cprob > 0.5) Then
            critcomppoiss = critpoiss(Mean, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double, dfm As Double
        Dim i As Double
        dfm = invcnormal(cprob) * Math.Sqrt(Mean)
        i = Int(Mean - dfm + 0.5)
        While (True)
            i = Int(i)
            If (i >= max_crit) Then
                critcomppoiss = i
                Exit Function
            End If
            dfm = Mean - i
            pr = comppoisson(i, Mean, dfm)
            tpr = 0
            If (pr > cprob) Then
                i = i + 1.0#
                dfm = dfm - 1.0#
                tpr = poissonTerm(i, Mean, dfm, 0.0#)
                If (pr < (1.00001) * tpr) Then
                    While (tpr > cprob)
                        i = i + 1.0#
                        tpr = tpr * Mean / i
                    End While
                Else
                    pr = pr - tpr
                    If (pr <= cprob) Then
                        critcomppoiss = i
                        Exit Function
                    End If
                    Dim temp As Double, temp2 As Double
                    temp = (pr - cprob) / tpr
                    If (temp > 10) Then
                        temp = Int(temp + 0.5)
                        i = i + temp
                        temp2 = poissonTerm(i, Mean, Mean - i, 0.0#)
                        i = i + temp * (tpr - temp2) / (2.0# * temp2)
                    ElseIf (pr / tpr > 0.00001) Then
                        i = i + 1.0#
                        tpr = tpr * Mean / i
                        pr = pr - tpr
                        If (pr <= cprob) Then
                            critcomppoiss = i
                            Exit Function
                        End If
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr > cprob)
                                i = i + 1.0#
                                tpr = tpr * Mean / i
                                pr = pr - tpr
                            End While
                            critcomppoiss = i
                            Exit Function
                        Else
                            temp = Math.Log(cprob / pr) / Math.Log(Mean / i)
                            temp = Int((Math.Log(cprob / pr) - temp * Math.Log(i / (temp + i))) / Math.Log(Mean / i) + 0.5)
                            i = i + temp
                            temp2 = poissonTerm(i, Mean, Mean - i, 0.0#)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(Mean / i)
                                i = i + temp
                            End If
                        End If
                    End If
                End If
            Else
                While ((tpr < cSmall) And (pr <= cprob))
                    tpr = poissonTerm(i, Mean, dfm, 0.0#)
                    pr = pr + tpr
                    i = i - 1.0#
                    dfm = dfm + 1.0#
                End While
                While (pr <= cprob)
                    tpr = tpr * (i + 1.0#) / Mean
                    pr = pr + tpr
                    i = i - 1.0#
                End While
                critcomppoiss = i + 1.0#
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critbinomial(ByVal N As Double, ByVal eprob As Double, ByVal cprob As Double) As Double
        '//i such that Pr(binomial(n,eprob,i)) >= cprob and  Pr(binomial(n,eprob,i-1)) < cprob
        If (cprob > 0.5) Then
            critbinomial = critcompbinomial(N, eprob, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double, dfm As Double
        Dim i As Double
        dfm = invcnormal(cprob) * Math.Sqrt(N * eprob * (1.0# - eprob))
        i = N * eprob + dfm
        While (True)
            i = Int(i)
            If (i < 0.0#) Then
                i = 0.0#
            ElseIf (i > N) Then
                i = N
            End If
            If (i >= max_crit) Then
                critbinomial = i
                Exit Function
            End If
            dfm = N * eprob - i
            pr = binomial(i, N - i, eprob, 1.0# - eprob, dfm)
            tpr = 0.0#
            If (pr >= cprob) Then
                If (i = 0.0#) Then
                    critbinomial = i
                    Exit Function
                End If
                tpr = binomialTerm(i, N - i, eprob, 1.0# - eprob, dfm, 0.0#)
                If (pr < (1.00001) * tpr) Then
                    tpr = tpr * ((i + 1.0#) * (1.0# - eprob)) / ((N - i) * eprob)
                    i = i - 1.0#
                    While (tpr >= cprob)
                        tpr = tpr * ((i + 1.0#) * (1.0# - eprob)) / ((N - i) * eprob)
                        i = i - 1
                    End While
                Else
                    pr = pr - tpr
                    If (pr < cprob) Then
                        critbinomial = i
                        Exit Function
                    End If
                    i = i - 1.0#
                    If (i = 0.0#) Then
                        critbinomial = i
                        Exit Function
                    End If
                    Dim temp As Double, temp2 As Double
                    temp = (pr - cprob) / tpr
                    If (temp > 10.0#) Then
                        temp = Int(temp + 0.5)
                        i = i - temp
                        temp2 = binomialTerm(i, N - i, eprob, 1.0# - eprob, N * eprob - i, 0.0#)
                        i = i - temp * (tpr - temp2) / (2.0# * temp2)
                    Else
                        tpr = tpr * ((i + 1.0#) * (1.0# - eprob)) / ((N - i) * eprob)
                        pr = pr - tpr
                        If (pr < cprob) Then
                            critbinomial = i
                            Exit Function
                        End If
                        i = i - 1.0#
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr >= cprob)
                                tpr = tpr * ((i + 1.0#) * (1.0# - eprob)) / ((N - i) * eprob)
                                pr = pr - tpr
                                i = i - 1.0#
                            End While
                            critbinomial = i + 1.0#
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log(((i + 1.0#) * (1.0# - eprob)) / ((N - i) * eprob)) + 0.5)
                            i = i - temp
                            If (i < 0.0#) Then
                                i = 0.0#
                            End If
                            temp2 = binomialTerm(i, N - i, eprob, 1.0# - eprob, N * eprob - i, 0.0#)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((i + 1.0#) * (1.0# - eprob)) / ((N - i) * eprob))
                                i = i - temp
                            End If
                        End If
                    End If
                End If
            Else
                While ((tpr < cSmall) And (pr < cprob))
                    i = i + 1.0#
                    dfm = dfm - 1.0#
                    tpr = binomialTerm(i, N - i, eprob, 1.0# - eprob, dfm, 0.0#)
                    pr = pr + tpr
                End While
                While (pr < cprob)
                    i = i + 1.0#
                    tpr = tpr * ((N - i + 1.0#) * eprob) / (i * (1.0# - eprob))
                    pr = pr + tpr
                End While
                critbinomial = i
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critcompbinomial(ByVal N As Double, ByVal eprob As Double, ByVal cprob As Double) As Double
        '//i such that 1-Pr(binomial(n,eprob,i)) > cprob and  1-Pr(binomial(n,eprob,i-1)) <= cprob
        If (cprob > 0.5) Then
            critcompbinomial = critbinomial(N, eprob, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double, dfm As Double
        Dim i As Double
        dfm = invcnormal(cprob) * Math.Sqrt(N * eprob * (1.0# - eprob))
        i = N * eprob - dfm
        While (True)
            i = Int(i)
            If (i < 0.0#) Then
                i = 0
            ElseIf (i > N) Then
                i = N
            End If
            If (i >= max_crit) Then
                critcompbinomial = i
                Exit Function
            End If
            dfm = N * eprob - i
            pr = compbinomial(i, N - i, eprob, 1.0# - eprob, dfm)
            tpr = 0.0#
            If (pr > cprob) Then
                i = i + 1.0#
                dfm = dfm - 1.0#
                tpr = binomialTerm(i, N - i, eprob, 1.0# - eprob, dfm, 0.0#)
                If (pr < (1.00001) * tpr) Then
                    While (tpr > cprob)
                        i = i + 1.0#
                        tpr = tpr * ((N - i + 1.0#) * eprob) / (i * (1.0# - eprob))
                    End While
                Else
                    pr = pr - tpr
                    If (pr <= cprob) Then
                        critcompbinomial = i
                        Exit Function
                    End If
                    Dim temp As Double, temp2 As Double
                    temp = (pr - cprob) / tpr
                    If (temp > 10.0#) Then
                        temp = Int(temp + 0.5)
                        i = i + temp
                        temp2 = binomialTerm(i, N - i, eprob, 1.0# - eprob, N * eprob - i, 0.0#)
                        i = i + temp * (tpr - temp2) / (2.0# * temp2)
                    Else
                        i = i + 1.0#
                        tpr = tpr * ((N - i + 1.0#) * eprob) / (i * (1.0# - eprob))
                        pr = pr - tpr
                        If (pr <= cprob) Then
                            critcompbinomial = i
                            Exit Function
                        End If
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr > cprob)
                                i = i + 1.0#
                                tpr = tpr * ((N - i + 1.0#) * eprob) / (i * (1.0# - eprob))
                                pr = pr - tpr
                            End While
                            critcompbinomial = i
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log(((N - i + 1.0#) * eprob) / (i * (1.0# - eprob))) + 0.5)
                            i = i + temp
                            If (i > N) Then
                                i = N
                            End If
                            temp2 = binomialTerm(i, N - i, eprob, 1.0# - eprob, N * eprob - i, 0.0#)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((N - i + 1.0#) * eprob) / (i * (1.0# - eprob)))
                                i = i + temp
                            End If
                        End If
                    End If
                End If
            Else
                While ((tpr < cSmall) And (pr <= cprob))
                    tpr = binomialTerm(i, N - i, eprob, 1.0# - eprob, dfm, 0.0#)
                    pr = pr + tpr
                    i = i - 1.0#
                    dfm = dfm + 1.0#
                End While
                While (pr <= cprob)
                    tpr = tpr * ((i + 1.0#) * (1.0# - eprob)) / ((N - i) * eprob)
                    pr = pr + tpr
                    i = i - 1.0#
                End While
                critcompbinomial = i + 1.0#
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function crithyperg(ByVal j As Double, ByVal k As Double, ByVal M As Double, ByVal cprob As Double) As Double
        '//i such that Pr(hypergeometric(i,j,k,m)) >= cprob and  Pr(hypergeometric(i-1,j,k,m)) < cprob
        Dim ha1 As Double, hprob As Double, hswap As Boolean
        If (cprob > 0.5) Then
            crithyperg = critcomphyperg(j, k, M, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double
        Dim i As Double
        i = j * k / M + invcnormal(cprob) * Math.Sqrt(j * k * (M - j) * (M - k) / (M * M * (M - 1.0#)))
        Dim MX As Double, mn As Double
        MX = Min(j, k)
        mn = max(0, j + k - M)
        While (True)
            If (i < mn) Then
                i = mn
            ElseIf (i > MX) Then
                i = MX
            End If
            i = Int(i + 0.5)
            If (i >= max_crit) Then
                crithyperg = i
                Exit Function
            End If
            pr = hypergeometric(i, j - i, k - i, M - k - j + i, False, ha1, hprob, hswap)
            tpr = 0
            If (pr >= cprob) Then
                If (i = mn) Then
                    crithyperg = mn
                    Exit Function
                End If
                tpr = hypergeometricTerm(i, j - i, k - i, M - k - j + i)
                If (pr < (1 + 0.00001) * tpr) Then
                    tpr = tpr * ((i + 1.0#) * (M - j - k + i + 1.0#)) / ((k - i) * (j - i))
                    i = i - 1.0#
                    While (tpr > cprob)
                        tpr = tpr * ((i + 1.0#) * (M - j - k + i + 1.0#)) / ((k - i) * (j - i))
                        i = i - 1.0#
                    End While
                Else
                    pr = pr - tpr
                    If (pr < cprob) Then
                        crithyperg = i
                        Exit Function
                    End If
                    i = i - 1.0#
                    If (i = mn) Then
                        crithyperg = mn
                        Exit Function
                    End If
                    Dim temp As Double, temp2 As Double
                    temp = (pr - cprob) / tpr
                    If (temp > 10) Then
                        temp = Int(temp + 0.5)
                        i = i - temp
                        temp2 = hypergeometricTerm(i, j - i, k - i, M - k - j + i)
                        i = i - temp * (tpr - temp2) / (2.0# * temp2)
                    Else
                        tpr = tpr * ((i + 1.0#) * (M - j - k + i + 1.0#)) / ((k - i) * (j - i))
                        pr = pr - tpr
                        If (pr < cprob) Then
                            crithyperg = i
                            Exit Function
                        End If
                        i = i - 1.0#
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr >= cprob)
                                tpr = tpr * ((i + 1.0#) * (M - j - k + i + 1.0#)) / ((k - i) * (j - i))
                                pr = pr - tpr
                                i = i - 1.0#
                            End While
                            crithyperg = i + 1.0#
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log(((i + 1.0#) * (M - j - k + i + 1.0#)) / ((k - i) * (j - i))) + 0.5)
                            i = i - temp
                            temp2 = hypergeometricTerm(i, j - i, k - i, M - k - j + i)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((i + 1.0#) * (M - j - k + i + 1.0#)) / ((k - i) * (j - i)))
                                i = i - temp
                            End If
                        End If
                    End If
                End If
            Else
                While ((tpr < cSmall) And (pr < cprob))
                    i = i + 1.0#
                    tpr = hypergeometricTerm(i, j - i, k - i, M - k - j + i)
                    pr = pr + tpr
                End While
                While (pr < cprob)
                    i = i + 1.0#
                    tpr = tpr * ((k - i + 1.0#) * (j - i + 1.0#)) / (i * (M - j - k + i))
                    pr = pr + tpr
                End While
                crithyperg = i
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critcomphyperg(ByVal j As Double, ByVal k As Double, ByVal M As Double, ByVal cprob As Double) As Double
        '//i such that 1-Pr(hypergeometric(i,j,k,m)) > cprob and  1-Pr(hypergeometric(i-1,j,k,m)) <= cprob
        Dim ha1 As Double, hprob As Double, hswap As Boolean
        If (cprob > 0.5) Then
            critcomphyperg = crithyperg(j, k, M, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double
        Dim i As Double
        i = j * k / M - invcnormal(cprob) * Math.Sqrt(j * k * (M - j) * (M - k) / (M * M * (M - 1.0#)))
        Dim MX As Double, mn As Double
        MX = Min(j, k)
        mn = max(0, j + k - M)
        While (True)
            If (i < mn) Then
                i = mn
            ElseIf (i > MX) Then
                i = MX
            End If
            i = Int(i + 0.5)
            If (i >= max_crit) Then
                critcomphyperg = i
                Exit Function
            End If
            pr = hypergeometric(i, j - i, k - i, M - k - j + i, True, ha1, hprob, hswap)
            tpr = 0.0#
            If (pr > cprob) Then
                i = i + 1.0#
                tpr = hypergeometricTerm(i, j - i, k - i, M - k - j + i)
                If (pr < (1.0# + 0.00001) * tpr) Then
                    While (tpr > cprob)
                        i = i + 1
                        tpr = tpr * ((k - i + 1.0#) * (j - i + 1.0#)) / (i * (M - j - k + i))
                    End While
                Else
                    pr = pr - tpr
                    If (pr <= cprob) Then
                        critcomphyperg = i
                        Exit Function
                    End If
                    Dim temp As Double, temp2 As Double
                    temp = (pr - cprob) / tpr
                    If (temp > 10) Then
                        temp = Int(temp + 0.5)
                        i = i + temp
                        temp2 = hypergeometricTerm(i, j - i, k - i, M - k - j + i)
                        i = i + temp * (tpr - temp2) / (2.0# * temp2)
                    Else
                        i = i + 1.0#
                        tpr = tpr * ((k - i + 1.0#) * (j - i + 1.0#)) / (i * (M - j - k + i))
                        pr = pr - tpr
                        If (pr <= cprob) Then
                            critcomphyperg = i
                            Exit Function
                        End If
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr > cprob)
                                i = i + 1.0#
                                tpr = tpr * ((k - i + 1.0#) * (j - i + 1.0#)) / (i * (M - j - k + i))
                                pr = pr - tpr
                            End While
                            critcomphyperg = i
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log(((k - i + 1.0#) * (j - i + 1.0#)) / (i * (M - j - k + i))) + 0.5)
                            i = i + temp
                            temp2 = hypergeometricTerm(i, j - i, k, M - k)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((k - i + 1.0#) * (j - i + 1.0#)) / (i * (M - j - k + i)))
                                i = i + temp
                            End If
                        End If
                    End If
                End If
            Else
                While ((tpr < cSmall) And (pr <= cprob))
                    tpr = hypergeometricTerm(i, j - i, k - i, M - k - j + i)
                    pr = pr + tpr
                    i = i - 1.0#
                End While
                While (pr <= cprob)
                    tpr = tpr * ((i + 1.0#) * (M - j - k + i + 1.0#)) / ((k - i) * (j - i))
                    pr = pr + tpr
                    i = i - 1.0#
                End While
                critcomphyperg = i + 1.0#
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critnegbinom(ByVal N As Double, ByVal eprob As Double, ByVal fprob As Double, ByVal cprob As Double) As Double
        '//i such that Pr(negbinomial(n,eprob,i)) >= cprob and  Pr(negbinomial(n,eprob,i-1)) < cprob
        If (cprob > 0.5) Then
            critnegbinom = critcompnegbinom(N, eprob, fprob, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double, dfm As Double
        Dim i As Double
        i = invgamma(N * fprob, cprob) / eprob
        While (True)
            If (i < 0.0#) Then
                i = 0.0#
            End If
            i = Int(i)
            If (i >= max_crit) Then
                critnegbinom = i
                Exit Function
            End If
            If eprob <= fprob Then
                pr = BETA(eprob, N, i + 1.0#)
            Else
                pr = compbeta(fprob, i + 1.0#, N)
            End If
            tpr = 0.0#
            If (pr >= cprob) Then
                If (i = 0.0#) Then
                    critnegbinom = i
                    Exit Function
                End If
                If eprob <= fprob Then
                    dfm = N - (N + i) * eprob
                Else
                    dfm = (N + i) * fprob - i
                End If
                tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0#)
                If (pr < (1 + 0.00001) * tpr) Then
                    tpr = tpr * (i + 1.0#) / ((N + i) * fprob)
                    i = i - 1.0#
                    While (tpr > cprob)
                        tpr = tpr * (i + 1.0#) / ((N + i) * fprob)
                        i = i - 1.0#
                    End While
                Else
                    pr = pr - tpr
                    If (pr < cprob) Then
                        critnegbinom = i
                        Exit Function
                    End If
                    i = i - 1.0#
                    If (i = 0.0#) Then
                        critnegbinom = i
                        Exit Function
                    End If
                    Dim temp As Double, temp2 As Double
                    temp = (pr - cprob) / tpr
                    If (temp > 10.0#) Then
                        temp = Int(temp + 0.5)
                        i = i - temp
                        If eprob <= fprob Then
                            dfm = N - (N + i) * eprob
                        Else
                            dfm = (N + i) * fprob - i
                        End If
                        temp2 = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0#)
                        i = i - temp * (tpr - temp2) / (2.0# * temp2)
                    Else
                        tpr = tpr * (i + 1.0#) / ((N + i) * fprob)
                        pr = pr - tpr
                        If (pr < cprob) Then
                            critnegbinom = i
                            Exit Function
                        End If
                        i = i - 1.0#
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr >= cprob)
                                tpr = tpr * (i + 1.0#) / ((N + i) * fprob)
                                pr = pr - tpr
                                i = i - 1.0#
                            End While
                            critnegbinom = i + 1.0#
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log((i + 1.0#) / ((N + i) * fprob)) + 0.5)
                            i = i - temp
                            If eprob <= fprob Then
                                dfm = N - (N + i) * eprob
                            Else
                                dfm = (N + i) * fprob - i
                            End If
                            temp2 = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0#)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log((i + 1.0#) / ((N + i) * fprob))
                                i = i - temp
                            End If
                        End If
                    End If
                End If
            Else
                While ((tpr < cSmall) And (pr < cprob))
                    i = i + 1.0#
                    If eprob <= fprob Then
                        dfm = N - (N + i) * eprob
                    Else
                        dfm = (N + i) * fprob - i
                    End If
                    tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0#)
                    pr = pr + tpr
                End While
                While (pr < cprob)
                    i = i + 1.0#
                    tpr = tpr * ((N + i - 1.0#) * fprob) / i
                    pr = pr + tpr
                End While
                critnegbinom = i
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critcompnegbinom(ByVal N As Double, ByVal eprob As Double, ByVal fprob As Double, ByVal cprob As Double) As Double
        '//i such that 1-Pr(negbinomial(n,eprob,i)) > cprob and  1-Pr(negbinomial(n,eprob,i-1)) <= cprob
        If (cprob > 0.5) Then
            critcompnegbinom = critnegbinom(N, eprob, fprob, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double, dfm As Double
        Dim i As Double
        i = invcompgamma(N * fprob, cprob) / eprob
        While (True)
            If (i < 0.0#) Then
                i = 0.0#
            End If
            i = Int(i)
            If (i >= max_crit) Then
                critcompnegbinom = i
                Exit Function
            End If
            If eprob <= fprob Then
                pr = compbeta(eprob, N, i + 1.0#)
            Else
                pr = BETA(fprob, i + 1.0#, N)
            End If
            If (pr > cprob) Then
                i = i + 1.0#
                If eprob <= fprob Then
                    dfm = N - (N + i) * eprob
                Else
                    dfm = (N + i) * fprob - i
                End If
                tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0#)
                If (pr < (1.00001) * tpr) Then
                    While (tpr > cprob)
                        i = i + 1.0#
                        tpr = tpr * ((N + i - 1.0#) * fprob) / i
                    End While
                Else
                    pr = pr - tpr
                    If (pr <= cprob) Then
                        critcompnegbinom = i
                        Exit Function
                    ElseIf (tpr < 0.000000000000001 * pr) Then
                        If (tpr < cSmall) Then
                            critcompnegbinom = i
                        Else
                            critcompnegbinom = i + Int((pr - cprob) / tpr)
                        End If
                        Exit Function
                    End If
                    Dim temp As Double, temp2 As Double
                    temp = (pr - cprob) / tpr
                    If (temp > 10.0#) Then
                        temp = Int(temp + 0.5)
                        i = i + temp
                        If eprob <= fprob Then
                            dfm = N - (N + i) * eprob
                        Else
                            dfm = (N + i) * fprob - i
                        End If
                        temp2 = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0#)
                        i = i + temp * (tpr - temp2) / (2.0# * temp2)
                    Else
                        i = i + 1.0#
                        tpr = tpr * ((N + i - 1.0#) * fprob) / i
                        pr = pr - tpr
                        If (pr <= cprob) Then
                            critcompnegbinom = i
                            Exit Function
                        End If
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr > cprob)
                                i = i + 1.0#
                                tpr = tpr * ((N + i - 1.0#) * fprob) / i
                                pr = pr - tpr
                            End While
                            critcompnegbinom = i
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log(((N + i - 1.0#) * fprob) / i) + 0.5)
                            i = i + temp
                            If eprob <= fprob Then
                                dfm = N - (N + i) * eprob
                            Else
                                dfm = (N + i) * fprob - i
                            End If
                            temp2 = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0#)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((N + i - 1.0#) * fprob) / i)
                                i = i + temp
                            End If
                        End If
                    End If
                End If
            Else
                If eprob <= fprob Then
                    dfm = N - (N + i) * eprob
                Else
                    dfm = (N + i) * fprob - i
                End If
                tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0#)
                If (tpr < 0.000000000000001 * pr) Then
                    If (tpr < cSmall) Then
                        critcompnegbinom = i
                    Else
                        critcompnegbinom = i - Int((cprob - pr) / tpr)
                    End If
                    Exit Function
                End If
                While ((tpr < cSmall) And (pr <= cprob))
                    pr = pr + tpr
                    i = i - 1.0#
                    If eprob <= fprob Then
                        dfm = N - (N + i) * eprob
                    Else
                        dfm = (N + i) * fprob - i
                    End If
                    tpr = N / (N + i) * binomialTerm(i, N, fprob, eprob, dfm, 0.0#)
                End While
                While (pr <= cprob)
                    pr = pr + tpr
                    i = i - 1.0#
                    If i < 0.0# Then
                        critcompnegbinom = 0.0#
                        Exit Function
                    End If
                    tpr = tpr * (i + 1.0#) / ((N + i) * fprob)
                End While
                critcompnegbinom = i + 1.0#
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critneghyperg(ByVal j As Double, ByVal k As Double, ByVal M As Double, ByVal cprob As Double) As Double
        '//i such that Pr(neghypergeometric(i,j,k,m)) >= cprob and  Pr(neghypergeometric(i-1,j,k,m)) < cprob
        Dim ha1 As Double, hprob As Double, hswap As Boolean
        If (cprob > 0.5) Then
            critneghyperg = critcompneghyperg(j, k, M, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double
        Dim i As Double, temp As Double, temp2 As Double, oneMinusP As Double
        pr = (M - k) / M
        i = invbeta(j * pr, pr * (k - j + 1.0#), cprob, oneMinusP) * (M - k)
        While (True)
            If (i < 0.0#) Then
                i = 0.0#
            ElseIf (i > M - k) Then
                i = M - k
            End If
            i = Int(i + 0.5)
            If (i >= max_crit) Then
                critneghyperg = i
                Exit Function
            End If
            pr = hypergeometric(i, j, M - k - i, k - j, False, ha1, hprob, hswap)
            tpr = 0.0#
            If (pr >= cprob) Then
                If (i = 0.0#) Then
                    critneghyperg = 0.0#
                    Exit Function
                End If
                tpr = hypergeometricTerm(j - 1.0#, i, k - j + 1.0#, M - k - i) * (k - j + 1.0#) / (M - j - i + 1.0#)
                If (pr < (1.0# + 0.00001) * tpr) Then
                    tpr = tpr * ((i + 1.0#) * (M - j - i)) / ((j + i) * (M - i - k))
                    i = i - 1.0#
                    While (tpr > cprob)
                        tpr = tpr * ((i + 1.0#) * (M - j - i)) / ((j + i) * (M - i - k))
                        i = i - 1.0#
                    End While
                Else
                    pr = pr - tpr
                    If (pr < cprob) Then
                        critneghyperg = i
                        Exit Function
                    End If
                    i = i - 1.0#

                    If (i = 0.0#) Then
                        critneghyperg = 0.0#
                        Exit Function
                    End If
                    temp = (pr - cprob) / tpr
                    If (temp > 10) Then
                        temp = Int(temp + 0.5)
                        i = i - temp
                        temp2 = hypergeometricTerm(j - 1.0#, i, k - j + 1.0#, M - k - i) * (k - j + 1.0#) / (M - j - i + 1.0#)
                        i = i - temp * (tpr - temp2) / (2 * temp2)
                    Else
                        tpr = tpr * ((i + 1.0#) * (M - j - i)) / ((j + i) * (M - i - k))
                        pr = pr - tpr
                        If (pr < cprob) Then
                            critneghyperg = i
                            Exit Function
                        End If
                        i = i - 1.0#
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr >= cprob)
                                tpr = tpr * ((i + 1.0#) * (M - j - i)) / ((j + i) * (M - i - k))
                                pr = pr - tpr
                                i = i - 1.0#
                            End While
                            critneghyperg = i + 1.0#
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log(((i + 1.0#) * (M - j - i)) / ((j + i) * (M - i - k))) + 0.5)
                            i = i - temp
                            temp2 = hypergeometricTerm(j - 1.0#, i, k - j + 1.0#, M - k - i) * (k - j + 1.0#) / (M - j - i + 1.0#)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((i + 1.0#) * (M - j - i)) / ((j + i) * (M - i - k)))
                                i = i - temp
                            End If
                        End If
                    End If
                End If
            Else
                While ((tpr < cSmall) And (pr < cprob))
                    i = i + 1.0#
                    tpr = hypergeometricTerm(j - 1.0#, i, k - j + 1.0#, M - k - i) * (k - j + 1.0#) / (M - j - i + 1.0#)
                    pr = pr + tpr
                End While
                While (pr < cprob)
                    i = i + 1.0#
                    tpr = tpr * ((j + i - 1.0#) * (M - i - k + 1.0#)) / (i * (M - j - i + 1.0#))
                    pr = pr + tpr
                End While
                critneghyperg = i
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critcompneghyperg(ByVal j As Double, ByVal k As Double, ByVal M As Double, ByVal cprob As Double) As Double
        '//i such that 1-Pr(neghypergeometric(i,j,k,m)) > cprob and  1-Pr(neghypergeometric(i-1,j,k,m)) <= cprob
        Dim ha1 As Double, hprob As Double, hswap As Boolean
        If (cprob > 0.5) Then
            critcompneghyperg = critneghyperg(j, k, M, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double
        Dim i As Double, temp As Double, temp2 As Double, oneMinusP As Double
        pr = (M - k) / M
        i = invcompbeta(j * pr, pr * (k - j + 1.0#), cprob, oneMinusP) * (M - k)
        While (True)
            If (i < 0.0#) Then
                i = 0.0#
            ElseIf (i > M - k) Then
                i = M - k
            End If
            i = Int(i + 0.5)
            If (i >= max_crit) Then
                critcompneghyperg = i
                Exit Function
            End If
            pr = hypergeometric(i, j, M - k - i, k - j, True, ha1, hprob, hswap)
            tpr = 0.0#
            If (pr > cprob) Then
                i = i + 1.0#
                tpr = hypergeometricTerm(j - 1.0#, i, k - j + 1.0#, M - k - i) * (k - j + 1.0#) / (M - j - i + 1.0#)
                If (pr < (1 + 0.00001) * tpr) Then
                    Do While (tpr > cprob)
                        i = i + 1.0#
                        temp = M - j - i + 1.0#
                        If temp = 0.0# Then Exit Do
                        tpr = tpr * ((j + i - 1.0#) * (M - i - k + 1.0#)) / (i * temp)
                    Loop
                Else
                    pr = pr - tpr
                    If (pr <= cprob) Then
                        critcompneghyperg = i
                        Exit Function
                    End If
                    temp = (pr - cprob) / tpr
                    If (temp > 10.0#) Then
                        temp = Int(temp + 0.5)
                        i = i + temp
                        temp2 = hypergeometricTerm(j - 1.0#, i, k - j + 1.0#, M - k - i) * (k - j + 1.0#) / (M - j - i + 1.0#)
                        i = i + temp * (tpr - temp2) / (2 * temp2)
                    Else
                        i = i + 1.0#
                        tpr = tpr * ((j + i - 1.0#) * (M - i - k + 1.0#)) / (i * (M - j - i + 1.0#))
                        pr = pr - tpr
                        If (pr <= cprob) Then
                            critcompneghyperg = i
                            Exit Function
                        End If
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr > cprob)
                                i = i + 1.0#
                                tpr = tpr * ((j + i - 1.0#) * (M - i - k + 1.0#)) / (i * (M - j - i + 1.0#))
                                pr = pr - tpr
                            End While
                            critcompneghyperg = i
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log(((j + i - 1.0#) * (M - i - k + 1.0#)) / (i * (M - j - i + 1.0#))) + 0.5)
                            i = i + temp
                            temp2 = hypergeometricTerm(j - 1.0#, i, k - j + 1.0#, M - k - i) * (k - j + 1.0#) / (M - j - i + 1.0#)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((j + i - 1.0#) * (M - i - k + 1.0#)) / (i * (M - j - i + 1.0#)))
                                i = i + temp
                            End If
                        End If
                    End If
                End If
            Else
                While ((tpr < cSmall) And (pr <= cprob))
                    tpr = hypergeometricTerm(j - 1.0#, i, k - j + 1.0#, M - k - i) * (k - j + 1.0#) / (M - j - i + 1.0#)
                    pr = pr + tpr
                    i = i - 1.0#
                End While
                While (pr <= cprob)
                    tpr = tpr * ((i + 1.0#) * (M - j - i)) / ((j + i) * (M - i - k))
                    pr = pr + tpr
                    i = i - 1.0#
                End While
                critcompneghyperg = i + 1.0#
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function AlterForIntegralChecks_Others(ByVal Value As Double) As Double
        If NonIntegralValuesAllowed_Others Then
            AlterForIntegralChecks_Others = Int(Value)
        ElseIf Value <> Int(Value) Then
            AlterForIntegralChecks_Others = ErrorValue
        Else
            AlterForIntegralChecks_Others = Value
        End If
    End Function

    Private Function AlterForIntegralChecks_df(ByVal Value As Double) As Double
        If NonIntegralValuesAllowed_df Then
            AlterForIntegralChecks_df = Value
        Else
            AlterForIntegralChecks_df = AlterForIntegralChecks_Others(Value)
        End If
    End Function

    Private Function AlterForIntegralChecks_NB(ByVal Value As Double) As Double
        If NonIntegralValuesAllowed_NB Then
            AlterForIntegralChecks_NB = Value
        Else
            AlterForIntegralChecks_NB = AlterForIntegralChecks_Others(Value)
        End If
    End Function

    Private Function GetRidOfMinusZeroes(ByVal x As Double) As Double
        If x = 0.0# Then
            GetRidOfMinusZeroes = 0.0#
        Else
            GetRidOfMinusZeroes = x
        End If
    End Function

    Public Function pmf_geometric(ByVal failures As Double, ByVal success_prob As Double) As Double
        failures = AlterForIntegralChecks_Others(failures)
        If (success_prob < 0.0# Or success_prob > 1.0#) Then
            pmf_geometric = ErrorValue
        ElseIf failures < 0.0# Then
            pmf_geometric = 0.0#
        ElseIf success_prob = 1.0# Then
            If failures = 0.0# Then
                pmf_geometric = 1.0#
            Else
                pmf_geometric = 0.0#
            End If
        Else
            pmf_geometric = success_prob * Math.Exp(failures * log0(-success_prob))
        End If
        pmf_geometric = GetRidOfMinusZeroes(pmf_geometric)
    End Function

    Public Function cdf_geometric(ByVal failures As Double, ByVal success_prob As Double) As Double
        failures = Int(failures)
        If (success_prob < 0.0# Or success_prob > 1.0#) Then
            cdf_geometric = ErrorValue
        ElseIf failures < 0.0# Then
            cdf_geometric = 0.0#
        ElseIf success_prob = 1.0# Then
            If failures >= 0.0# Then
                cdf_geometric = 1.0#
            Else
                cdf_geometric = 0.0#
            End If
        Else
            cdf_geometric = -expm1((failures + 1.0#) * log0(-success_prob))
        End If
        cdf_geometric = GetRidOfMinusZeroes(cdf_geometric)
    End Function

    Public Function comp_cdf_geometric(ByVal failures As Double, ByVal success_prob As Double) As Double
        failures = Int(failures)
        If (success_prob < 0.0# Or success_prob > 1.0#) Then
            comp_cdf_geometric = ErrorValue
        ElseIf failures < 0.0# Then
            comp_cdf_geometric = 1.0#
        ElseIf success_prob = 1.0# Then
            If failures >= 0.0# Then
                comp_cdf_geometric = 0.0#
            Else
                comp_cdf_geometric = 1.0#
            End If
        Else
            comp_cdf_geometric = Math.Exp((failures + 1.0#) * log0(-success_prob))
        End If
        comp_cdf_geometric = GetRidOfMinusZeroes(comp_cdf_geometric)
    End Function

    Public Function crit_geometric(ByVal success_prob As Double, ByVal crit_prob As Double) As Double
        If (success_prob <= 0.0# Or success_prob > 1.0# Or crit_prob < 0.0# Or crit_prob > 1.0#) Then
            crit_geometric = ErrorValue
        ElseIf (crit_prob = 0.0#) Then
            crit_geometric = ErrorValue
        ElseIf (success_prob = 1.0#) Then
            crit_geometric = 0.0#
        ElseIf (crit_prob = 1.0#) Then
            crit_geometric = ErrorValue
        Else
            crit_geometric = Int(log0(-crit_prob) / log0(-success_prob) - 1.0#)
            If -expm1((crit_geometric + 1.0#) * log0(-success_prob)) < crit_prob Then
                crit_geometric = crit_geometric + 1.0#
            End If
        End If
        crit_geometric = GetRidOfMinusZeroes(crit_geometric)
    End Function

    Public Function comp_crit_geometric(ByVal success_prob As Double, ByVal crit_prob As Double) As Double
        If (success_prob <= 0.0# Or success_prob > 1.0# Or crit_prob < 0.0# Or crit_prob > 1.0#) Then
            comp_crit_geometric = ErrorValue
        ElseIf (crit_prob = 1.0#) Then
            comp_crit_geometric = ErrorValue
        ElseIf (success_prob = 1.0#) Then
            comp_crit_geometric = 0.0#
        ElseIf (crit_prob = 0.0#) Then
            comp_crit_geometric = ErrorValue
        Else
            comp_crit_geometric = Int(Math.Log(crit_prob) / log0(-success_prob) - 1.0#)
            If Math.Exp((comp_crit_geometric + 1.0#) * log0(-success_prob)) > crit_prob Then
                comp_crit_geometric = comp_crit_geometric + 1.0#
            End If
        End If
        comp_crit_geometric = GetRidOfMinusZeroes(comp_crit_geometric)
    End Function

    Public Function lcb_geometric(ByVal failures As Double, ByVal prob As Double) As Double
        failures = AlterForIntegralChecks_Others(failures)
        If (prob < 0.0# Or prob > 1.0# Or failures < 0.0#) Then
            lcb_geometric = ErrorValue
        ElseIf (prob = 1.0#) Then
            lcb_geometric = 1.0#
        Else
            lcb_geometric = -expm1(log0(-prob) / (failures + 1.0#))
        End If
        lcb_geometric = GetRidOfMinusZeroes(lcb_geometric)
    End Function

    Public Function ucb_geometric(ByVal failures As Double, ByVal prob As Double) As Double
        failures = AlterForIntegralChecks_Others(failures)
        If (prob < 0.0# Or prob > 1.0# Or failures < 0.0#) Then
            ucb_geometric = ErrorValue
        ElseIf (prob = 0.0# Or failures = 0.0#) Then
            ucb_geometric = 1.0#
        ElseIf (prob = 1.0#) Then
            ucb_geometric = 0.0#
        Else
            ucb_geometric = -expm1(Math.Log(prob) / failures)
        End If
        ucb_geometric = GetRidOfMinusZeroes(ucb_geometric)
    End Function

    Public Function pmf_negbinomial(ByVal failures As Double, ByVal success_prob As Double, ByVal successes_reqd As Double) As Double
        Dim Q As Double, dfm As Double
        failures = AlterForIntegralChecks_Others(failures)
        successes_reqd = AlterForIntegralChecks_NB(successes_reqd)
        If (success_prob < 0.0# Or success_prob > 1.0# Or successes_reqd <= 0.0#) Then
            pmf_negbinomial = ErrorValue
        ElseIf (successes_reqd + failures > 0.0#) Then
            Q = 1.0# - success_prob
            If success_prob <= Q Then
                dfm = successes_reqd - (successes_reqd + failures) * success_prob
            Else
                dfm = (successes_reqd + failures) * Q - failures
            End If
            pmf_negbinomial = successes_reqd / (successes_reqd + failures) * binomialTerm(failures, successes_reqd, Q, success_prob, dfm, 0.0#)
        ElseIf (failures <> 0.0#) Then
            pmf_negbinomial = 0.0#
        Else
            pmf_negbinomial = 1.0#
        End If
        pmf_negbinomial = GetRidOfMinusZeroes(pmf_negbinomial)
    End Function

    Public Function cdf_negbinomial(ByVal failures As Double, ByVal success_prob As Double, ByVal successes_reqd As Double) As Double
        Dim Q As Double
        failures = Int(failures)
        successes_reqd = AlterForIntegralChecks_NB(successes_reqd)
        If (success_prob < 0.0# Or success_prob > 1.0# Or successes_reqd <= 0.0#) Then
            cdf_negbinomial = ErrorValue
        Else
            Q = 1.0# - success_prob
            If Q < success_prob Then
                cdf_negbinomial = compbeta(Q, failures + 1, successes_reqd)
            Else
                cdf_negbinomial = BETA(success_prob, successes_reqd, failures + 1)
            End If
        End If
        cdf_negbinomial = GetRidOfMinusZeroes(cdf_negbinomial)
    End Function

    Public Function comp_cdf_negbinomial(ByVal failures As Double, ByVal success_prob As Double, ByVal successes_reqd As Double) As Double
        Dim Q As Double
        failures = Int(failures)
        successes_reqd = AlterForIntegralChecks_NB(successes_reqd)
        If (success_prob < 0.0# Or success_prob > 1.0# Or successes_reqd <= 0.0#) Then
            comp_cdf_negbinomial = ErrorValue
        Else
            Q = 1.0# - success_prob
            If Q < success_prob Then
                comp_cdf_negbinomial = BETA(Q, failures + 1, successes_reqd)
            Else
                comp_cdf_negbinomial = compbeta(success_prob, successes_reqd, failures + 1)
            End If
        End If
        comp_cdf_negbinomial = GetRidOfMinusZeroes(comp_cdf_negbinomial)
    End Function

    Public Function crit_negbinomial(ByVal success_prob As Double, ByVal successes_reqd As Double, ByVal crit_prob As Double) As Double
        successes_reqd = AlterForIntegralChecks_NB(successes_reqd)
        If (success_prob <= 0.0# Or success_prob > 1.0# Or successes_reqd <= 0.0# Or crit_prob < 0.0# Or crit_prob > 1.0#) Then
            crit_negbinomial = ErrorValue
        ElseIf (crit_prob = 0.0#) Then
            crit_negbinomial = ErrorValue
        ElseIf (success_prob = 1.0#) Then
            crit_negbinomial = 0.0#
        ElseIf (crit_prob = 1.0#) Then
            crit_negbinomial = ErrorValue
        Else
            Dim i As Double, pr As Double
            crit_negbinomial = critnegbinom(successes_reqd, success_prob, 1.0# - success_prob, crit_prob)
            i = crit_negbinomial
            pr = cdf_negbinomial(i, success_prob, successes_reqd)
            If (pr = crit_prob) Then
            ElseIf (pr > crit_prob) Then
                i = i - 1.0#
                pr = cdf_negbinomial(i, success_prob, successes_reqd)
                If (pr >= crit_prob) Then
                    crit_negbinomial = i
                End If
            Else
                crit_negbinomial = i + 1.0#
            End If
        End If
        crit_negbinomial = GetRidOfMinusZeroes(crit_negbinomial)
    End Function

    Public Function comp_crit_negbinomial(ByVal success_prob As Double, ByVal successes_reqd As Double, ByVal crit_prob As Double) As Double
        successes_reqd = AlterForIntegralChecks_NB(successes_reqd)
        If (success_prob <= 0.0# Or success_prob > 1.0# Or successes_reqd <= 0.0# Or crit_prob < 0.0# Or crit_prob > 1.0#) Then
            comp_crit_negbinomial = ErrorValue
        ElseIf (crit_prob = 1.0#) Then
            comp_crit_negbinomial = ErrorValue
        ElseIf (success_prob = 1.0#) Then
            comp_crit_negbinomial = 0.0#
        ElseIf (crit_prob = 0.0#) Then
            comp_crit_negbinomial = ErrorValue
        Else
            Dim i As Double, pr As Double
            comp_crit_negbinomial = critcompnegbinom(successes_reqd, success_prob, 1.0# - success_prob, crit_prob)
            i = comp_crit_negbinomial
            pr = comp_cdf_negbinomial(i, success_prob, successes_reqd)
            If (pr = crit_prob) Then
            ElseIf (pr < crit_prob) Then
                i = i - 1.0#
                pr = comp_cdf_negbinomial(i, success_prob, successes_reqd)
                If (pr <= crit_prob) Then
                    comp_crit_negbinomial = i
                End If
            Else
                comp_crit_negbinomial = i + 1.0#
            End If
        End If
        comp_crit_negbinomial = GetRidOfMinusZeroes(comp_crit_negbinomial)
    End Function

    Public Function lcb_negbinomial(ByVal failures As Double, ByVal successes_reqd As Double, ByVal prob As Double) As Double
        failures = AlterForIntegralChecks_Others(failures)
        successes_reqd = AlterForIntegralChecks_NB(successes_reqd)
        If (prob < 0.0# Or prob > 1.0# Or failures < 0.0# Or successes_reqd <= 0.0#) Then
            lcb_negbinomial = ErrorValue
        ElseIf (prob = 0.0#) Then
            lcb_negbinomial = 0.0#
        ElseIf (prob = 1.0#) Then
            lcb_negbinomial = 1.0#
        Else
            Dim oneMinusP As Double
            lcb_negbinomial = invbeta(successes_reqd, failures + 1, prob, oneMinusP)
        End If
        lcb_negbinomial = GetRidOfMinusZeroes(lcb_negbinomial)
    End Function

    Public Function ucb_negbinomial(ByVal failures As Double, ByVal successes_reqd As Double, ByVal prob As Double) As Double
        failures = AlterForIntegralChecks_Others(failures)
        successes_reqd = AlterForIntegralChecks_NB(successes_reqd)
        If (prob < 0.0# Or prob > 1.0# Or failures < 0.0# Or successes_reqd <= 0.0#) Then
            ucb_negbinomial = ErrorValue
        ElseIf (prob = 0.0# Or failures = 0.0#) Then
            ucb_negbinomial = 1.0#
        ElseIf (prob = 1.0#) Then
            ucb_negbinomial = 0.0#
        Else
            Dim oneMinusP As Double
            ucb_negbinomial = invcompbeta(successes_reqd, failures, prob, oneMinusP)
        End If
        ucb_negbinomial = GetRidOfMinusZeroes(ucb_negbinomial)
    End Function

    Public Function pmf_binomial(ByVal SAMPLE_SIZE As Double, ByVal successes As Double, ByVal success_prob As Double) As Double
        Dim Q As Double, dfm As Double
        successes = AlterForIntegralChecks_Others(successes)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (success_prob < 0.0# Or success_prob > 1.0# Or SAMPLE_SIZE < 0.0#) Then
            pmf_binomial = ErrorValue
        Else
            Q = 1.0# - success_prob
            If success_prob <= Q Then
                dfm = SAMPLE_SIZE * success_prob - successes
            Else
                dfm = (SAMPLE_SIZE - successes) - SAMPLE_SIZE * Q
            End If
            pmf_binomial = binomialTerm(successes, SAMPLE_SIZE - successes, success_prob, Q, dfm, 0.0#)
        End If
        pmf_binomial = GetRidOfMinusZeroes(pmf_binomial)
    End Function

    Public Function cdf_binomial(ByVal SAMPLE_SIZE As Double, ByVal successes As Double, ByVal success_prob As Double) As Double
        Dim Q As Double, dfm As Double
        successes = Int(successes)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (success_prob < 0.0# Or success_prob > 1.0# Or SAMPLE_SIZE < 0.0#) Then
            cdf_binomial = ErrorValue
        Else
            Q = 1.0# - success_prob
            If success_prob <= Q Then
                dfm = SAMPLE_SIZE * success_prob - successes
            Else
                dfm = (SAMPLE_SIZE - successes) - SAMPLE_SIZE * Q
            End If
            cdf_binomial = binomial(successes, SAMPLE_SIZE - successes, success_prob, Q, dfm)
        End If
        cdf_binomial = GetRidOfMinusZeroes(cdf_binomial)
    End Function

    Public Function comp_cdf_binomial(ByVal SAMPLE_SIZE As Double, ByVal successes As Double, ByVal success_prob As Double) As Double
        Dim Q As Double, dfm As Double
        successes = Int(successes)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (success_prob < 0.0# Or success_prob > 1.0# Or SAMPLE_SIZE < 0.0#) Then
            comp_cdf_binomial = ErrorValue
        Else
            Q = 1.0# - success_prob
            If success_prob <= Q Then
                dfm = SAMPLE_SIZE * success_prob - successes
            Else
                dfm = (SAMPLE_SIZE - successes) - SAMPLE_SIZE * Q
            End If
            comp_cdf_binomial = compbinomial(successes, SAMPLE_SIZE - successes, success_prob, Q, dfm)
        End If
        comp_cdf_binomial = GetRidOfMinusZeroes(comp_cdf_binomial)
    End Function

    Public Function crit_binomial(ByVal SAMPLE_SIZE As Double, ByVal success_prob As Double, ByVal crit_prob As Double) As Double
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (success_prob < 0.0# Or success_prob > 1.0# Or SAMPLE_SIZE < 0.0# Or crit_prob < 0.0# Or crit_prob > 1.0#) Then
            crit_binomial = ErrorValue
        ElseIf (crit_prob = 0.0#) Then
            crit_binomial = ErrorValue
        ElseIf (success_prob = 0.0#) Then
            crit_binomial = 0.0#
        ElseIf (crit_prob = 1.0# Or success_prob = 1.0#) Then
            crit_binomial = SAMPLE_SIZE
        Else
            Dim pr As Double, i As Double
            crit_binomial = critbinomial(SAMPLE_SIZE, success_prob, crit_prob)
            i = crit_binomial
            pr = cdf_binomial(SAMPLE_SIZE, i, success_prob)
            If (pr = crit_prob) Then
            ElseIf (pr > crit_prob) Then
                i = i - 1.0#
                pr = cdf_binomial(SAMPLE_SIZE, i, success_prob)
                If (pr >= crit_prob) Then
                    crit_binomial = i
                End If
            Else
                crit_binomial = i + 1.0#
            End If
        End If
        crit_binomial = GetRidOfMinusZeroes(crit_binomial)
    End Function

    Public Function comp_crit_binomial(ByVal SAMPLE_SIZE As Double, ByVal success_prob As Double, ByVal crit_prob As Double) As Double
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (success_prob < 0.0# Or success_prob > 1.0# Or SAMPLE_SIZE < 0.0# Or crit_prob < 0.0# Or crit_prob > 1.0#) Then
            comp_crit_binomial = ErrorValue
        ElseIf (crit_prob = 1.0#) Then
            comp_crit_binomial = ErrorValue
        ElseIf (crit_prob = 0.0# Or success_prob = 1.0#) Then
            comp_crit_binomial = SAMPLE_SIZE
        ElseIf (success_prob = 0.0#) Then
            comp_crit_binomial = 0.0#
        Else
            Dim pr As Double, i As Double
            comp_crit_binomial = critcompbinomial(SAMPLE_SIZE, success_prob, crit_prob)
            i = comp_crit_binomial
            pr = comp_cdf_binomial(SAMPLE_SIZE, i, success_prob)
            If (pr = crit_prob) Then
            ElseIf (pr < crit_prob) Then
                i = i - 1.0#
                pr = comp_cdf_binomial(SAMPLE_SIZE, i, success_prob)
                If (pr <= crit_prob) Then
                    comp_crit_binomial = i
                End If
            Else
                comp_crit_binomial = i + 1.0#
            End If
        End If
        comp_crit_binomial = GetRidOfMinusZeroes(comp_crit_binomial)
    End Function

    Public Function lcb_binomial(ByVal SAMPLE_SIZE As Double, ByVal successes As Double, ByVal prob As Double) As Double
        successes = AlterForIntegralChecks_Others(successes)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (prob < 0.0# Or prob > 1.0#) Then
            lcb_binomial = ErrorValue
        ElseIf (SAMPLE_SIZE < successes Or successes < 0.0#) Then
            lcb_binomial = ErrorValue
        ElseIf (prob = 0.0# Or successes = 0.0#) Then
            lcb_binomial = 0.0#
        ElseIf (prob = 1.0#) Then
            lcb_binomial = 1.0#
        Else
            Dim oneMinusP As Double
            lcb_binomial = invcompbinom(successes - 1.0#, SAMPLE_SIZE - successes + 1.0#, prob, oneMinusP)
        End If
        lcb_binomial = GetRidOfMinusZeroes(lcb_binomial)
    End Function

    Public Function ucb_binomial(ByVal SAMPLE_SIZE As Double, ByVal successes As Double, ByVal prob As Double) As Double
        successes = AlterForIntegralChecks_Others(successes)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (prob < 0.0# Or prob > 1.0#) Then
            ucb_binomial = ErrorValue
        ElseIf (SAMPLE_SIZE < successes Or successes < 0.0#) Then
            ucb_binomial = ErrorValue
        ElseIf (prob = 0.0# Or successes = SAMPLE_SIZE#) Then
            ucb_binomial = 1.0#
        ElseIf (prob = 1.0#) Then
            ucb_binomial = 0.0#
        Else
            Dim oneMinusP As Double
            ucb_binomial = invbinom(successes, SAMPLE_SIZE - successes, prob, oneMinusP)
        End If
        ucb_binomial = GetRidOfMinusZeroes(ucb_binomial)
    End Function

    Public Function pmf_poisson(ByVal Mean As Double, ByVal i As Double) As Double
        i = AlterForIntegralChecks_Others(i)
        If (Mean < 0.0#) Then
            pmf_poisson = ErrorValue
        ElseIf (i < 0.0#) Then
            pmf_poisson = 0.0#
        Else
            pmf_poisson = poissonTerm(i, Mean, Mean - i, 0.0#)
        End If
        pmf_poisson = GetRidOfMinusZeroes(pmf_poisson)
    End Function

    Public Function cdf_poisson(ByVal Mean As Double, ByVal i As Double) As Double
        i = Int(i)
        If (Mean < 0.0#) Then
            cdf_poisson = ErrorValue
        ElseIf (i < 0.0#) Then
            cdf_poisson = 0.0#
        Else
            cdf_poisson = cpoisson(i, Mean, Mean - i)
        End If
        cdf_poisson = GetRidOfMinusZeroes(cdf_poisson)
    End Function

    Public Function comp_cdf_poisson(ByVal Mean As Double, ByVal i As Double) As Double
        i = Int(i)
        If (Mean < 0.0#) Then
            comp_cdf_poisson = ErrorValue
        ElseIf (i < 0.0#) Then
            comp_cdf_poisson = 1.0#
        Else
            comp_cdf_poisson = comppoisson(i, Mean, Mean - i)
        End If
        comp_cdf_poisson = GetRidOfMinusZeroes(comp_cdf_poisson)
    End Function

    Public Function crit_poisson(ByVal Mean As Double, ByVal crit_prob As Double) As Double
        If (crit_prob < 0.0# Or crit_prob > 1.0# Or Mean < 0.0#) Then
            crit_poisson = ErrorValue
        ElseIf (crit_prob = 0.0#) Then
            crit_poisson = ErrorValue
        ElseIf (Mean = 0.0#) Then
            crit_poisson = 0.0#
        ElseIf (crit_prob = 1.0#) Then
            crit_poisson = ErrorValue
        Else
            Dim pr As Double
            crit_poisson = critpoiss(Mean, crit_prob)
            pr = cpoisson(crit_poisson, Mean, Mean - crit_poisson)
            If (pr = crit_prob) Then
            ElseIf (pr > crit_prob) Then
                crit_poisson = crit_poisson - 1.0#
                pr = cpoisson(crit_poisson, Mean, Mean - crit_poisson)
                If (pr < crit_prob) Then
                    crit_poisson = crit_poisson + 1.0#
                End If
            Else
                crit_poisson = crit_poisson + 1.0#
            End If
        End If
        crit_poisson = GetRidOfMinusZeroes(crit_poisson)
    End Function

    Public Function comp_crit_poisson(ByVal Mean As Double, ByVal crit_prob As Double) As Double
        If (crit_prob < 0.0# Or crit_prob > 1.0# Or Mean < 0.0#) Then
            comp_crit_poisson = ErrorValue
        ElseIf (crit_prob = 1.0#) Then
            comp_crit_poisson = ErrorValue
        ElseIf (Mean = 0.0#) Then
            comp_crit_poisson = 0.0#
        ElseIf (crit_prob = 0.0#) Then
            comp_crit_poisson = ErrorValue
        Else
            Dim pr As Double
            comp_crit_poisson = critcomppoiss(Mean, crit_prob)
            pr = comppoisson(comp_crit_poisson, Mean, Mean - comp_crit_poisson)
            If (pr = crit_prob) Then
            ElseIf (pr < crit_prob) Then
                comp_crit_poisson = comp_crit_poisson - 1.0#
                pr = comppoisson(comp_crit_poisson, Mean, Mean - comp_crit_poisson)
                If (pr > crit_prob) Then
                    comp_crit_poisson = comp_crit_poisson + 1.0#
                End If
            Else
                comp_crit_poisson = comp_crit_poisson + 1.0#
            End If
        End If
        comp_crit_poisson = GetRidOfMinusZeroes(comp_crit_poisson)
    End Function

    Public Function lcb_poisson(ByVal i As Double, ByVal prob As Double) As Double
        i = AlterForIntegralChecks_Others(i)
        If (prob < 0.0# Or prob > 1.0# Or i < 0.0#) Then
            lcb_poisson = ErrorValue
        ElseIf (prob = 0.0# Or i = 0.0#) Then
            lcb_poisson = 0.0#
        ElseIf (prob = 1.0#) Then
            lcb_poisson = ErrorValue
        Else
            lcb_poisson = invcomppoisson(i - 1.0#, prob)
        End If
        lcb_poisson = GetRidOfMinusZeroes(lcb_poisson)
    End Function

    Public Function ucb_poisson(ByVal i As Double, ByVal prob As Double) As Double
        i = AlterForIntegralChecks_Others(i)
        If (prob <= 0.0# Or prob > 1.0#) Then
            ucb_poisson = ErrorValue
        ElseIf (i < 0.0#) Then
            ucb_poisson = ErrorValue
        ElseIf (prob = 1.0#) Then
            ucb_poisson = 0.0#
        Else
            ucb_poisson = invpoisson(i, prob)
        End If
        ucb_poisson = GetRidOfMinusZeroes(ucb_poisson)
    End Function

    Public Function pmf_hypergeometric(ByVal type1s As Double, ByVal SAMPLE_SIZE As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double) As Double
        type1s = AlterForIntegralChecks_Others(type1s)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (SAMPLE_SIZE < 0.0# Or tot_type1 < 0.0# Or SAMPLE_SIZE > POP_SIZE Or tot_type1 > POP_SIZE) Then
            pmf_hypergeometric = ErrorValue
        Else
            pmf_hypergeometric = hypergeometricTerm(type1s, SAMPLE_SIZE - type1s, tot_type1 - type1s, POP_SIZE - tot_type1 - SAMPLE_SIZE + type1s)
        End If
        pmf_hypergeometric = GetRidOfMinusZeroes(pmf_hypergeometric)
    End Function

    Public Function cdf_hypergeometric(ByVal type1s As Double, ByVal SAMPLE_SIZE As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double) As Double
        type1s = Int(type1s)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (SAMPLE_SIZE < 0.0# Or tot_type1 < 0.0# Or SAMPLE_SIZE > POP_SIZE Or tot_type1 > POP_SIZE) Then
            cdf_hypergeometric = ErrorValue
        Else
            Dim ha1 As Double, hprob As Double, hswap As Boolean
            cdf_hypergeometric = hypergeometric(type1s, SAMPLE_SIZE - type1s, tot_type1 - type1s, POP_SIZE - tot_type1 - SAMPLE_SIZE + type1s, False, ha1, hprob, hswap)
        End If
        cdf_hypergeometric = GetRidOfMinusZeroes(cdf_hypergeometric)
    End Function

    Public Function comp_cdf_hypergeometric(ByVal type1s As Double, ByVal SAMPLE_SIZE As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double) As Double
        type1s = Int(type1s)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (SAMPLE_SIZE < 0.0# Or tot_type1 < 0.0# Or SAMPLE_SIZE > POP_SIZE Or tot_type1 > POP_SIZE) Then
            comp_cdf_hypergeometric = ErrorValue
        Else
            Dim ha1 As Double, hprob As Double, hswap As Boolean
            comp_cdf_hypergeometric = hypergeometric(type1s, SAMPLE_SIZE - type1s, tot_type1 - type1s, POP_SIZE - tot_type1 - SAMPLE_SIZE + type1s, True, ha1, hprob, hswap)
        End If
        comp_cdf_hypergeometric = GetRidOfMinusZeroes(comp_cdf_hypergeometric)
    End Function

    Public Function crit_hypergeometric(ByVal SAMPLE_SIZE As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double, ByVal crit_prob As Double) As Double
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (crit_prob < 0.0# Or crit_prob > 1.0#) Then
            crit_hypergeometric = ErrorValue
        ElseIf (SAMPLE_SIZE < 0.0# Or tot_type1 < 0.0# Or SAMPLE_SIZE > POP_SIZE Or tot_type1 > POP_SIZE) Then
            crit_hypergeometric = ErrorValue
        ElseIf (crit_prob = 0.0#) Then
            crit_hypergeometric = ErrorValue
        ElseIf (SAMPLE_SIZE = 0.0# Or tot_type1 = 0.0#) Then
            crit_hypergeometric = 0.0#
        ElseIf (SAMPLE_SIZE = POP_SIZE Or tot_type1 = POP_SIZE) Then
            crit_hypergeometric = Min(SAMPLE_SIZE, tot_type1)
        ElseIf (crit_prob = 1.0#) Then
            crit_hypergeometric = Min(SAMPLE_SIZE, tot_type1)
        Else
            Dim ha1 As Double, hprob As Double, hswap As Boolean
            Dim i As Double, pr As Double
            crit_hypergeometric = crithyperg(SAMPLE_SIZE, tot_type1, POP_SIZE, crit_prob)
            i = crit_hypergeometric
            pr = hypergeometric(i, SAMPLE_SIZE - i, tot_type1 - i, POP_SIZE - tot_type1 - SAMPLE_SIZE + i, False, ha1, hprob, hswap)
            If (pr = crit_prob) Then
            ElseIf (pr > crit_prob) Then
                i = i - 1.0#
                pr = hypergeometric(i, SAMPLE_SIZE - i, tot_type1 - i, POP_SIZE - tot_type1 - SAMPLE_SIZE + i, False, ha1, hprob, hswap)
                If (pr >= crit_prob) Then
                    crit_hypergeometric = i
                End If
            Else
                crit_hypergeometric = i + 1.0#
            End If
        End If
        crit_hypergeometric = GetRidOfMinusZeroes(crit_hypergeometric)
    End Function

    Public Function comp_crit_hypergeometric(ByVal SAMPLE_SIZE As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double, ByVal crit_prob As Double) As Double
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (crit_prob < 0.0# Or crit_prob > 1.0#) Then
            comp_crit_hypergeometric = ErrorValue
        ElseIf (SAMPLE_SIZE < 0.0# Or tot_type1 < 0.0# Or SAMPLE_SIZE > POP_SIZE Or tot_type1 > POP_SIZE) Then
            comp_crit_hypergeometric = ErrorValue
        ElseIf (crit_prob = 1.0#) Then
            comp_crit_hypergeometric = ErrorValue
        ElseIf (SAMPLE_SIZE = 0.0# Or tot_type1 = 0.0#) Then
            comp_crit_hypergeometric = 0.0#
        ElseIf (SAMPLE_SIZE = POP_SIZE Or tot_type1 = POP_SIZE) Then
            comp_crit_hypergeometric = Min(SAMPLE_SIZE, tot_type1)
        ElseIf (crit_prob = 0.0#) Then
            comp_crit_hypergeometric = Min(SAMPLE_SIZE, tot_type1)
        Else
            Dim ha1 As Double, hprob As Double, hswap As Boolean
            Dim i As Double, pr As Double
            comp_crit_hypergeometric = critcomphyperg(SAMPLE_SIZE, tot_type1, POP_SIZE, crit_prob)
            i = comp_crit_hypergeometric
            pr = hypergeometric(i, SAMPLE_SIZE - i, tot_type1 - i, POP_SIZE - tot_type1 - SAMPLE_SIZE + i, True, ha1, hprob, hswap)
            If (pr = crit_prob) Then
            ElseIf (pr < crit_prob) Then
                i = i - 1.0#
                pr = hypergeometric(i, SAMPLE_SIZE - i, tot_type1 - i, POP_SIZE - tot_type1 - SAMPLE_SIZE + i, True, ha1, hprob, hswap)
                If (pr <= crit_prob) Then
                    comp_crit_hypergeometric = i
                End If
            Else
                comp_crit_hypergeometric = i + 1.0#
            End If
        End If
        comp_crit_hypergeometric = GetRidOfMinusZeroes(comp_crit_hypergeometric)
    End Function

    Public Function lcb_hypergeometric(ByVal type1s As Double, ByVal SAMPLE_SIZE As Double, ByVal POP_SIZE As Double, ByVal prob As Double) As Double
        type1s = AlterForIntegralChecks_Others(type1s)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (prob < 0.0# Or prob > 1.0#) Then
            lcb_hypergeometric = ErrorValue
        ElseIf (type1s < 0.0# Or type1s > SAMPLE_SIZE Or SAMPLE_SIZE > POP_SIZE) Then
            lcb_hypergeometric = ErrorValue
        ElseIf (prob = 0.0# Or type1s = 0.0# Or POP_SIZE = SAMPLE_SIZE) Then
            lcb_hypergeometric = type1s
        ElseIf (prob = 1.0#) Then
            lcb_hypergeometric = POP_SIZE - (SAMPLE_SIZE - type1s)
        ElseIf (prob < 0.5) Then
            lcb_hypergeometric = critneghyperg(type1s, SAMPLE_SIZE, POP_SIZE, prob * (1.000000000001)) + type1s
        Else
            lcb_hypergeometric = critcompneghyperg(type1s, SAMPLE_SIZE, POP_SIZE, (1.0# - prob) * (1.0# - 0.000000000001)) + type1s
        End If
        lcb_hypergeometric = GetRidOfMinusZeroes(lcb_hypergeometric)
    End Function

    Public Function ucb_hypergeometric(ByVal type1s As Double, ByVal SAMPLE_SIZE As Double, ByVal POP_SIZE As Double, ByVal prob As Double) As Double
        type1s = AlterForIntegralChecks_Others(type1s)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (prob < 0.0# Or prob > 1.0#) Then
            ucb_hypergeometric = ErrorValue
        ElseIf (type1s < 0.0# Or type1s > SAMPLE_SIZE Or SAMPLE_SIZE > POP_SIZE) Then
            ucb_hypergeometric = ErrorValue
        ElseIf (prob = 0.0# Or type1s = SAMPLE_SIZE Or POP_SIZE = SAMPLE_SIZE) Then
            ucb_hypergeometric = POP_SIZE - (SAMPLE_SIZE - type1s)
        ElseIf (prob = 1.0#) Then
            ucb_hypergeometric = type1s
        ElseIf (prob < 0.5) Then
            ucb_hypergeometric = critcompneghyperg(type1s + 1.0#, SAMPLE_SIZE, POP_SIZE, prob * (1.0# - 0.000000000001)) + type1s
        Else
            ucb_hypergeometric = critneghyperg(type1s + 1.0#, SAMPLE_SIZE, POP_SIZE, (1.0# - prob) * (1.000000000001)) + type1s
        End If
        ucb_hypergeometric = GetRidOfMinusZeroes(ucb_hypergeometric)
    End Function

    Public Function pmf_neghypergeometric(ByVal type2s As Double, ByVal type1s_reqd As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double) As Double
        type2s = AlterForIntegralChecks_Others(type2s)
        type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (type1s_reqd <= 0.0# Or tot_type1 < type1s_reqd Or tot_type1 > POP_SIZE) Then
            pmf_neghypergeometric = ErrorValue
        ElseIf (type2s < 0.0# Or tot_type1 + type2s > POP_SIZE) Then
            If type2s = 0.0# Then
                pmf_neghypergeometric = 1.0#
            Else
                pmf_neghypergeometric = 0.0#
            End If
        Else
            pmf_neghypergeometric = hypergeometricTerm(type1s_reqd - 1.0#, type2s, tot_type1 - type1s_reqd + 1.0#, POP_SIZE - tot_type1 - type2s) * (tot_type1 - type1s_reqd + 1.0#) / (POP_SIZE - type1s_reqd - type2s + 1.0#)
        End If
        pmf_neghypergeometric = GetRidOfMinusZeroes(pmf_neghypergeometric)
    End Function

    Public Function cdf_neghypergeometric(ByVal type2s As Double, ByVal type1s_reqd As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double) As Double
        type2s = Int(type2s)
        type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (type1s_reqd <= 0.0# Or tot_type1 < type1s_reqd Or tot_type1 > POP_SIZE) Then
            cdf_neghypergeometric = ErrorValue
        ElseIf (tot_type1 + type2s > POP_SIZE) Then
            cdf_neghypergeometric = 1.0#
        Else
            Dim ha1 As Double, hprob As Double, hswap As Boolean
            cdf_neghypergeometric = hypergeometric(type2s, type1s_reqd, POP_SIZE - tot_type1 - type2s, tot_type1 - type1s_reqd, False, ha1, hprob, hswap)
        End If
        cdf_neghypergeometric = GetRidOfMinusZeroes(cdf_neghypergeometric)
    End Function

    Public Function comp_cdf_neghypergeometric(ByVal type2s As Double, ByVal type1s_reqd As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double) As Double
        type2s = Int(type2s)
        type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (type1s_reqd <= 0.0# Or tot_type1 < type1s_reqd Or tot_type1 > POP_SIZE) Then
            comp_cdf_neghypergeometric = ErrorValue
        ElseIf (tot_type1 + type2s > POP_SIZE) Then
            comp_cdf_neghypergeometric = 0.0#
        Else
            Dim ha1 As Double, hprob As Double, hswap As Boolean
            comp_cdf_neghypergeometric = hypergeometric(type2s, type1s_reqd, POP_SIZE - tot_type1 - type2s, tot_type1 - type1s_reqd, True, ha1, hprob, hswap)
        End If
        comp_cdf_neghypergeometric = GetRidOfMinusZeroes(comp_cdf_neghypergeometric)
    End Function

    Public Function crit_neghypergeometric(ByVal type1s_reqd As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double, ByVal crit_prob As Double) As Double
        type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (crit_prob < 0.0# Or crit_prob > 1.0#) Then
            crit_neghypergeometric = ErrorValue
        ElseIf (type1s_reqd < 0.0# Or tot_type1 < type1s_reqd Or tot_type1 > POP_SIZE) Then
            crit_neghypergeometric = ErrorValue
        ElseIf (crit_prob = 0.0#) Then
            crit_neghypergeometric = ErrorValue
        ElseIf (POP_SIZE = tot_type1) Then
            crit_neghypergeometric = 0.0#
        ElseIf (crit_prob = 1.0#) Then
            crit_neghypergeometric = POP_SIZE - tot_type1
        Else
            Dim ha1 As Double, hprob As Double, hswap As Boolean
            Dim i As Double, pr As Double
            crit_neghypergeometric = critneghyperg(type1s_reqd, tot_type1, POP_SIZE, crit_prob)
            i = crit_neghypergeometric
            pr = hypergeometric(i, type1s_reqd, POP_SIZE - tot_type1 - i, tot_type1 - type1s_reqd, False, ha1, hprob, hswap)
            If (pr = crit_prob) Then
            ElseIf (pr > crit_prob) Then
                i = i - 1.0#
                pr = hypergeometric(i, type1s_reqd, POP_SIZE - tot_type1 - i, tot_type1 - type1s_reqd, False, ha1, hprob, hswap)
                If (pr >= crit_prob) Then
                    crit_neghypergeometric = i
                End If
            Else
                crit_neghypergeometric = i + 1.0#
            End If
        End If
        crit_neghypergeometric = GetRidOfMinusZeroes(crit_neghypergeometric)
    End Function

    Public Function comp_crit_neghypergeometric(ByVal type1s_reqd As Double, ByVal tot_type1 As Double, ByVal POP_SIZE As Double, ByVal crit_prob As Double) As Double
        type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd)
        tot_type1 = AlterForIntegralChecks_Others(tot_type1)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (crit_prob < 0.0# Or crit_prob > 1.0#) Then
            comp_crit_neghypergeometric = ErrorValue
        ElseIf (type1s_reqd <= 0.0# Or tot_type1 < type1s_reqd Or tot_type1 > POP_SIZE) Then
            comp_crit_neghypergeometric = ErrorValue
        ElseIf (crit_prob = 1.0#) Then
            comp_crit_neghypergeometric = ErrorValue
        ElseIf (crit_prob = 0.0# Or POP_SIZE = tot_type1) Then
            comp_crit_neghypergeometric = POP_SIZE - tot_type1
        Else
            Dim ha1 As Double, hprob As Double, hswap As Boolean
            Dim i As Double, pr As Double
            comp_crit_neghypergeometric = critcompneghyperg(type1s_reqd, tot_type1, POP_SIZE, crit_prob)
            i = comp_crit_neghypergeometric
            pr = hypergeometric(i, type1s_reqd, POP_SIZE - tot_type1 - i, tot_type1 - type1s_reqd, True, ha1, hprob, hswap)
            If (pr = crit_prob) Then
            ElseIf (pr < crit_prob) Then
                i = i - 1.0#
                pr = hypergeometric(i, type1s_reqd, POP_SIZE - tot_type1 - i, tot_type1 - type1s_reqd, True, ha1, hprob, hswap)
                If (pr <= crit_prob) Then
                    comp_crit_neghypergeometric = i
                End If
            Else
                comp_crit_neghypergeometric = i + 1.0#
            End If
        End If
        comp_crit_neghypergeometric = GetRidOfMinusZeroes(comp_crit_neghypergeometric)
    End Function

    Public Function lcb_neghypergeometric(ByVal type2s As Double, ByVal type1s_reqd As Double, ByVal POP_SIZE As Double, ByVal prob As Double) As Double
        type2s = AlterForIntegralChecks_Others(type2s)
        type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (prob < 0.0# Or prob > 1.0#) Then
            lcb_neghypergeometric = ErrorValue
        ElseIf (type1s_reqd <= 0.0# Or type1s_reqd > POP_SIZE Or type2s > POP_SIZE - type1s_reqd) Then
            lcb_neghypergeometric = ErrorValue
        ElseIf (prob = 0.0# Or POP_SIZE = type2s + type1s_reqd) Then
            lcb_neghypergeometric = type1s_reqd
        ElseIf (prob = 1.0#) Then
            lcb_neghypergeometric = POP_SIZE - type2s
        ElseIf (prob < 0.5) Then
            lcb_neghypergeometric = critneghyperg(type1s_reqd, type2s + type1s_reqd, POP_SIZE, prob * (1.000000000001)) + type1s_reqd
        Else
            lcb_neghypergeometric = critcompneghyperg(type1s_reqd, type2s + type1s_reqd, POP_SIZE, (1.0# - prob) * (1.0# - 0.000000000001)) + type1s_reqd
        End If
        lcb_neghypergeometric = GetRidOfMinusZeroes(lcb_neghypergeometric)
    End Function

    Public Function ucb_neghypergeometric(ByVal type2s As Double, ByVal type1s_reqd As Double, ByVal POP_SIZE As Double, ByVal prob As Double) As Double
        type2s = AlterForIntegralChecks_Others(type2s)
        type1s_reqd = AlterForIntegralChecks_Others(type1s_reqd)
        POP_SIZE = AlterForIntegralChecks_Others(POP_SIZE)
        If (prob < 0.0# Or prob > 1.0#) Then
            ucb_neghypergeometric = ErrorValue
        ElseIf (type1s_reqd <= 0.0# Or type1s_reqd > POP_SIZE Or type2s > POP_SIZE - type1s_reqd) Then
            ucb_neghypergeometric = ErrorValue
        ElseIf (prob = 0.0# Or type2s = 0.0# Or POP_SIZE = type2s + type1s_reqd) Then
            ucb_neghypergeometric = POP_SIZE - type2s
        ElseIf (prob = 1.0#) Then
            ucb_neghypergeometric = type1s_reqd
        ElseIf (prob < 0.5) Then
            ucb_neghypergeometric = critcompneghyperg(type1s_reqd, type2s + type1s_reqd - 1.0#, POP_SIZE, prob * (1.0# - 0.000000000001)) + type1s_reqd - 1.0#
        Else
            ucb_neghypergeometric = critneghyperg(type1s_reqd, type2s + type1s_reqd - 1.0#, POP_SIZE, (1.0# - prob) * (1.000000000001)) + type1s_reqd - 1.0#
        End If
        ucb_neghypergeometric = GetRidOfMinusZeroes(ucb_neghypergeometric)
    End Function

    Public Function pdf_exponential(ByVal x As Double, ByVal Lambda As Double) As Double
        If (Lambda <= 0.0#) Then
            pdf_exponential = ErrorValue
        ElseIf (x < 0.0#) Then
            pdf_exponential = 0.0#
        Else
            pdf_exponential = Math.Exp(-Lambda * x + Math.Log(Lambda))
        End If
        pdf_exponential = GetRidOfMinusZeroes(pdf_exponential)
    End Function

    Public Function cdf_exponential(ByVal x As Double, ByVal Lambda As Double) As Double
        If (Lambda <= 0.0#) Then
            cdf_exponential = ErrorValue
        ElseIf (x < 0.0#) Then
            cdf_exponential = 0.0#
        Else
            cdf_exponential = -expm1(-Lambda * x)
        End If
        cdf_exponential = GetRidOfMinusZeroes(cdf_exponential)
    End Function

    Public Function comp_cdf_exponential(ByVal x As Double, ByVal Lambda As Double) As Double
        If (Lambda <= 0.0#) Then
            comp_cdf_exponential = ErrorValue
        ElseIf (x < 0.0#) Then
            comp_cdf_exponential = 1.0#
        Else
            comp_cdf_exponential = Math.Exp(-Lambda * x)
        End If
        comp_cdf_exponential = GetRidOfMinusZeroes(comp_cdf_exponential)
    End Function

    Public Function inv_exponential(ByVal prob As Double, ByVal Lambda As Double) As Double
        If (Lambda <= 0.0# Or prob < 0.0# Or prob >= 1.0#) Then
            inv_exponential = ErrorValue
        Else
            inv_exponential = -log0(-prob) / Lambda
        End If
        inv_exponential = GetRidOfMinusZeroes(inv_exponential)
    End Function

    Public Function comp_inv_exponential(ByVal prob As Double, ByVal Lambda As Double) As Double
        If (Lambda <= 0.0# Or prob <= 0.0# Or prob > 1.0#) Then
            comp_inv_exponential = ErrorValue
        Else
            comp_inv_exponential = -Math.Log(prob) / Lambda
        End If
        comp_inv_exponential = GetRidOfMinusZeroes(comp_inv_exponential)
    End Function

    Public Function pdf_normal(ByVal x As Double) As Double
        If (Math.Abs(x) < 40.0#) Then
            pdf_normal = Math.Exp(-x * x * 0.5 - lstpi)
        Else
            pdf_normal = 0.0#
        End If
        pdf_normal = GetRidOfMinusZeroes(pdf_normal)
    End Function

    Public Function cdf_normal(ByVal x As Double) As Double
        cdf_normal = cnormal(x)
        cdf_normal = GetRidOfMinusZeroes(cdf_normal)
    End Function

    Public Function inv_normal(ByVal prob As Double) As Double
        If (prob <= 0.0# Or prob >= 1.0#) Then
            inv_normal = ErrorValue
        Else
            inv_normal = invcnormal(prob)
        End If
        inv_normal = GetRidOfMinusZeroes(inv_normal)
    End Function

    Public Function pdf_chi_sq(ByVal x As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        pdf_chi_sq = pdf_gamma(x, df / 2.0#, 2.0#)
        pdf_chi_sq = GetRidOfMinusZeroes(pdf_chi_sq)
    End Function

    Public Function cdf_chi_sq(ByVal x As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        If (df <= 0.0#) Then
            cdf_chi_sq = ErrorValue
        ElseIf (x <= 0.0#) Then
            cdf_chi_sq = 0.0#
        Else
            cdf_chi_sq = GAMMA(x / 2.0#, df / 2.0#)
        End If
        cdf_chi_sq = GetRidOfMinusZeroes(cdf_chi_sq)
    End Function

    Public Function comp_cdf_chi_sq(ByVal x As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        If (df <= 0.0#) Then
            comp_cdf_chi_sq = ErrorValue
        ElseIf (x <= 0.0#) Then
            comp_cdf_chi_sq = 1.0#
        Else
            comp_cdf_chi_sq = compgamma(x / 2.0#, df / 2.0#)
        End If
        comp_cdf_chi_sq = GetRidOfMinusZeroes(comp_cdf_chi_sq)
    End Function

    Public Function inv_chi_sq(ByVal prob As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        If (df <= 0.0# Or prob < 0.0# Or prob >= 1.0#) Then
            inv_chi_sq = ErrorValue
        ElseIf (prob = 0.0#) Then
            inv_chi_sq = 0.0#
        Else
            inv_chi_sq = 2.0# * invgamma(df / 2.0#, prob)
        End If
        inv_chi_sq = GetRidOfMinusZeroes(inv_chi_sq)
    End Function

    Public Function comp_inv_chi_sq(ByVal prob As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        If (df <= 0.0# Or prob <= 0.0# Or prob > 1.0#) Then
            comp_inv_chi_sq = ErrorValue
        ElseIf (prob = 1.0#) Then
            comp_inv_chi_sq = 0.0#
        Else
            comp_inv_chi_sq = 2.0# * invcompgamma(df / 2.0#, prob)
        End If
        comp_inv_chi_sq = GetRidOfMinusZeroes(comp_inv_chi_sq)
    End Function

    Public Function pdf_gamma(ByVal x As Double, ByVal shape_param As Double, ByVal scale_param As Double) As Double
        Dim xs As Double
        If (shape_param <= 0.0# Or scale_param <= 0.0#) Then
            pdf_gamma = ErrorValue
        ElseIf (x < 0.0#) Then
            pdf_gamma = 0.0#
        ElseIf (x = 0.0#) Then
            If (shape_param < 1.0#) Then
                pdf_gamma = ErrorValue
            ElseIf (shape_param = 1.0#) Then
                pdf_gamma = 1.0# / scale_param
            Else
                pdf_gamma = 0.0#
            End If
        Else
            xs = x / scale_param
            pdf_gamma = poissonTerm(shape_param, xs, xs - shape_param, Math.Log(shape_param) - Math.Log(x))
        End If
        pdf_gamma = GetRidOfMinusZeroes(pdf_gamma)
    End Function

    Public Function cdf_gamma(ByVal x As Double, ByVal shape_param As Double, ByVal scale_param As Double) As Double
        If (shape_param <= 0.0# Or scale_param <= 0.0#) Then
            cdf_gamma = ErrorValue
        ElseIf (x <= 0.0#) Then
            cdf_gamma = 0.0#
        Else
            cdf_gamma = GAMMA(x / scale_param, shape_param)
        End If
        cdf_gamma = GetRidOfMinusZeroes(cdf_gamma)
    End Function

    Public Function comp_cdf_gamma(ByVal x As Double, ByVal shape_param As Double, ByVal scale_param As Double) As Double
        If (shape_param <= 0.0# Or scale_param <= 0.0#) Then
            comp_cdf_gamma = ErrorValue
        ElseIf (x <= 0.0#) Then
            comp_cdf_gamma = 1.0#
        Else
            comp_cdf_gamma = compgamma(x / scale_param, shape_param)
        End If
        comp_cdf_gamma = GetRidOfMinusZeroes(comp_cdf_gamma)
    End Function

    Public Function inv_gamma(ByVal prob As Double, ByVal shape_param As Double, ByVal scale_param As Double) As Double
        If (shape_param <= 0.0# Or scale_param <= 0.0# Or prob < 0.0# Or prob >= 1.0#) Then
            inv_gamma = ErrorValue
        ElseIf (prob = 0.0#) Then
            inv_gamma = 0.0#
        Else
            inv_gamma = scale_param * invgamma(shape_param, prob)
        End If
        inv_gamma = GetRidOfMinusZeroes(inv_gamma)
    End Function

    Public Function comp_inv_gamma(ByVal prob As Double, ByVal shape_param As Double, ByVal scale_param As Double) As Double
        If (shape_param <= 0.0# Or scale_param <= 0.0# Or prob <= 0.0# Or prob > 1.0#) Then
            comp_inv_gamma = ErrorValue
        ElseIf (prob = 1.0#) Then
            comp_inv_gamma = 0.0#
        Else
            comp_inv_gamma = scale_param * invcompgamma(shape_param, prob)
        End If
        comp_inv_gamma = GetRidOfMinusZeroes(comp_inv_gamma)
    End Function

    Private Function pdftdist(ByVal x As Double, ByVal k As Double) As Double
        '//Probability density for a variate from t-distribution with k degress of freedom
        Dim x2 As Double, k2 As Double, logterm As Double
        If (k <= 0.0#) Then
            pdftdist = ErrorValue
        ElseIf (k > 1.0E+30) Then
            pdftdist = pdf_normal(x)
        Else
            If Math.Abs(x) >= Min(1.0#, k) Then
                k2 = k / x
                x2 = x + k2
                k2 = k2 / x2
                x2 = x / x2
            Else
                x2 = x * x
                k2 = k + x2
                x2 = x2 / k2
                k2 = k / k2
            End If
            If (k2 < cSmall) Then
                logterm = Math.Log(k) - 2.0# * Math.Log(Math.Abs(x))
            ElseIf (Math.Abs(x2) < 0.5) Then
                logterm = log0(-x2)
            Else
                logterm = Math.Log(k2)
            End If
            x2 = k * 0.5
            pdftdist = Math.Exp(0.5 + (k + 1.0#) * 0.5 * logterm + x2 * log0(-0.5 / (x2 + 1)) + logfbit(x2 - 0.5) - logfbit(x2)) * Math.Sqrt(x2 / ((1.0# + x2))) * OneOverSqrTwoPi
        End If
    End Function

    Public Function pdf_tdist(ByVal x As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        pdf_tdist = pdftdist(x, df)
        pdf_tdist = GetRidOfMinusZeroes(pdf_tdist)
    End Function

    Public Function cdf_tdist(ByVal x As Double, ByVal df As Double) As Double
        Dim tdistDensity As Double
        df = AlterForIntegralChecks_df(df)
        If (df <= 0.0#) Then
            cdf_tdist = ErrorValue
        Else
            cdf_tdist = tdist(x, df, tdistDensity)
        End If
        cdf_tdist = GetRidOfMinusZeroes(cdf_tdist)
    End Function

    Public Function inv_tdist(ByVal prob As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        If (df <= 0.0#) Then
            inv_tdist = ErrorValue
        ElseIf (prob <= 0.0# Or prob >= 1.0#) Then
            inv_tdist = ErrorValue
        Else
            inv_tdist = invtdist(prob, df)
        End If
        inv_tdist = GetRidOfMinusZeroes(inv_tdist)
    End Function

    Public Function pdf_fdist(ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0#) Then
            pdf_fdist = ErrorValue
        ElseIf (x < 0.0#) Then
            pdf_fdist = 0.0#
        ElseIf (x = 0.0# And df1 > 2.0#) Then
            pdf_fdist = 0.0#
        ElseIf (x = 0.0# And df1 < 2.0#) Then
            pdf_fdist = ErrorValue
        ElseIf (x = 0.0#) Then
            pdf_fdist = 1.0#
        Else
            Dim p As Double, Q As Double
            If x > 1.0# Then
                Q = df2 / x
                p = Q + df1
                Q = Q / p
                p = df1 / p
            Else
                p = df1 * x
                Q = df2 + p
                p = p / Q
                Q = df2 / Q
            End If
            'If p < cSmall And x <> 0# Or q < cSmall Then
            '   pdf_fdist = ErrorValue
            '   Exit Function
            'End If
            df2 = df2 / 2.0#
            df1 = df1 / 2.0#
            If (df1 >= 1.0#) Then
                df1 = df1 - 1.0#
                pdf_fdist = binomialTerm(df1, df2, p, Q, df2 * p - df1 * Q, Math.Log((df1 + 1.0#) * Q))
            Else
                pdf_fdist = df1 * df1 * Q / (p * (df1 + df2)) * binomialTerm(df1, df2, p, Q, df2 * p - df1 * Q, 0.0#)
            End If
        End If
        pdf_fdist = GetRidOfMinusZeroes(pdf_fdist)
    End Function

    Public Function cdf_fdist(ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0#) Then
            cdf_fdist = ErrorValue
        ElseIf (x <= 0.0#) Then
            cdf_fdist = 0.0#
        Else
            Dim p As Double, Q As Double
            If x > 1.0# Then
                Q = df2 / x
                p = Q + df1
                Q = Q / p
                p = df1 / p
            Else
                p = df1 * x
                Q = df2 + p
                p = p / Q
                Q = df2 / Q
            End If
            'If p < cSmall And x <> 0# Or q < cSmall Then
            '   cdf_fdist = ErrorValue
            '   Exit Function
            'End If
            df2 = df2 / 2.0#
            df1 = df1 / 2.0#
            If (p < 0.5) Then
                cdf_fdist = BETA(p, df1, df2)
            Else
                cdf_fdist = compbeta(Q, df2, df1)
            End If
        End If
        cdf_fdist = GetRidOfMinusZeroes(cdf_fdist)
    End Function

    Public Function comp_cdf_fdist(ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0#) Then
            comp_cdf_fdist = ErrorValue
        ElseIf (x <= 0.0#) Then
            comp_cdf_fdist = 1.0#
        Else
            Dim p As Double, Q As Double
            If x > 1.0# Then
                Q = df2 / x
                p = Q + df1
                Q = Q / p
                p = df1 / p
            Else
                p = df1 * x
                Q = df2 + p
                p = p / Q
                Q = df2 / Q
            End If
            'If p < cSmall And x <> 0# Or q < cSmall Then
            '   comp_cdf_fdist = ErrorValue
            '   Exit Function
            'End If
            df2 = df2 / 2.0#
            df1 = df1 / 2.0#
            If (p < 0.5) Then
                comp_cdf_fdist = compbeta(p, df1, df2)
            Else
                comp_cdf_fdist = BETA(Q, df2, df1)
            End If
        End If
        comp_cdf_fdist = GetRidOfMinusZeroes(comp_cdf_fdist)
    End Function

    Public Function inv_fdist(ByVal prob As Double, ByVal df1 As Double, ByVal df2 As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0# Or prob < 0.0# Or prob >= 1.0#) Then
            inv_fdist = ErrorValue
        ElseIf (prob = 0.0#) Then
            inv_fdist = 0.0#
        Else
            Dim temp As Double, oneMinusP As Double
            df1 = df1 / 2.0#
            df2 = df2 / 2.0#
            temp = invbeta(df1, df2, prob, oneMinusP)
            inv_fdist = df2 * temp / (df1 * oneMinusP)
            'If oneMinusP < cSmall Then inv_fdist = ErrorValue
        End If
        inv_fdist = GetRidOfMinusZeroes(inv_fdist)
    End Function

    Public Function comp_inv_fdist(ByVal prob As Double, ByVal df1 As Double, ByVal df2 As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0# Or prob <= 0.0# Or prob > 1.0#) Then
            comp_inv_fdist = ErrorValue
        ElseIf (prob = 1.0#) Then
            comp_inv_fdist = 0.0#
        Else
            Dim temp As Double, oneMinusP As Double
            df1 = df1 / 2.0#
            df2 = df2 / 2.0#
            temp = invcompbeta(df1, df2, prob, oneMinusP)
            comp_inv_fdist = df2 * temp / (df1 * oneMinusP)
            'If oneMinusP < cSmall Then comp_inv_fdist = ErrorValue
        End If
        comp_inv_fdist = GetRidOfMinusZeroes(comp_inv_fdist)
    End Function

    Public Function pdf_BETA(ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double) As Double
        If (shape_param1 <= 0.0# Or shape_param2 <= 0.0#) Then
            pdf_BETA = ErrorValue
        ElseIf (x < 0.0# Or x > 1.0#) Then
            pdf_BETA = 0.0#
        ElseIf (x = 0.0# And shape_param1 < 1.0# Or x = 1.0# And shape_param2 < 1.0#) Then
            pdf_BETA = ErrorValue
        ElseIf (x = 0.0# And shape_param1 = 1.0#) Then
            pdf_BETA = shape_param2
        ElseIf (x = 1.0# And shape_param2 = 1.0#) Then
            pdf_BETA = shape_param1
        ElseIf ((x = 0.0#) Or (x = 1.0#)) Then
            pdf_BETA = 0.0#
        Else
            Dim MX As Double, mn As Double
            MX = max(shape_param1, shape_param2)
            mn = Min(shape_param1, shape_param2)
            pdf_BETA = binomialTerm(shape_param1, shape_param2, x, 1.0# - x, (shape_param1 + shape_param2) * x - shape_param1, Math.Log(MX / (mn + MX)) + Math.Log(mn) - Math.Log(x * (1.0# - x)))
        End If
        pdf_BETA = GetRidOfMinusZeroes(pdf_BETA)
    End Function

    Public Function cdf_BETA(ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double) As Double
        If (shape_param1 <= 0.0# Or shape_param2 <= 0.0#) Then
            cdf_BETA = ErrorValue
        ElseIf (x <= 0.0#) Then
            cdf_BETA = 0.0#
        ElseIf (x >= 1.0#) Then
            cdf_BETA = 1.0#
        Else
            cdf_BETA = BETA(x, shape_param1, shape_param2)
        End If
        cdf_BETA = GetRidOfMinusZeroes(cdf_BETA)
    End Function

    Public Function comp_cdf_BETA(ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double) As Double
        If (shape_param1 <= 0.0# Or shape_param2 <= 0.0#) Then
            comp_cdf_BETA = ErrorValue
        ElseIf (x <= 0.0#) Then
            comp_cdf_BETA = 1.0#
        ElseIf (x >= 1.0#) Then
            comp_cdf_BETA = 0.0#
        Else
            comp_cdf_BETA = compbeta(x, shape_param1, shape_param2)
        End If
        comp_cdf_BETA = GetRidOfMinusZeroes(comp_cdf_BETA)
    End Function

    Public Function inv_BETA(ByVal prob As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double) As Double
        If (shape_param1 <= 0.0# Or shape_param2 <= 0.0# Or prob < 0.0# Or prob > 1.0#) Then
            inv_BETA = ErrorValue
        Else
            Dim oneMinusP As Double
            inv_BETA = invbeta(shape_param1, shape_param2, prob, oneMinusP)
        End If
        inv_BETA = GetRidOfMinusZeroes(inv_BETA)
    End Function

    Public Function comp_inv_BETA(ByVal prob As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double) As Double
        If (shape_param1 <= 0.0# Or shape_param2 <= 0.0# Or prob < 0.0# Or prob > 1.0#) Then
            comp_inv_BETA = ErrorValue
        Else
            Dim oneMinusP As Double
            comp_inv_BETA = invcompbeta(shape_param1, shape_param2, prob, oneMinusP)
        End If
        comp_inv_BETA = GetRidOfMinusZeroes(comp_inv_BETA)
    End Function

    Private Function gamma_nc1(ByVal x As Double, ByVal a As Double, ByVal nc As Double, ByRef nc_derivative As Double) As Double
        Dim aa As Double, bb As Double, nc_dtemp As Double
        Dim N As Double, p As Double, W As Double, S As Double, ps As Double
        Dim Result As Double, term As Double, ptx As Double, ptnc As Double
        N = a + Math.Sqrt(a ^ 2 + 4.0# * nc * x)
        If N > 0.0# Then N = Int(2.0# * nc * x / N)
        aa = N + a
        bb = N
        ptnc = poissonTerm(N, nc, nc - N, 0.0#)
        ptx = poissonTerm(aa, x, x - aa, 0.0#)
        aa = aa + 1.0#
        bb = bb + 1.0#
        p = nc / bb
        ps = p
        nc_derivative = ps
        S = x / aa
        W = p
        term = S * W
        Result = term
        If ptx > 0.0# Then
            While (((term > 0.000000000000001 * Result) And (p > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_derivative))
                aa = aa + 1.0#
                bb = bb + 1.0#
                p = nc / bb * p
                ps = p * S
                nc_derivative = nc_derivative + ps
                S = x / aa * S
                W = W + p
                term = S * W
                Result = Result + term
            End While
            W = W * ptnc
        Else
            W = comppoisson(N, nc, nc - N)
        End If
        gamma_nc1 = Result * ptx * ptnc + comppoisson(a + bb, x, (x - a) - bb) * W
        ps = 1.0#
        nc_dtemp = 0.0#
        aa = N + a
        bb = N
        p = 1.0#
        S = ptx
        W = GAMMA(x, aa)
        term = p * W
        Result = term
        While bb > 0.0# And ((term > 0.000000000000001 * Result) Or (ps > 0.0000000000000001 * nc_dtemp))
            S = aa / x * S
            ps = p * S
            nc_dtemp = nc_dtemp + ps
            p = bb / nc * p
            W = W + S
            term = p * W
            Result = Result + term
            aa = aa - 1.0#
            bb = bb - 1.0#
        End While
        If bb = 0.0# Then aa = a
        If N > 0.0# Then
            nc_dtemp = nc_derivative * ptx + nc_dtemp + p * aa / x * S
        Else
            nc_dtemp = poissonTerm(aa, x, x - aa, Math.Log(nc_derivative * x + aa) - Math.Log(x))
        End If
        gamma_nc1 = gamma_nc1 + Result * ptnc + cpoisson(bb - 1.0#, nc, nc - bb + 1.0#) * W
        If nc_dtemp = 0.0# Then
            nc_derivative = 0.0#
        Else
            nc_derivative = poissonTerm(N, nc, nc - N, Math.Log(nc_dtemp))
        End If
    End Function

    Private Function comp_gamma_nc1(ByVal x As Double, ByVal a As Double, ByVal nc As Double, ByRef nc_derivative As Double) As Double
        Dim aa As Double, bb As Double, nc_dtemp As Double
        Dim N As Double, p As Double, W As Double, S As Double, ps As Double
        Dim Result As Double, term As Double, ptx As Double, ptnc As Double
        N = a + Math.Sqrt(a ^ 2 + 4.0# * nc * x)
        If N > 0.0# Then N = Int(2.0# * nc * x / N)
        aa = N + a
        bb = N
        ptnc = poissonTerm(N, nc, nc - N, 0.0#)
        ptx = poissonTerm(aa, x, x - aa, 0.0#)
        S = 1.0#
        ps = 1.0#
        nc_dtemp = 0.0#
        p = 1.0#
        W = p
        term = 1.0#
        Result = 0.0#
        If ptx > 0.0# Then
            While bb > 0.0# And (((term > 0.000000000000001 * Result) And (p > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_dtemp))
                S = aa / x * S
                ps = p * S
                nc_dtemp = nc_dtemp + ps
                p = bb / nc * p
                term = S * W
                Result = Result + term
                W = W + p
                aa = aa - 1.0#
                bb = bb - 1.0#
            End While
            W = W * ptnc
        Else
            W = cpoisson(N, nc, nc - N)
        End If
        If bb = 0.0# Then aa = a
        If N > 0.0# Then
            nc_dtemp = (nc_dtemp + p * aa / x * S) * ptx
        ElseIf aa = 0 And x > 0 Then
            nc_dtemp = 0.0#
        Else
            nc_dtemp = poissonTerm(aa, x, x - aa, Math.Log(aa) - Math.Log(x))
        End If
        comp_gamma_nc1 = Result * ptx * ptnc + compgamma(x, aa) * W
        aa = N + a
        bb = N
        ps = 1.0#
        nc_derivative = 0.0#
        p = 1.0#
        S = ptx
        W = compgamma(x, aa)
        term = 0.0#
        Result = term
        Do
            W = W + S
            aa = aa + 1.0#
            bb = bb + 1.0#
            p = nc / bb * p
            ps = p * S
            nc_derivative = nc_derivative + ps
            S = x / aa * S
            term = p * W
            Result = Result + term
        Loop While (((term > 0.000000000000001 * Result) And (S > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_derivative))
        comp_gamma_nc1 = comp_gamma_nc1 + Result * ptnc + comppoisson(bb, nc, nc - bb) * W
        nc_dtemp = nc_derivative + nc_dtemp
        If nc_dtemp = 0.0# Then
            nc_derivative = 0.0#
        Else
            nc_derivative = poissonTerm(N, nc, nc - N, Math.Log(nc_dtemp))
        End If
    End Function

    Private Function inv_gamma_nc1(ByVal prob As Double, ByVal a As Double, ByVal nc As Double) As Double
        'Uses approx in A&S 26.4.27 for to get initial estimate the modified NR to improve it.
        Dim x As Double, pr As Double, dif As Double
        Dim hi As Double, lo As Double, nc_derivative As Double
        If (prob > 0.5) Then
            inv_gamma_nc1 = comp_inv_gamma_nc1(1.0# - prob, a, nc)
            Exit Function
        End If

        lo = 0.0#
        hi = 1.0E+308
        pr = Math.Exp(-nc)
        If pr > prob Then
            If 2.0# * prob > pr Then
                x = comp_inv_gamma((pr - prob) / pr, a + cSmall, 1.0#)
            Else
                x = inv_gamma(prob / pr, a + cSmall, 1.0#)
            End If
            If x < cSmall Then
                x = cSmall
                pr = gamma_nc1(x, a, nc, nc_derivative)
                If pr > prob Then
                    inv_gamma_nc1 = 0.0#
                    Exit Function
                End If
            End If
        Else
            x = inv_gamma(prob, (a + nc) / (1.0# + nc / (a + nc)), 1.0#)
            x = x * (1.0# + nc / (a + nc))
        End If
        dif = x
        Do
            pr = gamma_nc1(x, a, nc, nc_derivative)
            If pr < 3.0E-308 And nc_derivative = 0.0# Then
                lo = x
                dif = dif / 2.0#
                x = x - dif
            ElseIf nc_derivative = 0.0# Then
                hi = x
                dif = dif / 2.0#
                x = x - dif
            Else
                If pr < prob Then
                    lo = x
                Else
                    hi = x
                End If
                dif = -(pr / nc_derivative) * logdif(pr, prob)
                If x + dif < lo Then
                    dif = (lo - x) / 2.0#
                ElseIf x + dif > hi Then
                    dif = (hi - x) / 2.0#
                End If
                x = x + dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(x) * 0.0000000001))
        inv_gamma_nc1 = x
    End Function

    Private Function comp_inv_gamma_nc1(ByVal prob As Double, ByVal a As Double, ByVal nc As Double) As Double
        'Uses approx in A&S 26.4.27 for to get initial estimate the modified NR to improve it.
        Dim x As Double, pr As Double, dif As Double
        Dim hi As Double, lo As Double, nc_derivative As Double
        If (prob > 0.5) Then
            comp_inv_gamma_nc1 = inv_gamma_nc1(1.0# - prob, a, nc)
            Exit Function
        End If

        lo = 0.0#
        hi = 1.0E+308
        pr = Math.Exp(-nc)
        If pr > prob Then
            x = comp_inv_gamma(prob / pr, a + cSmall, 1.0#) ' Is this as small as x could be?
        Else
            x = comp_inv_gamma(prob, (a + nc) / (1.0# + nc / (a + nc)), 1.0#)
            x = x * (1.0# + nc / (a + nc))
        End If
        If x < cSmall Then x = cSmall
        dif = x
        Do
            pr = comp_gamma_nc1(x, a, nc, nc_derivative)
            If pr < 3.0E-308 And nc_derivative = 0.0# Then
                hi = x
                dif = dif / 2.0#
                x = x - dif
            ElseIf nc_derivative = 0.0# Then
                lo = x
                dif = dif / 2.0#
                x = x - dif
            Else
                If pr < prob Then
                    hi = x
                Else
                    lo = x
                End If
                dif = (pr / nc_derivative) * logdif(pr, prob)
                If x + dif < lo Then
                    dif = (lo - x) / 2.0#
                ElseIf x + dif > hi Then
                    dif = (hi - x) / 2.0#
                End If
                x = x + dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(x) * 0.0000000001))
        comp_inv_gamma_nc1 = x
    End Function

    Private Function ncp_gamma_nc1(ByVal prob As Double, ByVal x As Double, ByVal a As Double) As Double
        'Uses Normal approx for difference of 2 poisson distributed variables  to get initial estimate the modified NR to improve it.
        Dim ncp As Double, pr As Double, dif As Double, temp As Double, deriv As Double, B As Double, sqarg As Double, checked_nc_limit As Boolean, checked_0_limit As Boolean
        Dim hi As Double, lo As Double
        If (prob > 0.5) Then
            ncp_gamma_nc1 = comp_ncp_gamma_nc1(1.0# - prob, x, a)
            Exit Function
        End If

        lo = 0.0#
        hi = nc_limit
        checked_0_limit = False
        checked_nc_limit = False
        temp = inv_normal(prob) ^ 2
        B = 2.0# * (x - a) + temp
        sqarg = B ^ 2 - 4 * ((x - a) ^ 2 - temp * x)
        If sqarg < 0 Then
            ncp = B / 2
        Else
            ncp = (B + Math.Sqrt(sqarg)) / 2
        End If
        ncp = max(0.0#, Min(ncp, nc_limit))
        If ncp = 0.0# Then
            pr = cdf_gamma_nc(x, a, 0.0#)
            If pr < prob Then
                If (inv_gamma(prob, a, 1) <= x) Then
                    ncp_gamma_nc1 = 0.0#
                Else
                    ncp_gamma_nc1 = ErrorValue
                End If
                Exit Function
            Else
                checked_0_limit = True
            End If
        ElseIf ncp = nc_limit Then
            pr = cdf_gamma_nc(x, a, ncp)
            If pr > prob Then
                ncp_gamma_nc1 = ErrorValue
                Exit Function
            Else
                checked_nc_limit = True
            End If
        End If
        dif = ncp
        Do
            pr = cdf_gamma_nc(x, a, ncp)
            'Debug.Print ncp, pr, prob
            deriv = pdf_gamma_nc(x, a + 1.0#, ncp)
            If pr < 3.0E-308 And deriv = 0.0# Then
                hi = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            ElseIf deriv = 0.0# Then
                lo = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            Else
                If pr < prob Then
                    hi = ncp
                Else
                    lo = ncp
                End If
                dif = (pr / deriv) * logdif(pr, prob)
                If ncp + dif < lo Then
                    dif = (lo - ncp) / 2.0#
                    If Not checked_0_limit And (lo = 0.0#) Then
                        temp = cdf_gamma_nc(x, a, lo)
                        If temp < prob Then
                            If (inv_gamma(prob, a, 1) <= x) Then
                                ncp_gamma_nc1 = 0.0#
                            Else
                                ncp_gamma_nc1 = ErrorValue
                            End If
                            Exit Function
                        Else
                            checked_0_limit = True
                        End If
                    End If
                ElseIf ncp + dif > hi Then
                    dif = (hi - ncp) / 2.0#
                    If Not checked_nc_limit And (hi = nc_limit) Then
                        pr = cdf_gamma_nc(x, a, hi)
                        If pr > prob Then
                            ncp_gamma_nc1 = ErrorValue
                            Exit Function
                        Else
                            ncp = hi
                            deriv = pdf_gamma_nc(x, a + 1.0#, ncp)
                            dif = (pr / deriv) * logdif(pr, prob)
                            If ncp + dif < lo Then
                                dif = (lo - ncp) / 2.0#
                            End If
                            checked_nc_limit = True
                        End If
                    End If
                End If
                ncp = ncp + dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(ncp) * 0.0000000001))
        ncp_gamma_nc1 = ncp
        'Debug.Print "ncp_gamma_nc1", ncp_gamma_nc1
    End Function

    Private Function comp_ncp_gamma_nc1(ByVal prob As Double, ByVal x As Double, ByVal a As Double) As Double
        'Uses Normal approx for difference of 2 poisson distributed variables  to get initial estimate the modified NR to improve it.
        Dim ncp As Double, pr As Double, dif As Double, temp As Double, deriv As Double, B As Double, sqarg As Double, checked_nc_limit As Boolean, checked_0_limit As Boolean
        Dim hi As Double, lo As Double
        If (prob > 0.5) Then
            comp_ncp_gamma_nc1 = ncp_gamma_nc1(1.0# - prob, x, a)
            Exit Function
        End If

        lo = 0.0#
        hi = nc_limit
        checked_0_limit = False
        checked_nc_limit = False
        temp = inv_normal(prob) ^ 2
        B = 2.0# * (x - a) + temp
        sqarg = B ^ 2 - 4 * ((x - a) ^ 2 - temp * x)
        If sqarg < 0 Then
            ncp = B / 2
        Else
            ncp = (B - Math.Sqrt(sqarg)) / 2
        End If
        ncp = max(0.0#, ncp)
        If ncp <= 1.0# Then
            pr = comp_cdf_gamma_nc(x, a, 0.0#)
            If pr > prob Then
                If (comp_inv_gamma(prob, a, 1) <= x) Then
                    comp_ncp_gamma_nc1 = 0.0#
                Else
                    comp_ncp_gamma_nc1 = ErrorValue
                End If
                Exit Function
            Else
                checked_0_limit = True
            End If
            deriv = pdf_gamma_nc(x, a + 1.0#, ncp)
            If deriv = 0.0# Then
                ncp = nc_limit
            ElseIf a < 1 Then
                ncp = (prob - pr) / deriv
                If ncp >= nc_limit Then
                    ncp = -(pr / deriv) * logdif(pr, prob)
                End If
            Else
                ncp = -(pr / deriv) * logdif(pr, prob)
            End If
        End If
        ncp = Min(ncp, nc_limit)
        If ncp = nc_limit Then
            pr = comp_cdf_gamma_nc(x, a, ncp)
            If pr < prob Then
                comp_ncp_gamma_nc1 = ErrorValue
                Exit Function
            Else
                deriv = pdf_gamma_nc(x, a + 1.0#, ncp)
                dif = -(pr / deriv) * logdif(pr, prob)
                If ncp + dif < lo Then
                    dif = (lo - ncp) / 2.0#
                End If
                checked_nc_limit = True
            End If
        End If
        dif = ncp
        Do
            pr = comp_cdf_gamma_nc(x, a, ncp)
            'Debug.Print ncp, pr, prob
            deriv = pdf_gamma_nc(x, a + 1.0#, ncp)
            If pr < 3.0E-308 And deriv = 0.0# Then
                lo = ncp
                dif = dif / 2.0#
                ncp = ncp + dif
            ElseIf deriv = 0.0# Then
                hi = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            Else
                If pr < prob Then
                    lo = ncp
                Else
                    hi = ncp
                End If
                dif = -(pr / deriv) * logdif(pr, prob)
                If ncp + dif < lo Then
                    dif = (lo - ncp) / 2.0#
                    If Not checked_0_limit And (lo = 0.0#) Then
                        temp = comp_cdf_gamma_nc(x, a, lo)
                        If temp > prob Then
                            If (comp_inv_gamma(prob, a, 1) <= x) Then
                                comp_ncp_gamma_nc1 = 0.0#
                            Else
                                comp_ncp_gamma_nc1 = ErrorValue
                            End If
                            Exit Function
                        Else
                            checked_0_limit = True
                        End If
                    End If
                ElseIf ncp + dif > hi Then
                    If Not checked_nc_limit And (hi = nc_limit) Then
                        ncp = hi
                        pr = comp_cdf_gamma_nc(x, a, ncp)
                        If pr < prob Then
                            comp_ncp_gamma_nc1 = ErrorValue
                            Exit Function
                        Else
                            deriv = pdf_gamma_nc(x, a + 1.0#, ncp)
                            dif = -(pr / deriv) * logdif(pr, prob)
                            If ncp + dif < lo Then
                                dif = (lo - ncp) / 2.0#
                            End If
                            checked_nc_limit = True
                        End If
                    Else
                        dif = (hi - ncp) / 2.0#
                    End If
                End If
                ncp = ncp + dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(ncp) * 0.0000000001))
        comp_ncp_gamma_nc1 = ncp
        'Debug.Print "comp_ncp_gamma_nc1", comp_ncp_gamma_nc1
    End Function

    Public Function pdf_gamma_nc(ByVal x As Double, ByVal shape_param As Double, ByVal nc_param As Double) As Double
        '// Calculate pdf of noncentral gamma
        Dim nc_derivative As Double
        If (shape_param < 0.0#) Or (nc_param < 0.0#) Or (nc_param > nc_limit) Then
            pdf_gamma_nc = ErrorValue
        ElseIf (x < 0.0#) Then
            pdf_gamma_nc = 0.0#
        ElseIf (shape_param = 0.0# And nc_param = 0.0# And x > 0.0#) Then
            pdf_gamma_nc = 0.0#
        ElseIf (x = 0.0# Or nc_param = 0.0#) Then
            pdf_gamma_nc = Math.Exp(-nc_param) * pdf_gamma(x, shape_param, 1.0#)
        ElseIf shape_param >= 1.0# Then
            If x >= nc_param Then
                If (x < 1.0# Or x <= shape_param + nc_param) Then
                    pdf_gamma_nc = gamma_nc1(x, shape_param, nc_param, nc_derivative)
                Else
                    pdf_gamma_nc = comp_gamma_nc1(x, shape_param, nc_param, nc_derivative)
                End If
                pdf_gamma_nc = nc_derivative
            Else
                If (nc_param < 1.0# Or nc_param <= shape_param + x) Then
                    pdf_gamma_nc = gamma_nc1(nc_param, shape_param, x, nc_derivative)
                Else
                    pdf_gamma_nc = comp_gamma_nc1(nc_param, shape_param, x, nc_derivative)
                End If
                If nc_derivative = 0.0# Then
                    pdf_gamma_nc = 0.0#
                Else
                    pdf_gamma_nc = Math.Exp(Math.Log(nc_derivative) + (shape_param - 1.0#) * (Math.Log(x) - Math.Log(nc_param)))
                End If
            End If
        Else
            If x < nc_param Then
                If (x < 1.0# Or x <= shape_param + nc_param) Then
                    pdf_gamma_nc = gamma_nc1(x, shape_param, nc_param, nc_derivative)
                Else
                    pdf_gamma_nc = comp_gamma_nc1(x, shape_param, nc_param, nc_derivative)
                End If
                pdf_gamma_nc = nc_derivative
            Else
                If (nc_param < 1.0# Or nc_param <= shape_param + x) Then
                    pdf_gamma_nc = gamma_nc1(nc_param, shape_param, x, nc_derivative)
                Else
                    pdf_gamma_nc = comp_gamma_nc1(nc_param, shape_param, x, nc_derivative)
                End If
                If nc_derivative = 0.0# Then
                    pdf_gamma_nc = 0.0#
                Else
                    pdf_gamma_nc = Math.Exp(Math.Log(nc_derivative) + (shape_param - 1.0#) * (Math.Log(x) - Math.Log(nc_param)))
                End If
            End If
        End If
        pdf_gamma_nc = GetRidOfMinusZeroes(pdf_gamma_nc)
    End Function

    Public Function cdf_gamma_nc(ByVal x As Double, ByVal shape_param As Double, ByVal nc_param As Double) As Double
        '// Calculate cdf of noncentral gamma
        Dim nc_derivative As Double
        If (shape_param < 0.0#) Or (nc_param < 0.0#) Or (nc_param > nc_limit) Then
            cdf_gamma_nc = ErrorValue
        ElseIf (x < 0.0#) Then
            cdf_gamma_nc = 0.0#
        ElseIf (x = 0.0# And shape_param = 0.0#) Then
            cdf_gamma_nc = Math.Exp(-nc_param)
        ElseIf (shape_param + nc_param = 0.0#) Then    ' limit as shape_param+nc_param->0 is degenerate point mass at zero
            cdf_gamma_nc = 1.0#                         ' if fix central gamma, then works for degenerate poisson
        ElseIf (x = 0.0#) Then
            cdf_gamma_nc = 0.0#
        ElseIf (nc_param = 0.0#) Then
            cdf_gamma_nc = GAMMA(x, shape_param)
            'ElseIf (shape_param = 0#) Then              ' extends Ruben (1974) and Cohen (1988) recurrence
            '   cdf_gamma_nc = ((x + shape_param + 2#) * gamma_nc1(x, shape_param + 2#, nc_param) + (nc_param - shape_param - 2#) * gamma_nc1(x, shape_param + 4#, nc_param) - nc_param * gamma_nc1(x, shape_param + 6#, nc_param)) / x
        ElseIf (x < 1.0# Or x <= shape_param + nc_param) Then
            cdf_gamma_nc = gamma_nc1(x, shape_param, nc_param, nc_derivative)
        Else
            cdf_gamma_nc = 1.0# - comp_gamma_nc1(x, shape_param, nc_param, nc_derivative)
        End If
        cdf_gamma_nc = GetRidOfMinusZeroes(cdf_gamma_nc)
    End Function

    Public Function comp_cdf_gamma_nc(ByVal x As Double, ByVal shape_param As Double, ByVal nc_param As Double) As Double
        '// Calculate 1-cdf of noncentral gamma
        Dim nc_derivative As Double
        If (shape_param < 0.0#) Or (nc_param < 0.0#) Or (nc_param > nc_limit) Then
            comp_cdf_gamma_nc = ErrorValue
        ElseIf (x < 0.0#) Then
            comp_cdf_gamma_nc = 1.0#
        ElseIf (x = 0.0# And shape_param = 0.0#) Then
            comp_cdf_gamma_nc = -expm1(-nc_param)
        ElseIf (shape_param + nc_param = 0.0#) Then     ' limit as shape_param+nc_param->0 is degenerate point mass at zero
            comp_cdf_gamma_nc = 0.0#                     ' if fix central gamma, then works for degenerate poisson
        ElseIf (x = 0.0#) Then
            comp_cdf_gamma_nc = 1
        ElseIf (nc_param = 0.0#) Then
            comp_cdf_gamma_nc = compgamma(x, shape_param)
            'ElseIf (shape_param = 0#) Then              ' extends Ruben (1974) and Cohen (1988) recurrence
            '   comp_cdf_gamma_nc = ((x + shape_param + 2#) * comp_gamma_nc1(x, shape_param + 2#, nc_param) + (nc_param - shape_param - 2#) * comp_gamma_nc1(x, shape_param + 4#, nc_param) - nc_param * comp_gamma_nc1(x, shape_param + 6#, nc_param)) / x
        ElseIf (x < 1.0# Or x >= shape_param + nc_param) Then
            comp_cdf_gamma_nc = comp_gamma_nc1(x, shape_param, nc_param, nc_derivative)
        Else
            comp_cdf_gamma_nc = 1.0# - gamma_nc1(x, shape_param, nc_param, nc_derivative)
        End If
        comp_cdf_gamma_nc = GetRidOfMinusZeroes(comp_cdf_gamma_nc)
    End Function

    Public Function inv_gamma_nc(ByVal prob As Double, ByVal shape_param As Double, ByVal nc_param As Double) As Double
        If (shape_param < 0.0# Or nc_param < 0.0# Or nc_param > nc_limit Or prob < 0.0# Or prob >= 1.0#) Then
            inv_gamma_nc = ErrorValue
        ElseIf (prob = 0.0# Or shape_param = 0.0# And prob <= Math.Exp(-nc_param)) Then
            inv_gamma_nc = 0.0#
        Else
            inv_gamma_nc = inv_gamma_nc1(prob, shape_param, nc_param)
        End If
        inv_gamma_nc = GetRidOfMinusZeroes(inv_gamma_nc)
    End Function

    Public Function comp_inv_gamma_nc(ByVal prob As Double, ByVal shape_param As Double, ByVal nc_param As Double) As Double
        If (shape_param < 0.0# Or nc_param < 0.0# Or nc_param > nc_limit Or prob <= 0.0# Or prob > 1.0#) Then
            comp_inv_gamma_nc = ErrorValue
        ElseIf (prob = 1.0# Or shape_param = 0.0# And prob >= -expm1(-nc_param)) Then
            comp_inv_gamma_nc = 0.0#
        Else
            comp_inv_gamma_nc = comp_inv_gamma_nc1(prob, shape_param, nc_param)
        End If
        comp_inv_gamma_nc = GetRidOfMinusZeroes(comp_inv_gamma_nc)
    End Function

    Public Function ncp_gamma_nc(ByVal prob As Double, ByVal x As Double, ByVal shape_param As Double) As Double
        If (shape_param < 0.0# Or x < 0.0# Or prob <= 0.0# Or prob > 1.0#) Then
            ncp_gamma_nc = ErrorValue
        ElseIf (x = 0.0# And shape_param = 0.0#) Then
            ncp_gamma_nc = -Math.Log(prob)
        ElseIf (shape_param = 0.0# And prob = 1.0#) Then
            ncp_gamma_nc = 0.0#
        ElseIf (x = 0.0# Or prob = 1.0#) Then
            ncp_gamma_nc = ErrorValue
        Else
            ncp_gamma_nc = ncp_gamma_nc1(prob, x, shape_param)
        End If
        ncp_gamma_nc = GetRidOfMinusZeroes(ncp_gamma_nc)
    End Function

    Public Function comp_ncp_gamma_nc(ByVal prob As Double, ByVal x As Double, ByVal shape_param As Double) As Double
        If (shape_param < 0.0# Or x < 0.0# Or prob < 0.0# Or prob >= 1.0#) Then
            comp_ncp_gamma_nc = ErrorValue
        ElseIf (x = 0.0# And shape_param = 0.0#) Then
            comp_ncp_gamma_nc = -log0(-prob)
        ElseIf (shape_param = 0.0# And prob = 0.0#) Then
            comp_ncp_gamma_nc = 0.0#
        ElseIf (x = 0.0# Or prob = 0.0#) Then
            comp_ncp_gamma_nc = ErrorValue
        Else
            comp_ncp_gamma_nc = comp_ncp_gamma_nc1(prob, x, shape_param)
        End If
        comp_ncp_gamma_nc = GetRidOfMinusZeroes(comp_ncp_gamma_nc)
    End Function

    Public Function pdf_Chi2_nc(ByVal x As Double, ByVal df As Double, ByVal nc As Double) As Double
        '// Calculate pdf of noncentral chi-square
        df = AlterForIntegralChecks_df(df)
        pdf_Chi2_nc = 0.5 * pdf_gamma_nc(x / 2.0#, df / 2.0#, nc / 2.0#)
        pdf_Chi2_nc = GetRidOfMinusZeroes(pdf_Chi2_nc)
    End Function

    Public Function cdf_Chi2_nc(ByVal x As Double, ByVal df As Double, ByVal nc As Double) As Double
        '// Calculate cdf of noncentral chi-square
        '//   parametrized per Johnson & Kotz, SAS, etc. so that cdf_Chi2_nc(x,df,nc) = cdf_gamma_nc(x/2,df/2,nc/2)
        '//   If Xi ~ N(Di,1) independent, then sum(Xi,i=1..n) ~ Chi2_nc(n,nc) with nc=sum(Di,i=1..n)
        '//   Note that Knusel, Graybill, etc. use a different noncentrality parameter lambda=nc/2
        df = AlterForIntegralChecks_df(df)
        cdf_Chi2_nc = cdf_gamma_nc(x / 2.0#, df / 2.0#, nc / 2.0#)
        cdf_Chi2_nc = GetRidOfMinusZeroes(cdf_Chi2_nc)
    End Function

    Public Function comp_cdf_Chi2_nc(ByVal x As Double, ByVal df As Double, ByVal nc As Double) As Double
        '// Calculate 1-cdf of noncentral chi-square
        '//   parametrized per Johnson & Kotz, SAS, etc. so that cdf_Chi2_nc(x,df,nc) = cdf_gamma_nc(x/2,df/2,nc/2)
        '//   If Xi ~ N(Di,1) independent, then sum(Xi,i=1..n) ~ Chi2_nc(n,nc) with nc=sum(Di,i=1..n)
        '//   Note that Knusel, Graybill, etc. use a different noncentrality parameter lambda=nc/2
        df = AlterForIntegralChecks_df(df)
        comp_cdf_Chi2_nc = comp_cdf_gamma_nc(x / 2.0#, df / 2.0#, nc / 2.0#)
        comp_cdf_Chi2_nc = GetRidOfMinusZeroes(comp_cdf_Chi2_nc)
    End Function

    Public Function inv_Chi2_nc(ByVal prob As Double, ByVal df As Double, ByVal nc As Double) As Double
        df = AlterForIntegralChecks_df(df)
        inv_Chi2_nc = 2.0# * inv_gamma_nc(prob, df / 2.0#, nc / 2.0#)
        inv_Chi2_nc = GetRidOfMinusZeroes(inv_Chi2_nc)
    End Function

    Public Function comp_inv_Chi2_nc(ByVal prob As Double, ByVal df As Double, ByVal nc As Double) As Double
        df = AlterForIntegralChecks_df(df)
        comp_inv_Chi2_nc = 2.0# * comp_inv_gamma_nc(prob, df / 2.0#, nc / 2.0#)
        comp_inv_Chi2_nc = GetRidOfMinusZeroes(comp_inv_Chi2_nc)
    End Function

    Public Function ncp_Chi2_nc(ByVal prob As Double, ByVal x As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        ncp_Chi2_nc = 2.0# * ncp_gamma_nc(prob, x / 2.0#, df / 2.0#)
        ncp_Chi2_nc = GetRidOfMinusZeroes(ncp_Chi2_nc)
    End Function

    Public Function comp_ncp_Chi2_nc(ByVal prob As Double, ByVal x As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        comp_ncp_Chi2_nc = 2.0# * comp_ncp_gamma_nc(prob, x / 2.0#, df / 2.0#)
        comp_ncp_Chi2_nc = GetRidOfMinusZeroes(comp_ncp_Chi2_nc)
    End Function

    Private Function BETA_nc1(ByVal x As Double, ByVal y As Double, ByVal a As Double, ByVal B As Double, ByVal nc As Double, ByRef nc_derivative As Double) As Double
        'y is 1-x but held accurately to avoid possible cancellation errors
        Dim aa As Double, bb As Double, nc_dtemp As Double
        Dim N As Double, p As Double, W As Double, S As Double, ps As Double
        Dim Result As Double, term As Double, ptx As Double, ptnc As Double
        aa = a - nc * x * (a + B)
        bb = (x * nc - 1.0#) - a
        If (bb < 0.0#) Then
            N = bb - Math.Sqrt(bb ^ 2 - 4.0# * aa)
            N = Int(2.0# * aa / N)
        Else
            N = Int((bb + Math.Sqrt(bb ^ 2 - 4.0# * aa)) / 2.0#)
        End If
        If N < 0.0# Then
            N = 0.0#
        End If
        aa = N + a
        bb = N
        ptnc = poissonTerm(N, nc, nc - N, 0.0#)
        ptx = B * binomialTerm(aa, B, x, y, B * x - aa * y, 0.0#)  '  (aa + b)*(I(x, aa, b) - I(x, aa + 1, b))
        aa = aa + 1.0#
        bb = bb + 1.0#
        p = nc / bb
        ps = p
        nc_derivative = ps
        S = x / aa  ' (I(x, aa, b) - I(x, aa + 1, b)) / ptx
        W = p
        term = S * W
        Result = term
        If ptx > 0 Then
            While (((term > 0.000000000000001 * Result) And (p > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_derivative))
                S = (aa + B) * S
                aa = aa + 1.0#
                bb = bb + 1.0#
                p = nc / bb * p
                ps = p * S
                nc_derivative = nc_derivative + ps
                S = x / aa * S ' (I(x, aa, b) - I(x, aa + 1, b)) / ptx
                W = W + p
                term = S * W
                Result = Result + term
            End While
            W = W * ptnc
        Else
            W = comppoisson(N, nc, nc - N)
        End If
        If x > y Then
            S = compbeta(y, B, a + (bb + 1.0#))
        Else
            S = BETA(x, a + (bb + 1.0#), B)
        End If
        BETA_nc1 = Result * ptx * ptnc + S * W
        ps = 1.0#
        nc_dtemp = 0.0#
        aa = N + a
        bb = N
        p = 1.0#
        S = ptx / (aa + B) ' I(x, aa, b) - I(x, aa + 1, b)
        If x > y Then
            W = compbeta(y, B, aa) ' I(x, aa, b)
        Else
            W = BETA(x, aa, B) ' I(x, aa, b)
        End If
        term = p * W
        Result = term
        While bb > 0.0# And (((term > 0.000000000000001 * Result) And (S > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_dtemp))
            S = aa / x * S
            ps = p * S
            nc_dtemp = nc_dtemp + ps
            p = bb / nc * p
            aa = aa - 1.0#
            bb = bb - 1.0#
            If bb = 0.0# Then aa = a
            S = S / (aa + B) ' I(x, aa, b) - I(x, aa + 1, b)
            W = W + S ' I(x, aa, b)
            term = p * W
            Result = Result + term
        End While
        If N > 0.0# Then
            nc_dtemp = nc_derivative * ptx + nc_dtemp + p * aa / x * S
        ElseIf B = 0.0# Then
            nc_dtemp = 0.0#
        Else
            nc_dtemp = binomialTerm(aa, B, x, y, B * x - aa * y, Math.Log(B) + Math.Log((nc_derivative + aa / (x * (aa + B)))))
        End If
        nc_dtemp = nc_dtemp / y
        BETA_nc1 = BETA_nc1 + Result * ptnc + cpoisson(bb - 1.0#, nc, nc - bb + 1.0#) * W
        If nc_dtemp = 0.0# Then
            nc_derivative = 0.0#
        Else
            nc_derivative = poissonTerm(N, nc, nc - N, Math.Log(nc_dtemp))
        End If
    End Function

    Private Function comp_BETA_nc1(ByVal x As Double, ByVal y As Double, ByVal a As Double, ByVal B As Double, ByVal nc As Double, ByRef nc_derivative As Double) As Double
        'y is 1-x but held accurately to avoid possible cancellation errors
        Dim aa As Double, bb As Double, nc_dtemp As Double
        Dim N As Double, p As Double, W As Double, S As Double, ps As Double
        Dim Result As Double, term As Double, ptx As Double, ptnc As Double
        aa = a - nc * x * (a + B)
        bb = (x * nc - 1.0#) - a
        If (bb < 0.0#) Then
            N = bb - Math.Sqrt(bb ^ 2 - 4.0# * aa)
            N = Int(2.0# * aa / N)
        Else
            N = Int((bb + Math.Sqrt(bb ^ 2 - 4.0# * aa)) / 2)
        End If
        If N < 0.0# Then
            N = 0.0#
        End If
        aa = N + a
        bb = N
        ptnc = poissonTerm(N, nc, nc - N, 0.0#)
        ptx = B / (aa + B) * binomialTerm(aa, B, x, y, B * x - aa * y, 0.0#) '(1 - I(x, aa + 1, b)) - (1 - I(x, aa, b))
        ps = 1.0#
        nc_dtemp = 0.0#
        p = 1.0#
        S = 1.0#
        W = p
        term = 1.0#
        Result = 0.0#
        If ptx > 0 Then
            While bb > 0.0# And (((term > 0.000000000000001 * Result) And (p > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_dtemp))
                S = aa / x * S
                ps = p * S
                nc_dtemp = nc_dtemp + ps
                p = bb / nc * p
                aa = aa - 1.0#
                bb = bb - 1.0#
                If bb = 0.0# Then aa = a
                S = S / (aa + B) ' (1 - I(x, aa + 1, b)) - (1 - I(x, aa + 1, b))
                term = S * W
                Result = Result + term
                W = W + p
            End While
            W = W * ptnc
        Else
            W = cpoisson(N, nc, nc - N)
        End If
        If N > 0.0# Then
            nc_dtemp = (nc_dtemp + p * aa / x * S) * ptx
        ElseIf a = 0.0# Or B = 0.0# Then
            nc_dtemp = 0.0#
        Else
            nc_dtemp = binomialTerm(aa, B, x, y, B * x - aa * y, Math.Log(B) + Math.Log(aa / (x * (aa + B))))
        End If
        If x > y Then
            S = BETA(y, B, aa)
        Else
            S = compbeta(x, aa, B)
        End If
        comp_BETA_nc1 = Result * ptx * ptnc + S * W
        aa = N + a
        bb = N
        p = 1.0#
        nc_derivative = 0.0#
        S = ptx
        If x > y Then
            W = BETA(y, B, aa) '  1 - I(x, aa, b)
        Else
            W = compbeta(x, aa, B) ' 1 - I(x, aa, b)
        End If
        term = 0.0#
        Result = term
        Do
            W = W + S ' 1 - I(x, aa, b)
            S = (aa + B) * S
            aa = aa + 1.0#
            bb = bb + 1.0#
            p = nc / bb * p
            ps = p * S
            nc_derivative = nc_derivative + ps
            S = x / aa * S ' (1 - I(x, aa + 1, b)) - (1 - I(x, aa, b))
            term = p * W
            Result = Result + term
        Loop While (((term > 0.000000000000001 * Result) And (S > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_derivative))
        nc_dtemp = (nc_derivative + nc_dtemp) / y
        comp_BETA_nc1 = comp_BETA_nc1 + Result * ptnc + comppoisson(bb, nc, nc - bb) * W
        If nc_dtemp = 0.0# Then
            nc_derivative = 0.0#
        Else
            nc_derivative = poissonTerm(N, nc, nc - N, Math.Log(nc_dtemp))
        End If
    End Function

    Private Function inv_BETA_nc1(ByVal prob As Double, ByVal a As Double, ByVal B As Double, ByVal nc As Double, ByRef oneMinusP As Double) As Double
        'Uses approx in A&S 26.6.26 for to get initial estimate the modified NR to improve it.
        Dim x As Double, y As Double, pr As Double, dif As Double, temp As Double
        Dim hip As Double, lop As Double
        Dim hix As Double, lox As Double, nc_derivative As Double
        If (prob > 0.5) Then
            inv_BETA_nc1 = comp_inv_BETA_nc1(1.0# - prob, a, B, nc, oneMinusP)
            Exit Function
        End If

        lop = 0.0#
        hip = 1.0#
        lox = 0.0#
        hix = 1.0#
        pr = Math.Exp(-nc)
        If pr > prob Then
            If 2.0# * prob > pr Then
                x = invcompbeta(a + cSmall, B, (pr - prob) / pr, oneMinusP)
            Else
                x = invbeta(a + cSmall, B, prob / pr, oneMinusP)
            End If
            If x = 0.0# Then
                inv_BETA_nc1 = 0.0#
                Exit Function
            Else
                temp = oneMinusP
                y = invbeta((a + nc) ^ 2 / (a + 2.0# * nc), B, prob, oneMinusP)
                oneMinusP = (a + nc) * oneMinusP / (a + nc * (1.0# + y))
                If temp > oneMinusP Then
                    oneMinusP = temp
                Else
                    x = (a + 2.0# * nc) * y / (a + nc * (1.0# + y))
                End If
            End If
        Else
            y = invbeta((a + nc) ^ 2 / (a + 2.0# * nc), B, prob, oneMinusP)
            x = (a + 2.0# * nc) * y / (a + nc * (1.0# + y))
            oneMinusP = (a + nc) * oneMinusP / (a + nc * (1.0# + y))
            If oneMinusP < cSmall Then
                oneMinusP = cSmall
                pr = BETA_nc1(x, oneMinusP, a, B, nc, nc_derivative)
                If pr < prob Then
                    inv_BETA_nc1 = 1.0#
                    oneMinusP = 0.0#
                    Exit Function
                End If
            End If
        End If
        Do
            pr = BETA_nc1(x, oneMinusP, a, B, nc, nc_derivative)
            If pr < 3.0E-308 And nc_derivative = 0.0# Then
                hip = oneMinusP
                lox = x
                dif = dif / 2.0#
                x = x - dif
                oneMinusP = oneMinusP + dif
            ElseIf nc_derivative = 0.0# Then
                lop = oneMinusP
                hix = x
                dif = dif / 2.0#
                x = x - dif
                oneMinusP = oneMinusP + dif
            Else
                If pr < prob Then
                    hip = oneMinusP
                    lox = x
                Else
                    lop = oneMinusP
                    hix = x
                End If
                dif = -(pr / nc_derivative) * logdif(pr, prob)
                If x > oneMinusP Then
                    If oneMinusP - dif < lop Then
                        dif = (oneMinusP - lop) * 0.9
                    ElseIf oneMinusP - dif > hip Then
                        dif = (oneMinusP - hip) * 0.9
                    End If
                ElseIf x + dif < lox Then
                    dif = (lox - x) * 0.9
                ElseIf x + dif > hix Then
                    dif = (hix - x) * 0.9
                End If
                x = x + dif
                oneMinusP = oneMinusP - dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(Min(x, oneMinusP)) * 0.0000000001))
        inv_BETA_nc1 = x
    End Function

    Private Function comp_inv_BETA_nc1(ByVal prob As Double, ByVal a As Double, ByVal B As Double, ByVal nc As Double, ByRef oneMinusP As Double) As Double
        'Uses approx in A&S 26.6.26 for to get initial estimate the modified NR to improve it.
        Dim x As Double, y As Double, pr As Double, dif As Double, temp As Double
        Dim hip As Double, lop As Double
        Dim hix As Double, lox As Double, nc_derivative As Double
        If (prob > 0.5) Then
            comp_inv_BETA_nc1 = inv_BETA_nc1(1.0# - prob, a, B, nc, oneMinusP)
            Exit Function
        End If

        lop = 0.0#
        hip = 1.0#
        lox = 0.0#
        hix = 1.0#
        pr = Math.Exp(-nc)
        If pr > prob Then
            If 2.0# * prob > pr Then
                x = invbeta(a + cSmall, B, (pr - prob) / pr, oneMinusP)
            Else
                x = invcompbeta(a + cSmall, B, prob / pr, oneMinusP)
            End If
            If oneMinusP < cSmall Then
                oneMinusP = cSmall
                pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, nc_derivative)
                If pr > prob Then
                    comp_inv_BETA_nc1 = 1.0#
                    oneMinusP = 0.0#
                    Exit Function
                End If
            Else
                temp = oneMinusP
                y = invcompbeta((a + nc) ^ 2 / (a + 2.0# * nc), B, prob, oneMinusP)
                oneMinusP = (a + nc) * oneMinusP / (a + nc * (1.0# + y))
                If temp < oneMinusP Then
                    oneMinusP = temp
                Else
                    x = (a + 2.0# * nc) * y / (a + nc * (1.0# + y))
                End If
                If oneMinusP < cSmall Then
                    oneMinusP = cSmall
                    pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, nc_derivative)
                    If pr > prob Then
                        comp_inv_BETA_nc1 = 1.0#
                        oneMinusP = 0.0#
                        Exit Function
                    End If
                ElseIf x < cSmall Then
                    x = cSmall
                    pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, nc_derivative)
                    If pr < prob Then
                        comp_inv_BETA_nc1 = 0.0#
                        oneMinusP = 1.0#
                        Exit Function
                    End If
                End If
            End If
        Else
            y = invcompbeta((a + nc) ^ 2 / (a + 2.0# * nc), B, prob, oneMinusP)
            x = (a + 2.0# * nc) * y / (a + nc * (1.0# + y))
            oneMinusP = (a + nc) * oneMinusP / (a + nc * (1.0# + y))
            If oneMinusP < cSmall Then
                oneMinusP = cSmall
                pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, nc_derivative)
                If pr > prob Then
                    comp_inv_BETA_nc1 = 1.0#
                    oneMinusP = 0.0#
                    Exit Function
                End If
            ElseIf x < cSmall Then
                x = cSmall
                pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, nc_derivative)
                If pr < prob Then
                    comp_inv_BETA_nc1 = 0.0#
                    oneMinusP = 1.0#
                    Exit Function
                End If
            End If
        End If
        dif = x
        Do
            pr = comp_BETA_nc1(x, oneMinusP, a, B, nc, nc_derivative)
            If pr < 3.0E-308 And nc_derivative = 0.0# Then
                lop = oneMinusP
                hix = x
                dif = dif / 2.0#
                x = x - dif
                oneMinusP = oneMinusP + dif
            ElseIf nc_derivative = 0.0# Then
                hip = oneMinusP
                lox = x
                dif = dif / 2.0#
                x = x - dif
                oneMinusP = oneMinusP + dif
            Else
                If pr < prob Then
                    lop = oneMinusP
                    hix = x
                Else
                    hip = oneMinusP
                    lox = x
                End If
                dif = (pr / nc_derivative) * logdif(pr, prob)
                If x > oneMinusP Then
                    If oneMinusP - dif < lop Then
                        dif = (oneMinusP - lop) * 0.9
                    ElseIf oneMinusP - dif > hip Then
                        dif = (oneMinusP - hip) * 0.9
                    End If
                ElseIf x + dif < lox Then
                    dif = (lox - x) * 0.9
                ElseIf x + dif > hix Then
                    dif = (hix - x) * 0.9
                End If
                x = x + dif
                oneMinusP = oneMinusP - dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(Min(x, oneMinusP)) * 0.0000000001))
        comp_inv_BETA_nc1 = x
    End Function

    Private Function invBetaLessThanX(ByVal prob As Double, ByVal x As Double, ByVal y As Double, ByVal a As Double, ByVal B As Double) As Double
        Dim oneMinusP As Double
        If x >= y Then
            If invcompbeta(B, a, prob, oneMinusP) >= y * (1.0# - 0.000000000000001) Then
                invBetaLessThanX = 0.0#
            Else
                invBetaLessThanX = ErrorValue
            End If
        ElseIf invbeta(a, B, prob, oneMinusP) <= x * (1.0# + 0.000000000000001) Then
            invBetaLessThanX = 0.0#
        Else
            invBetaLessThanX = ErrorValue
        End If
    End Function

    Private Function compInvBetaLessThanX(ByVal prob As Double, ByVal x As Double, ByVal y As Double, ByVal a As Double, ByVal B As Double) As Double
        Dim oneMinusP As Double
        If x >= y Then
            If invbeta(B, a, prob, oneMinusP) >= y * (1.0# - 0.000000000000001) Then
                compInvBetaLessThanX = 0.0#
            Else
                compInvBetaLessThanX = ErrorValue
            End If
        ElseIf invcompbeta(a, B, prob, oneMinusP) <= x * (1.0# + 0.000000000000001) Then
            compInvBetaLessThanX = 0.0#
        Else
            compInvBetaLessThanX = ErrorValue
        End If
    End Function

    Private Function ncp_BETA_nc1(ByVal prob As Double, ByVal x As Double, ByVal y As Double, ByVal a As Double, ByVal B As Double) As Double
        'Uses Normal approx for difference of 2 a Negative Binomial and a poisson distributed variable to get initial estimate the modified NR to improve it.
        Dim ncp As Double, pr As Double, dif As Double, temp As Double, deriv As Double, C As Double, D As Double, E As Double, sqarg As Double, checked_nc_limit As Boolean, checked_0_limit As Boolean
        Dim hi As Double, lo As Double, nc_derivative As Double
        If (prob > 0.5) Then
            ncp_BETA_nc1 = comp_ncp_BETA_nc1(1.0# - prob, x, y, a, B)
            Exit Function
        End If

        lo = 0.0#
        hi = nc_limit
        checked_0_limit = False
        checked_nc_limit = False
        temp = inv_normal(prob) ^ 2
        C = B * x / y
        D = temp - 2.0# * (a - C)
        If D < 2 * nc_limit Then
            E = (C - a) ^ 2 - temp * C / y
            sqarg = D ^ 2 - 4 * E
            If sqarg < 0 Then
                ncp = D / 2
            Else
                ncp = (D + Math.Sqrt(sqarg)) / 2
            End If
        Else
            ncp = nc_limit
        End If
        ncp = Min(max(0.0#, ncp), nc_limit)
        If x > y Then
            pr = compbeta(y * (1 + ncp / (ncp + a)) / (1 + ncp / (ncp + a) * y), B, a + ncp ^ 2 / (2 * ncp + a))
        Else
            pr = BETA(x / (1 + ncp / (ncp + a) * y), a + ncp ^ 2 / (2 * ncp + a), B)
        End If
        'Debug.Print "ncp_BETA_nc1 ncp1 ", ncp, pr
        If ncp = 0.0# Then
            If pr < prob Then
                ncp_BETA_nc1 = invBetaLessThanX(prob, x, y, a, B)
                Exit Function
            Else
                checked_0_limit = True
            End If
        End If
        temp = Min(max(0.0#, invcompgamma(B * x, prob) / y - a), nc_limit)
        If temp = ncp Then
            C = pr
        ElseIf x > y Then
            C = compbeta(y * (1 + temp / (temp + a)) / (1 + temp / (temp + a) * y), B, a + temp ^ 2 / (2 * temp + a))
        Else
            C = BETA(x / (1 + temp / (temp + a) * y), a + temp ^ 2 / (2 * temp + a), B)
        End If
        'Debug.Print "ncp_BETA_nc1 ncp2 ", temp, c
        If temp = 0.0# Then
            If C < prob Then
                ncp_BETA_nc1 = invBetaLessThanX(prob, x, y, a, B)
                Exit Function
            Else
                checked_0_limit = True
            End If
        End If
        If pr * C = 0.0# Then
            ncp = Min(ncp, temp)
            pr = max(pr, C)
            If pr = 0.0# Then
                C = compbeta(y, B, a)
                If C < prob Then
                    ncp_BETA_nc1 = invBetaLessThanX(prob, x, y, a, B)
                    Exit Function
                Else
                    checked_0_limit = True
                End If
            End If
        ElseIf Math.Abs(Math.Log(pr / prob)) > Math.Abs(Math.Log(C / prob)) Then
            ncp = temp
            pr = C
        End If
        If ncp = 0.0# Then
            If B > 1 + 0.000001 Then
                deriv = BETA_nc1(x, y, a + 1.0#, B - 1.0#, ncp, nc_derivative)
                deriv = nc_derivative * y ^ 2 / (B - 1.0#)
            Else
                deriv = pr - BETA_nc1(x, y, a + 1.0#, B, ncp, nc_derivative)
            End If
            If deriv = 0.0# Then
                ncp = nc_limit
            Else
                ncp = (pr - prob) / deriv
                If ncp >= nc_limit Then
                    ncp = (pr / deriv) * logdif(pr, prob)
                End If
            End If
        Else
            If ncp = nc_limit Then
                If pr > prob Then
                    ncp_BETA_nc1 = ErrorValue
                    Exit Function
                Else
                    checked_nc_limit = True
                End If
            End If
            If pr > 0 Then
                temp = ncp * 0.999999 'Use numerical derivative on approximation since cheap compared to evaluating non-central BETA
                If x > y Then
                    C = compbeta(y * (1.0# + temp / (temp + a)) / (1 + temp / (temp + a) * y), B, a + temp ^ 2 / (2 * temp + a))
                Else
                    C = BETA(x / (1 + temp / (temp + a) * y), a + temp ^ 2 / (2 * temp + a), B)
                End If
                If pr <> C Then
                    dif = (0.000001 * ncp * pr / (pr - C)) * logdif(pr, prob)
                    If ncp - dif < 0.0# Then
                        ncp = ncp / 2.0#
                    ElseIf ncp - dif > nc_limit Then
                        ncp = (ncp + nc_limit) / 2.0#
                    Else
                        ncp = ncp - dif
                    End If
                End If
            Else
                ncp = ncp / 2.0#
            End If
        End If
        dif = ncp
        Do
            pr = BETA_nc1(x, y, a, B, ncp, nc_derivative)
            'Debug.Print ncp, pr, prob
            If B > 1 + 0.000001 Then
                deriv = BETA_nc1(x, y, a + 1.0#, B - 1.0#, ncp, nc_derivative)
                deriv = nc_derivative * y ^ 2 / (B - 1.0#)
            Else
                deriv = pr - BETA_nc1(x, y, a + 1.0#, B, ncp, nc_derivative)
            End If
            If pr < 3.0E-308 And deriv = 0.0# Then
                hi = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            ElseIf deriv = 0.0# Then
                lo = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            Else
                If pr < prob Then
                    hi = ncp
                Else
                    lo = ncp
                End If
                dif = (pr / deriv) * logdif(pr, prob)
                If ncp + dif < lo Then
                    dif = (lo - ncp) / 2.0#
                    If Not checked_0_limit And (lo = 0.0#) Then
                        temp = cdf_BETA_nc(x, a, B, lo)
                        If temp < prob Then
                            ncp_BETA_nc1 = invBetaLessThanX(prob, x, y, a, B)
                            Exit Function
                        Else
                            checked_0_limit = True
                        End If
                    End If
                ElseIf ncp + dif > hi Then
                    dif = (hi - ncp) / 2.0#
                    If Not checked_nc_limit And (hi = nc_limit) Then
                        temp = cdf_BETA_nc(x, a, B, hi)
                        If temp > prob Then
                            ncp_BETA_nc1 = ErrorValue
                            Exit Function
                        Else
                            checked_nc_limit = True
                        End If
                    End If
                End If
                ncp = ncp + dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(ncp) * 0.0000000001))
        ncp_BETA_nc1 = ncp
        'Debug.Print "ncp_BETA_nc1", ncp_BETA_nc1
    End Function

    Private Function comp_ncp_BETA_nc1(ByVal prob As Double, ByVal x As Double, ByVal y As Double, ByVal a As Double, ByVal B As Double) As Double
        'Uses Normal approx for difference of 2 a Negative Binomial and a poisson distributed variable to get initial estimate the modified NR to improve it.
        Dim ncp As Double, pr As Double, dif As Double, temp As Double, deriv As Double, C As Double, D As Double, E As Double, sqarg As Double, checked_nc_limit As Boolean, checked_0_limit As Boolean
        Dim hi As Double, lo As Double, nc_derivative As Double
        If (prob > 0.5) Then
            comp_ncp_BETA_nc1 = ncp_BETA_nc1(1.0# - prob, x, y, a, B)
            Exit Function
        End If

        lo = 0.0#
        hi = nc_limit
        checked_0_limit = False
        checked_nc_limit = False
        temp = inv_normal(prob) ^ 2
        C = B * x / y
        D = temp - 2.0# * (a - C)
        If D < 4 * nc_limit Then
            sqarg = D ^ 2 - 4 * E
            If sqarg < 0 Then
                ncp = D / 2
            Else
                ncp = (D - Math.Sqrt(sqarg)) / 2
            End If
        Else
            ncp = 0.0#
        End If
        ncp = Min(max(0.0#, ncp), nc_limit)
        If x > y Then
            pr = BETA(y * (1 + ncp / (ncp + a)) / (1 + ncp / (ncp + a) * y), B, a + ncp ^ 2 / (2 * ncp + a))
        Else
            pr = compbeta(x / (1 + ncp / (ncp + a) * y), a + ncp ^ 2 / (2 * ncp + a), B)
        End If
        'Debug.Print "comp_ncp_BETA_nc1 ncp1 ", ncp, pr
        If ncp = 0.0# Then
            If pr > prob Then
                comp_ncp_BETA_nc1 = compInvBetaLessThanX(prob, x, y, a, B)
                Exit Function
            Else
                checked_0_limit = True
            End If
        End If
        temp = Min(max(0.0#, invgamma(B * x, prob) / y - a), nc_limit)
        If temp = ncp Then
            C = pr
        ElseIf x > y Then
            C = BETA(y * (1 + temp / (temp + a)) / (1 + temp / (temp + a) * y), B, a + temp ^ 2 / (2 * temp + a))
        Else
            C = compbeta(x / (1 + temp / (temp + a) * y), a + temp ^ 2 / (2 * temp + a), B)
        End If
        'Debug.Print "comp_ncp_BETA_nc1 ncp2 ", temp, c
        If temp = 0.0# Then
            If C > prob Then
                comp_ncp_BETA_nc1 = compInvBetaLessThanX(prob, x, y, a, B)
                Exit Function
            Else
                checked_0_limit = True
            End If
        End If
        If pr * C = 0.0# Then
            ncp = max(ncp, temp)
            pr = max(pr, C)
        ElseIf Math.Abs(Math.Log(pr / prob)) > Math.Abs(Math.Log(C / prob)) Then
            ncp = temp
            pr = C
        End If
        If ncp = 0.0# Then
            If pr > prob Then
                comp_ncp_BETA_nc1 = compInvBetaLessThanX(prob, x, y, a, B)
                Exit Function
            Else
                If B > 1 + 0.000001 Then
                    deriv = BETA_nc1(x, y, a + 1.0#, B - 1.0#, 0.0#, nc_derivative)
                    deriv = nc_derivative * y ^ 2 / (B - 1.0#)
                Else
                    deriv = comp_BETA_nc1(x, y, a + 1.0#, B, 0.0#, nc_derivative) - pr
                End If
                If deriv = 0.0# Then
                    ncp = nc_limit
                Else
                    ncp = (prob - pr) / deriv
                    If ncp >= nc_limit Then
                        ncp = -(pr / deriv) * logdif(pr, prob)
                    End If
                End If
                checked_0_limit = True
            End If
        Else
            If ncp = nc_limit Then
                If pr < prob Then
                    comp_ncp_BETA_nc1 = ErrorValue
                    Exit Function
                Else
                    checked_nc_limit = True
                End If
            End If
            If pr > 0 Then
                temp = ncp * 0.999999 'Use numerical derivative on approximation since cheap compared to evaluating non-central BETA
                If x > y Then
                    C = BETA(y * (1.0# + temp / (temp + a)) / (1 + temp / (temp + a) * y), B, a + temp ^ 2 / (2 * temp + a))
                Else
                    C = compbeta(x / (1 + temp / (temp + a) * y), a + temp ^ 2 / (2 * temp + a), B)
                End If
                If pr <> C Then
                    dif = -(0.000001 * ncp * pr / (pr - C)) * logdif(pr, prob)
                    If ncp + dif < 0 Then
                        ncp = ncp / 2
                    ElseIf ncp + dif > nc_limit Then
                        ncp = (ncp + nc_limit) / 2
                    Else
                        ncp = ncp + dif
                    End If
                End If
            Else
                ncp = (nc_limit + ncp) / 2.0#
            End If
        End If
        dif = ncp
        Do
            pr = comp_BETA_nc1(x, y, a, B, ncp, nc_derivative)
            'Debug.Print ncp, pr, prob
            If B > 1 + 0.000001 Then
                deriv = BETA_nc1(x, y, a + 1.0#, B - 1.0#, ncp, nc_derivative)
                deriv = nc_derivative * y ^ 2 / (B - 1.0#)
            Else
                deriv = comp_BETA_nc1(x, y, a + 1.0#, B, ncp, nc_derivative) - pr
            End If
            If pr < 3.0E-308 And deriv = 0.0# Then
                lo = ncp
                dif = dif / 2.0#
                ncp = ncp + dif
            ElseIf deriv = 0.0# Then
                hi = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            Else
                If pr < prob Then
                    lo = ncp
                Else
                    hi = ncp
                End If
                dif = -(pr / deriv) * logdif(pr, prob)
                If ncp + dif < lo Then
                    dif = (lo - ncp) / 2.0#
                    If Not checked_0_limit And (lo = 0.0#) Then
                        temp = comp_cdf_BETA_nc(x, a, B, lo)
                        If temp > prob Then
                            comp_ncp_BETA_nc1 = compInvBetaLessThanX(prob, x, y, a, B)
                            Exit Function
                        Else
                            checked_0_limit = True
                        End If
                    End If
                ElseIf ncp + dif > hi Then
                    dif = (hi - ncp) / 2.0#
                    If Not checked_nc_limit And (hi = nc_limit) Then
                        temp = comp_cdf_BETA_nc(x, a, B, hi)
                        If temp < prob Then
                            comp_ncp_BETA_nc1 = ErrorValue
                            Exit Function
                        Else
                            checked_nc_limit = True
                        End If
                    End If
                End If
                ncp = ncp + dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(ncp) * 0.0000000001))
        comp_ncp_BETA_nc1 = ncp
        'Debug.Print "comp_ncp_BETA_nc1", comp_ncp_BETA_nc1
    End Function

    Public Function pdf_BETA_nc(ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, ByVal nc_param As Double) As Double
        If (shape_param1 < 0.0#) Or (shape_param2 < 0.0#) Or (nc_param < 0.0#) Or (nc_param > nc_limit) Or ((shape_param1 = 0.0#) And (shape_param2 = 0.0#)) Then
            pdf_BETA_nc = ErrorValue
        ElseIf (x < 0.0# Or x > 1.0#) Then
            pdf_BETA_nc = 0.0#
        ElseIf (x = 0.0# Or nc_param = 0.0#) Then
            pdf_BETA_nc = Math.Exp(-nc_param) * pdf_BETA(x, shape_param1, shape_param2)
        ElseIf (x = 1.0# And shape_param2 = 1.0#) Then
            pdf_BETA_nc = shape_param1 + nc_param
        ElseIf (x = 1.0#) Then
            pdf_BETA_nc = pdf_BETA(x, shape_param1, shape_param2)
        Else
            Dim nc_derivative As Double
            If (shape_param1 < 1.0# Or x * shape_param2 <= (1.0# - x) * (shape_param1 + nc_param)) Then
                pdf_BETA_nc = BETA_nc1(x, 1.0# - x, shape_param1, shape_param2, nc_param, nc_derivative)
            Else
                pdf_BETA_nc = comp_BETA_nc1(x, 1.0# - x, shape_param1, shape_param2, nc_param, nc_derivative)
            End If
            pdf_BETA_nc = nc_derivative
        End If
        pdf_BETA_nc = GetRidOfMinusZeroes(pdf_BETA_nc)
    End Function

    Public Function cdf_BETA_nc(ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, ByVal nc_param As Double) As Double
        Dim nc_derivative As Double
        If (shape_param1 < 0.0#) Or (shape_param2 < 0.0#) Or (nc_param < 0.0#) Or (nc_param > nc_limit) Or ((shape_param1 = 0.0#) And (shape_param2 = 0.0#)) Then
            cdf_BETA_nc = ErrorValue
        ElseIf (x < 0.0#) Then
            cdf_BETA_nc = 0.0#
        ElseIf (x >= 1.0#) Then
            cdf_BETA_nc = 1.0#
        ElseIf (x = 0.0# And shape_param1 = 0.0#) Then
            cdf_BETA_nc = Math.Exp(-nc_param)
        ElseIf (x = 0.0#) Then
            cdf_BETA_nc = 0.0#
        ElseIf (nc_param = 0.0#) Then
            cdf_BETA_nc = BETA(x, shape_param1, shape_param2)
        ElseIf (shape_param1 < 1.0# Or x * shape_param2 <= (1.0# - x) * (shape_param1 + nc_param)) Then
            cdf_BETA_nc = BETA_nc1(x, 1.0# - x, shape_param1, shape_param2, nc_param, nc_derivative)
        Else
            cdf_BETA_nc = 1.0# - comp_BETA_nc1(x, 1.0# - x, shape_param1, shape_param2, nc_param, nc_derivative)
        End If
        cdf_BETA_nc = GetRidOfMinusZeroes(cdf_BETA_nc)
    End Function

    Public Function comp_cdf_BETA_nc(ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, ByVal nc_param As Double) As Double
        Dim nc_derivative As Double
        If (shape_param1 < 0.0#) Or (shape_param2 < 0.0#) Or (nc_param < 0.0#) Or (nc_param > nc_limit) Or ((shape_param1 = 0.0#) And (shape_param2 = 0.0#)) Then
            comp_cdf_BETA_nc = ErrorValue
        ElseIf (x < 0.0#) Then
            comp_cdf_BETA_nc = 1.0#
        ElseIf (x >= 1.0#) Then
            comp_cdf_BETA_nc = 0.0#
        ElseIf (x = 0.0# And shape_param1 = 0.0#) Then
            comp_cdf_BETA_nc = -expm1(-nc_param)
        ElseIf (x = 0.0#) Then
            comp_cdf_BETA_nc = 1.0#
        ElseIf (nc_param = 0.0#) Then
            comp_cdf_BETA_nc = compbeta(x, shape_param1, shape_param2)
        ElseIf (shape_param1 < 1.0# Or x * shape_param2 >= (1.0# - x) * (shape_param1 + nc_param)) Then
            comp_cdf_BETA_nc = comp_BETA_nc1(x, 1.0# - x, shape_param1, shape_param2, nc_param, nc_derivative)
        Else
            comp_cdf_BETA_nc = 1.0# - BETA_nc1(x, 1.0# - x, shape_param1, shape_param2, nc_param, nc_derivative)
        End If
        comp_cdf_BETA_nc = GetRidOfMinusZeroes(comp_cdf_BETA_nc)
    End Function

    Public Function inv_BETA_nc(ByVal prob As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, ByVal nc_param As Double) As Double
        Dim oneMinusP As Double
        If (shape_param1 < 0.0#) Or (shape_param2 <= 0.0#) Or (nc_param < 0.0#) Or (nc_param > nc_limit) Or (prob < 0.0#) Or (prob > 1.0#) Then
            inv_BETA_nc = ErrorValue
        ElseIf (prob = 0.0# Or shape_param1 = 0.0# And prob <= Math.Exp(-nc_param)) Then
            inv_BETA_nc = 0.0#
        ElseIf (prob = 1.0#) Then
            inv_BETA_nc = 1.0#
        ElseIf (nc_param = 0.0#) Then
            inv_BETA_nc = invbeta(shape_param1, shape_param2, prob, oneMinusP)
        Else
            inv_BETA_nc = inv_BETA_nc1(prob, shape_param1, shape_param2, nc_param, oneMinusP)
        End If
        inv_BETA_nc = GetRidOfMinusZeroes(inv_BETA_nc)
    End Function

    Public Function comp_inv_BETA_nc(ByVal prob As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, ByVal nc_param As Double) As Double
        Dim oneMinusP As Double
        If (shape_param1 < 0.0#) Or (shape_param2 <= 0.0#) Or (nc_param < 0.0#) Or (nc_param > nc_limit) Or (prob < 0.0#) Or (prob > 1.0#) Then
            comp_inv_BETA_nc = ErrorValue
        ElseIf (prob = 1.0# Or shape_param1 = 0.0# And prob >= -expm1(-nc_param)) Then
            comp_inv_BETA_nc = 0.0#
        ElseIf (prob = 0.0#) Then
            comp_inv_BETA_nc = 1.0#
        ElseIf (nc_param = 0.0#) Then
            comp_inv_BETA_nc = invcompbeta(shape_param1, shape_param2, prob, oneMinusP)
        Else
            comp_inv_BETA_nc = comp_inv_BETA_nc1(prob, shape_param1, shape_param2, nc_param, oneMinusP)
        End If
        comp_inv_BETA_nc = GetRidOfMinusZeroes(comp_inv_BETA_nc)
    End Function

    Public Function ncp_BETA_nc(ByVal prob As Double, ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double) As Double
        If (shape_param1 < 0.0#) Or (shape_param2 <= 0.0#) Or (x < 0.0#) Or (x >= 1.0#) Or (prob <= 0.0#) Or (prob > 1.0#) Then
            ncp_BETA_nc = ErrorValue
        ElseIf (x = 0.0# And shape_param1 = 0.0#) Then
            ncp_BETA_nc = -Math.Log(prob)
        ElseIf (x = 0.0# Or prob = 1.0#) Then
            ncp_BETA_nc = ErrorValue
        Else
            ncp_BETA_nc = ncp_BETA_nc1(prob, x, 1.0# - x, shape_param1, shape_param2)
        End If
        ncp_BETA_nc = GetRidOfMinusZeroes(ncp_BETA_nc)
    End Function

    Public Function comp_ncp_BETA_nc(ByVal prob As Double, ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double) As Double
        If (shape_param1 < 0.0#) Or (shape_param2 <= 0.0#) Or (x < 0.0#) Or (x >= 1.0#) Or (prob < 0.0#) Or (prob >= 1.0#) Then
            comp_ncp_BETA_nc = ErrorValue
        ElseIf (x = 0.0# And shape_param1 = 0.0#) Then
            comp_ncp_BETA_nc = -log0(-prob)
        ElseIf (x = 0.0# Or prob = 0.0#) Then
            comp_ncp_BETA_nc = ErrorValue
        Else
            comp_ncp_BETA_nc = comp_ncp_BETA_nc1(prob, x, 1.0# - x, shape_param1, shape_param2)
        End If
        comp_ncp_BETA_nc = GetRidOfMinusZeroes(comp_ncp_BETA_nc)
    End Function

    Public Function pdf_fdist_nc(ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double, ByVal nc As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0# Or (nc < 0.0#) Or (nc > 2.0# * nc_limit)) Then
            pdf_fdist_nc = ErrorValue
        ElseIf (x < 0.0#) Then
            pdf_fdist_nc = 0.0#
        ElseIf (x = 0.0# Or nc = 0.0#) Then
            pdf_fdist_nc = Math.Exp(-nc / 2.0#) * pdf_fdist(x, df1, df2)
        Else
            Dim p As Double, Q As Double, nc_derivative As Double
            If x > 1.0# Then
                Q = df2 / x
                p = Q + df1
                Q = Q / p
                p = df1 / p
            Else
                p = df1 * x
                Q = df2 + p
                p = p / Q
                Q = df2 / Q
            End If
            If (df1 < 1.0# Or p * df2 <= Q * (df1 + nc)) Then
                pdf_fdist_nc = BETA_nc1(p, Q, df1 / 2.0#, df2 / 2.0#, nc / 2.0#, nc_derivative)
            Else
                pdf_fdist_nc = comp_BETA_nc1(p, Q, df1 / 2.0#, df2 / 2.0#, nc / 2.0#, nc_derivative)
            End If
            pdf_fdist_nc = (nc_derivative * Q) * (df1 * Q / df2)
        End If
        pdf_fdist_nc = GetRidOfMinusZeroes(pdf_fdist_nc)
    End Function

    Public Function cdf_fdist_nc(ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double, ByVal nc As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0# Or (nc < 0.0#) Or (nc > 2.0# * nc_limit)) Then
            cdf_fdist_nc = ErrorValue
        ElseIf (x <= 0.0#) Then
            cdf_fdist_nc = 0.0#
        Else
            Dim p As Double, Q As Double, nc_derivative As Double
            If x > 1.0# Then
                Q = df2 / x
                p = Q + df1
                Q = Q / p
                p = df1 / p
            Else
                p = df1 * x
                Q = df2 + p
                p = p / Q
                Q = df2 / Q
            End If
            'If p < cSmall And x <> 0# Or q < cSmall Then
            '   cdf_fdist_nc = ErrorValue
            '   Exit Function
            'End If
            df2 = df2 / 2.0#
            df1 = df1 / 2.0#
            nc = nc / 2.0#
            If (nc = 0.0# And p <= Q) Then
                cdf_fdist_nc = BETA(p, df1, df2)
            ElseIf (nc = 0.0#) Then
                cdf_fdist_nc = compbeta(Q, df2, df1)
            ElseIf (df1 < 1.0# Or p * df2 <= Q * (df1 + nc)) Then
                cdf_fdist_nc = BETA_nc1(p, Q, df1, df2, nc, nc_derivative)
            Else
                cdf_fdist_nc = 1.0# - comp_BETA_nc1(p, Q, df1, df2, nc, nc_derivative)
            End If
        End If
        cdf_fdist_nc = GetRidOfMinusZeroes(cdf_fdist_nc)
    End Function

    Public Function comp_cdf_fdist_nc(ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double, ByVal nc As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0# Or (nc < 0.0#) Or (nc > 2.0# * nc_limit)) Then
            comp_cdf_fdist_nc = ErrorValue
        ElseIf (x <= 0.0#) Then
            comp_cdf_fdist_nc = 1.0#
        Else
            Dim p As Double, Q As Double, nc_derivative As Double
            If x > 1.0# Then
                Q = df2 / x
                p = Q + df1
                Q = Q / p
                p = df1 / p
            Else
                p = df1 * x
                Q = df2 + p
                p = p / Q
                Q = df2 / Q
            End If
            'If p < cSmall And x <> 0# Or q < cSmall Then
            '   comp_cdf_fdist_nc = ErrorValue
            '   Exit Function
            'End If
            df2 = df2 / 2.0#
            df1 = df1 / 2.0#
            nc = nc / 2.0#
            If (nc = 0.0# And p <= Q) Then
                comp_cdf_fdist_nc = compbeta(p, df1, df2)
            ElseIf (nc = 0.0#) Then
                comp_cdf_fdist_nc = BETA(Q, df2, df1)
            ElseIf (df1 < 1.0# Or p * df2 >= Q * (df1 + nc)) Then
                comp_cdf_fdist_nc = comp_BETA_nc1(p, Q, df1, df2, nc, nc_derivative)
            Else
                comp_cdf_fdist_nc = 1.0# - BETA_nc1(p, Q, df1, df2, nc, nc_derivative)
            End If
        End If
        comp_cdf_fdist_nc = GetRidOfMinusZeroes(comp_cdf_fdist_nc)
    End Function

    Public Function inv_fdist_nc(ByVal prob As Double, ByVal df1 As Double, ByVal df2 As Double, ByVal nc As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0# Or (nc < 0.0#) Or (nc > 2.0# * nc_limit) Or prob < 0.0# Or prob >= 1.0#) Then
            inv_fdist_nc = ErrorValue
        ElseIf (prob = 0.0#) Then
            inv_fdist_nc = 0.0#
        Else
            Dim temp As Double, oneMinusP As Double
            df1 = df1 / 2.0#
            df2 = df2 / 2.0#
            If nc = 0.0# Then
                temp = invbeta(df1, df2, prob, oneMinusP)
            Else
                temp = inv_BETA_nc1(prob, df1, df2, nc / 2.0#, oneMinusP)
            End If
            inv_fdist_nc = df2 * temp / (df1 * oneMinusP)
            'If oneMinusP < cSmall Then inv_fdist_nc = ErrorValue
        End If
        inv_fdist_nc = GetRidOfMinusZeroes(inv_fdist_nc)
    End Function

    Public Function comp_inv_fdist_nc(ByVal prob As Double, ByVal df1 As Double, ByVal df2 As Double, ByVal nc As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0# Or df2 <= 0.0# Or (nc < 0.0#) Or (nc > 2.0# * nc_limit) Or prob <= 0.0# Or prob > 1.0#) Then
            comp_inv_fdist_nc = ErrorValue
        ElseIf (prob = 1.0#) Then
            comp_inv_fdist_nc = 0.0#
        Else
            Dim temp As Double, oneMinusP As Double
            df1 = df1 / 2.0#
            df2 = df2 / 2.0#
            If nc = 0.0# Then
                temp = invcompbeta(df1, df2, prob, oneMinusP)
            Else
                temp = comp_inv_BETA_nc1(prob, df1, df2, nc / 2.0#, oneMinusP)
            End If
            comp_inv_fdist_nc = df2 * temp / (df1 * oneMinusP)
            'If oneMinusP < cSmall Then comp_inv_fdist_nc = ErrorValue
        End If
        comp_inv_fdist_nc = GetRidOfMinusZeroes(comp_inv_fdist_nc)
    End Function

    Public Function ncp_fdist_nc(ByVal prob As Double, ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0#) Or (df2 <= 0.0#) Or (x <= 0.0#) Or (prob <= 0.0#) Or (prob >= 1.0#) Then
            ncp_fdist_nc = ErrorValue
        Else
            Dim p As Double, Q As Double
            If x > 1.0# Then
                Q = df2 / x
                p = Q + df1
                Q = Q / p
                p = df1 / p
            Else
                p = df1 * x
                Q = df2 + p
                p = p / Q
                Q = df2 / Q
            End If
            df2 = df2 / 2.0#
            df1 = df1 / 2.0#
            ncp_fdist_nc = ncp_BETA_nc1(prob, p, Q, df1, df2) * 2.0#
        End If
        ncp_fdist_nc = GetRidOfMinusZeroes(ncp_fdist_nc)
    End Function

    Public Function comp_ncp_fdist_nc(ByVal prob As Double, ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double) As Double
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        If (df1 <= 0.0#) Or (df2 <= 0.0#) Or (x <= 0.0#) Or (prob <= 0.0#) Or (prob >= 1.0#) Then
            comp_ncp_fdist_nc = ErrorValue
        Else
            Dim p As Double, Q As Double
            If x > 1.0# Then
                Q = df2 / x
                p = Q + df1
                Q = Q / p
                p = df1 / p
            Else
                p = df1 * x
                Q = df2 + p
                p = p / Q
                Q = df2 / Q
            End If
            df1 = df1 / 2.0#
            df2 = df2 / 2.0#
            comp_ncp_fdist_nc = comp_ncp_BETA_nc1(prob, p, Q, df1, df2) * 2.0#
        End If
        comp_ncp_fdist_nc = GetRidOfMinusZeroes(comp_ncp_fdist_nc)
    End Function

    Private Function t_nc1(ByVal T As Double, ByVal df As Double, ByVal nct As Double, ByRef nc_derivative As Double) As Double
        'y is 1-x but held accurately to avoid possible cancellation errors
        'nc_derivative holds t * derivative
        Dim aa As Double, bb As Double, nc_dtemp As Double
        Dim N As Double, p As Double, Q As Double, W As Double, V As Double, r As Double, S As Double, ps As Double
        Dim result1 As Double, result2 As Double, term1 As Double, term2 As Double, ptnc As Double, qtnc As Double, ptx As Double, qtx As Double
        Dim a As Double, B As Double, x As Double, y As Double, nc As Double
        Dim save_result1 As Double, save_result2 As Double, phi As Double, vScale As Double
        phi = cnormal(-Math.Abs(nct))
        a = 0.5
        B = df / 2.0#
        If Math.Abs(T) >= Min(1.0#, df) Then
            y = df / T
            x = T + y
            y = y / x
            x = T / x
        Else
            x = T * T
            y = df + x
            x = x / y
            y = df / y
        End If
        If y < cSmall Then
            t_nc1 = ErrorValue
            Exit Function
        End If
        nc = nct * nct / 2.0#
        aa = a - nc * x * (a + B)
        bb = (x * nc - 1.0#) - a
        If (bb < 0.0#) Then
            N = bb - Math.Sqrt(bb ^ 2 - 4.0# * aa)
            N = Int(2.0# * aa / N)
        Else
            N = Int((bb + Math.Sqrt(bb ^ 2 - 4.0# * aa)) / 2.0#)
        End If
        If N < 0.0# Then
            N = 0.0#
        End If
        aa = N + a
        bb = N + 0.5
        qtnc = poissonTerm(bb, nc, nc - bb, 0.0#)
        bb = N
        ptnc = poissonTerm(bb, nc, nc - bb, 0.0#)
        ptx = binomialTerm(aa, B, x, y, B * x - aa * y, 0.0#) / (aa + B) '(I(x, aa, b) - I(x, aa+1, b))/b
        qtx = binomialTerm(aa + 0.5, B, x, y, B * x - (aa + 0.5) * y, 0.0#) / (aa + B + 0.5) '(I(x, aa+1/2, b) - I(x, aa+3/2, b))/b
        If B > 1.0# Then
            ptx = B * ptx
            qtx = B * qtx
        End If
        vScale = max(ptx, qtx)
        If ptx = vScale Then
            S = 1.0#
        Else
            S = ptx / vScale
        End If
        If qtx = vScale Then
            r = 1.0#
        Else
            r = qtx / vScale
        End If
        S = (aa + B) * S
        r = (aa + B + 0.5) * r
        aa = aa + 1.0#
        bb = bb + 1.0#
        p = nc / bb * ptnc
        Q = nc / (bb + 0.5) * qtnc
        ps = p * S + Q * r
        nc_derivative = ps
        S = x / aa * S  ' I(x, aa, b) - I(x, aa+1, b)
        r = x / (aa + 0.5) * r ' I(x, aa+1/2, b) - I(x, aa+3/2, b)
        W = p
        V = Q
        term1 = S * W
        term2 = r * V
        result1 = term1
        result2 = term2
        While ((((term1 + term2) > 0.000000000000001 * (result1 + result2)) And (p > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_derivative))
            S = (aa + B) * S
            r = (aa + B + 0.5) * r
            aa = aa + 1.0#
            bb = bb + 1.0#
            p = nc / bb * p
            Q = nc / (bb + 0.5) * Q
            ps = p * S + Q * r
            nc_derivative = nc_derivative + ps
            S = x / aa * S ' I(x, aa, b) - I(x, aa+1, b)
            r = x / (aa + 0.5) * r ' I(x, aa+1/2, b) - I(x, aa+3/2, b)
            W = W + p
            V = V + Q
            term1 = S * W
            term2 = r * V
            result1 = result1 + term1
            result2 = result2 + term2
        End While
        If x > y Then
            S = compbeta(y, B, a + (bb + 1.0#))
            r = compbeta(y, B, a + (bb + 1.5))
        Else
            S = BETA(x, a + (bb + 1.0#), B)
            r = BETA(x, a + (bb + 1.5), B)
        End If
        nc_derivative = x * nc_derivative * vScale
        If B <= 1.0# Then vScale = vScale * B
        save_result1 = result1 * vScale + S * W
        save_result2 = result2 * vScale + r * V

        ps = 1.0#
        nc_dtemp = 0.0#
        aa = N + a
        bb = N
        vScale = max(ptnc, qtnc)
        If ptnc = vScale Then
            p = 1.0#
        Else
            p = ptnc / vScale
        End If
        If qtnc = vScale Then
            Q = 1.0#
        Else
            Q = qtnc / vScale
        End If
        S = ptx ' I(x, aa, b) - I(x, aa+1, b)
        r = qtx ' I(x, aa+1/2, b) - I(x, aa+3/2, b)
        If x > y Then
            W = compbeta(y, B, aa) ' I(x, aa, b)
            V = compbeta(y, B, aa + 0.5) ' I(x, aa+1/2, b)
        Else
            W = BETA(x, aa, B) ' I(x, aa, b)
            V = BETA(x, aa + 0.5, B) ' I(x, aa+1/2, b)
        End If
        term1 = p * W
        term2 = Q * V
        result1 = term1
        result2 = term2
        While bb > 0.0# And ((((term1 + term2) > 0.000000000000001 * (result1 + result2)) And (S > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_dtemp))
            S = aa / x * S
            r = (aa + 0.5) / x * r
            ps = p * S + Q * r
            nc_dtemp = nc_dtemp + ps
            p = bb / nc * p
            Q = (bb + 0.5) / nc * Q
            aa = aa - 1.0#
            bb = bb - 1.0#
            If bb = 0.0# Then aa = a
            S = S / (aa + B) ' I(x, aa, b) - I(x, aa+1, b)
            r = r / (aa + B + 0.5) ' I(x, aa+1/2, b) - I(x, aa+3/2, b)
            If B > 1.0# Then
                W = W + S ' I(x, aa, b)
                V = V + r ' I(x, aa+0.5, b)
            Else
                W = W + B * S
                V = V + B * r
            End If
            term1 = p * W
            term2 = Q * V
            result1 = result1 + term1
            result2 = result2 + term2
        End While
        nc_dtemp = x * nc_dtemp + p * aa * S + Q * (aa + 0.5) * r
        p = cpoisson(bb - 1.0#, nc, nc - bb + 1.0#)
        Q = cpoisson(bb - 0.5, nc, nc - bb + 0.5) - 2.0# * phi
        result1 = save_result1 + result1 * vScale + p * W
        result2 = save_result2 + result2 * vScale + Q * V
        If T > 0.0# Then
            t_nc1 = phi + 0.5 * (result1 + result2)
            nc_derivative = nc_derivative + nc_dtemp * vScale
        Else
            t_nc1 = phi - 0.5 * (result1 - result2)
        End If
    End Function

    Private Function comp_t_nc1(ByVal T As Double, ByVal df As Double, ByVal nct As Double, ByRef nc_derivative As Double) As Double
        'y is 1-x but held accurately to avoid possible cancellation errors
        'nc_derivative holds t * derivative
        Dim aa As Double, bb As Double, nc_dtemp As Double
        Dim N As Double, p As Double, Q As Double, W As Double, V As Double, r As Double, S As Double, ps As Double
        Dim result1 As Double, result2 As Double, term1 As Double, term2 As Double, ptnc As Double, qtnc As Double, ptx As Double, qtx As Double
        Dim a As Double, B As Double, x As Double, y As Double, nc As Double
        Dim save_result1 As Double, save_result2 As Double, vScale As Double
        a = 0.5
        B = df / 2.0#
        If Math.Abs(T) >= Min(1.0#, df) Then
            y = df / T
            x = T + y
            y = y / x
            x = T / x
        Else
            x = T * T
            y = df + x
            x = x / y
            y = df / y
        End If
        If y < cSmall Then
            comp_t_nc1 = ErrorValue
            Exit Function
        End If
        nc = nct * nct / 2.0#
        aa = a - nc * x * (a + B)
        bb = (x * nc - 1.0#) - a
        If (bb < 0.0#) Then
            N = bb - Math.Sqrt(bb ^ 2 - 4.0# * aa)
            N = Int(2.0# * aa / N)
        Else
            N = Int((bb + Math.Sqrt(bb ^ 2 - 4.0# * aa)) / 2)
        End If
        If N < 0.0# Then
            N = 0.0#
        End If
        aa = N + a
        bb = N + 0.5
        qtnc = poissonTerm(bb, nc, nc - bb, 0.0#)
        bb = N
        ptnc = poissonTerm(bb, nc, nc - bb, 0.0#)
        ptx = binomialTerm(aa, B, x, y, B * x - aa * y, 0.0#) / (aa + B) '((1 - I(x, aa+1, b)) - (1 - I(x, aa, b)))/b
        qtx = binomialTerm(aa + 0.5, B, x, y, B * x - (aa + 0.5) * y, 0.0#) / (aa + B + 0.5) '((1 - I(x, aa+3/2, b)) - (1 - I(x, aa+1/2, b)))/b
        If B > 1.0# Then
            ptx = B * ptx
            qtx = B * qtx
        End If
        vScale = max(ptnc, qtnc)
        If ptnc = vScale Then
            p = 1.0#
        Else
            p = ptnc / vScale
        End If
        If qtnc = vScale Then
            Q = 1.0#
        Else
            Q = qtnc / vScale
        End If
        nc_derivative = 0.0#
        S = ptx
        r = qtx
        If x > y Then
            V = BETA(y, B, aa + 0.5) '  1 - I(x, aa+1/2, b)
            W = BETA(y, B, aa) '  1 - I(x, aa, b)
        Else
            V = compbeta(x, aa + 0.5, B) ' 1 - I(x, aa+1/2, b)
            W = compbeta(x, aa, B) ' 1 - I(x, aa, b)
        End If
        term1 = 0.0#
        term2 = 0.0#
        result1 = term1
        result2 = term2
        Do
            If B > 1.0# Then
                W = W + S ' 1 - I(x, aa, b)
                V = V + r ' 1 - I(x, aa+1/2, b)
            Else
                W = W + B * S
                V = V + B * r
            End If
            S = (aa + B) * S
            r = (aa + B + 0.5) * r
            aa = aa + 1.0#
            bb = bb + 1.0#
            p = nc / bb * p
            Q = nc / (bb + 0.5) * Q
            ps = p * S + Q * r
            nc_derivative = nc_derivative + ps
            S = x / aa * S ' (1 - I(x, aa+1, b)) - (1 - I(x, aa, b))
            r = x / (aa + 0.5) * r ' (1 - I(x, aa+3/2, b)) - (1 - I(x, aa+1/2, b))
            term1 = p * W
            term2 = Q * V
            result1 = result1 + term1
            result2 = result2 + term2
        Loop While ((((term1 + term2) > 0.000000000000001 * (result1 + result2)) And (S > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_derivative))
        p = comppoisson(bb, nc, nc - bb)
        bb = bb + 0.5
        Q = comppoisson(bb, nc, nc - bb)
        nc_derivative = x * nc_derivative * vScale
        save_result1 = result1 * vScale + p * W
        save_result2 = result2 * vScale + Q * V
        ps = 1.0#
        nc_dtemp = 0.0#
        aa = N + a
        bb = N
        p = ptnc
        Q = qtnc
        vScale = max(ptx, qtx)
        If ptx = vScale Then
            S = 1.0#
        Else
            S = ptx / vScale
        End If
        If qtx = vScale Then
            r = 1.0#
        Else
            r = qtx / vScale
        End If
        W = p
        V = Q
        term1 = 1.0#
        term2 = 1.0#
        result1 = 0.0#
        result2 = 0.0#
        While bb > 0.0# And ((((term1 + term2) > 0.000000000000001 * (result1 + result2)) And (p > 0.0000000000000001 * W)) Or (ps > 0.0000000000000001 * nc_dtemp))
            r = (aa + 0.5) / x * r
            S = aa / x * S
            ps = p * S + Q * r
            nc_dtemp = nc_dtemp + ps
            p = bb / nc * p
            Q = (bb + 0.5) / nc * Q
            aa = aa - 1.0#
            bb = bb - 1.0#
            If bb = 0.0# Then aa = a
            r = r / (aa + B + 0.5) ' (1 - I(x, aa+3/2, b)) - (1 - I(x, aa+1/2, b))
            S = S / (aa + B) ' (1 - I(x, aa + 1, b)) - (1 - I(x, aa, b))
            term1 = S * W
            term2 = r * V
            result1 = result1 + term1
            result2 = result2 + term2
            W = W + p
            V = V + Q
        End While
        nc_dtemp = (x * nc_dtemp + p * aa * S + Q * (aa + 0.5) * r) * vScale
        If x > y Then
            r = BETA(y, B, a + (bb + 0.5))
            S = BETA(y, B, a + bb)
        Else
            r = compbeta(x, a + (bb + 0.5), B)
            S = compbeta(x, a + bb, B)
        End If
        If B <= 1.0# Then vScale = vScale * B
        result1 = save_result1 + result1 * vScale + S * W
        result2 = save_result2 + result2 * vScale + r * V
        If T > 0.0# Then
            comp_t_nc1 = 0.5 * (result1 + result2)
            nc_derivative = nc_derivative + nc_dtemp
        Else
            comp_t_nc1 = 1.0# - 0.5 * (result1 - result2)
        End If
    End Function

    Private Function inv_t_nc1(ByVal prob As Double, ByVal df As Double, ByVal nc As Double, ByRef oneMinusP As Double) As Double
        'Uses approximations in A&S 26.6.26 and 26.7.10 for to get initial estimate, the modified NR to improve it.
        Dim x As Double, y As Double, pr As Double, dif As Double, temp As Double, nc_BETA_param As Double
        Dim hix As Double, lox As Double, test As Double, nc_derivative As Double
        If (prob > 0.5) Then
            inv_t_nc1 = comp_inv_t_nc1(1.0# - prob, df, nc, oneMinusP)
            Exit Function
        End If
        nc_BETA_param = nc ^ 2 / 2.0#
        lox = 0.0#
        hix = t_nc_limit * Math.Sqrt(df)
        pr = Math.Exp(-nc_BETA_param)
        If pr > prob Then
            If 2.0# * prob > pr Then
                x = invcompbeta(0.5, df / 2.0#, (pr - prob) / pr, oneMinusP)
            Else
                x = invbeta(0.5, df / 2.0#, prob / pr, oneMinusP)
            End If
            If x = 0.0# Then
                inv_t_nc1 = 0.0#
                Exit Function
            Else
                temp = oneMinusP
                y = invbeta((0.5 + nc_BETA_param) ^ 2 / (0.5 + 2.0# * nc_BETA_param), df / 2.0#, prob, oneMinusP)
                oneMinusP = (0.5 + nc_BETA_param) * oneMinusP / (0.5 + nc_BETA_param * (1.0# + y))
                If temp > oneMinusP Then
                    oneMinusP = temp
                Else
                    x = (0.5 + 2.0# * nc_BETA_param) * y / (0.5 + nc_BETA_param * (1.0# + y))
                End If
                If oneMinusP < cSmall Then
                    pr = t_nc1(hix, df, nc, nc_derivative)
                    If pr < prob Then
                        inv_t_nc1 = ErrorValue
                        oneMinusP = 0.0#
                        Exit Function
                    End If
                    oneMinusP = 4.0# * cSmall
                End If
            End If
        Else
            y = invbeta((0.5 + nc_BETA_param) ^ 2 / (0.5 + 2.0# * nc_BETA_param), df / 2.0#, prob, oneMinusP)
            x = (0.5 + 2.0# * nc_BETA_param) * y / (0.5 + nc_BETA_param * (1 + y))
            oneMinusP = (0.5 + nc_BETA_param) * oneMinusP / (0.5 + nc_BETA_param * (1.0# + y))
            If oneMinusP < cSmall Then
                pr = t_nc1(hix, df, nc, nc_derivative)
                If pr < prob Then
                    inv_t_nc1 = ErrorValue
                    oneMinusP = 0.0#
                    Exit Function
                End If
                oneMinusP = 4.0# * cSmall
            End If
        End If
        test = Math.Sqrt(df * x) / Math.Sqrt(oneMinusP)
        Do
            pr = t_nc1(test, df, nc, nc_derivative)
            If pr < prob Then
                lox = test
            Else
                hix = test
            End If
            If nc_derivative = 0.0# Then
                If pr < prob Then
                    dif = (hix - lox) / 2.0#
                Else
                    dif = (lox - hix) / 2.0#
                End If
            Else
                dif = -(pr * test / nc_derivative) * logdif(pr, prob)
                If df < 2.0# Then dif = 2.0# * dif / df
                If test + dif < lox Then
                    If lox = 0 Then
                        dif = (lox - test) * 0.9999999999
                    Else
                        dif = (lox - test) * 0.9
                    End If
                ElseIf test + dif > hix Then
                    dif = (hix - test) * 0.9
                End If
            End If
            test = test + dif
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > test * 0.0000000001))
        inv_t_nc1 = test
    End Function

    Private Function comp_inv_t_nc1(ByVal prob As Double, ByVal df As Double, ByVal nc As Double, ByRef oneMinusP As Double) As Double
        'Uses approximations in A&S 26.6.26 and 26.7.10 for to get initial estimate, the modified NR to improve it.
        Dim x As Double, y As Double, pr As Double, dif As Double, temp As Double, nc_BETA_param As Double
        Dim hix As Double, lox As Double, test As Double, nc_derivative As Double
        If (prob > 0.5) Then
            comp_inv_t_nc1 = inv_t_nc1(1.0# - prob, df, nc, oneMinusP)
            Exit Function
        End If
        nc_BETA_param = nc ^ 2 / 2.0#
        lox = 0.0#
        hix = t_nc_limit * Math.Sqrt(df)
        pr = Math.Exp(-nc_BETA_param)
        If pr > prob Then
            If 2.0# * prob > pr Then
                x = invbeta(0.5, df / 2.0#, (pr - prob) / pr, oneMinusP)
            Else
                x = invcompbeta(0.5, df / 2.0#, prob / pr, oneMinusP)
            End If
            If oneMinusP < cSmall Then
                pr = comp_t_nc1(hix, df, nc, nc_derivative)
                If pr > prob Then
                    comp_inv_t_nc1 = ErrorValue
                    oneMinusP = 0.0#
                    Exit Function
                End If
                oneMinusP = 4.0# * cSmall
            Else
                temp = oneMinusP
                y = invcompbeta((0.5 + nc_BETA_param) ^ 2 / (0.5 + 2.0# * nc_BETA_param), df / 2.0#, prob, oneMinusP)
                oneMinusP = (0.5 + nc_BETA_param) * oneMinusP / (0.5 + nc_BETA_param * (1.0# + y))
                If temp < oneMinusP Then
                    oneMinusP = temp
                Else
                    x = (0.5 + 2.0# * nc_BETA_param) * y / (0.5 + nc_BETA_param * (1.0# + y))
                End If
                If oneMinusP < cSmall Then
                    pr = comp_t_nc1(hix, df, nc, nc_derivative)
                    If pr > prob Then
                        comp_inv_t_nc1 = ErrorValue
                        oneMinusP = 0.0#
                        Exit Function
                    End If
                    oneMinusP = 4.0# * cSmall
                End If
            End If
        Else
            y = invcompbeta((0.5 + nc_BETA_param) ^ 2 / (0.5 + 2.0# * nc_BETA_param), df / 2.0#, prob, oneMinusP)
            x = (0.5 + 2.0# * nc_BETA_param) * y / (0.5 + nc_BETA_param * (1.0# + y))
            oneMinusP = (0.5 + nc_BETA_param) * oneMinusP / (0.5 + nc_BETA_param * (1.0# + y))
            If oneMinusP < cSmall Then
                pr = comp_t_nc1(hix, df, nc, nc_derivative)
                If pr > prob Then
                    comp_inv_t_nc1 = ErrorValue
                    oneMinusP = 0.0#
                    Exit Function
                End If
                oneMinusP = 4.0# * cSmall
            End If
        End If
        test = Math.Sqrt(df * x) / Math.Sqrt(oneMinusP)
        dif = test
        Do
            pr = comp_t_nc1(test, df, nc, nc_derivative)
            If pr < prob Then
                hix = test
            Else
                lox = test
            End If
            If nc_derivative = 0.0# Then
                If pr < prob Then
                    dif = (lox - hix) / 2.0#
                Else
                    dif = (hix - lox) / 2.0#
                End If
            Else
                dif = (pr * test / nc_derivative) * logdif(pr, prob)
                If df < 2.0# Then dif = 2.0# * dif / df
                If test + dif < lox Then
                    dif = (lox - test) * 0.9
                ElseIf test + dif > hix Then
                    dif = (hix - test) * 0.9
                End If
            End If
            test = test + dif
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > test * 0.0000000001))
        comp_inv_t_nc1 = test
    End Function

    Private Function ncp_t_nc1(ByVal prob As Double, ByVal T As Double, ByVal df As Double) As Double
        'Uses Normal approx for non-central t (A&S 26.7.10) to get initial estimate the modified NR to improve it.
        Dim ncp As Double, pr As Double, dif As Double, temp As Double, deriv As Double, checked_tnc_limit As Boolean, checked_0_limit As Boolean
        Dim hi As Double, lo As Double, tnc_limit As Double, x As Double, y As Double
        If (prob > 0.5) Then
            ncp_t_nc1 = comp_ncp_t_nc1(1.0# - prob, T, df)
            Exit Function
        End If

        lo = 0.0#
        tnc_limit = Math.Sqrt(2.0# * nc_limit)
        hi = tnc_limit
        checked_0_limit = False
        checked_tnc_limit = False
        If T >= Min(1.0#, df) Then
            y = df / T
            x = T + y
            y = y / x
            x = T / x
        Else
            x = T * T
            y = df + x
            x = x / y
            y = df / y
        End If
        temp = -inv_normal(prob)
        If T > df Then
            ncp = T * (1.0# - 0.25 / df) + temp * Math.Sqrt(T) * Math.Sqrt((1.0# / T + 0.5 * T / df))
        Else
            ncp = T * (1.0# - 0.25 / df) + temp * Math.Sqrt((1.0# + (0.5 * T / df) * T))
        End If
        ncp = max(temp, ncp)
        'Debug.Print "ncp_estimate1", ncp
        If x > 1.0E-200 Then 'I think we can put more accurate bounds on when this will not deliver a sensible answer
            temp = invcompgamma(0.5 * x * df, prob) / y - 0.5
            If temp > 0 Then
                temp = Math.Sqrt(2.0# * temp)
                If temp > ncp Then
                    ncp = temp
                End If
            End If
        End If
        'Debug.Print "ncp_estimate2", ncp
        ncp = Min(ncp, tnc_limit)
        If ncp = tnc_limit Then
            pr = cdf_t_nc(T, df, ncp)
            If pr > prob Then
                ncp_t_nc1 = ErrorValue
                Exit Function
            Else
                checked_tnc_limit = True
            End If
        End If
        dif = ncp
        Do
            pr = cdf_t_nc(T, df, ncp)
            'Debug.Print ncp, pr, prob
            If ncp > 1 Then
                deriv = cdf_t_nc(T, df, ncp * (1 - 0.000001))
                deriv = 1000000.0# * (deriv - pr) / ncp
            ElseIf ncp > 0.000001 Then
                deriv = cdf_t_nc(T, df, ncp + 0.000001)
                deriv = 1000000.0# * (pr - deriv)
            ElseIf x < y Then
                deriv = comp_cdf_BETA(x, 1, df / 2) * OneOverSqrTwoPi
            Else
                deriv = cdf_BETA(y, df / 2, 1) * OneOverSqrTwoPi
            End If
            If pr < 3.0E-308 And deriv = 0.0# Then
                hi = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            ElseIf deriv = 0.0# Then
                lo = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            Else
                If pr < prob Then
                    hi = ncp
                Else
                    lo = ncp
                End If
                dif = (pr / deriv) * logdif(pr, prob)
                If ncp + dif < lo Then
                    dif = (lo - ncp) / 2.0#
                    If Not checked_0_limit And (lo = 0.0#) Then
                        temp = cdf_t_nc(T, df, lo)
                        If temp < prob Then
                            If invtdist(prob, df) <= T Then
                                ncp_t_nc1 = 0.0#
                            Else
                                ncp_t_nc1 = ErrorValue
                            End If
                            Exit Function
                        Else
                            checked_0_limit = True
                        End If
                        dif = dif * 1.99999999
                    End If
                ElseIf ncp + dif > hi Then
                    dif = (hi - ncp) / 2.0#
                    If Not checked_tnc_limit And (hi = tnc_limit) Then
                        temp = cdf_t_nc(T, df, hi)
                        If temp > prob Then
                            ncp_t_nc1 = ErrorValue
                            Exit Function
                        Else
                            checked_tnc_limit = True
                        End If
                        dif = dif * 1.99999999
                    End If
                End If
                ncp = ncp + dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(ncp) * 0.0000000001))
        ncp_t_nc1 = ncp
        'Debug.Print "ncp_t_nc1", ncp_t_nc1
    End Function

    Private Function comp_ncp_t_nc1(ByVal prob As Double, ByVal T As Double, ByVal df As Double) As Double
        'Uses Normal approx for non-central t (A&S 26.7.10) to get initial estimate the modified NR to improve it.
        Dim ncp As Double, pr As Double, dif As Double, temp As Double, temp1 As Double, temp2 As Double, deriv As Double, checked_tnc_limit As Boolean, checked_0_limit As Boolean
        Dim hi As Double, lo As Double, tnc_limit As Double, x As Double, y As Double
        If (prob > 0.5) Then
            comp_ncp_t_nc1 = ncp_t_nc1(1.0# - prob, T, df)
            Exit Function
        End If

        lo = 0.0#
        tnc_limit = Math.Sqrt(2.0# * nc_limit)
        hi = tnc_limit
        checked_0_limit = False
        checked_tnc_limit = False
        If T >= Min(1.0#, df) Then
            y = df / T
            x = T + y
            y = y / x
            x = T / x
        Else
            x = T * T
            y = df + x
            x = x / y
            y = df / y
        End If
        temp = -inv_normal(prob)
        temp1 = T * (1.0# - 0.25 / df)
        If T > df Then
            temp2 = temp * Math.Sqrt(T) * Math.Sqrt((1.0# / T + 0.5 * T / df))
        Else
            temp2 = temp * Math.Sqrt((1.0# + (0.5 * T / df) * T))
        End If
        ncp = max(temp, temp1 + temp2)
        'Debug.Print "comp_ncp ncp estimate1", ncp
        If x > 1.0E-200 Then 'I think we can put more accurate bounds on when this will not deliver a sensible answer
            temp = invcompgamma(0.5 * x * df, prob) / y - 0.5
            If temp > 0 Then
                temp = Math.Sqrt(2.0# * temp)
                If temp > ncp Then
                    temp = invgamma(0.5 * x * df, prob) / y - 0.5
                    If temp > 0 Then
                        ncp = Math.Sqrt(2.0# * temp)
                    Else
                        ncp = 0
                    End If
                Else
                    ncp = temp1 - temp2
                End If
            Else
                ncp = temp1 - temp2
            End If
        Else
            ncp = temp1 - temp2
        End If
        ncp = Min(max(0.0#, ncp), tnc_limit)
        If ncp = 0.0# Then
            pr = comp_cdf_t_nc(T, df, 0.0#)
            If pr > prob Then
                If -invtdist(prob, df) <= T Then
                    comp_ncp_t_nc1 = 0.0#
                Else
                    comp_ncp_t_nc1 = ErrorValue
                End If
                Exit Function
            ElseIf Math.Abs(pr - prob) <= -prob * 0.00000000000001 * Math.Log(pr) Then
                comp_ncp_t_nc1 = 0.0#
                Exit Function
            Else
                checked_0_limit = True
            End If
            If x < y Then
                deriv = -comp_cdf_BETA(x, 1, 0.5 * df) * OneOverSqrTwoPi
            Else
                deriv = -cdf_BETA(y, 0.5 * df, 1) * OneOverSqrTwoPi
            End If
            If deriv = 0.0# Then
                ncp = tnc_limit
            Else
                ncp = (pr - prob) / deriv
                If ncp >= tnc_limit Then
                    ncp = (pr / deriv) * logdif(pr, prob) 'If these two are miles apart then best to take invgamma estimate if > 0
                End If
            End If
        End If
        ncp = Min(ncp, tnc_limit)
        If ncp = tnc_limit Then
            pr = comp_cdf_t_nc(T, df, ncp)
            If pr < prob Then
                comp_ncp_t_nc1 = ErrorValue
                Exit Function
            Else
                checked_tnc_limit = True
            End If
        End If
        dif = ncp
        Do
            pr = comp_cdf_t_nc(T, df, ncp)
            'Debug.Print ncp, pr, prob
            If ncp > 1 Then
                deriv = comp_cdf_t_nc(T, df, ncp * (1 - 0.000001))
                deriv = 1000000.0# * (pr - deriv) / ncp
            ElseIf ncp > 0.000001 Then
                deriv = comp_cdf_t_nc(T, df, ncp + 0.000001)
                deriv = 1000000.0# * (deriv - pr)
            ElseIf x < y Then
                deriv = comp_cdf_BETA(x, 1, 0.5 * df) * OneOverSqrTwoPi
            Else
                deriv = cdf_BETA(y, 0.5 * df, 1) * OneOverSqrTwoPi
            End If
            If pr < 3.0E-308 And deriv = 0.0# Then
                lo = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            ElseIf deriv = 0.0# Then
                hi = ncp
                dif = dif / 2.0#
                ncp = ncp - dif
            Else
                If pr > prob Then
                    hi = ncp
                Else
                    lo = ncp
                End If
                dif = -(pr / deriv) * logdif(pr, prob)
                If ncp + dif < lo Then
                    dif = (lo - ncp) / 2.0#
                    If Not checked_0_limit And (lo = 0.0#) Then
                        temp = comp_cdf_t_nc(T, df, lo)
                        If temp > prob Then
                            If -invtdist(prob, df) <= T Then
                                comp_ncp_t_nc1 = 0.0#
                            Else
                                comp_ncp_t_nc1 = ErrorValue
                            End If
                            Exit Function
                        Else
                            checked_0_limit = True
                        End If
                        dif = dif * 1.99999999
                    End If
                ElseIf ncp + dif > hi Then
                    dif = (hi - ncp) / 2.0#
                    If Not checked_tnc_limit And (hi = tnc_limit) Then
                        temp = comp_cdf_t_nc(T, df, hi)
                        If temp < prob Then
                            comp_ncp_t_nc1 = ErrorValue
                            Exit Function
                        Else
                            checked_tnc_limit = True
                        End If
                        dif = dif * 1.99999999
                    End If
                End If
                ncp = ncp + dif
            End If
        Loop While ((Math.Abs(pr - prob) > prob * 0.00000000000001) And (Math.Abs(dif) > Math.Abs(ncp) * 0.0000000001))
        comp_ncp_t_nc1 = ncp
        'Debug.Print "comp_ncp_t_nc1", comp_ncp_t_nc1
    End Function

    Public Function pdf_t_nc(ByVal x As Double, ByVal df As Double, ByVal nc_param As Double) As Double
        '// Calculate pdf of noncentral t
        '// Deliberately set not to calculate when x and nc_param have opposite signs as the algorithm used is prone to cancellation error in these circumstances.
        '// The user can access t_nc1,comp_t_nc1 directly and check on the accuracy of the results, if required
        Dim nc_derivative As Double
        df = AlterForIntegralChecks_df(df)
        If (x < 0.0#) And (nc_param <= 0.0#) Then
            pdf_t_nc = pdf_t_nc(-x, df, -nc_param)
        ElseIf (df <= 0.0#) Or (nc_param < 0.0#) Or (nc_param > Math.Sqrt(2.0# * nc_limit)) Then
            pdf_t_nc = ErrorValue
        ElseIf (x < 0.0#) Then
            pdf_t_nc = ErrorValue
        ElseIf (x = 0.0# Or nc_param = 0.0#) Then
            pdf_t_nc = Math.Exp(-nc_param ^ 2 / 2) * pdftdist(x, df)
        Else
            If (df < 1.0# Or x < 1.0# Or x <= nc_param) Then
                pdf_t_nc = t_nc1(x, df, nc_param, nc_derivative)
            Else
                pdf_t_nc = comp_t_nc1(x, df, nc_param, nc_derivative)
            End If
            If nc_derivative < cSmall Then
                pdf_t_nc = Math.Exp(-nc_param ^ 2 / 2) * pdftdist(x, df)
            ElseIf df > 2.0# Then
                pdf_t_nc = nc_derivative / x
            Else
                pdf_t_nc = nc_derivative * (df / (2.0# * x))
            End If
        End If
        pdf_t_nc = GetRidOfMinusZeroes(pdf_t_nc)
    End Function

    Public Function cdf_t_nc(ByVal x As Double, ByVal df As Double, ByVal nc_param As Double) As Double
        '// Calculate cdf of noncentral t
        '// Deliberately set not to calculate when x and nc_param have opposite signs as the algorithm used is prone to cancellation error in these circumstances.
        '// The user can access t_nc1,comp_t_nc1 directly and check on the accuracy of the results, if required
        Dim tdistDensity As Double, nc_derivative As Double
        df = AlterForIntegralChecks_df(df)
        If (nc_param = 0.0#) Then
            cdf_t_nc = tdist(x, df, tdistDensity)
        ElseIf (x <= 0.0#) And (nc_param < 0.0#) Then
            cdf_t_nc = comp_cdf_t_nc(-x, df, -nc_param)
        ElseIf (df <= 0.0#) Or (nc_param < 0.0#) Or (nc_param > Math.Sqrt(2.0# * nc_limit)) Then
            cdf_t_nc = ErrorValue
        ElseIf (x < 0.0#) Then
            cdf_t_nc = ErrorValue
        ElseIf (df < 1.0# Or x < 1.0# Or x <= nc_param) Then
            cdf_t_nc = t_nc1(x, df, nc_param, nc_derivative)
        Else
            cdf_t_nc = 1.0# - comp_t_nc1(x, df, nc_param, nc_derivative)
        End If
        cdf_t_nc = GetRidOfMinusZeroes(cdf_t_nc)
    End Function

    Public Function comp_cdf_t_nc(ByVal x As Double, ByVal df As Double, ByVal nc_param As Double) As Double
        '// Calculate 1-cdf of noncentral t
        '// Deliberately set not to calculate when x and nc_param have opposite signs as the algorithm used is prone to cancellation error in these circumstances.
        '// The user can access t_nc1,comp_t_nc1 directly and check on the accuracy of the results, if required
        Dim tdistDensity As Double, nc_derivative As Double
        df = AlterForIntegralChecks_df(df)
        If (nc_param = 0.0#) Then
            comp_cdf_t_nc = tdist(-x, df, tdistDensity)
        ElseIf (x <= 0.0#) And (nc_param < 0.0#) Then
            comp_cdf_t_nc = cdf_t_nc(-x, df, -nc_param)
        ElseIf (df <= 0.0#) Or (nc_param < 0.0#) Or (nc_param > Math.Sqrt(2.0# * nc_limit)) Then
            comp_cdf_t_nc = ErrorValue
        ElseIf (x < 0.0#) Then
            comp_cdf_t_nc = ErrorValue
        ElseIf (df < 1.0# Or x < 1.0# Or x >= nc_param) Then
            comp_cdf_t_nc = comp_t_nc1(x, df, nc_param, nc_derivative)
        Else
            comp_cdf_t_nc = 1.0# - t_nc1(x, df, nc_param, nc_derivative)
        End If
        comp_cdf_t_nc = GetRidOfMinusZeroes(comp_cdf_t_nc)
    End Function

    Public Function inv_t_nc(ByVal prob As Double, ByVal df As Double, ByVal nc_param As Double) As Double
        df = AlterForIntegralChecks_df(df)
        If (nc_param = 0.0#) Then
            inv_t_nc = invtdist(prob, df)
        ElseIf (nc_param < 0.0#) Then
            inv_t_nc = -comp_inv_t_nc(prob, df, -nc_param)
        ElseIf (df <= 0.0# Or nc_param > Math.Sqrt(2.0# * nc_limit) Or prob <= 0.0# Or prob >= 1.0#) Then
            inv_t_nc = ErrorValue
        ElseIf (invcnormal(prob) < -nc_param) Then
            inv_t_nc = ErrorValue
        Else
            Dim oneMinusP As Double
            inv_t_nc = inv_t_nc1(prob, df, nc_param, oneMinusP)
        End If
        inv_t_nc = GetRidOfMinusZeroes(inv_t_nc)
    End Function

    Public Function comp_inv_t_nc(ByVal prob As Double, ByVal df As Double, ByVal nc_param As Double) As Double
        df = AlterForIntegralChecks_df(df)
        If (nc_param = 0.0#) Then
            comp_inv_t_nc = -invtdist(prob, df)
        ElseIf (nc_param < 0.0#) Then
            comp_inv_t_nc = -inv_t_nc(prob, df, -nc_param)
        ElseIf (df <= 0.0# Or nc_param > Math.Sqrt(2.0# * nc_limit) Or prob <= 0.0# Or prob >= 1.0#) Then
            comp_inv_t_nc = ErrorValue
        ElseIf (invcnormal(prob) > nc_param) Then
            comp_inv_t_nc = ErrorValue
        Else
            Dim oneMinusP As Double
            comp_inv_t_nc = comp_inv_t_nc1(prob, df, nc_param, oneMinusP)
        End If
        comp_inv_t_nc = GetRidOfMinusZeroes(comp_inv_t_nc)
    End Function

    Public Function ncp_t_nc(ByVal prob As Double, ByVal x As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        If (x = 0.0# And prob > 0.5) Then
            ncp_t_nc = -invcnormal(prob)
        ElseIf (x < 0) Then
            ncp_t_nc = -comp_ncp_t_nc(prob, -x, df)
        ElseIf (df <= 0.0# Or prob <= 0.0# Or prob >= 1.0#) Then
            ncp_t_nc = ErrorValue
        Else
            ncp_t_nc = ncp_t_nc1(prob, x, df)
        End If
        ncp_t_nc = GetRidOfMinusZeroes(ncp_t_nc)
    End Function

    Public Function comp_ncp_t_nc(ByVal prob As Double, ByVal x As Double, ByVal df As Double) As Double
        df = AlterForIntegralChecks_df(df)
        If (x = 0.0#) Then
            comp_ncp_t_nc = invcnormal(prob)
        ElseIf (x < 0) Then
            comp_ncp_t_nc = -ncp_t_nc(prob, -x, df)
        ElseIf (df <= 0.0# Or prob <= 0.0# Or prob >= 1.0#) Then
            comp_ncp_t_nc = ErrorValue
        Else
            comp_ncp_t_nc = comp_ncp_t_nc1(prob, x, df)
        End If
        comp_ncp_t_nc = GetRidOfMinusZeroes(comp_ncp_t_nc)
    End Function

    Public Function pmf_GammaPoisson(ByVal i As Double, ByVal gamma_shape As Double, ByVal gamma_scale As Double) As Double
        Dim p As Double, Q As Double, dfm As Double
        Q = gamma_scale / (1.0# + gamma_scale)
        p = 1.0# / (1.0# + gamma_scale)
        i = AlterForIntegralChecks_Others(i)
        If (gamma_shape <= 0.0# Or gamma_scale <= 0.0#) Then
            pmf_GammaPoisson = ErrorValue
        ElseIf (i < 0.0#) Then
            pmf_GammaPoisson = 0
        Else
            If p < Q Then
                dfm = gamma_shape - (gamma_shape + i) * p
            Else
                dfm = (gamma_shape + i) * Q - i
            End If
            pmf_GammaPoisson = (gamma_shape / (gamma_shape + i)) * binomialTerm(i, gamma_shape, Q, p, dfm, 0.0#)
        End If
        pmf_GammaPoisson = GetRidOfMinusZeroes(pmf_GammaPoisson)
    End Function

    Public Function cdf_GammaPoisson(ByVal i As Double, ByVal gamma_shape As Double, ByVal gamma_scale As Double) As Double
        Dim p As Double, Q As Double
        Q = gamma_scale / (1.0# + gamma_scale)
        p = 1.0# / (1.0# + gamma_scale)
        i = Int(i)
        If (gamma_shape <= 0.0# Or gamma_scale <= 0.0#) Then
            cdf_GammaPoisson = ErrorValue
        ElseIf (i < 0.0#) Then
            cdf_GammaPoisson = 0.0#
        ElseIf (p <= Q) Then
            cdf_GammaPoisson = BETA(p, gamma_shape, i + 1.0#)
        Else
            cdf_GammaPoisson = compbeta(Q, i + 1.0#, gamma_shape)
        End If
        cdf_GammaPoisson = GetRidOfMinusZeroes(cdf_GammaPoisson)
    End Function

    Public Function comp_cdf_GammaPoisson(ByVal i As Double, ByVal gamma_shape As Double, ByVal gamma_scale As Double) As Double
        Dim p As Double, Q As Double
        Q = gamma_scale / (1.0# + gamma_scale)
        p = 1.0# / (1.0# + gamma_scale)
        i = Int(i)
        If (gamma_shape <= 0.0# Or gamma_scale <= 0.0#) Then
            comp_cdf_GammaPoisson = ErrorValue
        ElseIf (i < 0.0#) Then
            comp_cdf_GammaPoisson = 1.0#
        ElseIf (p <= Q) Then
            comp_cdf_GammaPoisson = compbeta(p, gamma_shape, i + 1.0#)
        Else
            comp_cdf_GammaPoisson = BETA(Q, i + 1.0#, gamma_shape)
        End If
        comp_cdf_GammaPoisson = GetRidOfMinusZeroes(comp_cdf_GammaPoisson)
    End Function

    Public Function crit_GammaPoisson(ByVal gamma_shape As Double, ByVal gamma_scale As Double, ByVal crit_prob As Double) As Double
        Dim p As Double, Q As Double
        Q = gamma_scale / (1.0# + gamma_scale)
        p = 1.0# / (1.0# + gamma_scale)
        If (gamma_shape < 0.0# Or gamma_scale < 0.0#) Then
            crit_GammaPoisson = ErrorValue
        ElseIf (crit_prob < 0.0# Or crit_prob >= 1.0#) Then
            crit_GammaPoisson = ErrorValue
        ElseIf (crit_prob = 0.0#) Then
            crit_GammaPoisson = ErrorValue
        Else
            Dim i As Double, pr As Double
            crit_GammaPoisson = critnegbinom(gamma_shape, p, Q, crit_prob)
            i = crit_GammaPoisson
            If p <= Q Then
                pr = BETA(p, gamma_shape, i + 1.0#)
            Else
                pr = compbeta(Q, i + 1.0#, gamma_shape)
            End If
            If (pr = crit_prob) Then
            ElseIf (pr > crit_prob) Then
                i = i - 1.0#
                If p <= Q Then
                    pr = BETA(p, gamma_shape, i + 1.0#)
                Else
                    pr = compbeta(Q, i + 1.0#, gamma_shape)
                End If
                If (pr >= crit_prob) Then
                    crit_GammaPoisson = i
                End If
            Else
                crit_GammaPoisson = i + 1.0#
            End If
        End If
        crit_GammaPoisson = GetRidOfMinusZeroes(crit_GammaPoisson)
    End Function

    Public Function comp_crit_GammaPoisson(ByVal gamma_shape As Double, ByVal gamma_scale As Double, ByVal crit_prob As Double) As Double
        Dim p As Double, Q As Double
        Q = gamma_scale / (1.0# + gamma_scale)
        p = 1.0# / (1.0# + gamma_scale)
        If (gamma_shape < 0.0# Or gamma_scale < 0.0#) Then
            comp_crit_GammaPoisson = ErrorValue
        ElseIf (crit_prob <= 0.0# Or crit_prob > 1.0#) Then
            comp_crit_GammaPoisson = ErrorValue
        ElseIf (crit_prob = 1.0#) Then
            comp_crit_GammaPoisson = ErrorValue
        Else
            Dim i As Double, pr As Double
            comp_crit_GammaPoisson = critcompnegbinom(gamma_shape, p, Q, crit_prob)
            i = comp_crit_GammaPoisson
            If p <= Q Then
                pr = compbeta(p, gamma_shape, i + 1.0#)
            Else
                pr = BETA(Q, i + 1.0#, gamma_shape)
            End If
            If (pr = crit_prob) Then
            ElseIf (pr < crit_prob) Then
                i = i - 1.0#
                If p <= Q Then
                    pr = compbeta(p, gamma_shape, i + 1.0#)
                Else
                    pr = BETA(Q, i + 1.0#, gamma_shape)
                End If
                If (pr <= crit_prob) Then
                    comp_crit_GammaPoisson = i
                End If
            Else
                comp_crit_GammaPoisson = i + 1.0#
            End If
        End If
        comp_crit_GammaPoisson = GetRidOfMinusZeroes(comp_crit_GammaPoisson)
    End Function

    Private Function PBB(ByVal i As Double, ByVal ssmi As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double) As Double
        PBB = (BETA_shape1 / (i + BETA_shape1)) * (BETA_shape2 / (BETA_shape1 + BETA_shape2)) * ((i + ssmi + BETA_shape1 + BETA_shape2) / (ssmi + BETA_shape2)) * hypergeometricTerm(i, ssmi, BETA_shape1, BETA_shape2)
    End Function

    Public Function pmf_BetaNegativeBinomial(ByVal i As Double, ByVal r As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double) As Double
        i = AlterForIntegralChecks_Others(i)
        If (r <= 0.0# Or BETA_shape1 <= 0.0# Or BETA_shape2 <= 0.0#) Then
            pmf_BetaNegativeBinomial = ErrorValue
        ElseIf i < 0 Then
            pmf_BetaNegativeBinomial = 0.0#
        Else
            pmf_BetaNegativeBinomial = (BETA_shape2 / (BETA_shape1 + BETA_shape2)) * (r / (BETA_shape1 + r)) * BETA_shape1 * (i + BETA_shape1 + r + BETA_shape2) / ((i + r) * (i + BETA_shape2)) * hypergeometricTerm(i, r, BETA_shape2, BETA_shape1)
        End If
        pmf_BetaNegativeBinomial = GetRidOfMinusZeroes(pmf_BetaNegativeBinomial)
    End Function

    Private Function CBNB0(ByVal i As Double, ByVal r As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double, ByVal toBeAdded As Double) As Double
        Dim ha1 As Double, hprob As Double, hswap As Boolean
        Dim mrb2 As Double, other As Double, temp As Double
        If (r < 2.0# Or BETA_shape2 < 2.0#) Then
            'Assumption here that i is integral or greater than 4.
            mrb2 = max(r, BETA_shape2)
            other = Min(r, BETA_shape2)
            CBNB0 = PBB(i, other, mrb2, BETA_shape1)
            If i = 0.0# Then Exit Function
            CBNB0 = CBNB0 * (1.0# + i * (other + BETA_shape1) / (((i - 1.0#) + mrb2) * (other + 1.0#)))
            If i = 1.0# Then Exit Function
            i = i - 2.0#
            other = other + 2.0#
            temp = PBB(i, mrb2, other, BETA_shape1)
            If i = 0.0# Then
                CBNB0 = CBNB0 + temp
                Exit Function
            End If
            CBNB0 = CBNB0 + temp * (1.0# + i * (mrb2 + BETA_shape1) / (((i - 1.0#) + other) * (mrb2 + 1.0#)))
            If i = 1.0# Then Exit Function
            i = i - 2.0#
            mrb2 = mrb2 + 2.0#
            CBNB0 = CBNB0 + CBNB0(i, mrb2, BETA_shape1, other, CBNB0)
        ElseIf (BETA_shape1 < 1.0#) Then
            mrb2 = max(r, BETA_shape2)
            other = Min(r, BETA_shape2)
            CBNB0 = hypergeometric(i, mrb2 - 1.0#, other, BETA_shape1, False, ha1, hprob, hswap)
            If hswap Then
                temp = PBB(mrb2 - 1.0#, BETA_shape1, i + 1.0#, other)
                If (toBeAdded + (CBNB0 - temp)) < 0.1 * (toBeAdded + (CBNB0 + temp)) Then
                    CBNB0 = CBNB2(i, mrb2, BETA_shape1, other)
                Else
                    CBNB0 = CBNB0 - temp
                End If
            ElseIf ha1 < -0.9 * BETA_shape1 / (BETA_shape1 + other) Then
                CBNB0 = ErrorValue
            Else
                CBNB0 = hprob * (BETA_shape1 / (BETA_shape1 + other) + ha1)
            End If
        Else
            CBNB0 = hypergeometric(i, r, BETA_shape2, BETA_shape1 - 1.0#, False, ha1, hprob, hswap)
        End If
    End Function

    Private Function CBNB2(ByVal i As Double, ByVal r As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double) As Double
        Dim j As Double, ss As Double, bs2 As Double, temp As Double, d1 As Double, d2 As Double, d_count As Double, pbbval As Double
        'In general may be a good idea to take Min(i, BETA_shape1) down to just above 0 and then work on Max(i, BETA_shape1)
        ss = Min(r, BETA_shape2)
        bs2 = max(r, BETA_shape2)
        r = ss
        BETA_shape2 = bs2
        d1 = (i + 0.5) * (BETA_shape1 + 0.5) - (bs2 - 1.5) * (ss - 0.5)
        If d1 < 0.0# Then
            CBNB2 = CBNB0(i, ss, BETA_shape1, bs2, 0.0#)
            Exit Function
        End If
        d1 = Int(d1 / (bs2 + BETA_shape1)) + 1.0#
        If ss + d1 > bs2 Then d1 = Int(bs2 - ss)
        ss = ss + d1
        j = i - d1
        d2 = (j + 0.5) * (BETA_shape1 + 0.5) - (bs2 - 1.5) * (ss - 0.5)
        If d2 < 0.0# Then
            d2 = 2.0#
        Else
            'Could make this smaller
            d2 = Int(Math.Sqrt(d2)) + 2.0#
        End If
        If 2.0# * d2 > i Then
            d2 = Int(i / 2.0#)
        End If
        pbbval = PBB(i, r, BETA_shape2, BETA_shape1)
        ss = ss + d2
        bs2 = bs2 + d2
        j = j - 2.0# * d2
        CBNB2 = CBNB0(j, ss, BETA_shape1, bs2, 0.0#)
        temp = 1.0#
        d_count = d2 - 2.0#
        j = j + 1.0#
        Do While d_count >= 0.0#
            j = j + 1.0#
            bs2 = BETA_shape2 + d_count
            d_count = d_count - 1.0#
            temp = 1.0# + (j * (bs2 + BETA_shape1) / ((j + ss - 1.0#) * (bs2 + 1.0#))) * temp
        Loop
        j = i - d2 - d1
        temp = (ss * (j + bs2)) / (bs2 * (j + ss)) * temp
        d_count = d1 + d2 - 1.0#
        Do While d_count >= 0
            j = j + 1.0#
            ss = r + d_count
            d_count = d_count - 1.0#
            temp = 1.0# + (j * (ss + BETA_shape1) / ((j + bs2 - 1.0#) * (ss + 1.0#))) * temp
        Loop
        CBNB2 = CBNB2 + temp * pbbval
        Exit Function
    End Function

    Public Function cdf_BetaNegativeBinomial(ByVal i As Double, ByVal r As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double) As Double
        i = Int(i)
        If (r <= 0.0# Or BETA_shape1 <= 0.0# Or BETA_shape2 <= 0.0#) Then
            cdf_BetaNegativeBinomial = ErrorValue
        ElseIf i < 0 Then
            cdf_BetaNegativeBinomial = 0.0#
        Else
            cdf_BetaNegativeBinomial = CBNB0(i, r, BETA_shape1, BETA_shape2, 0.0#)
        End If
        cdf_BetaNegativeBinomial = GetRidOfMinusZeroes(cdf_BetaNegativeBinomial)
    End Function

    Public Function comp_cdf_BetaNegativeBinomial(ByVal i As Double, ByVal r As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double) As Double
        Dim ha1 As Double, hprob As Double, hswap As Boolean
        Dim mrb2 As Double, other As Double, temp As Double, CTEMP As Double
        i = Int(i)
        mrb2 = max(r, BETA_shape2)
        other = Min(r, BETA_shape2)
        If (other <= 0.0# Or BETA_shape1 <= 0.0#) Then
            comp_cdf_BetaNegativeBinomial = ErrorValue
        ElseIf i < 0.0# Then
            comp_cdf_BetaNegativeBinomial = 1.0#
        ElseIf (mrb2 > 100.0#) Then
            comp_cdf_BetaNegativeBinomial = CBNB0(mrb2 - 1.0#, i + 1.0#, other, BETA_shape1, 0.0#)
        ElseIf (other < 1.0# Or BETA_shape1 < 1.0#) Then
            If (i + BETA_shape1) < 60.0# Then
                i = i + 1.0#
                temp = pmf_BetaNegativeBinomial(i, r, BETA_shape1, BETA_shape2)
                CTEMP = temp
                Do While (i + BETA_shape1) < 60.0#
                    temp = temp * (i + r) * (i + BETA_shape2) / ((i + 1.0#) * (i + BETA_shape1 + r + BETA_shape2))
                    CTEMP = CTEMP + temp
                    i = i + 1.0#
                Loop
            Else
                CTEMP = 0.0#
            End If
            If other >= 1.0# Then
                comp_cdf_BetaNegativeBinomial = hypergeometric(mrb2, i, BETA_shape1, other - 1.0#, False, ha1, hprob, hswap)
            Else
                comp_cdf_BetaNegativeBinomial = hypergeometric(mrb2, i, BETA_shape1, -other, False, ha1, hprob, hswap)
            End If
            If hswap Then
                temp = PBB(i, mrb2, other, BETA_shape1) 'N.B. subtraction of PBB term can be done exactly from hypergeometric one if hswap false
                If temp > 0.9 * comp_cdf_BetaNegativeBinomial Then
                    comp_cdf_BetaNegativeBinomial = ErrorValue
                Else
                    comp_cdf_BetaNegativeBinomial = (comp_cdf_BetaNegativeBinomial - temp) + CTEMP
                End If
            Else
                If ha1 < -0.9 * mrb2 / (BETA_shape1 + mrb2) Then
                    comp_cdf_BetaNegativeBinomial = ErrorValue
                Else
                    comp_cdf_BetaNegativeBinomial = hprob * (mrb2 / (BETA_shape1 + mrb2) + ha1) + CTEMP
                End If
            End If
        Else
            comp_cdf_BetaNegativeBinomial = hypergeometric(i, r, BETA_shape2, BETA_shape1 - 1.0#, True, ha1, hprob, hswap)
        End If
        comp_cdf_BetaNegativeBinomial = GetRidOfMinusZeroes(comp_cdf_BetaNegativeBinomial)
    End Function

    Private Function critbetanegbinomial(ByVal a As Double, ByVal B As Double, ByVal r As Double, ByVal cprob As Double) As Double
        '//i such that Pr(betanegbinomial(i,r,a,b)) >= cprob and  Pr(betanegbinomial(i-1,r,a,b)) < cprob
        If (cprob > 0.5) Then
            critbetanegbinomial = critcompbetanegbinomial(a, B, r, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double
        Dim i As Double, temp As Double, temp2 As Double, oneMinusP As Double
        If B > r Then
            i = B
            B = r
            r = i
        End If
        If (a < 10.0# Or B < 10.0#) Then
            If r < a And a < 1.0# Then
                pr = cprob * a / r
            Else
                pr = cprob
            End If
            i = invcompbeta(a, B, pr, oneMinusP)
        Else
            pr = r / (r + a + B - 1.0#)
            i = invcompbeta(a * pr, B * pr, cprob, oneMinusP)
        End If
        If i = 0.0# Then
            i = max_crit / 2.0#
        Else
            i = r * (oneMinusP / i)
            If i >= max_crit Then
                i = max_crit - 1.0#
            End If
        End If
        While (True)
            If (i < 0.0#) Then
                i = 0.0#
            End If
            i = Int(i + 0.5)
            If (i >= max_crit) Then
                critbetanegbinomial = ErrorValue
                Exit Function
            End If
            pr = CBNB0(i, r, a, B, 0.0#)
            tpr = 0.0#
            If (pr > cprob * (1 + cfSmall)) Then
                If (i = 0.0#) Then
                    critbetanegbinomial = 0.0#
                    Exit Function
                End If
                tpr = pmf_BetaNegativeBinomial(i, r, a, B)
                If (pr < (1.0# + 0.00001) * tpr) Then
                    tpr = tpr * (((i + 1.0#) * (i + a + r + B)) / ((i + r) * (i + B)))
                    i = i - 1.0#
                    While (tpr > cprob)
                        tpr = tpr * (((i + 1.0#) * (i + a + r + B)) / ((i + r) * (i + B)))
                        i = i - 1.0#
                    End While
                Else
                    pr = pr - tpr
                    If (pr < cprob) Then
                        critbetanegbinomial = i
                        Exit Function
                    End If
                    i = i - 1.0#
                    If (i = 0.0#) Then
                        critbetanegbinomial = 0.0#
                        Exit Function
                    End If
                    temp = (pr - cprob) / tpr
                    If (temp > 10.0#) Then
                        temp = Int(temp + 0.5)
                        If temp > i Then
                            i = i / 10.0#
                        Else
                            i = Int(i - temp)
                            temp2 = pmf_BetaNegativeBinomial(i, r, a, B)
                            i = i - temp * (tpr - temp2) / (2.0# * temp2)
                        End If
                    Else
                        tpr = tpr * (((i + 1.0#) * (i + a + r + B)) / ((i + r) * (i + B)))
                        pr = pr - tpr
                        If (pr < cprob) Then
                            critbetanegbinomial = i
                            Exit Function
                        End If
                        i = i - 1.0#
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr >= cprob)
                                tpr = tpr * (((i + 1.0#) * (i + a + r + B)) / ((i + r) * (i + B)))
                                pr = pr - tpr
                                i = i - 1.0#
                            End While
                            critbetanegbinomial = i + 1.0#
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log((((i + 1.0#) * (i + a + r + B)) / ((i + r) * (i + B)))) + 0.5)
                            i = i - temp
                            temp2 = pmf_BetaNegativeBinomial(i, r, a, B)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log((((i + 1.0#) * (i + a + r + B)) / ((i + r) * (i + B))))
                                i = i - temp
                            End If
                        End If
                    End If
                End If
            ElseIf ((1.0# + cfSmall) * pr < cprob) Then
                While ((tpr < cSmall) And (pr < cprob))
                    i = i + 1.0#
                    tpr = pmf_BetaNegativeBinomial(i, r, a, B)
                    pr = pr + tpr
                End While
                temp = (cprob - pr) / tpr
                If temp <= 0.0# Then
                    critbetanegbinomial = i
                    Exit Function
                ElseIf temp < 10.0# Then
                    While (pr < cprob)
                        i = i + 1.0#
                        tpr = tpr * (((i + r - 1.0#) * (i + B - 1.0#)) / (i * (i + a + r + B - 1.0#)))
                        pr = pr + tpr
                    End While
                    critbetanegbinomial = i
                    Exit Function
                ElseIf i + temp > max_crit Then
                    critbetanegbinomial = ErrorValue
                    Exit Function
                Else
                    i = Int(i + temp)
                    temp2 = pmf_BetaNegativeBinomial(i, r, a, B)
                    If temp2 > 0.0# Then i = i + temp * (tpr - temp2) / (2.0# * temp2)
                End If
            Else
                critbetanegbinomial = i
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critcompbetanegbinomial(ByVal a As Double, ByVal B As Double, ByVal r As Double, ByVal cprob As Double) As Double
        '//i such that 1-Pr(betanegbinomial(i,r,a,b)) > cprob and  1-Pr(betanegbinomial(i-1,r,a,b)) <= cprob
        If (cprob > 0.5) Then
            critcompbetanegbinomial = critbetanegbinomial(a, B, r, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double
        Dim i As Double, temp As Double, temp2 As Double, oneMinusP As Double
        If B > r Then
            i = B
            B = r
            r = i
        End If
        If (a < 10.0# Or B < 10.0#) Then
            If r < a And a < 1.0# Then
                pr = cprob * a / r
            Else
                pr = cprob
            End If
            i = invbeta(a, B, pr, oneMinusP)
        Else
            pr = r / (r + a + B - 1.0#)
            i = invbeta(a * pr, B * pr, cprob, oneMinusP)
        End If
        If i = 0.0# Then
            i = max_crit / 2.0#
        Else
            i = r * (oneMinusP / i)
            If i >= max_crit Then
                i = max_crit - 1.0#
            End If
        End If
        While (True)
            If (i < 0.0#) Then
                i = 0.0#
            End If
            i = Int(i + 0.5)
            If (i >= max_crit) Then
                critcompbetanegbinomial = ErrorValue
                Exit Function
            End If
            pr = comp_cdf_BetaNegativeBinomial(i, r, a, B)
            tpr = 0.0#
            If (pr > cprob * (1 + cfSmall)) Then
                i = i + 1.0#
                tpr = pmf_BetaNegativeBinomial(i, r, a, B)
                If (pr < (1 + 0.00001) * tpr) Then
                    While (tpr > cprob)
                        i = i + 1.0#
                        tpr = tpr * (((i + r - 1.0#) * (i + B - 1.0#)) / (i * (i + a + r + B - 1.0#)))
                    End While
                Else
                    pr = pr - tpr
                    If (pr <= cprob) Then
                        critcompbetanegbinomial = i
                        Exit Function
                    End If
                    temp = (pr - cprob) / tpr
                    If (temp > 10.0#) Then
                        temp = Int(temp + 0.5)
                        i = i + temp
                        temp2 = pmf_BetaNegativeBinomial(i, r, a, B)
                        i = i + temp * (tpr - temp2) / (2.0# * temp2)
                    Else
                        i = i + 1.0#
                        tpr = tpr * (((i + r - 1.0#) * (i + B - 1.0#)) / (i * (i + a + r + B - 1.0#)))
                        pr = pr - tpr
                        If (pr <= cprob) Then
                            critcompbetanegbinomial = i
                            Exit Function
                        End If
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr > cprob)
                                i = i + 1.0#
                                tpr = tpr * (((i + r - 1.0#) * (i + B - 1.0#)) / (i * (i + a + r + B - 1.0#)))
                                pr = pr - tpr
                            End While
                            critcompbetanegbinomial = i
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log((((i + r - 1.0#) * (i + B - 1.0#)) / (i * (i + a + r + B - 1.0#)))) + 0.5)
                            i = i + temp
                            temp2 = pmf_BetaNegativeBinomial(i, r, a, B)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log((((i + r - 1.0#) * (i + B - 1.0#)) / (i * (i + a + r + B - 1.0#))))
                                i = i + temp
                            End If
                        End If
                    End If
                End If
            ElseIf ((1.0# + cfSmall) * pr < cprob) Then
                While ((tpr < cSmall) And (pr <= cprob))
                    tpr = pmf_BetaNegativeBinomial(i, r, a, B)
                    pr = pr + tpr
                    i = i - 1.0#
                End While
                temp = (cprob - pr) / tpr
                If temp <= 0.0# Then
                    critcompbetanegbinomial = i + 1.0#
                    Exit Function
                ElseIf temp < 100.0# Or i < 1000.0# Then
                    While (pr <= cprob)
                        tpr = tpr * (((i + 1.0#) * (i + a + r + B)) / ((i + r) * (i + B)))
                        pr = pr + tpr
                        i = i - 1.0#
                    End While
                    critcompbetanegbinomial = i + 1.0#
                    Exit Function
                ElseIf temp > i Then
                    i = i / 10.0#
                Else
                    i = Int(i - temp)
                    temp2 = pmf_BetaNegativeBinomial(i, r, a, B)
                    If temp2 > 0.0# Then i = i - temp * (tpr - temp2) / (2.0# * temp2)
                End If
            Else
                critcompbetanegbinomial = i
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Public Function crit_BetaNegativeBinomial(ByVal r As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double, ByVal crit_prob As Double) As Double
        If (BETA_shape1 <= 0.0# Or BETA_shape2 <= 0.0# Or r <= 0.0#) Then
            crit_BetaNegativeBinomial = ErrorValue
        ElseIf (crit_prob < 0.0# Or crit_prob >= 1.0#) Then
            crit_BetaNegativeBinomial = ErrorValue
        ElseIf (crit_prob = 0.0#) Then
            crit_BetaNegativeBinomial = ErrorValue
        Else
            crit_BetaNegativeBinomial = critbetanegbinomial(BETA_shape1, BETA_shape2, r, crit_prob)
        End If
        crit_BetaNegativeBinomial = GetRidOfMinusZeroes(crit_BetaNegativeBinomial)
    End Function

    Public Function comp_crit_BetaNegativeBinomial(ByVal r As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double, ByVal crit_prob As Double) As Double
        If (BETA_shape1 <= 0.0# Or BETA_shape2 <= 0.0# Or r <= 0.0#) Then
            comp_crit_BetaNegativeBinomial = ErrorValue
        ElseIf (crit_prob <= 0.0# Or crit_prob > 1.0#) Then
            comp_crit_BetaNegativeBinomial = ErrorValue
        ElseIf (crit_prob = 1.0#) Then
            comp_crit_BetaNegativeBinomial = 0.0#
        Else
            comp_crit_BetaNegativeBinomial = critcompbetanegbinomial(BETA_shape1, BETA_shape2, r, crit_prob)
        End If
        comp_crit_BetaNegativeBinomial = GetRidOfMinusZeroes(comp_crit_BetaNegativeBinomial)
    End Function

    Public Function pmf_BetaBinomial(ByVal i As Double, ByVal SAMPLE_SIZE As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double) As Double
        i = AlterForIntegralChecks_Others(i)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (BETA_shape1 <= 0.0# Or BETA_shape2 <= 0.0# Or SAMPLE_SIZE < 0.0#) Then
            pmf_BetaBinomial = ErrorValue
        ElseIf i < 0 Or i > SAMPLE_SIZE Then
            pmf_BetaBinomial = 0.0#
        Else
            pmf_BetaBinomial = (BETA_shape1 / (i + BETA_shape1)) * (BETA_shape2 / (BETA_shape1 + BETA_shape2)) * ((SAMPLE_SIZE + BETA_shape1 + BETA_shape2) / (SAMPLE_SIZE - i + BETA_shape2)) * hypergeometricTerm(i, SAMPLE_SIZE - i, BETA_shape1, BETA_shape2)
        End If
        pmf_BetaBinomial = GetRidOfMinusZeroes(pmf_BetaBinomial)
    End Function

    Public Function cdf_BetaBinomial(ByVal i As Double, ByVal SAMPLE_SIZE As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double) As Double
        i = Int(i)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (BETA_shape1 <= 0.0# Or BETA_shape2 <= 0.0# Or SAMPLE_SIZE < 0.0#) Then
            cdf_BetaBinomial = ErrorValue
        ElseIf i < 0.0# Then
            cdf_BetaBinomial = 0.0#
        ElseIf i >= SAMPLE_SIZE Then
            cdf_BetaBinomial = 1.0#
        Else
            cdf_BetaBinomial = CBNB0(i, SAMPLE_SIZE - i, BETA_shape2, BETA_shape1, 0.0#)
        End If
        cdf_BetaBinomial = GetRidOfMinusZeroes(cdf_BetaBinomial)
    End Function

    Public Function comp_cdf_BetaBinomial(ByVal i As Double, ByVal SAMPLE_SIZE As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double) As Double
        i = Int(i)
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (BETA_shape1 <= 0.0# Or BETA_shape2 <= 0.0# Or SAMPLE_SIZE < 0.0#) Then
            comp_cdf_BetaBinomial = ErrorValue
        ElseIf i < 0.0# Then
            comp_cdf_BetaBinomial = 1.0#
        ElseIf i >= SAMPLE_SIZE Then
            comp_cdf_BetaBinomial = 0.0#
        Else
            comp_cdf_BetaBinomial = CBNB0(SAMPLE_SIZE - i - 1.0#, i + 1.0#, BETA_shape1, BETA_shape2, 0.0#)
        End If
        comp_cdf_BetaBinomial = GetRidOfMinusZeroes(comp_cdf_BetaBinomial)
    End Function

    Private Function critbetabinomial(ByVal a As Double, ByVal B As Double, ByVal ss As Double, ByVal cprob As Double) As Double
        '//i such that Pr(betabinomial(i,ss,a,b)) >= cprob and  Pr(betabinomial(i-1,ss,a,b)) < cprob
        If (cprob > 0.5) Then
            critbetabinomial = critcompbetabinomial(a, B, ss, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double
        Dim i As Double, temp As Double, temp2 As Double, oneMinusP As Double
        If (a + B < 1.0#) Then
            i = invbeta(a, B, cprob, oneMinusP) * ss
        Else
            pr = ss / (ss + a + B - 1.0#)
            i = invbeta(a * pr, B * pr, cprob, oneMinusP) * ss
        End If
        While (True)
            If (i < 0.0#) Then
                i = 0.0#
            ElseIf (i > ss) Then
                i = ss
            End If
            i = Int(i + 0.5)
            If (i >= max_discrete) Then
                critbetabinomial = i
                Exit Function
            End If
            pr = cdf_BetaBinomial(i, ss, a, B)
            tpr = 0.0#
            If (pr >= cprob * (1 + cfSmall)) Then
                If (i = 0.0#) Then
                    critbetabinomial = 0.0#
                    Exit Function
                End If
                tpr = pmf_BetaBinomial(i, ss, a, B)
                If (pr < (1.0# + 0.00001) * tpr) Then
                    tpr = tpr * ((i + 1.0#) * (ss + B - i - 1.0#)) / ((a + i) * (ss - i))
                    i = i - 1.0#
                    While (tpr > cprob)
                        tpr = tpr * ((i + 1.0#) * (ss + B - i - 1.0#)) / ((a + i) * (ss - i))
                        i = i - 1.0#
                    End While
                Else
                    pr = pr - tpr
                    If (pr < cprob) Then
                        critbetabinomial = i
                        Exit Function
                    End If
                    i = i - 1.0#
                    If (i = 0.0#) Then
                        critbetabinomial = 0.0#
                        Exit Function
                    End If
                    temp = (pr - cprob) / tpr
                    If (temp > 10) Then
                        temp = Int(temp + 0.5)
                        i = i - temp
                        temp2 = pmf_BetaBinomial(i, ss, a, B)
                        i = i - temp * (tpr - temp2) / (2.0# * temp2)
                    Else
                        tpr = tpr * ((i + 1.0#) * (ss + B - i - 1.0#)) / ((a + i) * (ss - i))
                        pr = pr - tpr
                        If (pr < cprob) Then
                            critbetabinomial = i
                            Exit Function
                        End If
                        i = i - 1.0#
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr >= cprob)
                                tpr = tpr * ((i + 1.0#) * (ss + B - i - 1.0#)) / ((a + i) * (ss - i))
                                pr = pr - tpr
                                i = i - 1.0#
                            End While
                            critbetabinomial = i + 1.0#
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log(((i + 1.0#) * (ss + B - i - 1.0#)) / ((a + i) * (ss - i))) + 0.5)
                            i = i - temp
                            temp2 = pmf_BetaBinomial(i, ss, a, B)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((i + 1.0#) * (ss + B - i - 1.0#)) / ((a + i) * (ss - i)))
                                i = i - temp
                            End If
                        End If
                    End If
                End If
            ElseIf ((1.0# + cfSmall) * pr < cprob) Then
                While ((tpr < cSmall) And (pr < cprob))
                    i = i + 1.0#
                    tpr = pmf_BetaBinomial(i, ss, a, B)
                    pr = pr + tpr
                End While
                temp = (cprob - pr) / tpr
                If temp <= 0.0# Then
                    critbetabinomial = i
                    Exit Function
                ElseIf temp < 10.0# Then
                    While (pr < cprob)
                        i = i + 1.0#
                        tpr = tpr * ((a + i - 1.0#) * (ss - i + 1.0#)) / (i * (ss + B - i))
                        pr = pr + tpr
                    End While
                    critbetabinomial = i
                    Exit Function
                ElseIf temp > 4.0E+15 Then
                    i = 4.0E+15
                Else
                    i = Int(i + temp)
                    temp2 = pmf_BetaBinomial(i, ss, a, B)
                    If temp2 > 0.0# Then i = i + temp * (tpr - temp2) / (2.0# * temp2)
                End If
            Else
                critbetabinomial = i
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Private Function critcompbetabinomial(ByVal a As Double, ByVal B As Double, ByVal ss As Double, ByVal cprob As Double) As Double
        '//i such that 1-Pr(betabinomial(i,ss,a,b)) > cprob and  1-Pr(betabinomial(i-1,ss,a,b)) <= cprob
        If (cprob > 0.5) Then
            critcompbetabinomial = critbetabinomial(a, B, ss, 1.0# - cprob)
            Exit Function
        End If
        Dim pr As Double, tpr As Double
        Dim i As Double, temp As Double, temp2 As Double, oneMinusP As Double
        If (a + B < 1.0#) Then
            i = invcompbeta(a, B, cprob, oneMinusP) * ss
        Else
            pr = ss / (ss + a + B - 1.0#)
            i = invcompbeta(a * pr, B * pr, cprob, oneMinusP) * ss
        End If
        While (True)
            If (i < 0.0#) Then
                i = 0.0#
            ElseIf (i > ss) Then
                i = ss
            End If
            i = Int(i + 0.5)
            If (i >= max_discrete) Then
                critcompbetabinomial = i
                Exit Function
            End If
            pr = comp_cdf_BetaBinomial(i, ss, a, B)
            tpr = 0.0#
            If (pr >= cprob * (1 + cfSmall)) Then
                i = i + 1.0#
                tpr = pmf_BetaBinomial(i, ss, a, B)
                If (pr < (1 + 0.00001) * tpr) Then
                    Do While (tpr > cprob)
                        i = i + 1.0#
                        temp = ss + B - i
                        If temp = 0.0# Then Exit Do
                        tpr = tpr * ((a + i - 1.0#) * (ss - i + 1.0#)) / (i * temp)
                    Loop
                Else
                    pr = pr - tpr
                    If (pr <= cprob) Then
                        critcompbetabinomial = i
                        Exit Function
                    End If
                    temp = (pr - cprob) / tpr
                    If (temp > 10.0#) Then
                        temp = Int(temp + 0.5)
                        i = i + temp
                        temp2 = pmf_BetaBinomial(i, ss, a, B)
                        i = i + temp * (tpr - temp2) / (2.0# * temp2)
                    Else
                        i = i + 1.0#
                        tpr = tpr * ((a + i - 1.0#) * (ss - i + 1.0#)) / (i * (ss + B - i))
                        pr = pr - tpr
                        If (pr <= cprob) Then
                            critcompbetabinomial = i
                            Exit Function
                        End If
                        temp2 = (pr - cprob) / tpr
                        If (temp2 < temp - 0.9) Then
                            While (pr > cprob)
                                i = i + 1.0#
                                tpr = tpr * ((a + i - 1.0#) * (ss - i + 1.0#)) / (i * (ss + B - i))
                                pr = pr - tpr
                            End While
                            critcompbetabinomial = i
                            Exit Function
                        Else
                            temp = Int(Math.Log(cprob / pr) / Math.Log(((a + i - 1.0#) * (ss - i + 1.0#)) / (i * (ss + B - i))) + 0.5)
                            i = i + temp
                            temp2 = pmf_BetaBinomial(i, ss, a, B)
                            If (temp2 > nearly_zero) Then
                                temp = Math.Log((cprob / pr) * (tpr / temp2)) / Math.Log(((a + i - 1.0#) * (ss - i + 1.0#)) / (i * (ss + B - i)))
                                i = i + temp
                            End If
                        End If
                    End If
                End If
            ElseIf ((1.0# + cfSmall) * pr < cprob) Then
                While ((tpr < cSmall) And (pr <= cprob))
                    tpr = pmf_BetaBinomial(i, ss, a, B)
                    pr = pr + tpr
                    i = i - 1.0#
                End While
                temp = (cprob - pr) / tpr
                If temp <= 0.0# Then
                    critcompbetabinomial = i + 1.0#
                    Exit Function
                ElseIf temp < 100.0# Or i < 1000.0# Then
                    While (pr <= cprob)
                        tpr = tpr * ((i + 1.0#) * (ss + B - i - 1.0#)) / ((a + i) * (ss - i))
                        pr = pr + tpr
                        i = i - 1.0#
                    End While
                    critcompbetabinomial = i + 1.0#
                    Exit Function
                ElseIf temp > i Then
                    i = i / 10.0#
                Else
                    i = Int(i - temp)
                    temp2 = pmf_BetaNegativeBinomial(i, ss, a, B)
                    If temp2 > 0.0# Then i = i - temp * (tpr - temp2) / (2.0# * temp2)
                End If
            Else
                critcompbetabinomial = i + 1.0#
                Exit Function
            End If
        End While
        Return ErrorValue
    End Function

    Public Function crit_BetaBinomial(ByVal SAMPLE_SIZE As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double, ByVal crit_prob As Double) As Double
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (BETA_shape1 <= 0.0# Or BETA_shape2 <= 0.0# Or SAMPLE_SIZE < 0.0#) Then
            crit_BetaBinomial = ErrorValue
        ElseIf (crit_prob < 0.0# Or crit_prob > 1.0#) Then
            crit_BetaBinomial = ErrorValue
        ElseIf (crit_prob = 0.0#) Then
            crit_BetaBinomial = ErrorValue
        ElseIf (SAMPLE_SIZE = 0.0# Or crit_prob = 1.0#) Then
            crit_BetaBinomial = SAMPLE_SIZE
        Else
            crit_BetaBinomial = critbetabinomial(BETA_shape1, BETA_shape2, SAMPLE_SIZE, crit_prob)
        End If
        crit_BetaBinomial = GetRidOfMinusZeroes(crit_BetaBinomial)
    End Function

    Public Function comp_crit_BetaBinomial(ByVal SAMPLE_SIZE As Double, ByVal BETA_shape1 As Double, ByVal BETA_shape2 As Double, ByVal crit_prob As Double) As Double
        SAMPLE_SIZE = AlterForIntegralChecks_Others(SAMPLE_SIZE)
        If (BETA_shape1 <= 0.0# Or BETA_shape2 <= 0.0# Or SAMPLE_SIZE < 0.0#) Then
            comp_crit_BetaBinomial = ErrorValue
        ElseIf (crit_prob < 0.0# Or crit_prob > 1.0#) Then
            comp_crit_BetaBinomial = ErrorValue
        ElseIf (crit_prob = 1.0#) Then
            comp_crit_BetaBinomial = 0.0#
        ElseIf (SAMPLE_SIZE = 0.0# Or crit_prob = 0.0#) Then
            comp_crit_BetaBinomial = SAMPLE_SIZE
        Else
            comp_crit_BetaBinomial = critcompbetabinomial(BETA_shape1, BETA_shape2, SAMPLE_SIZE, crit_prob)
        End If
        comp_crit_BetaBinomial = GetRidOfMinusZeroes(comp_crit_BetaBinomial)
    End Function

    Public Function pdf_normal_os(ByVal x As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1) As Double
        ' pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid N(0,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then pdf_normal_os = ErrorValue : Exit Function
        Dim n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        If x <= 0 Then
            pdf_normal_os = pdf_BETA(cnormal(x), n1 + r, -r) * pdf_normal(x)
        Else
            pdf_normal_os = pdf_BETA(cnormal(-x), -r, n1 + r) * pdf_normal(-x)
        End If
    End Function

    Public Function cdf_normal_os(ByVal x As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1) As Double
        ' cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid N(0,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then cdf_normal_os = ErrorValue : Exit Function
        Dim n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        If x <= 0 Then
            cdf_normal_os = cdf_BETA(cnormal(x), n1 + r, -r)
        Else
            cdf_normal_os = comp_cdf_BETA(cnormal(-x), -r, n1 + r)
        End If
    End Function

    Public Function comp_cdf_normal_os(ByVal x As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1) As Double
        ' 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid N(0,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_cdf_normal_os = ErrorValue : Exit Function
        Dim n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        If x <= 0 Then
            comp_cdf_normal_os = comp_cdf_BETA(cnormal(x), n1 + r, -r)
        Else
            comp_cdf_normal_os = cdf_BETA(cnormal(-x), -r, n1 + r)
        End If
    End Function

    Public Function inv_normal_os(ByVal p As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1) As Double
        ' inverse of cdf_normal_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        ' accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then inv_normal_os = ErrorValue : Exit Function
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invbeta(n1 + r, -r, p, oneMinusxp)
        If Math.Abs(xp - 0.5) < 0.00000000000001 And xp <> 0.5 Then If cdf_BETA(0.5, n1 + r, -r) = p Then inv_normal_os = 0 : Exit Function
        If xp <= 0.5 Then
            inv_normal_os = inv_normal(xp)
        Else
            inv_normal_os = -inv_normal(oneMinusxp)
        End If
    End Function

    Public Function comp_inv_normal_os(ByVal p As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1) As Double
        ' inverse of comp_cdf_normal_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_inv_normal_os = ErrorValue : Exit Function
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invcompbeta(n1 + r, -r, p, oneMinusxp)
        If Math.Abs(xp - 0.5) < 0.00000000000001 And xp <> 0.5 Then If comp_cdf_BETA(0.5, n1 + r, -r) = p Then comp_inv_normal_os = 0 : Exit Function
        If xp <= 0.5 Then
            comp_inv_normal_os = inv_normal(xp)
        Else
            comp_inv_normal_os = -inv_normal(oneMinusxp)
        End If
    End Function

    Public Function pdf_gamma_os(ByVal x As Double, ByVal shape_param As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal scale_param As Double = 1, Optional ByVal nc_param As Double = 0) As Double
        ' pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then pdf_gamma_os = ErrorValue : Exit Function
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_gamma_nc(x / scale_param, shape_param, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            pdf_gamma_os = pdf_BETA(p, n1 + r, -r) * pdf_gamma_nc(x / scale_param, shape_param, nc_param) / scale_param
        Else
            pdf_gamma_os = pdf_BETA(comp_cdf_gamma_nc(x / scale_param, shape_param, nc_param), -r, n1 + r) * pdf_gamma_nc(x / scale_param, shape_param, nc_param) / scale_param
        End If
    End Function

    Public Function cdf_gamma_os(ByVal x As Double, ByVal shape_param As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal scale_param As Double = 1, Optional ByVal nc_param As Double = 0) As Double
        ' cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then cdf_gamma_os = ErrorValue : Exit Function
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_gamma_nc(x / scale_param, shape_param, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            cdf_gamma_os = cdf_BETA(p, n1 + r, -r)
        Else
            cdf_gamma_os = comp_cdf_BETA(comp_cdf_gamma_nc(x / scale_param, shape_param, nc_param), -r, n1 + r)
        End If
    End Function

    Public Function comp_cdf_gamma_os(ByVal x As Double, ByVal shape_param As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal scale_param As Double = 1, Optional ByVal nc_param As Double = 0) As Double
        ' 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_cdf_gamma_os = ErrorValue : Exit Function
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_gamma_nc(x / scale_param, shape_param, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            comp_cdf_gamma_os = comp_cdf_BETA(p, n1 + r, -r)
        Else
            comp_cdf_gamma_os = cdf_BETA(comp_cdf_gamma_nc(x / scale_param, shape_param, nc_param), -r, n1 + r)
        End If
    End Function

    Public Function inv_gamma_os(ByVal p As Double, ByVal shape_param As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal scale_param As Double = 1, Optional ByVal nc_param As Double = 0) As Double
        ' inverse of cdf_gamma_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        ' accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then inv_gamma_os = ErrorValue : Exit Function
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invbeta(n1 + r, -r, p, oneMinusxp)
        If xp <= 0.5 Then       ' avoid truncation error by working with xp <= 0.5
            inv_gamma_os = inv_gamma_nc(xp, shape_param, nc_param) * scale_param
        Else
            inv_gamma_os = comp_inv_gamma_nc(oneMinusxp, shape_param, nc_param) * scale_param
        End If
    End Function

    Public Function comp_inv_gamma_os(ByVal p As Double, ByVal shape_param As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal scale_param As Double = 1, Optional ByVal nc_param As Double = 0) As Double
        ' inverse of comp_cdf_gamma_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_inv_gamma_os = ErrorValue : Exit Function
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invcompbeta(n1 + r, -r, p, oneMinusxp)
        If xp <= 0.5 Then       ' avoid truncation error by working with xp <= 0.5
            comp_inv_gamma_os = inv_gamma_nc(xp, shape_param, nc_param) * scale_param
        Else
            comp_inv_gamma_os = comp_inv_gamma_nc(oneMinusxp, shape_param, nc_param) * scale_param
        End If
    End Function

    Public Function pdf_chi2_os(ByVal x As Double, ByVal df As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid chi2(df) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then pdf_chi2_os = ErrorValue : Exit Function
        df = AlterForIntegralChecks_df(df)
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_Chi2_nc(x, df, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            pdf_chi2_os = pdf_BETA(p, n1 + r, -r) * pdf_Chi2_nc(x, df, nc_param)
        Else
            pdf_chi2_os = pdf_BETA(comp_cdf_Chi2_nc(x, df, nc_param), -r, n1 + r) * pdf_Chi2_nc(x, df, nc_param)
        End If
    End Function

    Public Function cdf_chi2_os(ByVal x As Double, ByVal df As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid chi2(df) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then cdf_chi2_os = ErrorValue : Exit Function
        df = AlterForIntegralChecks_df(df)
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_Chi2_nc(x, df, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            cdf_chi2_os = cdf_BETA(p, n1 + r, -r)
        Else
            cdf_chi2_os = comp_cdf_BETA(comp_cdf_Chi2_nc(x, df, nc_param), -r, n1 + r)
        End If
    End Function

    Public Function comp_cdf_chi2_os(ByVal x As Double, ByVal df As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid chi2(df) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_cdf_chi2_os = ErrorValue : Exit Function
        df = AlterForIntegralChecks_df(df)
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_Chi2_nc(x, df, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            comp_cdf_chi2_os = comp_cdf_BETA(p, n1 + r, -r)
        Else
            comp_cdf_chi2_os = cdf_BETA(comp_cdf_Chi2_nc(x, df, nc_param), -r, n1 + r)
        End If
    End Function

    Public Function inv_chi2_os(ByVal p As Double, ByVal df As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' inverse of cdf_chi2_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        ' accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then inv_chi2_os = ErrorValue : Exit Function
        df = AlterForIntegralChecks_df(df)
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invbeta(n1 + r, -r, p, oneMinusxp)
        If xp <= 0.5 Then       ' avoid truncation error by working with xp <= 0.5
            inv_chi2_os = inv_Chi2_nc(xp, df, nc_param)
        Else
            inv_chi2_os = comp_inv_Chi2_nc(oneMinusxp, df, nc_param)
        End If
    End Function

    Public Function comp_inv_chi2_os(ByVal p As Double, ByVal df As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' inverse of comp_cdf_chi2_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_inv_chi2_os = ErrorValue : Exit Function
        df = AlterForIntegralChecks_df(df)
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invcompbeta(n1 + r, -r, p, oneMinusxp)
        If xp <= 0.5 Then       ' avoid truncation error by working with xp <= 0.5
            comp_inv_chi2_os = inv_Chi2_nc(xp, df, nc_param)
        Else
            comp_inv_chi2_os = comp_inv_Chi2_nc(oneMinusxp, df, nc_param)
        End If
    End Function

    Public Function pdf_F_os(ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then pdf_F_os = ErrorValue : Exit Function
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_fdist_nc(x, df1, df2, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            pdf_F_os = pdf_BETA(p, n1 + r, -r) * pdf_fdist_nc(x, df1, df2, nc_param)
        Else
            pdf_F_os = pdf_BETA(comp_cdf_fdist_nc(x, df1, df2, nc_param), -r, n1 + r) * pdf_fdist_nc(x, df1, df2, nc_param)
        End If
    End Function

    Public Function cdf_F_os(ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then cdf_F_os = ErrorValue : Exit Function
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_fdist_nc(x, df1, df2, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            cdf_F_os = cdf_BETA(p, n1 + r, -r)
        Else
            cdf_F_os = comp_cdf_BETA(comp_cdf_fdist_nc(x, df1, df2, nc_param), -r, n1 + r)
        End If
    End Function

    Public Function comp_cdf_F_os(ByVal x As Double, ByVal df1 As Double, ByVal df2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_cdf_F_os = ErrorValue : Exit Function
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_fdist_nc(x, df1, df2, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            comp_cdf_F_os = comp_cdf_BETA(p, n1 + r, -r)
        Else
            comp_cdf_F_os = cdf_BETA(comp_cdf_fdist_nc(x, df1, df2, nc_param), -r, n1 + r)
        End If
    End Function

    Public Function inv_F_os(ByVal p As Double, ByVal df1 As Double, ByVal df2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' inverse of cdf_F_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        ' accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then inv_F_os = ErrorValue : Exit Function
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invbeta(n1 + r, -r, p, oneMinusxp)
        If xp <= 0.5 Then       ' avoid truncation error by working with xp <= 0.5
            inv_F_os = inv_fdist_nc(xp, df1, df2, nc_param)
        Else
            inv_F_os = comp_inv_fdist_nc(oneMinusxp, df1, df2, nc_param)
        End If
    End Function

    Public Function comp_inv_F_os(ByVal p As Double, ByVal df1 As Double, ByVal df2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' inverse of comp_cdf_F_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_inv_F_os = ErrorValue : Exit Function
        df1 = AlterForIntegralChecks_df(df1)
        df2 = AlterForIntegralChecks_df(df2)
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invcompbeta(n1 + r, -r, p, oneMinusxp)
        If xp <= 0.5 Then       ' avoid truncation error by working with xp <= 0.5
            comp_inv_F_os = inv_fdist_nc(xp, df1, df2, nc_param)
        Else
            comp_inv_F_os = comp_inv_fdist_nc(oneMinusxp, df1, df2, nc_param)
        End If
    End Function

    Public Function pdf_BETA_os(ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' pdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then pdf_BETA_os = ErrorValue : Exit Function
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_BETA_nc(x, shape_param1, shape_param2, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            pdf_BETA_os = pdf_BETA(p, n1 + r, -r) * pdf_BETA_nc(x, shape_param1, shape_param2, nc_param)
        Else
            pdf_BETA_os = pdf_BETA(comp_cdf_BETA_nc(x, shape_param1, shape_param2, nc_param), -r, n1 + r) * pdf_BETA_nc(x, shape_param1, shape_param2, nc_param)
        End If
    End Function

    Public Function cdf_BETA_os(ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then cdf_BETA_os = ErrorValue : Exit Function
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_BETA_nc(x, shape_param1, shape_param2, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            cdf_BETA_os = cdf_BETA(p, n1 + r, -r)
        Else
            cdf_BETA_os = comp_cdf_BETA(comp_cdf_BETA_nc(x, shape_param1, shape_param2, nc_param), -r, n1 + r)
        End If
    End Function

    Public Function comp_cdf_BETA_os(ByVal x As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' 1-cdf for rth smallest (r>0) or -rth largest (r<0) of the n order statistics from a sample of iid gamma(a,1) variables
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_cdf_BETA_os = ErrorValue : Exit Function
        Dim p As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        p = cdf_BETA_nc(x, shape_param1, shape_param2, nc_param)
        If p <= 0.5 Then        ' avoid truncation error by working with p <= 0.5
            comp_cdf_BETA_os = comp_cdf_BETA(p, n1 + r, -r)
        Else
            comp_cdf_BETA_os = cdf_BETA(comp_cdf_BETA_nc(x, shape_param1, shape_param2, nc_param), -r, n1 + r)
        End If
    End Function

    Public Function inv_BETA_os(ByVal p As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' inverse of cdf_BETA_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        ' accuracy for median of extreme order statistic is limited by accuracy of IEEE double precision representation of n >> 10^15, not by this routine
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then inv_BETA_os = ErrorValue : Exit Function
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invbeta(n1 + r, -r, p, oneMinusxp)
        If xp <= 0.5 Then       ' avoid truncation error by working with xp <= 0.5
            inv_BETA_os = inv_BETA_nc(xp, shape_param1, shape_param2, nc_param)
        Else
            inv_BETA_os = comp_inv_BETA_nc(oneMinusxp, shape_param1, shape_param2, nc_param)
        End If
    End Function

    Public Function comp_inv_BETA_os(ByVal p As Double, ByVal shape_param1 As Double, ByVal shape_param2 As Double, Optional ByVal N As Double = 1, Optional ByVal r As Double = -1, Optional ByVal nc_param As Double = 0) As Double
        ' inverse of comp_cdf_BETA_os
        ' based on formula 2.1.5 "Order Statistics" by H.A. David (any edition)
        Dim oneMinusxp As Double
        N = AlterForIntegralChecks_Others(N) : r = AlterForIntegralChecks_Others(r)
        If N < 1 Or Math.Abs(r) > N Or r = 0 Then comp_inv_BETA_os = ErrorValue : Exit Function
        Dim xp As Double, n1 As Double : n1 = N + 1
        If r > 0 Then r = r - n1
        xp = invcompbeta(n1 + r, -r, p, oneMinusxp)
        If xp <= 0.5 Then       ' avoid truncation error by working with xp <= 0.5
            comp_inv_BETA_os = inv_BETA_nc(xp, shape_param1, shape_param2, nc_param)
        Else
            comp_inv_BETA_os = comp_inv_BETA_nc(oneMinusxp, shape_param1, shape_param2, nc_param)
        End If
    End Function
End Class
