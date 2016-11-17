using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.IO;

namespace LevelEditor
{
	public class TileMap : MonoBehaviour
	{
        public static Data.Level _tileData;

		public GameObject[] Trackparts;
		public Transform TrackContainer;

        public GameObject PlaceArea;

        [NonSerialized] public string LevelName;

        void Start()
        {
            _tileData = new Data.Level();


            LevelName = _tileData.LevelName;

            InitLevel();
        }

		public void InitLevel()
        {
			if ( TrackContainer == null )
				TrackContainer = new GameObject( "Track" ).transform;
			EmptyLevel();
        }

		public void EmptyLevel()
		{
			if( TrackContainer == null ) return;

			while( TrackContainer.childCount > 0 )
				foreach( Transform child in TrackContainer )
					DestroyImmediate( child.gameObject );

		}

        public void ChangeValue( string levelName = "" )
        {
            if( levelName != "" )
            {
                _tileData.LevelName = levelName;
            }
        }

        public void CreateJsonFile()
        {
            Data.Level.ToJsonFile( _tileData );
        }
	}
}