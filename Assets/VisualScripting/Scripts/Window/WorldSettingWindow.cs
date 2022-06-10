using System;
using AssetSystem.Reference;
using AssetSystem.Repository;
using MyWorldHub;
using MyWorldHub.ClientSystem.Auth;
using MyWorldHub.Unity;
using UnityEngine;
using VisualScripting.Scripts.Window.Item;

namespace VisualScripting.Scripts.Window
{
    public class WorldSettingWindow : VRUI.Window
    {
        
        private TextItem _worldIdText;

        private InputTextItem _worldNameText;
        
        private TextItem _authorIdText;

        private ButtonItem _saveButton;

        private TextItem _savingInfoText;
        
        private bool _isSaving = false;

        private string _loadedWorldId;

        private IWorldContext _worldContext;

        public override void OnCreate(object[] param)
        {
            
            DeactivateOnOpen = false;
            CloseOnParentClose = true;

            if (param.Length != 2 ||
                param[0] is not string || 
                param[1] is not IWorldContext)
            {
                Debug.LogError("parameter mismatch");
                return;
            }
            
            _loadedWorldId = param[0].ToString();
            _worldContext = (IWorldContext)param[1];

            _worldIdText = Instantiate(WindowDependency.TextItemPrefab,ScrollContentTransform);
            _worldIdText.ItemName = "WorldId";

            _worldNameText = Instantiate(WindowDependency.InputTextItemPrefab,ScrollContentTransform);
            _worldNameText.ItemName = "Name";

            _authorIdText = Instantiate(WindowDependency.TextItemPrefab,ScrollContentTransform);
            _authorIdText.ItemName = "AuthorId";

            _saveButton = Instantiate(WindowDependency.ButtonItemPrefab,ScrollContentTransform);
            _saveButton.text = "Save";
            _saveButton.onClick.AddListener(()=>Save(false));

            _saveButton = Instantiate(WindowDependency.ButtonItemPrefab,ScrollContentTransform);
            _saveButton.text = "Save as new world";
            _saveButton.onClick.AddListener(()=>Save(true));

            _savingInfoText = Instantiate(WindowDependency.TextItemPrefab,ScrollContentTransform);
            _savingInfoText.ItemName = "Status";
            _savingInfoText.text = "";
            
            Refresh();
        }

        private async void Refresh()
        {
            _worldIdText.text = _loadedWorldId;

            var repository = ServiceLocator.Resolve<IBundleDataRepository>();
            var (result,description) = await repository.GetWorldDescription(_loadedWorldId);

            if (!result)
            {
                Fail();
                return;
            }
            
            _worldNameText.text = description.Name;
            _authorIdText.text = description.AuthorId;
        }

        private async void Save(bool asNew)
        {
            var auth = ServiceLocator.Resolve<IAuthenticationService>();
            if (_isSaving || !auth.IsLoggedIn) return;

            _isSaving = true;
            Physics.autoSimulation = false;
            try
            {
                _savingInfoText.gameObject.SetActive(true);
                _savingInfoText.text = "Get latest description...";

                // 最新のデータを取得する
                var dataRepository = ServiceLocator.Resolve<IBundleDataRepository>();
                var converter = ServiceLocator.Resolve<IReferenceConverter>();
                var (result,description) = await dataRepository.GetWorldDescription(_loadedWorldId);
                if (!result)
                {
                    _savingInfoText.text = "Failed to get description";
                    return;
                }
                
                // bundleを生成
                _savingInfoText.text = "Convert IManagedObject to IMasterBundle...";
                var bundle = await converter.CreateMasterBundleAsync(_worldContext.GetRootObject());
                if (bundle == null)
                {
                    _savingInfoText.text = "Failed to create bundle";
                    return;
                }
                
                // bundleを保存
                _savingInfoText.text = "Saving worldBundle...";
                var bundleId = Guid.NewGuid().ToString();
                if (!await dataRepository.SaveBundle(bundleId, bundle))
                {
                    _savingInfoText.text = "Failed to save worldBundle";
                    return;
                }

                if (asNew)
                {
                    _loadedWorldId = Guid.NewGuid().ToString();
                    _worldContext.SetWorldId(_loadedWorldId);
                }

                // descriptionを保存する
                _savingInfoText.text = "Saving description...";
                var newDesc = await converter.CreateBundleDescription(_worldNameText.text, bundleId,
                    description.AuthorId, description.CreatedTime, description.UpdatedTime);

                if (await dataRepository.SaveWorldDescription(_loadedWorldId,newDesc))
                {
                    _savingInfoText.text = "Saved";
                    // 表示を更新
                    Refresh();
                }
                else
                {
                    _savingInfoText.text = "Failed to save";
                }
            }
            finally
            {
                _isSaving = false;
                Physics.autoSimulation = true;
            }
        }

        private void Fail()
        {
            _worldIdText.text = "Failed to load world data.";
            _worldNameText.text = "";
        }

    }
}