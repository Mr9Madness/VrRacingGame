using UnityEngine;
using System.Collections;

namespace Game
{
    public class PlayerController : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            Instantiate( Resources.Load( "Car Audi R8" ), new Vector3( 0, 1, 0 ), Quaternion.identity );
            AddPlayer( "iemand" );
            AddPlayer( "iemanda" );
            AddPlayer( "iemands" );

        }

        // Update is called once per frame
        void Update()
        {}

        public static void AddPlayer( string name )
        {
            GameObject otherplayer =
                Instantiate( Resources.Load( "Car Audi R8 nonPlayer" ), new Vector3( 5, 1, 15 * Data.Network.Players.Count ), Quaternion.identity )
                    as
                    GameObject;
            Data.Network.Players.Add( name, otherplayer );
        }
    }
}