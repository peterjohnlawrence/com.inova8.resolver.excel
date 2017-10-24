
namespace com.inova8.resolver
{
    using System;
    using System.Collections.Generic;
    public class ResolverClass
    {
        private Int32 intLookAhead;
        private FormulaTokenClass pLookAhead;
        private FormulaTokenClass pEOFLookAhead = new FormulaTokenClass("", FormulaTokenClass.TokenType.Noop, FormulaTokenClass.TokenSubtype.Nothing);
        private ExcelFormulaClass pExcelFormula;
        private DependentsClass pDependents = new DependentsClass();
        private NamesClass pNames = new NamesClass();
        private CellsClass pCells;
        private ConstraintsClass pConstraints = new ConstraintsClass();
        private SolutionClass pSolution;

        public ResolverClass()
        {
            pCells = new CellsClass(pNames);
        }
        public int NumberCells
        {
            get { return pCells.Count; }
        }
        public double MeasurementCriticalValue
        {
            get { return pSolution.MeasurementCriticalValue; }
        }
        public double ConstraintCriticalValue
        {
            get { return pSolution.ConstraintCriticalValue; }
        }
        public double GlobalCriticalValue
        {
            get { return pSolution.GlobalCriticalValue; }
        }
        public double ReconciledCost
        {
            get {
                SolutionClass.ConvergenceClass pConvergence = pSolution.Convergence;
                return pConvergence.ReconciledCost;
            }
        }
        public int RedundancyDegree
        {
            get { return pSolution.RedundancyDegree; }
        }
        internal SolutionClass Solution
        {
            get { return pSolution; }
        }
        internal CellsClass Cells
        {
            get { return pCells; }
        }
        public ConstraintsClass Constraints
        {
            get { return pConstraints; }
        }
        public DependentsClass Dependents
        {
            get { return pDependents; }
        }
        public ResultsClass Results
        {
            get { return pSolution.Results; }
        }
        public SolutionClass.ConvergenceClass Convergence
        {
            get { return pSolution.Convergence; }
        }

        private bool advance()
        {
            if (pExcelFormula.Count > (intLookAhead + 1))
            {
                intLookAhead++;
                pLookAhead = pExcelFormula[intLookAhead];
            }
            else
            {
                intLookAhead++;
                pLookAhead = pEOFLookAhead;
            }
            return true;
        }
        private bool start()
        {
            intLookAhead = 0;
            pLookAhead = pExcelFormula[intLookAhead];
            return true;
        }
        public bool Resolve(int MaximumTime, int Iterations, double Precision, double Convergence)
        {

            pSolution = new SolutionClass();
            return pSolution.Resolve(this, MaximumTime, Iterations, Precision, Convergence);
        }
        private bool addNonNegativityConstraints(CellClass variable)
        {
            NodeClass constraint;
            constraint = new NodeClass();
            pConstraints.Add(constraint);
            constraint.OperatorType = Constants.OperatorType.GrtEqual;
            constraint.Address = variable.Name + Constants.OperatorString(Constants.OperatorType.GrtEqual)+ "0";

            constraint.Left = new NodeClass();
            constraint.Left.Operand = variable;
            constraint.Left.OperatorType = Constants.OperatorType.ptrVariable;

            constraint.Right = new NodeClass();
            constraint.Right.OperandString = "0.0";
            constraint.Right.OperatorType = Constants.OperatorType.Real;

            return true;
        }
        public bool addName(string Name, string Address)
        {
            pNames.Add(Address,Name);
            return true;
        }
        
