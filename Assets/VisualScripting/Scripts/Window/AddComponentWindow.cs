using System;
using AssetSystem.Component;
using AssetSystem.Unity;
using UnityEngine;

namespace VisualScripting.Scripts.Window
{
    public class AddComponentWindow : VRUI.Window
    {
        private IManagedObject _managedObject;

        public override void OnCreate(object[] param)
        {
            
            DeactivateOnOpen = false;
            CloseOnParentClose = true;

            if (param.Length < 1 ||
                param[0] is not IManagedObject)
            {
                Debug.LogError(new ArgumentException("parameter not match"));
                return;
            }
            
            _managedObject = param[0] as IManagedObject;
            
            // コンポーネントのボタンを追加
            foreach (var type in ComponentInitializer.GetComponentTypes())
            {
                var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
                btn.text = type;
                btn.onClick.AddListener(() =>
                {
                    _managedObject.TryAddComponent(Guid.NewGuid().ToString(),type);
                });
            }
        }
    }
}