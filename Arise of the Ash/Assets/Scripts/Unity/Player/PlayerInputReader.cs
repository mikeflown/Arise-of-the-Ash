using Core.Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _runAction;
        private InputAction _interactAction;
        public PlayerInput Current { get; private set; }
        private void Awake()
        {
            var playerMap = _inputActions.FindActionMap("Player");
            _moveAction = playerMap.FindAction("Move");
            _jumpAction = playerMap.FindAction("Jump");
            _runAction = playerMap.FindAction("Run");
            _interactAction = playerMap.FindAction("Interact");
        }
        private void OnEnable()
        {
            _inputActions.Enable();
        }
        private void OnDisable()
        {
            _inputActions.Disable();
        }
        private void Update()
        {
            Current = new PlayerInput
            {
                Move = _moveAction.ReadValue<Vector2>(),
                JumpPressed = _jumpAction.WasPressedThisFrame(),
                JumpHeld = _jumpAction.IsPressed(),
                RunHeld = _runAction.IsPressed(),
                InteractPressed = _interactAction.WasPressedThisFrame(),
                InteractHeld = _interactAction.IsPressed()
            };
        }
    }
}