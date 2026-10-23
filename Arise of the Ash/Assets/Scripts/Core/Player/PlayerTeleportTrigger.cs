using UnityEngine;

namespace UnityAdapters.Interaction
{
    [RequireComponent(typeof(Collider))]
    public class PlayerTeleportTrigger : MonoBehaviour
    {
        [SerializeField] private Transform _destination;
        [SerializeField] private bool _useDestinationRotation = true;
        [SerializeField] private string _playerTag = "Player";
        [SerializeField] private float _cooldown = 0.5f;
        private float _lastTeleportTime = -10f;
        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.isTrigger = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (_destination == null)
                return;
            if (Time.time - _lastTeleportTime < _cooldown)
                return;
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc == null)
                cc = other.GetComponentInParent<CharacterController>();
            if (cc == null)
                return;
            if (!string.IsNullOrEmpty(_playerTag) && !other.CompareTag(_playerTag))
            {
                if (cc.transform != other.transform && !cc.CompareTag(_playerTag))
                {
                    if (!other.CompareTag(_playerTag) && !cc.CompareTag(_playerTag))
                        return;
                }
            }
            Teleport(cc.transform, cc);
            _lastTeleportTime = Time.time;
        }
        private void Teleport(Transform player, CharacterController cc)
        {
            if (cc != null)
                cc.enabled = false;
            player.position = _destination.position;
            if (_useDestinationRotation)
                player.rotation = _destination.rotation;
            if (cc != null)
                cc.enabled = true;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_destination == null)
                return;
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.8f);
            Gizmos.DrawWireSphere(transform.position, 0.35f);
            Gizmos.DrawLine(transform.position, _destination.position);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_destination.position, 0.4f);
            Gizmos.DrawLine(_destination.position, _destination.position + _destination.forward * 0.8f);
        }
#endif
    }
}