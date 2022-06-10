namespace VRUI
{
    public class InputTextView : TextView, IInputTextView
    {
        private bool _editable = false;
        
        public bool Editable
        {
            get => _editable;
            set
            {
                _editable = value;
                if (viewInputField != null) viewInputField.readOnly = !value;
                if (viewTMPInputField != null) viewTMPInputField.readOnly = !value;
            }
        }

        /// <summary>
        /// inputFieldが変更された場合に呼ばれる
        /// </summary>
        public ChangeEvent<string> OnValueEdit { get; set; } = new ();

        protected override void OnAwake()
        {
            if (viewInputField != null)
            {
                viewInputField.onValueChanged.AddListener(OnValueChanged);
            }
            else if (viewTMPInputField != null)
            {
                viewTMPInputField.onValueChanged.AddListener(OnValueChanged);
            }
            else
            {
                //Debug.LogError(new ArgumentException("textView not found or inputField."));
            }
        }

        private void OnValueChanged(string text)
        {
            OnValueEdit?.Invoke(text);
        }

        public void SetTextWithoutNotify(string text)
        {
            if (viewText != null) viewText.text = text;
            if (viewTextMeshPro != null) viewTextMeshPro.text = text;
            if (viewTextMeshProUGUI != null) viewTextMeshProUGUI.text = text;
            if (viewInputField != null) viewInputField.SetTextWithoutNotify(text);
            if (viewTMPInputField != null) viewTMPInputField.SetTextWithoutNotify(text);
        }

        private void OnDestroy()
        {
            if (viewInputField != null)
            {
                viewInputField.onValueChanged.RemoveListener(OnValueChanged);
            }
            else if (viewTMPInputField != null)
            {
                viewTMPInputField.onValueChanged.RemoveListener(OnValueChanged);
            }
        }
    }
}