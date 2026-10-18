using Core.Interaction;
using Core.Player;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    public class InteractableProxy : MonoBehaviour
    {
        [SerializeField] private LimbType _limbType;
        private IInteractable _interactable;
        public IInteractable Interactable => _interactable;
        private void Awake()
        {
            _interactable = new LimbPickup(_limbType, gameObject);
        }
    }
}