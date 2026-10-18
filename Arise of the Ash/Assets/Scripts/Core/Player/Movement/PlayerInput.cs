using UnityEngine;

namespace Core.Player.Movement
{
    public struct PlayerInput
    {
        public Vector2 Move;
        public bool JumpPressed;
        public bool JumpHeld;
        public bool RunHeld;
        public bool InteractPressed;
        public bool InteractHeld;
        public static PlayerInput Empty => new PlayerInput();
    }
}