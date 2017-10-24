
namespace com.inova8.resolver
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    public class ConstraintsClass : IEnumerable
    {
        private List<ConstraintClass> pConstraints = new List<ConstraintClass>();
        private int nDistinctBalanceErrors = 0;
        private double dConstraintCriticalValue = 0.0;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pConstraints.GetEnumerator();
        }
        public int Count
        {
            get { return pConstraints.Count; }
        }
        public int DistinctBalanceErrors
        {
            get { return nDistinctBalanceErrors; }
        }
        public double ConstraintCriticalValue
        {
            get { return dConstraintCriticalValue; }
        }
        internal void UpdateActiveConstraints()
        {
            foreach (ConstraintClass Balance in pConstraints)
            {
                if (Balance.Node.OperatorType != Constants.OperatorType.Equal)
                {
                    Balance.Active = false;
                }
                else
                {
                    Balance.Active = true;
                }
            }
        }
        internal void CalculateConstraintError()
        {
            double dTemp = 0.0;

            foreach (ConstraintClass Constraint in pConstraints)
            {
                Constraint.MeasuredResidual = Constraint.Node.EvaluateResidual(Constants.FunctionType.NULL);
                Constraint.ReconciledResidual = Constraint.Node.Evaluate(Constants.FunctionType.NULL);
                
                if ((Constraint.Active == true) || (Constraint.Node.OperatorType == Constants.OperatorType.Equal))
                {
                    if (Constraint.Dependents.Count > 0)
                    {
                        Constraint.MeasuredDeviation = 0.0;
                        Constraint.ReconciledDeviation = 0.0;

                        foreach (CellClass Argument in Constraint.Dependents)
                        {
                            //if (Argument.hasMeasurement)
                            //{
                            //    dTemp = Constraint.Node.DerivativeResidual(Argument,Function) * Argument.ReconciledVariable.InitialTolerance;
                            //    Constraint.ReconciledDeviation += dTemp * dTemp;
                            //}
                            //else
                            //{
                            dTemp = Constraint.Node.DerivativeResidual(Argument, Constants.FunctionType.NULL) * Argument.ReconciledVariable.ReconciledTolerance;
                            Constraint.ReconciledDeviation += dTemp * dTemp;
                            //}

                            if (Argument.hasMeasurement)
                            {
                                dTemp = Constraint.Node.Derivative(Argument, Constants.FunctionType.NULL) * Argument.ReconciledVariable.InitialTolerance;
                                Constraint.MeasuredDeviation += dTemp * dTemp;
                            }
                            else
                            {
                                dTemp = Constraint.Node.Derivative(Argument, Constants.FunctionType.NULL) * Argument.ReconciledVariable.ReconciledTolerance;
                                Constraint.MeasuredDeviation += dTemp * dTemp;
                            }

                        }
                    }
                    Constraint.ReconciledDeviation = System.Math.Sqrt(Constraint.ReconciledDeviation);
                    Constraint.MeasuredDeviation = System.Math.Sqrt(Constraint.MeasuredDeviation);
                }
            }
        }
        internal void CalculateMeasuredDeviation()
        {
            double dTemp = 0.0;

            foreach (ConstraintClass Constraint in pConstraints)
            {
                if ((Constraint.Active == true) || (Constraint.Node.OperatorType == Constants.OperatorType.Equal))
                {
                    Constraint.MeasuredDeviation = 0.0;
                    foreach (CellClass Argument in Constraint.Dependents)
                    {
                        if (Argument.hasMeasurement)
                        {
                            dTemp = Constraint.Node.Derivative(Argument, Constants.FunctionType.NULL) * Argument.ReconciledVariable.InitialTolerance;
                            Constraint.MeasuredDeviation += dTemp * dTemp;
                        }
                        else
                        {
                            dTemp = Constraint.Node.Derivative(Argument, Constants.FunctionType.NULL) * Argument.ReconciledVariable.ReconciledTolerance;
                            Constraint.MeasuredDeviation += dTemp * dTemp;
                        }
                    }
                    Constraint.MeasuredDeviation = System.Math.Sqrt(Constraint.MeasuredDeviation);
                }
            }
        }
        internal void DiagnoseBalanceErrors(double dConfidence, double dStandardTolerance/*, Microsoft.Office.Interop.Excel.Application xl*/, int nFixedValues)
        {
            double dBalanceMaximumValue = 0.0;

            nDistinctBalanceErrors = 0;
            this.CalculateConstraintError();

            // used to correct results to 95% confidence and 2*sd
            //dBalanceMaximumValue = xl.WorksheetFunction.NormSInv((1.0 - dConfidence)) / dStandardTolerance;
            //dBalanceMaximumValue = Functions.NormSInv((1.0 - dConfidence)) / dStandardTolerance;
            dBalanceMaximumValue = StatisticsClass.inv_normal((1.0 - dConfidence)) / dStandardTolerance;
    
            foreach (ConstraintClass Balance in pConstraints)
            {
                if (Balance.Active) nDistinctBalanceErrors++;
            }
            //credit the 0 tolerance measurements as equivalent to an active constraint.
            nDistinctBalanceErrors += nFixedValues;

            if (nDistinctBalanceErrors == 0)
            {
                dConstraintCriticalValue = dBalanceMaximumValue; // xl.WorksheetFunction.NormSInv(1.0);
            }
            else
            {
                double beta = (1 - System.Math.Pow((1 - dConfidence), (1.0 / nDistinctBalanceErrors)));
                //dConstraintCriticalValue = xl.WorksheetFunction.NormSInv(1.0 - beta / 2);
                //dConstraintCriticalValue = Functions.NormSInv(1.0 - beta / 2);
                dConstraintCriticalValue = StatisticsClass.inv_normal(1.0 - beta / 2);

            }

            foreach (ConstraintClass Balance in pConstraints)
            {
                if (Balance.ReconciledDeviation < Constants.ETA)
                {
                    Balance.ReconciledTest = Balance.MeasuredResidual / Constants.ETA;
                }
                else
                {
                    Balance.ReconciledTest = Balance.MeasuredResidual / Balance.ReconciledDeviation;
                }
                if (Balance.MeasuredDeviation < Constants.ETA)
                {
                    Balance.MeasuredTest = Balance.MeasuredResidual / Constants.ETA;
                }
                else
                {
                    Balance.MeasuredTest = Balance.MeasuredResidual / Balance.MeasuredDeviation;
                }
            }
        }
        internal Constants.LinearityType DetermineLinearity()
        {
           Constants.LinearityType tLinearity= Constants.LinearityType.Linear;
            foreach (ConstraintClass Balance in pConstraints)
            {
                tLinearity = Balance.Node.Linearity;
            }
            return tLinearity;
        }
        internal void CountOperators()
        {
            foreach (ConstraintClass Balance in pConstraints)
            {
                Balance.Node.CountOperators();
            }
        }
        internal void LinkExpressions()
        {
            foreach (ConstraintClass Constraint in pConstraints)
            {
                Constraint.Node.LinkExpressions();
            }
        }
        internal void LocateConstraintVariables()
        {
            foreach (ConstraintClass Constraint in pConstraints)
            {
                Constraint.Node.LocateArguments(Constraint);
            }
        }
        public int ActiveConstraints
        {
            get
            {
                int ac=0;
                foreach( ConstraintClass constraint in pConstraints)
                {
                    if (constraint.Active) ac++;
                }
                return ac;
            }
        }
        public int ActiveLinearInequalityConstraints
        {
            get
            {
                int ac = 0;
                foreach (ConstraintClass constraint in pConstraints)
                {
                    if (constraint.Active && (constraint.Node.OperatorType != Constants.OperatorType.Equal) && (constraint.Node.Linearity == Constants.LinearityType.Linear) ) ac++;
                }
                return ac;
            }
        }
        public int ActiveNonlinearInequalityConstraints
        {
            get
            {
                int ac = 0;
                foreach (ConstraintClass constraint in pConstraints)
                {
                    if (constraint.Active && (constraint.Node.OperatorType != Constants.OperatorType.Equal) && (constraint.Node.Linearity != Constants.LinearityType.Linear)) ac++;
                }
                return ac;
            }
        }
        internal int NumberNonLinearConstraints
        {
            get
            {
                int n = 0;
                foreach (ConstraintClass Balance in pConstraints)
                {
                    if (Balance.Node.Linearity == Constants.LinearityType.Nonlinear)
                    {
                        n = n + 1;
                    }
                }
                return n;
            }
        }
        public int LinearConstraints
        {
            get
            {
                int n = 0;
                foreach (ConstraintClass Balance in pConstraints)
                {
                    if (Balance.Node.Linearity == Constants.LinearityType.Linear && (Balance.Node.OperatorType == Constants.OperatorType.Equal))
                    {
                        n = n + 1;
                    }
                }
                return n;
            }
        }
        public int NonlinearConstraints
        {
            get
            {
                int n = 0;
                foreach (ConstraintClass Balance in pConstraints)
                {
                    if (Balance.Node.Linearity == Constants.LinearityType.Nonlinear && (Balance.Node.OperatorType == Constants.OperatorType.Equal))
                    {
                        n = n + 1;
                    }
                }
                return n;
            }
        }
        internal void Add(NodeClass Constraint)
        {
            ConstraintClass pConstraint = new ConstraintClass();
            pConstraint.Node = Constraint;
            pConstraints.Add(pConstraint);
        }
        internal ConstraintClass Item(int index)
        {
            ConstraintClass Item = null;
            Item = (ConstraintClass)pConstraints[index - 1];
            return Item;
        }
        internal void Sort()
        {
            ComplexityComparerClass myComparer= new ComplexityComparerClass();
            pConstraints.Sort(myComparer);
            return;
        }
        public class ComplexityComparerClass : IComparer<ConstraintClass>
        {
            public int Compare(ConstraintClass xConstraint, ConstraintClass yConstraint)
            {
                if (xConstraint == null)
                {
                    if (yConstraint == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (yConstraint == null)
                    {
                        return 1;
                    }
                    else
                    {
                        return Math.Sign(yConstraint.Complexity - xConstraint.Complexity);
                    }
                }
            }
        }
        //internal static int byComplexity(ConstraintClass xConstraint, ConstraintClass yConstraint)
        //{
        //    if (xConstraint == null)
        //    {
        //        if (yConstraint == null)
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            return -1;
        //        }
        //    }
        //    else
        //    {
        //        if (yConstraint == null)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return Math.Sign(yConstraint.Complexity - xConstraint.Complexity);
        //        }
        //    }
        //}
    }
}