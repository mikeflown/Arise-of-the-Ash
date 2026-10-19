using Core.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityAdapters.Player
{
    public class LimbDebugToggle : MonoBehaviour
    {
        private PlayerLimbs _limbs;
        public void Init(PlayerLimbs limbs)
        {
            _limbs = limbs;
        }
        private void Update()
        {
            if (_limbs == null || Keyboard.current == null)
                return;
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                _limbs.Toggle(LimbType.RightLeg);
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
                _limbs.Toggle(LimbType.LeftArm);
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
                _limbs.Toggle(LimbType.RightArm);
        }
    }
}