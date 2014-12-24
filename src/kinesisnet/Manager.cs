using System;
using Amazon;
using Amazon.Kinesis;
using KinesisNet.Interface;
using KinesisNet.Persistance;
using Serilog;

namespace KinesisNet
{
    public class KManager
    {
        private readonly AmazonKinesisClient _client;

        private readonly IConsumer _consumer = null;
        private readonly IProducer _producer = null;
        private readonly IUtilities _utilities = null;
        private readonly IDynamoDB _dynamoDb = null;

        public KManager(string awsKey, string awsSecret, string streamName, AmazonKinesisConfig config, string workerId = null)
        {
            if (workerId == null)
            {
                workerId = Environment.MachineName;
            }

            _client = new AmazonKinesisClient(awsKey, awsSecret, config);

            _utilities = new Utilities(_client, streamName, workerId);
            _dynamoDb = new DynamoDB(awsKey, awsSecret, config.RegionEndpoint, _utilities);

            _producer = new Producer(_client, streamName);
            _consumer = new Consumer(_client, _utilities, _dynamoDb);

            Log.Information("Instantiated KManager");
        }

        public KManager(string awsKey, string awsSecret, string streamName, RegionEndpoint regionEndpoint, string workerId = null) : 
            this(awsKey, awsSecret, streamName, new AmazonKinesisConfig() {RegionEndpoint = regionEndpoint}, workerId)
        {
            
        }

        public IConsumer Consumer
        {
            get { return _consumer; }
        }

        public IProducer Producer
        {
            get { return _producer; }
        }

        public IUtilities Utilities
        {
            get { return _utilities; }
        }
    }
}
