using AssetSystem.Reference.Json;
using AssetSystem.Repository;
using MyWorldHub;
using UnityEngine;

namespace AssetSystem
{
    public class MeshDownloadTest : MonoBehaviour
    {
        [SerializeField] private MeshFilter filter;
        [SerializeField] private string id;
        
        private async void Start()
        {
            ServiceLocator.Register<IBundleDataRepository>(new FirebaseBundleDataRepository(new JsonReferenceConverter()));
            var rep = new TriLibModelRepository();

            var mesh = await rep.Get<Mesh>(id);
            filter.mesh = mesh;
        }
    }
}