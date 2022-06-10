using UnityEngine;
using VisualScripting.Scripts.Window.Item;
using VRUI;

namespace VisualScripting.Scripts.Window
{
    public static class WindowDependency
    {
        private static TextItem _textItemPrefab;

        private static HeaderItem _headerItem;
        
        public static HeaderItem HeaderItemPrefab => _headerItem ??= Resources.Load<HeaderItem>("WindowItem/HeaderItem");

        public static TextItem TextItemPrefab => _textItemPrefab ??= Resources.Load<TextItem>("WindowItem/TextItem");

        private static InputTextItem _inputItem;

        public static InputTextItem InputTextItemPrefab => _inputItem ??= Resources.Load<InputTextItem>("WindowItem/InputTextItem");

        private static ToggleItem _toggleItem;

        public static ToggleItem ToggleItemPrefab => _toggleItem ??= Resources.Load<ToggleItem>("WindowItem/ToggleItem");

        private static Vector3Item _vector3Item;

        public static Vector3Item Vector3ItemPrefab => _vector3Item ??= Resources.Load<Vector3Item>("WindowItem/Vector3Item");

        private static ButtonItem _buttonItem;

        public static ButtonItem ButtonItemPrefab => _buttonItem ??= Resources.Load<ButtonItem>("WindowItem/ButtonItem");

        private static ScrollViewProvider _emptyWindow;
        public static ScrollViewProvider EmptyWindowPrefab => _emptyWindow ??= Resources.Load<ScrollViewProvider>("Windows/EmptyWindowPrefab");
    }
}