using UnityEngine;
using UnityEngine.UI;
using VisualScripting.Scripts.Graphs.Nodes;

namespace VisualScripting.Scripts.Graphs.Parts
{
    public class NodePair : MonoBehaviour
    {
        [SerializeField] public NodeHandle inHandle;

        [SerializeField] public NodeHandle outHandle;

        [SerializeField] public Image inImage;
        
        [SerializeField] public Image outImage;

        public void SetVisibility(bool node1, bool node2)
        {
            inHandle.gameObject.SetActive(node1);
            inImage.gameObject.SetActive(node1);

            outHandle.gameObject.SetActive(node2);
            outImage.gameObject.SetActive(node2);
        }

        public virtual void SetColor(Color color)
        {
            inImage.color = color;
            outImage.color = color;
        }
    }
}