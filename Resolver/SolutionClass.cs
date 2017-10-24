
namespace com.inova8.resolver
{
    using System;
    using System.Collections.Generic;

    public class SolutionClass
    {
        public enum Termination
        {
            LinearConvergence,
            MinimumPrecision,
            MinimumConvergence,
            MaximumIterations,
            MaximumTime
        }
        public class Stopwatch
        {
              private DateTime  startTime;
              private DateTime  stopTime;
              private bool running = false;
            
              public void Start() {
                this.startTime = DateTime.Now;
                this.running = true;
              }
            
              public void Stop() {
                this.stopTime = DateTime.Now;
                this.running = false;
              }

              //elaspsed time in milliseconds
              public int Elapsed
              {
                  get{
                      if (running)
                      {
                          //return (int)(new java.util.Date().getTime() - startTime.getTime());
                          return DateTime.Now.Subtract(startTime).Milliseconds;
                      }
                      else
                      {
                          //return (int)(stopTime.getTime() - startTime.getTime()); 
                          return stopTime.Subtract(startTime).Milliseconds;
                      }
                  }
              }

        }

        public class ConvergenceClass
        {
            private static System.String pLog = "";
            private int nConvergenceTime = 0;
            private bool bConverged;
            private int nIterations = 0;
            private double dCalculatedPrecision = 0.0;
            private double dCost = 0.0;
            private double dPreviousCalculatedPrecision = 0.0;
            private double dPreviousCost = 0.0;
            private Stopwatch pStopwatch;
            private double dReconciledCost;
            internal Termination eTermination;

            public System.String Log
            {
                get { return pLog; }
                set { pLog = value; }
            }
            internal void AddIterationToLog(SolutionClass Solution)
            {
                pLog += "#Iteration=" + nIterations.ToString() + "\r\n";
                pLog += "  dPrecision=" + dCalculatedPrecision.ToString() + "\r\n";
                pLog += "  dPrecisionChange=" + PrecisionChange.ToString() + "\r\n";
                pLog += "  dCost=" + dCost.ToString() + "\r\n";
                pLog += "  dCostChange=" + CostChange.ToString() + "\r\n";
                pLog += "  ActivateLinearInequalities=" + Solution.Constraints.ActiveLinearInequalityConstraints + "\r\n";
                pLog += "  ActivateNonlinearInequalities=" + Solution.Constraints.ActiveNonlinearInequalityConstraints + "\r\n";
            }
            public int ConvergenceTime
            {
                get { return nConvergenceTime; }
                set { nConvergenceTime = value; }
            }
            public bool Converged
            {
                get { return bConverged; }
                set { bConverged = value; }
            }
            public string Terminated
            {
                get
                {
                    switch (eTermination)
                    {
                        case Termination.LinearConvergence:
                            return "Linear";
                        case Termination.MinimumPrecision:
                            return "Minimum precision";
                        case Termination.MinimumConvergence:
                            return "Minimum cost change";
                        case Termination.MaximumIterations:
                            return "Maximum iterations";
                        case Termination.MaximumTime:
                            return "Maximum time";
                        default:
                            return "Unknown";
                    }
                }
            }
            public int Iterations
            {
                get { return nIterations; }
                set { nIterations = value; }
            }
            public double CalculatedPrecision
            {
                get { return Math.Abs(dCalculatedPrecision); }
                set { dCalculatedPrecision = value; }
            }
            public double Cost
            {
                get { return dCost; }
                set { dCost = value; }
            }
            public double PreviousCalculatedPrecision
            {
                get { return dPreviousCalculatedPrecision; }
                set { dPreviousCalculatedPrecision = value; }
            }
            public double PreviousCost
            {
                get { return dPreviousCost; }
                set { dPreviousCost = value; }
            }
            public double PrecisionChange
            {
                get { return Math.Abs(dCalculatedPrecision / dPreviousCalculatedPrecision); }

            }
            public double CostChange
            {
                get { return Math.Abs(1 - dCost / dPreviousCost); }
            }
            internal void Stop()
            {
                pStopwatch.Stop();
                nConvergenceTime = pStopwatch.Elapsed;
            }
            public double ElapsedSeconds
            {
                get { return pStopwatch.Elapsed/1000.0; }
            }
            public double ReconciledCost
            {
                get { return dReconciledCost; }
                internal set { dReconciledCost = value; }
            }
            internal void Retain()
            {
                dPreviousCost = dCost;
                dPreviousCalculatedPrecision = dCalculatedPrecision;
            }
            internal void Calculate(SolutionClass Solution, CovClass dUT)
            {
                dCost = Solution.Results.CalculateCost();
                dCalculatedPrecision = Solution.CalculatePrecision();
            }
            internal ConvergenceClass()
            {
                pStopwatch = new Stopwatch();
                pStopwatch.Start();
            }
        }
        private CovClass dUT; 
        private int nResults = 0;
        private ResultsClass pResults = new ResultsClass();
        private ResolverClass pProblem = null;
        internal double dConfidence = 0.95;
        internal double dStandardTolerance = 2.0;
        private double dGlobalCriticalValue = 0.0;
        private ConvergenceClass pConvergence;

