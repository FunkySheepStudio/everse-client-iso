using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Buildings.Components;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Maps.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SetBuildingCoordinatesGroup))]
    [UpdateAfter(typeof(RemoveCollinearPoints))]
    public partial class CalculatePointsCoordinates : SystemBase
    {
        protected override void OnUpdate()
        {
            TileSize tileSize;
            if (!TryGetSingleton<TileSize>(out tileSize))
                return;

            ZoomLevel zoomLevel;
            if (!TryGetSingleton<ZoomLevel>(out zoomLevel))
                return;

            InitialMapPosition initialMapPosition;
            if (!TryGetSingleton<InitialMapPosition>(out initialMapPosition))
                return;

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Components.Building building, in DynamicBuffer<GPSCoordinates> gPSCoordinates) =>
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
            .WithNone<SetBuildingCoordinatesOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();

        }
    }
}
