using AssetSystem.Reference;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AssetSystem.Resource
{
    public class PhysicMaterialResource : IResourceReference
    {
        public string Name;

        public float DynamicFriction;

        public float StaticFriction;

        public float Bounciness;

        public int FrictionCombine;

        public int BounceCombine;

        public UniTask<UnityEngine.Object> Generate()
        {
            var material = new PhysicMaterial
            {
                name = Name,
                dynamicFriction = DynamicFriction,
                staticFriction = StaticFriction,
                bounciness = Bounciness,
                frictionCombine = (PhysicMaterialCombine) FrictionCombine,
                bounceCombine = (PhysicMaterialCombine) BounceCombine
            };
            return UniTask.FromResult((UnityEngine.Object)material);
        }
    }
}