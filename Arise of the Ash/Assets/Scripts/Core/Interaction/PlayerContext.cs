using Core.Player;
using UnityEngine;

namespace Core.Interaction
{
    public class PlayerContext
    {
        public PlayerLimbs Limbs { get; }
        public Transform Transform { get; }
        public CharacterController CharacterController { get; }
        public PlayerContext(PlayerLimbs limbs, Transform transform, CharacterController controller)
        {
            Limbs = limbs;
            Transform = transform;
            CharacterController = controller;
        }
    }
}