using Core.Interaction;
using TMPro;
using UnityEngine;

namespace UnityAdapters.Interaction
{
    public class InteractionPromptView : MonoBehaviour
    {
        [SerializeField] private InteractionDetector _detector;
        [SerializeField] private TMP_Text _label;
        private void OnEnable()
        {
            if (_detector != null)
                _detector.OnInteractableChanged += Show;
        }
        private void OnDisable()
        {
            if (_detector != null)
                _detector.OnInteractableChanged -= Show;
        }
        private void Show(IInteractable interactable)
        {
            if (_label == null)
                return;
            bool visible = interactable != null && !string.IsNullOrEmpty(interactable.InteractionPrompt);
            _label.gameObject.SetActive(visible);
            if (visible)
                _label.text = interactable.InteractionPrompt;
        }
    }
}