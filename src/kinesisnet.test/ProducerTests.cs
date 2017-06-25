using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace KinesisNet.Test
{
    public class ProducerTests
    {
        private readonly string _awsKey;
        private readonly string _awsSecret;
        private readonly string _streamName;
        private readonly RegionEndpoint _regionEndpoint;

        private readonly KManager _manager;

        public ProducerTests()
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
        public void CannotPutWithoutAStreamName()
        {
            var ex = Assert.Throws<AggregateException>(() => _manager.Producer.PutRecordAsync("TestRecord").Result);

            Assert.Equal("You must set a stream name before you can put a record", ex.InnerExceptions.First().Message);
        }

        [Fact]
        public async Task CannotPutWithoutAStreamNameAsync()
        {
            //wait for throwsAsync in xunit 2
            //var ex = Assert.Throws<Exception>(async () => await _manager.Producer.PutRecordAsync("TestRecord")); 

            //Assert.Equal(ex.Message, "You must set a stream name before you can put a record");
        }
    }
}
