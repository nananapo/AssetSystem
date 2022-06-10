using System;
using System.Collections.Generic;
using System.Linq;
using AssetSystem.Component;
using AssetSystem.Reference;
using GraphConnectEngine.Variable;
using MyWorldHub.Unity;
using UnityEngine;
using VisualScripting.Scripts.Helper;

namespace AssetSystem.Unity
{
    public interface IManagedObject : IUniqueObject,IDisposable
    {

        /// <summary>
        /// ゲームオブジェクト
        /// </summary>
        public GameObject gameObject { get; }
        
        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// ローカル座標
        /// </summary>
        public Vector3 LocalPosition { get; set; }
        
        /// <summary>
        /// ローカル回転
        /// </summary>
        public Quaternion LocalRotation { get; set; }

        /// <summary>
        /// ローカル拡大
        /// </summary>
        public Vector3 Scale { get; set; }
        
        /// <summary>
        /// ワールド
        /// </summary>
        public IWorldContext WorldContext { get; }

        /// <summary>
        /// 親オブジェクト
        /// ルートならnull
        /// </summary>
        public IManagedObject Parent { get; }

        /// <summary>
        /// このオブジェクトのContext
        /// </summary>
        public ObjectContext ObjectContext { get; }
        
        /// <summary>
        /// グローバルな変数(共有)
        /// </summary>
        public IAsyncVariableHolder GlobalVariable { get; }

        /// <summary>
        /// ローカルな変数
        /// </summary>
        public IVariableHolder LocalVariable { get; }

        /// <summary>
        /// 隠し変数用
        /// </summary>
        public IHiddenVariableManager HiddenVariableManager { get;}
        
        /// <summary>
        /// グラフの可視設定
        /// </summary>
        public bool GraphVisibility { get; set; }

        public event EventHandler<(string ComponentId, ComponentBase Component)> OnComponentCreated;

        public event EventHandler<string> OnComponentRemoved;

        public void Initialize(string uniqueId,ObjectContext objectContext,IWorldContext worldContext);

        public void InitializeVariable(IAsyncVariableHolder globalVariable, IVariableHolder localVariable);

        /// <summary>
        /// 親を設定する
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(IManagedObject parent);

        /// <summary>
        /// 親を設定する
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public void SetParent(IManagedObject parent, Vector3 position, Quaternion rotation, Vector3 scale);

        /// <summary>
        /// 変数で管理されるオブジェクト系を再読み込みする
        /// </summary>
        public void ReloadVariableDependencies();

        /// <summary>
        /// 子を登録するだけ
        /// SetParentが自動で呼ぶように実装するので呼ぶ必要がない
        /// </summary>
        /// <param name="child"></param>
        public void RegisterChild(IManagedObject child);

        /// <summary>
        /// 子を解除するだけ
        /// SetParentが自動で呼ぶように実装するので呼ぶ必要がない
        /// </summary>
        /// <param name="child"></param>
        public void UnregisterChild(IManagedObject child);

        /// <summary>
        /// 登録されている子を取得する
        /// </summary>
        /// <returns></returns>
        public ISet<IManagedObject> GetChildren();

        /// <summary>
        /// すべてのコンポーネントを取得する
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ComponentBase> GetAllComponents();

        /// <summary>
        /// コンポーネントをidで取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ComponentBase GetComponentById(string id)
        {
            var components = GetAllComponents();
            return components.ContainsKey(id) ? components[id] : null;
        }

        /// <summary>
        /// コンポーネントをidで取得する
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponentById<T>(string id) where T : ComponentBase
        {
            return GetComponentById(id) as T;
        }

        /// <summary>
        /// コンポーネントを型で取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : ComponentBase
        {
            var components = GetComponents<T>();
            return components.Length > 0 ? components[0] : null;
        }

        /// <summary>
        /// 指定した型のコンポーネントをすべて取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetComponents<T>() where T : ComponentBase
        {
            return GetAllComponents()
                .Where(kv => kv.Value is T)
                .Select(kv => kv.Value as T)
                .ToArray();
        }

        /// <summary>
        /// コンポーネントを追加(しようと)する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public void TryAddComponent(string id, string type);

        /// <summary>
        /// コンポーネントを削除しようとする
        /// </summary>
        /// <param name="id"></param>
        public void TryRemoveComponent(string id);

        /// <summary>
        /// グラフを追加(しようと)する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void TryAddGraph(string id, string type,Vector3 position,Quaternion rotation);

        /// <summary>
        /// 依存しているリソースのidを取得する
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetResourceDependencies();

        /// <summary>
        /// 更新時に呼ぶ
        /// </summary>
        public void OnFixedUpdate();
    }
}