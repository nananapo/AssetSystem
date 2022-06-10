using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VisualScripting.Scripts.Graphs.Nodes
{
    public class EdgeCutter : MonoBehaviour
    {

        public Transform t1;
        public Transform t2;
        
        public XRSimpleInteractable interactable;

        public void Init(Action action)
        {
            interactable.selectEntered.AddListener(_ =>action());
        }
        
        private void Update()
        {
            if(t1 != null && t2 != null)
                transform.position = (t1.position + t2.position) / 2;
        }
    }
}