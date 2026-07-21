using ProjectileLogic;
using UnityEngine;

namespace EnemyLogic
{
    public class EnemyCannon : MonoBehaviour
    {
        [SerializeField] private CannonballPool _pool;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _fireCooldown = 2f;
        [SerializeField] private float _projectileSpeed = 8f;
        [SerializeField] private float _arcHeight = 1.5f;
        [SerializeField] private LayerMask _damageMask = ~0;
        [SerializeField] private float _damage = 15f;
        [SerializeField] private float _maxAimError = 3.5f;
        [SerializeField] private float _minAimError = 1.2f;

        private float _timer;
        private float _accuracy = 0f;

        public void SetAccuracy(float accuracy)
        {
            _accuracy = Mathf.Clamp01(accuracy);
        }

        public void TryFire(Vector2 targetPosition)
        {
            _timer -= Time.deltaTime;

            if (_timer > 0f)
                return;

            _timer = _fireCooldown;
            Fire(targetPosition);
        }

        private void Fire(Vector2 targetPosition)
        {
            if (_pool == null || _firePoint == null)
                return;

            Cannonball ball = _pool.Get();
            ball.Launch(_firePoint.position, ApplyAimError(targetPosition), _arcHeight, _projectileSpeed, _damageMask, _damage, 1f, 0);
        }

        private Vector2 ApplyAimError(Vector2 targetPosition)
        {
            float error = Mathf.Lerp(_maxAimError, _minAimError, _accuracy);
            Vector2 offset = UnityEngine.Random.insideUnitCircle * error;
            return targetPosition + offset;
        }
    }
}
