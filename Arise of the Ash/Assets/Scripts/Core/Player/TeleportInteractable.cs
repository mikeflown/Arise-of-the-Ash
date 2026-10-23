using Core.Interaction;
using Core.Player;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    public class TeleportInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _prompt = "Войти";
        [SerializeField] private Transform _destination;
        [SerializeField] private bool _useDestinationRotation = true;
        [SerializeField] private bool _requireAnyArm = false;
        public string InteractionPrompt => _prompt;
        public bool CanInteract(PlayerLimbs limbs)
        {
            if (_destination == null)
                return false;
            if (_requireAnyArm && (limbs == null || !limbs.HasAnyArm))
                return false;
            return true;
        }
        public void Interact(PlayerContext context)
        {
            if (_destination == null || context == null)
                return;
            CharacterController cc = context.CharacterController;
            Transform player = context.Transform;
            if (cc != null)
                cc.enabled = false;
            player.position = _destination.position;
            if (_useDestinationRotation)
                player.rotation = _destination.rotation;
            if (cc != null)
                cc.enabled = true;
        }
    }
}