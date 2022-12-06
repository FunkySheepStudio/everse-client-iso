using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    public partial class CalculatePointsOrder : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<Points> points, in Components.Building building) =>
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
            .WithNone<Components.Barriers.SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
