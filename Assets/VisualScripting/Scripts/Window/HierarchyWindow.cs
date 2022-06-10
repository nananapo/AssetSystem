using System;
using System.Collections.Generic;
using System.Linq;
using AssetSystem.Unity;
using MyWorldHub.Unity;
using UniRx;
using Unity.VisualScripting;
using VisualScripting.Scripts.Window.Item;

namespace VisualScripting.Scripts.Window
{
    public class HierarchyWindow : VRUI.Window
    {

        private readonly Dictionary<string, ButtonItem> _dirButtons = new ();

        private IManagedObject _selectedObject;

        private Type _detailWindowType;

        private IWorldContext _worldContext;

        private IManagedObject _upperDirObjectCache;

        private IManagedObject SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                _upperDirObjectCache = _selectedObject.Parent;
                Refresh();
                
                //指定されたwindowを開く
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, transform.parent).gameObject;
                var window = (VRUI.Window)obj.AddComponent(_detailWindowType);
                OpenObject(window, _selectedObject);
            }
        }

        private HeaderItem _dirNameText;

        /// <summary>
        /// param[0]に開くオブジェクト
        /// param[1]は開くウィンドウ
        /// </summary>
        /// <param name="param"></param>
        public override void OnCreate(object[] param)
        {
            
            DeactivateOnOpen = false;
            CloseOnParentClose = true;

            if (param.Length != 2 ||
                param[0] is not IManagedObject || 
                param[1] is not Type)
            {
                return;
            }
            
            _selectedObject = (IManagedObject) param[0];
            _detailWindowType = (Type) param[1];
            _worldContext = _selectedObject.WorldContext;

            _dirNameText = Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform);
            _dirNameText.text = "";

            var refreshButton = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            refreshButton.text = "Refresh";
            refreshButton.onClick.AddListener(Refresh);
            
            var moveToUpperDirButton = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            moveToUpperDirButton.text = "../";
            moveToUpperDirButton.onClick.AddListener(MoveToUpperDirectory);
            
            var addNewButton = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            addNewButton.text = "Add object";
            addNewButton.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab,transform.parent);
                var window = obj.AddComponent<AddObjectWindow>();
                OpenObject(window,_selectedObject);
            });
            
            // とりあえずリフレッシュ
            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Subscribe(_ =>
                {
                    if (_selectedObject.WorldContext == null)
                    {
                        SelectedObject = _upperDirObjectCache ?? _worldContext?.GetRootObject();
                        return;
                    }
                    Refresh();
                })
                .AddTo(this);
        }

        /// <summary>
        /// 表示を更新する
        /// TODO リアルタイムにしたい とりあえず0.1秒に一回リフレッシュ
        /// </summary>
        public void Refresh()
        {
            if (SelectedObject == null)
            {
                return;
            }

            //名前変更
            _dirNameText.SetText(SelectedObject.gameObject.name);

            var children = SelectedObject.GetChildren();
            //ボタン作成
            foreach (var child in children)
            {
                var id = child.UniqueId;
                if (_dirButtons.ContainsKey(id))
                {
                    _dirButtons[id].text = child.Name;
                }
                else
                {
                    var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
                    btn.text = child.gameObject.name;
                    btn.onClick.AddListener(() => SelectedObject = child);

                    _dirButtons[id] = btn;
                }
            }
            
            // 存在しないObjectを削除
            var childKeys = children.Select(obj => obj.UniqueId).ToList();
            foreach (var (id,btn) in _dirButtons.Where(kv => !childKeys.Contains(kv.Key)).ToList())
            {
                _dirButtons.Remove(id);
                Destroy(btn.gameObject);
            }
        }

        /// <summary>
        /// 上位ディレクトリに移動する
        /// </summary>
        public void MoveToUpperDirectory()
        {
            // 親があるかチェック
            if (_selectedObject?.Parent == null)
                return;
            //移動
            SelectedObject = _selectedObject.Parent;
        }
    }
}