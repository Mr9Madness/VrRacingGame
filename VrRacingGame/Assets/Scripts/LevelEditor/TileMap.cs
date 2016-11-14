using UnityEngine;
using System;

namespace LevelEditor
{
	public class TileMap : MonoBehaviour
	{
        public static Data.Level _tileData;

		public GameObject[] Trackparts;
		public Transform TrackContainer;

		public string MapName;

        void Start()
        {
            _tileData = new Data.Level();
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
			if( TrackContainer.childCount <= 0 ) return;

			while( TrackContainer.childCount > 0 )
				foreach( Transform child in TrackContainer )
					DestroyImmediate( child.gameObject );

		}

        public void CreateJsonFile()
        {
            Data.Level.ToJsonFile( "test", _tileData );
        }
	}
}