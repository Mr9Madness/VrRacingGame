using System;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    private LevelData _tileData;
    public GameObject[] Trackparts;

    public string MapName;

    void Start()
    {
        MapName = "level1";

        _tileData = LevelData.CreateFromJson( MapName );
        
        InitLevel();
    }

    void InitLevel()
    {
        for ( int x = 0; x <= _tileData.LevelWidth; x++ )
        {
            for ( int y = 0; y <= _tileData.LevelHeight; y++ )
            {
                int tileValue = _tileData.GetTile( x, y );
                Instantiate( Trackparts[ tileValue ], new Vector3( x * 31.5f, 0, y * 31.5f ), Quaternion.identity );
            }
        }
    }

}