using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Configs/Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        [Header("One Leg")]
        public float OneLegWalkSpeed = 2.2f;
        public float OneLegAcceleration = 8f;
        [Header("Two Legs")]
        public float TwoLegsWalkSpeed = 3.5f;
        public float TwoLegsRunSpeed = 6.5f;
        public float TwoLegsAcceleration = 12f;
        public float JumpForce = 7.5f;
        [Header("Full Body")]
        public float FullBodyWalkSpeed = 4f;
        public float FullBodyRunSpeed = 7.5f;
        public float FullBodyAcceleration = 14f;
        [Header("Common")]
        public float Gravity = -20f;
        public float GroundCheckDistance = 0.25f;
        public LayerMask GroundMask = ~0;
    }
}