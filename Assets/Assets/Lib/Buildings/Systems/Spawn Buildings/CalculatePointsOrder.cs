using FunkySheep.Buildings.Components;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using FunkySheep.Maps.Components;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components.Tags;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CheckPlayerPosition))]
    public partial class CalculatePointsOrder : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<Points> points, in Components.Building building, in Spawn spawn) =>
            {
                int maxPointIndex = 0;
                for (int i = 0; i < points.Length; i++)
                {
                    if (math.distance(building.center, points[maxPointIndex].Value) < math.distance(building.center, points[i].Value))
                    {
                        maxPointIndex = i;
                    }
                }

                NativeArray<Points> tempPoints = new NativeArray<Points>(points.Length, Allocator.Temp);
                tempPoints.CopyFrom(points.AsNativeArray());
                points.Clear();

                for (int i = 0; i < tempPoints.Length; i++)
                {
                    points.Add(tempPoints[(i + maxPointIndex) % tempPoints.Length]);
                }
            })
            .WithNone<SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
