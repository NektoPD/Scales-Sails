using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipLogic
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _rotationSpeed = 120f;
        [SerializeField] private float _spriteForwardOffset = -90f;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private float _throttleInput;
        private float _turnInput;
        private bool _canControl = true;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0f;
            _transform = transform;
        }

        private void Update()
        {
            if (!_canControl)
                return;

            ReadInput();
        }

        private void FixedUpdate()
        {
            if (!_canControl)
                return;

            Rotate();
            Move();
        }

        public void SetSpeed(float speed) => _moveSpeed = speed;

        public void Enable() => _canControl = true;

        public void Disable()
        {
            _canControl = false;
            _throttleInput = 0f;
            _turnInput = 0f;
            _rigidbody.linearVelocity = Vector2.zero;
        }

        private void ReadInput()
        {
            _throttleInput = 0f;
            _turnInput = 0f;

            if (Keyboard.current == null)
                return;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                _throttleInput += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                _throttleInput -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                _turnInput += 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                _turnInput -= 1f;
        }

        private void Rotate()
        {
            float nextAngle = _rigidbody.rotation + _turnInput * _rotationSpeed * Time.fixedDeltaTime;
            _rigidbody.MoveRotation(nextAngle);
        }

        private void Move()
        {
            float forwardAngle = (_rigidbody.rotation - _spriteForwardOffset) * Mathf.Deg2Rad;
            Vector2 forward = new Vector2(Mathf.Cos(forwardAngle), Mathf.Sin(forwardAngle));

            Vector2 nextPosition = _rigidbody.position + _throttleInput * _moveSpeed * Time.fixedDeltaTime * forward;
            _rigidbody.MovePosition(nextPosition);
        }
    }
}
