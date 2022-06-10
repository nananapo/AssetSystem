using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace VisualScripting.Scripts.Graphs.Parts
{
    public class GraphNameCard : MonoBehaviour
    {
        [SerializeField] public TextView graphNameText;
        [SerializeField] public Button infoButton;

        [SerializeField] public Sprite openMat;
        [SerializeField] public Sprite closeMat;

        private GraphStatusCard _statusCard;

        public void Init(string graphName,GraphStatusCard statusCard)
        {
            _statusCard = statusCard;
            
            graphNameText.SetText(graphName);
            
            infoButton.onClick.AddListener(() => ShowInfo(!statusCard.gameObject.activeSelf));
            
            ShowInfo(false);
        }

        public void ShowInfo(bool to)
        {
            _statusCard.gameObject.SetActive(to);
            infoButton.image.sprite = to ? closeMat : openMat;
        }
    }
}