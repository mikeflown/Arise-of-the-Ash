using UnityEngine;

namespace Core.Player.Movement
{
    public struct PlayerInputData
    {
        public Vector2 Move;
        public Vector2 Look;
        public bool JumpPressed;
        public bool JumpHeld;
        public bool RunHeld;
        public bool InteractPressed;
        public bool InteractHeld;

        public static PlayerInputData Empty => new PlayerInputData();
    }
}