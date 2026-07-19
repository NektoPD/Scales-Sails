using UnityEngine;

namespace ShipLogic
{
    public class AimTrajectory : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private Transform _marker;
        [SerializeField] private Color _color = new Color(1f, 0.5f, 0f, 1f);
        [SerializeField] private float _travelDuration = 0.4f;
        [SerializeField] private float _maxLength = 4f;

        private Vector2 _origin;
        private Vector2 _target;
        private float _arcHeight;
        private float _timer;
        private bool _isShowing;

        private void Awake()
        {
            SetupTrail();
            Hide();
        }

        public void Show(Vector2 origin, Vector2 target, float arcHeight)
        {
            _origin = origin;
            _target = ClampToMaxLength(origin, target);
            _arcHeight = arcHeight;

            if (_isShowing)
                return;

            _isShowing = true;
            _timer = 0f;

            if (_marker != null)
                _marker.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _isShowing = false;

            if (_marker != null)
                _marker.gameObject.SetActive(false);

            if (_trail != null)
                _trail.Clear();
        }

        private void Update()
        {
            if (!_isShowing || _marker == null)
                return;

            _timer += Time.deltaTime;
            float progress = _travelDuration > 0f ? _timer / _travelDuration : 1f;

            if (progress >= 1f)
            {
                progress = 0f;
                _timer = 0f;

                if (_trail != null)
                    _trail.Clear();
            }

            Vector2 groundPosition = Vector2.Lerp(_origin, _target, progress);
            float height = _arcHeight * 4f * progress * (1f - progress);
            _marker.position = new Vector3(groundPosition.x, groundPosition.y + height, _marker.position.z);
        }

        private Vector2 ClampToMaxLength(Vector2 origin, Vector2 target)
        {
            Vector2 direction = target - origin;

            if (direction.magnitude > _maxLength)
                return origin + direction.normalized * _maxLength;

            return target;
        }

        private void SetupTrail()
        {
            if (_trail == null)
                return;

            _trail.material = new Material(Shader.Find("Sprites/Default"));

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(_color, 0f),
                    new GradientColorKey(_color, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(_color.a, 0f),
                    new GradientAlphaKey(0f, 1f)
                });

            _trail.colorGradient = gradient;
        }
    }
}
