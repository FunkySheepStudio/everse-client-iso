using UnityEngine;
using Unity.Entities;

namespace FunkySheep.Terrain
{
    [AddComponentMenu("FunkySheep/Terrain/Manager")]
    public class Manager : MonoBehaviour
    {
        public int resolution;
        public GameObject tilePrefab;
        public FunkySheep.Types.Float tileSize;
        Tile[,] tiles;
        int boundary;
        Maps.Systems.CleanTiles cleanTilesSystem;

        private void Awake()
        {
            cleanTilesSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<Maps.Systems.CleanTiles>();
            if (resolution % 2 == 0)
            {
                resolution += 1;
                Debug.Log("Added +1 to terrain resolution because of odd number");
            }
            tiles = new Tile[resolution, resolution];

            boundary = (resolution - 1) / 2;
        }

        private void Start()
        {
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    GameObject tileGo = GameObject.Instantiate(tilePrefab, new Vector3((x - boundary) * tileSize.value, 0, (y - boundary) * tileSize.value), Quaternion.identity, transform);
                    Tile tile = tileGo.GetComponent<Tile>();
                    tileGo.name = x.ToString() + " " + y.ToString();
                    tile.position = new Vector2Int(x - boundary, y - boundary);
                    tile.GetComponent<UnityEngine.Terrain>().terrainData.size = new Vector3(
                        tileSize.value,
                        1,
                        tileSize.value
                    );
                    tile.Refresh();
                    tiles[x, y] = tile;
                }
            }
        }

        public void OnTilePositionChanged(Vector2Int deltaPosition)
        {
            Tile[,] newTiles = new Tile[resolution, resolution];
            for (int x = 0; x < resolution; x++)
            {
                int oldX = ClampListIndex(x + deltaPosition.x, resolution);
                for (int y = 0; y < resolution; y++)
                {
                    int oldY = ClampListIndex(y + deltaPosition.y, resolution);
                    newTiles[x, y] = tiles[oldX, oldY];

                    bool tilePositionChange = false;

                    if (
                        deltaPosition.x == -1 && x == 0 ||
                        deltaPosition.x == 1 && x == (resolution - 1)
                        )
                    {
                        newTiles[x, y].position += new Vector2Int(deltaPosition.x, 0) * resolution;
                        newTiles[x, y].transform.position += new Vector3(deltaPosition.x, 0, 0) * resolution * tileSize.value;
                        tilePositionChange = true;
                    }

                    if (
                        deltaPosition.y == -1 && y == 0 ||
                        deltaPosition.y == 1 && y == (resolution - 1)
                        )
                    {
                        newTiles[x, y].position += new Vector2Int(0, deltaPosition.y) * resolution;
                        newTiles[x, y].transform.position += new Vector3(0, 0, deltaPosition.y) * resolution * tileSize.value;
                        tilePositionChange = true;
                    }

                    if (tilePositionChange)
                    {
                        cleanTilesSystem.CleanTile(new Unity.Mathematics.int2
                        {
                            x = tiles[oldX, oldY].position.x,
                            y = tiles[oldX, oldY].position.y,
                        });

                        newTiles[x, y].Refresh();
                    }
                }
            }

            tiles = newTiles;
        }

        public static int ClampListIndex(int index, int listSize)
        {
            index = ((index % listSize) + listSize) % listSize;

            return index;
        }
    }
}
