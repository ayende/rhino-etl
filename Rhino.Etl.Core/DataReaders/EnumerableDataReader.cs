namespace Rhino.Etl.Core.DataReaders
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;

	/// <summary>
	/// Represent a data reader that is based on IEnumerable implementation.
	/// This is important because we can now pass an in memory generated code to code
	/// that requires this, such as the SqlBulkCopy class.
	/// </summary>
	public abstract class EnumerableDataReader : IDataReader
	{
		/// <summary>
		/// The enumerator that we are iterating on.
		/// Required so subclasses can access the current object.
		/// </summary>
		protected readonly IEnumerator enumerator;
		private long rowCount = 0;
		private bool isClosed = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableDataReader"/> class.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		protected EnumerableDataReader(IEnumerator enumerator)
		{
			this.enumerator = enumerator;
		}

		/// <summary>
		/// Gets the descriptors for the properties that this instance
		/// is going to handle
		/// </summary>
		/// <value>The property descriptors.</value>
		protected abstract IList<Descriptor> PropertyDescriptors { get; }

		#region IDataReader Members

		/// <summary>
		/// Closes the <see cref="T:System.Data.IDataReader"/> Object.
		/// </summary>
		public void Close()
		{
			DoClose();
			isClosed = true;
		}

		/// <summary>
		/// Perform the actual closing of the reader
		/// </summary>
		protected abstract void DoClose();

		/// <summary>
		/// Returns a <see cref="T:System.Data.DataTable"/> that describes the column metadata of the <see cref="T:System.Data.IDataReader"/>.
		/// </summary>
		/// <remarks>
		/// This is a very trivial implementation, anything that tries to do something really fancy with it
		/// may need more information
		/// </remarks>
		/// <returns>
		/// A <see cref="T:System.Data.DataTable"/> that describes the column metadata.
		/// </returns>
		public DataTable GetSchemaTable()
		{
			DataTable table = new DataTable("schema");
			table.Columns.Add("ColumnName", typeof(string));
			table.Columns.Add("ColumnOrdinal", typeof(int));
			table.Columns.Add("DataType", typeof(Type));

			for (int i = 0; i < PropertyDescriptors.Count; i++)
			{
				table.Rows.Add(
					PropertyDescriptors[i].Name,
					i,
					PropertyDescriptors[i].Type
					);
			}
			return table;
		}

		/// <summary>
		/// We do not support mutliply result set
		/// </summary>
		/// <returns>
		/// true if there are more rows; otherwise, false.
		/// </returns>
		public bool NextResult()
		{
			return false;
		}

		/// <summary>
		/// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
		/// </summary>
		/// <returns>
		/// true if there are more rows; otherwise, false.
		/// </returns>
		public bool Read()
		{
			bool next = enumerator.MoveNext();
			if (next)
				rowCount += 1;
			return next;
		}

		/// <summary>
		/// Gets a value indicating the depth of nesting for the current row.
		/// We do not support nesting.
		/// </summary>
		/// <value></value>
		/// <returns>The level of nesting.</returns>
		public int Depth
		{
			get { return 0; }
		}

		/// <summary>
		/// Gets a value indicating whether the data reader is closed.
		/// </summary>
		/// <value></value>
		/// <returns>true if the data reader is closed; otherwise, false.</returns>
		public bool IsClosed
		{
			get { return isClosed; }
		}

		/// <summary>
		/// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
		/// </summary>
		/// <value></value>
		/// <returns>The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.</returns>
		public int RecordsAffected
		{
			get { return -1; }
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Close();
		}

		/// <summary>
		/// Gets the name for the field to find.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The name of the field or the empty string (""), if there is no value to return.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public string GetName(int i)
		{
			return PropertyDescriptors[i].Name;
		}

		/// <summary>
		/// Gets the data type information for the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The data type information for the specified field.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public string GetDataTypeName(int i)
		{
			return PropertyDescriptors[i].Type.Name;
		}

		/// <summary>
		/// Gets the <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The <see cref="T:System.Type"/> information corresponding to the type of <see cref="T:System.Object"/> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)"/>.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public Type GetFieldType(int i)
		{
			return PropertyDescriptors[i].Type;
		}

		/// <summary>
		/// Return the value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The <see cref="T:System.Object"/> which will contain the field value upon return.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public object GetValue(int i)
		{
			return PropertyDescriptors[i].GetValue(enumerator.Current) ?? DBNull.Value;
		}

		/// <summary>
		/// Gets all the attribute fields in the collection for the current record.
		/// </summary>
		/// <param name="values">An array of <see cref="T:System.Object"/> to copy the attribute fields into.</param>
		/// <returns>
		/// The number of instances of <see cref="T:System.Object"/> in the array.
		/// </returns>
		public int GetValues(object[] values)
		{
			for (int i = 0; i < PropertyDescriptors.Count; i++)
			{
				values[i] = PropertyDescriptors[i].GetValue(enumerator.Current);
			}
			return PropertyDescriptors.Count;
		}

		/// <summary>
		/// Return the index of the named field.
		/// </summary>
		/// <param name="name">The name of the field to find.</param>
		/// <returns>The index of the named field.</returns>
		public int GetOrdinal(string name)
		{
			for (int i = 0; i < PropertyDescriptors.Count; i++)
			{
				if (string.Equals(PropertyDescriptors[i].Name, name, StringComparison.InvariantCultureIgnoreCase))
					return i;
			}
			throw new ArgumentException("There is not property with name: " + name);
		}

		/// <summary>
		/// Gets the value of the specified column as a Boolean.
		/// </summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the column.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public bool GetBoolean(int i)
		{
			return (bool)GetValue(i);
		}

		/// <summary>
		/// Gets the 8-bit unsigned integer value of the specified column.
		/// </summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>
		/// The 8-bit unsigned integer value of the specified column.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public byte GetByte(int i)
		{
			return (byte)GetValue(i);
		}

		/// <summary>
		/// We do not support this operation
		/// </summary>
		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the character value of the specified column.
		/// </summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>
		/// The character value of the specified column.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public char GetChar(int i)
		{
			return (char)GetValue(i);
		}

		/// <summary>
		/// We do not support this operation
		/// </summary>
		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Returns the GUID value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The GUID value of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public Guid GetGuid(int i)
		{
			return (Guid)GetValue(i);
		}

		/// <summary>
		/// Gets the 16-bit signed integer value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The 16-bit signed integer value of the specified field.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public short GetInt16(int i)
		{
			return (short)GetValue(i);
		}

		/// <summary>
		/// Gets the 32-bit signed integer value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The 32-bit signed integer value of the specified field.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public int GetInt32(int i)
		{
			return (int)GetValue(i);
		}

		/// <summary>
		/// Gets the 64-bit signed integer value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The 64-bit signed integer value of the specified field.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public long GetInt64(int i)
		{
			return (long)GetValue(i);
		}

		/// <summary>
		/// Gets the single-precision floating point number of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The single-precision floating point number of the specified field.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public float GetFloat(int i)
		{
			return (float)GetValue(i);
		}

		/// <summary>
		/// Gets the double-precision floating point number of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The double-precision floating point number of the specified field.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public double GetDouble(int i)
		{
			return (double)GetValue(i);
		}

		/// <summary>
		/// Gets the string value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The string value of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public string GetString(int i)
		{
			return (string)GetValue(i);
		}

		/// <summary>
		/// Gets the fixed-position numeric value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The fixed-position numeric value of the specified field.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public decimal GetDecimal(int i)
		{
			return (decimal)GetValue(i);
		}

		/// <summary>
		/// Gets the date and time data value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// The date and time data value of the specified field.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public DateTime GetDateTime(int i)
		{
			return (DateTime)GetValue(i);
		}

		/// <summary>
		/// We do not support nesting
		/// </summary>
		public IDataReader GetData(int i)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Return whether the specified field is set to null.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		/// true if the specified field is set to null; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception>
		public bool IsDBNull(int i)
		{
			return GetValue(i) == null || GetValue(i) == DBNull.Value;
		}

		/// <summary>
		/// Gets the number of columns in the current row.
		/// </summary>
		/// <value></value>
		/// <returns>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.</returns>
		public int FieldCount
		{
			get { return PropertyDescriptors.Count; }
		}

		/// <summary>
		/// Gets the <see cref="System.Object"/> with the specified i.
		/// </summary>
		/// <value></value>
		public object this[int i]
		{
			get { return GetValue(i); }
		}

		/// <summary>
		/// Gets the <see cref="System.Object"/> with the specified name.
		/// </summary>
		/// <value></value>
		public object this[string name]
		{
			get { return GetValue(GetOrdinal(name)); }
		}

		#endregion
	}
}