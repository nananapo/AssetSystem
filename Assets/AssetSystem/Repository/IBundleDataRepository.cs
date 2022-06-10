using System.Collections.Generic;
using AssetSystem.Reference;
using Cysharp.Threading.Tasks;

namespace AssetSystem.Repository
{
    public interface IBundleDataRepository
    {

        public UniTask<List<(string ObjectId,IBundleDescription Description)>> GetObjectList(long createdTime,bool start,bool at,int limit);

        public UniTask<List<(string worldId,IBundleDescription Description)>> GetWorldList(long createdTime,bool start,bool at,int limit);

        public UniTask<(bool result,IBundleDescription data)> GetObjectDescription(string objectId);

        public UniTask<(bool result,IBundleDescription data)> GetWorldDescription(string worldId);

        public UniTask<(bool result,IMasterBundle data)> GetMasterBundle(string bundleId);

        public UniTask<string> GetModelDataUrl(string id);
        
        public UniTask<bool> SaveObjectDescription(string objectId, IBundleDescription bundle);
        
        public UniTask<bool> SaveWorldDescription(string worldId, IBundleDescription bundle);
        
        public UniTask<bool> SaveBundle(string id, IMasterBundle masterBundle);
    }
}