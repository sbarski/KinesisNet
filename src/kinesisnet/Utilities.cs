using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using KinesisNet.Interface;
using Serilog;

namespace KinesisNet
{
    internal class Utilities : IUtilities
    {
        private readonly AmazonKinesisClient _client;

        private string _streamName;
        private string _workerId;
        private ILogger _logger;

        private int _readCapacityUnits;
        private int _writeCapacityUnits;

        private static ConcurrentDictionary<string, string> _workingConsumer; 

        public Utilities(AmazonKinesisClient client, string workerId)
        {
            _client = client;

            _readCapacityUnits = 1;
            _writeCapacityUnits = 1;

            if (_workingConsumer == null)
            {
                _workingConsumer = new ConcurrentDictionary<string, string>();
            }

            _logger = new LoggerConfiguration()
                .WriteTo
                .ColoredConsole()
                .CreateLogger();

            Serilog.Log.Logger = _logger;
        }

        public string WorkerId
        {
            get { return _workerId; }
        }

        public string StreamName
        {
            get { return _streamName; }
        }

        public int DynamoReadCapacityUnits
        {
            get { return _readCapacityUnits; }
        }

        public int DynamoWriteCapacityUnits
        {
            get { return _writeCapacityUnits; }
        }

        public IUtilities SetLogConfiguration(LoggerConfiguration configuration)
        {
            _logger = configuration.CreateLogger();

            Serilog.Log.Logger = _logger;

            return this;
        }

        public ILogger Log
        {
            get { return _logger; }
        }

        public SplitShardResponse SplitShard(Shard shard)
        {
            var startingHashKey = BigInteger.Parse(shard.HashKeyRange.StartingHashKey);
            var endingHashKey = BigInteger.Parse(shard.HashKeyRange.EndingHashKey);
            var newStartingHashKey = BigInteger.Divide(BigInteger.Add(startingHashKey, endingHashKey), new BigInteger(2));

            var splitShardRequest = new SplitShardRequest { StreamName = _streamName, ShardToSplit = shard.ShardId, NewStartingHashKey = newStartingHashKey.ToString() };

            var response = _client.SplitShard(splitShardRequest);

            return response;
        }

        public MergeShardsResponse MergeShards(string leftShard, string rightShard)
        {
            var mergeShardRequest = new MergeShardsRequest
            {
                ShardToMerge = leftShard,
                AdjacentShardToMerge = rightShard,
                StreamName = _streamName
            };

            var response = _client.MergeShards(mergeShardRequest);

            return response;
        }

        public DescribeStreamResponse GetStreamResponse()
        {
            var request = new DescribeStreamRequest() { StreamName = _streamName };

            return _client.DescribeStream(request);
        }

        public async Task<DescribeStreamResponse> GetStreamResponseAsync()
        {
            var request = new DescribeStreamRequest() { StreamName = _streamName };

            return await _client.DescribeStreamAsync(request);
        }

        public IList<Shard> GetActiveShards()
        {
            return GetShards().Where(m => m.SequenceNumberRange.EndingSequenceNumber == null).ToList();
        }

        public IList<Shard> GetDisabledShards()
        {
            return GetShards().Where(m => m.SequenceNumberRange.EndingSequenceNumber != null).ToList();
        }

        public IList<Shard> GetShards()
        {
            var stream = GetStreamResponse();

            return stream.StreamDescription.Shards;
        }

        public async Task<IList<Shard>> GetDisabledShardsAsync()
        {
            var shards = await GetShardsAsync();

            return shards.Where(m => m.SequenceNumberRange.EndingSequenceNumber != null).ToList();
        }

        public async Task<IList<Shard>> GetActiveShardsAsync()
        {
            var shards = await GetShardsAsync();

            return shards.Where(m => m.SequenceNumberRange.EndingSequenceNumber == null).ToList();
        }

        public async Task<IList<Shard>> GetShardsAsync()
        {
            var stream = await GetStreamResponseAsync();

            return stream.StreamDescription.Shards;
        }

        public IUtilities SetDynamoReadCapacityUnits(int readCapacityUnits)
        {
            _readCapacityUnits = readCapacityUnits;

            return this;
        }

        public IUtilities SetDynamoWriteCapacityUnits(int writeCapacityUnits)
        {
            _writeCapacityUnits = writeCapacityUnits;

            return this;
        }

        public async Task<ListStreamsResponse> ListStreamsAsync(string exclusiveStreamStartName = null)
        {
            var listStreamsRequest = new ListStreamsRequest()
            {
                ExclusiveStartStreamName = exclusiveStreamStartName ?? default(string)
            };

            var response = await _client.ListStreamsAsync(listStreamsRequest);

            return response;
        }

        public ListStreamsResponse ListStreams(string exclusiveStreamStartName = null)
        {
            var listStreamsRequest = new ListStreamsRequest()
            {
                ExclusiveStartStreamName = exclusiveStreamStartName ?? default(string)
            };

            var response = _client.ListStreams(listStreamsRequest);

            return response;
        }

        public IUtilities SetWorkerId(string workerId)
        {
            _workerId = workerId;

            return this;
        }

        public IUtilities SetStreamName(string streamName)
        {
            _streamName = streamName;

            return this;
        }
    }
}
