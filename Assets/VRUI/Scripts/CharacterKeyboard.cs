using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRUI
{
    public class CharacterKeyboard : MonoBehaviour
    {


        [SerializeField] private List<string> characters;
        [SerializeField] private List<string> shiftedCharacters;
        [SerializeField] private List<Button> keys;

        [SerializeField] private Button deleteButton;
        [SerializeField] private Button shiftButton;
        [SerializeField] private Button capsButton;
        [SerializeField] private Button cursorLeftButton;
        [SerializeField] private Button cursorRightButton;

        private int _cursorIndex = 0;
        
        private TMP_InputField _lastInputField;
        
        private bool _isCapsLocked = false;
        private bool _isShifted = false;

        private readonly List<TextView> _keyTexts = new();
        
        void Awake()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                var j = i;
                keys[i].onClick.AddListener(()=>OnKeyClicked(j));
                _keyTexts.Add(keys[i].GetComponent<TextView>());
            }
            
            RefreshKeyTexts();

            shiftButton.onClick.AddListener(() =>
            {
                _isShifted = !_isShifted;
                RefreshKeyTexts();
            });
            
            capsButton.onClick.AddListener(() =>
            {
                _isCapsLocked = !_isCapsLocked;
                RefreshKeyTexts();
            });
            
            cursorLeftButton.onClick.AddListener(() => _cursorIndex--);
            cursorRightButton.onClick.AddListener(() => _cursorIndex++);
            
            deleteButton.onClick.AddListener(() =>
            {
                var (inputField,cursor) = GetInputField();
                if (inputField == null ||
                    cursor == 0)
                    return;

                var text = inputField.text;
                if (cursor == text.Length)
                {
                    inputField.text = text.Substring(0, text.Length - 1);
                }
                else
                {
                    inputField.text = text.Substring(0, cursor-1) + text.Substring(cursor, text.Length-cursor);
                }
                _cursorIndex--;
            });
        }

        /// <summary>
        ///  キーの表示を更新
        /// </summary>
        private void RefreshKeyTexts()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                _keyTexts[i].text = GetKeyText(i);
            }
        }

        /// <summary>
        /// キーがクリックされたら呼ばれる
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private void OnKeyClicked(int index)
        {
            if (index >= characters.Count ||
                index >= shiftedCharacters.Count)
                throw new IndexOutOfRangeException();
            
            var (inputField,cursor) = GetInputField();
            if (inputField == null)
                return;

            string text = inputField.text;
            if (cursor == 0)
            {
                inputField.text = GetKeyText(index) + text;
            }
            else if (cursor == text.Length)
            {
                inputField.text = text + GetKeyText(index);
            }
            else
            {
                inputField.text = text.Substring(0, cursor)
                                  + GetKeyText(index)
                                  + text.Substring(cursor, text.Length-cursor);
            }

            _cursorIndex++;

            if (_isShifted)
            {
                _isShifted = false;
                RefreshKeyTexts();
            }
        }

        /// <summary>
        /// indexのキーのテキストを取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetKeyText(int index)
        {
            return _isCapsLocked
                ? (_isShifted ? characters[index] : shiftedCharacters[index])
                : (_isShifted ? shiftedCharacters[index] : characters[index]);
        }
        
        private (TMP_InputField field,int cursor) GetInputField()
        {
            // 選択されていない
            if (AutoInputFieldDetector.inputField == null)
            {
                return (null, -1);
            }

            var field = AutoInputFieldDetector.inputField;
            if (_lastInputField != field ||
                _cursorIndex > field.text.Length)
            {
                _cursorIndex = field.text.Length;
            }
            _lastInputField = field;

            if (_cursorIndex < 0)
            {
                _cursorIndex = 0;
            }

            return (field, _cursorIndex);
        }
        
    }
}
