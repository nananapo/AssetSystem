using System;
using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class GameObjectGetComponentGraph : Graph
    {

        private Type _selectedType;

        public Type SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType == value)
                    return;

                if (value.IsSubclassOf(typeof(Component)))
                {
                    _selectedType = value;
                }
                else
                {
                    _selectedType = null;
                }
            }
        }

        public GameObjectGetComponentGraph(string id) : base(id)
        {
            var resolver1 = new ItemTypeResolver(typeof(GameObject), "GameObject");
            var resolver2 = new ItemTypeResolver(typeof(void), "Component");
            
            AddNode(new InItemNode(this, resolver1));
            AddNode(new OutItemNode(this, resolver2,0));
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            GameObject gameObject = (GameObject)parameters[0];

            if (SelectedType == null)
            {
                return Task.FromResult(ProcessCallResult.Fail());
            }

            if (gameObject.TryGetComponent(SelectedType, out var result))
            {
                return Task.FromResult(ProcessCallResult.Success(new object[] {result}, OutProcessNodes[0]));
            }

            return Task.FromResult(ProcessCallResult.Fail());
        }

        public override string GraphName => "GameObjectGetComponent<Type>";
    }
}