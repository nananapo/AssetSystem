using System;
using System.Collections.Generic;
using System.Linq;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;
using VisualScripting.Scripts.Graphs;
using VisualScripting.Scripts.Graphs.UI;

namespace VisualScripting.Scripts.Helper
{
    public class GraphViewer : IDisposable
    {
        
        private bool _isDisposed = false;

        // Disposeはしない
        private readonly INodeConnector _connector;

        // Disposeはしない
        private readonly IHiddenVariableManager _hiddenVariableManager;

        /// <summary>
        /// 親オブジェクト
        /// </summary>
        private readonly Transform _parentTransform;

        /// <summary>
        /// 接続情報
        /// </summary>
        private readonly Dictionary<string,GraphUI> _graphUIs = new ();

        private readonly Action<string> _removeGraphAction;


        public GraphViewer(INodeConnector connector,IHiddenVariableManager hiddenVariableManager,IList<(IGraph,GraphUISetting)> graphs,Transform targetTransform,Action<string> removeGraphAction)
        {
            
            //GraphConnectEngine.Logger.SetLogMethod(Debug.Log);
            

            _connector = connector;
            _hiddenVariableManager = hiddenVariableManager;
            
            _removeGraphAction = removeGraphAction;
            
            // オブジェクトを生成
            var parent = new GameObject("GraphViewer");
            _parentTransform = parent.transform;
            
            /* 親の座標に固定する
            var locker = parent.AddComponent<TransformLocker>();
            locker.target = targetTransform;
            */

            // グラフの生成
            foreach(var (graph,setting) in graphs)
            {
                OnAddGraph(graph,setting.Position,setting.Rotation);
            }
        }

        /// <summary>
        /// グラフが追加されたら(IManagedObjectに)呼ばれる
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void OnAddGraph(IGraph graph,Vector3 position,Quaternion rotation)
        {
            if (_graphUIs.ContainsKey(graph.Id)) return;

            var ui = GraphUIInitializer.Initialize(graph,
                _connector,
                _hiddenVariableManager,
                _parentTransform, 
                position, 
                rotation,
                ()=>_removeGraphAction(graph.Id));
            
            if (ui == null)
            {
                Debug.LogError("Failed to Instantiate GraphUI");
                return;
            }

            _graphUIs[graph.Id] = ui;
        }

        /// <summary>
        /// グラフが削除されたら(IManagedObjectに)呼ばれる
        /// </summary>
        /// <param name="graphId"></param>
        public void OnRemoveGraph(string graphId)
        {
            if (!_graphUIs.ContainsKey(graphId)) return;

            var ui = _graphUIs[graphId];
            ui.Dispose();

            _graphUIs.Remove(graphId);
        }

        /// <summary>
        /// グラフが動かされたら(サーバーに)呼ばれる
        /// </summary>
        /// <param name="graphId"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void OnMovedGraph(string graphId, Vector3 position, Quaternion rotation)
        {
            
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                //UIを削除
                foreach(var id in _graphUIs.Keys.ToList())
                {
                    _graphUIs[id].Dispose();
                }
                _graphUIs.Clear();

                if (_parentTransform)
                {
                    GameObject.Destroy(_parentTransform.gameObject);
                }
            }
        }

    }
}