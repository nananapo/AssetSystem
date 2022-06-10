using System;
using AssetSystem.Reference;
using AssetSystem.Repository;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using MyWorldHub;
using UnityEngine;

namespace AssetSystem
{
    [DefaultExecutionOrder(-999999999)]
    public class WorldBundleSaveBehaviour : MonoBehaviour
    {
        [SerializeField] private string worldId;
        [SerializeField] private Test_IManagedObjectProvider provider;
        
        [SerializeField] private string worldName;
        [SerializeField] private string id;
        [SerializeField] private string authorId = "system";

        private async void Start()
        {
            await UniTask.Delay(5000);
            
            Physics.autoSimulation = false;
            
            var repository = ServiceLocator.Resolve<IBundleDataRepository>();
            var converter = ServiceLocator.Resolve<IReferenceConverter>();
            
            var masterBundle = await converter.CreateMasterBundleAsync(provider.Get());
            await repository.SaveBundle(id, masterBundle);
            await repository.SaveWorldDescription(worldId, await converter.CreateBundleDescription(
                worldName, id,authorId, DateTime.Now.ToUnixTime(),DateTime.Now.ToUnixTime()));
            
            Physics.autoSimulation = true;
        }
    }
}