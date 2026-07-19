using ProjectileLogic;
using UnityEngine;

namespace EnemyLogic
{
    public class KamikazeEnemy : EnemyShip
    {
        [SerializeField] private float _ramDamage = 40f;
        [SerializeField] private float _ramRange = 0.6f;

        protected override void FixedUpdate()
        {
            if (_isDead || _player == null)
                return;

            MoveTowardsTarget();
        }

        protected override void MoveTowardsTarget()
        {
            Vector2 toPlayer = (Vector2)_player.position - _rigidbody.position;
            float distance = toPlayer.magnitude;

            RotateTowards(toPlayer);

            if (distance <= _ramRange)
            {
                Explode();
                return;
            }

            Vector2 direction = toPlayer.normalized;
            Vector2 nextPosition = _rigidbody.position + _moveSpeed * Time.fixedDeltaTime * direction;
            _rigidbody.MovePosition(nextPosition);
        }

        private void Explode()
        {
            if (_player.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(_ramDamage);

            Kill();
        }
    }
}
