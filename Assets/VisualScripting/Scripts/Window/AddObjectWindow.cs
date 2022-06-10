using System.Collections.Generic;
using System.Linq;
using AssetSystem.Reference;
using AssetSystem.Repository;
using AssetSystem.Unity;
using MyWorldHub;
using MyWorldHub.Controller;
using MyWorldHub.Unity;
using UnityEngine;
using VisualScripting.Scripts.Window.Item;

namespace VisualScripting.Scripts.Window
{
    public class AddObjectWindow : VRUI.Window
    {

        private readonly List<ButtonItem> _buttonItems = new ();

        private IManagedObject _targetObject;

        private bool _processing = false;

        private long _startTime = 0;

        private const int Limit = 10;

        public override void OnCreate(object[] parameters)
        {
            if (parameters.Length != 1 ||
                parameters[0] is not IManagedObject)
            {
                Debug.LogError("parameter mismatch");
                return;
            }

            _targetObject = (IManagedObject) parameters[0];
            LoadPage(0,true,true,true);
        }

        private void AddNextBtn(long time,bool at)
        {
            var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            btn.text = "Next >>";
            btn.onClick.AddListener(()=>
            {
                LoadPage(time,true,at);
            });
            _buttonItems.Add(btn);
        }

        private void AddPreviewBtn(long time,bool at)
        {
            var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            btn.text = "<< Prev";
            btn.onClick.AddListener(()=>
            {
                LoadPage(time,false,at);
            });
            _buttonItems.Add(btn);
        }

        private async void LoadPage(long createdTime,bool start,bool at,bool asStart = false)
        {
            if(_processing) return;
            _processing = true;
            
            // 既にあるボタンを削除
            foreach (var buttonItem in _buttonItems)
            {
                Destroy(buttonItem);
            }
            _buttonItems.Clear();

            try
            {
                // リストを取得
                var repository = ServiceLocator.Resolve<IBundleDataRepository>();
                var data = await repository.GetObjectList(createdTime,start,at,Limit);

                // 0なら最後のページと表示
                if (data.Count == 0)
                {
                    var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
                    btn.text = "last page";
                    _buttonItems.Add(btn);

                    if (!asStart)
                    {
                        AddPreviewBtn(createdTime,true);
                    }
                    return;
                }

                // ボタンを追加
                foreach (var (_, description) in data)
                {
                    var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
                    btn.text = description.Name;
                    btn.onClick.AddListener(() => OnClick(description));
                    _buttonItems.Add(btn);
                }

                var time1 = data[0].Description.CreatedTime;
                if (asStart)
                {
                    _startTime = time1;
                }

                if (!asStart && time1 != _startTime)
                {
                    AddPreviewBtn(time1,false);
                }

                if (data.Count < Limit)
                {
                    AddNextBtn(data.Last().Description.CreatedTime,false);
                }
                
            }
            finally
            {
                _processing = false;
            }
            
        }

        private async void OnClick(IBundleDescription description)
        {
            if(_processing) return;
            _processing = true;

            var repository = ServiceLocator.Resolve<IBundleDataRepository>();
            try
            {
                var (result, bundle) = await repository.GetMasterBundle(description.BundleId);
                if (result)
                {
                    var g = new GameObject();
                    g.transform.parent = _targetObject.gameObject.transform;
                    g.transform.position = ServiceLocator.Resolve<PlayerProvider>().GetPlayerFrontPosition(0.5f);
                    
                    var obj = await _targetObject.WorldContext.LoadObject(description.BundleId, bundle, _targetObject,
                        ObjectContext.Shared);

                    obj.LocalPosition = g.transform.localPosition;
                    Destroy(g);
                }
            }
            finally
            {
                _processing = false;
            }
            
        }
        
    }
}