KinesisNet
==========

A simple kinesis consumer/producer library for .NET

## Overview 
This is a basic library for handling Amazon Kinesis partly inspired by the excellent [ReactoKinesisX](https://github.com/theburningmonk/ReactoKinesiX).

While it's not as (yet) fully featured as the aforementioned library it is enough to play with. Pull requests are welcome.

The idea behind this project was to make the consumption of the AWS Kinesis pipeline/stream easier on .NET. This library allows you to subscribe to a stream and automatically retrieve records as they appear in the stream. The library uses DynamoDB to store checkpoints so that you can interrupt the processing of messages (at any point) and then seamlessly resume from where you left off. 

(If you are going to use this library please set up an IAM user and grant him access to Kinesis and Dynamo. The library will automatically create the right tables and persist checkpoints. However, you can turn off Dynamo if you do not wish to use it).

The library also allows you to put records onto the stream (via the Producer component) and make changes to the stream (via the Utilities component). The major advantage of using a library such as this is that it will handle complex events such as resharding automatically. The library looks at the stream at regular intervals and automatically adjusts to the conditions at hand.

There are two major dependencies to this library - AWS SDK and Serilog. If folks prefer to use Log4Net please let me know and/or issue a pull request. 

Please feel free to drive-test this library and raise issues/pull requests against the code. There is a sample project included that will give you a sense for how to use KinesisNet.

## Basics
### Installation

Use Nuget to install KinesisNet into your project **Install-Package KinesisNet** or via the Package Manager.

### How to Run

Create an instance of KManager with your credentials and then use the Consumer to retrieve records or Producer to put records.

```csharp

    public class RecordProcessor : IRecordProcessor
    {
        public void Process(string shardId, string sequenceNumber, DateTime lastUpdateUtc, IList<Record> records, Action<string, string, DateTime> saveCheckpoint)
        {
            foreach (var record in records)
            {
                var msg = Encoding.UTF8.GetString(record.Data.ToArray());
                Console.WriteLine("ShardId: {0}, Data: {1}", shardId, msg);
            }
            
            saveCheckpoint(shardId, sequenceNumber, lastUpdateUtc);
        }
    }


    static void Main(string[] args)
   	{
        var awsKey = ConfigurationManager.AppSettings["AWSKey"];
        var awsSecret = ConfigurationManager.AppSettings["AWSSecret"];
        var streamName = ConfigurationManager.AppSettings["AWSStreamName"];
        var regionEndpoint = RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["AWSRegionEndpoint"]);
        var workerId = "MyComputer";

        var kManager = new KManager(awsKey, awsSecret, streamName, regionEndpoint, workerId);

        var result = kManager
                    .Consumer
                    .Start(new RecordProcessor());

        if (result.Success)
        {
        	Console.WriteLine("We are now processing results");
        }

        kManager.Producer.PutRecord("Create Record " + DateTime.UtcNow);

        kManager.Consumer.Stop(); //stop processing
    }

```

### Configuration

You may configure KinesisNet or get information about the Stream via the utilities component of KManager.

### Examples

To come....
