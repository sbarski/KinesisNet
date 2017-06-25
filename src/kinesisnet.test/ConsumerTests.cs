using System;
using System.Linq;
using Amazon;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace KinesisNet.Test
{
    public class ConsumerTests
    {
        private readonly string _awsKey;
        private readonly string _awsSecret;
        private readonly string _streamName;
        private readonly RegionEndpoint _regionEndpoint;

        private readonly KManager _manager;

        public ConsumerTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _awsKey = configuration["AWSKey"];
            _awsSecret = configuration["AWSSecret"];
            _regionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWSRegionEndpoint"]);

            _manager = new KManager(_awsKey, _awsSecret, _regionEndpoint);
        }

        [Fact]
        public void CanCreateConsumer()
        {
            Assert.NotNull(_manager);
            Assert.NotNull(_manager.Consumer);
        }

        [Fact]
        public void CannotRunConsumerWithoutStreamName()
        {
            var result = _manager.Consumer.Start(new RecordProcessor());

            Assert.Equal(result.Success, false);
            Assert.Equal(result.Message, "Please set a stream name.");
        }

        [Fact]
        public void ConsumerCanRunWithoutAnyShardsToProcess()
        {
            var recordProcessorMock = new Mock<RecordProcessor>();

            var result = _manager.Consumer.Start(recordProcessorMock.Object);

            Assert.Equal(result.Success, true);
            Assert.Equal(result.Message, "Processing System");
        }

        [Fact]
        public void CannotGetStreamResponseWithoutStreamName()
        {
            var ex = Assert.Throws<AggregateException>(() => _manager.Utilities.GetStreamResponse().Result);

            Assert.Equal("Please specify a stream name to get the stream response.", ex.InnerExceptions.First().Message);
        }
    }
}
