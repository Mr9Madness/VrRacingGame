using UnityEngine;
using System.Collections.Generic;

namespace Game
{
    public class Player : MonoBehaviour
    {
        void Start()
        {
        }

        void Update()
        {
            
        }

        public GUISkin skin;

        float _boxWidth = 150f;
        float _boxHeight = 50f;

        void OnGUI()
        {
            GUI.skin = skin;
            skin.box.fontSize = 10;
            GUI.contentColor = Color.white;

            foreach( KeyValuePair<string, GameObject> carController in Data.Network.Players )
            {
                Vector3 boxPosition =
                    Camera.main.WorldToScreenPoint( carController.Value.transform.position );
                boxPosition.y = Screen.height - boxPosition.y;
                boxPosition.x -= _boxWidth * 0.1f;
                boxPosition.y -= _boxHeight * 0.5f;

                Vector2 content = skin.box.CalcSize( new GUIContent( carController.Key ) );

                GUI.Box(
                    new Rect( boxPosition.x - content.x / 2 * .5f, boxPosition.y + 5f, content.x, content.y ),
                    carController.Key );
            }
        }
    }
}