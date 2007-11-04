using System;
using System.Collections;
using System.Diagnostics;
using Rhino.ETL.Impl;
using System.Collections.Generic;

namespace Rhino.ETL.Engine
{
	using System.Reflection;

	[DebuggerDisplay("Count = {items.Count}")]
	[DebuggerTypeProxy(typeof(Rhino.ETL.Impl.QuackingDictionary.QuackingDictionaryDebugView))]
	public class Row : QuackingDictionary
	{
		static Dictionary<Type, List<PropertyInfo>> propertiesCache = new Dictionary<Type, List<PropertyInfo>>();
		static Dictionary<Type, List<FieldInfo>> fieldsCache = new Dictionary<Type, List<FieldInfo>>();

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
				foreach (string column in new ArrayList(items.Keys))
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

		public static Row FromObject(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");
			Row row = new Row();
			foreach (PropertyInfo property in GetProperties(obj))
			{
				row[property.Name] = property.GetValue(obj, new object[0]);
			}
			foreach (FieldInfo field in GetFields(obj))
			{
				row[field.Name] = field.GetValue(obj);
			}
			return row;
		}

		private static List<PropertyInfo> GetProperties(object obj)
		{
			List<PropertyInfo> properties;
			if (propertiesCache.TryGetValue(obj.GetType(), out properties))
				return properties;
			
			properties = new List<PropertyInfo>();
			foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				if (property.CanRead == false || property.GetIndexParameters().Length > 0)
					continue;
				properties.Add(property);
			}
			propertiesCache[obj.GetType()] = properties;
			return properties;
		}

		private static List<FieldInfo> GetFields(object obj)
		{
			List<FieldInfo> fields;
			if (fieldsCache.TryGetValue(obj.GetType(), out fields))
				return fields;
			
			fields = new List<FieldInfo>();
			foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				fields.Add(fieldInfo);
			}
			fieldsCache[obj.GetType()] = fields;
			return fields;
		}
	}
}