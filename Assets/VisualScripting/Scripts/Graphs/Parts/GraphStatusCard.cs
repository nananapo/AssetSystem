using GraphConnectEngine;
using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Graphs.Parts
{
    public class GraphStatusCard : MonoBehaviour
    {
        [SerializeField] private TextView typeText;
        [SerializeField] private TextView timeText;
        [SerializeField] private TextView idText;

        private IGraph _graph;

        public void Init(IGraph graph)
        {
            _graph = graph;

            idText.SetText(_graph.Id);
            typeText.SetText("Waiting for update");
            timeText.SetText("");
            
            _graph.OnStatusChanged += OnStatusChanged;
        }

        private void OnStatusChanged(object sender, GraphStatusEventArgs args)
        {
            idText.SetText(_graph.Id);
            typeText.SetText(args.Type.ToString());
            timeText.SetText(System.DateTime.Now.ToString("yyyy/M/d  H:m:s"));
        }
    }
}