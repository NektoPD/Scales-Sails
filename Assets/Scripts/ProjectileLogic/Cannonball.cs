using System;
using DG.Tweening;
using UnityEngine;

namespace ProjectileLogic
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class Cannonball : MonoBehaviour, IDamageable
    {
        [SerializeField] private Transform _visual;
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private float _heightScaleFactor = 0.4f;
        [SerializeField] private float _despawnDuration = 0.25f;
        [SerializeField] private float _damage = 25f;
        [SerializeField] private LayerMask _damageMask = ~0;

        private SpriteRenderer _renderer;
        private Transform _transform;
        private CannonballPool _pool;
        private Vector3 _visualBaseScale;
        private Vector2 _origin;
        private Vector2 _target;
        private float _arcHeight;
        private float _speed;
        private float _distance;
        private float _progress;
        private bool _isFlying;

        public event Action<Vector2> Landed;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _transform = transform;

            if (_visual != null)
                _visualBaseScale = _visual.localScale;

            SetupTrailGradient();
        }

        public void Initialize(CannonballPool pool) => _pool = pool;

        public void Launch(Vector2 origin, Vector2 target, float arcHeight, float speed, LayerMask damageMask)
        {
            _transform.DOKill();

            _damageMask = damageMask;
            _origin = origin;
            _target = target;
            _arcHeight = arcHeight;
            _speed = speed;
            _distance = Vector2.Distance(origin, target);
            _progress = 0f;
            _isFlying = true;

            _transform.position = origin;

            if (_visual != null)
                _visual.localScale = _visualBaseScale;

            SetRendererAlpha(1f);

            if (_trail != null)
                _trail.Clear();
        }

        private void Update()
        {
            if (!_isFlying)
                return;

            if (_distance > 0.01f)
                _progress += _speed * Time.deltaTime / _distance;

            _progress = Mathf.Clamp01(_progress);

            Vector2 groundPosition = Vector2.Lerp(_origin, _target, _progress);
            float height = _arcHeight * 4f * _progress * (1f - _progress);

            _transform.position = new Vector3(groundPosition.x, groundPosition.y + height, _transform.position.z);

            if (_visual != null)
                _visual.localScale = _visualBaseScale * (1f + height * _heightScaleFactor);

            if (_progress >= 1f)
                Land(groundPosition);
        }

        private void Land(Vector2 position)
        {
            if (!_isFlying)
                return;

            _isFlying = false;
            Landed?.Invoke(position);
            PlayDespawnAnimation();
        }

        private void PlayDespawnAnimation()
        {
            _transform.DOKill();

            Sequence sequence = DOTween.Sequence();

            if (_visual != null)
                sequence.Append(_visual.DOScale(_visualBaseScale * 1.4f, _despawnDuration * 0.4f).SetEase(Ease.OutQuad));

            sequence.Join(_renderer.DOFade(0f, _despawnDuration).SetEase(Ease.InQuad));
            sequence.OnComplete(Despawn);
        }

        private void Despawn()
        {
            _transform.DOKill();
            SetRendererAlpha(1f);

            if (_visual != null)
                _visual.localScale = _visualBaseScale;

            if (_pool != null)
                _pool.Return(this);
            else
                gameObject.SetActive(false);
        }

        private void SetRendererAlpha(float alpha)
        {
            Color color = _renderer.color;
            color.a = alpha;
            _renderer.color = color;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isFlying)
                return;

            if ((_damageMask.value & (1 << other.gameObject.layer)) == 0)
                return;

            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_damage);
                Land(_transform.position);
            }
        }

        public void TakeDamage(float damage)
        {
            if (_isFlying)
                Land(_transform.position);
        }

        private void SetupTrailGradient()
        {
            if (_trail == null)
                return;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                });

            _trail.colorGradient = gradient;
        }
    }
}
