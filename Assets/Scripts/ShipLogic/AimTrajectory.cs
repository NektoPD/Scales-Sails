using UnityEngine;

namespace ShipLogic
{
    [RequireComponent(typeof(LineRenderer))]
    public class AimTrajectory : MonoBehaviour
    {
        [SerializeField] private Color _color = new Color(1f, 0.5f, 0f, 1f);
        [SerializeField] private int _pointsCount = 20;
        [SerializeField] private float _minWidth = 0.05f;
        [SerializeField] private float _maxWidth = 0.3f;

        private LineRenderer _line;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            SetupLine();
            Hide();
        }

        public void Show(Vector2 origin, Vector2 target, float arcHeight, float charge)
        {
            _line.enabled = true;
            _line.positionCount = _pointsCount;

            float width = Mathf.Lerp(_minWidth, _maxWidth, Mathf.Clamp01(charge));
            _line.startWidth = width;
            _line.endWidth = width;

            for (int i = 0; i < _pointsCount; i++)
            {
                float progress = _pointsCount > 1 ? (float)i / (_pointsCount - 1) : 0f;
                Vector2 groundPosition = Vector2.Lerp(origin, target, progress);
                float height = arcHeight * 4f * progress * (1f - progress);
                _line.SetPosition(i, new Vector3(groundPosition.x, groundPosition.y + height, 0f));
            }
        }

        public void Hide()
        {
            _line.positionCount = 0;
            _line.enabled = false;
        }

        private void SetupLine()
        {
            _line.material = new Material(Shader.Find("Sprites/Default"));
            _line.useWorldSpace = true;
            _line.numCapVertices = 4;

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

            _line.colorGradient = gradient;
        }
    }
}
