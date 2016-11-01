using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( TileMap ) )]
public class LevelGen : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if ( GUILayout.Button( "ReLoad" ) )
        {
            TileMap map = ( TileMap )target;
            map.InitLevel();
        }
        if( GUILayout.Button( "Empty" ) )
        {
            TileMap map = ( TileMap ) target;
            map.EmptyLevel();
        }
    }
}
