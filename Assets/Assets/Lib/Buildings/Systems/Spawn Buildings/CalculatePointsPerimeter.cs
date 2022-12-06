using Unity.Entities;
using UnityEngine;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CalculatePointsCounterClockwise))]
    public partial class CalculatePointsPerimeter : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Components.Building building, in DynamicBuffer<Points> points) =>
            {
                float perimeter = 0;

                for (int i = 0; i < points.Length; i++)
                {
                    perimeter += Vector2.Distance(points[i].Value, points[(i + 1) % points.Length].Value);
                }

                building.perimeter = perimeter;
                building.floorHeight = building.perimeter / points.Length;
            })
            .WithNone<Components.Barriers.SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
