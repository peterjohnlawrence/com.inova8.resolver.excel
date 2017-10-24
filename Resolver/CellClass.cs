
namespace com.inova8.resolver
{
    using System;
    public class CellClass //: IComparable<CellClass>
    {
        private System.String pAddress = "";
        private System.String pName = "";
        private NodeClass pNode = null;
        private ResultClass pReconciledVariable = null;
        private Nullable<double> pInitialValue = null;
        private Nullable<double> pMeasuredTolerance = null;
        private Nullable<double> pMeasuredValue = null;
        private bool phasMeasurement = false;
        //public virtual int CompareTo(CellClass value)
        //{
        //    return pAddress.CompareTo(value.Address);
        //}
        internal Nullable<double> InitialValue
        {
            get
            {
                if (pInitialValue.HasValue)
                {
                    return pInitialValue;
                }
                else if (pMeasuredValue.HasValue)
                {
                    return pMeasuredValue;
                }
                else
                {
                    return 0.0;
                }
            }
            set { pInitialValue = value; }
        }
        public Nullable<double> MeasuredTolerance
        {
            get { return pMeasuredTolerance; }
            set { pMeasuredTolerance = value; }
        }
        public Nullable<double> MeasuredValue
        {
            get { return pMeasuredValue; }
            set { pMeasuredValue = value; }
        }
        internal bool hasMeasurement
        {
            get { return phasMeasurement; }
            set { phasMeasurement = value; }
        }
        internal ResultClass ReconciledVariable
        {
            get { return pReconciledVariable; }
            set { pReconciledVariable = value; }

        }
        internal System.String Address
        {
            get { return pAddress; }
            set { pAddress = value; }

        }
        internal System.String Name
        {
            get { return pName; }
            set { pName = value; }

        }
        internal NodeClass Node
        {
            get { return pNode; }
            set { pNode = value; }
        }
    }
}