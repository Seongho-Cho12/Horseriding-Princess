﻿using Photon.Deterministic;
using Quantum.Core;
using UnityEngine;

namespace Quantum
{
    public unsafe partial struct KartInput
    {
        public void Update(Frame frame, Input input)
        {
            FP steeringDiff = input.Direction.X - PreviousSteering;

            if (FPMath.Abs(steeringDiff) > FP._0_10)
            {
                Steering += FPMath.Sign(steeringDiff) * FP._0_10;
            }
            else
            {
                Steering = input.Direction.X;
            }

            // 부스트 입력이 눌릴 때 단 한 번만 실행되도록 처리
            if (input.Direction.Y > FP._0 && !PreviousBoostPressed)
            {
                weakBoostPressed = true;
            }
            else
            {
                weakBoostPressed = false;
            }

            if (input.BoostEnhancement && !PreviousBoostEnhancementPressed)
            {
                boostEnhancementPressed = true;
            }
            else
            {
                boostEnhancementPressed = false;
            }

            PreviousBoostEnhancementPressed = input.BoostEnhancementPressed;

            // 부스트 입력 상태 갱신
            PreviousBoostPressed = input.Direction.Y > FP._0;

            Drifting = input.Drift;

            FP steeringMagnitude = FPMath.Abs(Steering);

            SameSteeringTime = FPMath.Sign(Steering) == FPMath.Sign(PreviousSteering) 
                ? SameSteeringTime + frame.DeltaTime 
                : 0;

            NoSteeringTime = FPMath.Abs(steeringMagnitude) < FP._0_05 
                ? NoSteeringTime + frame.DeltaTime 
                : 0;

            DriftingInputTime = Drifting && PreviousDrifting 
                ? DriftingInputTime + frame.DeltaTime 
                : 0;

            PreviousSteering = Steering;
            PreviousDrifting = input.Drift.IsDown;
        }

        public FP GetTotalSteering()
        {
            return Steering + SteeringOffset;
        }
    }
}
