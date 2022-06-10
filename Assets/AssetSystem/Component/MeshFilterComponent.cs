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
    public class MeshFilterComponent : ComponentBase
    {
        
        public const string DefinitionName = "MeshFilter";
        
        public override string ComponentName => DefinitionName;

        private readonly MeshFilter _component;

        private string _meshId;
        
        public string MeshId
        {
            get => _meshId;
            set => SetProperty(nameof(MeshId), value);
        }
        
        public MeshFilterComponent(string id,IManagedObject managedObject) : base(id,managedObject)
        {
            _component = ParentObject.gameObject.AddComponent<MeshFilter>();
            InitProperty<string>(nameof(MeshId), v =>
            {
                _meshId = v;
                ApplyMesh();
            });
        }

        public override IList<string> GetResourceDependencies()
        {
            return new List<string>();
        }

        protected override void OnPropertyUpdated(VariableUpdatedEventArgs args)
        {
            switch (args.Name)
            {
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
            return await hid.TryCreateIfNotExist(componentId, nameof(MeshId), "");
        }

        protected override void OnDispose()
        {
            GameObject.Destroy(_component);
        }
    }
}