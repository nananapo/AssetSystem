using System;
using UnityEngine;

namespace VRUI
{
    [RequireComponent(typeof(Window))]
    public class WindowInitializer : MonoBehaviour
    {
        private void Start()
        {
            var window = GetComponent<Window>();
            window.OnCreate(Array.Empty<object>());
            window.OnActivated();
        }
    }
}