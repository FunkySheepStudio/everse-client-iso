using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Maps.Components;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Roads.Systems
{
    [UpdateInGroup(typeof(RoadsGroup))]
    public partial class CalculatePointsCoordinates : SystemBase
    {
        protected override void OnUpdate()
        {
            TileSize tileSize;
            if (!SystemAPI.TryGetSingleton<TileSize>(out tileSize))
                return;

            ZoomLevel zoomLevel;
            if (!SystemAPI.TryGetSingleton<ZoomLevel>(out zoomLevel))
                return;

            InitialMapPosition initialMapPosition;
            if (!SystemAPI.TryGetSingleton<InitialMapPosition>(out initialMapPosition))
                return;

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Components.Road road, in DynamicBuffer<GPSCoordinates> gPSCoordinates) =>
            {

                DynamicBuffer<Points> points = buffer.AddBuffer<Points>(entity);

                for (int i = 0; i < gPSCoordinates.Length; i++)
                {
                    if (!gPSCoordinates[i].Value.Equals(gPSCoordinates[(i + 1) % gPSCoordinates.Length].Value))
                    {
                        float2 point = new float2();
                        point.x = (float)(((gPSCoordinates[i].Value.y + 180.0) / 360.0 * (1 << zoomLevel.Value)) - initialMapPosition.Value.x);
                        point.y = (float)(initialMapPosition.Value.y - ((1.0 - math.log(math.tan(gPSCoordinates[i].Value.x * math.PI / 180.0) + 1.0 / math.cos(gPSCoordinates[i].Value.x * math.PI / 180.0)) / math.PI) / 2.0 * (1 << zoomLevel.Value)) + 1);

                        point *= tileSize.Value;

                        points.Add(new Points
                        {
                            Value = point
                        });
                    }
                }
                buffer.RemoveComponent<GPSCoordinates>(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();

        }
    }
}
