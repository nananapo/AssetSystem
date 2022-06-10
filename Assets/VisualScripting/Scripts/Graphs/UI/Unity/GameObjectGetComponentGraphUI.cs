using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Variable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VisualScripting.Scripts.Graphs.Unity;

namespace VisualScripting.Scripts.Graphs.UI.Unity
{
    /*
    //キャッシュの仕組みを入れる
    public class GameObjectGetComponentGraphUI : GraphUI
    {

        [SerializeField] private TMP_Dropdown typeSelectDropdown;

        private List<Type> _typeIndexes = new List<Type>();

        private const string TypeVariable = "ComponentType";

        protected override async UniTask<IGraph> OnInit(string id)
        {

            //設定の作成
            await SettingVariables.TryCreateIfNotExist(TypeVariable, typeof(Type));

            _typeIndexes.Add(typeof(void));
            _typeIndexes.Add(typeof(Rigidbody));
            _typeIndexes.Add(typeof(RawImage));

            typeSelectDropdown.options.Add(new TMP_Dropdown.OptionData("Select Type"));
            for (int i = 1; i < _typeIndexes.Count; i++)
            {
                typeSelectDropdown.options.Add(new TMP_Dropdown.OptionData(_typeIndexes[i].Name));
            }
            
            typeSelectDropdown.SetValueWithoutNotify(await GetOptionIndex());
            typeSelectDropdown.Select();
            typeSelectDropdown.RefreshShownValue();

            var graph = new GameObjectGetComponentGraph(id);
            
            typeSelectDropdown.onValueChanged.AddListener(async value =>
            {
                await SettingVariables.Update(TypeVariable, _typeIndexes[value]);
            });

            SettingVariables.OnHiddenVariableUpdated += OnHiddenVariableUpdated;
            SettingVariables.OnHiddenVariableRemoved += OnHiddenVariableRemoved;
            
            return graph;
        }

        private async void OnHiddenVariableUpdated(object sender, VariableUpdatedEventArgs args)
        {
            if (args.Name == TypeVariable)
            {
                typeSelectDropdown.SetValueWithoutNotify(await GetOptionIndex((Type) args.Value));
                typeSelectDropdown.Select();
                typeSelectDropdown.RefreshShownValue();
            }
        }

        private void OnHiddenVariableRemoved(object sender, VariableRemovedEventArgs args)
        {
            if (args.Name == TypeVariable)
            {
                Destroy(gameObject);
            }
        }

        private async UniTask<int> GetOptionIndex(Type type = null)
        {
            if (type == null)
            {
                var r = await SettingVariables.Get<Type>(TypeVariable);
                if (!r.IsSucceeded)
                {
                    return 0;
                }
                type = r.Value;
            }

            for (int i = 1; i < _typeIndexes.Count; i++)
            {
                if (_typeIndexes[i] == type)
                {
                    return i;
                }
            }

            return 0;
        }

        private void OnDestroy()
        {
            SettingVariables.OnHiddenVariableUpdated += OnHiddenVariableUpdated;
            SettingVariables.OnHiddenVariableRemoved += OnHiddenVariableRemoved;
            
            Dispose();
        }
    }
    */
}