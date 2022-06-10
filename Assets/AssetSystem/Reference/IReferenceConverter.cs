using System.Threading.Tasks;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;

namespace AssetSystem.Reference
{
    public interface IReferenceConverter
    {

        public UniTask<(bool result,IBundleDescription data)> DecodeWorldDescription(string name,string bundleId,string authorId,long createdTime,long updatedTime);
        
        public UniTask<(bool result,IMasterBundle data)> DecodeMasterBundle(string data);

        public (bool result, IResourceReference data) ConvertPropertyToResource(
            IPropertyReference propertyReference);

        public Task<IMasterBundle> CreateMasterBundleAsync(IManagedObject rootObject);

        public Task<IBundleDescription> CreateBundleDescription(string name,string bundleId,string authorId,long createdTime,long updatedTime);
    }
}