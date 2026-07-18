using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipLogic
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _rotationSpeed = 180f;

        private readonly int _spriteForwardOffset = -90;

        private Rigidbody2D _rigidbody;
        private Camera _camera;
        private Transform _transform;

        private Vector2 _moveInput;
        private Vector2 _aimWorldPosition;
        private bool _canControl = true;

        public Vector2 AimWorldPosition => _aimWorldPosition;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _camera = Camera.main;
            _transform = transform;
        }

        private void Update()
        {
            if (!_canControl)
                return;

            ReadMoveInput();
            ReadAimInput();
        }

        private void FixedUpdate()
        {
            if (!_canControl)
                return;

            Move();
            RotateTowardsAim();
        }

        public void SetSpeed(float speed) => _moveSpeed = speed;

        public void Enable() => _canControl = true;

        public void Disable()
        {
            _canControl = false;
            _moveInput = Vector2.zero;
            _rigidbody.velocity = Vector2.zero;
        }

        private void ReadMoveInput()
        {
            Vector2 direction = Vector2.zero;

            if (Keyboard.current == null)
                return;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                direction.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                direction.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                direction.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                direction.x += 1f;

            _moveInput = direction.normalized;
        }

        private void ReadAimInput()
        {
            if (Mouse.current == null || _camera == null)
                return;

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            _aimWorldPosition = _camera.ScreenToWorldPoint(screenPosition);
        }

        private void Move()
        {
            Vector2 nextPosition = _rigidbody.position + _moveSpeed * Time.fixedDeltaTime * _moveInput;
            _rigidbody.MovePosition(nextPosition);
        }

        private void RotateTowardsAim()
        {
            Vector2 aimDirection = _aimWorldPosition - _rigidbody.position;

            if (aimDirection.sqrMagnitude < 0.01f)
                return;

            float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + _spriteForwardOffset;
            float nextAngle = Mathf.MoveTowardsAngle(_rigidbody.rotation, targetAngle, _rotationSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(nextAngle);
        }
    }
}
