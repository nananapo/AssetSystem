using System.Collections.Generic;
using System.Threading;
using Antlr4.Runtime.Sharpen;
using Cysharp.Threading.Tasks;
using MyWorldHub;
using TriLibCore;
using UnityEngine;

namespace AssetSystem.Repository
{
    public class TriLibModelRepository : IModelRepository
    {
        private readonly Transform _hiddenRootTransform;

        private readonly IDictionary<string, GameObject> _loadedObject;

        private readonly IDictionary<string, IAssetList> _assetLists;

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public TriLibModelRepository()
        {
            _hiddenRootTransform = new GameObject("TriLibRepository").transform;

            _loadedObject = new ConcurrentDictionary<string, GameObject>();
            _assetLists = new ConcurrentDictionary<string, IAssetList>();

            InitPrimitives();
        }

        private void InitPrimitives()
        {
            _loadedObject["PrimitiveType:Cube"] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _loadedObject["PrimitiveType:Sphere"] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _loadedObject["PrimitiveType:Capsule"] = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _loadedObject["PrimitiveType:Cylinder"] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _loadedObject["PrimitiveType:Plane"] = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _loadedObject["PrimitiveType:Quad"] = GameObject.CreatePrimitive(PrimitiveType.Quad);

            foreach (var pair in _loadedObject)
            {
                pair.Value.SetActive(false);
                pair.Value.transform.parent = _hiddenRootTransform;
                
                _assetLists[pair.Key] = new SimpleAssetList(new Object[]
                {
                    pair.Value.GetComponent<MeshFilter>().mesh,
                    pair.Value.GetComponent<MeshRenderer>().material
                });
            }
        }

        private bool TryParseId(string id,out string referenceId,out int index)
        {
            var sp = id.Split("?");
            
            if (sp.Length == 2)
            {
                if (int.TryParse(sp[1], out index))
                {
                    referenceId = sp[0];
                    return true;
                }
            }

            referenceId = null;
            index = -1;
            
            return false;
        }

        public async UniTask<T> Get<T>(string id) where T : Object
        {
            if (!TryParseId(id, out var referenceId, out var index)) return null;

            if (_loadedObject.ContainsKey(referenceId)) return _assetLists[referenceId].Get<T>(index);
            
            if (!await Download(referenceId))
            {
                //TODO Error
                return null;
            }

            return _assetLists[referenceId].Get<T>(index);
        }

        //TODO
        public UniTask Unload(string id)
        {
            if (!TryParseId(id, out var referenceId, out _)) return UniTask.CompletedTask;
            
            if (!_loadedObject.ContainsKey(referenceId)) return UniTask.CompletedTask;
            
            Object.Destroy(_loadedObject[referenceId]);
                
            _assetLists[referenceId].Dispose();
            _assetLists.Remove(referenceId);

            return UniTask.CompletedTask;
        }

        private async UniTask<bool> Download(string referenceId)
        {
            await _semaphoreSlim.WaitAsync();

            if (_loadedObject.ContainsKey(referenceId))
                return true;

            var worldDataRepository = ServiceLocator.Resolve<IBundleDataRepository>();

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var webRequest = AssetDownloader.CreateWebRequest(await worldDataRepository.GetModelDataUrl(referenceId));

            GameObject objectParent = null;

            var done = false;
            var error = false;

            AssetDownloader.LoadModelFromUri(webRequest, _ => { }, a =>
            {
                objectParent = a.RootGameObject;
                done = true;
            }, (_, progress) => { }, errorContext =>
            {
                error = true;
                Debug.Log(errorContext.GetInnerException().Message);
            }, _hiddenRootTransform.gameObject, assetLoaderOptions);

            //待つ
            await UniTask.WaitUntil(() => done || error);

            if (error)
            {
                //TODO Error
                return false;
            }

            //移動
            var newParent = new GameObject(referenceId)
            {
                transform =
                {
                    parent = _hiddenRootTransform
                }
            };
            newParent.gameObject.SetActive(false);

            for (var i = objectParent.transform.childCount - 1; i > -1; i--)
            {
                var child = objectParent.transform.GetChild(i);
                child.parent = newParent.transform;
            }

            _loadedObject[referenceId] = newParent;
            _assetLists[referenceId] = new AssetUnloaderWrapper(objectParent.GetComponent<AssetUnloader>());

            _semaphoreSlim.Release();

            return true;
        }
    }
}