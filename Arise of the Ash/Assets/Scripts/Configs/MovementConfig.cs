using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Configs/Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        [Header("One Leg")]
        public float OneLegHopInterval = 0.7f;
        public float OneLegHopForce = 3.2f;
        public float OneLegHopSpeed = 3.4f;
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
    }
}