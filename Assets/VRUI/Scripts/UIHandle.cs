using UnityEngine;

namespace VRUI
{
    //[RequireComponent(typeof(XRGrabInteractable))]
    public class UIHandle : MonoBehaviour
    {

        public Transform root;
        
        private void Start()
        {
            if (root != null)
            {
                transform.parent = null;
                root.parent = transform;
            }
        }
    }
}