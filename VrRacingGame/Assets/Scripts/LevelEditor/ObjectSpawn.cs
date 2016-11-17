using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LevelEditor
{
    public class ObjectSpawn : MonoBehaviour
    {
        static GameObject[] Objects;
        static Transform parent;

        public static int CurrentSelected = 0;

        void Start()
        {
            parent = ( FindObjectOfType( typeof( LevelEditor.TileMap ) ) as LevelEditor.TileMap ).TrackContainer;
            Objects = ( FindObjectOfType( typeof( LevelEditor.TileMap ) ) as LevelEditor.TileMap ).Trackparts;

        }
        
        /// <summary>
        /// Changes the Current Selected object in the level editor
        /// </summary>
        /// <param name="value">value can be either +1 or -1</param>
        public static void ChangeSelection( int value )
        {
            if( CurrentSelected >= Objects.Length - 1 && value > 0 )
            {
                CurrentSelected = 0;
                return;
            }

            if( CurrentSelected <= Objects.Length - 1 && value < 0 )
            {
                CurrentSelected = Objects.Length - 1;
                return;
            }

            CurrentSelected += value;

            Debug.Log( CurrentSelected );
        }

        public static void SpawnObject( RaycastHit mouseRaycastHit )
        {
            int x = Mathf.FloorToInt( mouseRaycastHit.point.x ) / 32;
            int z = Mathf.FloorToInt( mouseRaycastHit.point.z ) / 32;

            if( mouseRaycastHit.collider.tag == "PlaceArea" )
            {
                Instantiate( Objects[ CurrentSelected ], new Vector3( x * 32, 2, z * 32 ), Quaternion.Euler( 0, ( CurrentSelected - 1 ) * 90, 0 ), parent );
            }
            else
            {
                Destroy( mouseRaycastHit.collider.gameObject );
                Instantiate( Objects[ CurrentSelected ], new Vector3( x * 32, 2, z * 32 ), Quaternion.Euler( 0, ( CurrentSelected - 1 ) * 90, 0 ), parent );

            }
            TileMap._tileData.SetTile( Mathf.FloorToInt( mouseRaycastHit.point.x / 32 ), Mathf.FloorToInt( mouseRaycastHit.point.z / 32 ), CurrentSelected );

        }
    }
}