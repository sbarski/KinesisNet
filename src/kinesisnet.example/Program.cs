using System;
using System.Collections.Generic;
using System.Text;
using Amazon;
using Amazon.Kinesis.Model;
using KinesisNet.Interface;
using Microsoft.Extensions.Configuration;

namespace KinesisNet.Example
{
    public class RecordProcessor : IRecordProcessor
    {
        public void Process(string shardId, string sequenceNumber, DateTime lastUpdateUtc, IList<Record> records, Action<string, string, DateTime> saveCheckpoint)
        {
            foreach (var record in records)
            {
                var msg = Encoding.UTF8.GetString(record.Data.ToArray());
                Console.WriteLine("ShardId: {0}, Data: {1}", shardId, msg);
            }

            //save the checkpoint to dynamodb to say that we've successfully processed our records 
            saveCheckpoint(arg1: shardId, arg2: sequenceNumber, arg3: lastUpdateUtc);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var awsKey = configuration["AWSKey"];
            var awsSecret = configuration["AWSSecret"];
            var regionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWSRegionEndpoint"]);

            IKManager kManager = new KManager(awsKey, awsSecret, regionEndpoint);

            kManager.Utilities.SetStreamName("Notifications");

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
                        kManager.Producer.PutRecordAsync("abc " + DateTime.UtcNow);
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
