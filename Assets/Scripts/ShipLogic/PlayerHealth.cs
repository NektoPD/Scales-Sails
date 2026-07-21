using System;
using DG.Tweening;
using ProjectileLogic;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Image _healthFill;

        private Transform _transform;
        private float _currentHealth;
        private float _healthMultiplier = 1f;
        private bool _isDead;

        public event Action<float, float> HealthChanged;
        public event Action Died;

        public float MaxHealth => _maxHealth * _healthMultiplier;
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
            _currentHealth = MaxHealth;
            UpdateSprite();
            UpdateFill();
            HealthChanged?.Invoke(_currentHealth, MaxHealth);
        }

        private void UpdateFill()
        {
            if (_healthFill != null)
                _healthFill.fillAmount = MaxHealth > 0f ? _currentHealth / MaxHealth : 0f;
        }

        public void SetMaxHealthMultiplier(float multiplier)
        {
            float ratio = MaxHealth > 0f ? _currentHealth / MaxHealth : 1f;
            _healthMultiplier = multiplier;
            _currentHealth = MaxHealth * ratio;
            UpdateFill();
            HealthChanged?.Invoke(_currentHealth, MaxHealth);
            UpdateSprite();
        }

        public void TakeDamage(float damage)
        {
            if (_isDead || damage <= 0f)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, MaxHealth);
            UpdateFill();
            HealthChanged?.Invoke(_currentHealth, MaxHealth);
            UpdateSprite();

            if (_currentHealth <= 0f)
                Die();
        }

        private void UpdateSprite()
        {
            if (_spriteRenderer == null || _damagedSprite == null)
                return;

            bool isDamaged = _currentHealth <= MaxHealth * _damagedThreshold;
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
