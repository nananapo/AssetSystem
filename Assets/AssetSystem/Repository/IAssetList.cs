using System;

namespace AssetSystem.Repository
{
    public interface IAssetList : IDisposable
    {
        public T Get<T>(int index) where T : UnityEngine.Object;
    }
}