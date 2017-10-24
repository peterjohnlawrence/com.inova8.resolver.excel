using System;
namespace Resolver
{
	
	public class TokenClass
	{
		virtual protected internal bool Found
		{
			get
			{
				bool getFound = false;
				try
				{
					getFound = pFound;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "getFound");
				}
				return getFound;
			}
			
			set
			{
				try
				{
					pFound = value;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "setFound");
				}
			}
			
		}
		virtual protected internal int LexType
		{
			get
			{
				int getLexType = 0;
				try
				{
					getLexType = pType;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "getLexType");
				}
				return getLexType;
			}
			
			set
			{
				try
				{
					pType = value;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "setLexType");
				}
			}
			
		}
		virtual protected internal System.String Value
		{
			get
			{
				System.String getValue = "";
				try
				{
					//Variant
					getValue = pValue;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "getValue");
				}
				return getValue;
			}
			
			set
			{
				try
				{
					pValue = value;
				}
				catch (System.Exception _e_)
				{
					Err.set_Renamed(_e_, "setValue");
				}
			}
			
		}
		private bool pFound = false;
		private int pType = 0;
		private System.String pValue = "";
	}
}