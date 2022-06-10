using GraphConnectEngine.Graphs;
using GraphConnectEngine.Nodes;
using System.Threading.Tasks;
using UnityEngine;
using VisualScripting.Scripts.Helper;
using VRUI;

namespace VisualScripting.Scripts.Graphs.UI.Unity
{
    public class DebugTextGraphUI : GraphUI
    {
        [SerializeField] public TextView text;

        protected override void OnInit(INodeConnector connector, IHiddenVariableManager settingManager) 
        {
            ((DebugTextGraph)Graph).SetPrintFunction(str =>
            {
                text.text = str;
                return Task.CompletedTask;
            });
        }
    }
}