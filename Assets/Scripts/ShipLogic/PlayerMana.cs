using System;
using UnityEngine;
using UnityEngine.UI;

namespace ShipLogic
{
    public class PlayerMana : MonoBehaviour
    {
        [SerializeField] private float _maxMana = 100f;
        [SerializeField] private float _manaPerKill = 20f;
        [SerializeField] private float _startMana = 0f;
        [SerializeField] private Image _manaFill;

        private float _currentMana;

        public event Action<float, float> ManaChanged;

        public float MaxMana => _maxMana;
        public float CurrentMana => _currentMana;
        public bool IsEmpty => _currentMana <= 0f;

        private void UpdateFill()
        {
            if (_manaFill != null)
                _manaFill.fillAmount = _maxMana > 0f ? _currentMana / _maxMana : 0f;
        }

        private void Start()
        {
            _currentMana = Mathf.Clamp(_startMana, 0f, _maxMana);
            UpdateFill();
            ManaChanged?.Invoke(_currentMana, _maxMana);
        }

        public void AddKillReward()
        {
            AddMana(_manaPerKill);
        }

        public void AddMana(float amount)
        {
            if (amount <= 0f)
                return;

            _currentMana = Mathf.Clamp(_currentMana + amount, 0f, _maxMana);
            UpdateFill();
            ManaChanged?.Invoke(_currentMana, _maxMana);
        }

        public bool HasMana(float amount) => _currentMana >= amount;

        public void Drain(float amount)
        {
            if (amount <= 0f)
                return;

            _currentMana = Mathf.Clamp(_currentMana - amount, 0f, _maxMana);
            UpdateFill();
            ManaChanged?.Invoke(_currentMana, _maxMana);
        }
    }
}
