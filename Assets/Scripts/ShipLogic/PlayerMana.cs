using System;
using UnityEngine;

namespace ShipLogic
{
    public class PlayerMana : MonoBehaviour
    {
        [SerializeField] private float _maxMana = 100f;
        [SerializeField] private float _manaPerKill = 20f;
        [SerializeField] private float _startMana = 0f;

        private float _currentMana;

        public event Action<float, float> ManaChanged;

        public float MaxMana => _maxMana;
        public float CurrentMana => _currentMana;
        public bool IsEmpty => _currentMana <= 0f;

        private void Start()
        {
            _currentMana = Mathf.Clamp(_startMana, 0f, _maxMana);
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
            ManaChanged?.Invoke(_currentMana, _maxMana);
        }

        public bool HasMana(float amount) => _currentMana >= amount;

        public void Drain(float amount)
        {
            if (amount <= 0f)
                return;

            _currentMana = Mathf.Clamp(_currentMana - amount, 0f, _maxMana);
            ManaChanged?.Invoke(_currentMana, _maxMana);
        }
    }
}
