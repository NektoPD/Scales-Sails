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
        [SerializeField] private Wave[] _waves;
        [SerializeField] private float _timeBetweenWaves = 5f;

        private readonly List<EnemyShip> _alive = new List<EnemyShip>();
        private int _waveIndex;

        public event Action AllWavesCleared;

        private void Start()
        {
            StartCoroutine(RunWaves());
        }

        public void StopSpawning()
        {
            StopAllCoroutines();
        }

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
        }
    }
}
