using System;
using System.Collections.Generic;
using System.Linq;
using AssetSystem;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Graphs.Event;
using GraphConnectEngine.Variable;
using UnityEngine;
using VisualScripting.Scripts.Graphs;

namespace VisualScripting.Scripts.Helper
{
    public class GraphSystem : IDisposable
    {
        // プレフィックス
        public const string DefinitionPrefix = "graphdefinition";
        public const string PositionPrefix = "graphposition";
        public const string RotationPrefix = "graphrotation";

        private readonly IManagedObject _managedObject;

        /// <summary>
        /// エッジを表示するためのクラス
        /// 非表示 = null
        /// </summary>
        private EdgeViewer _edgeViewer;

        /// <summary>
        /// グラフを表示するためのクラス
        /// 非表示 = null
        /// </summary>
        private GraphViewer _graphViewer;

        /// <summary>
        /// コネクタ
        /// </summary>
        public VarNodeConnector Connector { get; }

        /// <summary>
        /// ID / グラフ
        /// </summary>
        private readonly Dictionary<string, IGraph> _graphs = new();

        /// <summary>
        /// ID / UIの設定
        /// TODO リアクティブにする
        /// </summary>
        private readonly Dictionary<string, GraphUISetting> GraphUISettings = new();

        private bool _visibility = false;
        
        /// <summary>
        /// グラフを表示中かどうか
        /// </summary>
        public bool Visibility
        {
            get => _visibility;
            set
            {
                if (_visibility == value) return;

                _visibility = value;

                if (_visibility)
                {
                    if (_managedObject.gameObject != null)
                    {
                        _graphViewer = new GraphViewer(Connector,_managedObject.HiddenVariableManager,_graphs
                            .Select(s=>(s.Value,GraphUISettings[s.Key])).ToList(),_managedObject.gameObject.transform, RemoveGraph);
                        _edgeViewer = new EdgeViewer(Connector,_managedObject.gameObject.transform);
                    }
                    else
                    {
                        _visibility = false;
                    }
                }
                else
                {
                    if (_graphViewer != null)
                    {
                        _graphViewer.Dispose();
                        _graphViewer = null;
                    }

                    if (_edgeViewer != null)
                    {
                        _edgeViewer.Dispose();
                        _edgeViewer = null;
                    }
                }
            }
        }

        /// <summary>
        /// グラフを読み込むために、生成時に必ずReloadGraphを呼ぶ
        /// </summary>
        /// <param name="managedObject"></param>
        public GraphSystem(IManagedObject managedObject)
        {
            _managedObject = managedObject;

            var hid = _managedObject.HiddenVariableManager;
            Connector = new VarNodeConnector(this,hid);
            
            Visibility = false;
            
            // グラフの変更に関するリスナ
            hid.AddListener(DefinitionPrefix,OnGraphDefinitionUpdated);
            hid.AddListener(PositionPrefix,OnGraphPositionUpdated);
            hid.AddListener(RotationPrefix,OnGraphRotationUpdated);
            hid.AddListener(DefinitionPrefix,OnGraphDefinitionRemoved);
        }

        /// <summary>
        /// グラフを再読み込みする
        /// </summary>
        public async UniTask Reload()
        {

            // 表示を削除
            var beforeVisibility = Visibility;
            Visibility = false;
            
            //初期化
            _graphs.Clear();
            GraphUISettings.Clear();

            // 定義をすべて取得
            var definitions = await _managedObject.HiddenVariableManager.GetAllVariables(DefinitionPrefix);

            // グラフを読み込む
            foreach(var graphId in definitions)
            {

                //GraphTypeを取得
                var valueResult = await _managedObject.HiddenVariableManager.Get<string>(DefinitionPrefix, graphId);
                if (!valueResult.IsSucceeded) continue;
                string graphType = valueResult.Value;
                
                // 座標を取得
                valueResult = await _managedObject.HiddenVariableManager.Get<string>(PositionPrefix, graphId);
                Vector3 graphPos = valueResult.IsSucceeded ? valueResult.Value.ToVector3() : Vector3.zero;

                // 回転を取得
                valueResult = await _managedObject.HiddenVariableManager.Get<string>(RotationPrefix, graphId);
                Quaternion graphRot = valueResult.IsSucceeded ? valueResult.Value.ToQuaternion() : Quaternion.identity;
                
                // グラフの作成
                var graph = GraphInitializer.Instantiate(graphId, graphType,Connector,_managedObject);
                if (graph == null)
                {
                    Debug.LogError($"Failed to Initialize Graph : {graphId} , {graphType}");
                    continue;
                }

                // グラフの追加
                AddGraph(graph,graphPos,graphRot);
            }
            
            // ノードの再読み込み
            Connector.Reload();

            // 元の表示に戻す
            Visibility = beforeVisibility;
        }

        
        #region グラフのリスナ

