using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(EnemyCannon))]
    public class GunboatEnemy : EnemyShip
    {
        private EnemyCannon _cannon;

        protected override void Awake()
        {
            base.Awake();
            _cannon = GetComponent<EnemyCannon>();
        }

        private void Update()
        {
            if (_isDead || _player == null)
                return;

            if (_inRange)
                _cannon.TryFire(GetFireTarget().position);
        }
    }
}
