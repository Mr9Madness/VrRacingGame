using UnityEngine;
using System.Collections;

namespace LevelEditor
{
	public class CameraController : MonoBehaviour
	{
		public float Speed = 500f;
		public float MouseSensitivity = 1.5f;
        public GameObject PauseMenu;

		private Rigidbody _rigidbody;
		private float _yaw = 45;
		private float _pitch = 0;

        bool isPaused;

		void Start()
		{
			_rigidbody = GetComponent< Rigidbody >();
		}

		void Update()
        {        
            if( Input.GetKeyDown( KeyCode.Escape ) ) 
                PauseMenu.SetActive( !PauseMenu.activeSelf );

            if( PauseMenu.activeSelf )
            {
                isPaused = true;
                PauseMenu.SetActive( isPaused );
                Time.timeScale = 0f;
            }
            else
            {
                isPaused = false;
                PauseMenu.SetActive( isPaused );
                Time.timeScale = 1f;
            }
            if( isPaused ) return;

			float horizontal = Input.GetAxis( "Horizontal" ) * Speed;
			float vertical = Input.GetAxis( "Vertical" ) * Speed;

            Move( horizontal, vertical );

			if( Input.GetMouseButton( 1 ) )
				Rotate();

            if( Input.GetAxis( "Scroll" ) != 0f )
            {

                ObjectSpawn.ChangeSelection(
                    Mathf.RoundToInt( Mathf.Clamp( Input.GetAxis( "Scroll" ), -1, 1 ) ) );
            }

            if( Input.GetMouseButton( 0 ) )
            {
                Ray mouseRay = Camera.main.ScreenPointToRay( Input.mousePosition );
                RaycastHit mouseRayHit;

                if( Physics.Raycast( mouseRay, out mouseRayHit, Mathf.Infinity ) )
                {
                    ObjectSpawn.SpawnObject( mouseRayHit );
                }
            }
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