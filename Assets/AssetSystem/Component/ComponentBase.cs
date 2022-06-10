using System;
using System.Collections.Generic;
using AssetSystem.Reference;
using AssetSystem.Unity;
using GraphConnectEngine.Variable;
using UnityEngine;

namespace AssetSystem.Component
{
    public abstract class ComponentBase : IUniqueObject, IDisposable
    {
        
        /// <summary>
        /// UniqueIdは不変
        /// </summary>
        public string UniqueId { get; }

        public abstract string ComponentName { get; }

        /// <summary>
        /// ParentObjectは不変
        /// </summary>
        protected readonly IManagedObject ParentObject;

        protected ComponentBase(string id, IManagedObject managedObject,bool registerEvent = true)
        {
            UniqueId = id;
            ParentObject = managedObject;

            if (registerEvent)
            {
                ParentObject.HiddenVariableManager.AddListener(id,OnPropertyUpdated);
            }
        }

        protected async void InitProperty<T>(string name,Action<T> onSucceed,Action onFail = null)
        {
            var result = await  ParentObject.HiddenVariableManager.Get<T>(UniqueId, name);
            if (result.IsSucceeded)
            {
                onSucceed(result.Value);
            }
            else
            {
                onFail?.Invoke();
            }
        }

        protected async void InitProperty<TValue, TStored>(string name, Func<TStored, TValue> castFunc,
            Action<TValue> onSucceed, Action onFail = null)
        {
            var result = await  ParentObject.HiddenVariableManager.Get<TStored>(UniqueId, name);
            if (result.IsSucceeded)
            {
                onSucceed(castFunc(result.Value));
            }
            else
            {
                onFail?.Invoke();
            }
        }

        protected void SetProperty<T>(string name, T value)
        {
            ParentObject.HiddenVariableManager.Update(UniqueId, name,value);
        }

        public abstract IList<string> GetResourceDependencies();

        protected virtual void OnPropertyUpdated(VariableUpdatedEventArgs args)
        {
            
        }

        public abstract void InstantiateDetailWindow(Transform parent);

        public void Dispose()
        {
            ParentObject.HiddenVariableManager.RemoveListener(UniqueId,OnPropertyUpdated);
            OnDispose();
        }

        protected virtual void OnDispose()
        {
            
        }
    }
}