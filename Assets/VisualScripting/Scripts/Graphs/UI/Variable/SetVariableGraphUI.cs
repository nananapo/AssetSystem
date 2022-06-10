using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Graphs.Variable;
using GraphConnectEngine.Variable;
using TMPro;
using UnityEngine;
using VisualScripting.Scripts.Window;

namespace VisualScripting.Scripts.Graphs.UI.Variable
{
    /*
    public class SetVariableGraphUI : GraphUI
    {

        [SerializeField] private TMP_Dropdown selectDropdown;

        private Dictionary<string, TMP_Dropdown.OptionData> opDic = new Dictionary<string, TMP_Dropdown.OptionData>();

        private const string SelectedVariable = "SelectedVariable";

        protected override async UniTask<IGraph> OnInit(string id)
        {

            //設定の作成
            await SettingVariables.TryCreateIfNotExist(SelectedVariable, "");

            var graph = new SetVariableAsyncGraph(id, GraphSystem.GlobalVariable);

            //追加
            selectDropdown.options.Add(new TMP_Dropdown.OptionData("Select Variable"));
            var keys = await graph.Holder.GetKeysAsync();
            for (int i = 0; i < keys.Length; i++)
            {
                AddOption(keys[i]);
            }
            
            //選択されている変数を取得
            var vResult = await SettingVariables.Get<string>(SelectedVariable);
            if (!vResult.IsSucceeded)
            {
                //TODO エラーハンドリング
                vResult = ValueResult<string>.Success("");
            }

            //選択されている変数をdropdowmでも選択
            var sI = GetOptionIndex(vResult.Value);
            selectDropdown.SetValueWithoutNotify(sI == -1 ? 0 : sI);
            selectDropdown.Select();
            selectDropdown.RefreshShownValue();

            selectDropdown.onValueChanged.AddListener(async index =>
            {
                string variableName = index == 0 ? "" : selectDropdown.options[index].text;

                if (!await SettingVariables.Update(SelectedVariable, variableName))
                {
                    var result = await SettingVariables.Get<string>(SelectedVariable);

                    if (result.IsSucceeded)
                    {
                        var i = GetOptionIndex(result.Value);
                        if (i != -1)
                        {
                            selectDropdown.SetValueWithoutNotify(i);
                            selectDropdown.Select();
                            selectDropdown.RefreshShownValue();
                            return;
                        }
                    }

                    selectDropdown.SetValueWithoutNotify(0);
                    selectDropdown.Select();
                    selectDropdown.RefreshShownValue();
                }
            });

            //リスナをつける
            graph.Holder.OnVariableCreated += OnVariableCreated;
            graph.Holder.OnVariableRemoved += OnVariableRemoved;
            SettingVariables.OnHiddenVariableUpdated += OnHiddenVariableUpdated;
            
            return graph;
        }

        private void OnHiddenVariableUpdated(object sender,VariableUpdatedEventArgs args)
        {
            if (args.Name == SelectedVariable)
            {
                var i = GetOptionIndex(args.Name);
                if (i != -1)
                {
                    selectDropdown.SetValueWithoutNotify(i);
                    selectDropdown.Select();
                    selectDropdown.RefreshShownValue();
                }
            }
        }

        private void OnVariableCreated(object sender, VariableCreatedEventArgs args)
        {
            AddOption(args.Name);
        }

        private void AddOption(string itemName)
        {
            if (itemName.StartsWith(VariablesListWindow.HidePrefix))
                return;

            var op = new TMP_Dropdown.OptionData(itemName);
            selectDropdown.options.Add(op);
            opDic[itemName] = op;
        }

        private void OnVariableRemoved(object sender, VariableRemovedEventArgs args)
        {
            if (opDic.ContainsKey(args.Name))
            {
                selectDropdown.options.Remove(opDic[args.Name]);
                opDic.Remove(args.Name);

                selectDropdown.SetValueWithoutNotify(0);
                selectDropdown.Select();
                selectDropdown.RefreshShownValue();
            }
        }

        private int GetOptionIndex(string value)
        {
            for (int i = 0; i < selectDropdown.options.Count; i++)
            {
                if (selectDropdown.options[i].text == value)
                {
                    return i;
                }
            }
            return -1;
        }

        private void OnDestroy()
        {
            if (ParentObject != null)
            {
                ((SetVariableAsyncGraph) Graph).Holder.OnVariableCreated -= OnVariableCreated;
                ((SetVariableAsyncGraph) Graph).Holder.OnVariableRemoved -= OnVariableRemoved;
            }

            SettingVariables.OnHiddenVariableUpdated -= OnHiddenVariableUpdated;

            Dispose();
        }

    }
    */
}