        public SolutionClass()
        {
            pConvergence = new ConvergenceClass();
        }
        internal double MeasurementCriticalValue
        {
            get { return pProblem.Results.MeasurementCriticalValue; }
        }
        internal double ConstraintCriticalValue
        {
            get { return pProblem.Constraints.ConstraintCriticalValue; }
        }
        internal double GlobalCriticalValue
        {
            get { return dGlobalCriticalValue; }
        }
        internal int RedundancyDegree
        {
            get { return pProblem.Results.RedundancyDegree; }
        }
        public ConstraintsClass Constraints
        {
            get { return pProblem.Constraints; }
        }
        public DependentsClass Dependents
        {
            get { return pProblem.Dependents; }
        }
        public ResultsClass Results
        {
            get { return pResults; }
        }
        public ConvergenceClass Convergence
        {
            get { return pConvergence; }
        }
        public int FixedValues
        {
            get { return pProblem.Results.FixedValues; }
        }
        internal bool Resolve(ResolverClass Problem, int MaximumTime, int Iterations, double Precision, double Convergence)
        {
            double InitialGradient = Precision;
            double GradientChange = 2.0;
            double dGradient = Precision;

            double[] dCp;

            pConvergence.Converged = false;
            pProblem = Problem;

            pProblem.Constraints.LinkExpressions();
            pProblem.Dependents.LinkExpressions();

            LocateReconciledVariables();
            pProblem.Constraints.LocateConstraintVariables();
            pProblem.Dependents.LocateDependentVariables();

            Problem.Constraints.DetermineLinearity();
            Problem.Constraints.CountOperators();
            Problem.Constraints.Sort();
            //Resize();
            dCp = new double[nResults + 1];
            dUT = new CovClass(nResults);
            //UpdateActiveConstraints();
            Problem.Constraints.UpdateActiveConstraints();
            //InitializeMeasurements();
            pResults.InitializeMeasurements();
            //InitializeCovariance(dUT);
            dUT.InitializeCovariance(pResults);
            pConvergence.Calculate(this, dUT);

            pConvergence.Retain();
            ReconcileLinearEqualities(0, dCp);

            if (Problem.Constraints.NumberNonLinearConstraints == 0)
            {
                ReconcileInequalities(dGradient, dCp);
                pResults.RetainFinalValues();
                pConvergence.eTermination = Termination.LinearConvergence;
                pConvergence.Converged = true;
            }
            else
            {
                pConvergence.Iterations = 1;
                dGradient *= nResults; // scale according to the number of variables
                ReconcileNonLinearEqualities(dGradient, dCp);
                //ReconcileInequalities(dGradient, dCp, dUT);  //causes problems should delay until the precision/convergence is complete
                pResults.RetainFinalValues();
                pConvergence.Calculate(this,dUT);
                pConvergence.AddIterationToLog(this);
                pConvergence.Retain();
                pConvergence.Log += "Action=  Retaining values" + "\r\n";
                do
                {
                    do
                    {
                        pConvergence.Iterations = pConvergence.Iterations + 1;
                        UpdateCovariance();
                        pResults.UpdateMeasurement();
                        Problem.Constraints.UpdateActiveConstraints();
                        ReconcileLinearEqualities(0, dCp);
                        ReconcileNonLinearEqualities(dGradient, dCp);
                        //ReconcileInequalities(dGradient, dCp, dUT);
                        pConvergence.Calculate(this,dUT);
                        pConvergence.AddIterationToLog(this);
                        if (pConvergence.CalculatedPrecision > pConvergence.PreviousCalculatedPrecision)
                        {
                            if (dGradient == 0.0)
                            {
                                if (InitialGradient == 0.0)
                                {
                                    InitialGradient = Precision;
                                    pConvergence.Log += "Action=  Setting initial descent =" + InitialGradient.ToString() + "\r\n";
                                }
                                dGradient = InitialGradient;
                            }
                            else
                            {
                                dGradient = dGradient * GradientChange;
                            }
                            pConvergence.Log += "Action=  Diverging precision, so increasing descent, rotating towards steepest descent direction=" + dGradient.ToString() + "\r\n";
                        }
                        else if (pConvergence.CalculatedPrecision < Precision)
                        {
                            pConvergence.eTermination = Termination.MinimumPrecision;
                            pConvergence.Converged = true;
                        }
                        else if (pConvergence.CostChange < Convergence)
                        {
                            pConvergence.eTermination = Termination.MinimumConvergence;
                            pConvergence.Converged = true;
                        }
                        else
                        {
                            pConvergence.Retain();
                            pResults.RetainFinalValues();
                            pConvergence.Log += "Action=  Retaining values" + "\r\n";
                        }
                    }
                    while (!pConvergence.Converged &&  (pConvergence.Iterations < Iterations) && pConvergence.ElapsedSeconds < MaximumTime);
                    if (pConvergence.Converged) 
                    {
                        pConvergence.Log += "Action=  Checking for inequalities" + "\r\n";
                        ReconcileInequalities(dGradient, dCp);
                        pConvergence.Calculate(this,dUT);
                        pConvergence.AddIterationToLog(this);

                        if (pConvergence.CalculatedPrecision < Precision)
                        {
                            pConvergence.eTermination = Termination.MinimumPrecision;
                            pConvergence.Converged = true;
                        }
                        else if (pConvergence.CostChange < Convergence)
                        {
                            pConvergence.eTermination = Termination.MinimumConvergence;
                            pConvergence.Converged = true;
                        }
                        else
                        {
                            pConvergence.Retain();
                            pResults.RetainFinalValues();
                            pConvergence.Log += "Action=  Retaining values" + "\r\n";
                        }
                    }
                }
                while (!pConvergence.Converged &&  (pConvergence.Iterations < Iterations) && pConvergence.ElapsedSeconds < MaximumTime);

                if (!pConvergence.Converged)
                {
                    if (pConvergence.Iterations >= Iterations)
                    {
                        pConvergence.eTermination = Termination.MaximumIterations;
                    }
                    else
                    {
                        pConvergence.eTermination = Termination.MaximumTime;
                    }
                }
            }
            pResults.RetainFinalValues();
            Diagnose();
            pConvergence.Stop();
            return pConvergence.Converged;
        }

