using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Graphs.Parts
{
    public class ItemStreamer : NodePair
    {
        [SerializeField] public TextView variableNameText;

        public void SetName(string text)
        {
            variableNameText.SetText(text);
        }
    }
}