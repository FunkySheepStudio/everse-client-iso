using FunkySheep.Buildings.Components;
using Unity.Entities;
using Unity.Collections;
using FunkySheep.Maps.Components;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components.Tags;

namespace FunkySheep.Buildings.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CalculatePointsOrder))]
    public partial class CalculatePointsCounterClockwise : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<GPSCoordinates> gPSCoordinates, in Building building, in Spawn spawn) =>
            {
                bool result = Geometry.utils.IsTriangleOrientedClockwise(
                    gPSCoordinates[gPSCoordinates.Length - 1].Value,
                    gPSCoordinates[0].Value,
                    gPSCoordinates[1].Value
                );

                if (!result)
                {
                    NativeArray<GPSCoordinates> tempPoints = new NativeArray<GPSCoordinates>(gPSCoordinates.Length, Allocator.Temp);
                    tempPoints.CopyFrom(gPSCoordinates.AsNativeArray());
                    gPSCoordinates.Clear();

                    for (int i = tempPoints.Length - 1; i >= 0; i--)
                    {
                        gPSCoordinates.Add(tempPoints[i]);
                    }
                }
            })
            .WithNone<SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
