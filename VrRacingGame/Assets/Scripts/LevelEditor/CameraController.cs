using UnityEngine;
using System.Collections;

namespace LevelEditor
{
	public class CameraController : MonoBehaviour
	{
		public float Speed = 500f;
		public float MouseSensitivity = 1.5f;
		private Rigidbody _rigidbody;


		private float _yaw = 0;
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

			Move( horizontal * Speed, vertical * Speed );
		}

		void Move( float horizontal, float vertical )
		{
			_rigidbody.velocity = new Vector3( horizontal, 0, vertical ) * Time.deltaTime;
		}

		void Rotate()
		{
			_yaw += MouseSensitivity * Input.GetAxis( "Mouse Y" ) * Time.deltaTime;
			_pitch -= MouseSensitivity * Input.GetAxis( "Mouse X" ) * Time.deltaTime;

			transform.eulerAngles = new Vector3( _yaw, _pitch );
		}
	}
}