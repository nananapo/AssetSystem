using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRUI
{
    public class TextView : MonoBehaviour, ITextView
    {

        private enum TextViewType
        {
            Text,
            TextMeshPro,
            TextMeshProUGUI,
            InputField,
            TMPInputField
        }

        [SerializeField] private TextViewType viewType;

        [SerializeField] protected Text viewText;

        [SerializeField] protected TextMeshPro viewTextMeshPro;
        
        [SerializeField] protected TextMeshProUGUI viewTextMeshProUGUI;
        
        [SerializeField] protected InputField viewInputField;
        
        [SerializeField] protected TMP_InputField viewTMPInputField;

        public string text
        {
            get => GetText();
            set => SetText(value);
        }

        private void Awake()
        {
            bool isNull = true;
            switch (viewType)
            {
                case TextViewType.Text:
                    viewText ??= GetComponent<Text>();
                    isNull = viewText == null;
                    break;
                case TextViewType.TextMeshPro:
                    viewTextMeshPro ??= GetComponent<TextMeshPro>();
                    isNull = viewTextMeshPro == null;
                    break;
                case TextViewType.TextMeshProUGUI:
                    viewTextMeshProUGUI ??= GetComponent<TextMeshProUGUI>();
                    isNull = viewTextMeshProUGUI == null;
                    break;
                case TextViewType.InputField:
                    viewInputField ??= GetComponent<InputField>();
                    isNull = viewInputField == null;
                    break;
                case TextViewType.TMPInputField:
                    viewTMPInputField ??= GetComponent<TMP_InputField>();
                    isNull = viewTMPInputField == null;
                    break;
            }

            if (isNull)
            {
                viewText ??= GetComponent<Text>();
                viewTextMeshPro ??= GetComponent<TextMeshPro>();
                viewTextMeshProUGUI ??= GetComponent<TextMeshProUGUI>();
                viewInputField ??= GetComponent<InputField>();
                viewTMPInputField ??= GetComponent<TMP_InputField>();

                if (viewText == null &&
                    viewTextMeshPro == null &&
                    viewTextMeshProUGUI == null &&
                    viewInputField == null &&
                    viewTMPInputField == null)
                {
                    Debug.LogError(new ArgumentNullException(nameof(viewText)));
                    return;
                }
                else
                {
                    Debug.LogWarning("Argument is not set up correctly");
                }
            }
            
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }

        public void SetText(string text)
        {
            if (viewText != null) viewText.text = text;
            if (viewTextMeshPro != null) viewTextMeshPro.text = text;
            if (viewTextMeshProUGUI != null) viewTextMeshProUGUI.text = text;
            if (viewInputField != null) viewInputField.text = text;
            if (viewTMPInputField != null) viewTMPInputField.text = text;
        }

        public string GetText()
        {
            if (viewText != null) return viewText.text;
            if (viewTextMeshPro != null) return viewTextMeshPro.text;
            if (viewTextMeshProUGUI != null) return viewTextMeshProUGUI.text;
            if (viewInputField != null) return viewInputField.text;
            if (viewTMPInputField != null) return viewTMPInputField.text;
            return null;
        }

    }
}