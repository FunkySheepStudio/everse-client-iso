using System;
using System.Collections.Generic;
using UnityEngine;

namespace FunkySheep.Terrain
{
    [AddComponentMenu("FunkySheep/Terrain/Manager")]
    public class Manager : MonoBehaviour
    {
        public int resolution;
        public GameObject tilePrefab;
        public FunkySheep.Types.Float tileSize;
        Tile[,] tiles;

        private void Awake()
        {
            if (resolution % 2 == 0)
            {
                resolution += 1;
                Debug.Log("Added +1 to terrain resolution because of odd number");
            }
            tiles = new Tile[resolution, resolution];
        }

        private void Start()
        {
            int boundary = (resolution - 1) / 2;
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    GameObject tileGo = GameObject.Instantiate(tilePrefab, new Vector3((x - boundary) * tileSize.value, 0, (y - boundary) * tileSize.value), Quaternion.identity, transform);
                    Tile tile = tileGo.GetComponent<Tile>();
                    tileGo.name = x.ToString() + " " + y.ToString();
                    tile.GetComponent<UnityEngine.Terrain>().terrainData.size = new Vector3(
                        tileSize.value,
                        1,
                        tileSize.value
                    );
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

                    if (deltaPosition.x == -1 && x == 0)
                    {
                        newTiles[x, y].transform.position += resolution * Vector3.left * tileSize.value;
                    } else if (deltaPosition.x == 1 && x == (resolution - 1))
                    {
                        newTiles[x, y].transform.position += resolution * Vector3.right * tileSize.value;
                    }

                    if (deltaPosition.y == -1 && y == 0)
                    {
                        newTiles[x, y].transform.position += resolution * Vector3.back * tileSize.value;
                    }
                    else if (deltaPosition.y == 1 && y == (resolution - 1))
                    {
                        newTiles[x, y].transform.position += resolution * Vector3.forward * tileSize.value;
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
