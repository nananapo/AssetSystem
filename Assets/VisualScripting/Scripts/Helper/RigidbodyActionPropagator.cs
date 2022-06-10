using UnityEngine;
using System;

namespace VisualScripting.Scripts.Helper{
    public class RigidbodyActionPropagator : MonoBehaviour{

        public Action<Collision> OnCollisionEnterAction;
        public Action<Collision> OnCollisionExitAction;
        public Action<Collider> OnTriggerEnterAction;
        public Action<Collider> OnTriggerExitAction;

        private void OnCollisionEnter(Collision collision){
            OnCollisionEnterAction?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision){
            OnCollisionExitAction?.Invoke(collision);
        }

        private void OnTriggerEnter(Collider collider){
            OnTriggerEnterAction?.Invoke(collider);
        }
        private void OnTriggerExit(Collider collider){
            OnTriggerExitAction?.Invoke(collider);
        }
    } 
}