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
		private float _yaw = 0;
		private float _pitch = 0;

        bool isPaused;

		void Start()
		{
			_rigidbody = GetComponent< Rigidbody >();
		}

		void Update()
        {        
            if( Input.GetKeyDown( KeyCode.Escape ) ) 
                PauseMenu.SetActive( PauseMenu.activeSelf ? false : true );

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
                    Vector3 mousePos = new Vector3( Mathf.FloorToInt( mouseRayHit.point.x ) / 32, 0, Mathf.FloorToInt( mouseRayHit.point.z ) / 32 );
                    ObjectSpawn.SpawnObject( mousePos, 0 );
                    LevelEditor.TileMap._tileData.SetTile( Mathf.RoundToInt( mousePos.x ),  Mathf.RoundToInt( mousePos.y ), 0 );
                    Debug.Log( mousePos );
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
			_yaw += MouseSensitivity * ( Input.GetAxis( "Right Stick Vertical" ) * MouseSensitivity + ( Input.GetMouseButton( 1 ) ? 
				Input.GetAxis( "Mouse Y" ) * MouseSensitivity * Time.deltaTime : 0 )   );

			_pitch -= ( Input.GetAxis( "Right Stick Horizontal" ) * MouseSensitivity + ( Input.GetMouseButton( 1 ) ? 
				Input.GetAxis( "Mouse X" ) * MouseSensitivity * Time.deltaTime : 0 ) );

			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, _pitch);
			Camera.main.transform.eulerAngles = new Vector3 (_yaw, transform.eulerAngles.y);
		}
	}
}