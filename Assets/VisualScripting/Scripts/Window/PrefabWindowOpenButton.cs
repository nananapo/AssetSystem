using UnityEngine;
using UnityEngine.UI;

namespace VisualScripting.Scripts.Window
{
    public class PrefabWindowOpenButton : MonoBehaviour
    {
        [SerializeField] private VRUI.Window from;
        
        [SerializeField] private VRUI.Window openPrefab;

        private void Start()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                from.OpenPrefab(openPrefab);
            });
        }
    }
}