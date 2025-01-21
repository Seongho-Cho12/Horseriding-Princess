namespace Quantum
{
    public unsafe class PickupSystem : SystemMainThreadFilter<PickupSystem.Filter>, ISignalOnTriggerEnter3D
    {
        // Quantum의 ECS 시스템이 Filter를 기반으로 게임 내 픽업 아이템을 추적.
        public struct Filter
        {
            public EntityRef Entity;
            public Carrot* Carrot;
        }

        // 당근 아이템은 리스폰되지 않도록 업데이트 로직에서 제거
        public override void Update(Frame frame, ref Filter filter)
        {
            // 당근은 재생성되지 않으므로 업데이트 로직 없음
        }

        // 충돌 감지 및 OnPickup 함수 호출
        public void OnTriggerEnter3D(Frame f, TriggerInfo3D info)
        {
            if (f.Unsafe.TryGetPointer<Kart>(info.Entity, out var kart) == false)
                return;

            if (f.Unsafe.TryGetPointer<Carrot>(info.Other, out var pickup) == false)
                return;

            // OnPickup 호출을 유지하고 아이템 효과는 해당 함수에서 처리
            pickup->OnPickup(f, info.Other, info.Entity);
        }
    }
}
