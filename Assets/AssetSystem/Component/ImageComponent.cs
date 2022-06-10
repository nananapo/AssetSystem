using System;
using System.Collections.Generic;
using AssetSystem.Repository;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using GraphConnectEngine.Variable;
using MyWorldHub;
using UnityEngine;
using UnityEngine.UI;
using VisualScripting.Scripts.Window;

namespace AssetSystem.Component
{
    public class ImageComponent : ComponentBase
    {
        
        public const string DefinitionName = "Image";
        
        public override string ComponentName => DefinitionName;

        private readonly Image _component;
        
        private string _sourceImageId;
        
        private string _materialId;

        public Color Color
        {
            get => _component.color;
            set => SetProperty(nameof(Color), value.Serialize());
        }
        
        public string SourceImageId
        {
            get => _sourceImageId;
            set => SetProperty(nameof(SourceImageId), value);
        }
        
        public string MaterialId
        {
            get => _materialId;
            set => SetProperty(nameof(MaterialId), value);
        }

        public bool RaycastTarget
        {
            get => _component.raycastTarget;
            set => SetProperty(nameof(RaycastTarget), value);
        }

        public Vector4 RaycastPadding
        {
            get => _component.raycastPadding;
            set => SetProperty(nameof(RaycastPadding), value.Serialize());
        }

        public bool Maskable
        {
            get => _component.maskable;
            set => SetProperty(nameof(Maskable), value);
        }

        public ImageComponent(string id,IManagedObject managedObject) : base(id,managedObject)
        {
            _component = ParentObject.gameObject.AddComponent<Image>();

            InitProperty<Color,string>(nameof(Color),SerializeExtension.ToColor,v => _component.color = v);
            InitProperty<string>(nameof(SourceImageId), v =>
            {
                _sourceImageId = v;
                ApplySourceImage();
            });
            InitProperty<string>(nameof(MaterialId), v =>
            {
                _sourceImageId = v;
                ApplyMaterial();
            });
            InitProperty<bool>(nameof(RaycastTarget), v => _component.raycastTarget = v);
            InitProperty<Vector4,string>(nameof(RaycastPadding),SerializeExtension.ToVector4, v => _component.raycastPadding = v);
            InitProperty<bool>(nameof(Maskable), v => _component.maskable = v);
        }

        //TODO
        public override IList<string> GetResourceDependencies()
        {
            return Array.Empty<string>();
        }

        protected override void OnPropertyUpdated(VariableUpdatedEventArgs args)
        {
            switch (args.Name)
            {
                case nameof(Color):
                    if (args.Type == typeof(string))
                    {
                        _component.color = args.Value.ToString().ToColor();
                    }
                    break;
                case nameof(SourceImageId):
                    if (args.Type == typeof(string))
                    {
                        _sourceImageId = args.Value.ToString();
                        ApplySourceImage();
                    }
                    break;
                case nameof(MaterialId):
                    if (args.Type == typeof(string))
                    {
                        _materialId = args.Value.ToString();
                        ApplyMaterial();
                    }
                    break;
                case nameof(RaycastTarget):
                    if (args.Type == typeof(bool))
                    {
                        _component.raycastTarget = (bool)args.Value;
                    }
                    break;
                case nameof(RaycastPadding):
                    if (args.Type == typeof(string))
                    {
                        _component.raycastPadding = args.Value.ToString().ToVector4();
                    }
                    break;
                case nameof(Maskable):
                    if (args.Type == typeof(bool))
                    {
                        _component.maskable = (bool)args.Value;
                    }
                    break;
            }
        }


        public override void InstantiateDetailWindow(Transform parent)
        {
            var text = GameObject.Instantiate(WindowDependency.TextItemPrefab,parent);
            text.ItemName = "INFO";
            text.text = "Not Implemented";
        }

        private async void ApplySourceImage()
        {
            if(!string.IsNullOrEmpty(SourceImageId))
            {
                var repository = ServiceLocator.Resolve<IModelRepository>();
                var image = await repository.Get<Sprite>(SourceImageId);
                _component.sprite = image;
            }
        }
        
        private async void ApplyMaterial()
        {
            if(!string.IsNullOrEmpty(MaterialId))
            {
                var repository = ServiceLocator.Resolve<IModelRepository>();
                var mat = await repository.Get<Material>(MaterialId);
                _component.material = mat;
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
            return await hid.TryCreateIfNotExist(componentId, nameof(Color), UnityEngine.Color.clear.Serialize()) &&
                   await hid.TryCreateIfNotExist(componentId, nameof(SourceImageId), "") &&
                   await hid.TryCreateIfNotExist(componentId, nameof(MaterialId), "") &&
                   await hid.TryCreateIfNotExist(componentId, nameof(RaycastTarget), true) &&
                   await hid.TryCreateIfNotExist(componentId, nameof(RaycastPadding),Vector4.zero.Serialize()) &&
                   await hid.TryCreateIfNotExist(componentId, nameof(Maskable), false);
        }

        protected override void OnDispose()
        {
            GameObject.Destroy(_component);
        }
    }
}