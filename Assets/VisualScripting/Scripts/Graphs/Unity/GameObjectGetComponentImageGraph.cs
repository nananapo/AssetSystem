using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;
using UnityEngine.UI;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class GameObjectGetComponentImageGraph : Graph
    {
        public GameObjectGetComponentImageGraph(string id) : base(id)
        {
            var resolver1 = new ItemTypeResolver(typeof(GameObject), "GameObject");
            var resolver2 = new ItemTypeResolver(typeof(RawImage), "RawImage");
            
            AddNode(new InItemNode(this, resolver1));
            AddNode(new OutItemNode(this, resolver1,0));
            AddNode(new OutItemNode(this, resolver2,1));
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            GameObject obj = (GameObject) parameters[0];

            if (!obj.TryGetComponent<RawImage>(out var image))
                return Task.FromResult(ProcessCallResult.Fail());

            return Task.FromResult(ProcessCallResult.Success(new object[]{obj,image},OutProcessNodes[0]));
        }

        public  override string GraphName => "GameObject.GetComponent<Image>";
    }
}