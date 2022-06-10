using System.Collections.Generic;
using GraphConnectEngine.Variable;
using Unity.VisualScripting;
using UnityEngine;
using VisualScripting.Scripts.Helper;
using VisualScripting.Scripts.Window.Item;

namespace VisualScripting.Scripts.Window
{
    /// <summary>
    /// 変数リストを表示し、作成ウィンドウと詳細ウィンドウへアクセスできるようにするウィンドウ
    ///
    /// Openで第一引数にIAsyncVariableHolderかVariableHolderを渡す。
    /// </summary>
    public class VariablesListWindow : VRUI.Window
    {

        private readonly Dictionary<string, ButtonItem> _buttons = new();

        private IAsyncVariableHolder _asyncVariableHolder;

        private IVariableHolder _variableHolder;

        private IVariableHolderEvent _variableHolderEvent;

        private bool _showHiddenVariables = false;

        public override async void OnCreate(object[] param)
        {
            
            DeactivateOnOpen = false;
            CloseOnParentClose = true;

            if (param.Length < 1)
            {
                Debug.LogError("VariableListWindow is invoked without args.");
                return;
            }
            
            // タイトル
            var title = Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform);
            title.text = "Variables";
            
            // 作成ボタン
            var createButton = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            createButton.text = "Create New";
            createButton.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, transform.parent);
                var win = obj.AddComponent<VariableCreateWindow>();
                OpenObject(win, _variableHolder != null ? _variableHolder : _asyncVariableHolder);
            });
            
            // 可視不可視トグル 
            var hiddenVarToggle = Instantiate(WindowDependency.ToggleItemPrefab, ScrollContentTransform);
            hiddenVarToggle.ItemName = "Show Hidden Variables";
            hiddenVarToggle.SetIsOnWithoutNotify(_showHiddenVariables);
            hiddenVarToggle.OnValueEdit.AddListener(async value =>
            {
                _showHiddenVariables = value;

                if (_asyncVariableHolder != null)
                    foreach (string item in await _asyncVariableHolder.GetKeysAsync())
                        CreateOrUpdateButton(item);
                
                if (_variableHolder != null)
                    foreach (string item in _variableHolder.GetKeys())
                        CreateOrUpdateButton(item);
            });

            // 変数のボタン作成
            var obj = param[0];
            if (obj is IVariableHolder variableHolder)
            {
                _variableHolder = variableHolder;
                
                foreach (string item in _variableHolder.GetKeys())
                { 
                    CreateOrUpdateButton(item);
                }
            }
            else if (obj is IAsyncVariableHolder asyncVariableHolder)
            {
                _asyncVariableHolder = asyncVariableHolder;

                foreach (string item in await _asyncVariableHolder.GetKeysAsync())
                { 
                    CreateOrUpdateButton(item);
                }
            }
            else
            {
                Debug.LogError("param[0] is not variable holder");
                return;
            }

            // リスナをつける
            if (obj is IVariableHolderEvent variableHolderEvent)
            {
                _variableHolderEvent = variableHolderEvent;
                _variableHolderEvent.OnVariableCreated += OnVariableCreated;
                _variableHolderEvent.OnVariableRemoved += OnVariableRemoved;
            }
        }

        private void CreateOrUpdateButton(string varName)
        {
            // 既にボタンがあるなら削除
            if (_buttons.ContainsKey(varName))
            {
                var oldBtn = _buttons[varName];
                Destroy(oldBtn.gameObject);
                _buttons.Remove(varName);
            }
            
            // 隠れ変数の設定チェック
            if (!_showHiddenVariables &&
                varName.StartsWith(IHiddenVariableManager.HidePrefix))
                return;

            // ボタン作成
            var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            btn.text = varName;
            btn.onClick.AddListener(() =>
            {
                // 詳細ウィンドウ
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, transform.parent);
                var win = obj.AddComponent<VariableDetailWindow>();
                OpenObject(win,_variableHolder != null ? _variableHolder : _asyncVariableHolder,varName);
            });
            _buttons[varName] = btn;

        }

        private void OnVariableCreated(object _, VariableCreatedEventArgs args)
        {
            // ボタンを作成
            CreateOrUpdateButton(args.Name);
        }

        private void OnVariableRemoved(object _, VariableRemovedEventArgs args)
        {
            //ボタンを削除
            var btn = _buttons[args.Name];
            _buttons.Remove(args.Name);
            Destroy(btn.gameObject);
        }

        private void OnDestroy()
        {
            if (_variableHolderEvent != null)
            {
                _variableHolderEvent.OnVariableCreated -= OnVariableCreated;
                _variableHolderEvent.OnVariableRemoved -= OnVariableRemoved;
            }
        }
    }
}