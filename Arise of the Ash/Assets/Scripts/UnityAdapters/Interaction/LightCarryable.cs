using Core.Interaction;
using Core.Player;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class LightCarryable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _prompt = "Взять";
        private Rigidbody _body;
        private Collider[] _colliders;
        private int _originalLayer;
        public string InteractionPrompt => _prompt;
        public bool IsHeld { get; private set; }
        public Rigidbody Body => _body;
        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>();
            _originalLayer = gameObject.layer;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        public bool CanInteract(PlayerLimbs limbs)
        {
            return !IsHeld && limbs != null && limbs.HasAnyArm;
        }
        public void Interact(PlayerContext context)
        {
        }
        public void SetHeld(bool held)
        {
            IsHeld = held;
            int layer = held ? LayerMask.NameToLayer("Held") : _originalLayer;
            if (layer < 0)
                layer = _originalLayer;
            SetLayer(layer);
            if (held)
            {
                if (!_body.isKinematic)
                    SetVelocity(Vector3.zero);

                _body.interpolation = RigidbodyInterpolation.None;
                _body.isKinematic = true;
                _body.useGravity = false;
            }
            else
            {
                _body.isKinematic = false;
                _body.useGravity = true;
                _body.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
        public void Throw(Vector3 velocity)
        {
            SetHeld(false);
            SetVelocity(velocity);
        }
        private void SetLayer(int layer)
        {
            gameObject.layer = layer;
            foreach (Collider col in _colliders)
                col.gameObject.layer = layer;
        }
        private void SetVelocity(Vector3 velocity)
        {
#if UNITY_6000_0_OR_NEWER
            _body.linearVelocity = velocity;
#else
            _body.velocity = velocity;
#endif
            _body.angularVelocity = Vector3.zero;
        }
    }
}