namespace Rhino.Etl.Core
{
    /// <summary>
    /// This is a utility clss that allows to treat a set of values as key, so it can be put
    /// into hash tables and retrieved easily.
    /// </summary>
    public class ObjectArrayKeys
    {
        private readonly object[] columnValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectArrayKeys"/> class.
        /// </summary>
        /// <param name="columnValues">The column values.</param>
        public ObjectArrayKeys(object[] columnValues)
        {
            this.columnValues = columnValues;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            ObjectArrayKeys other = obj as ObjectArrayKeys;
            if (other == null || other.columnValues.Length != this.columnValues.Length)
                return false;
            for (int i = 0; i < columnValues.Length; i++)
            {
                if(Equals(this.columnValues[i], other.columnValues[i])==false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
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