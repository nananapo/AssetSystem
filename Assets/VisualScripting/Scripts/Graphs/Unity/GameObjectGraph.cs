using System.Threading.Tasks;
using AssetSystem.Unity;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class GameObjectGraph : Graph
    {

        private ManagedObject _managedObject;
        
        public GameObjectGraph(string id,ManagedObject obj) : base(id, false, false)
        {
            _managedObject = obj;
            AddNode(new OutItemNode(this,new ItemTypeResolver(typeof(ManagedObject),"GameObject"),0));
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            Debug.LogWarning(_managedObject != null);
            return Task.FromResult(ProcessCallResult.Success(new object[] {_managedObject}, null));
        }

        public override string GraphName => "GameObject";
    }
}