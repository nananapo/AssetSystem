using System;
using UnityEngine;

namespace VisualScripting.Scripts.Helper
{
    public class TransformLocker : MonoBehaviour
    {

        public Transform target;

        private void Update()
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}