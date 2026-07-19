using System;
using DG.Tweening;
using ProjectileLogic;
using UnityEngine;

namespace FortLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Wall : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Sprite _intactSprite;
        [SerializeField] private Sprite _damagedSprite;
        [SerializeField] private float _damagedThreshold = 0.5f;
        [SerializeField] private float _breakDuration = 0.5f;
        [SerializeField] private float _hitShakeStrength = 0.1f;

        private SpriteRenderer _renderer;
        private Transform _transform;
        private float _currentHealth;
        private bool _isBroken;

        public event Action<float> Changed;
        public event Action Broken;

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsBroken => _isBroken;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _transform = transform;
        }

        private void Start()
        {
            ResetWall();
        }

        public void ResetWall()
        {
            _isBroken = false;
            _currentHealth = _maxHealth;

            if (_intactSprite != null)
                _renderer.sprite = _intactSprite;

            _renderer.enabled = true;
            _transform.localScale = Vector3.one;
        }

        public void TakeDamage(float damage)
        {
            if (_isBroken || damage <= 0f)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, _maxHealth);
            Changed?.Invoke(_currentHealth);

            UpdateSprite();
            PlayHitFeedback();

            if (_currentHealth <= 0f)
                Break();
        }

        private void UpdateSprite()
        {
            if (_damagedSprite == null)
                return;

            bool isDamaged = _currentHealth <= _maxHealth * _damagedThreshold;
            _renderer.sprite = isDamaged ? _damagedSprite : _intactSprite;
        }

        private void PlayHitFeedback()
        {
            _transform.DOKill();
            _transform.localScale = Vector3.one;
            _transform.DOShakePosition(0.2f, _hitShakeStrength, 12, 90f, false, false);
        }

        private void Break()
        {
            _isBroken = true;
            _transform.DOKill();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_transform.DOShakeRotation(_breakDuration, 25f));
            sequence.Join(_transform.DOScale(Vector3.zero, _breakDuration).SetEase(Ease.InBack));
            sequence.OnComplete(() =>
            {
                _renderer.enabled = false;
                Broken?.Invoke();
            });
        }
    }
}
