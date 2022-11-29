using Unity.Entities;
using Unity.Transforms;
using FunkySheep.Images.Components;
using FunkySheep.Props.Components;
using FunkySheep.Maps.Components;

namespace FunkySheep.Props.Systems
{
    [UpdateInGroup(typeof(SpawnPropsGroup))]
    public partial class CreatePropsEntities : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in DynamicBuffer<Pixels> pixels, in TileBoundaries tileBoundaries) =>
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i].Value.g == 173 && pixels[i].Value.b == 209 && pixels[i].Value.a == 158)
                    {
                        Entity tree = buffer.CreateEntity();
                        buffer.AddComponent<Translation>(tree);
                        buffer.SetComponent(tree, new Translation
                        {
                            Value = new Unity.Mathematics.float3
                            {
                                x = tileBoundaries.start.x + ((tileBoundaries.end.x - tileBoundaries.start.x) / 256 * (i % 256)),
                                y = 0,
                                z = tileBoundaries.start.y + ((tileBoundaries.end.y - tileBoundaries.start.y) / 256 * (i / 256))
                            }
                        });
                        buffer.AddSharedComponent(entity, new TilePosition { Value = tileBoundaries.tilePosition });
                    }
                }

                buffer.DestroyEntity(entity);
            })
           .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
           .ScheduleParallel();
        }
    }

}
