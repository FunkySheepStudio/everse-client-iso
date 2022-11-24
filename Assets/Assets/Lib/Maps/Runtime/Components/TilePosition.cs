using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Maps.Components
{
    public struct TilePosition : ISharedComponentData
    {
        public int2 Value;
    }
}
