using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class MathfCosGraph : Graph
    {

        public MathfCosGraph(string id) : base(id, true, true)
        {
            AddNode(new InItemNode(this,new ItemTypeResolver(typeof(float),"Rad")));
            AddNode(new OutItemNode(this,new ItemTypeResolver(typeof(float),"Sin"),0));
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            if (!(parameters[0] is float value))
            {
                return Task.FromResult(ProcessCallResult.Fail());
            }
            
            return Task.FromResult(ProcessCallResult.Success(new object[]{Mathf.Cos(value)}, OutProcessNodes[0]));
        }

        public override string GraphName => "Mathf.Cos";
    }
}