using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
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
            _awsKey = ConfigurationManager.AppSettings["AWSKey"];
            _awsSecret = ConfigurationManager.AppSettings["AWSSecret"];
            _regionEndpoint = RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["AWSRegionEndpoint"]);

            _manager = new KManager(_awsKey, _awsSecret, _regionEndpoint);
        }

        [Fact]
        public void CannotPutWithoutAStreamName()
        {
            var ex = Assert.Throws<Exception>(() => _manager.Producer.PutRecord("TestRecord"));

            Assert.Equal(ex.Message, "You must set a stream name before you can put a record");
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
