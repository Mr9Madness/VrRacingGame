using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( Game.TileMap ) )]
public class LevelGen : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if ( GUILayout.Button( "ReLoad" ) )
        {
            Game.TileMap map = ( Game.TileMap )target;
            map.InitLevel();
        }
        if( GUILayout.Button( "Empty" ) )
        {
            Game.TileMap map = ( Game.TileMap ) target;
            map.EmptyLevel();
        }
    }
}
