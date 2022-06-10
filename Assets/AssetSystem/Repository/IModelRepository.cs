using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AssetSystem.Repository
{
    public interface IModelRepository
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UniTask<T> Get<T>(string id) where T : Object;

        /// <summary>
        /// 全てのオブジェクトを削除してからUnload
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UniTask Unload(string id);
    }
}