using System.Collections.Generic;
using System.Linq;
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
    public class MeshRendererComponent : ComponentBase
    {
        
        public const string DefinitionName = "MeshRenderer";
        
        public override string ComponentName => DefinitionName;

        private readonly MeshRenderer _component;

        private List<string> _materialIds = new();
        
        public IList<string> MaterialIds
        {
            get => _materialIds;
            set => SetProperty(nameof(MaterialIds), string.Join(",",value));
        }
        
        public MeshRendererComponent(string id,IManagedObject managedObject) : base(id,managedObject)
        {
            _component = ParentObject.gameObject.AddComponent<MeshRenderer>();
            InitProperty<string>(nameof(MaterialIds), v =>
            {
                _materialIds = v.Split(",").ToList();
                ApplyMaterials();
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
                case nameof(MaterialIds):
                    if (args.Type == typeof(string))
                    {
                        _materialIds = args.Value.ToString().Split(",").ToList();
                        ApplyMaterials();
                    }
                    break;
            }
        }


        public override void InstantiateDetailWindow(Transform parent)
        {
            var matStrings = GameObject.Instantiate(WindowDependency.InputTextItemPrefab,parent);
            matStrings.ItemName = "Materials";
            matStrings.SetTextWithoutNotify(MaterialIds.ToString());
            matStrings.OnValueEdit += value =>
            {
                MaterialIds = value.Split(",");
            };
            
            Observable.EveryUpdate()
                .Select(_=>MaterialIds)
                .DistinctUntilChanged()
                .Subscribe(v=>matStrings.SetTextWithoutNotify(string.Join(",",v)))
                .AddTo(matStrings);
        }

        private async void ApplyMaterials()
        {
            var repository = ServiceLocator.Resolve<IModelRepository>();

            var mats = new List<Material>();
            foreach (var id in _materialIds)
            {
                var mat = await repository.Get<Material>(id);
                mats.Add(mat);
            }
            
            _component.materials = mats.ToArray();
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
            return  await hid.TryCreateIfNotExist(componentId, nameof(MaterialIds),"");
        }

        protected override void OnDispose()
        {
            GameObject.Destroy(_component);
        }
    }
}