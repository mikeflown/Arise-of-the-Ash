using Core.Interaction;
using Core.Player;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField] private float _interactionRadius = 2.5f;
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private Transform _cameraTransform;
        private PlayerLimbs _limbs;
        private PlayerContext _context;
        private IInteractable _currentInteractable;
        private InteractableHighlight _highlight;
        public IInteractable CurrentInteractable => _currentInteractable;
        public event System.Action<IInteractable> OnInteractableChanged;
        public void Init(PlayerLimbs limbs, PlayerContext context)
        {
            _limbs = limbs;
            _context = context;

            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;
        }
        private void Update()
        {
            if (_limbs == null || _cameraTransform == null)
                return;
            IInteractable found = null;
            InteractableHighlight highlight = null;
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _interactionRadius, _interactableLayer, QueryTriggerInteraction.Collide))
            {
                if (!hit.collider.transform.IsChildOf(transform))
                {
                    InteractableProxy proxy = hit.collider.GetComponentInParent<InteractableProxy>();
                    if (proxy != null && proxy.Interactable != null && proxy.Interactable.CanInteract(_limbs))
                    {
                        found = proxy.Interactable;
                        highlight = proxy.GetComponent<InteractableHighlight>();
                    }
                }
            }
            if (highlight != _highlight)
            {
                _highlight?.SetHighlighted(false);
                _highlight = highlight;
                _highlight?.SetHighlighted(true);
            }
            if (found != _currentInteractable)
            {
                _currentInteractable = found;
                OnInteractableChanged?.Invoke(_currentInteractable);
            }
        }
        public bool TryInteract()
        {
            if (_currentInteractable != null && _currentInteractable.CanInteract(_limbs))
            {
                _currentInteractable.Interact(_context);
                return true;
            }
            return false;
        }
        private void OnDisable()
        {
            _highlight?.SetHighlighted(false);
            _highlight = null;
            if (_currentInteractable != null)
            {
                _currentInteractable = null;
                OnInteractableChanged?.Invoke(null);
            }
        }
    }
}