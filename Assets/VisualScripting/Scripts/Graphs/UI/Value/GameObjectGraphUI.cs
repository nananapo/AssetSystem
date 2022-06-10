using GraphConnectEngine.Nodes;
using UnityEngine;
using VisualScripting.Scripts.Graphs.Nodes;
using VisualScripting.Scripts.Helper;

namespace VisualScripting.Scripts.Graphs.UI.Value
{
    public class GameObjectGraphUI : GraphUI
    {
        [SerializeField] private NodeHandle handle;

        protected override void OnInit(INodeConnector connector,IHiddenVariableManager settingManager)
        {
            handle.Init(Graph.OutItemNodes[0], connector);
        }
    }
}