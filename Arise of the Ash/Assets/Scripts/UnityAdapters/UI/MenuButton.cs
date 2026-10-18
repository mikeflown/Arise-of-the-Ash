using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityAdapters.UI
{
    public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _hoverSprite;
        [SerializeField] private MenuAudio _audio;
        private void Awake()
        {
            if (_image == null)
                _image = GetComponent<Image>();

            SetNormal();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_image != null && _hoverSprite != null)
                _image.sprite = _hoverSprite;

            _audio?.PlayHover();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            SetNormal();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _audio?.PlayClick();
        }
        private void SetNormal()
        {
            if (_image != null && _normalSprite != null)
                _image.sprite = _normalSprite;
        }
    }
}