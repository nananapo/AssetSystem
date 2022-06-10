using MyWorldHub;
using MyWorldHub.ClientSystem.Auth;
using MyWorldHub.Shared;
using VisualScripting.Scripts.Window.Item;

namespace VisualScripting.Scripts.Window
{
    public class UserProfileWindow : VRUI.Window
    {

        private IAuthenticationService _authenticationService;

        private InputTextItem _nameInputText;

        private HeaderItem _statusText;

        private bool _isSaving = false;

        public override void OnCreate(object[] parameters)
        {
            DeactivateOnOpen = false;
            CloseOnParentClose = true;

            _authenticationService = ServiceLocator.Resolve<IAuthenticationService>();

            // UID
            var uidText = Instantiate(WindowDependency.TextItemPrefab, ScrollContentTransform);
            uidText.text = _authenticationService.Id;
            Monitor(()=>_authenticationService.Id,uidText.SetText,this);

            // 名前
            _nameInputText = Instantiate(WindowDependency.InputTextItemPrefab, ScrollContentTransform);

            // Save Button
            var saveButton = Instantiate(WindowDependency.ButtonItemPrefab, ScrollContentTransform);
            saveButton.text = "Save";
            saveButton.onClick.AddListener(Save);

            // status text
            _statusText = Instantiate(WindowDependency.HeaderItemPrefab, ScrollContentTransform);
            _statusText.text = "";
            
            // 表示を更新
            Refresh();
        }

        private async void Save()
        {
            if (_isSaving || !_authenticationService.IsLoggedIn) return;

            _isSaving = true;
            try
            {
                
                _statusText.gameObject.SetActive(true);
                _statusText.text = "Saving...";
                
                // 最新のデータを取得する
                var dataRepository = ServiceLocator.Resolve<IPlayerDataRepository>();
                var playerData = await dataRepository.GetPlayer(_authenticationService.Id);
                if (playerData == null)
                {
                    _statusText.text = "Failed to save";
                    return;
                }

                // 保存する
                playerData.Name = _nameInputText.text;
                    
                if (await dataRepository.SavePlayerData(playerData))
                {
                    _statusText.text = "Saved";
                    // 表示を更新
                    Refresh();
                }
                else
                {
                    _statusText.text = "Failed to save";
                }
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async void Refresh()
        {
            var dataRepository = ServiceLocator.Resolve<IPlayerDataRepository>();
            
            // Name
            if (_authenticationService.IsLoggedIn)
            {
                var playerData = await dataRepository.GetPlayer(_authenticationService.Id);
                _nameInputText.SetTextWithoutNotify(playerData != null ? playerData.Name : "Failed to get data");
            }
            else
            {
                _nameInputText.SetTextWithoutNotify("Not Logged in");
            }
        }
    }
}