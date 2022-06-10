using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VRUI
{
    public class AutoInputFieldDetector : MonoBehaviour
    {

        public static TMP_InputField inputField;

        private static GameObject obj;
        
        private void Start()
        {
            obj = null;
            inputField = null;
        }

        private void Update()
        {
            var nobj = EventSystem.current.currentSelectedGameObject;
            
            if (nobj != null && nobj != obj)
            {
                if (nobj.TryGetComponent<TMP_InputField>(out var tmp))
                {
                    inputField = tmp;
                }
                obj = nobj;
            }
        }
    }
}