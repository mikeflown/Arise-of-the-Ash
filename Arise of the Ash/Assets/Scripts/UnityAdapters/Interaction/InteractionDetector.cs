using Core.Interaction;
using Core.Player;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField] private float _interactionRadius = 2f;
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private Transform _cameraTransform;
        private PlayerLimbs _limbs;
        private PlayerContext _context;
        private IInteractable _currentInteractable;
        public IInteractable CurrentInteractable => _currentInteractable;
        public event System.Action<IInteractable> OnInteractableChanged;
        public void Init(PlayerLimbs limbs, PlayerContext context)
        {
            _limbs = limbs;
            _context = context;
        }
        private void Update()
        {
            if (_limbs == null) return;
            Collider[] colliders = Physics.OverlapSphere(transform.position, _interactionRadius, _interactableLayer);
            IInteractable closest = null;
            float closestDistance = float.MaxValue;
            foreach (var col in colliders)
            {
                var proxy = col.GetComponent<InteractableProxy>();
                if (proxy != null && proxy.Interactable.CanInteract(_limbs))
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = proxy.Interactable;
                    }
                }
            }
            if (closest != _currentInteractable)
            {
                _currentInteractable = closest;
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
    }
}