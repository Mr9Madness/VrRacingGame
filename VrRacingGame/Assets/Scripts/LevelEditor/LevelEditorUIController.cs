using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.UI;

namespace LevelEditor
{
    public class LevelEditorUIController : MonoBehaviour 
    {
        public InputField Name;

        private TileMap _levelData;

    	void Start ()
        {
            _levelData = FindObjectOfType< LevelEditor.TileMap >() as LevelEditor.TileMap;
            Name.text = _levelData.LevelName;
    	}

        public void ChangeName()
        {
            _levelData.ChangeValue( Name.text );
        }
    }
}