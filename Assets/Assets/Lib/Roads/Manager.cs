using FunkySheep.Maps.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using FunkySheep.Roads.Types;
using FunkySheep.Roads.Systems;
using FunkySheep.Buildings.Systems;

namespace FunkySheep.Roads
{
    [AddComponentMenu("FunkySheep/Roads/Manager")]
    public class Manager : MonoBehaviour
    {
        public FunkySheep.Types.String urlTemplate;
        public FunkySheep.Types.Int32 zoomLevel;
        public GameObject prefab;
        public GameObject Connector;
        SpawnRoads spawnRoads;

        private void Awake()
        {
            spawnRoads = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SpawnRoads>();
        }

        public void OnPlayerLongPositionChanged(Vector2 playerPosition)
        {
            Clean(playerPosition);
            spawnRoads.Spawn(transform, prefab, Connector);
        }

        public void Clean(Vector2 playerPosition)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject road = transform.GetChild(i).gameObject;
                Vector2 roadPosition2D = new Vector2
                {
                    x = road.transform.position.x,
                    y = road.transform.position.z
                };

                if (math.distance(playerPosition, roadPosition2D) > 500)
                {
                    road.SetActive(false);
                }
                else
                {
                    road.SetActive(true);
                }

            }
        }

        public void Download(GameObject tileGo)
        {
            Terrain.Tile tile = tileGo.GetComponent<Terrain.Tile>();

            string waysUrl = InterpolatedUrl(tile.mapPosition, urlTemplate);
            StartCoroutine(FunkySheep.Network.Downloader.Download(waysUrl, (fileID, file) =>
            {
                string fileStr = System.Text.Encoding.Default.GetString(file);
                Types.JsonOsmWays waysRoot = JsonUtility.FromJson<Types.JsonOsmWays>(fileStr);
                CreateEntities(waysRoot.elements, tile);
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
                buffer.AddComponent<Components.Road>(entity, new Components.Road { });
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
