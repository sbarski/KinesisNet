using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using KinesisNet.Interface;

namespace KinesisNet
{
    internal class Producer : IProducer
    {
        private readonly AmazonKinesisClient _client;
        private readonly string _streamName;

        public Producer(AmazonKinesisClient client, string streamName)
        {
            _client = client;
            _streamName = streamName;
        }

        public PutRecordResponse PutRecord(string record, string partitionKey = null)
        {
            var bytes = Encoding.UTF8.GetBytes(record);

            return PutRecord(bytes, partitionKey);
        }

        public PutRecordResponse PutRecord(byte[] data, string partitionKey = null)
        {
            using (var ms = new MemoryStream(data))
            {
                var requestRecord = new PutRecordRequest()
                {
                    StreamName = _streamName,
                    Data = ms,
                    PartitionKey = partitionKey ?? Guid.NewGuid().ToString()
                };

                return _client.PutRecord(requestRecord);
            }
        }

        public async Task<PutRecordResponse> PutRecordAsync(string record, string partitionKey = null)
        {
            var bytes = Encoding.UTF8.GetBytes(record);

            return await PutRecordAsync(bytes, partitionKey);
        }

        public async Task<PutRecordResponse> PutRecordAsync(byte[] data, string partitionKey = null)
        {
            using (var ms = new MemoryStream(data))
            {
                var requestRecord = new PutRecordRequest()
                {
                    StreamName = _streamName,
                    Data = ms,
                    PartitionKey = partitionKey ?? Guid.NewGuid().ToString()
                };

                return await _client.PutRecordAsync(requestRecord, CancellationToken.None);
            }
        }
    }
}
