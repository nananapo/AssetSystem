using MyWorldHub.Activity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace VisualScripting.Scripts.Window
{
    public class MyMenuButton : MonoBehaviour
    {
        
        [SerializeField] private VRUI.Window fromWindow;
        [SerializeField] private Button worldSettingButton;
        [SerializeField] private Button hierarchyButton;
        [SerializeField] private Button userProfileButton;
        [SerializeField] private Button worldListWindow;

        private void Start()
        {
            worldSettingButton.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, fromWindow.transform.parent);
                var window = obj.AddComponent<WorldSettingWindow>();
                
                var context = ActivityManager.Singleton.LoadedWorldContext;
                fromWindow.OpenObject(window,context.GetWorldId(),context);
            });
            
            hierarchyButton.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, fromWindow.transform.parent);
                var window = obj.AddComponent<HierarchyWindow>();
                
                var context = ActivityManager.Singleton.LoadedWorldContext;
                fromWindow.OpenObject(window,context.GetRootObject(),typeof(InspectorWindow));
            });
            
            userProfileButton.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, fromWindow.transform.parent);
                var window = obj.AddComponent<UserProfileWindow>();
                fromWindow.OpenObject(window);
            });
            
            worldListWindow.onClick.AddListener(() =>
            {
                var obj = Instantiate(WindowDependency.EmptyWindowPrefab, fromWindow.transform.parent);
                var window = obj.AddComponent<WorldListWindow>();
                fromWindow.OpenObject(window);
            });
        }
    }
}