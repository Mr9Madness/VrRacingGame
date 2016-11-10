using UnityEngine;
using System.Collections;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        private float _yaw = 0;
        private float _pitch = 0;
        public float MouseSensitivity = 1.5f;

        bool input = false;

        void Start()
        {
        
        }
        
        void FixedUpdate()
        {               
            if( !input )
            {            
                Camera.main.transform.eulerAngles = transform.eulerAngles;
                _yaw = transform.eulerAngles.x;
                _pitch = transform.eulerAngles.y;
            }

            if( input )
                Rotate();

            input = false;

            if( Input.GetAxis( "Right Stick Horizontal" ) != 0f || Input.GetAxis( "Right Stick Vertical" ) != 0f )
                input = true;
            else if( Input.GetMouseButton( 1 ) || Input.GetMouseButton( 1 ) && Input.GetAxis( "Mouse Y" ) != 0f || Input.GetMouseButton( 1 ) && Input.GetAxis( "Mouse X" ) != 0f )
                input = true;
            
        }
        void Rotate()
        {
            _yaw += MouseSensitivity * ( Input.GetAxis( "Right Stick Vertical" ) * MouseSensitivity + ( Input.GetMouseButton( 1 ) ? 
                Input.GetAxis( "Mouse Y" ) * MouseSensitivity * Time.deltaTime : 0 )   );
            
            _pitch -= ( Input.GetAxis( "Right Stick Horizontal" ) * MouseSensitivity + ( Input.GetMouseButton( 1 ) ? 
                Input.GetAxis( "Mouse X" ) * MouseSensitivity * Time.deltaTime : 0 ) );
            
            Camera.main.transform.eulerAngles = new Vector3( _yaw, _pitch );
        }
    }
}