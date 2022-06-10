using GraphConnectEngine.Nodes;
using VisualScripting.Scripts.Graphs.Nodes;

namespace VisualScripting.Scripts.Graphs
{
    public static class NodeExtension
    {
        public static NodeHandle GetNodeHandle(this INode nodeBase)
        {
            return nodeBase.GetData("NodeHandle") as NodeHandle;
        }
    }
}