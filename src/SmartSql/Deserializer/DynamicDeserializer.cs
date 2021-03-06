﻿using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSql.Deserializer
{
    public class DynamicDeserializer : IDataReaderDeserializer
    {
        public TResult ToSinge<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default;
            dataReader.Read();
            var columns = GetColumns(dataReader);
            object dyRow = ToDynamicRow(dataReader, columns);
            return (TResult)dyRow;
        }

        private DynamicRow ToDynamicRow(DataReaderWrapper dataReader, IDictionary<string, int> columns)
        {
            var values = new object[columns.Count];
            dataReader.GetValues(values);
            return new DynamicRow(columns, values);
        }

        private IDictionary<string, int> GetColumns(DataReaderWrapper dataReader)
        {
            return Enumerable.Range(0, dataReader.FieldCount)
                .Select(i => new KeyValuePair<string, int>(dataReader.GetName(i), i)).ToDictionary((kv) => kv.Key, (kv) => kv.Value);
        }
        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;
            var columns = GetColumns(dataReader);
            while (dataReader.Read())
            {
                object dyRow = ToDynamicRow(dataReader, columns);
                list.Add((TResult)dyRow);
            }
            return list;
        }

        public async Task<TResult> ToSingeAsync<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default;
            await dataReader.ReadAsync();
            var columns = GetColumns(dataReader);
            object dyRow = ToDynamicRow(dataReader, columns);
            return (TResult)dyRow;
        }

        public async Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;
            var columns = GetColumns(dataReader);
            while (await dataReader.ReadAsync())
            {
                object dyRow = ToDynamicRow(dataReader, columns);
                list.Add((TResult)dyRow);
            }
            return list;
        }
    }
}