        private void LocateReconciledVariables() //CellsClass Variables)
        {
            //foreach (KeyValuePair<string, CellClass> pKeyVariable in pProblem.Cells)
            //{
            //    if (pKeyVariable.Value.Node == null) //not an expression could test for reconciledvariable
            //    {
            //        AddReconciledVariable(pKeyVariable.Value);
            //    }
            //}
            foreach (CellClass pCell in pProblem.Cells.CellsCollection())
            {
                if (pCell.Node == null) //not an expression could test for reconciledvariable
                {
                    AddReconciledVariable(pCell);
                }
            }
        }

        private ResultClass AddReconciledVariable(CellClass pVariable)
        {
            ResultClass AddReconciledVariable = null;
            ResultClass Result = new ResultClass();
            nResults = nResults + 1;
            pResults.Add(Result);
            Result.Cell = pVariable;
            pVariable.ReconciledVariable = Result;
            AddReconciledVariable = Result;
            Result.Index = nResults;
            return AddReconciledVariable;
        }

        private void UpdateCovariance()
        {
            int j = 0;

            dUT.Reset();
            for (j = 1; j <= nResults; j++)
            {
                dUT[j, j] = ((ResultClass)pResults[j - 1]).InitialTolerance;
            }
        }
        private double CalculatePrecision()
        {
            double dCalculatePrecision = 0.0;
            int j = 0;
            //double dStep = 0.0;
            double dStepTotal = 0.0;
            double dTemp1 = 0.0;
            //double dTemp2 = 0.0;
            //double dDerivative = 0.0;

            //dStep = 0.0;
            dStepTotal = 0.0;

            for (j = 0; j < nResults; j++)
            {
                ((ResultClass)pResults[j]).ReconciledTolerance = Math.Sqrt(dUT[j + 1]);
            }

            foreach (ConstraintClass Balance in pProblem.Constraints)
            {
                if ((Balance.Active == true) || (Balance.Node.OperatorType == Constants.OperatorType.Equal))
                {
                    Balance.MeasuredResidual = Balance.Node.Evaluate(Constants.FunctionType.NULL);
                    Balance.ReconciledDeviation = 0.0;
                    foreach (CellClass Argument in Balance.Dependents)
                    {
                        dTemp1 = Balance.Node.Derivative(Argument,Constants.FunctionType.NULL) * Argument.ReconciledVariable.ReconciledTolerance;
                        //dTemp1 = Balance.Node.DerivativeResidual(Argument) * Argument.ReconciledVariable.ReconciledTolerance;
                        Balance.ReconciledDeviation += dTemp1 * dTemp1;
                    }
                    if (Balance.ReconciledDeviation != 0.0)
                    {

                        dStepTotal += (Balance.MeasuredResidual * Balance.MeasuredResidual) / Balance.ReconciledDeviation;
                        //dStepTotal += (Balance.ReconciledResidual * Balance.ReconciledResidual) / Balance.ReconciledDeviation;
                    }
                    Balance.ReconciledDeviation = System.Math.Sqrt(Balance.ReconciledDeviation);
                }
            }
            dCalculatePrecision = System.Math.Sqrt(dStepTotal) / pProblem.Constraints.ActiveConstraints;
            return dCalculatePrecision;
        }

