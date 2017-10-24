namespace com.inova8.resolver
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    public class ResultsClass : IEnumerable
    {
        private List<ResultClass> pResults = new List<ResultClass>();
        private int nRedundancyDegree = 0;
        private int nFixedValues = 0;
        private int nDistinctMeasurementErrors = 0;
        private double dMeasurementCriticalValue = 0.0;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pResults.GetEnumerator();
        }
        //public void Sort()
        //{
        //    pResults.Sort();
        //    return;
        //}
        public int Count
        {
            get { return pResults.Count; }
        }
        public int FixedValues
        {
            //get{
            //    int Count =0;
            //    foreach (ResultClass result in pResults)
            //    {
            //        if (result.Cell.hasMeasurement && (result.Solvability == Constants.SolvabilityType.Fixed)) Count++;
            //    }
            //    return Count;
            //}
            get {return nFixedValues; }
            internal set { nFixedValues = value; }
        }
        public int RedundancyDegree
        {
            get { return nRedundancyDegree; }
            internal set {nRedundancyDegree=value; }
        }
        public int DistinctMeasurementErrors
        {
            get { return nDistinctMeasurementErrors; }
            internal set { nDistinctMeasurementErrors = value; }
        }
        public double MeasurementCriticalValue
        {
            get { return dMeasurementCriticalValue; }
            internal set { dMeasurementCriticalValue = value; }
        }
        internal ResultClass this[int index]
        {
            get { return pResults[index]; }
            set { pResults[index] = value; }
        }
        internal void Add(ResultClass Result)
        {
            pResults.Add(Result);
        }
        internal void UpdateMeasurement()
        {
            foreach (ResultClass result in pResults)
            {
                result.Value = result.ReconciledValue;
            }
        }
        internal void InitializeMeasurements()
        {
            foreach (ResultClass result in pResults)
            {
                if (result.Cell.hasMeasurement)
                {
                    result.InitialValue = result.Cell.MeasuredValue.Value;
                    if (result.Cell.MeasuredTolerance != null)
                    {
                        result.InitialTolerance = result.Cell.MeasuredTolerance.Value;
                    }
                    else { result.InitialTolerance = Constants.LARGEVALUE; }
                }
                else
                {
                    result.InitialValue = result.Cell.InitialValue.Value;
                    result.InitialTolerance = Constants.LARGEVALUE;
                }
            }
        }
        internal void RetainFinalValues()
        {
            foreach (ResultClass result in pResults)
            {
                result.ReconciledValue = result.Value;
            }
        }
        internal void DeduceSolvability()
        {
            foreach (ResultClass result in pResults)
            {
                if (result.Cell.hasMeasurement)
                {
                    // check for redundancy
                    if (result.InitialTolerance == 0)
                    {
                        result.Solvability = Constants.SolvabilityType.Fixed;
                    }
                    else if ((result.InitialTolerance - result.ReconciledTolerance) < (Constants.ETA * result.InitialTolerance))
                    {
                        result.Solvability = Constants.SolvabilityType.Determined;
                    }
                    else
                    {
                        result.Solvability = Constants.SolvabilityType.Redundant;
                    }
                }
                else
                {
                    // check for solvability
                    if (result.ReconciledTolerance > (result.InitialTolerance * Constants.ACCURACY))
                    {
                        result.Solvability = Constants.SolvabilityType.Unobservable;
                    }
                    else
                    {
                        result.Solvability = Constants.SolvabilityType.Observable;
                    }
                }
            }
        }
        internal void Diagnose(CovClass dUT)
        {
            int i = 0;
            int nNonSolvable = 0;
            for (i = 0; i < pResults.Count; i++)
            {
                ((ResultClass)pResults[i]).ReconciledTolerance = System.Math.Sqrt(dUT[i + 1]);
            }

            nNonSolvable = 0;
            nRedundancyDegree = 0;
            nFixedValues = 0;
            this.DeduceSolvability();

            foreach (ResultClass result in pResults)
            {
                if (result.Solvability == Constants.SolvabilityType.Unobservable)
                {
                    nNonSolvable += 1;
                }
                if (result.Solvability == Constants.SolvabilityType.Redundant)
                {
                    nRedundancyDegree += 1;
                }
                if (result.hasMeasurement && result.MeasuredTolerance == 0)
                {
                    //count the number of measurements which should really be constraints because they have a tolerance of 0
                    nFixedValues += 1;
                    //debit the Redundancy degree if this 0 tol measurement was classified as redundant.
                    if (result.Solvability == Constants.SolvabilityType.Redundant)
                    {
                        nRedundancyDegree -= 1;
                    }
                }
            }
        }
        internal void DiagnoseMeasurements(double dConfidence, double dStandardTolerance)
        {
            //double dMeasurementMaximumValue = 0.0;
            // used to correct measurements to assumption of 95% confidence and tolerance provided at 2*sd
            //dMeasurementMaximumValue = StatisticsClass.inv_normal(1.0 - dConfidence) / dStandardTolerance;

            foreach (ResultClass result in pResults)
            {
                if (result.Cell.hasMeasurement)
                {
                    result.MeasuredError = result.Value - result.InitialValue.Value;
                    if (result.ReconciledTolerance > Constants.ETA)
                    {
                        result.ReconciledTest = result.MeasuredError / result.ReconciledTolerance;
                        //Only count if tolerance is not 0
                        nDistinctMeasurementErrors = nDistinctMeasurementErrors + 1;
                    }
                    else
                    {
                        result.ReconciledTest = 0.0;
                    }
                }
                else
                {
                    result.MeasuredError = 0.0;
                    result.ReconciledTest = 0.0;
                }
            }

            if (nDistinctMeasurementErrors == 0)
            {
                dMeasurementCriticalValue = 0;// xl.WorksheetFunction.NormSInv(0);
            }
            else
            {
                double beta = (1 - System.Math.Pow((1 - dConfidence), (1.0 / nDistinctMeasurementErrors)));
                //dMeasurementCriticalValue = xl.WorksheetFunction.NormSInv(1.0 - beta / 2);
                //dMeasurementCriticalValue = Functions.NormSInv(1.0 - beta / 2);
                dMeasurementCriticalValue = StatisticsClass.inv_normal(1.0 - beta / 2);

                foreach (ResultClass result in pResults)
                {
                    if (result.InitialTolerance < Constants.ETA)
                    {
                        result.MeasuredTest = result.MeasuredError / Constants.ETA;
                    }
                    else
                    {
                        result.MeasuredTest = result.MeasuredError / result.InitialTolerance; //   / dMeasurementMaximumValue;
                    }
                    //result.MeasurementTest = result.MeasurementTest / dMeasurementCriticalValue;
                }
            }
        }
        internal double CalculateCost()
        {
            double CalculateCost = 0.0;
            double dInitialTolerance = 0.0;
            double dIncrementalCost = 0.0;

            foreach (ResultClass result in pResults)
            {
                dInitialTolerance = result.InitialTolerance;
                if (result.Cell.hasMeasurement && dInitialTolerance > 0.0)
                {
                    dIncrementalCost = (result.Value - result.InitialValue.Value) / dInitialTolerance;
                    CalculateCost += dIncrementalCost * dIncrementalCost;
                }
            }
            return System.Math.Sqrt(CalculateCost) / pResults.Count;
        }

    }
}