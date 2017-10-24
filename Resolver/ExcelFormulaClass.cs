/*

Copyright (c) 2007 E. W. Bachtal, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial 
  portions of the Software.

The software is provided "as is", without warranty of any kind, express or implied, including but not 
limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In 
no event shall the authors or copyright holders be liable for any claim, damages or other liability, 
whether in an action of contract, tort or otherwise, arising from, out of or in connection with the 
software or the use or other dealings in the software. 

http://ewbi.blogs.com/develops/2007/03/excel_formula_p.html
http://ewbi.blogs.com/develops/2004/12/excel_formula_p.html

v1.0  Original
v1.1  Added support for in-formula scientific notation.

*/

namespace com.inova8.resolver
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class ExcelFormulaClass : IList<FormulaTokenClass>
    {

        string formula;
        List<FormulaTokenClass> tokens;

        internal ExcelFormulaClass(string formula)
        {
            if (formula == null) throw new ArgumentNullException("formula");
            this.formula = formula.Trim();
            tokens = new List<FormulaTokenClass>();
            ParseToTokens();
        }

        internal string Formula
        {
            get { return formula; }
        }

        public FormulaTokenClass this[int index]
        {
            get
            {
                return tokens[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public int IndexOf(FormulaTokenClass item)
        {
            return tokens.IndexOf(item);
        }

        public void Insert(int index, FormulaTokenClass item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void Add(FormulaTokenClass item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(FormulaTokenClass item)
        {
            return tokens.Contains(item);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(FormulaTokenClass item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(FormulaTokenClass[] array, int arrayIndex)
        {
            //tokens.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return tokens.Count; }
        }

        public IEnumerator<FormulaTokenClass> GetEnumerator()
        {
            return tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tokens.GetEnumerator();
        }

        internal int ArrayIndexOf(string[] a, string v)
        {
            int indexOf = -1;
            foreach (string arrayElement in a)
            {
                indexOf++;
                if (v.Equals(arrayElement)) return indexOf;
            }
            return -1;
        }
         
        private void ParseToTokens()
        {

            // No attempt is made to verify formulas; assumes formulas are derived from Excel, where 
            // they can only exist if valid; stack overflows/underflows sunk as nulls without exceptions.

            if ((formula.Length < 2) || (formula[0] != '=')) return;

            ExcelFormulaTokens tokens1 = new ExcelFormulaTokens();
            ExcelFormulaStack stack = new ExcelFormulaStack();

            const char QUOTE_DOUBLE = '"';
            const char QUOTE_SINGLE = '\'';
            const char BRACKET_CLOSE = ']';
            const char BRACKET_OPEN = '[';
            const char BRACE_OPEN = '{';
            const char BRACE_CLOSE = '}';
            const char PAREN_OPEN = '(';
            const char PAREN_CLOSE = ')';
            const char SEMICOLON = ';';
            const char WHITESPACE = ' ';
            const char COMMA = ',';
            const char ERROR_START = '#';

            const string OPERATORS_SN = "+-";
            const string OPERATORS_INFIX = "+-*/^&=><";
            const string OPERATORS_POSTFIX = "%";

            string[] ERRORS = new string[] { "#NULL!", "#DIV/0!", "#VALUE!", "#REF!", "#NAME?", "#NUM!", "#N/A" };

            string[] COMPARATORS_MULTI = new string[] { ">=", "<=", "<>" };

            bool inString = false;
            bool inPath = false;
            bool inRange = false;
            bool inError = false;

            int index = 1;
            string value = "";

            while (index < formula.Length)
            {

                // state-dependent character evaluation (order is important)

                // double-quoted strings
                // embeds are doubled
                // end marks token

                if (inString)
                {
                    if (formula[index] == QUOTE_DOUBLE)
                    {
                        if (((index + 2) <= formula.Length) && (formula[index + 1] == QUOTE_DOUBLE))
                        {
                            value += QUOTE_DOUBLE;
                            index++;
                        }
                        else
                        {
                            inString = false;
                            tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand, FormulaTokenClass.TokenSubtype.Text));
                            value = "";
                        }
                    }
                    else
                    {
                        value += formula[index];
                    }
                    index++;
                    continue;
                }

                // single-quoted strings (links)
                // embeds are double
                // end does not mark a token

                if (inPath)
                {
                    if (formula[index] == QUOTE_SINGLE)
                    {
                        if (((index + 2) <= formula.Length) && (formula[index + 1] == QUOTE_SINGLE))
                        {
                            value += QUOTE_SINGLE;
                            index++;
                        }
                        else
                        {
                            inPath = false;
                        }
                    }
                    else
                    {
                        value += formula[index];
                    }
                    index++;
                    continue;
                }

                // bracked strings (R1C1 range index or linked workbook name)
                // no embeds (changed to "()" by Excel)
                // end does not mark a token

                if (inRange)
                {
                    if (formula[index] == BRACKET_CLOSE)
                    {
                        inRange = false;
                    }
                    value += formula[index];
                    index++;
                    continue;
                }

                // error values
                // end marks a token, determined from absolute list of values

                if (inError)
                {
                    value += formula[index];
                    index++;
                    //if (Array.IndexOf(ERRORS, value) != -1)
                    if (ArrayIndexOf(ERRORS, value) != -1)
                    {
                        inError = false;
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand, FormulaTokenClass.TokenSubtype.Error));
                        value = "";
                    }
                    continue;
                }

                // scientific notation check

                if ((OPERATORS_SN).IndexOf(formula[index]) != -1)
                {
                    if (value.Length > 1)
                    {
                        if (Regex.IsMatch(value, @"^[1-9]{1}(\.[0-9]+)?E{1}$"))
                        {
                            value += formula[index];
                            index++;
                            continue;
                        }
                    }
                }

                // independent character evaluation (order not important)

                // establish state-dependent character evaluations

                if (formula[index] == QUOTE_DOUBLE)
                {
                    if (value.Length > 0)
                    {  // unexpected
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Unknown));
                        value = "";
                    }
                    inString = true;
                    index++;
                    continue;
                }

                if (formula[index] == QUOTE_SINGLE)
                {
                    if (value.Length > 0)
                    { // unexpected
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Unknown));
                        value = "";
                    }
                    inPath = true;
                    index++;
                    continue;
                }

                if (formula[index] == BRACKET_OPEN)
                {
                    inRange = true;
                    value += BRACKET_OPEN;
                    index++;
                    continue;
                }

                if (formula[index] == ERROR_START)
                {
                    if (value.Length > 0)
                    { // unexpected
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Unknown));
                        value = "";
                    }
                    inError = true;
                    value += ERROR_START;
                    index++;
                    continue;
                }

                // mark start and end of arrays and array rows

                if (formula[index] == BRACE_OPEN)
                {
                    if (value.Length > 0)
                    { // unexpected
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Unknown));
                        value = "";
                    }
                    stack.Push(tokens1.Add(new FormulaTokenClass("ARRAY", FormulaTokenClass.TokenType.Function, FormulaTokenClass.TokenSubtype.Start)));
                    stack.Push(tokens1.Add(new FormulaTokenClass("ARRAYROW", FormulaTokenClass.TokenType.Function, FormulaTokenClass.TokenSubtype.Start)));
                    index++;
                    continue;
                }

                if (formula[index] == SEMICOLON)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(stack.Pop());
                    tokens1.Add(new FormulaTokenClass(",", FormulaTokenClass.TokenType.Argument));
                    stack.Push(tokens1.Add(new FormulaTokenClass("ARRAYROW", FormulaTokenClass.TokenType.Function, FormulaTokenClass.TokenSubtype.Start)));
                    index++;
                    continue;
                }

                if (formula[index] == BRACE_CLOSE)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(stack.Pop());
                    tokens1.Add(stack.Pop());
                    index++;
                    continue;
                }

                // trim white-space

                if (formula[index] == WHITESPACE)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(new FormulaTokenClass("", FormulaTokenClass.TokenType.Whitespace));
                    index++;
                    while ((formula[index] == WHITESPACE) && (index < formula.Length))
                    {
                        index++;
                    }
                    continue;
                }

                // multi-character comparators

                if ((index + 2) <= formula.Length)
                {
                    //if (Array.IndexOf(COMPARATORS_MULTI, formula.Substring(index, 2)) != -1)
                    if (ArrayIndexOf(COMPARATORS_MULTI, formula.Substring(index, 2)) != -1)
                    {
                        if (value.Length > 0)
                        {
                            tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand));
                            value = "";
                        }
                        tokens1.Add(new FormulaTokenClass(formula.Substring(index, 2), FormulaTokenClass.TokenType.OperatorInfix, FormulaTokenClass.TokenSubtype.Logical));
                        index += 2;
                        continue;
                    }
                }

                // standard infix operators

                if ((OPERATORS_INFIX).IndexOf(formula[index]) != -1)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(new FormulaTokenClass(formula[index].ToString(), FormulaTokenClass.TokenType.OperatorInfix));
                    index++;
                    continue;
                }

                // standard postfix operators (only one)

                if ((OPERATORS_POSTFIX).IndexOf(formula[index]) != -1)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(new FormulaTokenClass(formula[index].ToString(), FormulaTokenClass.TokenType.OperatorPostfix));
                    index++;
                    continue;
                }

                // start subexpression or function

                if (formula[index] == PAREN_OPEN)
                {
                    if (value.Length > 0)
                    {
                        stack.Push(tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Function, FormulaTokenClass.TokenSubtype.Start)));
                        value = "";
                    }
                    else
                    {
                        stack.Push(tokens1.Add(new FormulaTokenClass("", FormulaTokenClass.TokenType.Subexpression, FormulaTokenClass.TokenSubtype.Start)));
                    }
                    index++;
                    continue;
                }

                // function, subexpression, or array parameters, or operand unions

                if (formula[index] == COMMA)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand));
                        value = "";
                    }
                    if (stack.Current.Type != FormulaTokenClass.TokenType.Function)
                    {
                        tokens1.Add(new FormulaTokenClass(",", FormulaTokenClass.TokenType.OperatorInfix, FormulaTokenClass.TokenSubtype.Union));
                    }
                    else
                    {
                        tokens1.Add(new FormulaTokenClass(",", FormulaTokenClass.TokenType.Argument));
                    }
                    index++;
                    continue;
                }

                // stop subexpression

                if (formula[index] == PAREN_CLOSE)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(stack.Pop());
                    index++;
                    continue;
                }

                // token accumulation

                value += formula[index];
                index++;

            }

            // dump remaining accumulation

            if (value.Length > 0)
            {
                tokens1.Add(new FormulaTokenClass(value, FormulaTokenClass.TokenType.Operand));
            }

            // move tokenList to new set, excluding unnecessary white-space tokens and converting necessary ones to intersections

            ExcelFormulaTokens tokens2 = new ExcelFormulaTokens(tokens1.Count);

            while (tokens1.MoveNext())
            {

                FormulaTokenClass token = tokens1.Current;

                if (token == null) continue;

                if (token.Type != FormulaTokenClass.TokenType.Whitespace)
                {
                    tokens2.Add(token);
                    continue;
                }

                if ((tokens1.BOF) || (tokens1.EOF)) continue;

                FormulaTokenClass previous = tokens1.Previous;

                if (previous == null) continue;

                if (!(
                      ((previous.Type == FormulaTokenClass.TokenType.Function) && (previous.Subtype == FormulaTokenClass.TokenSubtype.Stop)) ||
                      ((previous.Type == FormulaTokenClass.TokenType.Subexpression) && (previous.Subtype == FormulaTokenClass.TokenSubtype.Stop)) ||
                      (previous.Type == FormulaTokenClass.TokenType.Operand)
                      )
                    ) continue;

                FormulaTokenClass next = tokens1.Next;

                if (next == null) continue;

                if (!(
                      ((next.Type == FormulaTokenClass.TokenType.Function) && (next.Subtype == FormulaTokenClass.TokenSubtype.Start)) ||
                      ((next.Type == FormulaTokenClass.TokenType.Subexpression) && (next.Subtype == FormulaTokenClass.TokenSubtype.Start)) ||
                      (next.Type == FormulaTokenClass.TokenType.Operand)
                      )
                    ) continue;

                tokens2.Add(new FormulaTokenClass("", FormulaTokenClass.TokenType.OperatorInfix, FormulaTokenClass.TokenSubtype.Intersection));

            }

            // move tokens to final list, switching infix "-" operators to prefix when appropriate, switching infix "+" operators 
            // to noop when appropriate, identifying operand and infix-operator subtypes, and pulling "@" from function names

            tokens = new List<FormulaTokenClass>(tokens2.Count);

            while (tokens2.MoveNext())
            {

                FormulaTokenClass token = tokens2.Current;

                if (token == null) continue;

                FormulaTokenClass previous = tokens2.Previous;
                FormulaTokenClass next = tokens2.Next;

                if ((token.Type == FormulaTokenClass.TokenType.OperatorInfix) && (token.Value == "-"))
                {
                    if (tokens2.BOF)
                        token.Type = FormulaTokenClass.TokenType.OperatorPrefix;
                    else if (
                            ((previous.Type == FormulaTokenClass.TokenType.Function) && (previous.Subtype == FormulaTokenClass.TokenSubtype.Stop)) ||
                            ((previous.Type == FormulaTokenClass.TokenType.Subexpression) && (previous.Subtype == FormulaTokenClass.TokenSubtype.Stop)) ||
                            (previous.Type == FormulaTokenClass.TokenType.OperatorPostfix) ||
                            (previous.Type == FormulaTokenClass.TokenType.Operand)
                            )
                        token.Subtype = FormulaTokenClass.TokenSubtype.Math;
                    else
                        token.Type = FormulaTokenClass.TokenType.OperatorPrefix;

                    tokens.Add(token);
                    continue;
                }

                if ((token.Type == FormulaTokenClass.TokenType.OperatorInfix) && (token.Value == "+"))
                {
                    if (tokens2.BOF)
                        continue;
                    else if (
                            ((previous.Type == FormulaTokenClass.TokenType.Function) && (previous.Subtype == FormulaTokenClass.TokenSubtype.Stop)) ||
                            ((previous.Type == FormulaTokenClass.TokenType.Subexpression) && (previous.Subtype == FormulaTokenClass.TokenSubtype.Stop)) ||
                            (previous.Type == FormulaTokenClass.TokenType.OperatorPostfix) ||
                            (previous.Type == FormulaTokenClass.TokenType.Operand)
                            )
                        token.Subtype = FormulaTokenClass.TokenSubtype.Math;
                    else
                        continue;

                    tokens.Add(token);
                    continue;
                }

                if ((token.Type == FormulaTokenClass.TokenType.OperatorInfix) && (token.Subtype == FormulaTokenClass.TokenSubtype.Nothing))
                {
                    if (("<>=").IndexOf(token.Value.Substring(0, 1)) != -1)
                        token.Subtype = FormulaTokenClass.TokenSubtype.Logical;
                    else if (token.Value == "&")
                        token.Subtype = FormulaTokenClass.TokenSubtype.Concatenation;
                    else
                        token.Subtype = FormulaTokenClass.TokenSubtype.Math;

                    tokens.Add(token);
                    continue;
                }

                if ((token.Type == FormulaTokenClass.TokenType.Operand) && (token.Subtype == FormulaTokenClass.TokenSubtype.Nothing))
                {
                    double d;
                    bool isNumber = double.TryParse(token.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out d);
                    if (!isNumber)
                        if ((token.Value == "TRUE") || (token.Value == "FALSE"))
                            token.Subtype = FormulaTokenClass.TokenSubtype.Logical;
                        else
                            token.Subtype = FormulaTokenClass.TokenSubtype.Range;
                    else
                        token.Subtype = FormulaTokenClass.TokenSubtype.Number;

                    tokens.Add(token);
                    continue;
                }

                if (token.Type == FormulaTokenClass.TokenType.Function)
                {
                    if (token.Value.Length > 0)
                    {
                        if (token.Value.Substring(0, 1) == "@")
                        {
                            token.Value = token.Value.Substring(1);
                        }
                    }
                }

                tokens.Add(token);

            }

        }

        internal class ExcelFormulaTokens
        {

            private int index = -1;
            private List<FormulaTokenClass> tokens;

            internal ExcelFormulaTokens() : this(4) { }

            internal ExcelFormulaTokens(int capacity)
            {
                tokens = new List<FormulaTokenClass>(capacity);
            }

            internal int Count
            {
                get { return tokens.Count; }
            }

            internal bool BOF
            {
                get { return (index <= 0); }
            }

            internal bool EOF
            {
                get { return (index >= (tokens.Count - 1)); }
            }

            internal FormulaTokenClass Current
            {
                get
                {
                    if (index == -1) return null;
                    return tokens[index];
                }
            }

            internal FormulaTokenClass Next
            {
                get
                {
                    if (EOF) return null;
                    return tokens[index + 1];
                }
            }

            internal FormulaTokenClass Previous
            {
                get
                {
                    if (index < 1) return null;
                    return tokens[index - 1];
                }
            }

            internal FormulaTokenClass Add(FormulaTokenClass token)
            {
                tokens.Add(token);
                return token;
            }

            internal bool MoveNext()
            {
                if (EOF) return false;
                index++;
                return true;
            }

            internal void Reset()
            {
                index = -1;
            }

        }

        internal class ExcelFormulaStack
        {

            private Stack<FormulaTokenClass> stack = new Stack<FormulaTokenClass>();

            internal ExcelFormulaStack() { }

            internal void Push(FormulaTokenClass token)
            {
                stack.Push(token);
            }

            internal FormulaTokenClass Pop()
            {
                if (stack.Count == 0) return null;
                return new FormulaTokenClass("", stack.Pop().Type, FormulaTokenClass.TokenSubtype.Stop);
            }

            internal FormulaTokenClass Current
            {
                get { return (stack.Count > 0) ? stack.Peek() : null; }
            }

        }

    }

}
