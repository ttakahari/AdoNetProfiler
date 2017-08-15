using System;
using System.Collections;
#if !NETSTANDARD1_6
using System.Data;
#endif
using System.Data.Common;

namespace AdoNetProfiler
{
    /// <summary>
    /// The database data reader wrapped <see cref="DbDataReader"/>. 
    /// </summary>
    public class AdoNetProfilerDbDataReader : DbDataReader
    {
        private readonly DbDataReader _reader;
        private readonly IAdoNetProfiler _profiler;
        private int _records;

        /// <inheritdoc cref="DbDataReader.Depth" />
        public override int Depth => _reader.Depth;

        /// <inheritdoc cref="DbDataReader.FieldCount" />
        public override int FieldCount => _reader.FieldCount;

        /// <inheritdoc cref="DbDataReader.HasRows" />
        public override bool HasRows => _reader.HasRows;

        /// <inheritdoc cref="DbDataReader.IsClosed" />
        public override bool IsClosed => _reader.IsClosed;

        /// <inheritdoc cref="DbDataReader.RecordsAffected" />
        public override int RecordsAffected => _reader.RecordsAffected;

        /// <summary>
        /// Get a value as <see cref="object"/> by the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The value.</returns>
        public override object this[string name] => _reader[name];

        /// <summary>
        /// Get a value as <see cref="object"/> by the index.
        /// </summary>
        /// <param name="ordinal">The index.</param>
        /// <returns>The value.</returns>
        public override object this[int ordinal] => _reader[ordinal];
        
        internal AdoNetProfilerDbDataReader(DbDataReader reader, IAdoNetProfiler profiler)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            _reader   = reader;
            _profiler = profiler;
        }

#if !NETSTANDARD1_6
        /// <inheritdoc cref="DbDataReader.Close()" />
        public override void Close()
        {
            _reader.Close();
        }
#endif

        /// <inheritdoc cref="DbDataReader.GetBoolean(int)" />
        public override bool GetBoolean(int ordinal)
        {
            return _reader.GetBoolean(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetByte(int)" />
        public override byte GetByte(int ordinal)
        {
            return _reader.GetByte(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetBytes(int, long, byte[], int, int)" />
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return _reader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        /// <inheritdoc cref="DbDataReader.GetChar(int)" />
        public override char GetChar(int ordinal)
        {
            return _reader.GetChar(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetChars(int, long, char[], int, int)" />
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return _reader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        /// <inheritdoc cref="DbDataReader.GetDataTypeName(int)" />
        public override string GetDataTypeName(int ordinal)
        {
            return _reader.GetDataTypeName(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetDateTime(int)" />
        public override DateTime GetDateTime(int ordinal)
        {
            return _reader.GetDateTime(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetDecimal(int)" />
        public override decimal GetDecimal(int ordinal)
        {
            return _reader.GetDecimal(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetDouble(int)" />
        public override double GetDouble(int ordinal)
        {
            return _reader.GetDouble(ordinal);
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator()" />
        public override IEnumerator GetEnumerator()
        {
            return _reader.GetEnumerator();
        }

        /// <inheritdoc cref="DbDataReader.GetFieldType(int)" />
        public override Type GetFieldType(int ordinal)
        {
            return _reader.GetFieldType(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetFloat(int)" />
        public override float GetFloat(int ordinal)
        {
            return _reader.GetFloat(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetGuid(int)" />
        public override Guid GetGuid(int ordinal)
        {
            return _reader.GetGuid(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetInt16(int)" />
        public override short GetInt16(int ordinal)
        {
            return _reader.GetInt16(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetInt32(int)" />
        public override int GetInt32(int ordinal)
        {
            return _reader.GetInt32(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetInt64(int)" />
        public override long GetInt64(int ordinal)
        {
            return _reader.GetInt64(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetName(int)" />
        public override string GetName(int ordinal)
        {
            return _reader.GetName(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetOrdinal(string)" />
        public override int GetOrdinal(string name)
        {
            return _reader.GetOrdinal(name);
        }

#if !NETSTANDARD1_6
        /// <inheritdoc cref="DbDataReader.GetSchemaTable()" />
        public override DataTable GetSchemaTable()
        {
            return _reader.GetSchemaTable();
        }
#endif

        /// <inheritdoc cref="DbDataReader.GetString(int)" />
        public override string GetString(int ordinal)
        {
            return _reader.GetString(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetValue(int)" />
        public override object GetValue(int ordinal)
        {
            return _reader.GetValue(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.GetValues(object[])" />
        public override int GetValues(object[] values)
        {
            return _reader.GetValues(values);
        }

        /// <inheritdoc cref="DbDataReader.IsDBNull(int)" />
        public override bool IsDBNull(int ordinal)
        {
            return _reader.IsDBNull(ordinal);
        }

        /// <inheritdoc cref="DbDataReader.NextResult()" />
        public override bool NextResult()
        {
            return _reader.NextResult();
        }

        /// <inheritdoc cref="DbDataReader.Read()" />
        public override bool Read()
        {
            var result = _reader.Read();

            if (result)
            {
                _records++;
            }

            return result;
        }

        /// <summary>
        /// Free, release, or reset managed or unmanaged resources.
        /// </summary>
        /// <param name="disposing">Wether to free, release, or resetting unmanaged resources or not.</param>
        protected override void Dispose(bool disposing)
        {
            _profiler.OnReaderFinish(this, _records);

            if (disposing)
            {
                _reader.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
