using System;
using System.Threading.Tasks;
using AssetSystem.Unity;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class GameObjectSetPositionGraph : Graph
    {

        public GameObjectSetPositionGraph(string id) : base(id, true, true)
        {
            AddNode(new InItemNode(this,new ItemTypeResolver(typeof(ManagedObject),"GameObject")));
            AddNode(new InItemNode(this,new ItemTypeResolver(typeof(Vector3),"Position")));
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            var obj = parameters[0] as ManagedObject;
            if (obj == null)
                return Task.FromResult(ProcessCallResult.Fail());

            if (!(parameters[1] is Vector3 pos))
            {
                return Task.FromResult(ProcessCallResult.Fail());
            }

            obj.LocalPosition = pos;

            return Task.FromResult(ProcessCallResult.Success(Array.Empty<object>(), OutProcessNodes[0]));
        }

        public override string GraphName => "GameObject.SetLocalPosition";
    }
}