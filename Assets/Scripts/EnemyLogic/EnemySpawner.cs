using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyLogic
{
    [Serializable]
    public class WaveEntry
    {
        public EnemyShip Prefab;
        public int Count = 3;
    }

    [Serializable]
    public class Wave
    {
        public List<WaveEntry> Entries = new List<WaveEntry>();
        public float SpawnInterval = 1f;
    }

    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform _fort;
        [SerializeField] private Transform _player;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private ShipLogic.PlayerMana _playerMana;

        [Header("Enemy prefabs")]
        [SerializeField] private EnemyShip _sloop;
        [SerializeField] private EnemyShip _gunboat;
        [SerializeField] private EnemyShip _battleship;
        [SerializeField] private EnemyShip _kamikaze;
        [SerializeField] private EnemyShip _transport;

        [Header("Flow")]
        [SerializeField] private Wave[] _waves;
        [SerializeField] private float _timeBetweenWaves = 6f;
        [SerializeField] private bool _autoGenerateWaves = true;

        private readonly List<EnemyShip> _alive = new List<EnemyShip>();
        private int _waveIndex;

        public event Action AllWavesCleared;

        private void Start()
        {
            if (_autoGenerateWaves || _waves == null || _waves.Length == 0)
                _waves = BuildWaves();

            StartCoroutine(RunWaves());
        }

        public void StopSpawning()
        {
            StopAllCoroutines();
        }

        private Wave[] BuildWaves()
        {
            List<Wave> waves = new List<Wave>
            {
                // 1 - onboarding: only fast weak sloops. Learn to aim.
                MakeWave(1.1f, E(_sloop, 3)),
                // 2 - introduce ranged pressure.
                MakeWave(1.0f, E(_sloop, 3), E(_gunboat, 2)),
                // 3 - first threat to the fort.
                MakeWave(0.9f, E(_sloop, 2), E(_gunboat, 2), E(_kamikaze, 2)),
                // 4 - tanky target + reinforcements from a transport.
                MakeWave(0.9f, E(_gunboat, 3), E(_battleship, 1), E(_transport, 1)),
                // 5 - mixed swarm, rising count.
                MakeWave(0.8f, E(_sloop, 4), E(_gunboat, 3), E(_kamikaze, 3)),
                // 6 - two battleships anchor the line.
                MakeWave(0.8f, E(_gunboat, 4), E(_battleship, 2), E(_kamikaze, 3)),
                // 7 - transports flood the field.
                MakeWave(0.7f, E(_sloop, 4), E(_transport, 2), E(_gunboat, 4), E(_kamikaze, 4)),
                // 8 - boss wave: everything at once.
                MakeWave(0.6f, E(_battleship, 3), E(_gunboat, 5), E(_kamikaze, 5), E(_transport, 2), E(_sloop, 5)),
            };

            return waves.ToArray();
        }

        private Wave MakeWave(float interval, params WaveEntry[] entries)
        {
            List<WaveEntry> valid = new List<WaveEntry>();

            foreach (WaveEntry entry in entries)
            {
                if (entry.Prefab != null && entry.Count > 0)
                    valid.Add(entry);
            }

            return new Wave { Entries = valid, SpawnInterval = interval };
        }

        private WaveEntry E(EnemyShip prefab, int count) => new WaveEntry { Prefab = prefab, Count = count };

        private IEnumerator RunWaves()
        {
            for (_waveIndex = 0; _waveIndex < _waves.Length; _waveIndex++)
            {
                yield return SpawnWave(_waves[_waveIndex]);

                while (_alive.Count > 0)
                    yield return null;

                yield return new WaitForSeconds(_timeBetweenWaves);
            }

            AllWavesCleared?.Invoke();
        }

        private IEnumerator SpawnWave(Wave wave)
        {
            foreach (WaveEntry entry in wave.Entries)
            {
                for (int i = 0; i < entry.Count; i++)
                {
                    SpawnEnemy(entry.Prefab);
                    yield return new WaitForSeconds(wave.SpawnInterval);
                }
            }
        }

        private void SpawnEnemy(EnemyShip prefab)
        {
            if (prefab == null || _spawnPoints.Length == 0)
                return;

            Transform point = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)];

            EnemyShip enemy = Instantiate(prefab, point.position, Quaternion.identity);
            enemy.Initialize(_fort, _player);
            enemy.Died += OnEnemyDied;
            _alive.Add(enemy);
        }

        private void OnEnemyDied(EnemyShip enemy)
        {
            enemy.Died -= OnEnemyDied;
            _alive.Remove(enemy);

            if (_playerMana != null)
                _playerMana.AddKillReward();
        }
    }
}
