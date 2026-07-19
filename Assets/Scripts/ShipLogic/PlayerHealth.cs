using System;
using DG.Tweening;
using ProjectileLogic;
using UnityEngine;

namespace ShipLogic
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 200f;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _intactSprite;
        [SerializeField] private Sprite _damagedSprite;
        [SerializeField] private float _damagedThreshold = 0.5f;
        [SerializeField] private float _deathDuration = 0.5f;

        private Transform _transform;
        private float _currentHealth;
        private bool _isDead;

        public event Action<float, float> HealthChanged;
        public event Action Died;

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsDead => _isDead;

        private void Awake()
        {
            _transform = transform;

            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            _currentHealth = _maxHealth;
            UpdateSprite();
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if (_isDead || damage <= 0f)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, _maxHealth);
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
            UpdateSprite();

            if (_currentHealth <= 0f)
                Die();
        }

        private void UpdateSprite()
        {
            if (_spriteRenderer == null || _damagedSprite == null)
                return;

            bool isDamaged = _currentHealth <= _maxHealth * _damagedThreshold;
            _spriteRenderer.sprite = isDamaged ? _damagedSprite : _intactSprite;
        }

        private void Die()
        {
            _isDead = true;
            Died?.Invoke();
            PlayDeathAnimation();
        }

        private void PlayDeathAnimation()
        {
            _transform.DOKill();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_transform.DOScale(Vector3.zero, _deathDuration).SetEase(Ease.InBack));
            sequence.Join(_transform.DORotate(new Vector3(0f, 0f, 180f), _deathDuration, RotateMode.FastBeyond360));
        }
    }
}
