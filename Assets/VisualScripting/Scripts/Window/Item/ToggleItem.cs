using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace VisualScripting.Scripts.Window.Item
{
    public class ToggleItem : NamedItem,IToggleView
    {
        
        [SerializeField] private Toggle toggle;
        public bool Editable { get; set; } = true;

        public ChangeEvent<bool> OnValueEdit { get; set; } = new();
        
        public bool IsOn {
            get => toggle.isOn;
            set => toggle.isOn = value;
        }

        private void Awake()
        {
            toggle.onValueChanged.AddListener(value =>
            {
                if (Editable)
                {
                    OnValueEdit?.Invoke(value);
                }
                else
                {
                    // 戻す
                    toggle.SetIsOnWithoutNotify(!value);
                }
            });
        }
        
        public void SetIsOnWithoutNotify(bool isOn)
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }
    }
}