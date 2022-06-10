using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AssetSystem.Reference
{
    public interface IResourceReference
    {
        public UniTask<Object> Generate();
    }
}