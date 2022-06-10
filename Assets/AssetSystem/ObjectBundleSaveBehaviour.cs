using AssetSystem.Reference;
using AssetSystem.Repository;
using AssetSystem.Unity;
using MyWorldHub;
using UnityEngine;

namespace AssetSystem
{
    public class ObjectBundleSaveBehaviour : MonoBehaviour
    {
        [SerializeField] private Test_IManagedObjectProvider target;
        [SerializeField] private string id;
        
        private async void Start()
        {
            Physics.autoSimulation = false;
            
            var repository = ServiceLocator.Resolve<IBundleDataRepository>();
            var converter = ServiceLocator.Resolve<IReferenceConverter>();
            
            var masterBundle = await converter.CreateMasterBundleAsync(target.Get());
            await repository.SaveBundle(id, masterBundle);
            
            Physics.autoSimulation = true;
        }
    }
}