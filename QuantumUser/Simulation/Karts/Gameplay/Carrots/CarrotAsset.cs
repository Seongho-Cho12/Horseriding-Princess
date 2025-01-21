using System;
using Photon.Deterministic;

namespace Quantum
{
    public abstract partial class CarrotAsset: AssetObject
    {
        public FP StaminaBoostAmount = FP.FromFloat_UNSAFE(20.0f);
    }
}
