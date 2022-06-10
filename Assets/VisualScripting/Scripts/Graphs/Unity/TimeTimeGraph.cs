using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class TimeTimeGraph : Graph
    {

        public TimeTimeGraph(string id) : base(id, true, true)
        {
            AddNode(new OutItemNode(this,new ItemTypeResolver(typeof(float),"Time"),0));
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            return Task.FromResult(ProcessCallResult.Success(new object[]{Time.time}, OutProcessNodes[0]));
        }

        public override string GraphName => "Time.time";
    }
}