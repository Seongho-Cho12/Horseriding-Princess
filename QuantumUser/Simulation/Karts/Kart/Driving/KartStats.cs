using Photon.Deterministic;
using UnityEngine.Serialization;

namespace Quantum
{
    public unsafe partial class KartStats : AssetObject
    {
        public FPVector3 overlapShapeSize;
        public FPVector3 overlapShapeOffset;

        public LayerMask overlapLayerMask;

        public FPAnimationCurve acceleration;
        public FPAnimationCurve turningRate;
        public FPAnimationCurve frictionEffect;
        public FP maxSpeed;
        public FP minThrottle;
        public FP gravity;
        public FP drag;
        public FP rotationCorrectionRate;
        public FP rotationSmoothingThreshold;
        public FP maxTilt;
        public FP groundDistance;
        // 스태미나 관련 속성 (말 유형별 설정)
        public FP StaminaConsumption;
        public FP CarrotRecovery;
        public FP NaturalRecovery;
        public FP BoostMultiplier;

        public FP maxStamina;      // 최대 스태미나
        public FP corneringStaminaMultiplier;  // 코너링 스태미나 소모 배율
        public FP weakBoostStaminaCost;       // 약한 부스트 스태미나 고정 소모량
        public FP strongBoostStaminaCost;     // 강한 부스트 스태미나 고정 소모량
    }
}