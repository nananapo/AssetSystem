using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using GraphConnectEngine.Variable;

namespace VisualScripting.Scripts.Helper
{

    /// <summary>
    /// 状態を変える関数はすべてfalseが返されるが、成功した場合はイベントが呼ばれる
    /// </summary>
    public class VarNodeConnector : IAsyncNodeConnector, IDisposable
    {

        public const string ConnectionPrefix = "nodeconnection";

        public event EventHandler<NodeConnectEventArgs> OnConnect;
        public event EventHandler<NodeConnectEventArgs> OnDisconnect;

        private INodeConnector _connector = new NodeConnector();

        private readonly GraphSystem _system;

        private readonly IHiddenVariableManager _holder;

        public VarNodeConnector(GraphSystem system, IHiddenVariableManager holder)
        {
            _system = system;
            _holder = holder;

            Reload();
            
            _holder.AddListener(ConnectionPrefix,OnVariableUpdated);
            _holder.AddListener(ConnectionPrefix,OnVariableRemoved);
        }

        /// <summary>
        /// VariableHolderから再読み込みする
        /// </summary>
        public async void Reload()
        {
            _connector = new NodeConnector();
            
            _connector.OnConnect += OnConnected;
            _connector.OnDisconnect += OnDisconnected;

            var values = (await _holder.GetAllVariables(ConnectionPrefix))
                .Select(s => s.Split("_"))
                .Where(s => s.Length == 2)
                .Select(s => (GetNodeFromString(s[0].Split(":")), GetNodeFromString(s[1].Split(":"))))
                .Where(s => s.Item1 != null && s.Item1 != s.Item2)
                .ToList();

            foreach (var (node1, node2) in values)
            {
                _connector.ConnectNode(node1, node2);
            }
        }

        private void OnConnected(object _, NodeConnectEventArgs args)
        {
            InvokeConnectEvent(args);
        }

        private void OnDisconnected(object _, NodeConnectEventArgs args)
        {
            InvokeDisconnectEvent(args);
        }

        public void InvokeConnectEvent(NodeConnectEventArgs args)
        {
            OnConnect?.Invoke(this, args);
        }

        public void InvokeDisconnectEvent(NodeConnectEventArgs args)
        {
            OnDisconnect?.Invoke(this, args);
        }

        public T[] GetOtherNodes<T>(INode key)
        {
            return _connector.GetOtherNodes<T>(key);
        }

        public INode[] GetOtherNodes(INode key)
        {
            return _connector.GetOtherNodes(key);
        }

        public bool TryGetOtherNodes<T>(INode key, out T[] result)
        {
            return _connector.TryGetOtherNodes(key, out result);
        }

        public bool TryGetOtherNodes(INode key, out INode[] result)
        {
            return _connector.TryGetOtherNodes(key, out result);
        }

        public bool TryGetAnotherNode<T>(INode key, out T result) where T : class
        {
            return _connector.TryGetAnotherNode(key, out result);
        }

        public bool TryGetAnotherNode(INode key, out INode result)
        {
            return _connector.TryGetAnotherNode(key, out result);
        }

        public bool IsConnected(INode node1, INode node2)
        {
            return _connector.IsConnected(node1, node2);
        }

        public bool ConnectNode(INode node1, INode node2)
        {
            _holder.TryCreateIfNotExist(ConnectionPrefix, GetVariableName(node1,node2),"",false);
            return false;
        }

        public bool DisconnectNode(INode node1, INode node2)
        {
            _holder.Remove(ConnectionPrefix, GetVariableName(node1,node2));
            return false;
        }

        public bool DisconnectAllNode(INode node)
        {
            foreach (var another in _connector.GetOtherNodes(node))
            {
                DisconnectNode(node, another);
            }
            return false;
        }

        public ISet<(INode, INode)> GetAllNodePairs()
        {
            return _connector.GetAllNodePairs();
        }

        public Task<T[]> GetOtherNodesAsync<T>(INode key)
        {
            return Task.FromResult(_connector.GetOtherNodes<T>(key));
        }

        public Task<INode[]> GetOtherNodesAsync(INode key)
        {
            return Task.FromResult(_connector.GetOtherNodes(key));
        }

        public Task<ValueResult<T[]>> TryGetOtherNodesAsync<T>(INode key)
        {
            if (_connector.TryGetOtherNodes<T>(key, out var result))
            {
                return Task.FromResult(ValueResult<T[]>.Success(result));
            }

            return Task.FromResult(ValueResult<T[]>.Fail());
        }

        public Task<ValueResult<INode[]>> TryGetOtherNodesAsync(INode key)
        {
            if (_connector.TryGetOtherNodes(key, out var result))
            {
                return Task.FromResult(ValueResult<INode[]>.Success(result));
            }

            return Task.FromResult(ValueResult<INode[]>.Fail());
        }

