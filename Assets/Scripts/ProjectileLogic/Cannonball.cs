using System;
using UnityEngine;

namespace ProjectileLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Cannonball : MonoBehaviour
    {
        [SerializeField] private Transform _visual;
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private float _heightScaleFactor = 0.4f;
        [SerializeField] private float _lifeTimeAfterLand = 0.1f;

        private Transform _transform;
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
            _transform = transform;

            if (_visual != null)
                _visualBaseScale = _visual.localScale;

            SetupTrailGradient();
        }

        public void Launch(Vector2 origin, Vector2 target, float arcHeight, float speed)
        {
            _origin = origin;
            _target = target;
            _arcHeight = arcHeight;
            _speed = speed;
            _distance = Vector2.Distance(origin, target);
            _progress = 0f;
            _isFlying = true;

            _transform.position = origin;

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
            _isFlying = false;
            Landed?.Invoke(position);
            Destroy(gameObject, _lifeTimeAfterLand);
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
