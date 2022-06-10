using GraphConnectEngine.Graphs.Event;
using GraphConnectEngine.Nodes;
using UnityEngine;
using UnityEngine.UI;
using VisualScripting.Scripts.Graphs.Nodes;
using VisualScripting.Scripts.Graphs.Unity;
using VisualScripting.Scripts.Helper;

namespace VisualScripting.Scripts.Graphs.UI.Event
{
    public class ButtonProcessSenderGraphUI : GraphUI
    {

        [SerializeField] private NodeHandle outHandle;

        [SerializeField] private Button btn;

        protected override void OnInit(INodeConnector connector, IHiddenVariableManager settingManager)
        {
            createProcessStreamer = false;
            createItemStreamer = false;

            outHandle.Init(Graph.OutProcessNodes[0], connector);

            btn.onClick.AddListener(async () =>
            {
                await ((ButtonProcessGraph)Graph).Update("local");
            });
        }

        protected override string GetCustomGraphName() => "Button";
    }
}