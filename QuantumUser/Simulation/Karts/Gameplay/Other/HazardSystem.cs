using Photon.Deterministic;

namespace Quantum
{
    public unsafe class HazardSystem : SystemMainThreadFilter<HazardSystem.Filter>, ISignalOnCollision3D
    {
        // Hazard가 부착된 모든 엔티티 탐색 & 처리
        public struct Filter
        {
            public EntityRef Entity;
            public Hazard* Hazard;
            public Transform3D* Transform3D;
        }

        // 장애물 상태 업데이트
        public override void Update(Frame f, ref Filter filter)
        {
            filter.Hazard->Update(f, filter.Entity);

            // 삭제 여부 검사
            if (!filter.Hazard->MarkedForDestruction)
            {
                return;
            }

            if (filter.Hazard->DamageRadius > FP._0)
            {
                // 데미지 범위 내에 있는 모든 엔티티 검색
                var hits = f.Physics3D.OverlapShape(
                    filter.Transform3D->Position,
                    FPQuaternion.Identity,
                    Shape3D.CreateSphere(filter.Hazard->DamageRadius),
                    f.SimulationConfig.KartLayer
                );

                if (hits.Count > 0)
                {
                    for (int i = 0; i < hits.Count; i++)
                    {
                        var hit = hits[i];
                        if (f.Unsafe.TryGetPointer(hit.Entity, out KartHitReceiver* hitReceiver))
                        {
                            hitReceiver->TakeHit(f, hit.Entity, filter.Hazard->HitDuration);
                        }
                    }
                }
            }

            // 장애물 삭제
            f.Destroy(filter.Entity);
        }

        public void OnCollision3D(Frame f, CollisionInfo3D info)
        {
            if (f.Unsafe.TryGetPointer<Hazard>(info.Entity, out Hazard* hazard) == false)
                return;

            if (!info.IsStatic)
                return;

            if (FPVector3.Dot(info.ContactNormal, FPVector3.Up) < FP._0_75)
            {
                hazard->MarkedForDestruction = true;
            }
        }
    }
}
