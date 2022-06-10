using UnityEngine;

namespace VisualScripting.Scripts.Helper
{
    public class DontDestroyMonoBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}