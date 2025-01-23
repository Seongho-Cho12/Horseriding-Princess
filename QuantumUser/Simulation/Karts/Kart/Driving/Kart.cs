using Photon.Deterministic;
using Quantum.Physics3D;
using Quantum.Prototypes;
using UnityEngine;
using Plane = Photon.Deterministic.Plane;

namespace Quantum
{
    public unsafe partial struct Kart
    {
        public bool IsGrounded => GroundedWheels > 1;
        public bool IsOffroad => OffroadWheels >= 4;

        public void Update(Frame frame, KartSystem.Filter filter)
        {
            Transform3D* transform = filter.Transform3D;
            Wheels* wheelComp = filter.Wheels;
            Drifting* drifting = filter.Drifting;

            var stats = frame.FindAsset(StatsAsset);

            FPVector3 up = transform->Up;
            FPVector3 targetUp = up;
            FPVector3 averagePoint = transform->Position + FPVector3.Up * stats.groundDistance;

            GroundedWheels = 0;
            OffroadWheels = 0;
            SurfaceFrictionMultiplier = 0;
            SurfaceSpeedMultiplier = 1;
            SurfaceHandlingMultiplier = 1;

            for (int i = 0; i < wheelComp->WheelStatuses.Length; i++)
            {
                WheelStatus* status = wheelComp->WheelStatuses.GetPointer(i);

                if (status->Grounded)
                {
                    targetUp += status->HitNormal;
                    averagePoint += status->HitPoint;
                    GroundedWheels++;

                    DrivingSurface surface = frame.FindAsset(status->HitSurface);
                    SurfaceFrictionMultiplier += surface.FrictionMultiplier;
                    SurfaceSpeedMultiplier += surface.SpeedMultiplier;
                    SurfaceHandlingMultiplier += surface.HandlingMultiplier;

                    if (surface.Offroad)
                    {
                        OffroadWheels++;
                    }
                }
            }

            averagePoint /= (GroundedWheels + 1);
            targetUp /= (GroundedWheels + 1);

            SurfaceFrictionMultiplier /= wheelComp->WheelStatuses.Length;
            SurfaceSpeedMultiplier /= (GroundedWheels + 1);
            SurfaceHandlingMultiplier /= (GroundedWheels + 1);

            AirTime = !IsGrounded ? AirTime + frame.DeltaTime : 0;

            ApplyOverlapCollision(frame, filter.Entity);

            // Cap ground normal to a maximum tilt angle
            FP tiltAngle = FPVector3.Angle(targetUp, FPVector3.Up);
            if (tiltAngle > stats.maxTilt)
            {
                targetUp = FPVector3.Lerp(FPVector3.Up, targetUp, FPMath.InverseLerp(0, tiltAngle, stats.maxTilt));
            }

            FPVector3 newPosition = transform->Position + CollisionPositionCompensation + Velocity * frame.DeltaTime;

            FPVector3 targetForward;

            if (IsGrounded)
            {
                Plane avgGround = new(averagePoint, targetUp);
                FP distance = avgGround.SignedDistanceTo(newPosition);

                //Remove gravity if close to ground
                if (distance < stats.groundDistance)
                {
                    FP velocityToGround = FPVector3.Dot(Velocity, targetUp);

                    if (velocityToGround < FP._0)
                    {
                        Velocity -= targetUp * velocityToGround;
                    }

                    newPosition += (stats.groundDistance - distance) * targetUp;
                }

                targetForward = FPVector3.Cross(transform->Right, targetUp);
            }
            else
            {
                targetUp = FPVector3.MoveTowards(targetUp, FPVector3.Up,
                    stats.rotationCorrectionRate * frame.DeltaTime);
                targetForward = FPVector3.Cross(transform->Right, targetUp);
            }

            // Draw.Line(transform->Position, transform->Position + Velocity, ColorRGBA.Red);
            // Draw.Ray(transform->Position + FPVector3.Up, targetUp);
            // Draw.Line(transform->Position + FPVector3.Up, transform->Position + FPVector3.Up + targetForward);

            FPQuaternion lookRotation = FPQuaternion.LookRotation(targetForward, targetUp);

            FP angle = FPQuaternion.Angle(lookRotation, transform->Rotation);

            FP smoothing = FPMath.Clamp01(angle / stats.rotationSmoothingThreshold);
            FP wheelMultiplier = FP._0_25 + (FP._0_75 * (GroundedWheels / 4));

            FP lerp = FP._0_50 * smoothing * wheelMultiplier;

            lookRotation = FPQuaternion.Slerp(transform->Rotation, lookRotation, lerp);

            bool hasInput = FPMath.Abs(filter.KartInput->GetTotalSteering()) > FP._0_05;
            if (hasInput)
            {
                lookRotation *= FPQuaternion.AngleAxis(
                    filter.KartInput->GetTotalSteering()
                    * SurfaceHandlingMultiplier
                    * FPMath.Sign(FPVector3.Dot(Velocity, transform->Forward))
                    * GetTurningRate(frame, drifting)
                    * frame.DeltaTime,
                    FPVector3.Up
                );
            }

            // To use Quantum physics enable this velocity assignment and disable Overlap collisions
            //body->Velocity = velocity;
            transform->Position = newPosition;
            transform->Rotation = lookRotation;

            SidewaysSpeedSqr = FPVector3.Project(Velocity, transform->Right).SqrMagnitude;

            // Apply physics after position change so the next frame's broadphase queries have an accurate predicted position

            ApplyExternalForce(frame);
            ApplyGravity(frame, targetUp);
            ApplyFriction(frame, transform);
            ApplyDrag(frame);
            LimitVelocity(frame, filter.Entity);
            // 스태미나 관련
            ApplyStaminaConsumption(frame, filter.KartInput, filter.Entity);
            // 부스트 강화 관련
            ApplyBoostEnhancement(frame, filter.KartInput, ref this);
            // 슬립스트림 관련
            CheckSlipstream(frame, ref this, filter);
            // 최종 랩 부스트 관련
            if (frame.Unsafe.TryGetPointer(filter.Entity, out RaceProgress* raceProgress))
            {
                CheckFinalLapBoost(frame, ref this, raceProgress);
            }

            ExternalForce = FPVector3.Zero;
        }

