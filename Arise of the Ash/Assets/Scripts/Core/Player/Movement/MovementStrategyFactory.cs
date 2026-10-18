using Configs;
using Core.Player;

namespace Core.Player.Movement
{
    public class MovementStrategyFactory
    {
        private readonly MovementConfig _config;
        public MovementStrategyFactory(MovementConfig config)
        {
            _config = config;
        }
        public IMovementStrategy Create(PlayerLimbs limbs)
        {
            if (limbs.HasBothLegs)
                return new TwoLegsMovementStrategy(_config);
            return new OneLegMovementStrategy(_config);
        }
    }
}