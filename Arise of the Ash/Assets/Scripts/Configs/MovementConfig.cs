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
        [Header("Slope / Walls")]
        [Tooltip("Максимальный угол поверхности, по которой можно ходить (градусы). Выше — соскальзывание.")]
        public float MaxWalkableSlope = 45f;
        [Tooltip("Скорость соскальзывания со склонов круче MaxWalkableSlope")]
        public float SlopeSlideSpeed = 6f;
        [Tooltip("Сила отталкивания от почти вертикальных стен")]
        public float WallSlideStrength = 8f;
        [Header("Mantle (подтягивание)")]
        public float MantleCheckDistance = 0.55f;
        public float MantleMinHeight = 0.35f;
        public float MantleMaxHeight = 1.35f;
        public float MantleDuration = 0.55f;
        public float MantleForwardOffset = 0.35f;
        public LayerMask MantleLayer = ~0;
        [Header("Heavy Drag")]
        public float HeavyDragSpeed = 1.6f;
        public float HeavyDragMaxDistance = 2.2f;
        public float HeavyDragStopDistance = 0.9f;
    }
}