using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Amazon;
using Amazon.Kinesis.Model;
using KinesisNet.Interface;

namespace KinesisNet.Example
{
    public class RecordProcessor : IRecordProcessor
    {
        public void Process(string shardId, IList<Record> records)
        {
            foreach (var record in records)
            {
                var msg = Encoding.UTF8.GetString(record.Data.ToArray());
                Console.WriteLine("ShardId: {0}, Data: {1}", shardId, msg);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var awsKey = ConfigurationManager.AppSettings["AWSKey"];
            var awsSecret = ConfigurationManager.AppSettings["AWSSecret"];
            var regionEndpoint = RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["AWSRegionEndpoint"]);

            IKManager kManager = new KManager(awsKey, awsSecret, regionEndpoint);

            kManager.Utilities.SetStreamName("TestStream");

            var result = kManager
                        .Consumer
                        .Start(new RecordProcessor());

            if (result.Success)
            {
                do
                {
                    var m = Console.ReadKey();

                    if (m.Key == ConsoleKey.Enter)
                    {
                        kManager.Producer.PutRecord("abc " + DateTime.UtcNow);
                    }

                    if (m.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                } while (true);
            }

            kManager.Consumer.Stop();
        }
    }
}
