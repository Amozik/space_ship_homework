using Network;
using UnityEngine;

namespace Mechanics
{
    public class PlanetOrbit : NetworkMovableObject
    {
        private const float CIRCLE_RADIUS = Mathf.PI * 2;

        protected override float Speed => smoothTime;

        [SerializeField] 
        private Transform aroundPoint;
        [SerializeField] 
        private float smoothTime = .3f;
        [SerializeField] 
        private float circleInSecond = 1f;

        [SerializeField] 
        private float offsetSin = 1;
        [SerializeField] 
        private float offsetCos = 1;
        [SerializeField] 
        private float rotationSpeed;

        private float _dist;
        private float _currentAng;
        private Vector3 _currentPositionSmoothVelocity;
        private float _currentRotationAngle;
        
        public float CircleInSecond
        {
            set => circleInSecond = value;
        }

        public float RotationSpeed
        {
            set => rotationSpeed = value;
        }

        private void Start()
        {
            if (isServer)
            {
                _dist = (transform.position - aroundPoint.position).magnitude;
            }
            Initiate(UpdatePhase.FixedUpdate);
        }

        protected override void HasAuthorityMovement()
        {
            if (!isServer)
            {
                return;
            }

            Vector3 p = aroundPoint.position;
            p.x += Mathf.Sin(_currentAng) * _dist * offsetSin;
            p.z += Mathf.Cos(_currentAng) * _dist * offsetCos;
            transform.position = p;
            _currentRotationAngle += Time.deltaTime * rotationSpeed;
            _currentRotationAngle = Mathf.Clamp(_currentRotationAngle, 0, 361);
            if (_currentRotationAngle >= 360)
            {
                _currentRotationAngle = 0;
            }
            transform.rotation = Quaternion.AngleAxis(_currentRotationAngle, transform.up);
            _currentAng += CIRCLE_RADIUS * circleInSecond * Time.deltaTime;

            SendToServer();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        protected override void SendToServer()
        {
            _serverPosition = transform.position;
            _serverEuler = transform.eulerAngles;
        }

        protected override void FromServerUpdate()
        {
            if (!isClient)
            {
                return;
            }
            transform.position = Vector3.SmoothDamp(transform.position,
                _serverPosition, ref _currentPositionSmoothVelocity, Speed);
            transform.rotation = Quaternion.Euler(_serverEuler);
        }
    }
}
