namespace Rhino.ETL.Engine
{
	public class ObjectArrayKeys
	{
		private object[] columnValues;

		public ObjectArrayKeys(object[] columnValues)
		{
			this.columnValues = columnValues;
		}


		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			ObjectArrayKeys other = obj as ObjectArrayKeys;
			if(other.columnValues.Length!=this.columnValues.Length)
				return false;
			for (int i = 0; i < columnValues.Length; i++)
			{
				if(Equals(this.columnValues[i], other.columnValues[i])==false)
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int result = 0;
			foreach (object value in columnValues)
			{
				if(value==null)
					continue;
				result = 29*result + value.GetHashCode();
			}
			return result;
		}
	}
}