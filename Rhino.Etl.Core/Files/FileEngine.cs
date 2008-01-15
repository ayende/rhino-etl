namespace Rhino.Etl.Core.Files
{
    using System;
    using System.Collections;
    using System.Text;
    using FileHelpers;

    /// <summary>
    /// Adapter class to facilitate the nicer syntax
    /// </summary>
    public class FileEngine : IDisposable, IEnumerable
    {
        private readonly FileHelperAsyncEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEngine"/> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public FileEngine(FileHelperAsyncEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Writes the specified object ot the file
        /// </summary>
        /// <param name="t">The t.</param>
        public void Write(object t)
        {
            engine.WriteNext(t);
        }

        /// <summary>
        /// Set the behavior on error
        /// </summary>
        /// <param name="errorMode">The error mode.</param>
        public FileEngine OnError(ErrorMode errorMode)
        {
            engine.ErrorMode = errorMode;
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return engine.ErrorManager.HasErrors;  }
        }

        /// <summary>
        /// Outputs the errors to the specified file
        /// </summary>
        /// <param name="file">The file.</param>
        public void OutputErrors(string file)
        {
            engine.ErrorManager.SaveErrors(file);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IDisposable d = engine;
            d.Dispose();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            IEnumerable e = engine;
            return e.GetEnumerator();
        }
    }
}