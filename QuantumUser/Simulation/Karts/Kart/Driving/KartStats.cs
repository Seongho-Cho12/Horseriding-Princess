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
        // 스태미나 관련 추가
        // 스태미나 관련 속성 (말 유형별 설정)
        public FP CarrotLoverStaminaConsumption = FP.FromFloat_UNSAFE(2.0f);
        public FP PaceMakerStaminaConsumption = FP.FromFloat_UNSAFE(1.5f);
        public FP SpeedRacerStaminaConsumption = FP.FromFloat_UNSAFE(2.0f);

        public FP CarrotLoverCarrotRecovery = FP.FromFloat_UNSAFE(20.0f);
        public FP PaceMakerCarrotRecovery = FP.FromFloat_UNSAFE(15.0f);
        public FP SpeedRacerCarrotRecovery = FP.FromFloat_UNSAFE(15.0f);

        public FP CarrotLoverBoostMultiplier = FP.FromFloat_UNSAFE(1.5f);
        public FP PaceMakerBoostMultiplier = FP.FromFloat_UNSAFE(1.5f);
        public FP SpeedRacerBoostMultiplier = FP.FromFloat_UNSAFE(2.0f);

        public FP maxStamina = FP.FromFloat_UNSAFE(200.0f);      // 최대 스태미나
        public FP corneringStaminaMultiplier = FP.FromFloat_UNSAFE(1.5f);  // 코너링 스태미나 소모 배율
        public FP weakBoostStaminaCost = FP.FromFloat_UNSAFE(10.0f);       // 약한 부스트 스태미나 고정 소모량
        public FP strongBoostStaminaCost = FP.FromFloat_UNSAFE(25.0f);     // 강한 부스트 스태미나 고정 소모량
    }
}