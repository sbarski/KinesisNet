using System.Collections.Generic;
using Amazon.Kinesis.Model;

namespace KinesisNet.Interface
{
    public interface IRecordProcessor
    {
        void Process(string shardId, IList<Record> records);
    }
}
