
namespace com.inova8.resolver
{
    using System;
    public class ResultClass //: IComparable<ResultClass>
    {
        private double dValue = 0.0;
        //private double dInitialValue = 0.0;
        private Nullable<double> dInitialValue = 0.0;

        private double dReconciledValue = 0.0;
        private double dReconciledTolerance = 0.0;
        private double dInitialTolerance = 0.0;

        private double dMeasuredTest = 0.0;
        private double dReconciledTest = 0.0;
        private double dMeasuredError = 0.0;
        private CellClass pCell = null;
        private Constants.SolvabilityType eSolvable = Constants.SolvabilityType.Unobservable;
        private int nIndex = 0;

        internal CellClass Cell
        {
            get { return pCell; }
            set { pCell = value; }
        }
        public double Value
        {
            get { return dValue; }
            set { dValue = value; }
        }
        public bool hasMeasurement
        {
            get { return pCell.hasMeasurement; }
        }
        public Nullable<double> InitialValue
        {
            get { return dInitialValue; }
            set { dInitialValue = value; }
        }
        public Nullable<double> MeasuredValue
        {
            get { return pCell.MeasuredValue; }
        }
        public double ReconciledValue
        {
            get { return dReconciledValue; }
            set { dReconciledValue = value; }
        }
        public double MeasuredTest
        {
            get { return dMeasuredTest; }
            set { dMeasuredTest = value; }
        }
        public double ReconciledTest
        {
            get { return dReconciledTest; }
            set { dReconciledTest = value; }
        }
        public double MeasuredError
        {
            get { return dMeasuredError; }
            set { dMeasuredError = value; }
        }

        public double InitialTolerance
        {
            get { return dInitialTolerance; }
            set { dInitialTolerance = value; }
        }
        public Nullable<double> MeasuredTolerance
        {
            get { return pCell.MeasuredTolerance; }
        }
        public double ReconciledTolerance
        {
            get { return dReconciledTolerance; }
            set { dReconciledTolerance = value; }
        }
        public System.String CellAddress
        {
            get { return pCell.Address; }
        }
        public System.String CellName
        {
            get { return pCell.Name; }
        }
        public System.String SolvabilityText
        {
            get { return Constants.SolvableString(eSolvable); }
        }
        internal Constants.SolvabilityType Solvability
        {
            get { return eSolvable; }
            set { eSolvable = value; }
        }
        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; }
        }

    }
}