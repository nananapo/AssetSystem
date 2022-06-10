using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace VisualScripting.Scripts.Window.Item
{
    public class NamedButtonItem : NamedItem,ITextView,IButtonView
    {

        public ChangeEvent onClick { get; set; } = new();

        [SerializeField] private Button button;
        
        [SerializeField] private TextView buttonText;

        public string text
        {
            get => buttonText.text; 
            set => buttonText.text = value;
        }

        private void Awake()
        {
            button.onClick.AddListener(()=>onClick?.Invoke());
        }

        public void SetText(string text)
        {
            buttonText.SetText(text);
        }

        public string GetText()
        {
            return buttonText.GetText();
        }
    }
}