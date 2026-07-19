using UnityEngine;

namespace EnemyLogic
{
    public class TransportEnemy : EnemyShip
    {
        [SerializeField] private EnemyShip _reinforcementPrefab;
        [SerializeField] private int _reinforcementCount = 3;
        [SerializeField] private float _spawnRadius = 1.5f;

        private bool _hasSpawned;

        protected override void MoveTowardsTarget()
        {
            base.MoveTowardsTarget();

            if (_inRange && !_hasSpawned)
                SpawnReinforcements();
        }

        private void SpawnReinforcements()
        {
            _hasSpawned = true;

            if (_reinforcementPrefab != null)
            {
                for (int i = 0; i < _reinforcementCount; i++)
                {
                    Vector2 offset = Random.insideUnitCircle * _spawnRadius;
                    Vector2 spawnPosition = _rigidbody.position + offset;

                    EnemyShip enemy = Instantiate(_reinforcementPrefab, spawnPosition, Quaternion.identity);
                    enemy.Initialize(_target, _player);
                }
            }

            Kill();
        }
    }
}
