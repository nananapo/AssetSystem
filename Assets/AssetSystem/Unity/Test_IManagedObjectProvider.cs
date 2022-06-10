using UnityEngine;

namespace AssetSystem.Unity
{
    public class Test_IManagedObjectProvider : MonoBehaviour
    {
        public IManagedObject Get()
        {
            return gameObject.transform.GetChild(0).GetComponent<IManagedObject>();
        }
    }
}