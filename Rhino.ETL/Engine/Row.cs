using System;
using System.Collections;
using System.Diagnostics;
using Rhino.ETL.Impl;
using System.Collections.Generic;

namespace Rhino.ETL
{
	[DebuggerDisplay("Count = {items.Count}")]
	[DebuggerTypeProxy(typeof(Rhino.ETL.Impl.QuackingDictionary.QuackingDictionaryDebugView))]
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

		
		public void Copy(Row row)
		{
			items = new Hashtable(row.items, StringComparer.InvariantCultureIgnoreCase);
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

		public ObjectArrayKeys CreateKey(IEnumerable columns)
		{
			ArrayList list = new ArrayList();
			columns = columns ?? Columns;
			foreach (string column in columns)
			{
				list.Add(items[column]);
			}
			return new ObjectArrayKeys(list.ToArray());
		}
    }
}
