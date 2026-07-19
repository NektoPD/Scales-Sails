using UnityEngine;
using UnityEngine.UI;

namespace EnemyLogic
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private EnemyShip _enemy;
        [SerializeField] private Image _fillImage;
        [SerializeField] private GameObject _root;
        [SerializeField] private bool _hideWhenFull = true;

        private void Awake()
        {
            if (_enemy == null)
                _enemy = GetComponentInParent<EnemyShip>();
        }

        private void OnEnable()
        {
            if (_enemy != null)
                _enemy.HealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            if (_enemy != null)
                _enemy.HealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float current, float max)
        {
            float normalized = max > 0f ? current / max : 0f;

            if (_fillImage != null)
                _fillImage.fillAmount = normalized;

            if (_hideWhenFull && _root != null)
                _root.SetActive(normalized < 1f);
        }
    }
}
