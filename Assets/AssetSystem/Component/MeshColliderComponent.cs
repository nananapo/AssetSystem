using System.Collections.Generic;
using AssetSystem.Repository;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using GraphConnectEngine.Variable;
using MyWorldHub;
using UniRx;
using UnityEngine;
using VisualScripting.Scripts.Window;

namespace AssetSystem.Component
{
    public class MeshColliderComponent : ComponentBase
    {
        
        public const string DefinitionName = "MeshCollider";
        
        public override string ComponentName => DefinitionName;

        private readonly MeshCollider _component;
        
        private string _physicMaterialId;
        
        private string _meshId;

        public bool Convex
        {
            get => _component.convex;
            set => SetProperty(nameof(Convex), value);
        }
        
        public bool IsTrigger
        {
            get => _component.isTrigger;
            set => SetProperty(nameof(IsTrigger), value);
        }
        
        public string PhysicMaterialId
        {
            get => _physicMaterialId;
            set => SetProperty(nameof(PhysicMaterialId), value);
        }
        
        public string MeshId
        {
            get => _meshId;
            set => SetProperty(nameof(MeshId), value);
        }


        public MeshColliderComponent(string id,IManagedObject managedObject) : base(id,managedObject)
        {
            _component = ParentObject.gameObject.AddComponent<MeshCollider>();

            InitProperty<bool>(nameof(Convex),v=>_component.convex = v);
            InitProperty<bool>(nameof(IsTrigger),v=>_component.isTrigger = v);
            InitProperty<string>(nameof(PhysicMaterialId), v =>
            {
                _physicMaterialId = v;
                ApplyPhysicMaterial();
            });
            InitProperty<string>(nameof(MeshId), v =>
            {
                _meshId = v;
                ApplyMesh();
            });
        }

        public override IList<string> GetResourceDependencies()
        {
            return new []{PhysicMaterialId};
        }

        protected override void OnPropertyUpdated(VariableUpdatedEventArgs args)
        {
            switch (args.Name)
            {
                case nameof(Convex):
                    if (args.Type == typeof(bool))
                    {
                        _component.convex = (bool)args.Value;
                    }
                    break;
                case nameof(IsTrigger):
                    if (args.Type == typeof(bool))
                    {
                        _component.isTrigger = (bool)args.Value;
                    }
                    break;
                case nameof(PhysicMaterialId):
                    if (args.Type == typeof(string))
                    {
                        _physicMaterialId = args.Value.ToString();
                        ApplyPhysicMaterial();
                    }
                    break;
                case nameof(MeshId):
                    if (args.Type == typeof(string))
                    {
                        _meshId = args.Value.ToString();
                        ApplyMesh();
                    }
                    break;
            }
        }

        public override void InstantiateDetailWindow(Transform parent)
        {

            var convex = GameObject.Instantiate(WindowDependency.ToggleItemPrefab,parent);
            convex.ItemName = nameof(Convex);
            convex.SetIsOnWithoutNotify(Convex);
            convex.OnValueEdit += value => Convex = value;
            
            Observable.EveryUpdate()
                .Select(_=>Convex)
                .DistinctUntilChanged()
                .Subscribe(convex.SetIsOnWithoutNotify)
                .AddTo(convex);
            

            var toggle = GameObject.Instantiate(WindowDependency.ToggleItemPrefab,parent);
            toggle.ItemName = nameof(IsTrigger);
            toggle.SetIsOnWithoutNotify(IsTrigger);
            toggle.OnValueEdit += value =>IsTrigger = value;
            
            Observable.EveryUpdate()
                .Select(_=>IsTrigger)
                .DistinctUntilChanged()
                .Subscribe(toggle.SetIsOnWithoutNotify)
                .AddTo(toggle);

            
            var matString = GameObject.Instantiate(WindowDependency.InputTextItemPrefab,parent);
            matString.ItemName = "PhysicMaterial";
            matString.SetTextWithoutNotify(PhysicMaterialId);
            matString.OnValueEdit += value => PhysicMaterialId = value;
            
            Observable.EveryUpdate()
                .Select(_=>PhysicMaterialId)
                .DistinctUntilChanged()
                .Subscribe(matString.SetTextWithoutNotify)
                .AddTo(matString);

            
            var meshString = GameObject.Instantiate(WindowDependency.InputTextItemPrefab,parent);
            meshString.ItemName = "Mesh";
            meshString.SetTextWithoutNotify(MeshId);
            meshString.OnValueEdit += value => MeshId = value;
            
            Observable.EveryUpdate()
                .Select(_=>MeshId)
                .DistinctUntilChanged()
                .Subscribe(meshString.SetTextWithoutNotify)
                .AddTo(meshString);
        }

        private void ApplyPhysicMaterial()
        {
            if(!string.IsNullOrEmpty(PhysicMaterialId))
            {
                var material = ParentObject.WorldContext.GetResource<PhysicMaterial>(PhysicMaterialId);
                _component.material = material;
            }
        }

        private async void ApplyMesh()
        {
            if (!string.IsNullOrEmpty(MeshId))
            {
                var repository = ServiceLocator.Resolve<IModelRepository>();
                var mesh = await repository.Get<Mesh>(MeshId);
                _component.sharedMesh = mesh;
            }
        }

        /// <summary>
        /// コンポーネントに必要な変数を追加する
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="managedObject"></param>
        /// <returns></returns>
        public static async UniTask<bool> CreateVariable(string componentId,IManagedObject managedObject)
        {
            var hid = managedObject.HiddenVariableManager;
            return  await hid.TryCreateIfNotExist(componentId, nameof(Convex),false) && 
                    await hid.TryCreateIfNotExist(componentId, nameof(IsTrigger),false) &&
                    await hid.TryCreateIfNotExist(componentId, nameof(MeshId), "") &&
                    await hid.TryCreateIfNotExist(componentId, nameof(PhysicMaterialId), "");
        }

        protected override void OnDispose()
        {
            GameObject.Destroy(_component);
        }
    }
}