        private async void OnGraphDefinitionUpdated(VariableUpdatedEventArgs args)
        {
            var graphId = args.Name;
            
            if (args.Type != typeof(string) ||
                _graphs.ContainsKey(graphId)) return;
            
            // グラフを生成する
            var graph = GraphInitializer.Instantiate(graphId, args.Value.ToString(),Connector,_managedObject);
            if (graph == null)
            {
                Debug.LogError($"Failed to Initialize Graph : {graphId} , {args.Value}");
                return;
            }
            
            // 座標を取得
            var valueResult = await _managedObject.HiddenVariableManager.Get<string>(PositionPrefix, graphId);
            Vector3 graphPos = valueResult.IsSucceeded ? valueResult.Value.ToVector3() : Vector3.zero;

            // 回転を取得
            valueResult = await _managedObject.HiddenVariableManager.Get<string>(RotationPrefix, graphId);
            Quaternion graphRot = valueResult.IsSucceeded ? valueResult.Value.ToQuaternion() : Quaternion.identity;
            
            // TODO グラフの生成時に生成する変数はどこが生成する？(サーバー側のみ？(が好ましい))

            // commit
            AddGraph(graph,graphPos, graphRot,false);
        }
        
        private void OnGraphPositionUpdated(VariableUpdatedEventArgs args)
        {
            if (args.Type != typeof(string)) return;

            var pos = args.Value.ToString().ToVector3();

            if (GraphUISettings.ContainsKey(args.Name))
            {
                GraphUISettings[args.Name].Position = pos;
            }
            else
            {
                GraphUISettings[args.Name] = new GraphUISetting
                {
                    Position = pos,
                    Rotation = Quaternion.identity
                };
            }
        }
        
        private void OnGraphRotationUpdated(VariableUpdatedEventArgs args)
        {
            if (args.Type != typeof(string)) return;

            var rot = args.Value.ToString().ToQuaternion();

            if (GraphUISettings.ContainsKey(args.Name))
            {
                GraphUISettings[args.Name].Rotation = rot;
            }
            else
            {
                GraphUISettings[args.Name] = new GraphUISetting
                {
                    Position = Vector3.zero,
                    Rotation = rot
                };
            }
        }
        
        private void OnGraphDefinitionRemoved(VariableRemovedEventArgs args)
        {
            RemoveGraph(args.Name);
        }
        
        #endregion
        
        
        /// <summary>
        /// FixedUpdateグラフを呼ぶ
        /// </summary>
        public void OnFixedUpdate()
        {
            foreach (var graph in _graphs.Values.OfType<UpdaterGraph>())
            {
                //TODO ループしてしまう
                graph.Update(_managedObject.ObjectContext,Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// グラフを追加する
        /// 既に初期化したグラフを渡す
        ///
        /// 内部向けで、公開する予定なし
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="overwriteSetting"></param>
        private void AddGraph(IGraph graph,Vector3 position,Quaternion rotation,bool overwriteSetting = true)
        {
            // 既に存在するならスルー
            var graphId = graph.Id;
            
            if (_graphs.ContainsKey(graphId))
            {
                return;
            }
            
            _graphs[graphId] = graph;

            if (overwriteSetting || !GraphUISettings.ContainsKey(graphId))
            {
                GraphUISettings[graphId] = new GraphUISetting
                {
                    Position = position,
                    Rotation = rotation
                };
            }

            // グラフを表示する
            if (Visibility)
            {
                _graphViewer.OnAddGraph(graph, position, rotation);
            }
        }

        /// <summary>
        /// グラフを削除する
        /// </summary>
        /// <param name="graphId"></param>
        private void RemoveGraph(string graphId)
        {
            if(_graphs.TryGetValue(graphId, out _))
            {
                _graphs.Remove(graphId);
                GraphUISettings.Remove(graphId);

                //表示しているなら消す
                if (Visibility)
                {
                    //TODO _edgeViewerは存在しないedgeに対してどうしてる？
                    //TODO _edgeViewer.OnRemoveGraph(graphId);
                    _graphViewer.OnRemoveGraph(graphId);
                }
            }
        }

        /// <summary>
        /// グラフを取得する
        /// </summary>
        /// <param name="graphId"></param>
        /// <returns></returns>
        public IGraph GetGraph(string graphId)
        {
            return _graphs.ContainsKey(graphId) ? _graphs[graphId] : null;
        }

        public void Dispose()
        {
            //グラフを削除
            foreach(var id in _graphs.Keys.ToList())
            {
                RemoveGraph(id);
            }
            
            _edgeViewer?.Dispose();
            _graphViewer?.Dispose();
            Connector?.Dispose();
        }
    }
}