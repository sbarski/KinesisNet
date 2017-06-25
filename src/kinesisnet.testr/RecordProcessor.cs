using System;
using System.Collections.Generic;
using System.Threading;
using Amazon.Kinesis.Model;
using KinesisNet.Interface;

namespace KinesisNet.Test
{
    public class RecordProcessor : IRecordProcessor
    {
        public void Process(string shardId, string sequenceNumber, DateTime lastUpdate, IList<Record> records, Action<string, string, DateTime> saveCheckpoint)
        {
            Console.WriteLine(Thread.CurrentThread);
        }
    }
}
