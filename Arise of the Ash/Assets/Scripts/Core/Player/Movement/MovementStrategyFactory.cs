using Configs;

namespace Core.Player.Movement
{
    public class MovementStrategyFactory
    {
        private readonly MovementConfig _config;
        public MovementStrategyFactory(MovementConfig config)
        {
            _config = config;
        }
        public IMovementStrategy Create(Player.PlayerLimbs limbs)
        {
            if (limbs.HasBothLegs)
                return new TwoLegsMovementStrategy(_config);
            return new OneLegMovementStrategy(_config);
        }
    }
}