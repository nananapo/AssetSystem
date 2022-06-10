using UnityEngine;

namespace VisualScripting.Scripts.Window.Item
{
    /// <summary>
    /// DestroyされたらゲームオブジェクトもDestroy
    /// </summary>
    public abstract class WindowItem : MonoBehaviour
    {
        private void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}