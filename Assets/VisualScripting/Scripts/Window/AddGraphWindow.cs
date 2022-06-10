using System;
using AssetSystem.Unity;
using MyWorldHub;
using MyWorldHub.Controller;
using UnityEngine;
using VisualScripting.Scripts.Graphs;

namespace VisualScripting.Scripts.Window
{
    public class AddGraphWindow : VRUI.Window
    {

        private IManagedObject _managedObject;

        public override void OnCreate(object[] param)
        {
            
            DeactivateOnOpen = false;
            CloseOnParentClose = true;
            
            if (param.Length < 1 ||
                param[0] is not IManagedObject)
            {
                Debug.LogError("param mismatch");
                return;
            }
            
            _managedObject = param[0] as IManagedObject;

            foreach (var type in GraphInitializer.GetGraphTypes())
            {
                var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
                btn.text = type;
                btn.onClick.AddListener(() =>
                {
                    var g = new GameObject();
                    g.transform.parent = _managedObject.gameObject.transform;
                    g.transform.position = ServiceLocator.Resolve<PlayerProvider>().GetPlayerFrontPosition(0.5f);

                    _managedObject.TryAddGraph(Guid.NewGuid().ToString(),type,g.transform.position, Quaternion.identity);
                    
                    Destroy(g);
                });
            }
        }
    }
}