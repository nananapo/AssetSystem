using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Variable;
using UnityEngine;

namespace VisualScripting.Scripts.Helper
{
    public class HiddenVariableManager : IHiddenVariableManager
    {

        private readonly IAsyncVariableHolder _asyncHolder;

        private readonly Dictionary<string, Action<VariableUpdatedEventArgs>> _updatedEvents = new ();

        private readonly Dictionary<string, Action<VariableRemovedEventArgs>> _removedEvents = new();

        public HiddenVariableManager(IAsyncVariableHolder holder)
        {
            _asyncHolder = holder;
            _asyncHolder.OnVariableUpdated += OnVariableUpdated;
            _asyncHolder.OnVariableRemoved += OnVariableRemoved;
        }

        public Task<bool> TryCreate(string id, string name, Type type)
        {
            return _asyncHolder.TryCreateAsync(((IHiddenVariableManager)this).GetPrefixedName(id,name), type);
        }

        public async Task<bool> TryCreateIfNotExist(string id, string name, Type type)
        {
            if (await _asyncHolder.ContainsKeyAsync(((IHiddenVariableManager)this).GetPrefixedName(id,name)))
            {
                return true;
            }
            else
            {
                return await TryCreate(id,name, type);
            }
        }

        public Task<bool> Remove(string id, string name)
        {
            return _asyncHolder.RemoveAsync(((IHiddenVariableManager)this).GetPrefixedName(id,name));
        }

        public Task<bool> Update(string id, string name, object value)
        {
            return _asyncHolder.UpdateAsync(((IHiddenVariableManager)this).GetPrefixedName(id,name), value);
        }

        public Task<ValueResult<T>> Get<T>(string id, string name)
        {
            return _asyncHolder.TryGetAsync<T>(((IHiddenVariableManager)this).GetPrefixedName(id,name));
        }

        public Task<ValueResult<object>> Get(string id, string name)
        {
            return _asyncHolder.TryGetAsync(((IHiddenVariableManager) this).GetPrefixedName(id, name));
        }

        public async Task<IList<string>> GetAllVariables(string id)
        {
            var prefix = ((IHiddenVariableManager) this).GetPrefix(id);
            var pLen = prefix.Length;
            
            return (await _asyncHolder.GetKeysAsync())
                .Where(s => s.StartsWith(prefix))
                .Select(s => s.Substring(pLen))
                .ToList();
        }


        private void OnVariableUpdated(object sender, VariableUpdatedEventArgs args)
        {
            foreach(var (id,action) in _updatedEvents.ToList())
            {
                var prefix = ((IHiddenVariableManager)this).GetPrefix(id);
                if (args.Name.StartsWith(prefix))
                {
                    var name = args.Name.Substring(prefix.Length);
                    action(new VariableUpdatedEventArgs(name, args.Type, args.Value));
                }
            }
        }

        private void OnVariableRemoved(object sender, VariableRemovedEventArgs args)
        {
            foreach (var (id, action) in _removedEvents.ToList())
            {
                var prefix = ((IHiddenVariableManager)this).GetPrefix(id);
                if (args.Name.StartsWith(prefix))
                {
                    var name = args.Name.Substring(prefix.Length);
                    action(new VariableRemovedEventArgs(name));
                }
            }
        }

        public void AddListener(string id, Action<VariableUpdatedEventArgs> listener)
        {
            if (!_updatedEvents.ContainsKey(id))
            {
                _updatedEvents[id] = listener;
            }
            else
            {
                _updatedEvents[id] += listener;
            }
        }

        public void AddListener(string id, Action<VariableRemovedEventArgs> listener)
        {
            if (!_removedEvents.ContainsKey(id))
            {
                _removedEvents[id] = listener;
            }
            else
            {
                _removedEvents[id] += listener;
            }
        }

        public void RemoveListener(string id, Action<VariableUpdatedEventArgs> listener)
        {
            if (!_updatedEvents.ContainsKey(id))
            {
                return;
            }
            _updatedEvents[id] -= listener;
        }

        public void RemoveListener(string id, Action<VariableRemovedEventArgs> listener)
        {
            if (!_removedEvents.ContainsKey(id))
            {
                return;
            }
            _removedEvents[id] -= listener;
        }

        public void Dispose()
        {
            _asyncHolder.OnVariableUpdated -= OnVariableUpdated;
            _asyncHolder.OnVariableRemoved -= OnVariableRemoved;
        }
    }
}