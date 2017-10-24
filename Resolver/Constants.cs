namespace com.inova8.resolver
{
    using System;
    public class Constants
    {

        internal static int NextGeneratedLabel = 0;

        protected internal static double ETA = 0.0001;
        protected internal static double SMALLVALUE = 1e-20;
        protected internal static double LARGEVALUE = 100000000.0;
        protected internal static double ACCURACY = 0.001;

        internal const string OpenParen = "(";
        internal const string CloseParen = ")";
        internal const string OpenBracket = "[";
        internal const string CloseBracket = "]";
        internal const string OpenBrace = "{";
        internal const string CloseBrace = "}";
        internal const string Comma = ",";
        internal const string Equal = "=";
        internal const string Grt = ">";
        internal const string GrtEqual = ">=";
        internal const string Less = "<";
        internal const string LessEqual = "<=";

         internal static System.String EqualityString(OperatorType Equality)
        {
            switch (Equality)
            {
                case OperatorType.Equal: return Equal;
                case OperatorType.Grt: return Grt;
                case OperatorType.GrtEqual: return GrtEqual;
                case OperatorType.Less: return Less;
                case OperatorType.LessEqual: return LessEqual;
                default:  return "=";
            }
         }

         internal static OperatorType StringEquality(String Equality)
         {
             if (Equality.Equals(Constants.Equal))
             {
                 return Constants.OperatorType.Equal;
             }
             else if (Equality.Equals(Constants.Grt))
             {
                 return Constants.OperatorType.Grt;
             }
             else if (Equality.Equals(Constants.GrtEqual))
             {
                 return Constants.OperatorType.GrtEqual;
             }
             else if (Equality.Equals(Constants.Less))
             {
                 return Constants.OperatorType.Less;
             }
             else if (Equality.Equals(Constants.LessEqual))
             {
                 return Constants.OperatorType.LessEqual;
             }
             else
             {
                 return Constants.OperatorType.Equal;
             }
         }
        internal enum OperatorType
        {
            NullType = 0,
            StringType,
            Real,
            Equal,
            Grt,
            GrtEqual,
            Less,
            LessEqual,
            Minus,
            Plus,
            Multiply,
            Divide,
            Expon,
            UnaryMinus,
            UnaryPlus,
            Nested,
            Identifier,
            ptrVariable,
            ListHead,
            ListSeparator,
            EmptyList,
            ptrExpression,
            Function,
            Range
        }
        internal enum FunctionType
        {
            NULL,   // Used for initialization
            ABS, // 	Returns the absolute value of a number
            ACOS, //	Returns the arccosine of a number
            ACOSH, //	Returns the inverse hyperbolic cosine of a number
            ASIN, //	Returns the arcsine of a number
            ASINH, //	Returns the inverse hyperbolic sine of a number
            ATAN, //	Returns the arctangent of a number
            ATAN2, //	Returns the arctangent from x- and y-coordinates
            ATANH, //	Returns the inverse hyperbolic tangent of a number
            CEILING, //	Rounds a number to the nearest integer or to the nearest multiple of significance
            COMBIN, //	Returns the number of combinations for a given number of objects
            COS, //	Returns the cosine of a number
            COSH, //	Returns the hyperbolic cosine of a number
            DEGREES, //	Converts radians to degrees
            EVEN, //	Rounds a number up to the nearest even integer
            EXP, //	Returns e raised to the power of a given number
            FACT, //	Returns the factorial of a number
            FACTDOUBLE, //	Returns the double factorial of a number
            FLOOR, //	Rounds a number down, toward zero
            GCD, //	Returns the greatest common divisor
            INT, //	Rounds a number down to the nearest integer
            LCM, //	Returns the least common multiple
            LN, //	Returns the natural logarithm of a number
            LOG, //	Returns the logarithm of a number to a specified base
            LOG10, //	Returns the base-10 logarithm of a number
            MDETERM, //	Returns the matrix determinant of an array
            MINVERSE, //	Returns the matrix inverse of an array
            MMULT, //	Returns the matrix product of two arrays
            MOD, //	Returns the remainder from division
            MROUND, //	Returns a number rounded to the desired multiple
            MULTINOMIAL, //	Returns the multinomial of a set of numbers
            ODD, //	Rounds a number up to the nearest odd integer
            PI, //	Returns the value of pi
            POWER, //	Returns the result of a number raised to a power
            PRODUCT, //	Multiplies its arguments
            QUOTIENT, //	Returns the integer portion of a division
            RADIANS, //	Converts degrees to radians
            RAND, //	Returns a random number between 0 and 1
            RANDBETWEEN, //	Returns a random number between the numbers you specify
            ROMAN, //	Converts an arabic numeral to roman, as text
            ROUND, //	Rounds a number to a specified number of digits
            ROUNDDOWN, //	Rounds a number down, toward zero
            ROUNDUP, //	Rounds a number up, away from zero
            SERIESSUM, //	Returns the sum of a power series based on the formula
            SIGN, //	Returns the sign of a number
            SIN, //	Returns the sine of the given angle
            SINH, //	Returns the hyperbolic sine of a number
            SQRT, //	Returns a positive square root
            SQRTPI, //	Returns the square root of (number * pi)
            SUBTOTAL, //	Returns a subtotal in a list or database
            SUM, //	Adds its arguments
            SUMIF, //	Adds the cells specified by a given criteria
            SUMIFS, //	Adds the cells in a range that meet multiple criteria
            SUMPRODUCT, //	Returns the sum of the products of corresponding array components
            SUMSQ, //	Returns the sum of the squares of the arguments
            SUMX2MY2, //	Returns the sum of the difference of squares of corresponding values in two arrays
            SUMX2PY2, //	Returns the sum of the sum of squares of corresponding values in two arrays
            SUMXMY2, //	Returns the sum of squares of differences of corresponding values in two arrays
            TAN, //	Returns the tangent of a number
            TANH, //	Returns the hyperbolic tangent of a number
            TRUNC, //	Truncates a number to an integer
            USERDEF //	Userdefined function
        }
        internal static System.String FunctionString(FunctionType Function)
        {
            switch (Function)
            {
                case FunctionType.NULL: return "NULL";
                case FunctionType.ABS: return "ABS";
                case FunctionType.ACOS: return "ACOS";
                case FunctionType.ACOSH: return "ACOSH";
                case FunctionType.ASIN: return "ASIN";
                case FunctionType.ASINH: return "ASINH";
                case FunctionType.ATAN: return "ATAN";
                case FunctionType.ATAN2: return "ATAN2";
                case FunctionType.ATANH: return "ATANH";
                case FunctionType.CEILING: return "CEILING";
                case FunctionType.COMBIN: return "COMBIN";
                case FunctionType.COS: return "COS";
                case FunctionType.COSH: return "COSH";
                case FunctionType.DEGREES: return "DEGREES";
                case FunctionType.EVEN: return "EVEN";
                case FunctionType.EXP: return "EXP";
                case FunctionType.FACT: return "FACT";
                case FunctionType.FACTDOUBLE: return "FACTDOUBLE";
                case FunctionType.FLOOR: return "FLOOR";
                case FunctionType.GCD: return "GCD";
                case FunctionType.INT: return "INT";
                case FunctionType.LCM: return "LCM";
                case FunctionType.LN: return "LN";
                case FunctionType.LOG: return "LOG";
                case FunctionType.LOG10: return "LOG10";
                case FunctionType.MDETERM: return "MDETERM";
                case FunctionType.MINVERSE: return "MINVERSE";
                case FunctionType.MMULT: return "MMULT";
                case FunctionType.MOD: return "MOD";
                case FunctionType.MROUND: return "MROUND";
                case FunctionType.MULTINOMIAL: return "MULTINOMIAL";
                case FunctionType.ODD: return "ODD";
                case FunctionType.PI: return "PI";
                case FunctionType.POWER: return "POWER";
                case FunctionType.PRODUCT: return "PRODUCT";
                case FunctionType.QUOTIENT: return "QUOTIENT";
                case FunctionType.RADIANS: return "RADIANS";
                case FunctionType.RAND: return "RAND";
                case FunctionType.RANDBETWEEN: return "RANDBETWEEN";
                case FunctionType.ROMAN: return "ROMAN";
                case FunctionType.ROUND: return "ROUND";
                case FunctionType.ROUNDDOWN: return "ROUNDDOWN";
                case FunctionType.ROUNDUP: return "ROUNDUP";
                case FunctionType.SERIESSUM: return "SERIESSUM";
                case FunctionType.SIGN: return "SIGN";
                case FunctionType.SIN: return "SIN";
                case FunctionType.SINH: return "SINH";
                case FunctionType.SQRT: return "SQRT";
                case FunctionType.SQRTPI: return "SQRTPI";
                case FunctionType.SUBTOTAL: return "SUBTOTAL";
                case FunctionType.SUM: return "SUM";
                case FunctionType.SUMIF: return "SUMIF";
                case FunctionType.SUMIFS: return "SUMIFS";
                case FunctionType.SUMPRODUCT: return "SUMPRODUCT";
                case FunctionType.SUMSQ: return "SUMSQ";
                case FunctionType.SUMX2MY2: return "SUMX2MY2";
                case FunctionType.SUMX2PY2: return "SUMX2PY2";
                case FunctionType.SUMXMY2: return "SUMXMY2";
                case FunctionType.TAN: return "TAN";
                case FunctionType.TANH: return "TANH";
                case FunctionType.TRUNC: return "TRUNC";
                case FunctionType.USERDEF: return "USERDEF";
                default: return "<unknownfunction>";
            }
        }
        internal static FunctionType StringFunction(String FunctionName)
        {
            foreach (FunctionType fn in Enum.GetValues(typeof(FunctionType)))
            {
                if (FunctionString(fn) == FunctionName)
                {
                    return fn;
                }
            }
            return FunctionType.NULL;
        }
        internal enum LinearityType
        {
            Constant = 0,
            Linear = 1,
            Nonlinear = 2,
            Unknown = 3
        }
        internal enum SolvabilityType
        {
            Observable = 0,
            Unobservable = 1,
            Redundant = 2,
            Determined = 3,
            Fixed = 4
        }

        internal static System.String SolvableString(SolvabilityType Solvability)
        {
            //System.String SolvableString = "";
            switch (Solvability)
            {
                case SolvabilityType.Observable:
                    return "Observable";
                case SolvabilityType.Unobservable:
                    return "Unobservable";
                case SolvabilityType.Redundant:
                    return "Redundant";
                case SolvabilityType.Determined:
                    return "Determined";
                case SolvabilityType.Fixed:
                    return "Fixed";
                default:
                    return "";
            }
        }
        internal static System.String OperatorString(OperatorType Operator)
        {
            switch (Operator)
            {
                case OperatorType.NullType:
                    return "<EOF>";
                case OperatorType.StringType:
                    return "<string>";
                case OperatorType.Real:
                    return "<real>";
                case OperatorType.Equal:
                    return "=";
                case OperatorType.Grt:
                    return ">";
                case OperatorType.GrtEqual:
                    return ">=";
                case OperatorType.Less:
                    return "<";
                case OperatorType.LessEqual:
                    return "<=";
                case OperatorType.Minus:
                    return "-";
                case OperatorType.Plus:
                    return "+";
                case OperatorType.Multiply:
                    return "*";
                case OperatorType.Divide:
                    return "/";
                case OperatorType.Expon:
                    return "^";
                case OperatorType.UnaryMinus:
                    return "-";
                case OperatorType.UnaryPlus:
                    return "+";
                case OperatorType.Nested:
                    return "<nested>";
                case OperatorType.Identifier:
                    return "<id>";
                case OperatorType.ptrVariable:
                    return "<variable>";
                case OperatorType.ListHead:
                    return "<list>";
                case OperatorType.ListSeparator:
                    return ",";
                case OperatorType.EmptyList:
                    return "<empty>";
                case OperatorType.ptrExpression:
                    return "<expression>";
                case OperatorType.Function:
                    return "<function>";
                case OperatorType.Range:
                    return "<range>";
                default:
                    return "NULL";
            }
        }
        internal static void Main()
        {
            NextGeneratedLabel = 1;
        }
 
    }

}