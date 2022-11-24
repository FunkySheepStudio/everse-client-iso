using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Maps.Systems
{
    public partial class CleanTiles : SystemBase
    {
        protected override void OnUpdate()
        {

        }

        public void CleanTile(int2 tilePosition)
        {
            Components.TilePosition filterPosition = new Components.TilePosition
            {
                Value = tilePosition
            };

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Components.TilePosition entityPosition) =>
            {
                buffer.DestroyEntity(entity);
            })
            .WithSharedComponentFilter(filterPosition)
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
            this.CompleteDependency();
        }
    }
}
