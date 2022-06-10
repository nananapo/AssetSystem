using UnityEngine;
using VRUI;

namespace VisualScripting.Scripts.Window.Item
{
    public abstract class NamedItem : WindowItem,INamedItem
    {

        [SerializeField] private TextView nameText;

        private string _itemName = "";

        public string ItemName
        {
            get => _itemName;
            set
            {
                _itemName = value;
                nameText.text = value;
            }
        }

    }
}