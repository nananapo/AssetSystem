using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Variable;
using VisualScripting.Scripts.Window;

namespace VisualScripting.Scripts.Helper
{
    public interface IHiddenVariableManager : IDisposable
    {

        public const string HidePrefix = "!";
        
        /// <summary>
        /// ID用の接頭辞を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetPrefix(string id)
        {
            return HidePrefix + id + "_";
        }

        /// <summary>
        /// 接頭辞のついた変数名を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetPrefixedName(string id, string name)
        {
            return GetPrefix(id) + name;
        }
        
        /// <summary>
        /// 変数を作成する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<bool> TryCreate(string id, string name, Type type);

        /// <summary>
        /// 変数を作成する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> TryCreate<T>(string id, string name, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (await TryCreate(id, name, value.GetType()))
            {
                return await Update(id, name, value);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 変数を作成する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<bool> TryCreateIfNotExist(string id, string name, Type type);

        /// <summary>
        /// 変数を作成する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="overwrite"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> TryCreateIfNotExist<T>(string id, string name,T value,bool overwrite = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var isExists = (await Get(id, name)).IsSucceeded;

            if (isExists)
            { 
                return !overwrite || await Update(id, name, value);
            }
            
            if (await TryCreateIfNotExist(id, name, value.GetType()))
            {
                return await Update(id, name, value);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 変数を削除する
        /// TODO 削除してない感
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<bool> Remove(string id, string name);

        /// <summary>
        /// 変数を更新する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> Update(string id, string name, object value);

        /// <summary>
        /// 変数を型チェックして取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<ValueResult<T>> Get<T>(string id, string name);
        
        /// <summary>
        /// 変数を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<ValueResult<object>> Get(string id, string name);

        /// <summary>
        /// idに属する変数をすべて取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IList<string>> GetAllVariables(string id);

        public void AddListener(string id, Action<VariableUpdatedEventArgs> listener);

        public void AddListener(string id, Action<VariableRemovedEventArgs> listener);

        public void RemoveListener(string id, Action<VariableUpdatedEventArgs> listener);

        public void RemoveListener(string id, Action<VariableRemovedEventArgs> listener);
    }
}