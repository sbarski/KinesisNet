KinesisNet
==========

A simple kinesis consumer/producer library for .NET

## Overview 
This is a basic library for handling Amazon Kinesis partly inspired by the excellent [ReactoKinesisX](https://github.com/theburningmonk/ReactoKinesiX).

While it's not as (yet) fully featured as the aforementioned library it is enough to play with.

## Basics
### Installation

Use Nuget to install KinesisNet into your project **Install-Package KinesisNet** or via the Package Manager.

### How to Run

Create an instance of KManager with your credentials and then use the Consumer to retrieve records or Producer to put records.

```csharp

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