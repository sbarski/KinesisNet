using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using Amazon;
using Xunit;

namespace KinesisNet.Test
{
    public class UtilitiesTests
    {
        private readonly string _awsKey;
        private readonly string _awsSecret;
        private readonly string _streamName;
        private readonly RegionEndpoint _regionEndpoint;

        private readonly KManager _manager;

        public UtilitiesTests()
        {
            _awsKey = ConfigurationManager.AppSettings["AWSKey"];
            _awsSecret = ConfigurationManager.AppSettings["AWSSecret"];
            _streamName = ConfigurationManager.AppSettings["AWSStreamName"];
            _regionEndpoint = RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["AWSRegionEndpoint"]);

            _manager = new KManager(_awsKey, _awsSecret, _streamName, _regionEndpoint);
        }

        [Fact]
        public void GetStreamResponse()
        {
            var response = _manager.Utilities.GetStreamResponse();

            Assert.NotNull(response);
            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public async void GetStreamResponseAsync()
        {
            var response = await _manager.Utilities.GetStreamResponseAsync();

            Assert.NotNull(response);
            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public void GetShards()
        {
            var shards = _manager.Utilities.GetActiveShards();

            Assert.Equal(shards.Count(), 1);
        }

        [Fact]
        public void SplitShard()
        {
            var shards = _manager.Utilities.GetActiveShards();

            var response = _manager.Utilities.SplitShard(shards.FirstOrDefault());

            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);

            Thread.Sleep(TimeSpan.FromSeconds(10)); //this will fail because the update process may take longer

            shards = _manager.Utilities.GetActiveShards();

            Assert.Equal(shards.Count, 2);
        }

        [Fact]
        public void MergeShards()
        {
            var shards = _manager.Utilities.GetActiveShards();

            var response = _manager.Utilities.MergeShards(shards.ElementAt(0).ShardId, shards.ElementAt(1).ShardId);

            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);

            Thread.Sleep(TimeSpan.FromSeconds(10)); //this will fail because the update process may take longer

            shards = _manager.Utilities.GetActiveShards();

            Assert.Equal(shards.Count, 1);
        }
    }
}
