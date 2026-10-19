using UnityEngine;

namespace UnityAdapters.Interaction
{
    public class InteractableHighlight : MonoBehaviour
    {
        [SerializeField] private GameObject _highlight;
        private void Awake()
        {
            SetHighlighted(false);
        }
        public void Bind(GameObject highlight)
        {
            _highlight = highlight;
            SetHighlighted(false);
        }
        public void SetHighlighted(bool value)
        {
            if (_highlight != null)
                _highlight.SetActive(value);
        }
    }
}