        public Task<ValueResult<T>> TryGetAnotherNodeAsync<T>(INode key) where T : class
        {
            if (_connector.TryGetAnotherNode<T>(key, out var result))
            {
                return Task.FromResult(ValueResult<T>.Success(result));
            }

            return Task.FromResult(ValueResult<T>.Fail());
        }

        public Task<ValueResult<INode>> TryGetAnotherNodeAsync(INode key)
        {
            if (_connector.TryGetAnotherNode(key, out var result))
            {
                return Task.FromResult(ValueResult<INode>.Success(result));
            }

            return Task.FromResult(ValueResult<INode>.Fail());
        }

        public Task<bool> IsConnectedAsync(INode node1, INode node2)
        {
            return Task.FromResult(_connector.IsConnected(node1, node2));
        }

        public Task<bool> ConnectNodeAsync(INode node1, INode node2)
        {
            _holder.TryCreateIfNotExist(ConnectionPrefix, GetVariableName(node1,node2),"",false);
            return Task.FromResult(false);
        }

        public Task<bool> DisconnectNodeAsync(INode node1, INode node2)
        {
            _holder.Remove(ConnectionPrefix, GetVariableName(node1,node2));
            return Task.FromResult(false);
        }

        public Task<bool> DisconnectAllNodeAsync(INode node)
        {
            foreach (var another in _connector.GetOtherNodes(node))
            {
                DisconnectNode(node, another);
            }
            return Task.FromResult(false);
        }

        public Task<ISet<(INode, INode)>> GetAllNodePairsAsync()
        {
            return Task.FromResult(_connector.GetAllNodePairs());
        }
        
        public void Dispose()
        {
            _holder.RemoveListener(ConnectionPrefix,OnVariableUpdated);
            _holder.RemoveListener(ConnectionPrefix,OnVariableRemoved);
        }

        private void OnVariableUpdated(VariableUpdatedEventArgs args)
        {
            var sep = args.Name.Split("_");

            if (sep.Length != 2) return;

            var node1 = GetNodeFromString(sep[0].Split(":"));
            var node2 = GetNodeFromString(sep[1].Split(":"));

            if (node1 == null || node2 == null) return;
            
            _connector.ConnectNode(node1, node2);
        }

        private void OnVariableRemoved(VariableRemovedEventArgs args)
        {
            var sep = args.Name.Split("_");

            if (sep.Length != 2) return;

            var node1 = GetNodeFromString(sep[0].Split(":"));
            var node2 = GetNodeFromString(sep[1].Split(":"));

            if (node1 == null || node2 == null) return;
            
            _connector.DisconnectNode(node1, node2);
        }

        private string GetVariableName(INode node1, INode node2)
        {
            return SerializeNode(node1) + "_" + SerializeNode(node2);
        }

        private INode GetNodeFromString(string[] nodeData)
        {
            if (nodeData.Length != 3) return null;
            
            // 値のチェック
            if (!int.TryParse(nodeData[1], out var type) ||
                !int.TryParse(nodeData[2], out var index) ||
                type < 0 || type > 3)
            {
                return null;
            }

            // graphを取得する
            var graph = _system.GetGraph(nodeData[0]);

            // graphが存在するか確認
            if (graph == null)
            {
                return null;
            }

            // nodeを取得する
            INode node = null;

            switch (type)
            {
                case 0:
                    node = graph.InProcessNodes.GetOrNull(index);
                    break;
                case 1:
                    node = graph.OutProcessNodes.GetOrNull(index);
                    break;
                case 2:
                    node = graph.InItemNodes.GetOrNull(index);
                    break;
                case 3:
                    node = graph.OutItemNodes.GetOrNull(index);
                    break;
            }

            return node;
        }

        private string SerializeNode(INode node)
        {
            var nodeType = -1;
            var nodeIndex = -1;
            switch (node)
            {
                case InProcessNode processNode:
                    nodeType = 0;
                    nodeIndex = node.Graph.InProcessNodes.IndexOf(processNode);
                    break;
                case OutProcessNode processNode:
                    nodeType = 1;
                    nodeIndex = node.Graph.OutProcessNodes.IndexOf(processNode);
                    break;
                case InItemNode itemNode:
                    nodeType = 2;
                    nodeIndex = node.Graph.InItemNodes.IndexOf(itemNode);
                    break;
                case OutItemNode itemNode:
                    nodeType = 3;
                    nodeIndex = node.Graph.OutItemNodes.IndexOf(itemNode);
                    break;
            }

            return node.Graph.Id + ":" + nodeType + ":" + nodeIndex;
        }
    }
}