using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class Vector3SetYGraph : Graph
    {

        public Vector3SetYGraph(string id) : base(id, true, true)
        {
            AddNode(new InItemNode(this,new ItemTypeResolver(typeof(Vector3),"Vector3")));
            AddNode(new InItemNode(this,new ItemTypeResolver(typeof(float),"Value")));
            AddNode(new OutItemNode(this,new ItemTypeResolver(typeof(Vector3),"Vector3"),0));
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            if (!(parameters[0] is Vector3 vec))
            {
                return Task.FromResult(ProcessCallResult.Fail());
            }
            
            if (!(parameters[1] is float value))
            {
                return Task.FromResult(ProcessCallResult.Fail());
            }

            Vector3 result = vec;
            result.y = value;
            
            return Task.FromResult(ProcessCallResult.Success(new object[]{result}, OutProcessNodes[0]));
        }

        public override string GraphName => "Vector3.SetY";
    }
}