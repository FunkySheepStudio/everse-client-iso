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
                    GameObject tileGo = GameObject.Instantiate(tilePrefab, new Vector3(x - boundary, 0, y - boundary), Quaternion.identity, transform);
                    tiles[x, y] = tileGo.GetComponent<Tile>();
                }
            }
        }
    }
}
