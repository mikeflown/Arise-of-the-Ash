using Core.Interaction;
using Core.Player;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class HeavyDraggable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _prompt = "Тащить";
        [SerializeField] private float _mass = 80f;
        private Rigidbody _body;
        public string InteractionPrompt => _prompt;
        public bool IsBeingDragged { get; private set; }
        public Rigidbody Body => _body;
        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _body.mass = _mass;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _body.constraints = RigidbodyConstraints.FreezeRotation;
        }
        public bool CanInteract(PlayerLimbs limbs)
        {
            return !IsBeingDragged && limbs != null && limbs.HasBothArms;
        }
        public void Interact(PlayerContext context)
        {
        }
        public void BeginDrag()
        {
            IsBeingDragged = true;
            _body.linearDamping = 4f;
            _body.angularDamping = 8f;
        }
        public void EndDrag()
        {
            IsBeingDragged = false;
            _body.linearDamping = 0.5f;
            _body.angularDamping = 0.5f;
#if UNITY_6000_0_OR_NEWER
            Vector3 v = _body.linearVelocity;
            _body.linearVelocity = new Vector3(v.x * 0.3f, v.y, v.z * 0.3f);
#else
            Vector3 v = _body.velocity;
            _body.velocity = new Vector3(v.x * 0.3f, v.y, v.z * 0.3f);
#endif
        }
        public void ApplyDragForce(Vector3 worldDirection, float speed)
        {
            if (!IsBeingDragged)
                return;
            Vector3 flat = new Vector3(worldDirection.x, 0f, worldDirection.z);
            if (flat.sqrMagnitude < 0.001f)
            {
#if UNITY_6000_0_OR_NEWER
                Vector3 cur = _body.linearVelocity;
                _body.linearVelocity = new Vector3( Mathf.MoveTowards(cur.x, 0f, 15f * Time.fixedDeltaTime), cur.y, Mathf.MoveTowards(cur.z, 0f, 15f * Time.fixedDeltaTime));
#else
                Vector3 cur = _body.velocity;
                _body.velocity = new Vector3(
                    Mathf.MoveTowards(cur.x, 0f, 15f * Time.fixedDeltaTime), cur.y, Mathf.MoveTowards(cur.z, 0f, 15f * Time.fixedDeltaTime));
#endif
                return;
            }
            flat.Normalize();
            Vector3 targetVel = flat * speed;
#if UNITY_6000_0_OR_NEWER
            Vector3 current = _body.linearVelocity;
            _body.linearVelocity = new Vector3(Mathf.MoveTowards(current.x, targetVel.x, 20f * Time.fixedDeltaTime), current.y, Mathf.MoveTowards(current.z, targetVel.z, 20f * Time.fixedDeltaTime));
#else
            Vector3 current = _body.velocity;
            _body.velocity = new Vector3(Mathf.MoveTowards(current.x, targetVel.x, 20f * Time.fixedDeltaTime), current.y, Mathf.MoveTowards(current.z, targetVel.z, 20f * Time.fixedDeltaTime));
#endif
        }
    }
}