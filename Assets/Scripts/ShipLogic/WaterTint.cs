using UnityEngine;
using UnityEngine.Tilemaps;

namespace ShipLogic
{
    [RequireComponent(typeof(Tilemap))]
    public class WaterTint : MonoBehaviour
    {
        [SerializeField] private Color _colorA = new Color(0.85f, 0.92f, 1f, 1f);
        [SerializeField] private Color _colorB = Color.white;
        [SerializeField] private float _speed = 0.4f;

        private Tilemap _tilemap;

        private void Awake()
        {
            _tilemap = GetComponent<Tilemap>();
        }

        private void Update()
        {
            float pulse = (Mathf.Sin(Time.time * _speed) + 1f) * 0.5f;
            _tilemap.color = Color.Lerp(_colorA, _colorB, pulse);
        }
    }
}
