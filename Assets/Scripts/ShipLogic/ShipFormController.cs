using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipLogic
{
    public enum ShipForm
    {
        Normal,
        Small,
        Big
    }

    [Serializable]
    public class ShipFormConfig
    {
        public float ScaleMultiplier = 1f;
        public float SpeedMultiplier = 1f;
        public float RotationMultiplier = 1f;
        public float DamageMultiplier = 1f;
        public float BallScaleMultiplier = 1f;
        public int PierceCount = 0;
        public float HealthMultiplier = 1f;
        public float ManaDrainPerSecond = 15f;
    }

    public class ShipFormController : MonoBehaviour
    {
        [SerializeField] private ShipController _controller;
        [SerializeField] private CannonballLauncher _launcher;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private PlayerMana _mana;
        [SerializeField] private Transform _visual;

        [SerializeField] private float _activationCost = 20f;
        [SerializeField] private float _transitionDuration = 0.3f;

        [SerializeField] private ShipFormConfig _small = new ShipFormConfig
        {
            ScaleMultiplier = 0.6f,
            SpeedMultiplier = 1.6f,
            RotationMultiplier = 1.6f,
            DamageMultiplier = 0.5f,
            BallScaleMultiplier = 0.6f,
            PierceCount = 0,
            HealthMultiplier = 0.6f,
            ManaDrainPerSecond = 15f
        };

        [SerializeField] private ShipFormConfig _big = new ShipFormConfig
        {
            ScaleMultiplier = 1.6f,
            SpeedMultiplier = 0.55f,
            RotationMultiplier = 0.5f,
            DamageMultiplier = 2f,
            BallScaleMultiplier = 1.7f,
            PierceCount = 3,
            HealthMultiplier = 2f,
            ManaDrainPerSecond = 20f
        };

        private readonly ShipFormConfig _normal = new ShipFormConfig();

        private ShipForm _currentForm = ShipForm.Normal;
        private Vector3 _baseVisualScale;

        public event Action<ShipForm> FormChanged;

        public ShipForm CurrentForm => _currentForm;

        private void Awake()
        {
            if (_visual == null)
                _visual = transform;

            _baseVisualScale = _visual.localScale;
        }

        private void Start()
        {
            ApplyStats(_normal);
        }

        private void Update()
        {
            HandleInput();
            HandleDrain();
        }

        private void HandleInput()
        {
            if (Keyboard.current == null)
                return;

            if (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame)
                ToggleForm(ShipForm.Big);

            if (Keyboard.current.leftCtrlKey.wasPressedThisFrame || Keyboard.current.rightCtrlKey.wasPressedThisFrame)
                ToggleForm(ShipForm.Small);
        }

        private void ToggleForm(ShipForm form)
        {
            if (_currentForm == form)
            {
                EnterForm(ShipForm.Normal);
                return;
            }

            if (_mana == null || !_mana.HasMana(_activationCost))
                return;

            _mana.Drain(_activationCost);
            EnterForm(form);
        }

        private void HandleDrain()
        {
            if (_currentForm == ShipForm.Normal || _mana == null)
                return;

            _mana.Drain(GetConfig(_currentForm).ManaDrainPerSecond * Time.deltaTime);

            if (_mana.IsEmpty)
                EnterForm(ShipForm.Normal);
        }

        private void EnterForm(ShipForm form)
        {
            _currentForm = form;

            ShipFormConfig config = GetConfig(form);
            ApplyStats(config);

            _visual.DOKill();
            _visual.DOScale(_baseVisualScale * config.ScaleMultiplier, _transitionDuration).SetEase(Ease.OutBack);

            FormChanged?.Invoke(form);
        }

        private void ApplyStats(ShipFormConfig config)
        {
            if (_controller != null)
                _controller.SetMovementMultipliers(config.SpeedMultiplier, config.RotationMultiplier);

            if (_launcher != null)
                _launcher.SetModifiers(config.DamageMultiplier, config.BallScaleMultiplier, config.PierceCount);

            if (_health != null)
                _health.SetMaxHealthMultiplier(config.HealthMultiplier);
        }

        private ShipFormConfig GetConfig(ShipForm form)
        {
            switch (form)
            {
                case ShipForm.Small:
                    return _small;
                case ShipForm.Big:
                    return _big;
                default:
                    return _normal;
            }
        }
    }
}
