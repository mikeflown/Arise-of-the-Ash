using System;
using Core.Player;
using UnityEngine;

namespace Core.Interaction
{
    public class LimbPickup : IInteractable
    {
        private readonly LimbType _limbType;
        private readonly GameObject _gameObjectToDestroy;
        private readonly Action<LimbType> _onPickedUp;
        public string InteractionPrompt => $"Интегрировать конечность E";
        public LimbPickup(LimbType limbType, GameObject gameObjectToDestroy, Action<LimbType> onPickedUp = null)
        {
            _limbType = limbType;
            _gameObjectToDestroy = gameObjectToDestroy;
            _onPickedUp = onPickedUp;
        }
        public bool CanInteract(PlayerLimbs limbs) => !limbs.Has(_limbType);
        public void Interact(PlayerContext context)
        {
            context.Limbs.Add(_limbType);
            _onPickedUp?.Invoke(_limbType);

            if (_gameObjectToDestroy != null)
            {
                UnityEngine.Object.Destroy(_gameObjectToDestroy);
            }
        }
    }
}