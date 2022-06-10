using TriLibCore;
using UnityEngine;

namespace AssetSystem.Repository
{
    public class AssetUnloaderWrapper : IAssetList
    {
        private readonly AssetUnloader _unloader;
        
        public AssetUnloaderWrapper(AssetUnloader unloader)
        {
            _unloader = unloader;
        }
        
        public void Dispose()
        {
            Object.Destroy(_unloader.gameObject);
        }

        public T Get<T>(int index) where T : Object
        {
            return _unloader.Allocations[index] as T;
        }
    }
}