        private void ReconcileLinearEqualities(double dTolerance, double[] dCp)
        {
            double dResidual = 0.0;
            foreach (ConstraintClass Balance in pProblem.Constraints)
            {
                if ((Balance.Node.Linearity != Constants.LinearityType.Nonlinear) && (Balance.Node.OperatorType == Constants.OperatorType.Equal))
                {
                    dResidual = Balance.Node.Evaluate(Constants.FunctionType.NULL);
                    CalculateGain(Balance, dCp);
                    UpdateEstimate(dResidual, 0, dCp);
                    Balance.Active = true;
                }
            }
        }

        private void ReconcileNonLinearEqualities(double dTolerance, double[] dCp)
        {
            double dResidual = 0.0;
            foreach (ConstraintClass Balance in pProblem.Constraints)
            {
                if ((Balance.Node.Linearity == Constants.LinearityType.Nonlinear) && (Balance.Node.OperatorType == Constants.OperatorType.Equal))
                {
                    dResidual = Balance.Node.Evaluate(Constants.FunctionType.NULL);
                    CalculateGain(Balance, dCp);
                    UpdateEstimate(dResidual, dTolerance, dCp);
                    Balance.Active = true;
                }
            }
        }

        private void ReconcileInequalities(double dGradient, double[] dCp)
        {
            ConstraintClass BalanceMax = null;
            double dResidual = 0.0;
            double dResidualMax = 0.0;
            double dAlpha = 0.0;
            //double dAlphaMax = 0.0;
            double dMultiplier = 0.0;
            double dMultiplierMax = 0.0;
            //double dLastMultiplierMax = -Constants.LARGEVALUE;
            double dPreviousMultiplierMax = Constants.LARGEVALUE;
            Boolean diverging = false;

            do
            {
                dAlpha = 0.0;
                dMultiplier = 0.0;
                dMultiplierMax = 0.0;// Constants.LARGEVALUE; //0.0;
                BalanceMax = null;

                //find the inactive constraint with the largest alpha coefficient

                foreach (ConstraintClass Balance in pProblem.Constraints)
                {
                    if ((Balance.Node.OperatorType != Constants.OperatorType.Equal) && (Balance.Active == false))
                    {
                        dResidual = Balance.Node.Evaluate(Constants.FunctionType.NULL);
                        if (dResidual < 0)
                        {
                            CalculateGain(Balance, dCp);
                            dAlpha = Alpha(dCp);
                            dMultiplier = dResidual / dAlpha;
                            if (dMultiplier < dMultiplierMax)
                            //if (Math.Abs(dMultiplier) < Math.Abs(dMultiplierMax))
                            //if (dAlpha > dAlphaMax)
                            {
                                //dAlphaMax = dAlpha;
                                BalanceMax = Balance;
                                dResidualMax = dResidual;
                                dMultiplierMax = dMultiplier;
                            }
                        }
                    }
                }
                //as long as it is not null then activate this constraint unless we are moving away as indicated by the multiplier increasing (negatively)
                if (!(BalanceMax == null) )
                {
                    if (Math.Abs(dPreviousMultiplierMax) < Math.Abs(dMultiplierMax)) 
                    {
                        diverging = true;
                    }
                    else
                    {
                        //dLastMultiplierMax = dMultiplierMax;
                        dResidualMax = BalanceMax.Node.Evaluate(Constants.FunctionType.NULL);
                        CalculateGain(BalanceMax, dCp);
                        if (BalanceMax.Node.Linearity == Constants.LinearityType.Nonlinear)
                        {
                            UpdateEstimate(dResidualMax, dGradient, dCp);
                        }
                        else
                        {
                            UpdateEstimate(dResidualMax, 0, dCp);
                        }
                        BalanceMax.Active = true;
                        dPreviousMultiplierMax = dMultiplierMax;
                    }
                }
            }
            while (!(BalanceMax == null) && !diverging);
        }

