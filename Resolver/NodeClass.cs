
namespace com.inova8.resolver
{
    using System;
    public class NodeClass
    {
        internal FormulaTokenClass pToken;
        private System.String pAddress = "";
        private NodeClass pLeft = null;
        private NodeClass pRight = null;
        private CellClass pOperand = null;
        private System.String strOperand = "";
        private Constants.OperatorType pOperatorType = Constants.OperatorType.NullType;
        private Constants.LinearityType pLinearity = Constants.LinearityType.Unknown;
        private Constants.FunctionType pFunction = Constants.FunctionType.NULL;
        //protected Constants.FunctionType lastFunctionContext = Constants.FunctionType.NULL;
        private int pLinearOperatorCount;
        private int pNonlinearOperatorCount;
        private int pVariableCount;
        private UserFunctions userFunctions = new UserFunctions();
        internal Constants.LinearityType Linearity
        {
            get
            {
                Constants.LinearityType getLinearity = 0;
                //LinearType
                if (pLinearity == Constants.LinearityType.Unknown)
                {
                    pLinearity = DeduceLinearity();
                }
                getLinearity = pLinearity;
                return getLinearity;
            }
        }
        internal int LinearOperatorCount
        {
            get { return pLinearOperatorCount; }
        }
        internal int NonlinearOperatorCount
        {
            get { return pNonlinearOperatorCount; }
        }
        internal int VariableCount
        {
            get { return pVariableCount; }
        }
        internal System.String Address
        {
            get { return pAddress; }
            set { pAddress = value; }

        }
        internal NodeClass Left
        {
            get { return pLeft; }
            set { pLeft = value; }

        }
        internal NodeClass Right
        {
            get { return pRight; }
            set { pRight = value; }

        }
        internal CellClass Operand
        {
            get { return pOperand; }
            set { pOperand = value; }

        }
        internal System.String OperandString
        {
            get { return System.Convert.ToString(pOperand); }
            set { strOperand = value; }

        }
        internal Constants.OperatorType OperatorType
        {
            get { return pOperatorType; }
            set { pOperatorType = value; }
        }
        internal Constants.FunctionType Function
        {
            get { return pFunction; }
            set { pFunction = value; }
        }
        public NodeClass()
        {
            pAddress = System.Convert.ToString(-Constants.NextGeneratedLabel);
            Constants.NextGeneratedLabel = Constants.NextGeneratedLabel + 1;
            pRight = null;
            pLeft = null;
            pOperand = null;
            pOperatorType = Constants.OperatorType.NullType;// 0;
            pLinearity = Constants.LinearityType.Unknown;
        }
        internal virtual double Evaluate(Constants.FunctionType functionContext)
        {
            double Evaluate = 0.0;
            double dLeftResidual = 0.0;
            double dRightResidual = 0.0;
            CellClass tVariable = null;
            Constants.FunctionType lastFunctionContext = Constants.FunctionType.NULL;

            //virtual void pushFunctionContext(){
            //}
            //virtual void popFunctionContext(){
            //}

            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    Evaluate = System.Double.Parse(strOperand);
                    return Evaluate;
                case Constants.OperatorType.ptrVariable:
                    tVariable = pOperand;
                    Evaluate = tVariable.ReconciledVariable.Value;
                    return Evaluate;
                case Constants.OperatorType.ptrExpression:
                    tVariable = pOperand;
                    Evaluate = tVariable.Node.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.UnaryMinus:
                    Evaluate = -pLeft.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.UnaryPlus:
                    Evaluate = pLeft.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.Plus:
                    Evaluate = pLeft.Evaluate(functionContext) + pRight.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.Minus:
                    Evaluate = pLeft.Evaluate(functionContext) - pRight.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.Multiply:
                    Evaluate = pLeft.Evaluate(functionContext) * pRight.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.Divide:
                    dLeftResidual = pLeft.Evaluate(functionContext);
                    dRightResidual = pRight.Evaluate(functionContext);
                    if (System.Math.Abs(dRightResidual) > Constants.ETA)
                    {
                        Evaluate = dLeftResidual / dRightResidual;
                        return Evaluate;
                    }
                    else
                    {
                        Evaluate = Constants.LARGEVALUE * Math.Sign(Math.Sign(dLeftResidual) * Math.Sign(dRightResidual));
                        return Evaluate;
                    }
                case Constants.OperatorType.Expon:
                    Evaluate = System.Math.Pow(pLeft.Evaluate(functionContext), pRight.Evaluate(functionContext));
                    return Evaluate;
                case Constants.OperatorType.Equal:
                    Evaluate = pLeft.Evaluate(functionContext) - pRight.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.Grt:
                    Evaluate = pLeft.Evaluate(functionContext) - pRight.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.GrtEqual:
                    Evaluate = pLeft.Evaluate(functionContext) - pRight.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.Less:
                    Evaluate = -pLeft.Evaluate(functionContext) + pRight.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.LessEqual:
                    Evaluate = -pLeft.Evaluate(functionContext) + pRight.Evaluate(functionContext);
                    return Evaluate;
                case Constants.OperatorType.Range:
                case Constants.OperatorType.ListSeparator:
                    switch (functionContext)
                    {
                        case Constants.FunctionType.SUM:
                            Evaluate = userFunctions.SUM_Evaluate(this, functionContext);
                            return Evaluate;
                        case Constants.FunctionType.PRODUCT:
                            Evaluate = userFunctions.PRODUCT_Evaluate(this, functionContext);
                            return Evaluate;
                        case Constants.FunctionType.USERDEF:
                            Evaluate = userFunctions.USERDEF_Evaluate(this, functionContext);
                            return Evaluate;
                        default:
                            Evaluate = -12121212;
                            return Evaluate;
                    }
                case Constants.OperatorType.Function:
                    lastFunctionContext = functionContext;
                    functionContext = this.Function;
                    switch (functionContext)
                    {
                        case Constants.FunctionType.SUM:
                            Evaluate = userFunctions.SUM_Evaluate(this, functionContext);
                            functionContext = lastFunctionContext;
                            return Evaluate;
                        case Constants.FunctionType.PRODUCT:
                            Evaluate = userFunctions.PRODUCT_Evaluate(this, functionContext);
                            functionContext = lastFunctionContext;
                            return Evaluate;
                        case Constants.FunctionType.USERDEF:
                            Evaluate = userFunctions.USERDEF_Evaluate(this, functionContext);
                            functionContext = lastFunctionContext;
                            return Evaluate;
                        default:
                            Evaluate = -12121212;
                            functionContext = lastFunctionContext;
                            return Evaluate;
                    }

                default:
                    return -1212121212;
            }
        }
        internal virtual double EvaluateResidual(Constants.FunctionType functionContext)
        {
            double EvaluateResidual = 0.0;
            double dLeftResidual = 0.0;
            double dRightResidual = 0.0;
            CellClass tVariable = null;
            Constants.FunctionType lastFunctionContext = Constants.FunctionType.NULL;

            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    EvaluateResidual = System.Double.Parse(strOperand);
                    return EvaluateResidual;
                case Constants.OperatorType.ptrVariable:
                    tVariable = pOperand;
                    if (tVariable.hasMeasurement)
                    {
                        //EvaluateResidual = tVariable.Expression.Left.EvaluateResidual 'removed by PJL to ensure that residual errors reflect edits
                        EvaluateResidual = tVariable.ReconciledVariable.InitialValue.Value;
                    }
                    else
                    {
                        //if (tVariable.ReconciledVariable.MeasurementTolerance > (SolutionClass.REC_LARGEVALUE / 10))
                        if (!tVariable.ReconciledVariable.Cell.hasMeasurement)
                        {
                            EvaluateResidual = tVariable.ReconciledVariable.Value;
                        }
                        else
                        {
                            EvaluateResidual = tVariable.ReconciledVariable.InitialValue.Value;
                        }
                    }
                    return EvaluateResidual;
                case Constants.OperatorType.ptrExpression:
                    return pOperand.Node.EvaluateResidual(functionContext);
                case Constants.OperatorType.UnaryMinus:
                    EvaluateResidual = -pLeft.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.UnaryPlus:
                    EvaluateResidual = pLeft.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.Plus:
                    EvaluateResidual = pLeft.EvaluateResidual(functionContext) + pRight.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.Minus:
                    EvaluateResidual = pLeft.EvaluateResidual(functionContext) - pRight.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.Multiply:
                    EvaluateResidual = pLeft.EvaluateResidual(functionContext) * pRight.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.Divide:
                    dLeftResidual = pLeft.EvaluateResidual(functionContext);
                    dRightResidual = pRight.EvaluateResidual(functionContext);
                    if (System.Math.Abs(dRightResidual) > Constants.ETA)
                    {
                        EvaluateResidual = dLeftResidual / dRightResidual;
                        return EvaluateResidual;
                    }
                    else
                    {
                        EvaluateResidual = Constants.LARGEVALUE * Math.Sign(Math.Sign(dLeftResidual) * Math.Sign(dRightResidual));
                        return EvaluateResidual;
                    }
                case Constants.OperatorType.Expon:
                    EvaluateResidual = System.Math.Pow(pLeft.EvaluateResidual(functionContext), pRight.EvaluateResidual(functionContext));
                    return EvaluateResidual;
                case Constants.OperatorType.Equal:
                    EvaluateResidual = pLeft.EvaluateResidual(functionContext) - pRight.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.Grt:
                    EvaluateResidual = pLeft.EvaluateResidual(functionContext) - pRight.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.GrtEqual:
                    EvaluateResidual = pLeft.EvaluateResidual(functionContext) - pRight.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.Less:
                    EvaluateResidual = -pLeft.EvaluateResidual(functionContext) + pRight.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.LessEqual:
                    EvaluateResidual = -pLeft.EvaluateResidual(functionContext) + pRight.EvaluateResidual(functionContext);
                    return EvaluateResidual;
                case Constants.OperatorType.Range:
                case Constants.OperatorType.ListSeparator:
                    switch (functionContext)
                    {
                        case Constants.FunctionType.SUM:
                            EvaluateResidual = userFunctions.SUM_EvaluateResidual(this, functionContext);  
                            return EvaluateResidual;
                        case Constants.FunctionType.PRODUCT:
                            EvaluateResidual = userFunctions.PRODUCT_EvaluateResidual(this, functionContext);
                            return EvaluateResidual;
                        case Constants.FunctionType.USERDEF:
                            EvaluateResidual = userFunctions.USERDEF_EvaluateResidual(this, functionContext);
                            return EvaluateResidual;
                        default:
                            EvaluateResidual = -12121212;
                            return EvaluateResidual;
                    }
                case Constants.OperatorType.Function:
                    lastFunctionContext = functionContext;
                    functionContext = this.Function;
                    switch (functionContext)
                    {
                        case Constants.FunctionType.SUM:
                            EvaluateResidual = userFunctions.SUM_EvaluateResidual(this, functionContext);  
                            functionContext = lastFunctionContext;
                            return EvaluateResidual;
                        case Constants.FunctionType.PRODUCT:
                            EvaluateResidual = userFunctions.PRODUCT_EvaluateResidual(this, functionContext);  
                            functionContext = lastFunctionContext;
                            return EvaluateResidual;
                        case Constants.FunctionType.USERDEF:
                            EvaluateResidual = userFunctions.USERDEF_EvaluateResidual(this, functionContext);
                           functionContext = lastFunctionContext;
                           return EvaluateResidual;
                        default:
                            EvaluateResidual = -12121212;
                            functionContext = lastFunctionContext;
                            return EvaluateResidual;
                    }
                default:
                    EvaluateResidual = -12121212;
                    return EvaluateResidual;
            }
         }
        internal virtual double Derivative(CellClass wrt, Constants.FunctionType functionContext)
        {
            double Derivative = 0.0;
            double dLeftDerivative = 0.0;
            double dRightDerivative = 0.0;
            double dLeftEvaluate = 0.0;
            double dRightEvaluate = 0.0;
            Constants.FunctionType lastFunctionContext = Constants.FunctionType.NULL;

            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    Derivative = 0.0;
                    return Derivative;
                case Constants.OperatorType.ptrVariable:
                    if (pOperand == wrt)
                    {
                        Derivative = 1.0;
                    }
                    else
                    {
                        Derivative = 0.0;
                    }
                    return Derivative;
                case Constants.OperatorType.ptrExpression:
                    return pOperand.Node.Derivative(wrt, functionContext);
                case Constants.OperatorType.UnaryMinus:
                    Derivative = -pLeft.Derivative(wrt, functionContext);
                    return Derivative;
                case Constants.OperatorType.UnaryPlus:
                    Derivative = pLeft.Derivative(wrt, functionContext);
                    return Derivative;
                case Constants.OperatorType.Plus:
                    Derivative = pLeft.Derivative(wrt, functionContext) + pRight.Derivative(wrt, functionContext);
                    return Derivative;
                case Constants.OperatorType.Minus:
                    Derivative = pLeft.Derivative(wrt, functionContext) - pRight.Derivative(wrt, functionContext);
                    return Derivative;
                case Constants.OperatorType.Multiply:
                    dLeftDerivative = pLeft.Derivative(wrt, functionContext);
                    dRightDerivative = pRight.Derivative(wrt, functionContext);

                    Derivative = (pLeft.Evaluate(Constants.FunctionType.NULL) * dRightDerivative) + (dLeftDerivative * pRight.Evaluate(Constants.FunctionType.NULL));
                    return Derivative;
                case Constants.OperatorType.Divide:
                    dRightEvaluate = pRight.Evaluate(Constants.FunctionType.NULL);
                    if (System.Math.Abs(dRightEvaluate) > Constants.ETA)
                    {
                        dLeftDerivative = pLeft.Derivative(wrt, functionContext);
                        dRightDerivative = pRight.Derivative(wrt, functionContext);
                        dLeftEvaluate = pLeft.Evaluate(Constants.FunctionType.NULL);

                        Derivative = ((dLeftDerivative * dRightEvaluate) - (dLeftEvaluate * dRightDerivative)) / (dRightEvaluate * dRightEvaluate);
                        return Derivative;
                    }
                    else
                    {
                        Derivative = Constants.LARGEVALUE * Math.Sign(Math.Sign(dLeftEvaluate) * Math.Sign(dRightEvaluate));
                        return Derivative;
                    }
                case Constants.OperatorType.Expon:
                    // f = x^y
                    // ln(f) = y.ln(x) 
                    // dln(f)/dz = 1/f.df/dz = d(y.ln(x))/dz = dy/dz.ln(x) + y/x.dx/dz
                    // df/dz = f.(dy/dz.ln(x) + y/z .dx/dz) = x^y.(dy/dz.ln(x) + y/z.dx/dz)= ln(x).x^y.dy/dz + y.x^(y-1).dx/dz 
                    // df/dz = ln(x).x^y.dy/dz + y.x^(y-1).dx/dz 
                    dRightEvaluate = pRight.Evaluate(functionContext);
                    dLeftEvaluate = pLeft.Evaluate(functionContext);
                    dLeftDerivative = pRight.Derivative(wrt, functionContext);
                    dLeftDerivative = pLeft.Derivative(wrt, functionContext);

                    //Derivative = System.Math.Pow(dRightEvaluate * dLeftEvaluate, dRightEvaluate - 1);
                    if (dLeftEvaluate > Constants.ETA)
                    {
                        Derivative = System.Math.Log(dLeftEvaluate) * System.Math.Pow(dLeftEvaluate, dRightEvaluate) * dRightDerivative
                                        + dRightEvaluate * System.Math.Pow(dLeftEvaluate, dRightEvaluate - 1) * dLeftDerivative;
                    }
                    else
                    {
                        Derivative = 0.0;
                    }
                    return Derivative;
                case Constants.OperatorType.Equal:
                    Derivative = pLeft.Derivative(wrt, functionContext) - pRight.Derivative(wrt, functionContext);
                    return Derivative;
                case Constants.OperatorType.Grt:
                    Derivative = pLeft.Derivative(wrt, functionContext) - pRight.Derivative(wrt, functionContext);
                    return Derivative;
                case Constants.OperatorType.GrtEqual:
                    Derivative = pLeft.Derivative(wrt, functionContext) - pRight.Derivative(wrt, functionContext);
                    return Derivative;
                case Constants.OperatorType.Less:
                    Derivative = -pLeft.Derivative(wrt, functionContext) + pRight.Derivative(wrt, functionContext);
                    return Derivative;
                case Constants.OperatorType.LessEqual:
                    Derivative = -pLeft.Derivative(wrt, functionContext) + pRight.Derivative(wrt, functionContext);
                    return Derivative;
                case Constants.OperatorType.Range:
                case Constants.OperatorType.ListSeparator:
                    switch (functionContext)
                    {
                        case Constants.FunctionType.SUM:
                            Derivative =  userFunctions.SUM_Derivative(this, wrt, functionContext);  
                            return Derivative;
                        case Constants.FunctionType.PRODUCT:
                            Derivative = userFunctions.PRODUCT_Derivative(this, wrt, functionContext);  
                            return Derivative;
                        case Constants.FunctionType.USERDEF:
                            Derivative = userFunctions.USERDEF_Derivative(this, wrt, functionContext);
                            return Derivative;
                        default:
                            Derivative = -12121212;
                            return Derivative;
                    }
                case Constants.OperatorType.Function:
                    lastFunctionContext = functionContext;
                    functionContext = this.Function;
                    switch (functionContext)
                    {
                        case Constants.FunctionType.SUM:
                            Derivative = userFunctions.SUM_Derivative(this, wrt, functionContext);   
                            functionContext = lastFunctionContext;
                            return Derivative;
                        case Constants.FunctionType.PRODUCT:
                            Derivative = userFunctions.PRODUCT_Derivative(this, wrt, functionContext);  
                            functionContext = lastFunctionContext;
                            return Derivative;
                        case Constants.FunctionType.USERDEF:
                            Derivative = userFunctions.USERDEF_Derivative(this, wrt, functionContext);
                            functionContext = lastFunctionContext;
                            return Derivative;
                        default:
                            functionContext = lastFunctionContext;
                            Derivative = -12121212;
                            return Derivative;
                    }
                default:
                    Derivative = -12121212;
                    return Derivative;
            }
        }
        internal virtual double DerivativeResidual(CellClass wrt, Constants.FunctionType functionContext)
        {
            double DerivativeResidual = 0.0;
            double dLeftDerivativeResidual = 0.0;
            double dRightDerivativeResidual = 0.0;
            double dLeftEvaluateResidual = 0.0;
            double dRightEvaluateResidual = 0.0;
            Constants.FunctionType lastFunctionContext = Constants.FunctionType.NULL;

            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    DerivativeResidual = 0.0;
                    return DerivativeResidual;
                case Constants.OperatorType.ptrVariable:
                    if (pOperand == wrt)
                    {
                        DerivativeResidual = 1.0;
                    }
                    else
                    {
                        DerivativeResidual = 0.0;
                    }
                    return DerivativeResidual;
                case Constants.OperatorType.ptrExpression:
                    return pOperand.Node.DerivativeResidual(wrt, functionContext);
                case Constants.OperatorType.UnaryMinus:
                    DerivativeResidual = -pLeft.DerivativeResidual(wrt, functionContext);
                    return DerivativeResidual;
                case Constants.OperatorType.UnaryPlus:
                    DerivativeResidual = pLeft.DerivativeResidual(wrt, functionContext);
                    return DerivativeResidual;
                case Constants.OperatorType.Plus:
                    DerivativeResidual = pLeft.DerivativeResidual(wrt, functionContext) + pRight.DerivativeResidual(wrt, functionContext);
                    return DerivativeResidual;
                case Constants.OperatorType.Minus:
                    DerivativeResidual = pLeft.DerivativeResidual(wrt, functionContext) - pRight.DerivativeResidual(wrt, functionContext);
                    return DerivativeResidual;
                case Constants.OperatorType.Multiply:
                    dLeftDerivativeResidual = pLeft.DerivativeResidual(wrt, functionContext);
                    dRightDerivativeResidual = pRight.DerivativeResidual(wrt, functionContext);

                    DerivativeResidual = (pLeft.EvaluateResidual(functionContext) * dRightDerivativeResidual) + (dLeftDerivativeResidual * pRight.EvaluateResidual(functionContext));
                    return DerivativeResidual;
                case Constants.OperatorType.Divide:
                    dRightEvaluateResidual = pRight.EvaluateResidual(functionContext);
                    if (System.Math.Abs(dRightEvaluateResidual) > Constants.ETA)
                    {
                        dLeftDerivativeResidual = pLeft.DerivativeResidual(wrt, functionContext);
                        dRightDerivativeResidual = pRight.DerivativeResidual(wrt, functionContext);
                        dLeftEvaluateResidual = pLeft.EvaluateResidual(functionContext);

                        DerivativeResidual = ((dLeftDerivativeResidual * dRightEvaluateResidual) - (dLeftEvaluateResidual * dRightDerivativeResidual)) / (dRightEvaluateResidual * dRightEvaluateResidual);
                        return DerivativeResidual;
                    }
                    else
                    {
                        DerivativeResidual = Constants.LARGEVALUE * Math.Sign(Math.Sign(dLeftEvaluateResidual) * Math.Sign(dRightEvaluateResidual));
                        return DerivativeResidual;
                    }
                case Constants.OperatorType.Expon:
                    // f = x^y
                    // ln(f) = y.ln(x) 
                    // dln(f)/dz = 1/f.df/dz = d(y.ln(x))/dz = dy/dz.ln(x) + y/x.dx/dz
                    // df/dz = f.(dy/dz.ln(x) + y/z .dx/dz) = x^y.(dy/dz.ln(x) + y/z.dx/dz)= ln(x).x^y.dy/dz + y.x^(y-1).dx/dz 
                    // df/dz = ln(x).x^y.dy/dz + y.x^(y-1).dx/dz 
                    dRightEvaluateResidual = pRight.EvaluateResidual(functionContext);
                    dLeftEvaluateResidual = pLeft.EvaluateResidual(functionContext);
                    dLeftDerivativeResidual = pRight.DerivativeResidual(wrt, functionContext);
                    dLeftDerivativeResidual = pLeft.DerivativeResidual(wrt, functionContext);
                    if (dLeftEvaluateResidual > Constants.ETA)
                    {
                        //DerivativeResidual = System.Math.Pow(dRightEvaluateResidual * pLeft.EvaluateResidual(), dRightEvaluateResidual - 1);
                        DerivativeResidual = System.Math.Log(dLeftEvaluateResidual) * System.Math.Pow(dLeftEvaluateResidual, dRightEvaluateResidual) * dRightDerivativeResidual
                                        + dRightEvaluateResidual * System.Math.Pow(dLeftEvaluateResidual, dRightEvaluateResidual - 1) * dLeftDerivativeResidual;
                    }
                    else
                    {
                        DerivativeResidual = 0.0;
                    }
                    return DerivativeResidual;
                case Constants.OperatorType.Equal:
                    DerivativeResidual = pLeft.DerivativeResidual(wrt, functionContext) - pRight.DerivativeResidual(wrt, functionContext);
                    return DerivativeResidual;
                case Constants.OperatorType.Grt:
                    DerivativeResidual = pLeft.DerivativeResidual(wrt, functionContext) - pRight.DerivativeResidual(wrt, functionContext);
                    return DerivativeResidual;
                case Constants.OperatorType.GrtEqual:
                    DerivativeResidual = pLeft.DerivativeResidual(wrt, functionContext) - pRight.DerivativeResidual(wrt, functionContext);
                    return DerivativeResidual;
                case Constants.OperatorType.Less:
                    DerivativeResidual = -pLeft.DerivativeResidual(wrt, functionContext) + pRight.DerivativeResidual(wrt, functionContext);
                    return DerivativeResidual;
                case Constants.OperatorType.LessEqual:
                    DerivativeResidual = -pLeft.DerivativeResidual(wrt, functionContext) + pRight.DerivativeResidual(wrt, functionContext);
                    return DerivativeResidual;
                case Constants.OperatorType.Range:
                case Constants.OperatorType.ListSeparator:
                    switch (functionContext)
                    {
                        case Constants.FunctionType.SUM:
                            DerivativeResidual = userFunctions.SUM_DerivativeResidual(this, wrt, functionContext);
                            return DerivativeResidual;
                        case Constants.FunctionType.PRODUCT:
                            DerivativeResidual = userFunctions.PRODUCT_DerivativeResidual(this, wrt, functionContext);
                            return DerivativeResidual;
                        case Constants.FunctionType.USERDEF:
                            DerivativeResidual = userFunctions.USERDEF_DerivativeResidual(this, wrt, functionContext);
                            return DerivativeResidual;
                        default:
                            DerivativeResidual = -12121212;
                            return DerivativeResidual;
                    }
                case Constants.OperatorType.Function:
                    lastFunctionContext = functionContext;
                    functionContext = this.Function;
                    switch (functionContext)
                    {
                        case Constants.FunctionType.SUM:
                            DerivativeResidual = userFunctions.SUM_DerivativeResidual(this, wrt, functionContext);
                            functionContext = lastFunctionContext;
                            return DerivativeResidual;
                        case Constants.FunctionType.PRODUCT:
                            DerivativeResidual = userFunctions.PRODUCT_DerivativeResidual(this, wrt, functionContext);
                            functionContext = lastFunctionContext;
                            return DerivativeResidual;
                        case Constants.FunctionType.USERDEF:
                            DerivativeResidual = userFunctions.USERDEF_DerivativeResidual(this, wrt, functionContext);
                            functionContext = lastFunctionContext;
                            return DerivativeResidual;
                       default:
                            DerivativeResidual = -12121212;
                            functionContext = lastFunctionContext;
                            return DerivativeResidual;
                    }

                default:
                    DerivativeResidual = -12121212;
                    return DerivativeResidual;
            }
        }
        internal string Serialize(bool bVariableNames)
        {
            string Serialize = "";

            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    Serialize = strOperand;
                    return Serialize;
                case Constants.OperatorType.ptrVariable:
                    if (bVariableNames)
                    {
                        Serialize = pOperand.Name;// Address;
                    }
                    else
                    {
                        Serialize = pOperand.ReconciledVariable.Value.ToString();
                    };
                    return Serialize;
                case Constants.OperatorType.ptrExpression:
                    Serialize = Constants.OpenBracket + pOperand.Node.Serialize(bVariableNames) + Constants.CloseBracket;
                    return Serialize;
                case Constants.OperatorType.UnaryMinus:
                    Serialize = Constants.OperatorString(Constants.OperatorType.UnaryMinus) + pLeft.Serialize(bVariableNames);
                    return Serialize;
                case Constants.OperatorType.UnaryPlus:
                    Serialize = Constants.OperatorString(Constants.OperatorType.UnaryPlus) + pLeft.Serialize(bVariableNames);
                    return Serialize;
                case Constants.OperatorType.Plus:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.Plus) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.Minus:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.Minus) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.Multiply:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.Multiply) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.Divide:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.Divide) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.Expon:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.Expon) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.Equal:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.Equal) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.Grt:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.Grt) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.GrtEqual:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.GrtEqual) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.Less:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.Less) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.LessEqual:
                    Serialize = Constants.OpenParen + pLeft.Serialize(bVariableNames) + Constants.OperatorString(Constants.OperatorType.LessEqual) + pRight.Serialize(bVariableNames) + Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.ListSeparator:
                    Serialize = Constants.Comma + pLeft.Serialize(bVariableNames);
                    if (pRight != null)//.OperatorType == Constants.OperatorType.ListSeparator)
                    {
                        Serialize += pRight.Serialize(bVariableNames);
                    }
                    return Serialize;
                case Constants.OperatorType.Function:
                    Serialize = this.strOperand + Constants.OpenParen + pLeft.Serialize(bVariableNames);

                    if (pRight != null) //.OperatorType == Constants.OperatorType.ListSeparator)
                    {
                        Serialize += pRight.Serialize(bVariableNames);
                    }
                    Serialize += Constants.CloseParen;
                    return Serialize;
                case Constants.OperatorType.Range:
                    Serialize = Constants.OpenBrace + pLeft.Serialize(bVariableNames);
                    if (pRight != null)//.OperatorType == Constants.OperatorType.ListSeparator)
                    {
                        Serialize += pRight.Serialize(bVariableNames);
                    }
                    return Serialize + Constants.CloseBrace;
                default:
                    Serialize = Constants.OperatorString(Constants.OperatorType.NullType);
                    return Serialize;
            }

        }
        internal void CountOperators()
        {
            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    pLinearOperatorCount = 0;
                    pNonlinearOperatorCount = 0;
                    pVariableCount = 0;
                    return;
                case Constants.OperatorType.ptrVariable:
                    pLinearOperatorCount = 0;
                    pNonlinearOperatorCount = 0;
                    pVariableCount = 1;
                    return;
                case Constants.OperatorType.ptrExpression:
                    pOperand.Node.CountOperators();
                    pLinearOperatorCount = pOperand.Node.LinearOperatorCount;
                    pNonlinearOperatorCount = pOperand.Node.NonlinearOperatorCount;
                    pVariableCount = pOperand.Node.VariableCount;
                    return;
                case Constants.OperatorType.UnaryMinus:
                    pLeft.CountOperators();
                    pLinearOperatorCount = pLeft.LinearOperatorCount;
                    pNonlinearOperatorCount = pLeft.NonlinearOperatorCount;
                    pVariableCount = pLeft.VariableCount;
                    return;
                case Constants.OperatorType.UnaryPlus:
                    pLeft.CountOperators();
                    pLinearOperatorCount = pLeft.LinearOperatorCount;
                    pNonlinearOperatorCount = pLeft.NonlinearOperatorCount;
                    pVariableCount = pLeft.VariableCount;
                    return;
                case Constants.OperatorType.Plus:
                case Constants.OperatorType.Minus:
                    pLeft.CountOperators();
                    pRight.CountOperators();
                    pLinearOperatorCount = pLeft.LinearOperatorCount + pRight.LinearOperatorCount;
                    pNonlinearOperatorCount = pLeft.NonlinearOperatorCount + pRight.NonlinearOperatorCount;
                    pVariableCount = pLeft.VariableCount + pRight.VariableCount;
                    if (pLeft.VariableCount > 0 && pRight.VariableCount > 0)
                    {
                        pLinearOperatorCount += 1;
                    }
                    return;
                case Constants.OperatorType.Multiply:
                case Constants.OperatorType.Divide:
                    pLeft.CountOperators();
                    pRight.CountOperators();
                    pLinearOperatorCount = pLeft.LinearOperatorCount + pRight.LinearOperatorCount;
                    pNonlinearOperatorCount = pLeft.NonlinearOperatorCount + pRight.NonlinearOperatorCount;
                    pVariableCount = pLeft.VariableCount + pRight.VariableCount;
                    if (pLeft.VariableCount > 0 && pRight.VariableCount > 0)
                    {
                        pNonlinearOperatorCount += 1;
                    }
                    return;
                case Constants.OperatorType.Expon:
                    pLeft.CountOperators();
                    pRight.CountOperators();
                    pLinearOperatorCount = pLeft.LinearOperatorCount + pRight.LinearOperatorCount;
                    pNonlinearOperatorCount = pLeft.NonlinearOperatorCount + pRight.NonlinearOperatorCount;
                    pVariableCount = pLeft.VariableCount + pRight.VariableCount;
                    if (pRight.VariableCount > 0)
                    {
                        pNonlinearOperatorCount += 1;
                    }
                    return;
                case Constants.OperatorType.Equal:
                case Constants.OperatorType.Grt:
                case Constants.OperatorType.GrtEqual:
                case Constants.OperatorType.Less:
                case Constants.OperatorType.LessEqual:
                    pLeft.CountOperators();
                    pRight.CountOperators();
                    pLinearOperatorCount = pLeft.LinearOperatorCount + pRight.LinearOperatorCount + 1;
                    pNonlinearOperatorCount = pLeft.NonlinearOperatorCount + pRight.NonlinearOperatorCount;
                    pVariableCount = pLeft.VariableCount + pRight.VariableCount;
                    return;
                case Constants.OperatorType.Range:
                case Constants.OperatorType.ListSeparator:
                    pLeft.CountOperators();
                    if (pRight != null)
                    {
                        pRight.CountOperators();
                        pLinearOperatorCount = pLeft.LinearOperatorCount + pRight.LinearOperatorCount + 1;
                        pNonlinearOperatorCount = pLeft.NonlinearOperatorCount + pRight.NonlinearOperatorCount;
                        pVariableCount = pLeft.VariableCount + pRight.VariableCount;
                    }
                    else
                    {
                        pLinearOperatorCount = pLeft.LinearOperatorCount + 1;
                        pNonlinearOperatorCount = pLeft.NonlinearOperatorCount ;
                        pVariableCount = pLeft.VariableCount;
                    }
                    return;
                case Constants.OperatorType.Function:
                    pLeft.CountOperators();
                    if (pRight != null)
                    {
                        pRight.CountOperators();
                        pLinearOperatorCount = pLeft.LinearOperatorCount + pRight.LinearOperatorCount + 1;
                        pNonlinearOperatorCount = pLeft.NonlinearOperatorCount + pRight.NonlinearOperatorCount;
                        pVariableCount = pLeft.VariableCount + pRight.VariableCount;
                    }
                    else
                    {
                        pLinearOperatorCount = pLeft.LinearOperatorCount + 1;
                        pNonlinearOperatorCount = pLeft.NonlinearOperatorCount;
                        pVariableCount = pLeft.VariableCount;
                    }
                    return;
                default:
                    return;
            } 
        }
        private Constants.LinearityType DeduceLinearity()
        {
            Constants.LinearityType DeduceLinearity = 0;
            Constants.LinearityType dLeftLinearity = 0;
            Constants.LinearityType dRightLinearity = 0;

            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    DeduceLinearity = Constants.LinearityType.Constant;
                    return DeduceLinearity;
                case Constants.OperatorType.ptrVariable:
                    DeduceLinearity = Constants.LinearityType.Linear;
                    return DeduceLinearity;
                case Constants.OperatorType.ptrExpression:
                    DeduceLinearity = pOperand.Node.DeduceLinearity();
                    return DeduceLinearity;
                case Constants.OperatorType.UnaryMinus:
                    DeduceLinearity = pLeft.Linearity;
                    return DeduceLinearity;
                case Constants.OperatorType.UnaryPlus:
                    DeduceLinearity = pLeft.Linearity;
                    return DeduceLinearity;
                case Constants.OperatorType.Plus:
                case Constants.OperatorType.Minus:
                    dLeftLinearity = pLeft.Linearity;
                    dRightLinearity = pRight.Linearity;
                    if ((dLeftLinearity == Constants.LinearityType.Nonlinear) || (dRightLinearity == Constants.LinearityType.Nonlinear))
                    {
                        DeduceLinearity = Constants.LinearityType.Nonlinear;
                    }
                    else
                    {
                        if ((dLeftLinearity == Constants.LinearityType.Linear) || (dRightLinearity == Constants.LinearityType.Linear))
                        {
                            DeduceLinearity = Constants.LinearityType.Linear;
                        }
                        else
                        {
                            DeduceLinearity = Constants.LinearityType.Constant;
                        }
                    }
                    return DeduceLinearity;
                case Constants.OperatorType.Multiply:
                case Constants.OperatorType.Divide:
                case Constants.OperatorType.Expon:
                    dLeftLinearity = pLeft.Linearity;
                    dRightLinearity = pRight.Linearity;
                    if ((dLeftLinearity == Constants.LinearityType.Nonlinear) || (dRightLinearity == Constants.LinearityType.Nonlinear))
                    {
                        DeduceLinearity = Constants.LinearityType.Nonlinear;
                    }
                    else
                    {
                        if ((dLeftLinearity == Constants.LinearityType.Linear) && (dRightLinearity == Constants.LinearityType.Linear))
                        {
                            DeduceLinearity = Constants.LinearityType.Nonlinear;
                        }
                        else
                        {
                            if ((dLeftLinearity == Constants.LinearityType.Linear) || (dRightLinearity == Constants.LinearityType.Linear))
                            {
                                DeduceLinearity = Constants.LinearityType.Linear;
                            }
                            else
                            {
                                DeduceLinearity = Constants.LinearityType.Constant;
                            }
                        }
                    }
                    return DeduceLinearity;
                case Constants.OperatorType.Equal:
                case Constants.OperatorType.Grt:
                case Constants.OperatorType.GrtEqual:
                case Constants.OperatorType.Less:
                case Constants.OperatorType.LessEqual:
                    dLeftLinearity = pLeft.Linearity;
                    dRightLinearity = pRight.Linearity;
                    if ((dLeftLinearity == Constants.LinearityType.Nonlinear) || (dRightLinearity == Constants.LinearityType.Nonlinear))
                    {
                        DeduceLinearity = Constants.LinearityType.Nonlinear;
                    }
                    else
                    {
                        if ((dLeftLinearity == Constants.LinearityType.Linear) || (dRightLinearity == Constants.LinearityType.Linear))
                        {
                            DeduceLinearity = Constants.LinearityType.Linear;
                        }
                        else
                        {
                            DeduceLinearity = Constants.LinearityType.Constant;
                        }
                    }
                    return DeduceLinearity;
                case Constants.OperatorType.Range:
                case Constants.OperatorType.ListSeparator:
                    dLeftLinearity = pLeft.Linearity;
                    if (pRight!= null) dRightLinearity = pRight.Linearity;
                    if ((dLeftLinearity == Constants.LinearityType.Nonlinear) || (dRightLinearity == Constants.LinearityType.Nonlinear))
                    {
                        DeduceLinearity = Constants.LinearityType.Nonlinear;
                    }
                    return DeduceLinearity;
                case Constants.OperatorType.Function:
                    dLeftLinearity = pLeft.Linearity;
                    if (pRight != null)  dRightLinearity = pRight.Linearity;
                    if ((dLeftLinearity == Constants.LinearityType.Nonlinear) || (dRightLinearity == Constants.LinearityType.Nonlinear))
                    {
                        DeduceLinearity = Constants.LinearityType.Nonlinear;
                    }
                    if (this.strOperand !="SUM")
                    {
                        DeduceLinearity = Constants.LinearityType.Nonlinear;
                    }
                    return DeduceLinearity;
                default:
                    DeduceLinearity = Constants.LinearityType.Unknown;
                    return DeduceLinearity;
            }             
        }
        internal void LinkExpressions()
        {

            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    return;
                case Constants.OperatorType.ptrVariable:
                    if (pOperand.Node != null)
                    {
                        pOperatorType = Constants.OperatorType.ptrExpression;
                        pOperand.Node.LinkExpressions();
                    }
                    return;
                case Constants.OperatorType.ptrExpression:
                    //pOperand.ReconciledVariable = null;
                    return;
                case Constants.OperatorType.UnaryMinus:
                case Constants.OperatorType.UnaryPlus:
                    pLeft.LinkExpressions();
                    return;
                case Constants.OperatorType.Plus:
                case Constants.OperatorType.Minus:
                case Constants.OperatorType.Multiply:
                case Constants.OperatorType.Divide:
                case Constants.OperatorType.Expon:
                case Constants.OperatorType.Equal:
                case Constants.OperatorType.Grt:
                case Constants.OperatorType.GrtEqual:
                case Constants.OperatorType.Less:
                case Constants.OperatorType.LessEqual:
                    pLeft.LinkExpressions();
                    pRight.LinkExpressions();
                    return;
                case Constants.OperatorType.Range:
                case Constants.OperatorType.ListSeparator:
                    pLeft.LinkExpressions();
                    if (pRight != null) pRight.LinkExpressions();
                    return;
                case Constants.OperatorType.Function:
                    pLeft.LinkExpressions();
                    if (pRight != null) pRight.LinkExpressions();
                    return;
                default:
                    return;
            }          
        }
        internal void LocateArguments(ConstraintClass Balance)
        {
            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    return;
                case Constants.OperatorType.ptrVariable:
                    Balance.AddDependent(pOperand);
                    return;
                case Constants.OperatorType.ptrExpression:
                    pOperand.Node.LocateArguments(Balance);
                    return;
                case Constants.OperatorType.UnaryMinus:
                case Constants.OperatorType.UnaryPlus:
                    pLeft.LocateArguments(Balance);
                    return;
                case Constants.OperatorType.Plus:
                case Constants.OperatorType.Minus:
                case Constants.OperatorType.Multiply:
                case Constants.OperatorType.Divide:
                case Constants.OperatorType.Expon:
                case Constants.OperatorType.Equal:
                case Constants.OperatorType.Grt:
                case Constants.OperatorType.GrtEqual:
                case Constants.OperatorType.Less:
                case Constants.OperatorType.LessEqual:
                    pLeft.LocateArguments(Balance);
                    pRight.LocateArguments(Balance);
                    return;
                case Constants.OperatorType.Range:
                case Constants.OperatorType.ListSeparator:
                    pLeft.LocateArguments(Balance);
                    if (pRight != null) pRight.LocateArguments(Balance);
                    return;
                case Constants.OperatorType.Function:
                    pLeft.LocateArguments(Balance);
                    if (pRight != null) pRight.LocateArguments(Balance);
                    return;
                default:
                    return;
            } 
        }
        internal void LocateDependentArguments(DependentClass Expression)
        {
            switch (pOperatorType)
            {
                case Constants.OperatorType.Real:
                    return;
                case Constants.OperatorType.ptrVariable:
                    Expression.AddDependent(pOperand);
                    return;
                case Constants.OperatorType.ptrExpression:
                    pOperand.Node.LocateDependentArguments(Expression);
                    return;
                case Constants.OperatorType.UnaryMinus:
                case Constants.OperatorType.UnaryPlus:
                    pLeft.LocateDependentArguments(Expression);
                    return;
                case Constants.OperatorType.Plus:
                case Constants.OperatorType.Minus:
                case Constants.OperatorType.Multiply:
                case Constants.OperatorType.Divide:
                case Constants.OperatorType.Expon:
                case Constants.OperatorType.Equal:
                case Constants.OperatorType.Grt:
                case Constants.OperatorType.GrtEqual:
                case Constants.OperatorType.Less:
                case Constants.OperatorType.LessEqual:
                    pLeft.LocateDependentArguments(Expression);
                    pRight.LocateDependentArguments(Expression);
                    return;
                case Constants.OperatorType.Range:
                case Constants.OperatorType.ListSeparator:
                    pLeft.LocateDependentArguments(Expression);
                    if (pRight != null) pRight.LocateDependentArguments(Expression);
                    return;
                case Constants.OperatorType.Function:
                    pLeft.LocateDependentArguments(Expression);
                    if (pRight != null) pRight.LocateDependentArguments(Expression);
                    return;
                default:
                    return;
            }
        }
     }
}