using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Graphs.UI.Value
{
    public class Int32GraphUI : VariableValueGraphUI<int>
    {
        [SerializeField] private InputTextView text;

        private void Start()
        {
            text.OnValueEdit.AddListener(async str =>
            {
                if (int.TryParse(str, out int result))
                {
                    await UpdateValueAsync(result);
                }
            });
        }

        protected override void OnValueInitialized(int value)
        {
            text.SetTextWithoutNotify(value.ToString());
        }

        protected override void OnVariableUpdated(int value)
        {
            text.SetTextWithoutNotify(value.ToString());
        }
    }
}