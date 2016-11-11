using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( LevelEditor.TileMap ) )]
public class LevelGenEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if ( GUILayout.Button( "ReLoad" ) )
        {
            LevelEditor.TileMap map = ( LevelEditor.TileMap )target;
            map.InitLevel();
        }
        if( GUILayout.Button( "Empty" ) )
        {
            LevelEditor.TileMap map = ( LevelEditor.TileMap ) target;
            map.EmptyLevel();
        }
    }
}