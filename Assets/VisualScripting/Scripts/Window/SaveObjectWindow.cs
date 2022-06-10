using System;
using AssetSystem.Reference;
using AssetSystem.Repository;
using AssetSystem.Unity;
using MyWorldHub;
using MyWorldHub.ClientSystem.Auth;
using UnityEngine;
using VisualScripting.Scripts.Window.Item;

namespace VisualScripting.Scripts.Window
{
    public class SaveObjectWindow : VRUI.Window
    {

        private IManagedObject _targetObject;

        private bool _isSaving = false;

        private InputTextItem _itemNameText;

        private HeaderItem _statusText;

        public override void OnCreate(object[] parameters)
        {
            if (parameters.Length != 1 ||
                parameters[0] is not IManagedObject)
            {
                Debug.LogError("parameter mismatch");
                return;
            }

            _targetObject = (IManagedObject) parameters[0];


            var title = Instantiate(WindowDependency.HeaderItemPrefab,ScrollContentTransform);
            title.text = "Save this object";

            _itemNameText = Instantiate(WindowDependency.InputTextItemPrefab,ScrollContentTransform);
            _itemNameText.ItemName = "Name";
            _itemNameText.text = "";

            var idText = Instantiate(WindowDependency.TextItemPrefab,ScrollContentTransform);
            idText.ItemName = "ItemId";
            idText.text = _targetObject.UniqueId;

            var saveButton = Instantiate(WindowDependency.ButtonItemPrefab,ScrollContentTransform);
            saveButton.text = "Save";
            saveButton.onClick.AddListener(Save);

            _statusText = Instantiate(WindowDependency.HeaderItemPrefab,ScrollContentTransform);
            _statusText.text = "";
        }

        private async void Save()
        {
            
            var auth = ServiceLocator.Resolve<IAuthenticationService>();
            if (!auth.IsLoggedIn)
            {
                _statusText.text = "You are not logged in.";
                return;
            }
            
            if (_isSaving)
                return;

            if (_itemNameText.text.Length < 1)
            {
                _statusText.text = "Param[Name] error";
                return;
            }

            Physics.autoSimulation = false;
            _isSaving = true;
            try
            {
                var dataRepository = ServiceLocator.Resolve<IBundleDataRepository>();
                var converter = ServiceLocator.Resolve<IReferenceConverter>();
                
                // convert bundle
                _statusText.text = "Converting object to bundle";
                var bundle = await converter.CreateMasterBundleAsync(_targetObject);
                if (bundle == null)
                {
                    _statusText.text = "Failed to create bundle.";
                    return;
                }

                // save bundle
                _statusText.text = "Saving bundle";
                var bundleId = Guid.NewGuid().ToString();
                if (!await dataRepository.SaveBundle(bundleId, bundle))
                {
                    _statusText.text = "Failed to save bundle.";
                    return;
                }

                // get latest description
                _statusText.text = "Get latest description";
                
                var description = await converter.CreateBundleDescription(_itemNameText.text, bundleId, "", DateTime.Now.ToUnixTime(), DateTime.Now.ToUnixTime());

                // save description
                _statusText.text = "Saving description";
                if (await dataRepository.SaveObjectDescription(Guid.NewGuid().ToString(), description))
                {
                    _statusText.text = "Saved";
                }
                else
                {
                    _statusText.text = "Failed to save description.";
                }
            }
            finally
            {
                Physics.autoSimulation = true;
                _isSaving = false;
            }
        }
    }
}