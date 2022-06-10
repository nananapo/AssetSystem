using System;
using System.Collections.Generic;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using GraphConnectEngine.Variable;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using VisualScripting.Scripts.Window;

namespace AssetSystem.Component
{
    public class CanvasComponent : ComponentBase
    {

        public const string DefinitionName = "Canvas";
        
        public override string ComponentName => DefinitionName;

        private readonly Canvas _canvas;
        private readonly CanvasScaler _canvasScaler;
        private readonly GraphicRaycaster _graphicRaycaster;
        private readonly TrackedDevicePhysicsRaycaster _trackedDevicePhysicsRaycaster;

        public RenderMode RenderMode
        {
            get => _canvas.renderMode;
            set => SetProperty(nameof(RenderMode), (int)value);
        }
        
        public int SortingOrder
        {
            get => _canvas.sortingOrder;
            set => SetProperty(nameof(SortingOrder), value);
        }
        
        public CanvasComponent(string id,IManagedObject managedObject) : base(id,managedObject)
        {
            var gameObject = ParentObject.gameObject;
            _canvas = gameObject.AddComponent<Canvas>();
            _canvasScaler = gameObject.AddComponent<CanvasScaler>();
            _graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            _trackedDevicePhysicsRaycaster = gameObject.AddComponent<TrackedDevicePhysicsRaycaster>();

            InitProperty<RenderMode,int>(nameof(RenderMode),v=>(RenderMode)v,v=>_canvas.renderMode = v);
            InitProperty<int>(nameof(SortingOrder),v => _canvas.sortingOrder = v);
        }

        public override IList<string> GetResourceDependencies()
        {
            return Array.Empty<string>();
        }

        protected override void OnPropertyUpdated(VariableUpdatedEventArgs args)
        {
            switch (args.Name)
            {
                case nameof(RenderMode):
                    if (args.Type == typeof(int))
                    {
                        _canvas.renderMode = (RenderMode)((int)args.Value);
                    }
                    break;
                case nameof(SortingOrder):
                    if (args.Type == typeof(int))
                    {
                        _canvas.sortingOrder = (int)args.Value;
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

        /// <summary>
        /// コンポーネントに必要な変数を追加する
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="managedObject"></param>
        /// <returns></returns>
        public static async UniTask<bool> CreateVariable(string componentId,IManagedObject managedObject)
        {
            var hid = managedObject.HiddenVariableManager;
            return await hid.TryCreateIfNotExist(componentId, nameof(RenderMode), (int)RenderMode.WorldSpace) &&
                   await hid.TryCreateIfNotExist(componentId, nameof(SortingOrder), 0);
        }

        protected override void OnDispose()
        {
            GameObject.Destroy(_canvas);
            GameObject.Destroy(_canvasScaler);
            GameObject.Destroy(_graphicRaycaster);
            GameObject.Destroy(_trackedDevicePhysicsRaycaster);
        }
    }
}