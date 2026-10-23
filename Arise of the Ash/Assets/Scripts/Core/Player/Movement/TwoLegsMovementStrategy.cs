using Configs;
using UnityEngine;

namespace Core.Player.Movement
{
    public class TwoLegsMovementStrategy : IMovementStrategy
    {
        private readonly MovementConfig _config;
        private Vector3 _velocity;
        private Vector3 _groundNormal = Vector3.up;
        public bool CanJump => true;
        public bool CanRun => true;
        public TwoLegsMovementStrategy(MovementConfig config)
        {
            _config = config;
        }
        public void Tick(float deltaTime, in PlayerInputData input, CharacterController controller, Transform transform)
        {
            bool isGrounded = controller.isGrounded;
            UpdateGroundNormal(controller, transform);
            float slopeAngle = Vector3.Angle(_groundNormal, Vector3.up);
            bool tooSteep = slopeAngle > _config.MaxWalkableSlope;
            Vector3 moveDir = transform.right * input.Move.x + transform.forward * input.Move.y;
            moveDir.y = 0f;
            if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();
            if (isGrounded && !tooSteep && moveDir.sqrMagnitude > 0.001f)
                moveDir = Vector3.ProjectOnPlane(moveDir, _groundNormal).normalized;
            float targetSpeed = input.RunHeld ? _config.TwoLegsRunSpeed : _config.TwoLegsWalkSpeed;
            Vector3 targetVelocity = moveDir * targetSpeed;
            if (isGrounded && tooSteep)
            {
                Vector3 slide = Vector3.ProjectOnPlane(Vector3.down, _groundNormal).normalized;
                _velocity.x = Mathf.MoveTowards(_velocity.x, slide.x * _config.SlopeSlideSpeed, 20f * deltaTime);
                _velocity.z = Mathf.MoveTowards(_velocity.z, slide.z * _config.SlopeSlideSpeed, 20f * deltaTime);
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, targetVelocity.x, _config.TwoLegsAcceleration * deltaTime);
                _velocity.z = Mathf.MoveTowards(_velocity.z, targetVelocity.z, _config.TwoLegsAcceleration * deltaTime);
            }
            if (isGrounded && input.JumpPressed && !tooSteep)
                _velocity.y = _config.JumpForce;
            if (isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;
            else
                _velocity.y += _config.Gravity * deltaTime;
            if (!isGrounded)
                ApplyWallSlide(controller, transform, deltaTime);
            controller.Move(_velocity * deltaTime);
        }
        private void UpdateGroundNormal(CharacterController controller, Transform transform)
        {
            if (!controller.isGrounded)
            {
                _groundNormal = Vector3.up;
                return;
            }
            Vector3 origin = transform.position + Vector3.up * 0.2f;
            if (Physics.SphereCast(origin, controller.radius * 0.9f, Vector3.down,
                    out RaycastHit hit, 0.6f, ~0, QueryTriggerInteraction.Ignore))
            {
                _groundNormal = hit.normal;
            }
            else
            {
                _groundNormal = Vector3.up;
            }
        }
        private void ApplyWallSlide(CharacterController controller, Transform transform, float deltaTime)
        {
            Vector3 horizontalVel = new Vector3(_velocity.x, 0f, _velocity.z);
            if (horizontalVel.sqrMagnitude < 0.01f)
                return;
            Vector3 origin = transform.position + Vector3.up * (controller.height * 0.5f);
            if (Physics.SphereCast(origin, controller.radius * 0.95f, horizontalVel.normalized,
                    out RaycastHit hit, 0.15f, ~0, QueryTriggerInteraction.Ignore))
            {
                float wallAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (wallAngle > _config.MaxWalkableSlope)
                {
                    Vector3 intoWall = Vector3.Project(horizontalVel, -hit.normal);
                    _velocity.x -= intoWall.x;
                    _velocity.z -= intoWall.z;
                    Vector3 slide = Vector3.ProjectOnPlane(Vector3.down, hit.normal);
                    _velocity += slide * (_config.WallSlideStrength * deltaTime);
                }
            }
        }
    }
}