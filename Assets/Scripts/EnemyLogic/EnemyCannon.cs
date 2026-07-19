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

        private float _timer;

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
            ball.Launch(_firePoint.position, targetPosition, _arcHeight, _projectileSpeed, _damageMask, _damage, 1f, 0);
        }
    }
}
