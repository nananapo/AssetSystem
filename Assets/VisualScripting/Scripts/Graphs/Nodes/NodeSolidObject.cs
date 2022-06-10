using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Nodes
{
    public class NodeSolidObject : MonoBehaviour
    {
        
        private NodeHandle _handle;
        
        private void Start()
        {
            tag = "NodeSolidObject";
        }

        public void Init(NodeHandle handle)
        {
            _handle = handle;
        }

        public NodeHandle GetHandle()
        {
            return _handle;
        }
    }
}