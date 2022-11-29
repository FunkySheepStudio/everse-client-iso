using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Props.Components
{
    public struct TileBoundaries : IComponentData
    {
        public int2 tilePosition;
        public float2 start;
        public float2 end;
    }
}
