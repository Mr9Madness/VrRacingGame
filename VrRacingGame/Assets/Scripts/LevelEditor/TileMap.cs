using UnityEngine;
using System;

namespace LevelEditor
{
	public class TileMap : MonoBehaviour
	{
        private Data.LevelData _tileData;

		public GameObject[] Trackparts;
		public Transform TrackContainer;

		public string MapName;

        void Start()
        {
            _tileData = new Data.LevelData();
        }

		public void InitLevel()
		{
			_tileData = Data.LevelData.CreateFromJson( MapName );

			if ( TrackContainer == null )
				TrackContainer = new GameObject( "Track" ).transform;

			EmptyLevel();

			for( int x = 0; x < _tileData.LevelWidth; x++ )
			{
				for ( int y = 0; y < _tileData.LevelHeight; y++ )
				{
					int tileValue = _tileData.GetTile( x, y );
					Instantiate( Trackparts[ tileValue ], new Vector3( x * 31.5f, 0, y * 31.5f ), Quaternion.Euler( 0, tileValue * 90, 0 ), TrackContainer );
				}
			}
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
            Data.LevelData.ToJsonFile( "test", _tileData );
        }
	}
}