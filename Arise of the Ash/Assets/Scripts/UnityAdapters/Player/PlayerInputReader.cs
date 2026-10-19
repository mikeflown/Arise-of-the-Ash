using Core.Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityAdapters.Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        private InputActionMap _playerMap;
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
            _playerMap = _inputActions.FindActionMap("Player");
            _moveAction = _playerMap.FindAction("Move");
            _lookAction = _playerMap.FindAction("Look");
            _jumpAction = _playerMap.FindAction("Jump");
            _runAction = _playerMap.FindAction("Run");
            _interactAction = _playerMap.FindAction("Interact");
        }
        private void OnEnable()
        {
            _playerMap?.Enable();
            _inputActions?.FindActionMap("UI")?.Enable();
        }
        private void OnDisable()
        {
            _playerMap?.Disable();
        }
        private void Update()
        {
            if (_playerMap == null)
                return;
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