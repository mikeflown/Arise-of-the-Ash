using Core.Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityAdapters.Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _runAction;
        private InputAction _interactAction;
        public PlayerInputData Current { get; private set; }
        private void Awake()
        {
            if (_inputActions == null)
            {
                Debug.LogError("Input Actions не назначен в PlayerInputReader!");
                return;
            }
            var playerMap = _inputActions.FindActionMap("Player");
            _moveAction = playerMap.FindAction("Move");
            _lookAction = playerMap.FindAction("Look");
            _jumpAction = playerMap.FindAction("Jump");
            _runAction = playerMap.FindAction("Run");
            _interactAction = playerMap.FindAction("Interact");
        }
        private void OnEnable() => _inputActions?.Enable();
        private void OnDisable() => _inputActions?.Disable();
        private void Update()
        {
            if (_inputActions == null) return;

            Current = new PlayerInputData
            {
                Move = _moveAction.ReadValue<Vector2>(),
                Look = _lookAction.ReadValue<Vector2>(),
                JumpPressed = _jumpAction.WasPressedThisFrame(),
                JumpHeld = _jumpAction.IsPressed(),
                RunHeld = _runAction.IsPressed(),
                InteractPressed = _interactAction.WasPressedThisFrame(),
                InteractHeld = _interactAction.IsPressed()
            };
        }
    }
}