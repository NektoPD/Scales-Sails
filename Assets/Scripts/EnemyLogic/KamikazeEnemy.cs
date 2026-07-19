using ProjectileLogic;
using UnityEngine;

namespace EnemyLogic
{
    public class KamikazeEnemy : EnemyShip
    {
        [SerializeField] private float _ramDamage = 40f;
        [SerializeField] private float _ramRange = 0.6f;

        protected override void MoveTowardsTarget()
        {
            Vector2 toTarget = (Vector2)_target.position - _rigidbody.position;
            float distance = toTarget.magnitude;

            RotateTowards(toTarget);

            if (distance <= _ramRange)
            {
                Explode();
                return;
            }

            Vector2 direction = toTarget.normalized;
            Vector2 nextPosition = _rigidbody.position + _moveSpeed * Time.fixedDeltaTime * direction;
            _rigidbody.MovePosition(nextPosition);
        }

        private void Explode()
        {
            if (_target.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(_ramDamage);

            Kill();
        }
    }
}
