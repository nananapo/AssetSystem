using System;
using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class ParameterOutGraph : Graph
    {
        public ParameterOutGraph(string id,string[] names,Type[] types) : base(id)
        {
            for (int i=0;i<names.Length;i++)
            {
                var key = names[i];
                var value = types[i];
                AddNode(new OutItemNode(this, new ItemTypeResolver(value, key), 0));
            }
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            return Task.FromResult(ProcessCallResult.Success(parameters, OutProcessNodes[0]));
        }

        public override string GraphName =>  "ParameterOut";
    }
}
