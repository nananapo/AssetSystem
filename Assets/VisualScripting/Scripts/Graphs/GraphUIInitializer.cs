using System;
using GraphConnectEngine;
using GraphConnectEngine.Graphs;
using GraphConnectEngine.Graphs.Event;
using GraphConnectEngine.Graphs.Value;
using GraphConnectEngine.Nodes;
using UnityEngine;
using VisualScripting.Scripts.Graphs.UI;
using VisualScripting.Scripts.Graphs.Unity;
using VisualScripting.Scripts.Helper;

namespace VisualScripting.Scripts.Graphs
{
    public static class GraphUIInitializer
    {
        public static GraphUI Initialize(IGraph targetGraph,INodeConnector connector,IHiddenVariableManager variableManager,Transform parent,Vector3 position,Quaternion rotation,Action removeAction)
        {

            GraphUI prefab;
            
            switch (targetGraph)
            {
                case UpdaterGraph graph:
                    prefab = GraphUIDependency.UpdateGraphUIPrefab;
                    break;
                case ButtonProcessGraph graph:
                    prefab = GraphUIDependency.ButtonGraphPrefab;
                    break;
                case DebugTextGraph graph:
                    prefab = GraphUIDependency.DebugTextGraphUIPrefab;
                    break;
                case ValueFuncGraph<string> graph:
                    prefab = GraphUIDependency.StringGraphUIPrefab;
                    break;
                case null:
                    // Error
                    return null;
                default:
                    prefab = GraphUIDependency.SimpleGraphUIPrefab;
                    break;
            }
            
            // 生成
            var ui = GameObject.Instantiate(prefab,parent);
            ui.transform.localPosition = position;
            ui.transform.localRotation = rotation;
            
            // 初期化
            ui.Init(targetGraph,connector,variableManager,removeAction);

            return ui;
        }
    }
}