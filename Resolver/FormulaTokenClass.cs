namespace com.inova8.resolver
{
    using System;

    public class FormulaTokenClass
    {

        private string value;
        private TokenType type;
        private TokenSubtype subtype;
        
        internal FormulaTokenClass(string value, TokenType type) : this(value, type, TokenSubtype.Nothing) { }

        internal FormulaTokenClass(string value, TokenType type, TokenSubtype subtype)
        {
            this.value = value;
            this.type = type;
            this.subtype = subtype;
        }

        public string Value
        {
            get { return value; }
            internal set { this.value = value; }
        }

        public TokenType Type
        {
            get { return type; }
            internal set { type = value; }
        }

        public TokenSubtype Subtype
        {
            get { return subtype; }
            internal set { subtype = value; }
        }
        internal TokenCategory Category
        {
            get
            {
                if (Type == TokenType.OperatorInfix)
                {
                    if (Subtype == TokenSubtype.Math)
                    {
                        if (value == "*" || value == "/")
                        {
                            return TokenCategory.Product;
                        }
                        else if (value == "+" || value == "-")
                        {
                            return TokenCategory.Sum;
                        }
                        else if (value == "^")
                        {
                            return TokenCategory.Expon;
                        }
                        else
                        {
                            return TokenCategory.Nothing;
                        }
                    }
                    else
                    {
                        return TokenCategory.Nothing;
                    }
                }
                else if (Type == TokenType.OperatorPrefix)
                {
                    if (value == "+" || value == "-" || value == "!")
                    {
                        return TokenCategory.Sum;
                    }
                    else
                    {
                        return TokenCategory.Nothing;
                    }
                }
                else
                {
                    return TokenCategory.Nothing;
                }
            }
        }

        public enum TokenType
        {
            Noop,
            Operand,            // x y z 27 abc
            Function,           // fnc
            Subexpression,      // ()
            Argument,           // ,
            OperatorPrefix,     // + -
            OperatorInfix,      // + - * / ^ & = > <
            OperatorPostfix,    // %
            Whitespace,         // 
            Unknown
        };

        public enum TokenSubtype
        {
            Nothing,
            Start,              // (
            Stop,               // )
            Text,               // abc
            Number,             // 27
            Logical,            // true false
            Error,
            Range,              // x y z
            Math,               // + - * / ^
            Concatenation,      // &
            Intersection,       //
            Union               //
        };
        public enum TokenCategory
        {
            Nothing,
            Product,
            Sum,
            Expon
        };
    }
}
