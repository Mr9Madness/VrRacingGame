using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum Direction
    {
        TopRight,
        BottomRight,
        BottomLeft,
        TopLeft
    }

    public class ErrorNotice : MonoBehaviour
    {
        public RectTransform TargetPos;
        private GameObject _canvas;

        private GameObject _errorBox;
        private Image _errorPanel;
        private Text _errorText;

        public void CreateBox( string message, float displayTime = 500f, Direction direction = Direction.TopRight )
        {            
            _canvas = GameObject.Find( "Canvas" ).transform.FindChild( "Notices" ).gameObject;
            
            // TODO - Create a new object and put it in _parent var
            _errorBox = new GameObject( "Error Box" );
            _errorBox.transform.SetParent( _canvas.transform );
            _errorBox.transform.position = _canvas.transform.position;

            _errorPanel = _errorBox.AddComponent< Image >() as Image;
            _errorPanel.rectTransform.sizeDelta = new Vector2( 200, 50 );
            _errorPanel.rectTransform.localScale = Vector3.one;
            _errorBox.SetActive( TargetPos.gameObject.activeSelf );

            _errorText = new GameObject("Error Text").AddComponent< Text >() as Text;
            _errorText.transform.position = _errorBox.transform.position;
            _errorText.transform.SetParent( _errorBox.transform );
            _errorText.font = Resources.GetBuiltinResource< Font >( "Arial.ttf" );
            _errorText.rectTransform.localScale = Vector3.one;
            _errorText.rectTransform.sizeDelta = _errorPanel.rectTransform.sizeDelta;
            _errorText.resizeTextForBestFit = true;
            _errorText.color = Color.black;

            _errorText.alignment = TextAnchor.MiddleCenter;

            switch( direction )
            {
                case Direction.TopRight:
                    _errorBox.transform.localPosition = new Vector3( TargetPos.localPosition.x + ( _errorPanel.rectTransform.sizeDelta.x / 2 ), 
                        TargetPos.localPosition.y + _errorPanel.rectTransform.sizeDelta.y * 2 );
                break;
            }

            Show( message, displayTime );
        }

        public void Show( string message, float displayTime = 500f )
        {
            StartCoroutine( FadeAlpha( displayTime ) );
            DisplayText = message;
        }

        IEnumerator FadeAlpha( float displayTime )
        {
            _errorBox.SetActive( true );

            Color resetColor = _errorText.color;
            resetColor.a = 1;
            _errorText.color = resetColor;

            yield return new WaitForSeconds( displayTime );
        }

        public string DisplayText
        {
            get
            {
                if( _errorText.GetComponent< Text >() != null )
                    return _errorText.GetComponent< Text >().text;
                return null;
            }
            set
            {
                if( _errorText.GetComponent< Text >() != null )
                    _errorText.GetComponent< Text >().text = value;
            }
        }
    }
}