        public bool addVariable(string label, string initial, string measurement, string tolerance, bool NonNegative )
        {
            CellClass variable;
            Nullable<double> init = null;
            Nullable<double> meas = null;
            Nullable<double> tol = null;
            if (initial.Trim().Length != 0) { init = double.Parse(initial.Trim()); }
            if (measurement.Trim().Length != 0) { meas = double.Parse(measurement.Trim()); }
            if (tolerance.Trim().Length != 0) { tol = double.Parse(tolerance.Trim()); }

            variable = Cells.AddMeasurement(pNames.Key(label), init, meas, tol);
            if (NonNegative && (variable != null))
            {
                addNonNegativityConstraints(variable);
            }

            return true;
        }
        public bool addDependent(string label, string Dependent)
        {
            string address= pNames.Key(label.Trim());

            pExcelFormula = new ExcelFormulaClass(Dependent);
            start();

            NodeClass expr = this.expression();
            pDependents.Add(expr);
            pCells.AddAddress(address, expr);
            expr.Address = pNames.Label(address);

            return true;
        }
        public bool addConstraint(string lblReference, string txtReference, string txtEquality, string lblConstraintReference, string txtConstraintReference)
        {

            NodeClass constraint;
            NodeClass expr;

            string lblReferenceTrimmed = pNames.Key(lblReference.Trim());  
            string txtEqualityTrimmed = txtEquality.Trim();
            string lblConstraintReferenceTrimmed = pNames.Key(lblConstraintReference.Trim()); 

            constraint = new NodeClass();
            pConstraints.Add(constraint);

            constraint.OperatorType =  Constants.StringEquality(txtEqualityTrimmed);

            constraint.Address = pNames.Label(lblReferenceTrimmed) + txtEqualityTrimmed + pNames.Label(lblConstraintReferenceTrimmed);

            pExcelFormula = new ExcelFormulaClass(txtReference);
            start();
            expr = expression();
            expr.Address = lblReferenceTrimmed;
            pDependents.Add(expr);

            constraint.Left = new NodeClass();
            constraint.Left.Operand = pCells.AddAddress(lblReferenceTrimmed, expr);
            constraint.Left.OperatorType = Constants.OperatorType.ptrExpression;

            pExcelFormula = new ExcelFormulaClass(txtConstraintReference);
            start();
            expr = expression();
            expr.Address = lblConstraintReferenceTrimmed;
            pDependents.Add(expr);

            constraint.Right = new NodeClass();
            constraint.Right.Operand = pCells.AddAddress(lblConstraintReferenceTrimmed, expr);
            constraint.Right.OperatorType = Constants.OperatorType.ptrExpression;

            return true;
        }
        private NodeClass expression()
        {
            //expression
            //  : sumExpr // boolExpr
            //  ;
            NodeClass expression;
            expression = sumExpr();
            return expression;
        }
        private NodeClass sumExpr()
        {
            //sumExpr
            //    : productExpr ((SUB | ADD)^ productExpr)*
            //    ;

            NodeClass sumExpr;
            sumExpr = productExpr();
            while (pLookAhead.Category == FormulaTokenClass.TokenCategory.Sum)
            {
                NodeClass leftNode;
                NodeClass rightNode;

                leftNode = sumExpr;
                sumExpr = new NodeClass();
                sumExpr.pToken = pLookAhead;
                if (pLookAhead.Value == Constants.OperatorString(Constants.OperatorType.Plus))
                {
                    sumExpr.OperatorType = Constants.OperatorType.Plus;
                }
                else if (pLookAhead.Value == Constants.OperatorString(Constants.OperatorType.Minus))
                {
                    sumExpr.OperatorType = Constants.OperatorType.Minus;
                }
                advance();
                rightNode = productExpr();
                sumExpr.Right = rightNode;
                sumExpr.Left = leftNode;
            }
            return sumExpr;
        }
        private NodeClass productExpr()
        {
            //productExpr
            //     : expExpr ((DIV | MULT)^ expExpr)*
            //     ;

            NodeClass productExpr;
            productExpr = expExpr();
            while (pLookAhead.Category == FormulaTokenClass.TokenCategory.Product)
            {
                NodeClass leftNode;
                NodeClass rightNode;

                leftNode = productExpr;
                productExpr = new NodeClass();
                productExpr.pToken = pLookAhead;
                if (pLookAhead.Value == Constants.OperatorString(Constants.OperatorType.Multiply))
                {
                    productExpr.OperatorType = Constants.OperatorType.Multiply;
                }
                else if (pLookAhead.Value == Constants.OperatorString(Constants.OperatorType.Divide))
                {
                    productExpr.OperatorType = Constants.OperatorType.Divide;
                }
                advance();
                rightNode = expExpr();
                productExpr.Right = rightNode;
                productExpr.Left = leftNode;
            }
            return productExpr;
        }
        private NodeClass expExpr()
        {
            //expExpr
            //    : unaryOperation (EXP^ unaryOperation)*
            //    ;

            NodeClass expExpr;
            expExpr = unaryOperation();
            while (pLookAhead.Category == FormulaTokenClass.TokenCategory.Expon)
            {
                NodeClass leftNode;
                NodeClass rightNode;

                leftNode = expExpr;
                expExpr = new NodeClass();
                expExpr.pToken = pLookAhead;
                expExpr.OperatorType = Constants.OperatorType.Expon;
                advance();
                rightNode = unaryOperation();
                expExpr.Right = rightNode;
                expExpr.Left = leftNode;
            }
            return expExpr;
        }
        private NodeClass unaryOperation()
        {
            // unaryOperation
            //     : NOT^ operand
            //     | ADD o=operand -> ^(POS $o)
            //     | SUB o=operand -> ^(NEG $o)
            //     | operand
            //     ;
            NodeClass unaryOperation;
            if (pLookAhead.Type == FormulaTokenClass.TokenType.OperatorPrefix)
            {
                unaryOperation = new NodeClass();
                unaryOperation.pToken = pLookAhead;
                if (pLookAhead.Value == Constants.OperatorString(Constants.OperatorType.UnaryPlus))
                {
                    unaryOperation.OperatorType = Constants.OperatorType.UnaryPlus;
                }
                else if (pLookAhead.Value == Constants.OperatorString(Constants.OperatorType.UnaryMinus))
                {
                    unaryOperation.OperatorType = Constants.OperatorType.UnaryMinus;
                }
                advance();
                unaryOperation.Left = operand();

            }
            else
            {
                unaryOperation = operand();
            }
            return unaryOperation;
        }
        private NodeClass operand()
        {
            //operand
            //    : literal 
            //    | functionExpr -> ^(CALL functionExpr)
            //    | percent
            //    | VARIABLE
            //    | LPAREN expression RPAREN -> ^(expression)
            //    ;

            NodeClass operand;

            if (pLookAhead.Type == FormulaTokenClass.TokenType.Operand && ((pLookAhead.Subtype == FormulaTokenClass.TokenSubtype.Number)))
            {
                operand = literal();
            }
            else if (pLookAhead.Type == FormulaTokenClass.TokenType.Function)
            {
                operand = functionExpr();
            }
            else if (pLookAhead.Type == FormulaTokenClass.TokenType.OperatorPostfix)
            {
                operand = percent(); 
            }
            else if (pLookAhead.Type == FormulaTokenClass.TokenType.Operand && ((pLookAhead.Subtype == FormulaTokenClass.TokenSubtype.Range)))
            {
                
                //detect multivariable ranges
                List<string> ranges = ExpandRange(pNames.Key(pLookAhead.Value));//pLookAhead.Value);

                if (ranges.Count == 1)
                {
                    operand = new NodeClass();
                    operand.pToken = pLookAhead;
                    operand.OperatorType = Constants.OperatorType.ptrVariable;

                    operand.Operand = pCells.AddAddress(ranges[0]);
                }
                else
                {
                    operand = new NodeClass();
                    operand.pToken = pLookAhead;
                    operand.OperatorType = Constants.OperatorType.Range;
                    
                    NodeClass addVariable = new NodeClass();
                    addVariable.pToken = null;
                    addVariable.OperatorType = Constants.OperatorType.ptrVariable;
                    addVariable.Operand = pCells.AddAddress(ranges[0]);
                    operand.Left = addVariable;
                    
                    NodeClass listNode = operand;
                    for(int index =1; index < ranges.Count; index++)
                    {
                        string range = ranges[index];
                        NodeClass list = new NodeClass();
                        list.pToken = null;
                        list.OperatorType = Constants.OperatorType.ListSeparator;

                        listNode.Right = list;

                        addVariable = new NodeClass();
                        addVariable.pToken = null;
                        addVariable.OperatorType = Constants.OperatorType.ptrVariable;
                        addVariable.Operand = pCells.AddAddress(range);
                        list.Left = addVariable;

                        listNode = list;
                    }
                    listNode.Right = null;
                }

                advance();
            }
            else if (pLookAhead.Type == FormulaTokenClass.TokenType.Subexpression)
            {
                advance();
                operand = expression();
                advance();
            }
            else
            {
                operand = null;
            }
            return operand;
        }
        private NodeClass functionExpr()
        {
            //functionExpr
            //    : FUNCNAME LPAREN! (expression (COMMA! expression)*)? RPAREN!
            //    ;

            NodeClass functionExpr;
            NodeClass functionArg;
            Constants.FunctionType function = Constants.StringFunction(pLookAhead.Value); ;

            functionExpr = new NodeClass();
            functionExpr.pToken = pLookAhead;
            functionExpr.OperatorType = Constants.OperatorType.Function; // Check for string or boolean
            functionExpr.OperandString = pLookAhead.Value;
            functionExpr.Function = function;
            advance();
            if (pLookAhead.Subtype != FormulaTokenClass.TokenSubtype.Stop) //at least it has arguments
            {
                functionExpr.Left = expression();
                while (pLookAhead.Subtype != FormulaTokenClass.TokenSubtype.Stop)
                {
                    //advance();
                    functionArg = new NodeClass();
                    functionArg.pToken = pLookAhead;
                    functionArg.OperatorType = Constants.OperatorType.ListSeparator; // Check for string or boolean
                    functionArg.OperandString = pLookAhead.Value;
                    functionExpr.Right = functionArg;
                    advance();
                    functionArg.Left = expression();
                    functionArg.Right = null; //assume it is the end of the list
                }
                advance();
            }
            return functionExpr;
        }
        private NodeClass literal()
        {
            //literal
            //    : NUMBER 
            //    | STRING 
            //    | TRUE
            //    | FALSE
            //    ;

            NodeClass literal;
            literal = new NodeClass();
            literal.pToken = pLookAhead;
            literal.OperatorType = Constants.OperatorType.Real; // Check for string or boolean
            literal.OperandString = pLookAhead.Value;
            advance();
            return literal;
        }

