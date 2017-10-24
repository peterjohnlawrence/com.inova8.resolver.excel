
namespace com.inova8.resolver
{
    using System;
    using System.Collections.Generic;
    public class ConstraintClass //: IComparable<ConstraintClass>
    {
        private bool pActive = false;
        private List<CellClass> pDependents = new List<CellClass>(20);
        private NodeClass pNode = null;
        private double pMeasuredResidual = 0.0;
        private double pReconciledResidual = 0.0;
        private bool bVisited = false;
        private double pReconciledDeviation = 0.0;
        private double pMeasuredDeviation = 0.0;
        private double pMeasuredTest = 0.0;
        private double pReconciledTest = 0.0;

        //public virtual int CompareTo(ConstraintClass value)
        //{
        //    return pNode.Address.CompareTo(value.Node.Address);
        //}
        public bool Active
        {
            get { return pActive; }
            set { pActive = value; }
        }
        internal bool Visited
        {
            get { return bVisited; }
            set { bVisited = value; }
        }
        internal int LinearOperatorCount
        {
            get { return pNode.LinearOperatorCount; }
        }
        internal int NonlinearOperatorCount
        {
            get { return pNode.NonlinearOperatorCount; }
        }
        internal int VariableCount
        {
            get { return pNode.VariableCount; }
        }
        internal int Complexity
        {
            get { return pNode.VariableCount + pNode.NonlinearOperatorCount; }
        }
        public List<CellClass> Dependents
        {
            get { return pDependents; }
        }
        public double MeasuredResidual
        {
            get { return pMeasuredResidual; }
            set { pMeasuredResidual = value; }
        }
        public double ReconciledResidual
        {
            get { return pReconciledResidual; }
            set { pReconciledResidual = value; }
        }
        public string Serialize
        {
            get { return pNode.Serialize(true); }
        }
        public string Trace
        {
            get { return pNode.Serialize(false); }
        }
        public double ReconciledDeviation
        {
            get { return pReconciledDeviation; }
            set { pReconciledDeviation = value; }
        }
        public double MeasuredDeviation
        {
            get { return pMeasuredDeviation; }
            set { pMeasuredDeviation = value; }
        }
        public double MeasuredTest
        {
            get { return pMeasuredTest; }
            set { pMeasuredTest = value; }
        }

        public double ReconciledTest
        {
            get { return pReconciledTest; }
            set { pReconciledTest = value; }
        }

        internal NodeClass Node
        {
            get { return pNode; }
            set { pNode = value; }
        }

        public System.String Address
        {
            get { return pNode.Address; }
            set { pNode.Address = value; }
        }

        internal void AddDependent(CellClass nDependent)
        {
            pDependents.Add(nDependent);
        }

        //public CellClass DependentItem(int index)
        //{
        //    CellClass DependentItem = null;
        //    DependentItem = (CellClass)pDependents[index - 1];
        //    return DependentItem;
        //}

        //internal int DependentItemCount
        //{
        //    get { return pDependents.Count; }
        //}

        internal ConstraintClass()
        {
            pActive = false;
        }
    }
}