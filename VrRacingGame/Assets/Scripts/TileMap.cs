using System;
using UnityEngine;
using System.IO;

public class TileMap : MonoBehaviour
{
    public static TileMapData TileData;

    private string MapName;
    void Start()
    {
        MapName = "map1";
        string json = File.ReadAllText( @"Assets\Data\Map\" + MapName + ".json" );

        TileData = new TileMapData().CreateFromJson( json );

        Debug.Log( TileData.mapHeight );
    }

    void Update()
    {}
}

[ Serializable ]
public struct TileMapData
{
    public string mapWidth;
    public string mapHeight;
    public int[ , ] map;

    public TileMapData CreateFromJson( string json )
    {
        return JsonUtility.FromJson<TileMapData>( json );
    }
}