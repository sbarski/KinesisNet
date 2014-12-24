using KinesisNet.Model;

namespace KinesisNet.Interface
{
    public interface IConsumer
    {
        IConsumer EnableSaveToDynamo(bool saveProgress = true);

        Result Start(IRecordProcessor recordProcessor);
        void Stop();
    }
}
