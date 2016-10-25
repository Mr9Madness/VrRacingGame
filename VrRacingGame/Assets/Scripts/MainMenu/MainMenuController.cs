using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        public EventSystem EventSystem;
        public GameObject SelectedObject;

        public Dropdown WindowSizeDropdown;

        private bool _buttonSelected;

        void Start()
        {
            EventSystem.SetSelectedGameObject( SelectedObject );

            Resolution[] supportedResolutions = Screen.resolutions;

            WindowSizeDropdown.options.Clear();

            foreach( Resolution supportedResolution in supportedResolutions )
                WindowSizeDropdown.options.Add( new Dropdown.OptionData( supportedResolution.ToString() ) );

            WindowSizeDropdown.value = WindowSizeDropdown.options.Count;
        }

        void Update()
        {
            if( Input.GetAxisRaw( "Vertical" ) != 0f || _buttonSelected == false )
            {
                EventSystem.SetSelectedGameObject( SelectedObject );
                _buttonSelected = true;
            }
        }

        public void ChangeButton( GameObject button )
        {
            EventSystem.SetSelectedGameObject( button );
        }

        private void OnDisable()
        {
            _buttonSelected = false;
        }
    }
}