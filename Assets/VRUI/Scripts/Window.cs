using System;
using AssetSystem.Unity;
using UniRx;
using UnityEngine;

namespace VRUI
{
    [RequireComponent(typeof(ScrollViewProvider))]
    public abstract class Window : MonoBehaviour
    {
        
        [NonSerialized] public bool DeactivateOnOpen = true;

        [NonSerialized] public bool CloseOnParentClose = true;
        
        private Window _openedWindow;

        private Action _closeCallback;

        private Window _sender;
        
        private Transform _scrollContentParent;
        
        protected Transform ScrollContentTransform => _scrollContentParent ??= GetComponent<ScrollViewProvider>().Content;

        private void OnInstantiate(Window sender)
        {
            if (sender == null) return;
            _sender = sender;
            sender._closeCallback += OnParentClose;
        }

        public virtual void OnCreate(object[] parameters)
        {
            
        }

        public virtual void OnActivated()
        {
            
        }

        protected virtual void OnDeactivated()
        {
            
        }
        
        public void OpenPrefab(Window windowPrefab,params object[] parameters)
        {
            // null check
            if (windowPrefab == null)
            {
                throw new ArgumentNullException(nameof(windowPrefab));
            }

            if (windowPrefab.gameObject.scene.IsValid())
            {
                throw new ArgumentException("window is not prefab. it's object. you must use OpenObject with object window.");
            }
            
            Debug.Log($"Opening Window(Prefab) {windowPrefab} {windowPrefab.GetType().FullName}");
            
            // close opened window
            if (_openedWindow != null)
            {
                _openedWindow._closeCallback -= OnChildClose;
                _openedWindow.Close();
                _openedWindow = null;
            }
            
            // deactivate self
            if (DeactivateOnOpen)
            {
                gameObject.SetActive(false);
                OnDeactivated();
            }

            _openedWindow = Instantiate(windowPrefab, transform.parent);
            _openedWindow._closeCallback += OnChildClose;
            
            _openedWindow.OnInstantiate(this);
            _openedWindow.OnCreate(parameters);
            _openedWindow.OnActivated();
        }
        
        public void OpenObject(Window windowObject,params object[] parameters)
        {
            
            // null check
            if (windowObject == null)
            {
                throw new ArgumentNullException(nameof(windowObject));
            }

            if (!windowObject.gameObject.scene.IsValid())
            {
                throw new ArgumentException("window is not object. it's prefab. you must use OpenPrefab with window prefab.");
            }
            
            Debug.Log($"Opening Window(Object) {windowObject} {windowObject.GetType().FullName}");

            // close opened window
            if (_openedWindow != null)
            {
                _openedWindow._closeCallback -= OnChildClose;
                _openedWindow.Close();
                _openedWindow = null;
            }
            
            // deactivate self
            if (DeactivateOnOpen)
            {
                gameObject.SetActive(false);
                OnDeactivated();
            }

            _openedWindow = windowObject;
            _openedWindow._closeCallback += OnChildClose;
            
            _openedWindow.OnInstantiate(this);
            _openedWindow.OnCreate(parameters);
            _openedWindow.OnActivated();
        }

        private void OnChildClose()
        {
            if (DeactivateOnOpen || !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                OnActivated();
            }
        }

        private void OnParentClose()
        {
            if (CloseOnParentClose)
            {
                if (_sender != null)
                {
                    _sender._closeCallback -= OnParentClose;
                }
                Close();
            }
        }

        public void Close()
        {
            Destroy(gameObject);

            if (_sender != null)
            {
                _sender._closeCallback -= OnParentClose;
            }
            
            _closeCallback?.Invoke();
        }

        private void OnDestroy()
        {
            if (_openedWindow != null)
            {
                _openedWindow._closeCallback -= OnChildClose;
            }
        }
        
        public void Monitor<T>(Func<T> valueProvider, Action<T> onValueChanged, GameObject context)
        {
            Observable
                .EveryUpdate()
                .Select(_ => valueProvider())
                .DistinctUntilChanged()
                .Subscribe(onValueChanged)
                .AddTo(context);
        }
        
        public void Monitor<T>(Func<T> valueProvider, Action<T> onValueChanged)
        {
            Monitor(valueProvider,onValueChanged,gameObject);
        }

        
        public void Monitor<T>(Func<T> valueProvider, Action<T> onValueChanged, Transform context)
        {
            Monitor(valueProvider,onValueChanged,context.gameObject);
        }
        
        public void Monitor<T>(Func<T> valueProvider, Action<T> onValueChanged, Component context)
        {
            Monitor(valueProvider,onValueChanged,context.gameObject);
        }
        
        public void Monitor<T>(Func<T> valueProvider, Action<T> onValueChanged, IManagedObject context)
        {
            Monitor(valueProvider,onValueChanged,context.gameObject);
        }
    }
}