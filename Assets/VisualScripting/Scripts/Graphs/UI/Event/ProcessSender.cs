using System.Threading;
using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Graphs.Event;
using GraphConnectEngine.Nodes;

namespace VisualScripting.Scripts.Graphs.UI.Event
{
    public class ProcessSender :IProcessSender
    {

        private SemaphoreSlim sem;

        private bool _dismissOnMax;

        private INodeConnector Connector;

        public ProcessSender(INodeConnector connector,int semSize,bool dismissOnMax)
        {
            Connector = connector;
            sem = new SemaphoreSlim(semSize);
            _dismissOnMax = dismissOnMax;
        }
        
        public async Task Fire(object sender,IGraph graph,object[] parameters)
        {
            if (_dismissOnMax && sem.CurrentCount <= 0)
            {
                return;
            }
            
            await sem.WaitAsync();
            
            try
            {
                await graph.InvokeWithoutCheck(ProcessData.Fire(sender, Connector), true, parameters);
            }
            finally
            {
                sem.Release();
            }
        }
    }
}