        private NodeClass percent()
        {
            //percent
            //    : NUMBER PERCENT^
            //    ;

            NodeClass percent;
            percent = new NodeClass();
            percent.pToken = pLookAhead;
            percent.OperatorType = Constants.OperatorType.Real; // Check for string or boolean
            percent.OperandString = pLookAhead.Value;
            advance();
            return percent;
        }
        private static List<string> ExpandRange(string value)
        {
            List<string> pRange = new List<string>();
            string[] pLimits = new string[2];
            string[] pCells = new string[2];
            int pStartRow;
            int pStartCol;
            int pEndRow;
            int pEndCol;


            if (value.Contains(":"))
            {
                pLimits = value.Split(':');
                pCells = pLimits[0].Split('$');
                pStartRow = Convert.ToInt16(pCells[2]);
                pStartCol = ConvertToNumber(pCells[1]);
                pCells = pLimits[1].Split('$');
                pEndRow = Convert.ToInt16(pCells[2]);
                pEndCol = ConvertToNumber(pCells[1]);

                for (int pRow = pStartRow; pRow <= pEndRow; pRow++)
                {
                    for (int pCol = pStartCol; pCol <= pEndCol; pCol++)
                    {
                        pRange.Add("$" + ConvertToLetter(pCol) + "$" + Convert.ToString(pRow));
                    }
                }
            }
            else
            {
                pRange.Add(value);
            }
            return pRange;
        }
        private static string ConvertToLetter(int iCol)
        {
            string pConvertToLetter = "";
            int iAlpha = Convert.ToInt32(Math.Floor(iCol / 27.0));
            int iRemainder = iCol - (iAlpha * 26);
            if (iAlpha > 0)
            {
                pConvertToLetter += Convert.ToChar(iAlpha + 64);
            }
            if (iRemainder > 0)
            {
                pConvertToLetter = pConvertToLetter + Convert.ToChar(iRemainder + 64);
            }
            return pConvertToLetter;
        }
        private static int ConvertToNumber(string iCol)
        {
            string sChars = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int pConvertToNumber;
            pConvertToNumber = 1;

            for (int i=0; i<iCol.Length; i++)
            {
                pConvertToNumber *= sChars.IndexOf(iCol[i]);
            } 
            return pConvertToNumber;
        }
        public Nullable<double> VariableSensitivity(ResultClass variable, ResultClass measurement)
        {
            return pSolution.VariableSensitivity(variable, measurement);
        }
        public Nullable<double> ConstraintSensitivity(ConstraintClass constraint, ResultClass measurement)
        {
            return pSolution.ConstraintSensitivity(constraint, measurement);
        }
    }
}