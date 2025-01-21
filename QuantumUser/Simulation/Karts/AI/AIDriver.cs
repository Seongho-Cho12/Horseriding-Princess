using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe partial struct AIDriver
    {
        /// <summary>
        /// AI 주행상태 업데이트
        /// </summary>
        /// <param name="frame"> 현재 게임 프레임 </param>
        /// <param name="filter"> AI 차량의 ECS 필터 (위치, 속도 등) </param>
        /// <param name="input"> AI의 조작 입력 (방향, 드리프트, 부스터 등) </param>
        public void Update(Frame frame, KartSystem.Filter filter, ref Input input)
        {

            // AI 차량의 현재 위치와 다음 위치를 업데이트
            AIDriverSettings settings = frame.FindAsset(SettingsRef);

            FP distance = FPVector3.Distance(TargetLocation, filter.Transform3D->Position);
            FP distanceNext = FPVector3.Distance(TargetLocation, NextTargetLocation);
            FP predictionAmount = FPMath.InverseLerp(distance, distanceNext, settings.PredictionRange);

            FPVector3 toWaypoint = TargetLocation - filter.Transform3D->Position;
            FPVector3 toNextWaypoint = NextTargetLocation - filter.Transform3D->Position;

            // 정지 상태 확인 후 리스폰
            FPVector3 flatVelocity = filter.Kart->Velocity;
            flatVelocity.Y = 0;
            toWaypoint.Y = 0;
            toNextWaypoint.Y = 0;

            StationaryTime = flatVelocity.SqrMagnitude < FP._7 ? StationaryTime + frame.DeltaTime : 0;

            if (StationaryTime > 5)
            {
                input.Respawn = true;
                StationaryTime = 0;
            }

            //Draw.Ray(filter.Transform3D->Position, toWaypoint, ColorRGBA.Green);
            //Draw.Ray(filter.Transform3D->Position, toNextWaypoint, ColorRGBA.Blue);

            FPVector3 targetDirection = FPVector3.Lerp(toWaypoint, toNextWaypoint, predictionAmount).Normalized;

            //Draw.Ray(filter.Transform3D->Position, targetDirection * 5, ColorRGBA.Magenta);

            // 방향 및 드리프트 결정 로직
            FP turnAngle = FPVector3.Angle(toWaypoint, toNextWaypoint);
            FP signedAngle = FPVector3.SignedAngle(targetDirection, flatVelocity, FPVector3.Up);
            FP desiredDirection = FPMath.Sign(signedAngle);

            // 드리프트 조건 확인
            // AI가 드리프트를 시작할지 종료할지 결정
            if (frame.Unsafe.TryGetPointer(filter.Entity, out Drifting* drifting))
            {
                bool shouldStartDrift = turnAngle >= settings.DriftingAngle && !drifting->IsDrifting;
                bool shouldEndDrift = turnAngle < settings.DriftingStopAngle && drifting->IsDrifting;

                input.Drift = !drifting->IsDrifting && shouldStartDrift || drifting->IsDrifting && shouldEndDrift;
            }

            // 드리프트 중이 아닌 경우, 조향 강도 결정
            FP steeringStrength = settings.SteeringCurve.Evaluate(FPMath.Abs(signedAngle));

            input.Direction = new FPVector2(FPMath.Clamp(-desiredDirection * steeringStrength, -1, 1), 1);
        }

        /// <summary>
        /// AI 타겟 위치 업데이트
        /// AI의 현재 목표 및 다음 목표 위치를 업데이트.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="entity"></param>
        public void UpdateTarget(Frame frame, EntityRef entity)
        {
            RaceTrack* raceTrack = frame.Unsafe.GetPointerSingleton<RaceTrack>();
            RaceProgress* raceProgress = frame.Unsafe.GetPointer<RaceProgress>(entity);

            AIDriverSettings settings = frame.FindAsset(SettingsRef);

            // 현재 체크포인트 목표 위치 가져옴
            // 난이도에 따라 목표 위치를 설정
            TargetLocation = raceTrack->GetCheckpointTargetPosition(frame, raceProgress->TargetCheckpointIndex, settings.Difficulty);

            //체크포인트가 마지막에 도달하면 처음으로 돌아감
            int nextIndex = raceProgress->TargetCheckpointIndex + 1;

            if (nextIndex >= raceTrack->GetCheckpoints(frame).Count)
            {
                nextIndex = 0;
            }

            NextTargetLocation = raceTrack->GetCheckpointTargetPosition(frame, nextIndex, settings.Difficulty);
        }
    }
}
