using System.Collections.Generic;
using System.Linq;
using AssetSystem.Repository;
using MyWorldHub;
using MyWorldHub.Activity;
using VisualScripting.Scripts.Window.Item;

namespace VisualScripting.Scripts.Window
{
    public class WorldListWindow : VRUI.Window
    {

        private readonly List<ButtonItem> _buttonItems = new ();

        private bool _processing = false;

        private long _startTime = 0;

        private const int Limit = 10;

        public override void OnCreate(object[] param)
        {
            DeactivateOnOpen = false;
            CloseOnParentClose = true;
            
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

            var repository = ServiceLocator.Resolve<IBundleDataRepository>();
            try
            {
                // リストを取得
                var data = await repository.GetWorldList(createdTime,start,at,Limit);
                
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
                foreach (var (id,description) in data)
                {
                    var btn = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
                    btn.text = description.Name;
                    btn.onClick.AddListener(()=>OnClick(id));
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

        private async void OnClick(string id)
        {
            if(_processing) return;
            _processing = true;
            await ActivityManager.Singleton.LoadWorld(id);
            _processing = false;
        }

    }
}