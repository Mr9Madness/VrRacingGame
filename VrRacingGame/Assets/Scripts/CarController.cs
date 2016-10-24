using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts
{
    public class CarController : MonoBehaviour
    {
        /// <summary>
        /// Index to position = 0 Left back, 1 Left front, 2 Right back, 3 Right front
        /// </summary>
        [ SerializeField ] private MeshRenderer[] _wheelMeshes = new MeshRenderer[ 4 ];

        /// <summary>
        /// Index to position = 0 Left back, 1 Left front, 2 Right back, 3 Right front
        /// </summary>
        [SerializeField ] private WheelCollider[] _wheelColliders = new WheelCollider[ 4 ];

        [ Range( 0, 1 ) ] [ SerializeField ] private float _tractionControl;
        [ SerializeField ] private float _fullTorqueOverAllWheels;

        private float _slipLimit = 0.3f;

        public float CurrentSpeed { get { return _rigidbody.velocity.magnitude * 3.6f; } }


        private Rigidbody _rigidbody;
        private float _accelValue;
        private float _brakeValue;
        private float _steeringAngle;
        private float _currentTorque;

        void Start()
        {
            _rigidbody = GetComponent< Rigidbody >();
        }

        void FixedUpdate()
        {
            float horizontal = Input.GetAxis( "Horizontal" );
            float vertical = Input.GetAxis( "Vertical" );
            float handbrake = Input.GetAxis( "Handbrake" );

            Move( horizontal, vertical, handbrake );
        }

        void Move( float steeringValue, float accelaration, float handbrake )
        {
            steeringValue = Mathf.Clamp( steeringValue, -1, 1 );
            _accelValue = Mathf.Clamp( accelaration, 0, 1 );
            _brakeValue = -1 * Mathf.Clamp( accelaration, -1, 0 );
            handbrake = Mathf.Clamp( handbrake, 0, 1 );

            _steeringAngle = steeringValue * 30f;
            _wheelColliders[ 1 ].steerAngle = _steeringAngle;
            _wheelColliders[ 3 ].steerAngle = _steeringAngle;

            float thrustTorque = _accelValue * ( _currentTorque / 4f );

            for ( int i = 0; i < 4; i++ )
            {
                Quaternion rot;
                Vector3 pos;

                _wheelColliders[ i ].GetWorldPose( out pos, out rot );
                _wheelMeshes[ i ].transform.position = pos;
                _wheelMeshes[ i ].transform.rotation = rot * Quaternion.Euler(0, 0, 90 );

                _wheelColliders[ i ].motorTorque = thrustTorque;

                if( CurrentSpeed > 5 && Vector3.Angle( transform.forward, _rigidbody.velocity ) < 50f )
                {
                    _wheelColliders[ i ].brakeTorque = 20000 * _brakeValue;
                }
                else if( _brakeValue > 0 )
                {
                    _wheelColliders[ i ].brakeTorque = 0f;
                    _wheelColliders[ i ].motorTorque = -1000 * _brakeValue;
                }
            }

            if( handbrake > 0f )
            {
                float hbTorque = handbrake * float.MaxValue;

                _wheelColliders[ 0 ].brakeTorque = hbTorque;
                _wheelColliders[ 2 ].brakeTorque = hbTorque;
            }

            _wheelColliders[ 0 ].attachedRigidbody.AddForce( -transform.up * 100f * _wheelColliders[ 0 ].attachedRigidbody.velocity.magnitude );

            WheelHit wheelHit;
            for ( int i = 0; i < 4; i++ )
            {
                _wheelColliders[ i ].GetGroundHit( out wheelHit );
                if ( wheelHit.forwardSlip >= _slipLimit && _currentTorque >= 0 )
                    _currentTorque -= 10 * _tractionControl;
                else
                {
                    _currentTorque += 10 * _tractionControl;
                    if ( _currentTorque > _fullTorqueOverAllWheels )
                        _currentTorque = _fullTorqueOverAllWheels;
                }
            }
        }
    }
}