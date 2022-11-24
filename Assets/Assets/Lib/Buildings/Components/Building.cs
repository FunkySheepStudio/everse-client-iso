using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Components
{
    public struct Building : IComponentData
    {
        public float2 center;
        public float perimeter;
    }
}
