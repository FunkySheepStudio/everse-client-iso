using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FunkySheep.Props.Systems
{
    [UpdateInGroup(typeof(SpawnPropsGroup))]
    [UpdateAfter(typeof(CheckPlayerPosition))]
    public partial class SpawnSystem : SystemBase
    {
        protected override void OnUpdate()
        {
        }

        public void Spawn(Transform parent, GameObject prefab)
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Translation translation, in Components.Tags.Spawn spawn) =>
            {
                GameObject prop = GameObject.Instantiate(prefab, parent);
                prop.transform.position = translation.Value;
                buffer.DestroyEntity(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
            this.CompleteDependency();
        }
    }

}
