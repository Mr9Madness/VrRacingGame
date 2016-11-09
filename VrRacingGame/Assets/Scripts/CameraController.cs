using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        private float _yaw = 0;
        private float _pitch = 0;
        public float MouseSensitivity = 1.5f;

        Vector3 oldRotation;

        bool input = false;

        void Start()
        {
        
        }
        
        void FixedUpdate()
        {
            if( Input.GetAxis( "Right Stick Horizontal" ) != 0f || Input.GetAxis( "Right Stick Vertical" ) != 0f )
                input = true;
            else if( Input.GetMouseButton( 1 ) && Input.GetAxis( "Mouse Y" ) != 0f || Input.GetAxis( "Mouse X" ) != 0f )
                input = true;



            if( !input ) 
            {
                oldRotation = transform.eulerAngles;
                Camera.main.transform.eulerAngles = oldRotation;
            }

            Rotate();
        }
        void Rotate()
        {
            Debug.Log( Input.GetAxis( "Right Stick Vertical" ) );
            Debug.Log( Input.GetAxis( "Right Stick Horizontal" ) );

//            if( controllerInput ) {
            _yaw += MouseSensitivity * Input.GetAxis( "Right Stick Vertical" );
            _pitch -= MouseSensitivity * Input.GetAxis( "Right Stick Horizontal" );
//            } 
//            else if( !controllerInput )
//            {
//                if( !Input.GetMouseButton( 1 ) ) return; 
//
//                _yaw += MouseSensitivity * Input.GetAxis( "Mouse Y" ) * Time.deltaTime;
//                _pitch -= MouseSensitivity * Input.GetAxis( "Mouse X" ) * Time.deltaTime;
//            }            

            Camera.main.transform.eulerAngles = new Vector3( _yaw, _pitch );
        }
    }
}