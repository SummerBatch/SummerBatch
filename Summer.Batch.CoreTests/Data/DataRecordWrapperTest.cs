using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Data;

namespace Summer.Batch.CoreTests.Data
{
    [TestClass]
    public class DataRecordWrapperTest
    {
        [TestMethod]
        public void TestGetNullable1()
        {
            var dataRecordWrapper = new DataRecordWrapper(new MockDataRecord(2));

            var result = dataRecordWrapper.Get<int?>(1);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestGetNullable2()
        {
            var dataRecordWrapper = new DataRecordWrapper(new MockDataRecord(null));

            var result = dataRecordWrapper.Get<int?>(1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetNullable3()
        {
            var dataRecordWrapper = new DataRecordWrapper(new MockDataRecord(2L));

            var result = dataRecordWrapper.Get<int?>(1);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestGetNullable4()
        {
            var now = DateTime.Now;
            var dataRecordWrapper = new DataRecordWrapper(new MockDataRecord(now));

            var result = dataRecordWrapper.Get<DateTime?>(1);

            Assert.AreEqual(now, result);
        }

        [TestMethod]
        public void TestGetNullable5()
        {
            var dataRecordWrapper = new DataRecordWrapper(new MockDataRecord(null));

            var result = dataRecordWrapper.Get<DateTime?>(1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetDecimal1()
        {
            var dataRecordWrapper = new DataRecordWrapper(new MockDataRecord(3m));

            var result = dataRecordWrapper.Get<long>(1);

            Assert.AreEqual(3L, result);
        }

        [TestMethod]
        public void TestGetDecimal2()
        {
            var dataRecordWrapper = new DataRecordWrapper(new MockDataRecord(3m));

            var result = dataRecordWrapper.Get<long?>(1);

            Assert.AreEqual(3L, result);
        }

        private class MockDataRecord : IDataRecord
        {
            private readonly object _value;

            public MockDataRecord(object value)
            {
                _value = value;
            }

            public object GetValue(int i)
            {
                return _value;
            }

            public bool IsDBNull(int i)
            {
                return _value == null;
            }

            public string GetName(int i)
            {
                throw new NotImplementedException();
            }

            public string GetDataTypeName(int i)
            {
                throw new NotImplementedException();
            }

            public Type GetFieldType(int i)
            {
                throw new NotImplementedException();
            }

            public int GetValues(object[] values)
            {
                throw new NotImplementedException();
            }

            public int GetOrdinal(string name)
            {
                throw new NotImplementedException();
            }

            public bool GetBoolean(int i)
            {
                throw new NotImplementedException();
            }

            public byte GetByte(int i)
            {
                throw new NotImplementedException();
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public char GetChar(int i)
            {
                throw new NotImplementedException();
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public Guid GetGuid(int i)
            {
                throw new NotImplementedException();
            }

            public short GetInt16(int i)
            {
                throw new NotImplementedException();
            }

            public int GetInt32(int i)
            {
                throw new NotImplementedException();
            }

            public long GetInt64(int i)
            {
                throw new NotImplementedException();
            }

            public float GetFloat(int i)
            {
                throw new NotImplementedException();
            }

            public double GetDouble(int i)
            {
                throw new NotImplementedException();
            }

            public string GetString(int i)
            {
                throw new NotImplementedException();
            }

            public decimal GetDecimal(int i)
            {
                throw new NotImplementedException();
            }

            public DateTime GetDateTime(int i)
            {
                throw new NotImplementedException();
            }

            public IDataReader GetData(int i)
            {
                throw new NotImplementedException();
            }

            public int FieldCount { get { return 1; } }

            object IDataRecord.this[int i]
            {
                get { throw new NotImplementedException(); }
            }

            object IDataRecord.this[string name]
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}