using UnityEngine;

namespace VisualScripting.Scripts.Graphs.Parts
{
    public class ProcessStreamer : NodePair
    {
        public override void SetColor(Color color)
        {
            outImage.color = color;
        }
    }
}