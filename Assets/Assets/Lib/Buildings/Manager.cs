using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Buildings.Types;
using FunkySheep.Maps.Components;
using FunkySheep.Buildings.Components;
using FunkySheep.Buildings.Systems;

namespace FunkySheep.Buildings
{
    [AddComponentMenu("FunkySheep/Buildings/Manager")]
    public class Manager : MonoBehaviour
    {
        public FunkySheep.Types.String waysUrlTemplate;
        public FunkySheep.Types.String relationsUrlTemplate;
        public FunkySheep.Types.Int32 zoomLevel;
        public FunkySheep.Types.Float tileSize;
        public FunkySheep.Types.Vector2Int initialMapPositionRounded;
        public GameObject prefab;
        SpawnBuildings spawnBuildings;

        private void Awake()
        {
            spawnBuildings = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SpawnBuildings>();
        }

        public void OnPlayerLongPositionChanged(Vector2 playerPosition)
        {
            Clean(playerPosition);
            spawnBuildings.Spawn(transform, prefab);
        }

        public void ClearTile(Vector2Int tilePosition)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject building = transform.GetChild(i).gameObject;
                Vector2 buildingPosition2D = new Vector2
                {
                    x = building.transform.position.x,
                    y = building.transform.position.z
                };

                if (buildingPosition2D.x > tilePosition.x * tileSize.value &&
                    buildingPosition2D.x < (tilePosition.x + 1) * tileSize.value &&
                    buildingPosition2D.y > tilePosition.y * tileSize.value &&
                    buildingPosition2D.y < (tilePosition.y + 1) * tileSize.value
                )
                {
                    Destroy(building);
                }
            }
        }

        public void Clean(Vector2 playerPosition)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject building = transform.GetChild(i).gameObject;
                Vector2 buildingPosition2D = new Vector2
                {
                    x = building.transform.position.x,
                    y = building.transform.position.z
                };

                if (math.distance(playerPosition, buildingPosition2D) > 100)
                {
                    building.SetActive(false);
                }
                else
                {
                    building.SetActive(true);
                }

            }
        }

        public void Download(GameObject tileGo)
        {
            Terrain.Tile tile = tileGo.GetComponent<Terrain.Tile>();

            string waysUrl = InterpolatedUrl(tile.mapPosition, waysUrlTemplate);
            StartCoroutine(FunkySheep.Network.Downloader.Download(waysUrl, (fileID, file) =>
            {
                string fileStr = System.Text.Encoding.Default.GetString(file);
                Types.JsonOsmWays waysRoot = JsonUtility.FromJson<Types.JsonOsmWays>(fileStr);
                CreateEntities(waysRoot.elements, tile);
            }));

            string relationsUrl = InterpolatedUrl(tile.mapPosition, relationsUrlTemplate);
            StartCoroutine(FunkySheep.Network.Downloader.Download(relationsUrl, (fileID, file) =>
            {
                string fileStr = System.Text.Encoding.Default.GetString(file);
                Types.JsonOsmRelations relationsRoot = JsonUtility.FromJson<Types.JsonOsmRelations>(fileStr);

                for (int i = 0; i < relationsRoot.elements.Length; i++)
                {
                    CreateEntities(relationsRoot.elements[i].members, tile);
                }
            }));
        }

        void CreateEntities(JsonOsmWay[] buildings, Terrain.Tile tile)
        {
            EntityCommandBufferSystem ecbSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer buffer = ecbSystem.CreateCommandBuffer();

            Maps.Components.TilePosition tilePosition = new Maps.Components.TilePosition
            {
                Value = new int2
                (
                    tile.position.x,
                    tile.position.y
                )
            };

            for (int i = 0; i < buildings.Length; i++)
            {
                Entity entity = buffer.CreateEntity();
                buffer.AddComponent<Components.Building>(entity, new Components.Building { });
                buffer.AddSharedComponent<Maps.Components.TilePosition>(entity, tilePosition);
                DynamicBuffer<GPSCoordinates> coordinates = buffer.AddBuffer<GPSCoordinates>(entity);

                for (int j = 0; j < buildings[i].geometry.Length; j++)
                {
                    coordinates.Add(new GPSCoordinates
                    {
                        Value = new double2
                        {
                            x = buildings[i].geometry[j].lat,
                            y = buildings[i].geometry[j].lon
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Interpolate the url inserting the boundaries and the types of OSM data to download
        /// </summary>
        /// <param boundaries="boundaries">The gps boundaries to download in</param>
        /// <returns>The interpolated Url</returns>
        public string InterpolatedUrl(Vector2Int tileMapPosition, FunkySheep.Types.String url)
        {
            double4 gpsBoundaries = FunkySheep.Maps.Utils.CaclulateGpsBoundaries(
                zoomLevel.value,
                tileMapPosition
            );

            string[] parameters = new string[5];
            string[] parametersNames = new string[5];

            parameters[0] = gpsBoundaries.w.ToString().Replace(',', '.');
            parametersNames[0] = "startLatitude";

            parameters[1] = gpsBoundaries.x.ToString().Replace(',', '.');
            parametersNames[1] = "startLongitude";

            parameters[2] = gpsBoundaries.y.ToString().Replace(',', '.');
            parametersNames[2] = "endLatitude";

            parameters[3] = gpsBoundaries.z.ToString().Replace(',', '.');
            parametersNames[3] = "endLongitude";

            return url.Interpolate(parameters, parametersNames);
        }
    }
}
