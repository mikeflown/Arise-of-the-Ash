using UnityEngine;

namespace Core.Player.Movement
{
    public interface IMovementStrategy
    {
        void Tick(float deltaTime, in PlayerInputData input, CharacterController controller, Transform transform);
        bool CanJump { get; }
        bool CanRun { get; }
    }
}