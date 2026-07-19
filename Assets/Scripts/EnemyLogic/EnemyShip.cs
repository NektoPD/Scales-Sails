using System;
using DG.Tweening;
using ProjectileLogic;
using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public abstract class EnemyShip : MonoBehaviour, IDamageable
    {
        [SerializeField] protected float _maxHealth = 50f;
        [SerializeField] protected float _moveSpeed = 2f;
        [SerializeField] protected float _rotationSpeed = 90f;
        [SerializeField] protected float _stoppingDistance = 4f;
        [SerializeField] protected float _spriteForwardOffset = -90f;
        [SerializeField] protected float _deathDuration = 0.4f;

        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected Sprite _intactSprite;
        [SerializeField] protected Sprite _damagedSprite;
        [SerializeField] protected float _damagedThreshold = 0.5f;

        [SerializeField] protected float _turretPriorityRadius = 6f;

        protected Rigidbody2D _rigidbody;
        protected Transform _transform;
        protected Transform _target;
        protected Transform _player;
        protected float _currentHealth;
        protected bool _isDead;
        protected bool _inRange;

        public event Action<EnemyShip> Died;
        public event Action<float, float> HealthChanged;

        public bool IsDead => _isDead;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0f;
            _transform = transform;

            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public virtual void Initialize(Transform fort, Transform player)
        {
            _target = fort;
            _player = player;
            _currentHealth = _maxHealth;
            _isDead = false;
            _inRange = false;
            UpdateSprite();
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        protected virtual void FixedUpdate()
        {
            if (_isDead || _target == null)
                return;

            MoveTowardsTarget();
        }

        protected virtual void MoveTowardsTarget()
        {
            Vector2 toTarget = (Vector2)_target.position - _rigidbody.position;
            float distance = toTarget.magnitude;

            _inRange = distance <= _stoppingDistance;

            RotateTowards(toTarget);

            if (_inRange)
                return;

            Vector2 direction = toTarget.normalized;
            Vector2 nextPosition = _rigidbody.position + _moveSpeed * Time.fixedDeltaTime * direction;
            _rigidbody.MovePosition(nextPosition);
        }

        protected void RotateTowards(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
                return;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + _spriteForwardOffset;
            float nextAngle = Mathf.MoveTowardsAngle(_rigidbody.rotation, targetAngle, _rotationSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(nextAngle);
        }

        public virtual void TakeDamage(float damage)
        {
            if (_isDead || damage <= 0f)
                return;

            _currentHealth -= damage;
            HealthChanged?.Invoke(Mathf.Max(_currentHealth, 0f), _maxHealth);
            UpdateSprite();

            if (_currentHealth <= 0f)
                Die();
        }

        protected Transform GetFireTarget()
        {
            FortLogic.FortDefence nearestTurret = null;
            float bestDistance = _turretPriorityRadius;

            foreach (FortLogic.FortDefence turret in FortLogic.FortDefence.Active)
            {
                if (turret == null || turret.IsDead)
                    continue;

                float distance = Vector2.Distance(_rigidbody.position, turret.transform.position);

                if (distance <= bestDistance)
                {
                    bestDistance = distance;
                    nearestTurret = turret;
                }
            }

            if (nearestTurret != null)
                return nearestTurret.transform;

            return _player;
        }

        protected void UpdateSprite()
        {
            if (_spriteRenderer == null || _damagedSprite == null)
                return;

            bool isDamaged = _currentHealth <= _maxHealth * _damagedThreshold;
            _spriteRenderer.sprite = isDamaged ? _damagedSprite : _intactSprite;
        }

        protected void Kill()
        {
            _currentHealth = 0f;
            Die();
        }

        protected virtual void Die()
        {
            if (_isDead)
                return;

            _isDead = true;
            _rigidbody.linearVelocity = Vector2.zero;
            Died?.Invoke(this);
            PlayDeathAnimation();
        }

        protected virtual void PlayDeathAnimation()
        {
            _transform.DOKill();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_transform.DOScale(Vector3.zero, _deathDuration).SetEase(Ease.InBack));
            sequence.Join(_transform.DORotate(new Vector3(0f, 0f, 180f), _deathDuration, RotateMode.FastBeyond360));
            sequence.OnComplete(() => Destroy(gameObject));
        }
    }
}
