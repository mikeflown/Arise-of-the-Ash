using Configs;
using UnityEngine;

namespace Core.Player.Movement
{
    public class OneLegMovementStrategy : IMovementStrategy
    {
        private readonly MovementConfig _config;
        private Vector3 _velocity;
        public bool CanJump => false;
        public bool CanRun => false;
        public OneLegMovementStrategy(MovementConfig config)
        {
            _config = config;
        }
        public void Tick(float deltaTime, in PlayerInput input, CharacterController controller, Transform transform)
        {
            Vector3 moveDir = transform.right * input.Move.x + transform.forward * input.Move.y;
            moveDir.y = 0f;
            moveDir.Normalize();
            float targetSpeed = _config.OneLegWalkSpeed;
            Vector3 targetVelocity = moveDir * targetSpeed;
            _velocity.x = Mathf.MoveTowards(_velocity.x, targetVelocity.x, _config.OneLegAcceleration * deltaTime);
            _velocity.z = Mathf.MoveTowards(_velocity.z, targetVelocity.z, _config.OneLegAcceleration * deltaTime);
            if (controller.isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;
            else
                _velocity.y += _config.Gravity * deltaTime;
            controller.Move(_velocity * deltaTime);
        }
    }
}