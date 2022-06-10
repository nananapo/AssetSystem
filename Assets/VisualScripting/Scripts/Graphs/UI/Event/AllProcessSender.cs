using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Graphs.Event;
using GraphConnectEngine.Nodes;

namespace VisualScripting.Scripts.Graphs.UI.Event
{
    public class AllProcessSender : IProcessSender
    {
        private INodeConnector Connector;

        public AllProcessSender(INodeConnector connector)
        {
            Connector = connector;
        }

        public Task Fire(object sender,IGraph graph, object[] parameters)
        {
            graph.InvokeWithoutCheck(ProcessData.Fire(sender, Connector), true,parameters);
            return Task.CompletedTask;
        }
    }
}