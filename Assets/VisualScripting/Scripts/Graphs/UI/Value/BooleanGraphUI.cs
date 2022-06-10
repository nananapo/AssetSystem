using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace VisualScripting.Scripts.Graphs.UI.Value
{
    public class BooleanGraphUI : VariableValueGraphUI<bool>
    {
        
        [SerializeField] private Button changeValueButton;

        [SerializeField] private TextView valueText;

        private void Start()
        {
            changeValueButton.onClick.AddListener(async () =>
            {
                var result = await GetValueAsync();
                if (result.IsSucceeded)
                {
                    await UpdateValueAsync(!result.Value);
                }
            });
        }

        protected override void OnValueInitialized(bool value)
        {
            valueText.SetText(value.ToString());
        }

        protected override void OnVariableUpdated(bool value)
        {
            valueText.SetText(value.ToString());
        }
    }
}