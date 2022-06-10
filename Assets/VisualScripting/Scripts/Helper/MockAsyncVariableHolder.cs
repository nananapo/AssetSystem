using System;
using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Variable;

namespace VisualScripting.Scripts.Helper
{
    public class MockAsyncVariableHolder : IAsyncVariableHolder
    {

        private readonly VariableHolder _holder = new();

        public event EventHandler<VariableCreatedEventArgs> OnVariableCreated;
        public event EventHandler<VariableUpdatedEventArgs> OnVariableUpdated;
        public event EventHandler<VariableRemovedEventArgs> OnVariableRemoved;
        public event EventHandler<EventArgs> OnDisposed;

        public MockAsyncVariableHolder()
        {
            _holder.OnVariableCreated += (sender, args) => OnVariableCreated?.Invoke(sender, args);
            _holder.OnVariableUpdated += (sender, args) => OnVariableUpdated?.Invoke(sender, args);
            _holder.OnVariableRemoved += (sender, args) => OnVariableRemoved?.Invoke(sender, args);
        }
        
        public Task<bool> ContainsKeyAsync(string key)
        {
            return Task.FromResult(_holder.ContainsKey(key));
        }

        public Task<ValueResult<T>> TryGetAsync<T>(string key)
        {
            return Task.FromResult(_holder.TryGet<T>(key));
        }

        public Task<ValueResult<object>> TryGetAsync(string key)
        {
            return Task.FromResult(_holder.TryGet(key));
        }

        public Task<ValueResult<Type>> TryGetVariableTypeAsync(string key)
        {
            return Task.FromResult(_holder.TryGetVariableType(key));
        }

        public Task<bool> TryCreateAsync(string key, object obj)
        {
            return Task.FromResult(_holder.TryCreate(key,obj));
        }

        public Task<bool> TryCreateAsync(string key, Type type)
        {
            return Task.FromResult(_holder.TryCreate(key, type));
        }

        public Task<bool> UpdateAsync(string key, object obj)
        {
            var rs = _holder.Update(key, obj);
            return Task.FromResult(rs);
        }

        public Task<bool> RemoveAsync(string name)
        {
            return Task.FromResult(_holder.Remove(name));
        }

        public Task<string[]> GetKeysAsync()
        {
            return Task.FromResult(_holder.GetKeys());
        }
        
        public void Dispose()
        {
            _holder.Dispose();
        }

        public void CreateWithoutNotify(string key, object obj)
        {
            _holder.CreateWithoutNotify(key, obj);
        }

        public void UpdateWithoutNotify(string key, object obj)
        {
            _holder.UpdateWithoutNotify(key, obj);
        }

        public void RemoveWithoutNotify(string name)
        {
            _holder.RemoveWithoutNotify(name);
        }
    }
}