using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuFunctions : MonoBehaviour
{
    public void InitScene( int sceneIndex )
    {
        SceneManager.LoadScene( sceneIndex );
    }
    public void InitScene( string sceneName )
    {
        SceneManager.LoadScene( sceneName );
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Change the preset Quality level
    /// </summary>
    public void ChangeQualityLevel()
    {
        if ( PlayerPrefs.GetInt( "QualityLevel", -1 ) != -1 )
        {
            QualitySettings.SetQualityLevel( PlayerPrefs.GetInt( "QualityLevel" ) );
            QualitySettings.vSyncCount = PlayerPrefs.GetInt( "VSync", 0 );
            GetComponent< Dropdown >().value = PlayerPrefs.GetInt( "QualityLevel" );
            return;
        }

        int value = GetComponent< Dropdown >().value;
        QualitySettings.SetQualityLevel( value );
        QualitySettings.vSyncCount = PlayerPrefs.GetInt( "VSync", 0 );
        PlayerPrefs.SetInt( "QualityLevel", value );
    }

    /// <summary>
    /// Turn off or on V-Sync
    /// </summary>
    public void ChangeV_Sync()
    {
        if ( PlayerPrefs.GetInt( "VSync", -1 ) != -1 )
        {
            QualitySettings.vSyncCount = PlayerPrefs.GetInt( "VSync" );
            GetComponent< Toggle >().isOn = PlayerPrefs.GetInt( "VSync" ) == 1;
        }

        int value = GetComponent< Toggle >().isOn ? 1 : 0;
        QualitySettings.vSyncCount = value;
        PlayerPrefs.SetInt( "VSync", value );
    }

    public void ChangeResolution()
    {
        Dropdown dropdown = GetComponent< Dropdown >();
        Resolution[] supportedResolutions = Screen.resolutions;

        Screen.SetResolution( supportedResolutions[ dropdown.value ].width,
            supportedResolutions[ dropdown.value ].height,
            PlayerPrefs.GetInt( "Fullscreen", 0 ) == 1 );

    }

    public void ChangeWindowType()
    {
        bool value = GetComponent< Toggle >().isOn;
        Screen.SetResolution( Screen.currentResolution.width, Screen.currentResolution.height, value );
        PlayerPrefs.SetInt( "Fullscreen", value ? 1 : 0 );
    }

    /// <summary>
    /// Sets volume of the specified audio setting
    /// </summary>
    /// <param name="audioName">Name of the audio setting</param>
    public void SetVolume( string audioName )
    {
        int value = Mathf.RoundToInt( GetComponent< Slider >().value );
        PlayerPrefs.SetInt( audioName, value );
    }
}