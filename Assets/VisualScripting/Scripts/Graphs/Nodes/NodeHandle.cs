using GraphConnectEngine.Nodes;
using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Graphs.Nodes
{
    public class NodeHandle : MonoBehaviour
    {
        //接続
        [Header("接続")]
        [SerializeField] private NodeSolidObject nodeSolid;
        [SerializeField] private NodeMovableObject nodeMovable;

        //説明用
        //TODO 逐次生成
        [Header("ヘルプ表示")]
        [SerializeField] private GameObject helpObject;
        [SerializeField] private TextView nodeNameText;
        [SerializeField] private TextView nodeTypeText;

        /// <summary>
        /// 割り当てられたノード
        /// </summary>
        public INode Node { get; private set; }

        public INodeConnector Connector { get; private set; }

        public void Init(INode node,INodeConnector connector)
        {
            Node = node;
            Connector = connector;

            //NodeHandleを追加
            Node.SetData("NodeHandle", this);

            //動かせる部分を初期化
            nodeMovable.Init(this);
            nodeSolid.Init(this);

            //説明を消す
            helpObject.SetActive(false);
        }

        /// <summary>
        /// 説明を表示、更新
        /// </summary>
        public void ShowHelp()
        {
            helpObject.SetActive(true);
            nodeNameText.SetText(Node.GetType().Name);

            if (Node is InProcessNode || Node is OutProcessNode)
            {
                nodeTypeText.SetText("Process");
            }
            else if (Node is InItemNode iitem)
            {
                nodeTypeText.SetText("In(" + iitem.TypeResolver.GetItemType().Name + ")");
            }
            else if (Node is OutItemNode oitem)
            {
                nodeTypeText.SetText("Out(" + oitem.TypeResolver.GetItemType().Name + ")");
            }
        }

        /// <summary>
        /// 説明を消す
        /// </summary>
        public void DismissHelp()
        {
            helpObject.SetActive(false);
        }
    }
}