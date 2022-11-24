using FunkySheep.Buildings.Components;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using FunkySheep.Maps.Components;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components.Tags;

namespace FunkySheep.Buildings.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CheckPlayerPosition))]
    public partial class CalculatePointsOrder : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<GPSCoordinates> gPSCoordinates, in Building building, in Spawn spawn) =>
            {
                int maxPointIndex = 0;
                for (int i = 0; i < gPSCoordinates.Length; i++)
                {
                    if (math.distance(building.center, gPSCoordinates[maxPointIndex].Value) < math.distance(building.center, gPSCoordinates[i].Value))
                    {
                        maxPointIndex = i;
                    }
                }

                NativeArray<GPSCoordinates> tempPoints = new NativeArray<GPSCoordinates>(gPSCoordinates.Length, Allocator.Temp);
                tempPoints.CopyFrom(gPSCoordinates.AsNativeArray());
                gPSCoordinates.Clear();

                for (int i = 0; i < tempPoints.Length; i++)
                {
                    gPSCoordinates.Add(tempPoints[(i + maxPointIndex) % tempPoints.Length]);
                }
            })
            .WithNone<SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
