using System;
using System.Collections.Generic;
using System.Linq;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;

namespace AssetSystem.Component
{
    public static class ComponentInitializer
    {

        private static readonly Dictionary<string,
            (Func<string,IManagedObject, UniTask<bool>> CreateVariableFunc,
            Func<string, IManagedObject, ComponentBase> InstantiateComponentFunc)> _components = new ()
        {
            {BoxColliderComponent.DefinitionName, (BoxColliderComponent.CreateVariable, (id,managedObject) =>  new BoxColliderComponent(id, managedObject))},
            {CapsuleColliderComponent.DefinitionName,(CapsuleColliderComponent.CreateVariable,(id,managedObject) =>  new CapsuleColliderComponent(id, managedObject))},
            {SphereColliderComponent.DefinitionName,(SphereColliderComponent.CreateVariable,(id,managedObject) =>  new SphereColliderComponent(id, managedObject))},
            {MeshColliderComponent.DefinitionName,(MeshColliderComponent.CreateVariable,(id,managedObject) =>  new MeshColliderComponent(id, managedObject))},
            {MeshFilterComponent.DefinitionName,(MeshFilterComponent.CreateVariable,(id,managedObject) =>  new MeshFilterComponent(id, managedObject))},
            {MeshRendererComponent.DefinitionName,(MeshRendererComponent.CreateVariable,(id,managedObject) =>  new MeshRendererComponent(id, managedObject))},
            {RigidbodyComponent.DefinitionName,(RigidbodyComponent.CreateVariable,(id,managedObject) =>  new RigidbodyComponent(id, managedObject))},
            {CanvasComponent.DefinitionName,(CanvasComponent.CreateVariable,(id,managedObject) =>  new CanvasComponent(id,managedObject))},
            {ImageComponent.DefinitionName,(ImageComponent.CreateVariable,(id,managedObject) =>  new ImageComponent(id, managedObject))},
        };

        public static UniTask<ComponentBase> Instantiate(string id, string componentType, IManagedObject managedObject)
        {
            if (_components.ContainsKey(componentType))
            {
                return UniTask.FromResult(_components[componentType].InstantiateComponentFunc(id, managedObject));
            }
            return UniTask.FromResult<ComponentBase>(null);
        }
        
        public static UniTask<bool> CreateVariable(string id, string componentType, IManagedObject managedObject)
        {
            if (_components.ContainsKey(componentType))
            {
                return _components[componentType].CreateVariableFunc(id, managedObject);
            }
            return UniTask.FromResult(false);
        }

        public static IList<string> GetComponentTypes()
        {
            return _components.Keys.ToList();
        }

    }
}