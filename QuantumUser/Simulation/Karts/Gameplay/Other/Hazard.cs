using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe partial struct Hazard
    {
        // Hazard의 수명 추적
        public void Update(Frame f, EntityRef entityRef)
        {
            if (TotalTime >= TimeAlive)
            {
                MarkedForDestruction = true;
            }
            else
            {
                TotalTime += f.DeltaTime;
            }
        }
    }
}
