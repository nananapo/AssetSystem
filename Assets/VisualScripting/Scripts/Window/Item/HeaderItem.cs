using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Window.Item
{
    public class HeaderItem : WindowItem,ITextView
    {
        
        [SerializeField] private TextView textView;

        public string text
        {
            get => textView.text; 
            set => textView.text = value;
        }

        public void SetText(string text)
        {
            textView.SetText(text);
        }

        public string GetText()
        {
            return textView.GetText();
        }
    }
}