using UI;
using UnityEngine;

namespace Character
{
    public class CameraOrbit : MonoBehaviour
    {
        public Vector3 LookPosition { get; private set; }
        public int LookAngle => lookAngle;
        private Vector3 CameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                halfExtends.y = _regularCamera.nearClipPlane * Mathf.Tan(.5f * Mathf.Deg2Rad * _regularCamera.fieldOfView);
                halfExtends.x = halfExtends.y * _regularCamera.aspect;
                halfExtends.z = .0f;
                return halfExtends;
            }
        }

        [SerializeField] 
        private Transform focus = default;
        [SerializeField, Range(0.01f, 1.0f)] 
        private float distance = 5.0f;
        [SerializeField, Range(0, 90)] 
        private int lookAngle;
        [SerializeField, Min(.0f)] 
        private float focusRadius = 1.0f;
        [SerializeField, Range(.0f, 1.0f)] 
        private float focusCentering = .5f;
        [SerializeField, Range(.1f, 5.0f)] 
        private float sensitive = .5f;
        [SerializeField, Range(1.0f, 360f)] 
        private float rotationSpeed = 90.0f;
        [SerializeField, Range(-89.0f, 89.0f)] 
        private float minVerticalAngle = -30.0f, maxVerticalAngle = 60.0f;
        [SerializeField] 
        private LayerMask obstacleMask;
        
        private Vector3 _focusPoint;
        private Vector2 _orbitAngles = new Vector2(45.0f, 0f);
        private float _currentDistance;
        private float _desiredDistance;
        private Camera _regularCamera;

        public void Initiate(Transform cameraAttach)
        {
            focus = cameraAttach;
            transform.parent = null;
            _desiredDistance = distance;
            _currentDistance = distance;
            _regularCamera = GetComponent<Camera>();
            _focusPoint = focus.position;
            transform.localRotation = ConstrainAngles(ref _orbitAngles);
        }

        public void CameraMovement()
        {
            UpdateFocusPoint();
            Quaternion lookRotation = ManualRotation(ref _orbitAngles) ? ConstrainAngles(ref _orbitAngles) : transform.localRotation;
            Vector3 lookDirection = lookRotation * Vector3.forward;
            LookPosition = _focusPoint + lookDirection;
            if (Physics.BoxCast(_focusPoint, CameraHalfExtends, -lookDirection, out RaycastHit hit, lookRotation, distance - _regularCamera.nearClipPlane, obstacleMask))
            {
                _desiredDistance = hit.distance * _regularCamera.nearClipPlane;
            }
            else
            {
                _desiredDistance = distance;
            }
            _currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * 20.0f);
            Vector3 lookPosition = _focusPoint - lookDirection * _currentDistance;

            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        public void SetFov(float fov, float changeSpeed)
        {
            _regularCamera.fieldOfView = Mathf.Lerp(_regularCamera.fieldOfView, fov, changeSpeed * Time.deltaTime);
        }

        public void ShowPlayerLabels(PlayerLabel label)
        {
            label.DrawLabel(_regularCamera);
        }

        private void OnValidate()
        {
            UpdateMinMaxVerticalAngles();
        }

        private void UpdateMinMaxVerticalAngles()
        {
            if (maxVerticalAngle < minVerticalAngle)
            {
                minVerticalAngle = maxVerticalAngle;
            }
        }

        private void UpdateFocusPoint()
        {
            var targetPoint = focus.position;
            if (focusRadius > .0f)
            {
                float distance = Vector3.Distance(targetPoint, _focusPoint);
                float t = 1.0f;
                if (distance > .01f && focusCentering > .0f) 
                {
                    t = Mathf.Pow(1.0f - focusCentering, Time.deltaTime);
                }

                if(distance > focusRadius)
                {
                    t = Mathf.Min(t, focusRadius / distance);
                }

                _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
            }
            else
            {
                _focusPoint = targetPoint;
            }
        }

        private static float GetAngle(Vector2 direction)
        {
            float angle = Mathf.Acos(direction.y) * Mathf.Deg2Rad;
            return direction.x < .0f ? 360.0f - angle : angle;
        }

        private bool ManualRotation(ref Vector2 orbitAngles)
        {
            Vector2 input = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
            float e = Mathf.Epsilon;
            if(input.x < -e || input.x > e || input.y < -e || input.y > e)
            {
                orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input * sensitive;
                return true;
            }
            return false;
        }

        private Quaternion ConstrainAngles(ref Vector2 orbitAngles)
        {
            orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);
            if(orbitAngles.y < .0f)
            {
                orbitAngles.y += 360.0f;
            }
            else if(orbitAngles.y >= 360.0f)
            {
                orbitAngles.y -= 360.0f;
            }
            return Quaternion.Euler(orbitAngles);
        }
    }
}
