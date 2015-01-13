﻿using System;
using System.Collections.Generic;
using Amazon.Kinesis.Model;

namespace KinesisNet.Interface
{
    public interface IRecordProcessor
    {
        void Process(string shardId, string sequenceNumber, DateTime lastUpdate, IList<Record> records, Action<string, string, DateTime> saveCheckpoint);
    }
}
