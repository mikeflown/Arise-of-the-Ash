using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAdapters.UI
{
    public class MenuBackgroundSwitcher : MonoBehaviour
    {
        [Header("Backgrounds")]
        [SerializeField] private Image _backgroundA;
        [SerializeField] private Image _backgroundB;
        [Header("Blink Sequence")]
        [SerializeField] private int _blinksInChain = 4;
        [SerializeField] private float _blinkInterval = 0.12f;
        [Header("Calm Period")]
        [SerializeField] private float _calmDuration = 4.5f;
        [SerializeField] private bool _showADuringCalm = true;
        [Header("Start")]
        [SerializeField] private float _initialDelay = 1f;
        [SerializeField] private bool _playOnEnable = true;
        public event Action OnChainStarted;
        public event Action OnChainFinished;
        public event Action OnCalmStarted;
        private Coroutine _routine;
        private bool _showingA;
        private void Awake()
        {
            if (_backgroundA == null || _backgroundB == null)
            {
                Debug.LogError($"{nameof(MenuBackgroundSwitcher)}: оба фона должны быть назначены.", this);
                enabled = false;
                return;
            }
            SetBackground(_showADuringCalm);
        }
        private void OnEnable()
        {
            if (_playOnEnable)
                StartSwitching();
        }
        private void OnDisable()
        {
            StopSwitching();
        }
        public void StartSwitching()
        {
            if (_routine != null)
                return;
            _routine = StartCoroutine(MainLoop());
        }
        public void StopSwitching()
        {
            if (_routine == null)
                return;
            StopCoroutine(_routine);
            _routine = null;
            SetBackground(_showADuringCalm);
        }
        private IEnumerator MainLoop()
        {
            if (_initialDelay > 0f)
                yield return new WaitForSecondsRealtime(_initialDelay);
            while (true)
            {
                OnChainStarted?.Invoke();
                for (int i = 0; i < _blinksInChain; i++)
                {
                    SetBackground(!_showingA);
                    yield return new WaitForSecondsRealtime(_blinkInterval);
                }
                OnChainFinished?.Invoke();
                SetBackground(_showADuringCalm);
                OnCalmStarted?.Invoke();
                if (_calmDuration > 0f)
                    yield return new WaitForSecondsRealtime(_calmDuration);
            }
        }
        private void SetBackground(bool showA)
        {
            _showingA = showA;
            _backgroundA.enabled = showA;
            _backgroundB.enabled = !showA;
        }
#if UNITY_EDITOR
        [ContextMenu("Force One Chain")]
        private void ForceOneChain()
        {
            if (!Application.isPlaying) return;
            StartCoroutine(OneChainPreview());
        }
        private IEnumerator OneChainPreview()
        {
            for (int i = 0; i < _blinksInChain; i++)
            {
                SetBackground(!_showingA);
                yield return new WaitForSecondsRealtime(_blinkInterval);
            }
            SetBackground(_showADuringCalm);
        }
#endif
    }
}