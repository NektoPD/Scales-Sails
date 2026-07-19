using UnityEngine;
using UnityEngine.InputSystem;
using ProjectileLogic;

namespace ShipLogic
{
    public class CannonballLauncher : MonoBehaviour
    {
        [SerializeField] private CannonballPool _pool;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private AimTrajectory _aimTrajectory;
        [SerializeField] private float _minDistance = 2f;
        [SerializeField] private float _maxDistance = 9f;
        [SerializeField] private float _maxChargeTime = 1.2f;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _minArcHeight = 0.6f;
        [SerializeField] private float _maxArcHeight = 2f;

        private Camera _camera;
        private float _chargeTime;
        private bool _isCharging;
        private bool _canShoot = true;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!_canShoot || Mouse.current == null)
                return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
                StartCharge();

            if (_isCharging)
            {
                _chargeTime = Mathf.Min(_chargeTime + Time.deltaTime, _maxChargeTime);
                UpdateAim();
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && _isCharging)
                Fire();
        }

        public void Enable() => _canShoot = true;

        public void Disable()
        {
            _canShoot = false;
            _isCharging = false;
            _chargeTime = 0f;

            if (_aimTrajectory != null)
                _aimTrajectory.Hide();
        }

        private void StartCharge()
        {
            _isCharging = true;
            _chargeTime = 0f;
        }

        private void UpdateAim()
        {
            if (_aimTrajectory == null || _firePoint == null || _camera == null)
                return;

            GetShotData(out Vector2 origin, out Vector2 target, out float arcHeight);
            float charge = Mathf.Clamp01(_chargeTime / _maxChargeTime);
            _aimTrajectory.Show(origin, target, arcHeight, charge);
        }

        private void Fire()
        {
            _isCharging = false;

            if (_aimTrajectory != null)
                _aimTrajectory.Hide();

            if (_pool == null || _firePoint == null || _camera == null)
                return;

            GetShotData(out Vector2 origin, out Vector2 target, out float arcHeight);

            Cannonball cannonball = _pool.Get();
            cannonball.Launch(origin, target, arcHeight, _projectileSpeed);

            _chargeTime = 0f;
        }

        private void GetShotData(out Vector2 origin, out Vector2 target, out float arcHeight)
        {
            float chargeNormalized = Mathf.Clamp01(_chargeTime / _maxChargeTime);

            origin = _firePoint.position;
            Vector2 cursorWorld = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = (cursorWorld - origin).normalized;

            float distance = Mathf.Lerp(_minDistance, _maxDistance, chargeNormalized);
            arcHeight = Mathf.Lerp(_minArcHeight, _maxArcHeight, chargeNormalized);
            target = origin + direction * distance;
        }
    }
}

