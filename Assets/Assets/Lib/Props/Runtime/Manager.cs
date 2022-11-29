using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using FunkySheep.Terrain;
using FunkySheep.Images.Components;
using Unity.Mathematics;
using FunkySheep.Props.Systems;
using FunkySheep.Props.Components;

namespace FunkySheep.Props
{
    [AddComponentMenu("FunkySheep/Props/Manager")]
    public class Manager : MonoBehaviour
    {
        public FunkySheep.Types.String osmUrl;
        public FunkySheep.Types.Float tileSize;
        public GameObject prefab;
        EntityManager entityManager;
        CheckPlayerPosition checkPlayerPositionSystem;
        SpawnSystem spawnSystem;

        private void Awake()
        {
            checkPlayerPositionSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CheckPlayerPosition>();
            spawnSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SpawnSystem>();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void OnTerrainTileRefreshed(GameObject terrainTileGo)
        {
            Tile tile = terrainTileGo.GetComponent<Tile>();
            DownLoadOsm(tile);
        }

        void DownLoadOsm(Tile tile)
        {
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };

            string[] values = new string[3] {
                tile.zoomLevel.value.ToString(),
                tile.mapPosition.x.ToString(),
                tile.mapPosition.y.ToString()
            };

            string url = osmUrl.Interpolate(values, variables);
            StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(url, (fileID, texture) =>
            {
                ProcessOsmMap(tile, texture);
            }));
        }

        public void ProcessOsmMap(Tile tile, Texture2D texture)
        {
            Entity props = entityManager.CreateEntity();
            NativeArray<Pixels> pixels = texture.GetRawTextureData<Pixels>();
            entityManager.AddBuffer<Pixels>(props);
            entityManager.GetBuffer<Pixels>(props).CopyFrom(pixels.ToArray());
            entityManager.AddBuffer<Components.IsPropCreated>(props);
            NativeArray<Components.IsPropCreated> createdList = new NativeArray<Components.IsPropCreated>(new Components.IsPropCreated[pixels.Length], Allocator.Temp);
            entityManager.GetBuffer<Components.IsPropCreated>(props).CopyFrom(createdList);
            createdList.Dispose();

            entityManager.AddComponent<TileBoundaries>(props);
            entityManager.SetComponentData(props, new TileBoundaries
            {
                tilePosition = new int2(tile.position.x, tile.position.y),
                start = new float2(tile.position.x * tileSize.value, tile.position.y * tileSize.value),
                end = new float2((tile.position.x + 1) * tileSize.value, (tile.position.y + 1) * tileSize.value)
            });
        }

        public void OnPlayerLongPositionChanged(Vector2 playerPosition)
        {
            Clean(playerPosition);
            checkPlayerPositionSystem.CheckPropsNearPlayer(playerPosition);
            spawnSystem.Spawn(transform, prefab);
        }

        public void Clean(Vector2 playerPosition)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject prop = transform.GetChild(i).gameObject;
                Vector2 propPosition2D = new Vector2
                {
                    x = prop.transform.position.x,
                    y = prop.transform.position.z
                };

                if (math.distance(playerPosition, propPosition2D) > 100)
                {
                    prop.SetActive(false);
                }
                else
                {
                    prop.SetActive(true);
                }

            }
        }
    }
}
