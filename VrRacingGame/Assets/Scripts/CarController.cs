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

        [ SerializeField ] private float _fullTorqueOverAllWheels;
        [ SerializeField ] private float _brakeTorque;
        [ SerializeField ] private float _reverseTorque;

        public float CurrentSpeed { get { return _rigidbody.velocity.magnitude * 3.6f; } }

        private enum DriveType
        {
            RearWheelDrive,
            FrontWheelDrive,
            FourWheelDrive
        }

        private Quaternion[] _wheelMeshLocalRotation;
        private Rigidbody _rigidbody;
        private float _slipLimit = 0.3f;
        private float _tractionControl = 1;
        private float _steerHelper = 1;
        private float _accelValue;
        private float _brakeValue;
        private float _steeringAngle;
        private float _currentTorque;
        private DriveType _driveType = DriveType.RearWheelDrive;
        private float _oldRotation;
        private float _maxSpeed = 200;

        void Start()
        {
            _wheelMeshLocalRotation = new Quaternion[ 4 ];
            for ( int i = 0; i < 4; i++ )
                _wheelMeshLocalRotation[ i ] = _wheelMeshes[ i ].transform.localRotation;
            
            _rigidbody = GetComponent< Rigidbody >();
            _currentTorque = _fullTorqueOverAllWheels - ( _tractionControl * _fullTorqueOverAllWheels );

            LoadSettings();
        }

        void FixedUpdate()
        {
            float horizontal = Input.GetAxis( "Horizontal" );
            float vertical = Input.GetAxis( "Vertical" );
            float handbrake = Input.GetAxis( "Handbrake" );

            Move( horizontal, vertical, vertical, handbrake );
        }

        private void LoadSettings()
        {
            _tractionControl = PlayerPrefs.GetFloat( "TractionControll", 1 );
            _driveType = ( DriveType )PlayerPrefs.GetInt( "DriveType", 1 );

        }

        private void Move( float steeringValue, float accelaration, float footbrake, float handbrake )
        {
            for( int i = 0; i < 4; i++ )
            {
                Quaternion quat;
                Vector3 position;
                _wheelColliders[ i ].GetWorldPose( out position, out quat );
                _wheelMeshes[ i ].transform.position = position;
                _wheelMeshes[ i ].transform.rotation = quat;
            }

            steeringValue = Mathf.Clamp( steeringValue, -1, 1 );
            _accelValue = Mathf.Clamp( accelaration, 0, 1 );
            _brakeValue = -1 * Mathf.Clamp( footbrake, -1, 0 );
            handbrake = Mathf.Clamp( handbrake, 0, 1 );

            _steeringAngle = steeringValue * 90;
            _wheelColliders[ 0 ].steerAngle = _steeringAngle;
            _wheelColliders[ 1 ].steerAngle = _steeringAngle;

            SteerHelper();
            ApplyDrive( _accelValue, _brakeValue );
            CapSpeed();

            if( handbrake > 0f )
            {
                var hbTorque = handbrake * float.MaxValue;
                _wheelColliders[ 2 ].brakeTorque = hbTorque;
                _wheelColliders[ 3 ].brakeTorque = hbTorque;
            }
        }
        private void CapSpeed()
        {
            float speed = _rigidbody.velocity.magnitude;
            
            speed *= 3.6f;
            if( speed > _maxSpeed )
                _rigidbody.velocity = ( _maxSpeed / 3.6f ) * _rigidbody.velocity.normalized;
            
        }

        private void SteerHelper()
        {
            for( int i = 0; i < 4; i++ )
            {
                WheelHit wheelhit;
                _wheelColliders[ i ].GetGroundHit( out wheelhit );
                if( wheelhit.normal == Vector3.zero )
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if( Mathf.Abs( _oldRotation - transform.eulerAngles.y ) < 10f )
            {
                var turnadjust = ( transform.eulerAngles.y - _oldRotation ) * _steerHelper;
                Quaternion velRotation = Quaternion.AngleAxis( turnadjust, Vector3.up );
                _rigidbody.velocity = velRotation * _rigidbody.velocity;
            }
            _oldRotation = transform.eulerAngles.y;
        }

        private void ApplyDrive( float accel, float footbrake )
        {

            float thrustTorque;
            switch( _driveType )
            {
                case DriveType.FourWheelDrive:
                    thrustTorque = accel * ( _currentTorque / 4f );
                    for( int i = 0; i < 4; i++ )
                    {
                        _wheelColliders[ i ].motorTorque = thrustTorque;
                    }
                    break;

                case DriveType.FrontWheelDrive:
                    thrustTorque = accel * ( _currentTorque / 2f );

                    _wheelColliders[ 0 ].motorTorque = _wheelColliders[ 1 ].motorTorque = thrustTorque;
                    break;

                case DriveType.RearWheelDrive:
                    thrustTorque = accel * ( _currentTorque / 2f );

                    _wheelColliders[ 2 ].motorTorque = _wheelColliders[ 3 ].motorTorque = thrustTorque;
                    break;

            }

            for( int i = 0; i < 4; i++ )
            {
                if( CurrentSpeed > 5 && Vector3.Angle( transform.forward, _rigidbody.velocity ) < 50f )
                {
                    _wheelColliders[ i ].brakeTorque = _brakeTorque * footbrake;
                }
                else if( footbrake > 0 )
                {
                    _wheelColliders[ i ].brakeTorque = 0f;
                    _wheelColliders[ i ].motorTorque = -_reverseTorque * footbrake;
                }
            }
        }

    }
}