using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        public EventSystem EventSystem;
        private GameObject _lastSelectedObject;

        public Dropdown WindowSizeDropdown;

        public RectTransform Dingen;

        public static UI.ErrorNotice ConnectError;

        void Start()
        {
            ConnectError = Dingen.gameObject.AddComponent< UI.ErrorNotice >() as UI.ErrorNotice;
            ConnectError.TargetPos = Dingen;

            Resolution[] supportedResolutions = Screen.resolutions;

            WindowSizeDropdown.options.Clear();

            foreach( Resolution supportedResolution in supportedResolutions )
                WindowSizeDropdown.options.Add( new Dropdown.OptionData( supportedResolution.ToString() ) );

            WindowSizeDropdown.value = WindowSizeDropdown.options.Count;
        }

        void Update()
        {
            if ( EventSystem.currentSelectedGameObject != null )
                _lastSelectedObject = EventSystem.currentSelectedGameObject;

            if ( Input.GetAxisRaw( "MenuVertical" ) != 0f && EventSystem.currentSelectedGameObject == null )
            {
                ChangeButton( _lastSelectedObject );
            }
        }

        public void ChangeButton( GameObject button )
        {
            EventSystem.SetSelectedGameObject( button );
            _lastSelectedObject = button;
        }
    }
}