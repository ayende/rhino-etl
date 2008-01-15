namespace Rhino.Etl.Tests.Util
{
    using System;
    using System.Collections.Generic;
    using Core;
    using MbUnit.Framework;
    using Mocks;
    using Rhino.Etl.Core.DataReaders;

    [TestFixture]
    public class DictionaryEnumeratorDataReaderFixture
    {
        [Test]
        public void WillDisposeInternalEnumeratorAndEnumerableWhenDisposed()
        {
            MockRepository mocks = new MockRepository();
            IEnumerable<Row> enumerable = mocks.DynamicMultiMock<IEnumerable<Row>>(typeof(IDisposable));
            IEnumerator<Row> enumerator = mocks.DynamicMock<IEnumerator<Row>>();
            using(mocks.Record())
            {
                SetupResult.For(enumerable.GetEnumerator()).Return(enumerator);
                enumerator.Dispose();
                ((IDisposable)enumerable).Dispose();
            }
            using (mocks.Playback())
            {
                DictionaryEnumeratorDataReader reader =
                    new DictionaryEnumeratorDataReader(new Dictionary<string, Type>(), enumerable);
                reader.Dispose();
            }
        }
    }
}