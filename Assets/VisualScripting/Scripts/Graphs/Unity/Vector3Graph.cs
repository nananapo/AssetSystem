using System;
using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class Vector3Graph : Graph
    {
        public Vector3Graph(string id) : base(id)
        {
            var resolverX = new ItemTypeResolver(typeof(Single), "x");
            var resolverY = new ItemTypeResolver(typeof(Single), "y");
            var resolverZ = new ItemTypeResolver(typeof(Single), "z");
            var resolverV = new ItemTypeResolver(typeof(Vector3), "Vector3");
            
            AddNode(new InItemNode(this, resolverX));
            AddNode(new InItemNode(this, resolverY));
            AddNode(new InItemNode(this, resolverZ));

            AddNode(new OutItemNode(this, resolverX,0));
            AddNode(new OutItemNode(this, resolverY,1));
            AddNode(new OutItemNode(this, resolverZ,2));
            AddNode(new OutItemNode(this, resolverV, 3));
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            return Task.FromResult(ProcessCallResult.Success(new []
            {
                parameters[0],
                parameters[1],
                parameters[2],
                new Vector3((Single)parameters[0], (Single) parameters[1], (Single) parameters[2])
            }, OutProcessNodes[0]));
        }

        public override string GraphName => "Vector3";
    }
}