using System.Collections.Generic;
using AssetSystem.Component;
using AssetSystem.Unity;
using UnityEngine;

namespace VisualScripting.Scripts.Window
{
    public class InspectorWindow : VRUI.Window
    {
        private IManagedObject _selectedObject;

        private readonly Dictionary<string, GameObject> _componentButtons = new ();

        public override void OnCreate(params object[] param)
        {
            
            DeactivateOnOpen = false;
            CloseOnParentClose = true;
            
            if (param.Length < 1 ||
                param[0] is not IManagedObject)
            {
                Debug.LogError("param mismatch");
                return;
            }
            
            _selectedObject = (IManagedObject) param[0];
            
            Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform).text = "Inspector";

            // 名前
            var nameView = Instantiate(WindowDependency.InputTextItemPrefab, ScrollContentTransform);
            nameView.ItemName = "Name";
            nameView.SetTextWithoutNotify(_selectedObject.Name);
            nameView.OnValueEdit.AddListener(value => _selectedObject.Name = value);
            Monitor(() => _selectedObject.Name,nameView.SetTextWithoutNotify);
            
            // 保存ボタン
            var saveObjectBtn  = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            saveObjectBtn.text = "Save this object";
            saveObjectBtn.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, transform.parent).gameObject;
                var window = obj.AddComponent<SaveObjectWindow>();
                OpenObject(window, _selectedObject);
            });
            
            // 削除ボタン
            var removeObjectBtn  = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            removeObjectBtn.text = "Remove this object";
            removeObjectBtn.onClick.AddListener(() =>
            {
                _selectedObject.WorldContext.RemoveObject(_selectedObject);
                Close();
            });
            
            Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform).text = "Transform";
            
            // 座標
            var positionView = Instantiate(WindowDependency.Vector3ItemPrefab, ScrollContentTransform);
            positionView.ItemName = "LocalPosition";
            positionView.SetWithoutNotify(_selectedObject.LocalPosition);
            positionView.OnValueEdit.AddListener(value => _selectedObject.LocalPosition = value);
            Monitor(() => _selectedObject.LocalPosition,positionView.SetWithoutNotify);

            // 回転
            var rotationView = Instantiate(WindowDependency.Vector3ItemPrefab, ScrollContentTransform);
            rotationView.ItemName = "LocalRotation";
            rotationView.SetWithoutNotify(_selectedObject.LocalRotation.eulerAngles);
            rotationView.OnValueEdit.AddListener(value => _selectedObject.LocalRotation = Quaternion.Euler(value));
            Monitor(() => _selectedObject.LocalRotation,value => rotationView.SetWithoutNotify(value.eulerAngles),this);

            // 拡大
            var scaleView = Instantiate(WindowDependency.Vector3ItemPrefab, ScrollContentTransform);
            scaleView.ItemName = "LocalScale";
            scaleView.SetWithoutNotify(_selectedObject.Scale);
            scaleView.OnValueEdit.AddListener(value => _selectedObject.Scale = value);
            Monitor(()=> _selectedObject.Scale,scaleView.SetWithoutNotify);
            
            
            Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform).text = "Variable";
            
            // グローバル変数
            var openGlobalBtn  = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            openGlobalBtn.text = "Global Variable";
            openGlobalBtn.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, transform.parent).gameObject;
                var window = obj.AddComponent<VariablesListWindow>();
                OpenObject(window, _selectedObject.GlobalVariable);
            });
            
            // ローカル変数
            var openLocalBtn  = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            openLocalBtn.text = "Local Variable";
            openLocalBtn.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, transform.parent).gameObject;
                var window = obj.AddComponent<VariablesListWindow>();
                OpenObject(window, _selectedObject.LocalVariable);
            });
            
            
            Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform).text = "Graph";
            
            // グラフの可視設定
            // TODO これは共有されるもの？ リスナをつける必要ある？
            var showGraphToggle = Instantiate(WindowDependency.ToggleItemPrefab, ScrollContentTransform);
            showGraphToggle.ItemName = "Show Graph";
            showGraphToggle.SetIsOnWithoutNotify(_selectedObject.GraphVisibility);
            showGraphToggle.OnValueEdit.AddListener(value => _selectedObject.GraphVisibility = value);
            
            // グラフの作成ボタン
            var openAddGraphBtn  = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            openAddGraphBtn.text = "Add Graph";
            openAddGraphBtn.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, transform.parent).gameObject;
                var window = obj.AddComponent<AddGraphWindow>();
                OpenObject(window, _selectedObject);
            });
            
            
            Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform).text = "Components";
            
            // コンポーネント追加ボタン
            var openAddCompBtn  = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            openAddCompBtn.text = "Add Component";
            openAddCompBtn.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, transform.parent).gameObject;
                var window = obj.AddComponent<AddComponentWindow>();
                OpenObject(window, _selectedObject);
            });

            // コンポーネントボタンを追加
            foreach (var (id,component) in _selectedObject.GetAllComponents())
            {
                AddComponentDetailButton(id,component);
            }
            
            // リスナを追加
            _selectedObject.OnComponentCreated += OnComponentCreated;
            _selectedObject.OnComponentRemoved += OnComponentRemoved;
        }

        private void OnComponentCreated(object sender, (string ComponentId, ComponentBase Component) args)
        {
            AddComponentDetailButton(args.ComponentId, args.Component);
        }

        /// <summary>
        /// コンポーネントが削除されたらボタンを消す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="componentId"></param>
        private void OnComponentRemoved(object sender, string componentId)
        {
            RemoveComponentButton(componentId);
        }

        /// <summary>
        /// ボタンを削除する
        /// </summary>
        /// <param name="componentId"></param>
        private void RemoveComponentButton(string componentId)
        {
            if (!_componentButtons.ContainsKey(componentId))
                return;
            
            //削除
            var btn = _componentButtons[componentId];
            Destroy(btn.gameObject);
            _componentButtons.Remove(componentId);
        }

        /// <summary>
        /// ボタンを作成する
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="componentBase"></param>
        private void AddComponentDetailButton(string componentId, ComponentBase componentBase)
        {
            // 既にあるなら削除
            RemoveComponentButton(componentId);

            //生成
            var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            btn.text = componentBase.ComponentName;
            btn.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, transform.parent).gameObject;
                var window = obj.AddComponent<ComponentDetailWindow>();
                OpenObject(window, _selectedObject,componentBase);
            });
            
            // 追加
            _componentButtons[componentId] = btn.gameObject;
        }

        private void OnDestroy()
        {
            if (_selectedObject != null)
            {
                // リスナを削除
                _selectedObject.OnComponentCreated -= OnComponentCreated;
                _selectedObject.OnComponentRemoved -= OnComponentRemoved;
            }
        }
    }
}