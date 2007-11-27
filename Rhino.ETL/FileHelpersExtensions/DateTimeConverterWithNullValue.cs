namespace Rhino.ETL.FileHelpersExtensions
{
	using System;
	using FileHelpers;

	public class DateTimeConverterWithNullValue : ConverterBase
	{
		private readonly string mFormat;
		private readonly string nullValue;

		public DateTimeConverterWithNullValue(string format, string nullValue)
		{
			if (string.IsNullOrEmpty(format))
			{
				throw new InvalidOperationException("The format of the DateTime Converter can be null or empty.");
			}
			try
			{
				DateTime.Now.ToString(format);
			}
			catch
			{
				throw new InvalidOperationException("The format: '" + format + " is invalid for the DateTime Converter.");
			}
			mFormat = format;
			this.nullValue = nullValue;
		}

		public override string FieldToString(object from)
		{
			if (from == null)
			{
				return string.Empty;
			}
			return Convert.ToDateTime(from).ToString(mFormat);
		}

		public override object StringToField(string from)
		{
			object val;
			if (string.IsNullOrEmpty(from) || nullValue == from)
			{
				return null;
			}
			try
			{
				val = DateTime.ParseExact(from.Trim(), mFormat, null);
			}
			catch
			{
				string extra;
				if (from.Length > mFormat.Length)
				{
					extra = " There are more chars than in the format string: '" + mFormat + "'";
				}
				else if (from.Length < mFormat.Length)
				{
					extra = " There are less chars than in the format string: '" + mFormat + "'";
				}
				else
				{
					extra = " Using the format: '" + mFormat + "'";
				}
				throw new ConvertException(from, typeof (DateTime), extra);
			}
			return val;
		}
	}
}