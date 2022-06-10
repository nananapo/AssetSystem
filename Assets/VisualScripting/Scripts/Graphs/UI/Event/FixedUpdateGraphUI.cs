using GraphConnectEngine.Nodes;
using UnityEngine;
using VisualScripting.Scripts.Graphs.Nodes;
using VisualScripting.Scripts.Helper;

namespace VisualScripting.Scripts.Graphs.UI.Event
{
    public class FixedUpdateGraphUI : GraphUI
    {

        [SerializeField] private NodeHandle outHandle;
 
        
        protected override void OnInit(INodeConnector connector, IHiddenVariableManager settingManager)
        {
            createItemStreamer = false;
            createProcessStreamer = false;
            outHandle.Init(Graph.OutProcessNodes[0], connector);
        }

        protected override string GetCustomGraphName() => "FixedUpdate";
    }
}