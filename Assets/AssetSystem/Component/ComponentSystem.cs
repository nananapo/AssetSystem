using System;
using System.Collections.Generic;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using GraphConnectEngine.Variable;
using UnityEngine;

namespace AssetSystem.Component
{
    /// <summary>
    /// コンポーネントを管理するクラス
    ///
    /// コンポーネントを新しく削除したり作成したりはしない
    /// </summary>
    public class ComponentSystem : IDisposable
    {

        public const string DefinitionPrefix = "componentdef";

        private readonly IManagedObject _managedObject;

        private readonly Dictionary<string, ComponentBase> _components = new ();

        /// <summary>
        /// コンポーネントが作成されたときに呼ばれるリスナ
        ///
        /// コンポーネントが更新されたときのリスナが欲しい場合は
        /// ① IHiddenVariableにコンポーネントのidでリスナを登録
        /// ② 毎フレーム更新
        /// のどちらかを行う
        /// </summary>
        public event EventHandler<(string ComponentId, ComponentBase Component)> OnComponentCreated;

        /// <summary>
        /// コンポーネントが削除されたときに呼ばれるリスナ
        /// </summary>
        public event EventHandler<string> OnComponentRemoved;

        /// <summary>
        /// コンストラクタでは変数を読み込まないので、Reloadを手動で行う
        /// </summary>
        /// <param name="managedObject"></param>
        public ComponentSystem(IManagedObject managedObject)
        {
            _managedObject = managedObject;
            
            // リスナを追加する
            var hid = _managedObject.HiddenVariableManager;
            hid.AddListener(DefinitionPrefix,OnDefinitionUpdated);
            hid.AddListener(DefinitionPrefix,OnDefinitionRemoved);
        }
        
        /// <summary>
        /// コンポーネントを変数から再読み込みする
        /// </summary>
        public async UniTask Reload()
        {
            // TODO サーバー用に、定期的に使用されていない隠し変数を削除するとか
            // コンポーネントをすべて削除する
            foreach (var (id,component) in _components)
            {
                component.Dispose();
                OnComponentRemoved?.Invoke(this,id);
            }
            _components.Clear();

            var definitions = await _managedObject.HiddenVariableManager.GetAllVariables(DefinitionPrefix);
            foreach (var id in definitions)
            {
                var res = await _managedObject.HiddenVariableManager.Get<string>(DefinitionPrefix,id);
                if (!res.IsSucceeded)
                {
                    Debug.LogWarning($"Failed to get component type : {id}");
                    continue;
                }

                await CreateNewComponent(id, res.Value);
            }
        }

        /// <summary>
        /// 内部用のComponent作成関数
        /// 新しくComponentを作る処理はIManagedObjectにお任せ(変数作成の部分の話)
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        private async UniTask<bool> CreateNewComponent(string componentId, string componentType)
        {
            var component = await ComponentInitializer.Instantiate(componentId, componentType, _managedObject);
            if (component == null)
            {
                Debug.LogWarning($"Failed to Initialize Component : {componentId} , {componentType}");
                return false;
            }

            _components[componentId] = component;
            OnComponentCreated?.Invoke(this,(componentId,component));
            return true;
        }
        
        private async void OnDefinitionUpdated(VariableUpdatedEventArgs args)
        {
            if (args.Type != typeof(string) ||
                _components.ContainsKey(args.Name)) return;

            await CreateNewComponent(args.Name, args.Value.ToString());
        }
        
        private void OnDefinitionRemoved(VariableRemovedEventArgs args)
        {
            var id = args.Name;
            
            if (_components.ContainsKey(id))
            {
                _components[id].Dispose();
                _components.Remove(id);
                OnComponentRemoved?.Invoke(this,id);
            }
        }

        /// <summary>
        /// すべてのコンポーネントを取得する
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,ComponentBase> GetAllComponents()
        {
            return _components.ToDictionary();
        }

        /// <summary>
        /// コンポーネントが依存しているリソースのIDをすべて返す
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetResourceDependencies()
        {
            var list = new HashSet<string>();
            foreach (var (_,component) in _components)
            {
                foreach (var dependency in component.GetResourceDependencies())
                {
                    list.Add(dependency);
                }
            }
            return list;
        }
        
        public void Dispose()
        {
            var hid = _managedObject.HiddenVariableManager;
            hid.RemoveListener(DefinitionPrefix,OnDefinitionUpdated);
            hid.RemoveListener(DefinitionPrefix,OnDefinitionRemoved);
        }
    }
}