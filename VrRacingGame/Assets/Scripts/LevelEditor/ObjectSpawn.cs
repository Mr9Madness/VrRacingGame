using UnityEngine;
using System.Collections;

public class ObjectSpawn : MonoBehaviour
{
    GameObject[] Objects;
    Transform parent;

    void Start()
    {
        //Objects = ( FindObjectOfType( typeof( LevelEditor.TileMap ) ) as LevelEditor.TileMap ).Trackparts;
        //parent = ( FindObjectOfType( typeof( LevelEditor.TileMap ) ) as LevelEditor.TileMap ).TrackContainer;
    }
    
    void Update()
    {
        
    }

    public static void SpawnObject( Vector3 pos, int TileValue )
    {
        //Instantiate( Objects[ TileValue ], pos, parent );
    }
}