using System.Collections.Generic;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using GraphConnectEngine.Variable;
using UniRx;
using UnityEngine;
using VisualScripting.Scripts.Window;

namespace AssetSystem.Component
{
    public class RigidbodyComponent : ComponentBase
    {
        
        public const string DefinitionName = "Rigidbody";
        
        public override string ComponentName => DefinitionName;

        private readonly Rigidbody _component;

        public bool UseGravity
        {
            get => _component.useGravity;
            set => SetProperty(nameof(UseGravity), value);
        }
        
        public bool IsKinematic
        {
            get => _component.isKinematic;
            set => SetProperty(nameof(IsKinematic), value);
        }
        
        public RigidbodyComponent(string id,IManagedObject managedObject) : base(id,managedObject)
        {
            if (!ParentObject.gameObject.TryGetComponent(out _component))
            {
                _component = ParentObject.gameObject.AddComponent<Rigidbody>();
            }
            InitProperty<bool>(nameof(UseGravity), v => _component.useGravity = v);
            InitProperty<bool>(nameof(IsKinematic), v => _component.isKinematic = v);
        }

        public override IList<string> GetResourceDependencies()
        {
            return new List<string>();
        }

        protected override void OnPropertyUpdated(VariableUpdatedEventArgs args)
        {
            switch (args.Name)
            {
                case nameof(UseGravity):
                    if (args.Type == typeof(bool))
                    {
                        _component.useGravity = (bool)args.Value;
                    }
                    break;
                case nameof(IsKinematic):
                    if (args.Type == typeof(bool))
                    {
                        _component.isKinematic = (bool)args.Value;
                    }
                    break;
            }
        }

        public override void InstantiateDetailWindow(Transform parent)
        {
            var useGravity = GameObject.Instantiate(WindowDependency.ToggleItemPrefab,parent);
            useGravity.ItemName = nameof(UseGravity);
            useGravity.SetIsOnWithoutNotify(UseGravity);
            useGravity.OnValueEdit += value => UseGravity = value;
            
            Observable.EveryUpdate()
                .Select(_=>UseGravity)
                .DistinctUntilChanged()
                .Subscribe(useGravity.SetIsOnWithoutNotify)
                .AddTo(useGravity);
            
            var isKinematic = GameObject.Instantiate(WindowDependency.ToggleItemPrefab,parent);
            isKinematic.ItemName = nameof(IsKinematic);
            isKinematic.SetIsOnWithoutNotify(IsKinematic);
            isKinematic.OnValueEdit += value => IsKinematic = value;
            
            Observable.EveryUpdate()
                .Select(_=>IsKinematic)
                .DistinctUntilChanged()
                .Subscribe(isKinematic.SetIsOnWithoutNotify)
                .AddTo(isKinematic);
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
            return  await hid.TryCreateIfNotExist(componentId, nameof(UseGravity),true) && 
                    await hid.TryCreateIfNotExist(componentId,nameof(IsKinematic),false);
        }

        protected override void OnDispose()
        {
            GameObject.Destroy(_component);
        }
    }
}