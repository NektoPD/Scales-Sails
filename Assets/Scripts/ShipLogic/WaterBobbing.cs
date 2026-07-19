using UnityEngine;

namespace ShipLogic
{
    public class WaterBobbing : MonoBehaviour
    {
        [SerializeField] private Transform _visual;
        [SerializeField] private float _positionAmplitude = 0.05f;
        [SerializeField] private float _positionSpeed = 1.2f;
        [SerializeField] private float _rotationAmplitude = 2f;
        [SerializeField] private float _rotationSpeed = 0.9f;

        private Vector3 _basePosition;
        private float _phaseOffset;

        private void Awake()
        {
            if (_visual == null || _visual == transform)
            {
                Debug.LogWarning($"{nameof(WaterBobbing)} on '{name}' needs a child visual Transform (not the root). Disabling.", this);
                enabled = false;
                return;
            }

            _basePosition = _visual.localPosition;
            _phaseOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            float time = Time.time + _phaseOffset;

            float verticalOffset = Mathf.Sin(time * _positionSpeed) * _positionAmplitude;
            _visual.localPosition = _basePosition + new Vector3(0f, verticalOffset, 0f);

            float tilt = Mathf.Sin(time * _rotationSpeed) * _rotationAmplitude;
            _visual.localRotation = Quaternion.Euler(0f, 0f, tilt);
        }
    }
}
