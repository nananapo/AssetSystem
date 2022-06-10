using System;
using GraphConnectEngine.Variable;
using UnityEngine;
using VisualScripting.Scripts.Window.Item;

namespace VisualScripting.Scripts.Window
{
    public class VariableDetailWindow : VRUI.Window
    {

        private IVariableHolderEvent _event;

        private IVariableHolder _variableHolder;

        private IAsyncVariableHolder _asyncVariableHolder;
        
        private TextItem _typeText;
        
        private InputTextItem _valueInput;

        private string _variableName;
        
        private Type _variableType;

        public override void OnCreate(object[] param)
        {
            
            DeactivateOnOpen = false;
            CloseOnParentClose = true;

            if (param.Length != 2)
            {
                Debug.LogError("parameter mismatch.");
                return;
            }
            
            var obj = param[0];
            _variableName = (string) param[1];
            
            if (obj is IVariableHolder variableHolder)
            {
                _variableHolder = variableHolder;
            }
            else if (obj is IAsyncVariableHolder asyncVariableHolder)
            {
                _asyncVariableHolder = asyncVariableHolder;
            }
            else
            {
                Debug.LogError("param[0] is not variableHolder.");
                return;
            }
            
            if (obj is IVariableHolderEvent holderEvent)
            {
                _event = holderEvent;
                _event.OnVariableRemoved += OnVariableRemoved;
                _event.OnVariableUpdated += OnVariableUpdated;
            }

            // タイトル
            var title = Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform);
            title.text = "Variable Detail";

            // 変数名
            var nameText = Instantiate(WindowDependency.TextItemPrefab, ScrollContentTransform);
            nameText.ItemName = "Name";
            nameText.text = _variableName;
            
            // 型
            _typeText = Instantiate(WindowDependency.TextItemPrefab, ScrollContentTransform);
            _typeText.ItemName = "Type";
            _typeText.text = "INITIALIZING";

            // 値
            _valueInput = Instantiate(WindowDependency.InputTextItemPrefab, ScrollContentTransform);
            _valueInput.ItemName = "Value";
            _valueInput.SetTextWithoutNotify("INITIALIZING");
            _valueInput.OnValueEdit.AddListener(str =>
            {
                object valueToSave;
                if (_variableType == typeof(int))
                {
                    if (!int.TryParse(str, out var r)) return;
                    valueToSave = r;
                }
                else if (_variableType == typeof(float))
                {
                    if (!float.TryParse(str, out var r)) return;
                    valueToSave = r;
                }
                else if (_variableType == typeof(string))
                {
                    valueToSave = str;
                }
                else
                {
                    return;
                }
                
                if (_variableHolder != null)
                    _variableHolder.Update(_variableName, valueToSave);
                else
                    _asyncVariableHolder.UpdateAsync(_variableName, valueToSave);
            });
            
            // 削除ボタン
            var removeButton = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            removeButton.text = "Remove";
            removeButton.onClick.AddListener( () =>
            {
                if (_variableHolder != null)
                {
                    _variableHolder.Remove(_variableName);
                }
                else if (_asyncVariableHolder != null)
                {
                    _asyncVariableHolder.RemoveAsync(_variableName);
                }
            });

            // 値の更新
            RefreshValue();
        }
        
        private async void RefreshValue()
        {

            if (_variableHolder != null)
            {
                var typeResult = _variableHolder.TryGetVariableType(_variableName);
                if (!typeResult.IsSucceeded) return;
                _variableType = typeResult.Value;
                
                var resultValue = _variableHolder.TryGet(_variableName);
                if (!resultValue.IsSucceeded) return;
                _valueInput.SetTextWithoutNotify(resultValue.Value.ToString());
            }
            else if(_asyncVariableHolder != null)
            {
                var typeResult = await _asyncVariableHolder.TryGetVariableTypeAsync(_variableName);
                if (!typeResult.IsSucceeded) return;
                _variableType = typeResult.Value;
                
                var resultValue = await _asyncVariableHolder.TryGetAsync(_variableName);
                if (!resultValue.IsSucceeded) return;
                _valueInput.SetTextWithoutNotify(resultValue.Value.ToString());
            }
            
            _typeText.text = _variableType.Name;
        }

        private void OnVariableRemoved(object sender, VariableRemovedEventArgs args)
        {
            if (args.Name == _variableName)
            {
                Close();
            }
        }

        private void OnVariableUpdated(object sender, VariableUpdatedEventArgs args)
        {
            if (args.Name == _variableName)
            {
                RefreshValue();
            }
        }
        private void OnDestroy()
        {
            if (_event != null)
            {
                _event.OnVariableRemoved -= OnVariableRemoved;
                _event.OnVariableUpdated -= OnVariableUpdated;
            }
        }
    }
}