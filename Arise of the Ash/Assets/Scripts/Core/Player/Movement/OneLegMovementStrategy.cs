using Configs;
using UnityEngine;

namespace Core.Player.Movement
{
    public class OneLegMovementStrategy : IMovementStrategy
    {
        private readonly MovementConfig _config;
        private Vector3 _velocity;
        private float _timer;
        private bool _airborne;
        public bool CanJump => false;
        public bool CanRun => false;
        public OneLegMovementStrategy(MovementConfig config)
        {
            _config = config;
        }
        public void Tick(float deltaTime, in PlayerInputData input, CharacterController controller, Transform transform)
        {
            bool wantsMove = input.Move.sqrMagnitude > 0.01f;
            bool grounded = controller.isGrounded;
            Vector3 moveDir = transform.right * input.Move.x + transform.forward * input.Move.y;
            moveDir.y = 0f;
            if (moveDir.sqrMagnitude > 1f)
                moveDir.Normalize();
            if (!wantsMove)
            {
                _timer = 0f;
                _airborne = false;
                _velocity.x = 0f;
                _velocity.z = 0f;
            }
            else
            {
                _timer += deltaTime;
                float interval = Mathf.Max(0.05f, _config.OneLegHopInterval);
                float t = Mathf.Repeat(_timer, interval) / interval;
                if (grounded && !_airborne && t < 0.08f)
                {
                    _velocity.x = moveDir.x * _config.OneLegHopSpeed * 0.35f;
                    _velocity.z = moveDir.z * _config.OneLegHopSpeed * 0.35f;
                }
                else if (grounded && !_airborne && t >= 0.08f && t < 0.2f)
                {
                    _airborne = true;
                    _velocity.x = moveDir.x * _config.OneLegHopSpeed;
                    _velocity.z = moveDir.z * _config.OneLegHopSpeed;
                    _velocity.y = _config.OneLegHopForce;
                }
                else if (grounded && _airborne && t > 0.45f)
                {
                    _airborne = false;
                    _velocity.x = 0f;
                    _velocity.z = 0f;
                    _velocity.y = -2f;
                }
            }
            if (grounded && _velocity.y < 0f)
                _velocity.y = -2f;
            else if (!grounded)
                _velocity.y += _config.Gravity * deltaTime;
            controller.Move(_velocity * deltaTime);
        }
    }
}