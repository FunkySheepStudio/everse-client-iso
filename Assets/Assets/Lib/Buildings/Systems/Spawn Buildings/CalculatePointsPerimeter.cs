using FunkySheep.Buildings.Components;
using Unity.Entities;
using UnityEngine;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components.Tags;

namespace FunkySheep.Buildings.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CalculatePointsCounterClockwise))]
    public partial class CalculatePointsPerimeter : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Building building, in DynamicBuffer<Points> points, in Spawn spawn) =>
            {
                float perimeter = 0;

                for (int i = 0; i < points.Length; i++)
                {
                    perimeter += Vector2.Distance(points[i].Value, points[(i + 1) % points.Length].Value);
                }

                building.perimeter = perimeter;

                buffer.AddComponent<SpawnBuildingOver>(entity);
            })
            .WithNone<SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
