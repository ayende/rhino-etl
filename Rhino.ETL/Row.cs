using System;
using System.Collections;
using Rhino.ETL.Impl;
using System.Collections.Generic;

namespace Rhino.ETL
{
    public class Row : QuackingDictionary
    {
		public Row()
			: base(new Hashtable(StringComparer.InvariantCultureIgnoreCase))
		{
		}

		private Row(IDictionary itemsToClone)
			: base(new Hashtable(itemsToClone, StringComparer.InvariantCultureIgnoreCase))
    	{
    	}

    	public IEnumerable<string> Columns
        {
            get
            {
                //We likely would want to change the row when iterating on the columns, so we
                //want to make sure that we send a copy, to avoid enumeration modified exception
				foreach (string  column in new ArrayList(items.Keys))
            	{
            		yield return column;
            	}
            }
        }

		public Row Clone()
		{
			Row row = new Row(items);
			return row;
		}
    }
}
