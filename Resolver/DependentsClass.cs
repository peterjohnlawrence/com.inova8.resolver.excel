
namespace com.inova8.resolver
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    public class DependentsClass : IEnumerable
    {
        private List<DependentClass> pDependents = new List<DependentClass>();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return pDependents.GetEnumerator();
        }
        //public void Sort()
        //{
        //    pDependents.Sort();
        //    return;
        //}
        internal void LinkExpressions()
        {
            foreach (DependentClass Expression in pDependents)
            {
                Expression.Node.LinkExpressions();
            }
        }
        internal void LocateDependentVariables()
        {
            foreach (DependentClass dependent in pDependents)
            {
                dependent.Node.LocateDependentArguments(dependent);
            }
        }
        public int Count
        {
            get { return pDependents.Count;  }
        }
        internal void Add(NodeClass Dependent)
        {
            DependentClass pDependent = new DependentClass();
            pDependent.Node = Dependent;
            pDependents.Add(pDependent);
        }
        internal DependentClass Item(int index)
        {
            DependentClass Item = null;
            Item = (DependentClass)pDependents[index - 1];
            return Item;
        }
    }
}