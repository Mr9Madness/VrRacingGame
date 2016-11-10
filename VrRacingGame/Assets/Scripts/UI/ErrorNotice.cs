using System;
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

    public class ErrorNotice
    {
        private GameObject _parent;
        private GameObject _canvas;
        private GameObject _errorBox;


        public ErrorNotice( GameObject targetObject )
        {
            _parent = targetObject;
            _canvas = GameObject.FindGameObjectWithTag( "Canvas" );
        }

        public void CreateBox(Direction direction)
        {
            // TODO - Create a new object and put it in _parent var
            _errorBox = new GameObject( "Error Box" );
            _errorBox.transform.SetParent( _parent.transform );

            switch( direction )
            {
            case Direction.TopRight:
                
                break;
            }
        }

        public string DisplayText
        {
            get
            {
                if( _parent.GetComponentInChildren< Text >() != null )
                    return _parent.GetComponentInChildren< Text >().text;
                return null;
            }
            set
            {
                if( _parent.GetComponentInChildren< Text >() != null )
                    _parent.GetComponentInChildren< Text >().text = value;
            }
        }
    }
}

