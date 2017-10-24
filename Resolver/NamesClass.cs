namespace com.inova8.resolver
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class NamesClass
    {
        //public struct CellKey
        //{
        //    public readonly string Address;
        //    public readonly string Name;
        //    public CellKey(string address, string name)
        //    {
        //        Address = address;
        //        Name = name;
        //    }
        //    public override int GetHashCode()
        //    {
        //        return Address.GetHashCode() ^ Name.GetHashCode();
        //    }
        //    public override bool Equals(object o)
        //    {
        //        if (!(o is CellKey)) return false;
        //        return Address.Equals(((CellKey)o).Address) || Name.Equals(((CellKey)o).Name);
        //    }
        //}
        private Dictionary<string, string> pNames = new Dictionary<string, string>();
        private Dictionary<string, string> pAddresses = new Dictionary<string, string>();

        internal void Add(string Address, string Name)
        {
            if (!pNames.ContainsKey(Address)) pNames.Add(Address, Name);
            if (!pAddresses.ContainsKey(Name)) pAddresses.Add(Name, Address);
            return;
        }
        internal int Count
        {
            get { return pNames.Count; }
        }
        internal string Name(string address)
        {
            return pNames[address];
        }
        internal string Address(string name)
        {
            return pAddresses[name];
        }
        internal string Label(string address)
        {
            if (pNames.ContainsKey(address))
            {
                return pNames[address];
            }else
            {
                return address;
            }
        }
        internal string Key(string label)
        {
            if (label[0] == '$')
            {
                return label;
            }
            else
            {
                return pAddresses[label];
            }
        }
        internal bool ContainsKey(string address)
        {
            return pNames.ContainsKey(address);
        }
    }
}
