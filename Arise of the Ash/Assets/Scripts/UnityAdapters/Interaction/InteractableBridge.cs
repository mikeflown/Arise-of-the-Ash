using Core.Interaction;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    public class InteractableBridge : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _interactableSource;
        private IInteractable _interactable;
        public IInteractable Interactable
        {
            get
            {
                if (_interactable == null)
                    Resolve();
                return _interactable;
            }
        }
        private void Awake()
        {
            Resolve();
        }
        private void Resolve()
        {
            if (_interactableSource is IInteractable direct)
            {
                _interactable = direct;
                return;
            }
            var behaviours = GetComponentsInParent<MonoBehaviour>();
            foreach (var b in behaviours)
            {
                if (b is IInteractable i)
                {
                    _interactable = i;
                    return;
                }
            }
        }
    }
}