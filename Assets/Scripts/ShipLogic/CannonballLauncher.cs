using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ProjectileLogic;

namespace ShipLogic
{
    public class CannonballLauncher : MonoBehaviour
    {
        [SerializeField] private Cannonball _cannonballPrefab;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _minDistance = 2f;
        [SerializeField] private float _maxDistance = 9f;
        [SerializeField] private float _maxChargeTime = 1.2f;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _minArcHeight = 0.6f;
        [SerializeField] private float _maxArcHeight = 2f;

        [Header("Charge Bar")]
        [SerializeField] private GameObject _chargeBarRoot;
        [SerializeField] private Image _chargeBarFill;

        private Camera _camera;
        private float _chargeTime;
        private bool _isCharging;
        private bool _canShoot = true;

        private void Awake()
        {
            _camera = Camera.main;
            HideChargeBar();
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
                UpdateChargeBar();
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
            HideChargeBar();
        }

        private void StartCharge()
        {
            _isCharging = true;
            _chargeTime = 0f;

            if (_chargeBarRoot != null)
                _chargeBarRoot.SetActive(true);

            UpdateChargeBar();
        }

        private void Fire()
        {
            _isCharging = false;

            if (_cannonballPrefab == null || _firePoint == null || _camera == null)
            {
                HideChargeBar();
                return;
            }

            float chargeNormalized = Mathf.Clamp01(_chargeTime / _maxChargeTime);

            Vector2 origin = _firePoint.position;
            Vector2 cursorWorld = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = (cursorWorld - origin).normalized;

            float distance = Mathf.Lerp(_minDistance, _maxDistance, chargeNormalized);
            float arcHeight = Mathf.Lerp(_minArcHeight, _maxArcHeight, chargeNormalized);
            Vector2 target = origin + direction * distance;

            Cannonball cannonball = Instantiate(_cannonballPrefab, origin, Quaternion.identity);
            cannonball.Launch(origin, target, arcHeight, _projectileSpeed);

            _chargeTime = 0f;
            HideChargeBar();
        }

        private void UpdateChargeBar()
        {
            if (_chargeBarFill != null)
                _chargeBarFill.fillAmount = Mathf.Clamp01(_chargeTime / _maxChargeTime);
        }

        private void HideChargeBar()
        {
            if (_chargeBarFill != null)
                _chargeBarFill.fillAmount = 0f;

            if (_chargeBarRoot != null)
                _chargeBarRoot.SetActive(false);
        }
    }
}
