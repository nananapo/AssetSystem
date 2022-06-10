using AssetSystem.Reference;
using AssetSystem.Repository;
using MyWorldHub;
using MyWorldHub.Unity;
using UnityEngine;

namespace AssetSystem.Unity
{
    public class Test_WorldLoader : MonoBehaviour
    {

        public string worldId;
        
        private async void Start()
        {
            var repository = ServiceLocator.Resolve<IBundleDataRepository>();

            var (result, desc) = await repository.GetWorldDescription(worldId);
            if (!result)
                return;

            IMasterBundle bundle;
            (result, bundle) = await repository.GetMasterBundle(desc.BundleId);
            if (!result)
                return;

            IWorldContext context = new WorldContext(worldId, bundle);
            await context.LoadWorldResourcesAsync();
            await context.LoadWorldAsync();
            
            ServiceLocator.Register<IWorldContext>(context);
        }
    }
}