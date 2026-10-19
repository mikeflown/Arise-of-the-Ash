using UnityEngine;

namespace UnityAdapters.UI
{
    public class MenuHoverArrow : MonoBehaviour
    {
        [SerializeField] private RectTransform _arrow;
        [SerializeField] private float _leftOffset = 36f;
        private void Awake()
        {
            if (_arrow != null)
                _arrow.gameObject.SetActive(false);
        }
        public void Show(RectTransform target)
        {
            if (_arrow == null || target == null)
                return;
            _arrow.gameObject.SetActive(true);
            _arrow.position = target.position;
            _arrow.anchoredPosition += new Vector2(-target.rect.width * 0.5f - _leftOffset, 0f);
        }
        public void Hide(RectTransform target)
        {
            if (_arrow != null)
                _arrow.gameObject.SetActive(false);
        }
    }
}