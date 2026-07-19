using UnityEngine;
using EnemyLogic;
using FortLogic;
using ShipLogic;

namespace GameLogic
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Fort _fort;
        [SerializeField] private ShipController _playerShip;
        [SerializeField] private CannonballLauncher _playerLauncher;
        [SerializeField] private EnemySpawner _spawner;
        [SerializeField] private GameObject _gameOverScreen;

        private bool _isGameOver;

        private void OnEnable()
        {
            if (_fort != null)
                _fort.Destroyed += OnFortDestroyed;
        }

        private void OnDisable()
        {
            if (_fort != null)
                _fort.Destroyed -= OnFortDestroyed;
        }

        private void OnFortDestroyed()
        {
            if (_isGameOver)
                return;

            _isGameOver = true;

            if (_playerShip != null)
                _playerShip.Disable();

            if (_playerLauncher != null)
                _playerLauncher.Disable();

            if (_spawner != null)
                _spawner.StopSpawning();

            if (_gameOverScreen != null)
                _gameOverScreen.SetActive(true);
        }
    }
}
