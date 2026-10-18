using Configs;
using UnityEngine;

namespace Core.Player.Movement
{
    public class TwoLegsMovementStrategy : IMovementStrategy
    {
        private readonly MovementConfig _config;
        private Vector3 _velocity;
        public bool CanJump => true;
        public bool CanRun => true;
        public TwoLegsMovementStrategy(MovementConfig config)
        {
            _config = config;
        }
        public void Tick(float deltaTime, in PlayerInputData input, CharacterController controller, Transform transform)
        {
            bool isGrounded = controller.isGrounded;
            Vector3 moveDir = transform.right * input.Move.x + transform.forward * input.Move.y;
            moveDir.y = 0f;
            if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();
            float targetSpeed = input.RunHeld ? _config.TwoLegsRunSpeed : _config.TwoLegsWalkSpeed;
            Vector3 targetVelocity = moveDir * targetSpeed;
            _velocity.x = Mathf.MoveTowards(_velocity.x, targetVelocity.x, _config.TwoLegsAcceleration * deltaTime);
            _velocity.z = Mathf.MoveTowards(_velocity.z, targetVelocity.z, _config.TwoLegsAcceleration * deltaTime);
            if (isGrounded && input.JumpPressed)
                _velocity.y = _config.JumpForce;
            if (isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;
            else
                _velocity.y += _config.Gravity * deltaTime;
            controller.Move(_velocity * deltaTime);
        }
    }
}