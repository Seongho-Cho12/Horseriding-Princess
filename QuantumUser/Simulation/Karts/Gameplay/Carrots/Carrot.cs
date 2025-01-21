using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct Carrot
    {
        // 프레임 업데이트 제거 (당근은 리스폰되지 않음)
        public void Update(Frame f, EntityRef entity)
        {
            // 당근은 업데이트 로직이 필요 없음
        }

        // 아이템 획득 시 호출, 당근 효과 적용 후 삭제
        public void OnPickup(Frame f, EntityRef thisEntity, EntityRef kartEntity)
        {
            if (PickedUp) { return; }

            // 아이템 속성 가져오기
            CarrotAsset asset = f.FindAsset(Asset);

            // 스태미나 증가 효과 적용
            if (f.Unsafe.TryGetPointer<Kart>(kartEntity, out var kart))
            {
                var stats = f.FindAsset(kart->StatsAsset);

                // 말의 유형에 따른 스태미나 회복량 적용
                FP staminaBoost = FP._0;
                switch (kart->SelectedHorseType)
                {
                    case HorseType.CarrotLover:
                        staminaBoost = stats.CarrotLoverCarrotRecovery;
                        break;
                    case HorseType.PaceMaker:
                        staminaBoost = stats.PaceMakerCarrotRecovery;
                        break;
                    case HorseType.SpeedRacer:
                        staminaBoost = stats.SpeedRacerCarrotRecovery;
                        break;
                }

                // 스태미나 증가 및 최대값 제한
                kart->Stamina = FPMath.Min(kart->Stamina + staminaBoost, stats.maxStamina);
                f.Set(kartEntity, *kart);
            }

            // 아이템 비활성화 후 제거 (리스폰 없음)
            TogglePickedUp(f, thisEntity, true);
            f.Destroy(thisEntity);  // 당근 제거

            // 아이템 획득 이벤트 발생
            f.Events.OnCarrotCollected(thisEntity);
        }

        // 아이템 활성/비활성화
        private void TogglePickedUp(Frame f, EntityRef entity, bool pickedUp)
        {
            // 당근을 제거할 것이므로 PickedUp 상태만 변경
            PickedUp = pickedUp;
        }
    }
}
