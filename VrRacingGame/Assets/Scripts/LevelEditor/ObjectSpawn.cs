using UnityEngine;
using System.Collections;

namespace LevelEditor
{
    public class ObjectSpawn : MonoBehaviour
    {
        static GameObject[] Objects;
        static Transform parent;

        void Start()
        {
            parent = ( FindObjectOfType( typeof( LevelEditor.TileMap ) ) as LevelEditor.TileMap ).TrackContainer;
            Objects = ( FindObjectOfType( typeof( LevelEditor.TileMap ) ) as LevelEditor.TileMap ).Trackparts;

        }
        
        void Update()
        {
            
        }

        public static void SpawnObject( Vector3 pos, int tileValue )
        {
            Instantiate( Objects[ tileValue ], pos * 32, Quaternion.identity, parent );
        }
    }
}