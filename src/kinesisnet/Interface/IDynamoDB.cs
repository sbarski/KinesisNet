using System.Collections.Generic;
using System.Threading.Tasks;
using KinesisNet.Model;

namespace KinesisNet.Interface
{
    internal interface IDynamoDB
    {
        void Init();
        Task SaveToDatabase(KShard kShard);
        Task<List<KShard>> GetShards(IList<string> shardIds);
    }
}
