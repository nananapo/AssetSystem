using System.Collections.Generic;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using GraphConnectEngine.Variable;
using UniRx;
using UnityEngine;
using VisualScripting.Scripts.Window;

namespace AssetSystem.Component
{
    public class BoxColliderComponent : ComponentBase
    {

        public const string DefinitionName = "BoxCollider";
        
        public override string ComponentName => DefinitionName;

        private readonly BoxCollider _component;
        
        private string _physicMaterialId;

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

        public Vector3 Center
        {
            get => _component.center;
            set => SetProperty(nameof(Center), value.Serialize());
        }

        public Vector3 Size
        {
            get => _component.size;
            set => SetProperty(nameof(Size), value.Serialize());
        }

        public BoxColliderComponent(string id,IManagedObject managedObject) : base(id,managedObject)
        {
            _component = ParentObject.gameObject.AddComponent<BoxCollider>();

            InitProperty<bool>(nameof(IsTrigger),v=>_component.isTrigger = v);
            InitProperty<Vector3,string>(nameof(Center),SerializeExtension.ToVector3,v=>_component.center = v);
            InitProperty<Vector3,string>(nameof(Size),SerializeExtension.ToVector3,v=>_component.size = v);
            InitProperty<string>(nameof(PhysicMaterialId), v =>
            {
                _physicMaterialId = v;
                ApplyPhysicMaterial();
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
                case nameof(IsTrigger):
                    if (args.Type == typeof(bool))
                    {
                        _component.isTrigger = (bool)args.Value;
                    }
                    break;
                case nameof(Center):
                    if (args.Type == typeof(string))
                    {
                        _component.center = args.Value.ToString().ToVector3();
                    }
                    break;
                case nameof(Size):
                    if (args.Type == typeof(string))
                    {
                        _component.size = args.Value.ToString().ToVector3();
                    }
                    break;
                case nameof(PhysicMaterialId):
                    if (args.Type == typeof(string))
                    {
                        _physicMaterialId = args.Value.ToString();
                        ApplyPhysicMaterial();
                    }
                    break;
            }
        }

        public override void InstantiateDetailWindow(Transform parent)
        {

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


            var center = GameObject.Instantiate(WindowDependency.Vector3ItemPrefab,parent);
            center.ItemName = nameof(Center);
            center.SetWithoutNotify(Center);
            center.OnValueEdit += value => Center = value;
            
            Observable.EveryUpdate()
                .Select(_=>Center)
                .DistinctUntilChanged()
                .Subscribe(center.SetWithoutNotify)
                .AddTo(center);


            var size = GameObject.Instantiate(WindowDependency.Vector3ItemPrefab,parent);
            size.ItemName = nameof(Size);
            size.SetWithoutNotify(Size);
            size.OnValueEdit += value => Size = value;
            
            Observable.EveryUpdate()
                .Select(_=>Size)
                .DistinctUntilChanged()
                .Subscribe(size.SetWithoutNotify)
                .AddTo(size);
        }

        private void ApplyPhysicMaterial()
        {
            if(!string.IsNullOrEmpty(PhysicMaterialId))
            {
                var material = ParentObject.WorldContext.GetResource<PhysicMaterial>(PhysicMaterialId);
                _component.material = material;
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
            return await hid.TryCreateIfNotExist(componentId, nameof(IsTrigger), false) &&
                   await hid.TryCreateIfNotExist(componentId, nameof(PhysicMaterialId), "") &&
                   await hid.TryCreateIfNotExist(componentId, nameof(Center), Vector3.zero.Serialize()) &&
                   await hid.TryCreateIfNotExist(componentId, nameof(Size), Vector3.one.Serialize());
        }

        protected override void OnDispose()
        {
            GameObject.Destroy(_component);
        }
    }
}