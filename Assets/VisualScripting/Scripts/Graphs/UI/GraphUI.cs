using System;
using System.Collections.Generic;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;
using UnityEngine.Serialization;
using VisualScripting.Scripts.Helper;

namespace VisualScripting.Scripts.Graphs.UI
{
    [RequireComponent(typeof(GraphUIInstaller))]
    public abstract class GraphUI : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// 割り当てられたグラフ
        /// </summary>
        public IGraph Graph { get; private set; }

        /// <summary>
        /// 依存を追加
        /// </summary>
        private GraphUIInstaller Installer { get; set; }

        /// <summary>
        /// グラフにProcessStreamerを自動生成するかどうかのフラグ
        /// </summary>
        [FormerlySerializedAs("CreateProcessStreamer")] [SerializeField] protected bool createProcessStreamer = true;

        /// <summary>
        /// グラフにItemStreamerを自動生成するかどうかのフラグ
        /// </summary>
        [FormerlySerializedAs("CreateItemStreamer")] [SerializeField] protected bool createItemStreamer = true;

        protected bool isDisposed { get; private set; }

        /// <summary>
        /// 呼ぶ
        /// TODO グラフ登録とHiddenのInit
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="connector"></param>
        /// <param name="settingManager"></param>
        /// <param name="removeFunction"></param>
        public void Init(IGraph graph, INodeConnector connector, IHiddenVariableManager settingManager, Action removeFunction)
        {
            //初期化チェック
            if (Graph != null)
            {
                throw new InvalidOperationException("GraphUI is already initialized.");
            }

            //null check
            if(connector == null)
            {
                throw new ArgumentNullException(nameof(connector));
            }

            Installer = gameObject.GetComponent<GraphUIInstaller>();
            Graph = graph;

            OnInit(connector, settingManager);

            //名前とステータス表示
            Installer.graphNameCard.Init(GetCustomGraphName(), Installer.graphStatusCard);
            Installer.graphStatusCard.Init(Graph);

            //削除ボタンにリスナをつける
            if (Installer.graphRemover != null)
            {
                Installer.graphRemover.selectEntered.AddListener(_ => removeFunction());
            }

            //ノードの自動生成
            InitNodes(connector);
        }

        protected abstract void OnInit(INodeConnector connector,IHiddenVariableManager settingManager);

        /// <summary>
        /// ノードを生成する
        /// </summary>
        private void InitNodes(INodeConnector connector)
        {
            GraphConnectEngine.Logger.Debug("GraphUI.InitNodes");

            int mp = Mathf.Max(Graph.InProcessNodes.Count, Graph.OutProcessNodes.Count);

            var colors = new[]
            {
                Color.red,
                Color.green,
                Color.blue,
                Color.yellow,
                Color.cyan
            };
            
            //process
            if (createProcessStreamer)
            {
                for (int i = 0; i < mp; i++)
                {
                    var streamer = Instantiate(GraphUIDependency.ProcessStreamerPrefab, Installer.canvasParent);

                    bool enableIn = Graph.InProcessNodes.Count > i;
                    bool enableOut = Graph.OutProcessNodes.Count > i;
                    streamer.SetVisibility(enableIn, enableOut);

                    if (enableIn)
                        streamer.inHandle.Init(Graph.InProcessNodes[i], connector);

                    if (enableOut)
                        streamer.outHandle.Init(Graph.OutProcessNodes[i], connector);
                    
                    streamer.SetColor(colors[i%colors.Length]);
                }
            }
            
            //item 
            if (createItemStreamer)
            {
                List<int> already = new List<int>();

                //まずinNode
                for (int i = 0; i < Graph.InItemNodes.Count; i++)
                {
                    var streamer = Instantiate(GraphUIDependency.ItemStreamerPrefab, Installer.canvasParent);

                    string itemName = Graph.InItemNodes[i].TypeResolver.ItemName;
                    int outIndex = -1;
                    
                    for (int j = 0; j < Graph.OutItemNodes.Count; j++)
                    {
                        if (Graph.OutItemNodes[j].TypeResolver.ItemName == itemName)
                        {
                            outIndex = j;
                            already.Add(outIndex);
                            break;
                        }
                    }

                    streamer.SetVisibility(true, outIndex != -1);
                    streamer.SetName(itemName);
                    streamer.SetColor(colors[i % colors.Length]);

                    streamer.inHandle.Init(Graph.InItemNodes[i], connector);

                    if (outIndex != -1)
                    {
                        streamer.outHandle.Init(Graph.OutItemNodes[outIndex], connector);
                    }
                }

                //OutItemNodes
                for (int i = 0; i < Graph.OutItemNodes.Count; i++)
                {
                    if (!already.Contains(i))
                    {
                        var streamer = Instantiate(GraphUIDependency.ItemStreamerPrefab, Installer.canvasParent);
                        string itemName = Graph.OutItemNodes[i].TypeResolver.ItemName;
                        
                        streamer.SetVisibility(false, true);
                        streamer.SetName(itemName);
                        streamer.SetColor(colors[i % colors.Length]);

                        streamer.outHandle.Init(Graph.OutItemNodes[i], connector);
                    }
                }
            }

        }

        /// <summary>
        /// Disposeを呼ぶ
        /// </summary>
        private void OnDestroy()
        {
            Dispose();
        }

        /// <summary>
        /// 必ず呼ぶ
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                Destroy(Installer);
                Destroy(gameObject);
            }
        }

        protected virtual string GetCustomGraphName() => Graph.GraphName;
    }
}