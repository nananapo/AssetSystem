using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using GraphConnectEngine.Variable;
using UnityEngine;
using VisualScripting.Scripts.Graphs.Nodes;
using VisualScripting.Scripts.Helper;

namespace VisualScripting.Scripts.Graphs.UI.Value
{
    public abstract class VariableValueGraphUI<T> : GraphUI
    {

        private const string ValueVariable = "Value";

        [SerializeField] private NodeHandle outHandle;

        private IHiddenVariableManager _settingManager;

        protected override async void OnInit(INodeConnector connector, IHiddenVariableManager settingManager)
        {
            _settingManager = settingManager;
            outHandle.Init(Graph.OutItemNodes[0], connector);

            // ���X�i��ǉ�
            settingManager.AddListener(Graph.Id, OnHiddenVariableUpdated);
            settingManager.AddListener(Graph.Id, OnHiddenVariableRemoved);

            // �l���擾����
            var result = await GetValueAsync();
            if (result.IsSucceeded)
            {
                OnValueInitialized(result.Value);
            }
        }

        private void OnHiddenVariableUpdated(VariableUpdatedEventArgs args)
        {
            if (args.Name == ValueVariable)
            {
                OnVariableUpdated((T) args.Value);
            }
        }

        private void OnHiddenVariableRemoved(VariableRemovedEventArgs args)
        {
            if (args.Name == ValueVariable)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// ���������ɌĂ΂��
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnValueInitialized(T value);

        /// <summary>
        /// �l���擾����
        /// </summary>
        /// <returns></returns>
        public async Task<ValueResult<T>> GetValueAsync()
        {
            return await _settingManager.Get<T>(Graph.Id, ValueVariable);
        }

        /// <summary>
        /// �l���X�V����
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected async Task<bool> UpdateValueAsync(T value)
        {
            return await _settingManager.Update(Graph.Id, ValueVariable, value);
        }

        /// <summary>
        /// �l���X�V���ꂽ�Ƃ��ɌĂ΂��
        /// 
        /// Update
        /// ��
        /// OnVariableUpdated
        /// 
        /// �̂悤�ɌĂ΂��̂Ń��[�v�ɒ���
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnVariableUpdated(T value);

        private void OnDestroy()
        {
            _settingManager.RemoveListener(Graph.Id, OnHiddenVariableUpdated);
            _settingManager.RemoveListener(Graph.Id, OnHiddenVariableRemoved);
            Dispose();
        }

    }
}