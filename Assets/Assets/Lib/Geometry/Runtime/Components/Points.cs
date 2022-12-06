using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Geometry.Components
{
    public struct Points : IBufferElementData
    {
        public float2 Value;
        public float3 ToFloat3()
        {
            return new float3(Value.x, 0, Value.y);
        }
    }
}
