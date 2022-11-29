using FunkySheep.Buildings.Components;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components.Tags;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    public partial class CheckPlayerPosition : SystemBase
    {
        protected override void OnUpdate()
        {
        }

        public void CheckBuildingNearPlayer(float2 position)
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Components.Building building, in SetBuildingCoordinatesOver setBuildingCoordinatesOver) =>
            {
                if (math.distance(position, building.center) < 100)
                {
                    buffer.AddComponent<Spawn>(entity);
                }
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithNone<Spawn>()
            .WithNone<SpawnBuildingOver>()
            .ScheduleParallel();
            this.CompleteDependency();
        }
    }

}
