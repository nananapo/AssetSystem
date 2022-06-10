using System.Collections.Generic;
using UnityEngine;

namespace AssetSystem.Repository
{
    public class SimpleAssetList : IAssetList
    {
        private readonly IList<Object> _list;
        private readonly bool _unload;

        public SimpleAssetList(IList<Object> list,bool unload = false)
        {
            _list = list;
            _unload = unload;
        }

        public void Dispose()
        {
            if (!_unload) return;
            for (var i = _list.Count - 1; i > -1; i--)
                Object.Destroy(_list[i]);
        }

        public T Get<T>(int index) where T : Object
        {
            return _list.Count > index ? _list[index] as T : null;
        }
    }
}