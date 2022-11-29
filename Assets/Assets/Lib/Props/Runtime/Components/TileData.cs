using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Props.Components
{
    public struct TileData : IComponentData
    {
        public float size;
        public int2 tilePosition;
        public float2 start;
        public float2 end;
        public int3 color;
    }
}
