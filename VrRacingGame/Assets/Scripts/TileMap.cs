using System;
using UnityEngine;

namespace Game
{
    public class TileMap : MonoBehaviour
    {
        private Data.Level _tileData;

        public GameObject[] Trackparts;
        public Transform TrackContainer;

        public static string MapName = "straight";

        void Start()
        {        
            InitLevel();
        }

        public void InitLevel()
        {
            _tileData = Data.Level.CreateFromJson( MapName );

            if ( TrackContainer == null )
                TrackContainer = new GameObject( "Track" ).transform;

            EmptyLevel();

            if( _tileData == null ) return;

            for( int x = 0; x < _tileData.LevelWidth; x++ )
            {
                for ( int y = 0; y < _tileData.LevelHeight; y++ )
                {
                    int tileValue = _tileData.GetTile( x, y );
                    if( tileValue < 0 )
                        return;
                    
                    Instantiate( Trackparts[ tileValue ], new Vector3( x * 31.5f, 0, y * 31.5f ), Quaternion.Euler( 0, ( tileValue - 1 ) * 90, 0 ), TrackContainer );
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
    }
}