        private double Alpha(double[] dCp)
        {
            int i = 0;
            double dAlpha = 0.0;

            for (i = 1; i <= nResults; i++)
            {
                if (dCp[i] != 0.0)
                {
                    dAlpha += (dCp[i] * dCp[i]);
                }
            }
            return dAlpha;
        }
        private void CalculateGain(ConstraintClass Constraint, double[] dCp)
        {
            double dDerivative = 0.0;
            int i = 0;
            int j = 0;

            for (i = 1; i <= nResults; i++)
            {
                dCp[i] = 0.0;
            }

            foreach (CellClass Argument in Constraint.Dependents)
            {
                j = Argument.ReconciledVariable.Index;
                dDerivative = Constraint.Node.Derivative(Argument, Constants.FunctionType.NULL);

                dCp[j] -= (dUT[j, j] * dDerivative);

                if (j < nResults)
                {
                    for (i = j + 1; i <= nResults; i++)
                    {
                        dCp[i] -= (dUT[i, j] * dDerivative);
                    }
                }
            }
        }
        private void UpdateEstimate(double dResidual, double dTolerance, double[] dCp)
        {

            int j = 0;
            //int i = 0;
            int k = 0;
            double dAlpha = 0.0;
            double dSAlpha = 0.0;
            //double dLastAlpha = 0.0;
            double dLastSAlpha = 0.0;
            //double dGamma = 0.0;
            double dSGamma = 0.0;
            double dDelta = 0.0;
            double dLambda = 0.0;
            double dSigma = 0.0;
            double dLastSAlphaSGamma = 0.0;
            double dLambdaSAlpha = 0.0;
            double dTemp = 0.0;
            double[] dTemporary = new double[nResults + 1];
            //bool eSingularity = true;

            // SSt Factorization
            for (j = 1; j <= nResults; j++)
            {
                dTemporary[j] = dCp[j];
            }

            dAlpha = (dTolerance * dTolerance) + (dTemporary[1] * dTemporary[1]);
            dSAlpha = System.Math.Sqrt(dAlpha);

            //dGamma = 0.0;
            //dSGamma = 0.0;

            if (dAlpha > 0.0)
            {
                dSGamma = 1 / dSAlpha;
            }

            if (dTemporary[1] != 0.0)
            {
                dTemporary[1] = dTemporary[1] * dUT[1, 1];
                dUT[1, 1] = dUT[1, 1] * dTolerance * dSGamma;
            }
            for (j = 2; j <= nResults; j++)
            {

                dLastSAlpha = dSAlpha;
                //dLastAlpha = dAlpha;

                dDelta = dCp[j];
                dAlpha = dAlpha + (dDelta * dCp[j]);

                if (System.Math.Abs(dAlpha) > Constants.SMALLVALUE)
                {
                    //eSingularity = false;
                    dSAlpha = System.Math.Sqrt(dAlpha);
                    dLambda = (-dDelta) * dSGamma;

                    dSGamma = 1.0 / dSAlpha;

                    dLastSAlphaSGamma = dLastSAlpha / dSAlpha;
                    dLambdaSAlpha = dLambda / dSAlpha;

                    if (dDelta != 0.0)
                    {
                        for (k = 1; k <= (j - 1); k++)
                        {
                            dSigma = dUT[j, k];
                            if (dSigma != 0.0)
                            {
                                dUT[j, k] = (dSigma * dLastSAlphaSGamma) + (dLambdaSAlpha * dTemporary[k]);
                                dTemporary[k] += (dDelta * dSigma);
                            }
                            else
                            {
                                dUT[j, k] = dLambdaSAlpha * dTemporary[k];
                            }
                        }
                    }
                    else
                    {
                        for (k = 1; k <= (j - 1); k++)
                        {
                            dSigma = dUT[j, k];
                            if (dSigma != 0.0)
                            {
                                dUT[j, k] = (dSigma * dLastSAlphaSGamma) + (dLambdaSAlpha * dTemporary[k]);
                            }
                            else
                            {
                                dUT[j, k] = dLambdaSAlpha * dTemporary[k];
                            }
                        }
                    }
                    dTemporary[j] *= dUT[j, j];
                    dUT[j, j] = dUT[j, j] * dLastSAlpha * dSGamma;
                }
                else
                {
                    //singularity?
                    dAlpha = 0.0;
                }
            }

            dTemp = dResidual * dSGamma * dSGamma;
            if (System.Math.Abs(dTemp) < Constants.LARGEVALUE) //Test to ensure that Kalman gain has not become unstable due to singularity
            {
                for (j = 1; j <= nResults; j++)
                {
                    ((ResultClass)pResults[j - 1]).Value = ((ResultClass)pResults[j - 1]).Value + (dTemporary[j] * dTemp);
                }
            }
            else
            {
                //Kalman gain has become unstable due to singularity
                dTemp = 0.0;
            }
        }

  
        private void Diagnose()
        {
            pProblem.Results.Diagnose(dUT);
            pConvergence.ReconciledCost = pProblem.Results.CalculateCost() * nResults;
            pConvergence.ReconciledCost *= pConvergence.ReconciledCost;

            //WorksheetFunction wf = xl.WorksheetFunction;
            if (pProblem.Results.RedundancyDegree>0 )
            {
                //dGlobalCriticalValue = wf.ChiInv((1.0 - dConfidence), (double)pProblem.Results.RedundancyDegree);
                //dGlobalCriticalValue = Functions.ArcChiSquare((1.0 - dConfidence), pProblem.Results.RedundancyDegree);
                dGlobalCriticalValue = StatisticsClass.inv_chi_sq((dConfidence), pProblem.Results.RedundancyDegree);
            }
            else
            {
                dGlobalCriticalValue=0;
            }
            pProblem.Results.DiagnoseMeasurements(dConfidence, dStandardTolerance);//, xl);
            pProblem.Constraints.DiagnoseBalanceErrors(dConfidence, dStandardTolerance/*, xl*/, pProblem.Results.FixedValues);
        }

        internal double VariableSensitivity(ResultClass variable, ResultClass measurement)
        {
            double sensitivity = 0;
            double covariance=0;

            for (int k = Math.Max(variable.Index, measurement.Index); k <=nResults; k++)
            {
                    covariance += dUT[ k, variable.Index] * dUT[k, measurement.Index];
            }
            sensitivity += covariance /( ((ResultClass)pResults[measurement.Index - 1]).InitialTolerance*((ResultClass)pResults[measurement.Index - 1]).InitialTolerance);
            return sensitivity;

        }
        internal Nullable<double> ConstraintSensitivity(ConstraintClass constraint, ResultClass measurement)
        {
            double sensitivity = 0;
          
            foreach( CellClass variable in constraint.Dependents)
            {
                sensitivity += constraint.Node.Derivative(variable, Constants.FunctionType.NULL) * VariableSensitivity(variable.ReconciledVariable, measurement);
            }
            return sensitivity;
        }
    }
}