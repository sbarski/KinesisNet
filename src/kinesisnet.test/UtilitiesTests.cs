using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Microsoft.Extensions.Configuration;
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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _awsKey = configuration["AWSKey"];
            _awsSecret = configuration["AWSSecret"];
            _regionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWSRegionEndpoint"]);

            _manager = new KManager(_awsKey, _awsSecret, _streamName, _regionEndpoint);
        }

        [Fact]
        public async void ListStreams()
        {
            var response = await _manager.Utilities.ListStreamsAsync();
            Assert.NotNull(response);
            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.Equal(response.StreamNames.Count, 1);

            response = await _manager.Utilities.ListStreamsAsync("Test");
            Assert.NotNull(response);
            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.Equal(response.StreamNames.Count, 1);
        }

        [Fact]
        public async Task ListStreamsAsync()
        {
            var response = await _manager.Utilities.ListStreamsAsync();

            Assert.NotNull(response);
            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.Equal(response.StreamNames.Count, 1);

            response = await _manager.Utilities.ListStreamsAsync("Test");
            Assert.NotNull(response);
            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.Equal(response.StreamNames.Count, 1);
        }

        [Fact]
        public async void GetStreamResponse()
        {
            var response = await _manager.Utilities.GetStreamResponse();

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
        public async void SplitShard()
        {
            var shards = _manager.Utilities.GetActiveShards();

            var response = await _manager.Utilities.SplitShard(shards.FirstOrDefault());

            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);

            Thread.Sleep(TimeSpan.FromSeconds(10)); //this will fail because the update process may take longer

            shards = _manager.Utilities.GetActiveShards();

            Assert.Equal(shards.Count, 2);
        }

        [Fact]
        public async void MergeShards()
        {
            var shards = _manager.Utilities.GetActiveShards();

            var response = await _manager.Utilities.MergeShards(shards.ElementAt(0).ShardId, shards.ElementAt(1).ShardId);

            Assert.Equal(response.HttpStatusCode, HttpStatusCode.OK);

            Thread.Sleep(TimeSpan.FromSeconds(10)); //this will fail because the update process may take longer

            shards = _manager.Utilities.GetActiveShards();

            Assert.Equal(shards.Count, 1);
        }
    }
}
