
namespace com.inova8.resolver
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class CellsClass 
    {
        private Dictionary<string, CellClass> pCellAddresses = new Dictionary<string, CellClass>();
        private NamesClass pNames;
        public CellsClass(NamesClass Names)
        {
            pNames = Names;
        }
        public Dictionary<string, CellClass>.ValueCollection CellsCollection()
        {
            return pCellAddresses.Values;
        }
        internal int Count
        {
            get { return pCellAddresses.Count; }
        }
        internal void Add(CellClass Cell)
        {
            pCellAddresses[Cell.Address] = Cell;
            if (pNames.ContainsKey(Cell.Address))
            {
                Cell.Name = pNames.Name(Cell.Address);
            }
            else
            {
                Cell.Name = Cell.Address;
            } 
        }
        internal CellClass AddAddress(System.String Address)
        {
            CellClass AddCell = null;
            CellClass Cell = null;
            if (pCellAddresses.ContainsKey(Address))
            {
                Cell = pCellAddresses[Address];
                Cell.Address = Address;
                if (pNames.ContainsKey(Address))
                {
                    Cell.Name = pNames.Name(Address);
                }
                else
                {
                    Cell.Name = Address;
                }

                AddCell = Cell;
            }
            else
            {
                Cell = new CellClass();
                pCellAddresses[Address] = Cell;

                //optional
                Cell.Address = Address;
                if (pNames.ContainsKey(Address))
                {
                    Cell.Name = pNames.Name(Address);
                }
                else
                {
                    Cell.Name = Address;
                }
                AddCell = Cell;

            }
            return AddCell;

        }
        internal CellClass AddAddress(System.String Address, NodeClass CellNode )
        {
            CellClass AddCell = null;
            CellClass Cell = null;
            if (pCellAddresses.ContainsKey(Address))
            {
                Cell = pCellAddresses[Address];
                if (null != CellNode && Cell.Node == null)
                {
                    Cell.Node = CellNode;
                }
                Cell.Address = Address;
                if (pNames.ContainsKey(Address))
                {
                    Cell.Name = pNames.Name(Address);
                }
                else
                {
                    Cell.Name = Address;
                }

                AddCell = Cell;
            }
            else
            {
                Cell = new CellClass();
                if (null != CellNode)
                {
                    Cell.Node = CellNode;
                }
                pCellAddresses[Address] = Cell;

                //optional
                Cell.Address = Address;
                if (pNames.ContainsKey(Address))
                {
                    Cell.Name = pNames.Name(Address);
                }
                else
                {
                    Cell.Name = Address;
                }
                AddCell = Cell;

            }
            return AddCell;

        }
        internal CellClass AddMeasurement(System.String Address, Nullable<double> Initial, Nullable<double> Measurement, Nullable<double> Tolerance)
        {
            CellClass Cell = null;
            if (pCellAddresses.ContainsKey(Address))
            {
                Cell = pCellAddresses[Address];
                Cell.InitialValue = Initial;
                Cell.MeasuredValue = Measurement;
                Cell.MeasuredTolerance = Tolerance;
                if (Measurement.HasValue)
                {
                    Cell.hasMeasurement = true;
                }
                else
                {
                    Cell.hasMeasurement = false;
                }
                return Cell;
            }
            else
            {
                return null;
            }

        }
        //internal CellClass ItemByAddress(string address)
        //{
        //    return (CellClass)pCellAddresses[address];
        //}
    }
}