        public Shape3D GetOverlapShape(Frame frame)
        {
            KartStats stats = frame.FindAsset(StatsAsset);

            return Shape3D.CreateBox(stats.overlapShapeSize, stats.overlapShapeOffset);
        }

        private void ApplyExternalForce(Frame frame)
        {
            Velocity += ExternalForce * frame.DeltaTime;
        }

        private void ApplyOverlapCollision(Frame frame, EntityRef entity)
        {
            FP collisionSetback = FP._0;
            FPVector3 bounce = new();

            var hits = frame.Physics3D.GetQueryHits(OverlapQuery);
            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    Hit3D overlap = hits[i];

                    if (overlap.Entity == entity) { continue; }

                    if (frame.Unsafe.TryGetPointer(entity, out Transform3D* transform))
                    {
                        Draw.Ray(transform->Position + FPVector3.Up * 2, overlap.Normal, ColorRGBA.Red);
                    }

                    // don't pop out on the other side of thin obstacles
                    FP dot = FPVector3.Dot(Velocity.Normalized, overlap.Normal);
                    if (dot > 0) { continue; }

                    // don't sink further in the ground
                    if (FPVector3.Dot(overlap.Normal, FPVector3.Down) > FP._0_50) { continue; }

                    bounce += overlap.Normal;

                    FPVector3 flatVelocity = Velocity;
                    flatVelocity.Y = 0;

                    Velocity -= dot * dot * flatVelocity * FP._0_50;

                    collisionSetback = FPMath.Max(collisionSetback, overlap.OverlapPenetration);
                }

                bounce = bounce.Normalized;
            }

            bounce.Y = 0;

