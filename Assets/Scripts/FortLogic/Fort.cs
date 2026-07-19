using System;
using DG.Tweening;
using ProjectileLogic;
using UnityEngine;

namespace FortLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Fort : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 500f;
        [SerializeField] private float _hitShakeStrength = 0.15f;
        [SerializeField] private float _destroyShakeDuration = 0.6f;

        private SpriteRenderer _renderer;
        private Transform _transform;
        private Vector3 _basePosition;
        private float _currentHealth;
        private bool _isDestroyed;

        public event Action<float, float> HealthChanged;
        public event Action Destroyed;

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsDestroyed => _isDestroyed;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _transform = transform;
            _basePosition = _transform.localPosition;
        }

        private void Start()
        {
            _currentHealth = _maxHealth;
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if (_isDestroyed || damage <= 0f)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, _maxHealth);
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
            PlayHitFeedback();

            if (_currentHealth <= 0f)
                DestroyFort();
        }

        private void PlayHitFeedback()
        {
            _transform.DOKill();
            _transform.localPosition = _basePosition;
            _transform.DOShakePosition(0.2f, _hitShakeStrength, 10, 90f, false, false)
                .OnComplete(() => _transform.localPosition = _basePosition);
        }

        private void DestroyFort()
        {
            _isDestroyed = true;
            _transform.DOKill();
            _transform.DOShakePosition(_destroyShakeDuration, 0.3f)
                .OnComplete(() => Destroyed?.Invoke());
        }
    }
}
