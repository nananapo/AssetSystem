using System;
using System.Collections.Generic;
using GraphConnectEngine.Nodes;
using UnityEngine;
using VisualScripting.Scripts.Graphs;
using VisualScripting.Scripts.Graphs.Nodes;

namespace VisualScripting.Scripts.Helper
{
    public class EdgeViewer : IDisposable
    {

        private const string EdgeRendererPath = "GraphSystem/Parts/EdgeRenderer";

        private const string EdgeCutterPath = "GraphSystem/Parts/EdgeCutter";

        private static EdgeRenderer _edgeRendererPrefab;
        private static EdgeCutter _edgeCutterPrefab;

        public static EdgeRenderer EdgeRendererPrefab => _edgeRendererPrefab ??= Resources.Load<GameObject>(EdgeRendererPath).GetComponent<EdgeRenderer>();
        public static EdgeCutter EdgeCutterPrefab => _edgeCutterPrefab ??= Resources.Load<EdgeCutter>(EdgeCutterPath).GetComponent<EdgeCutter>();

        private bool _isDisposed = false;

        /// <summary>
        /// コネクター
        /// </summary>
        private readonly INodeConnector Connector;

        /// <summary>
        /// エッジの親オブジェクト
        /// </summary>
        private Transform ParentTransform;

        /// <summary>
        /// 接続情報
        /// </summary>
        private Dictionary<Tuple<INode, INode>, ConnectionData> _pair2data = new();


        public EdgeViewer(INodeConnector connector,Transform target)
        {
            if(connector == null) throw new ArgumentNullException(nameof(connector));
            
            // 親を作って場所を固定
            var parentObj = new GameObject("EdgeViewer");
            ParentTransform = parentObj.transform;
            var locker = parentObj.AddComponent<TransformLocker>();
            locker.target = target;
            
            Connector = connector;
            Connector.OnConnect += OnConnect;
            Connector.OnDisconnect += OnDisconnect;

            //既に繋がれているエッジを作成
            foreach(var (a,b) in Connector.GetAllNodePairs())
            {
                OnConnect(null, new NodeConnectEventArgs(Connector, a, b));
            }
        }

        private void OnConnect(object _,NodeConnectEventArgs args)
        {
            if (_isDisposed) return;

            INode sender = args.SenderNode;
            INode another = args.OtherNode; 
            
            var tuple = Tuple.Create(sender, another);

            if (_pair2data.ContainsKey(tuple) || _pair2data.ContainsKey(Tuple.Create(another, sender)))
            {
                return;
            }

            var handle1 = sender.GetNodeHandle();
            var handle2 = another.GetNodeHandle();

            //null チェック
            if (handle1 == null || handle2 == null)
                return;
            
            //Edge
            var edgeRenderer = GameObject.Instantiate(EdgeRendererPrefab, ParentTransform);
            edgeRenderer.Init(handle1.transform, handle2.transform);

            //Cutter
            var edgeCutter = GameObject.Instantiate(EdgeCutterPrefab, ParentTransform);
            edgeCutter.Init(() =>
            {
                Connector.DisconnectNode(sender, another);
            });
            edgeCutter.t1 = handle1.transform;
            edgeCutter.t2 = handle2.transform;

            //登録
            _pair2data[tuple] = new ConnectionData()
            {
                Handle1 = handle1,
                Handle2 = handle2,
                Node1 = sender,
                Node2 = another,
                EdgeRenderer = edgeRenderer,
                EdgeCutter = edgeCutter
            };
        }

        private void OnDisconnect(object _, NodeConnectEventArgs args)
        {
            if (_isDisposed) return;

            INode sender = args.SenderNode;
            INode another = args.OtherNode;

            var tuple = Tuple.Create(sender, another);
            
            if (!_pair2data.ContainsKey(tuple))
            {
                tuple = Tuple.Create(another, sender);
                if (!_pair2data.ContainsKey(tuple))
                {
                    return;
                }
            }

            GameObject.Destroy(_pair2data[tuple].EdgeRenderer.gameObject);
            GameObject.Destroy(_pair2data[tuple].EdgeCutter.gameObject);
            
            _pair2data.Remove(tuple);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                _pair2data.Clear();

                if (ParentTransform)
                {
                    GameObject.Destroy(ParentTransform.gameObject);
                }

                if(Connector != null)
                {
                    Connector.OnConnect -= OnConnect;
                    Connector.OnDisconnect -= OnDisconnect;
                }
            }
        }

        private class ConnectionData
        {
            public INode Node1;
            public INode Node2;
            public NodeHandle Handle1;
            public NodeHandle Handle2;
            public EdgeRenderer EdgeRenderer;
            public EdgeCutter EdgeCutter;
        }
    }
}