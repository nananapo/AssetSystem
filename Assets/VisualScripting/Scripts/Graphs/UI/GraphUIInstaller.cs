using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VisualScripting.Scripts.Graphs.Parts;

namespace VisualScripting.Scripts.Graphs.UI
{
    public class GraphUIInstaller : MonoBehaviour
    {
        [SerializeField] public XRSimpleInteractable graphRemover;

        [Header("Canvas")] 
        [SerializeField] public Transform canvasParent;
        [SerializeField] public GraphNameCard graphNameCard;
        [SerializeField] public GraphStatusCard graphStatusCard;
    }
}