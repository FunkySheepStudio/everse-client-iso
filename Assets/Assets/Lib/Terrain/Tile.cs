using UnityEngine;

namespace FunkySheep.Terrain
{
    [AddComponentMenu("FunkySheep/Terrain/Tile")]
    public class Tile : MonoBehaviour
    {
        UnityEngine.Terrain terrain;
        private void Awake()
        {
            terrain = GetComponent<UnityEngine.Terrain>();
            terrain.terrainData = new TerrainData();
            GetComponent<TerrainCollider>().terrainData = terrain.terrainData;
        }
    }
}
