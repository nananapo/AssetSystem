using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Window.Item
{
    public class InputTextItem : NamedItem,IInputTextView
    {
        
        [SerializeField] private InputTextView textView;
        
        public bool Editable 
        { 
            get => textView.Editable; 
            set => textView.Editable = value;
        }

        public string text
        {
            get => textView.text; 
            set => textView.text = value;
        }

        public ChangeEvent<string> OnValueEdit
        {
            get => textView.OnValueEdit; 
            set => textView.OnValueEdit = value;
        }
        
        public void SetText(string text)
        {
            textView.SetText(text);
        }

        public void SetTextWithoutNotify(string text)
        {
            textView.SetTextWithoutNotify(text);
        }

        public string GetText()
        {
            return textView.GetText();
        }
    }
}