using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Graphs.UI.Value
{
    public class StringGraphUI : VariableValueGraphUI<string>
    {
        [SerializeField] private InputTextView text;

        private void Start()
        {
            text.OnValueEdit.AddListener(async str=>
            {
                await UpdateValueAsync(str);
            });
        }

        protected override void OnValueInitialized(string value)
        {
            text.SetTextWithoutNotify(value);
        }

        protected override void OnVariableUpdated(string value)
        {
            text.SetTextWithoutNotify(value);
        }
    }
}