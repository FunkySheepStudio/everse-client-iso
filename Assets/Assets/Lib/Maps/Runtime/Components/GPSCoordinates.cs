using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Maps.Components
{
    public struct GPSCoordinates : IBufferElementData
    {
        public double2 Value;
    }
}
