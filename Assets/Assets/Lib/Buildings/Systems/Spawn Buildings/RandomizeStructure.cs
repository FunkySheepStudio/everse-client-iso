using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Geometry.Components;
using FunkySheep.Buildings.Components.Barriers;
using Unity.Collections;
using System.Linq;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEditor;
using System;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(SpawnBuildingGroup))]
    [UpdateAfter(typeof(CreateStructure))]
    public partial class RandomizeStructure : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, int entityInQueryIndex, EntityCommandBuffer buffer, ref DynamicBuffer <Vertices> vertices, in Components.Building building) =>
            {
                //NativeArray<Vertices> randomizedVertices = new NativeArray<Vertices>(vertices.Length, Allocator.Temp);

                int nodeCount = vertices.Length / 10;
                var seed = (uint)(entityInQueryIndex + 1);
                var rnd = new Unity.Mathematics.Random(seed);

                for (int i = 0; i < nodeCount; i += 2)
                {
                    int waveType = rnd.NextInt(0, 2);
                    float wave = rnd.NextFloat(0, 10);

                    for (int j = 0; j < 10; j++)
                    {
                        int verticeIndex = (j * nodeCount) + i;
                        int nextVerticeIndex = (j * nodeCount) + (i + 1) % nodeCount;

                        float stageRatio = (float)j / (float)10;
                        if (waveType == 1)
                        {
                            stageRatio = math.cos(stageRatio * wave);
                        }

                        if (waveType == 2)
                        {
                            stageRatio = math.sin(stageRatio * wave);
                        }


                        float3 centerPoint = (vertices[verticeIndex].Value + vertices[nextVerticeIndex].Value) / 2;

                        vertices[verticeIndex] = new Vertices
                        {
                            Value = new float3
                            {
                                x = math.lerp(vertices[verticeIndex].Value.x, centerPoint.x, stageRatio),
                                y = vertices[verticeIndex].Value.y,
                                z = math.lerp(vertices[verticeIndex].Value.z, centerPoint.z, stageRatio)
                            }
                        };

                        vertices[nextVerticeIndex] = new Vertices
                        {
                            Value = new float3
                            {
                                x = math.lerp(vertices[nextVerticeIndex].Value.x, centerPoint.x, stageRatio),
                                y = vertices[nextVerticeIndex].Value.y,
                                z = math.lerp(vertices[nextVerticeIndex].Value.z, centerPoint.z, stageRatio)
                            }
                        };

                        /* = Vector3.Lerp(structure[i, joinIndex], centerPoint, stageRatio);
                        structure[i, (joinIndex + 1) % floorPoints.Count] = Vector3.Lerp(structure[i, (joinIndex + 1) % floorPoints.Count], centerPoint, stageRatio);*/
                    }
                }

                //randomizedVertices.Dispose();
                buffer.AddComponent<SpawnBuildingOver>(entity);
            })
            .WithNone<SpawnBuildingOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }

        public void Debug()
        {
            Entities.ForEach((in Components.Building building, in DynamicBuffer<Vertices> vertices, in SpawnBuildingOver spawnBuildingOver) =>
            {
                int nodeCount = vertices.Length / 10;
                for (int i = 0; i < nodeCount; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        int index = (j * nodeCount) + i;
                        int nextIndex = (j * nodeCount) + (i + 1) % nodeCount;

                        UnityEngine.Debug.DrawLine(vertices[index].Value, vertices[nextIndex].Value);
                        if (j < 9)
                        {
                            UnityEngine.Debug.DrawLine(vertices[index].Value, vertices[index + nodeCount].Value);
                        }
                    }
                }
            })
            .WithoutBurst()
            .Run();
        }
    }
}
