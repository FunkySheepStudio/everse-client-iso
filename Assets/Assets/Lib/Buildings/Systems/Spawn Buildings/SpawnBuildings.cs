using FunkySheep.Buildings.Components;
using Unity.Entities;
using UnityEngine;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components.Tags;
using System.Linq;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CalculatePointsPerimeter))]
    public partial class SpawnBuildings : SystemBase
    {
        protected override void OnUpdate()
        {
        }

        public void Spawn(Transform root, GameObject prefabs)
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Components.Building building, in DynamicBuffer<Points> points, in Spawn spawn) =>
            {
                GameObject buildingGo = GameObject.Instantiate(prefabs, root);
                buildingGo.transform.localPosition = new Vector3
                (
                    building.center.x,
                    0,
                    building.center.y
                );

                Building buildingComponent = buildingGo.GetComponent<Building>();
                buildingComponent.points = new float2[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    buildingComponent.points[i] = points[i].Value;
                }

                buffer.AddComponent<SpawnBuildingOver>(entity);
            })

            .WithNone<SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
