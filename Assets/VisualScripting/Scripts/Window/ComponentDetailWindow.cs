using AssetSystem.Component;
using AssetSystem.Unity;
using UnityEngine;
namespace VisualScripting.Scripts.Window
{
    public class ComponentDetailWindow : VRUI.Window
    {

        private IManagedObject _managedObject;

        private ComponentBase _component;

        public override void OnCreate(object[] param)
        {
            
            DeactivateOnOpen = false;
            CloseOnParentClose = true;

            if (param.Length < 2 ||
                param[0] is not IManagedObject || 
                param[1] is not ComponentBase)
            {
                Debug.LogError("param mismatch");
                return;
            }
            
            _managedObject = (IManagedObject)param[0];
            _component = (ComponentBase)param[1];

            // IDと型
            var componentIdTextView = Instantiate(WindowDependency.TextItemPrefab,ScrollContentTransform);
            componentIdTextView.text = _component.UniqueId;
            
            var componentTypeTextView = Instantiate(WindowDependency.TextItemPrefab,ScrollContentTransform);
            componentTypeTextView.text = _component.ComponentName;

            // 詳細
            _component.InstantiateDetailWindow(ScrollContentTransform);

            // 削除リスナ
            _managedObject!.OnComponentRemoved += OnComponentRemoved;
            
            // 削除ボタン
            var removeComponentButton = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            removeComponentButton.text = "Remove Component";
            removeComponentButton.onClick.AddListener(() =>
            {
                _managedObject.TryRemoveComponent(_component.UniqueId);
            });
        }

        private void OnComponentRemoved(object sender,string componentId)
        {
            if (componentId == _component!.UniqueId)
            {
                Close();
            }
        }

        private void OnDestroy()
        {
            _managedObject.OnComponentRemoved -= OnComponentRemoved;
        }
    }
}