            CollisionPositionCompensation = bounce * collisionSetback;
        }

        private void ApplyGravity(Frame frame, FPVector3 up)
        {
            KartStats stats = frame.FindAsset(StatsAsset);
            Velocity += up * stats.gravity * frame.DeltaTime * (IsGrounded ? FP._0_10 : FP._1);
        }

        private void ApplyDrag(Frame frame)
        {
            KartStats stats = frame.FindAsset(StatsAsset);
            Velocity -= Velocity * stats.drag * frame.DeltaTime * (IsGrounded ? FP._1 : FP._0_10);
        }

        private void ApplyFriction(Frame frame, Transform3D* t)
        {
            if (!IsGrounded)
            {
                return;
            }

            KartStats stats = frame.FindAsset(StatsAsset);

            FPVector3 frictionDirection = t->Right;
            FP frictionAmount = FPVector3.Dot(Velocity, frictionDirection);
            FP effect = stats.frictionEffect.Evaluate(GetNormalizedSpeed(frame));
            FPVector3 friction = frictionDirection * (frictionAmount * SurfaceFrictionMultiplier * effect);

            Velocity -= friction;
        }

        /// <summary>
        /// Returns a number between 0-1, 0 being stationary and 1 being max speed the car allows
        /// </summary>
        public FP GetNormalizedSpeed(Frame f)
        {
            FP maxSpeed = f.FindAsset(StatsAsset).maxSpeed;
            return FPMath.InverseLerp(0, maxSpeed * maxSpeed, Velocity.SqrMagnitude);
        }

        private void LimitVelocity(Frame frame, EntityRef entity)
        {
            // no hard clamping so kart doesn't suddenly "hit a wall" when a boost ends
            Velocity = FPVector3.MoveTowards(
                Velocity,
                FPVector3.ClampMagnitude(Velocity, GetMaxSpeed(frame, entity)),
                FP._0_10
            );
        }

        public FP GetAcceleration(Frame frame, EntityRef entity)
        {
            KartStats stats = frame.FindAsset(StatsAsset);
            FP bonus = 0;

            if (frame.Unsafe.TryGetPointer(entity, out KartBoost* kartBoost) && kartBoost->CurrentBoost != null)
            {
                BoostConfig config = frame.FindAsset(kartBoost->CurrentBoost);
                bonus += config.AccelerationBonus;
            }

            return stats.acceleration.Evaluate(GetNormalizedSpeed(frame)) * SurfaceSpeedMultiplier + bonus;
        }

        public FP GetMaxSpeed(Frame frame, EntityRef entity)
        {
            KartStats stats = frame.FindAsset(StatsAsset);
            FP bonus = 1;

            if (frame.Unsafe.TryGetPointer(entity, out KartBoost* kartBoost) && kartBoost->CurrentBoost != null)
            {
                BoostConfig config = frame.FindAsset(kartBoost->CurrentBoost);
                bonus += config.MaxSpeedBonus;
            }

            return stats.maxSpeed * SurfaceSpeedMultiplier + bonus;
        }

        public FP GetTurningRate(Frame frame, Drifting* drifting)
        {
            KartStats stats = frame.FindAsset(StatsAsset);

            return stats.turningRate.Evaluate(drifting->Direction != 0 ? 1 : GetNormalizedSpeed(frame));
        }
        private void ApplyStaminaConsumption(Frame frame, KartInput* input, EntityRef entity)
        {
            if (frame.Unsafe.TryGetPointer<Kart>(entity, out var kart))
            {
                KartStats stats = frame.FindAsset(StatsAsset);

                // 스태미나 감소 속도 적용
                FP staminaConsumption = stats.StaminaConsumption * frame.DeltaTime;

                // 조향 시 추가 스태미나 소모
                if (FPMath.Abs(input->Steering) > FP._0_05)
                {
                    staminaConsumption *= stats.corneringStaminaMultiplier;
                }

                // 부스트 입력 처리
                if (input->weakBoostPressed)
                {
                    if (kart->Stamina >= stats.weakBoostStaminaCost)
                    {
                        // 스태미나 소모 및 속도 증가 적용
                        kart->Stamina -= stats.weakBoostStaminaCost;
                        if (kart->BoostMultiplier <= FP._1)
                        {
                            kart->BoostMultiplier = FP._1;
                        }
                        kart->Velocity += kart->Velocity.Normalized * stats.BoostMultiplier * kart->BoostMultiplier;
                        kart->Velocity = FPVector3.ClampMagnitude(kart->Velocity, kart->MaxSpeed);

                        // 부스트 적용 후 입력 초기화
                        input->weakBoostPressed = false;
                    }
                }
                
                // 속도가 특정 값 이상이면 천천히 스태미나 소모
                if (kart->Velocity.SqrMagnitude > FP.FromFloat_UNSAFE(150.0f))
                {
                    kart->Stamina -= staminaConsumption;
                }
                else
                {
                    // 특정 속도 이하일 때 스태미나 회복
                    kart->Stamina += stats.NaturalRecovery * frame.DeltaTime;
                    kart->Stamina = FPMath.Min(kart->Stamina, stats.maxStamina);
                }

                // 최소 스태미나 보장
                kart->Stamina = FPMath.Max(kart->Stamina, FP._0);

                frame.Set(entity, *kart);
            }
        }

        private void ApplyBoostEnhancement(Frame frame, KartInput* input, ref Kart kart)
        {
            KartStats stats = frame.FindAsset(kart.StatsAsset);

            if (input->boostEnhancementPressed && kart.Stamina >= stats.boostEnhancementStaminaCost)
            {
                kart.Stamina -= stats.boostEnhancementStaminaCost;
                kart.BoostMultiplier = stats.boostEnhancementMultiplier;
                kart.BoostEnhancementTimer = stats.boostEnhancementDuration;
                Debug.Log($"Boost Enhancement Activated! New Multiplier: {kart.BoostMultiplier.AsFloat}");
            }

            // 부스트 강화 타이머 감소 처리
            if (kart.BoostEnhancementTimer > FP._0)
            {
                kart.BoostEnhancementTimer -= frame.DeltaTime;
                if (kart.BoostEnhancementTimer <= FP._0)
                {
                    kart.BoostMultiplier = FP._1;  // 기본 부스트 배율로 복구
                }
            }
        }

        private void CheckSlipstream(Frame frame, ref Kart kart, KartSystem.Filter filter)
        {
            if (!frame.Unsafe.TryGetPointer<Transform3D>(filter.Entity, out Transform3D* kartTransform))
                return;

            FPVector3 kartPosition = kartTransform->Position;

            var allKarts = frame.Filter<Kart>();

            EntityRef otherKartEntity;
            Kart otherKart;

            while (allKarts.Next(out otherKartEntity, out otherKart))
            {
                if (otherKartEntity == filter.Entity) continue;

                if (frame.Unsafe.TryGetPointer<Transform3D>(otherKartEntity, out Transform3D* otherKartTransform))
                {
                    FPVector3 directionToOther = otherKartTransform->Position - kartPosition;
                    FP distance = directionToOther.Magnitude;

                    if (distance < FP.FromFloat_UNSAFE(5.0f))  // 슬립스트림 거리 조건
                    {
                        FP angle = FPVector3.Angle(otherKartTransform->Forward, directionToOther.Normalized);
                        if (angle < FP.FromFloat_UNSAFE(30.0f))  // 방향 각도 조건
                        {
                            kart.isSlipstreaming = true;
                            kart.slipstreamTimer += frame.DeltaTime;

                            KartStats stats = frame.FindAsset(kart.StatsAsset);

                            // 슬립스트림 효과 적용
                            kart.Stamina += stats.slipstreamStaminaRecovery * frame.DeltaTime;

                            // 슬립스트림 시 드래그 0 적용
                            stats.drag = FP._0;

                            return;
                        }
                    }
                }
            }

            kart.isSlipstreaming = false;
            kart.slipstreamTimer = FP._0;
        }

        private void CheckFinalLapBoost(Frame frame, ref Kart kart, RaceProgress* raceProgress)
        {
            KartStats stats = frame.FindAsset(kart.StatsAsset);

            // 마지막 랩이며, 남은 체크포인트가 2개 이하인지 확인
            if (raceProgress->CurrentLap == raceProgress->TotalLaps - 1 &&
                raceProgress->TotalCheckpoints - raceProgress->TargetCheckpointIndex <= 2)
            {
                kart.MaxSpeed = stats.maxSpeed * 100;  // 최고속도 100배 증가
                Debug.Log("Final Lap Boost Activated! Run!");
            }
            else
            {
                kart.MaxSpeed = stats.maxSpeed;  // 정상 속도 유지
            }
        }
    }
}