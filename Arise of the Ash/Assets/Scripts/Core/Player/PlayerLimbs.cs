using System;
using System.Collections.Generic;

namespace Core.Player
{
    public class PlayerLimbs
    {
        public event Action<LimbType> OnLimbAdded;
        public event Action OnLimbsChanged;
        private readonly HashSet<LimbType> _limbs = new();
        public bool Has(LimbType limb) => _limbs.Contains(limb);
        public bool HasBothLegs => Has(LimbType.LeftLeg) && Has(LimbType.RightLeg);
        public bool HasAnyArm => Has(LimbType.LeftArm) || Has(LimbType.RightArm);
        public bool HasBothArms => Has(LimbType.LeftArm) && Has(LimbType.RightArm);
        public bool HasFullBody => HasBothLegs && HasBothArms;
        public int LimbCount => _limbs.Count;
        public void Add(LimbType limb)
        {
            if (_limbs.Add(limb))
            {
                OnLimbAdded?.Invoke(limb);
                OnLimbsChanged?.Invoke();
            }
        }
        public void AddRange(params LimbType[] limbs)
        {
            bool changed = false;
            foreach (var limb in limbs)
            {
                if (_limbs.Add(limb))
                {
                    OnLimbAdded?.Invoke(limb);
                    changed = true;
                }
            }
            if (changed)
                OnLimbsChanged?.Invoke();
        }
        public void Toggle(LimbType limb)
        {
            if (!_limbs.Remove(limb))
            {
                _limbs.Add(limb);
                OnLimbAdded?.Invoke(limb);
            }
            OnLimbsChanged?.Invoke();
        }
        public void DebugAddAll()
        {
            AddRange(LimbType.LeftLeg, LimbType.RightLeg, LimbType.LeftArm, LimbType.RightArm);
        }
    }
}