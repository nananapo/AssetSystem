using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class InstantiateGraph : Graph
    {

        private GameObject _gameObject;
        
        public InstantiateGraph(string id,GameObject gameObject) : base(id)
        {
            var resolver1 = new ItemTypeResolver(typeof(Vector3), "position");
            var resolver2 = new ItemTypeResolver(typeof(GameObject), "Instantiated GameObject");
            
            AddNode(new InItemNode(this, resolver1));
            AddNode(new OutItemNode(this, resolver1,0));
            AddNode(new OutItemNode(this, resolver2,1));

            _gameObject = gameObject;
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {

            Vector3 position = (Vector3) parameters[0];
            var obj = GameObject.Instantiate(_gameObject, position, Quaternion.identity);
            
            return Task.FromResult(ProcessCallResult.Success(new object[]
            {
                position,
                obj
            },OutProcessNodes[0]));
        }

        public override string GraphName => "Instantiate";
    }
}