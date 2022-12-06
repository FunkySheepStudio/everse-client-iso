using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FunkySheep.Props.Systems
{
    [UpdateInGroup(typeof(SpawnPropsGroup))]
    [UpdateAfter(typeof(CreatePropsEntities))]
    public partial class CheckPlayerPosition : SystemBase
    {
        protected override void OnUpdate()
        {
        }

        public void CheckPropsNearPlayer(float2 position)
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer,in LocalTransform localTransform) =>
            {
                if (math.distance(position, new float2(localTransform.Position.x, localTransform.Position.z)) < 100)
                {
                    buffer.AddComponent<Components.Tags.Spawn>(entity);
                }
            })
            .WithNone<Components.Tags.Spawn>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
            this.CompleteDependency();
        }
    }

}
