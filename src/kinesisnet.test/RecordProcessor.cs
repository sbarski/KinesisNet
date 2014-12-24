using System;
using System.Collections.Generic;
using System.Threading;
using Amazon.Kinesis.Model;
using KinesisNet.Interface;

namespace KinesisNet.Test
{
    public class RecordProcessor : IRecordProcessor
    {
        public void Process(string shardId, IList<Record> records)
        {
            Console.WriteLine(Thread.CurrentThread);
        }
    }
}
