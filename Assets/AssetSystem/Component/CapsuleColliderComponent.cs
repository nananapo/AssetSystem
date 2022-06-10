using System.Collections.Generic;
using System.Globalization;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using GraphConnectEngine.Variable;
using UniRx;
using UnityEngine;
using VisualScripting.Scripts.Window;

namespace AssetSystem.Component
{
    public class CapsuleColliderComponent : ComponentBase
    {

        public const string DefinitionName = "CapsuleCollider";
        
        public override string ComponentName => DefinitionName;

        private readonly CapsuleCollider _component;
        
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

        public float Radius
        {
            get => _component.radius;
            set => SetProperty(nameof(Radius), value);
        }

        public float Height
        {
            get => _component.height;
            set => SetProperty(nameof(Height), value);
        }

        public int Direction
        {
            get => _component.direction;
            set => SetProperty(nameof(Direction), value);
        }

        public CapsuleColliderComponent(string id,IManagedObject managedObject) : base(id,managedObject)
        {
            _component = ParentObject.gameObject.AddComponent<CapsuleCollider>();

            InitProperty<bool>(nameof(IsTrigger),v=>_component.isTrigger = v);
            InitProperty<Vector3,string>(nameof(Center),SerializeExtension.ToVector3,v=>_component.center = v);
            InitProperty<float>(nameof(Radius),v => _component.radius = v);
            InitProperty<float>(nameof(Height),v => _component.height = v);
            InitProperty<int>(nameof(Direction),v => _component.direction = v);
            
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
                case nameof(Radius):
                    if (args.Type == typeof(float))
                    {
                        _component.radius = (float)args.Value;
                    }
                    break;
                case nameof(Height):
                    if (args.Type == typeof(float))
                    {
                        _component.height = (float)args.Value;
                    }
                    break;
                case nameof(Direction):
                    if (args.Type == typeof(int))
                    {
                        _component.direction = (int)args.Value;
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


            var radius = GameObject.Instantiate(WindowDependency.InputTextItemPrefab,parent);
            radius.ItemName = nameof(Radius);
            radius.SetTextWithoutNotify(Radius.ToString(CultureInfo.InvariantCulture));
            radius.OnValueEdit += str =>
            {
                if (float.TryParse(str, out var value))
                    Radius = value;
            };
            
            Observable.EveryUpdate()
                .Select(_=>Radius)
                .DistinctUntilChanged()
                .Subscribe(v=>radius.SetTextWithoutNotify(v.ToString(CultureInfo.InvariantCulture)))
                .AddTo(radius);


            var height = GameObject.Instantiate(WindowDependency.InputTextItemPrefab,parent);
            height.ItemName = nameof(Height);
            height.SetTextWithoutNotify(Height.ToString(CultureInfo.InvariantCulture));
            height.OnValueEdit += str =>
            {
                if (float.TryParse(str, out var value))
                    Height = value;
            };
            
            Observable.EveryUpdate()
                .Select(_=>Height)
                .DistinctUntilChanged()
                .Subscribe(v=>height.SetTextWithoutNotify(v.ToString(CultureInfo.InvariantCulture)))
                .AddTo(height);


            var direction = GameObject.Instantiate(WindowDependency.InputTextItemPrefab,parent);
            direction.ItemName = nameof(Direction);
            direction.SetTextWithoutNotify(Direction.ToString());
            direction.OnValueEdit += str =>
            {
                if (int.TryParse(str, out var value))
                    Direction = value;
            };
            
            Observable.EveryUpdate()
                .Select(_=>Direction)
                .DistinctUntilChanged()
                .Subscribe(v=>direction.SetTextWithoutNotify(v.ToString()))
                .AddTo(direction);
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
                   await hid.TryCreateIfNotExist(componentId, nameof(Radius), 1f) &&
                   await hid.TryCreateIfNotExist(componentId, nameof(Height), 1f) &&
                   await hid.TryCreateIfNotExist(componentId, nameof(Direction), 0);
        }

        protected override void OnDispose()
        {
            GameObject.Destroy(_component);
        }
    }
}