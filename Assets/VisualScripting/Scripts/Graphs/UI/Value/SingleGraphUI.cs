using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Graphs.UI.Value
{
    public class SingleGraphUI : VariableValueGraphUI<float>
    {
        [SerializeField] private InputTextView text;

        private void Start()
        {
            text.OnValueEdit.AddListener(async str =>
            {
                if (float.TryParse(str, out float result))
                {
                    await UpdateValueAsync(result);
                }
            });
        }

        protected override void OnValueInitialized(float value)
        {
            text.SetTextWithoutNotify(value.ToString());
        }

        protected override void OnVariableUpdated(float value)
        {
            text.SetTextWithoutNotify(value.ToString());
        }
    }
}