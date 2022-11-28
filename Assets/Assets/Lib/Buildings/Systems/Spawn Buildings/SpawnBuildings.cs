using FunkySheep.Buildings.Components;
using Unity.Entities;
using UnityEngine;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components.Tags;
using System.Linq;
using Unity.Mathematics;
using FunkySheep.Geometry.Components;
using UnityEngine.Rendering;
using Unity.Entities.UniversalDelegates;

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
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Components.Building building, in DynamicBuffer<Points> points, in DynamicBuffer<Triangles> triangles, in DynamicBuffer<Vertices> vertices, in DynamicBuffer<Uvs> uvs, in Spawn spawn) =>
            {
                GameObject buildingGo = GameObject.Instantiate(prefabs, root);
                buildingGo.transform.localPosition = new Vector3
                (
                    building.center.x,
                    0,
                    building.center.y
                );

                Building buildingComponent = buildingGo.GetComponent<Building>();
                buildingComponent.floorHeight = building.floorHeight;
                buildingComponent.perimeter = building.perimeter;
                buildingComponent.points = new float2[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    buildingComponent.points[i] = points[i].Value;
                }

                buildingComponent.wallPrefab = new GameObject("Walls", new System.Type[] { typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider) });
                MeshFilter meshFilter = buildingComponent.wallPrefab.GetComponent<MeshFilter>();
                buildingComponent.wallPrefab.transform.parent = buildingGo.transform;

                Mesh mesh = new Mesh();
                mesh.indexFormat = IndexFormat.UInt32;
                mesh.Clear();
                mesh.SetVertices(vertices.AsNativeArray());
                mesh.SetIndices(triangles.AsNativeArray(), MeshTopology.Triangles, 0);
                mesh.SetUVs(0, uvs.AsNativeArray());
                mesh.RecalculateNormals();

                meshFilter.mesh = mesh;

                buildingComponent.wallPrefab.GetComponent<MeshCollider>().sharedMesh = mesh;

                buildingComponent.Create();

                buffer.AddComponent<SpawnBuildingOver>(entity);
            })

            .WithNone<SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
