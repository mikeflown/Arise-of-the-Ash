namespace Core.Interaction
{
    public interface IInteractable
    {
        bool CanInteract(Player.PlayerLimbs limbs);
        void Interact(PlayerContext context);
        string InteractionPrompt { get; }
    }
}