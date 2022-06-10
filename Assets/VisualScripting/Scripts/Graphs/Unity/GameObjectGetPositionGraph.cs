using System.Threading.Tasks;
using AssetSystem.Unity;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class GameObjectGetPositionGraph : Graph
    {

        public GameObjectGetPositionGraph(string id) : base(id, true, true)
        {
            AddNode(new InItemNode(this,new ItemTypeResolver(typeof(ManagedObject),"GameObject")));
            AddNode(new OutItemNode(this,new ItemTypeResolver(typeof(Vector3),"Position"),0));
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            var obj = parameters[0] as ManagedObject;
            if (obj == null)
                return Task.FromResult(ProcessCallResult.Fail());

            return Task.FromResult(ProcessCallResult.Success(new object[] {obj.LocalPosition}, null));
        }

        public override string GraphName => "GameObject.GetLocalPosition";
    }
}