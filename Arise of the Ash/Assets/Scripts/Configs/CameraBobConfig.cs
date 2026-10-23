using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "CameraBobConfig", menuName = "Configs/Camera Bob Config")]
    public class CameraBobConfig : ScriptableObject
    {
        [Header("Одна нога")]
        public float OneLegHopInterval = 0.7f;
        public float OneLegPush = 0.035f;
        public float OneLegRise = 0.14f;
        public float OneLegSide = 0.012f;
        [Header("Две ноги — ходьба")]
        public float TwoLegsWalkFrequency = 2.1f;
        public float TwoLegsWalkVerticalAmplitude = 0.07f;
        public float TwoLegsWalkSideAmplitude = 0.05f;
        public float TwoLegsWalkSmoothing = 10f;
        [Header("Две ноги — бег")]
        public float TwoLegsRunFrequency = 2.8f;
        public float TwoLegsRunVerticalAmplitude = 0.11f;
        public float TwoLegsRunSideAmplitude = 0.09f;
        public float TwoLegsRunSmoothing = 12f;
        [Header("Общие")]
        public float MouseSensitivity = 0.15f;
    }
}