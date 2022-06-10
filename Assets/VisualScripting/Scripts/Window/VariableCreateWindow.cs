using GraphConnectEngine.Variable;
using UnityEngine;
using VisualScripting.Scripts.Window.Item;

namespace VisualScripting.Scripts.Window
{
    public class VariableCreateWindow : VRUI.Window
    {
        
        private IVariableHolder _variableHolder;
        
        private IAsyncVariableHolder _asyncVariableHolder;
        
        private InputTextItem _nameInput;

        private ButtonItem _typeSelectButton;
        
        private int _typeSelectIndex = 0;

        public override void OnCreate(object[] param)
        {
            
            DeactivateOnOpen = false;
            CloseOnParentClose = true;

            if (param.Length != 1)
            {
                Debug.LogError("parameter mismatch.");
                return;
            }
            
            object obj = param[0];
            if (obj is IVariableHolder variableHolder)
            {
                _variableHolder = variableHolder;
            }
            else if (obj is IAsyncVariableHolder asyncVariableHolder)
            {
                _asyncVariableHolder = asyncVariableHolder;
            }

            // タイトル
            var title = Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform);
            title.text = "Create new variable";
            
            // 名前設定input
            _nameInput = Instantiate(WindowDependency.InputTextItemPrefab, ScrollContentTransform);
            _nameInput.ItemName = "Name";
            
            // 型設定ボタン
            var typeSelectHeader = Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform);
            typeSelectHeader.text = "Type";
            _typeSelectButton = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            _typeSelectButton.text = "Int";
            _typeSelectButton.onClick.AddListener(() =>
            {
                var buttonStrings = new [] {"Int", "String", "Float", "Bool"};
                _typeSelectIndex = (_typeSelectIndex + 1) % buttonStrings.Length;
                _typeSelectButton.text = buttonStrings[_typeSelectIndex];
            });

            // 作成ボタン
            var createButton = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            createButton.text = "Create";
            createButton.onClick.AddListener(OnCreateButtonClicked);
        }

        private async void OnCreateButtonClicked()
        {
            var nameText = _nameInput.text;

            if (nameText.Length == 0)
            {
                return;
            }

            object defaultValue;
            switch (_typeSelectIndex)
            {
                case 0:
                    defaultValue = 0;
                    break;
                case 1:
                    defaultValue = 0f;
                    break;
                case 2:
                    defaultValue = "";
                    break;
                case 3:
                    defaultValue = true;
                    break;
                default:
                    return;
            }

            if (_variableHolder != null)
            {
                if (_variableHolder.TryCreate(nameText, defaultValue))
                {
                    _nameInput.text = "";
                    Close();
                }
            }
            else if (_asyncVariableHolder != null)
            {
                if (await _asyncVariableHolder.TryCreateAsync(nameText, defaultValue))
                {
                    _nameInput.text = "";
                    Close();
                }
            }
        }
        
    }
}