using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAdapters.UI
{
    public class SliderValueLabel : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private float _multiplier = 1f;
        [SerializeField] private string _format = "0";
        private void Awake()
        {
            if (_slider == null)
                _slider = GetComponentInParent<Slider>();
            _slider.onValueChanged.AddListener(Refresh);
            Refresh(_slider.value);
        }
        private void OnEnable()
        {
            if (_slider != null)
                Refresh(_slider.value);
        }
        private void Refresh(float value)
        {
            if (_label != null)
                _label.text = (value * _multiplier).ToString(_format);
        }
    }
}