using ShipLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UILogic
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private PlayerMana _mana;
        [SerializeField] private Image _healthFill;
        [SerializeField] private Image _manaFill;
        [SerializeField] private GameObject _formHints;
        [SerializeField] private float _hintManaThreshold = 20f;

        private void OnEnable()
        {
            if (_health != null)
                _health.HealthChanged += OnHealthChanged;

            if (_mana != null)
                _mana.ManaChanged += OnManaChanged;
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.HealthChanged -= OnHealthChanged;

            if (_mana != null)
                _mana.ManaChanged -= OnManaChanged;
        }

        private void OnHealthChanged(float current, float max)
        {
            if (_healthFill != null)
                _healthFill.fillAmount = max > 0f ? current / max : 0f;
        }

        private void OnManaChanged(float current, float max)
        {
            if (_manaFill != null)
                _manaFill.fillAmount = max > 0f ? current / max : 0f;

            if (_formHints != null)
                _formHints.SetActive(current >= _hintManaThreshold);
        }
    }
}
