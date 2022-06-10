using System;
using System.Collections.Generic;
using System.Linq;
using AssetSystem.Component;
using GraphConnectEngine.Variable;
using MyWorldHub.Unity;
using UnityEngine;
using VisualScripting.Scripts.Graphs;
using VisualScripting.Scripts.Helper;

namespace AssetSystem.Unity
{
    public class ManagedObject : IManagedObject
    {

        private const string PrefixObject = "object";

        public string UniqueId { get; private set; }

        public GameObject gameObject { get; }
        
        public string Name { get => gameObject.name; set => HiddenVariableManager.Update(PrefixObject,"name",value); }

        public Vector3 LocalPosition
        {
            get => gameObject.transform.localPosition;
            set
            {
                //TODO 通信
                gameObject.transform.localPosition = value;
            }
        }

        public Quaternion LocalRotation
        {
            get => gameObject.transform.localRotation;
            set
            {
                //TODO 通信
                gameObject.transform.localRotation = value;
            }
        }
        
        public Vector3 Scale 
        {
            get => gameObject.transform.localScale;
            set
            {
                //TODO 通信
                gameObject.transform.localScale = value;
            }
        }

        public IWorldContext WorldContext { get; private set; }
        
        public IManagedObject Parent { get; private set; }
        
        public ObjectContext ObjectContext { get; private set; }

        public IAsyncVariableHolder GlobalVariable { get; private set; }

        public IVariableHolder LocalVariable { get; private set; }

        public IHiddenVariableManager HiddenVariableManager { get; private set; }

        /// <summary>
        /// TODO これも変数でやってしまって、このプロパティをなくして便利関数セットを作りたい
        /// </summary>
        public bool GraphVisibility { get=> _graphSystem.Visibility; set => _graphSystem.Visibility = value; }

        /// <summary>
        /// コンポーネントのイベントはComponentSystemから流す
        /// </summary>
        public event EventHandler<(string ComponentId, ComponentBase Component)> OnComponentCreated;
        public event EventHandler<string> OnComponentRemoved;

        /// <summary>
        /// グラフの管理用
        /// </summary>
        private GraphSystem _graphSystem;

        /// <summary>
        /// コンポーネントの管理用
        /// </summary>
        private ComponentSystem _componentSystem;

        /// <summary>
        /// 子
        /// </summary>
        private readonly Dictionary<string, IManagedObject> _children = new ();

        public ManagedObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public void Initialize(string uniqueId, ObjectContext objectContext, IWorldContext worldContext)
        {
            WorldContext = worldContext;
            UniqueId = uniqueId;
            ObjectContext = objectContext;
        }

        public void InitializeVariable(IAsyncVariableHolder globalVariable, IVariableHolder localVariable)
        {
            // VariableManager
            LocalVariable = localVariable;
            GlobalVariable = globalVariable;
            HiddenVariableManager = new HiddenVariableManager(GlobalVariable);

            // Variable系
            _graphSystem = new GraphSystem(this);
            _componentSystem = new ComponentSystem(this);
            HiddenVariableManager.AddListener(PrefixObject,OnVariableUpdated);
            ReloadVariableDependencies();

            // ComponentSystemのイベントを流す
            _componentSystem.OnComponentCreated += (_,args) =>
            {
                OnComponentCreated?.Invoke(this,args);
            };
            _componentSystem.OnComponentRemoved += (_,args) =>
            {
                OnComponentRemoved?.Invoke(this,args);
            };
        }

        public void SetParent(IManagedObject parent)
        {
            // 登録解除
            if (Parent != null)
            {
                Parent.UnregisterChild(this);
            }
            
            Parent = parent;
            gameObject.transform.parent = null;
            
            if (parent != null)
            {
                gameObject.transform.parent = parent.gameObject.transform;
                // 登録する
                parent.RegisterChild(this);
            }
        }

        public void SetParent(IManagedObject parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            SetParent(parent);

            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = rotation;
            gameObject.transform.localScale = scale;
        }

        /// <summary>
        /// 変数によって生成されるオブジェクトをすべて再生成する
        /// </summary>
        public async void ReloadVariableDependencies()
        {
            // 名前を読み込む
            var result = await HiddenVariableManager.Get<string>(PrefixObject, "name");
            if (result.IsSucceeded)
            {
                gameObject.name = result.Value;
            }
            
            // タグを読み込む
            result = await HiddenVariableManager.Get<string>(PrefixObject, "tag");
            if (result.IsSucceeded)
            {
                gameObject.tag = result.Value;
            }
            
            await _componentSystem.Reload();
            await _graphSystem.Reload();
        }

        public void RegisterChild(IManagedObject child)
        {
            _children[child.UniqueId] = child;
        }

        public void UnregisterChild(IManagedObject child)
        {
            _children.Remove(child.UniqueId);
        }

        public ISet<IManagedObject> GetChildren()
        {
            return _children.Values.ToHashSet();
        }

        public Dictionary<string, ComponentBase> GetAllComponents()
        {
            return _componentSystem.GetAllComponents();
        }

        public async void TryAddComponent(string id, string type)
        {
            if (await HiddenVariableManager.TryCreate(ComponentSystem.DefinitionPrefix, id, type))
            {
                await ComponentInitializer.CreateVariable(id, type, this);
            }
        }

        public async void TryRemoveComponent(string id)
        {
            await HiddenVariableManager.Remove(ComponentSystem.DefinitionPrefix, id);
        }

        public async void TryAddGraph(string id, string type,Vector3 position,Quaternion rotation)
        {
            await HiddenVariableManager.TryCreate(GraphSystem.PositionPrefix, id, position.Serialize());
            await HiddenVariableManager.TryCreate(GraphSystem.RotationPrefix, id, rotation.Serialize());
            await GraphInitializer.CreateVariable(id, type, this);
            await HiddenVariableManager.TryCreate(GraphSystem.DefinitionPrefix, id, type);
        }

        public ISet<string> GetResourceDependencies()
        {
            return _componentSystem.GetResourceDependencies();
        }

        public void OnFixedUpdate()
        {
            _graphSystem?.OnFixedUpdate();
        }

        private void OnVariableUpdated(VariableUpdatedEventArgs args)
        {
            switch (args.Name)
            {
                case "name":
                    gameObject.name = args.Value.ToString();
                    break;
                case "tag":
                    gameObject.tag = args.Value.ToString();
                    break;
            }
        }

        /// <summary>
        /// Disposeされたかどうか
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 子がDisposeされてから呼ばれる
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                SetParent(null);
                
                _isDisposed = true;
                _graphSystem?.Dispose();
                _componentSystem?.Dispose();
                GlobalVariable?.Dispose();
                LocalVariable?.Dispose();
                HiddenVariableManager?.Dispose();

                WorldContext = null;
                GlobalVariable = null;
                LocalVariable = null;
                HiddenVariableManager = null;
                _graphSystem = null;
                _componentSystem = null;
                
                GameObject.Destroy(gameObject);
            }
        }
    }
}