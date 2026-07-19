using System;
using System.Collections.Generic;
using DG.Tweening;
using ProjectileLogic;
using UnityEngine;

namespace FortLogic
{
    public class FortDefence : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 150f;
        [SerializeField] private Transform _rotor;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private CannonballPool _pool;
        [SerializeField] private float _detectionRadius = 8f;
        [SerializeField] private float _fireCooldown = 1.5f;
        [SerializeField] private float _projectileSpeed = 9f;
        [SerializeField] private float _arcHeight = 1.5f;
        [SerializeField] private float _rotationSpeed = 180f;
        [SerializeField] private float _spriteForwardOffset = -90f;
        [SerializeField] private LayerMask _enemyMask = ~0;
        [SerializeField] private LayerMask _damageMask = ~0;
        [SerializeField] private float _deathDuration = 0.5f;

        public static readonly List<FortDefence> Active = new List<FortDefence>();

        private Transform _transform;
        private float _currentHealth;
        private float _timer;
        private bool _isDead;

        public event Action<float, float> HealthChanged;
        public event Action Destroyed;

        public float DetectionRadius => _detectionRadius;
        public bool IsDead => _isDead;

        private void Awake()
        {
            _transform = transform;

            if (_rotor == null)
                _rotor = transform;
        }

        private void OnEnable()
        {
            _currentHealth = _maxHealth;
            _isDead = false;
            Active.Add(this);
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        private void OnDisable()
        {
            Active.Remove(this);
        }

        private void Update()
        {
            if (_isDead)
                return;

            _timer -= Time.deltaTime;

            Transform target = FindNearestEnemy();

            if (target == null)
                return;

            RotateTowards(target.position);
            TryFire(target.position);
        }

        private Transform FindNearestEnemy()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _enemyMask);

            Transform nearest = null;
            float bestDistance = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                float distance = Vector2.Distance(_transform.position, hit.transform.position);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = hit.transform;
                }
            }

            return nearest;
        }

        private void RotateTowards(Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - (Vector2)_rotor.position;

            if (direction.sqrMagnitude < 0.001f)
                return;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + _spriteForwardOffset;
            float nextAngle = Mathf.MoveTowardsAngle(_rotor.eulerAngles.z, targetAngle, _rotationSpeed * Time.deltaTime);
            _rotor.rotation = Quaternion.Euler(0f, 0f, nextAngle);
        }

        private void TryFire(Vector2 targetPosition)
        {
            if (_timer > 0f || _pool == null || _firePoint == null)
                return;

            _timer = _fireCooldown;

            Cannonball ball = _pool.Get();
            ball.Launch(_firePoint.position, targetPosition, _arcHeight, _projectileSpeed, _damageMask);
        }

        public void TakeDamage(float damage)
        {
            if (_isDead || damage <= 0f)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, _maxHealth);
            HealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0f)
                Die();
        }

        private void Die()
        {
            _isDead = true;
            Active.Remove(this);
            _transform.DOKill();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_transform.DOScale(Vector3.zero, _deathDuration).SetEase(Ease.InBack));
            sequence.OnComplete(() =>
            {
                Destroyed?.Invoke();
                gameObject.SetActive(false);
            });
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}
