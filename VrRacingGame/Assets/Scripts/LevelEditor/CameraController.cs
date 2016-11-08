using UnityEngine;
using System.Collections;

namespace LevelEditor
{
	public class CameraController : MonoBehaviour
	{
		public float Speed = 500f;
		public float MouseSensitivity = 1.5f;
		private Rigidbody _rigidbody;

		private float _yaw = 45;
		private float _pitch = 0;

		void Start()
		{
			_rigidbody = GetComponent< Rigidbody >();
		}

		void FixedUpdate()
		{
			float horizontal = Input.GetAxis( "Horizontal" );
			float vertical = Input.GetAxis( "Vertical" );

			if( Input.GetMouseButton( 1 ) )
				Rotate();

            if( Input.GetMouseButton( 0 ) )
            {
                Ray mouseRay = Camera.main.ScreenPointToRay( Input.mousePosition );
                RaycastHit mouseRayHit;

                if( Physics.Raycast( mouseRay, out mouseRayHit, Mathf.Infinity ) )
                {
                    Vector3 mousePos = new Vector3( Mathf.FloorToInt( mouseRayHit.point.x ) / 32, 0, Mathf.FloorToInt( mouseRayHit.point.y ) / 32 );
                    Debug.Log( mousePos );
                    Debug.Log( mouseRayHit.point );
                }
            }

			Move( horizontal * Speed, vertical * Speed );
		}

		void Move( float horizontal, float vertical )
		{
            Vector3 horizontalPos = transform.rotation * new Vector3( horizontal, 0, vertical );
            _rigidbody.velocity = horizontalPos * Time.deltaTime;
		}

		void Rotate()
		{
			_yaw += MouseSensitivity * Input.GetAxis( "Mouse Y" ) * Time.deltaTime;
			_pitch -= MouseSensitivity * Input.GetAxis( "Mouse X" ) * Time.deltaTime;

            transform.eulerAngles = new Vector3( transform.eulerAngles.x, _pitch );
            Camera.main.transform.eulerAngles = new Vector3( _yaw, transform.eulerAngles.y );
		}
	}
}