
namespace com.inova8.resolver
{
    using System;
    using System.Collections.Generic;
    public class DependentClass //: IComparable<DependentClass>
    {
        private List<CellClass> pDependents = new List<CellClass>(20);
        private NodeClass pNode = null;

        //public virtual int CompareTo(DependentClass value)
        //{
        //    return pNode.Address.CompareTo(value.Node.Address);
        //}
        public double Evaluation
        {
            get { return pNode.Evaluate(Constants.FunctionType.NULL); }
        }
        public string Serialize
        {
            get { return pNode.Serialize(true); }
        }
        public string Trace
        {
            get { return pNode.Serialize(false); }
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
    }
}