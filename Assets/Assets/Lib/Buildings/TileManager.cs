using UnityEngine;
using UnityEngine.Pool;
using Unity.Mathematics;

namespace FunkySheep.Buildings
{
    [AddComponentMenu("FunkySheep/Buildings/Tile Manager")]
    public class TileManager : MonoBehaviour
    {
        public GameObject prefab;
        public FunkySheep.Types.Float tileSize;
        ObjectPool<GameObject> pool;
        Terrain.Tile terrainTile;

        private void Awake()
        {
            pool = new ObjectPool<GameObject>(
                () => { return Instantiate(prefab, transform); },
                (building) => { building.SetActive(true); },
                (building) => { building.SetActive(false); },
                (building) => { Destroy(building); },
                false,
                50,
                100
            );

            terrainTile = GetComponent<Terrain.Tile>();
        }

        public void Clear()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject go = transform.GetChild(i).gameObject;
                if (go.activeSelf)
                    pool.Release(go);
            }
        }

        public void Spawn(Types.JsonOsmWay[] buildings)
        {
            for (int i = 0; i < buildings.Length; i++)
            {
                for (int j = 0; j < buildings[i].geometry.Length; j++)
                {
                    GameObject go = pool.Get();
                    go.transform.localPosition = GetPointPosition(buildings[i].geometry[j]);
                }
            }
        }

        Vector3 GetPointPosition(Types.JsonOsmGeometry pointGps)
        {
            return Maps.Utils.GpsToMapRealRelative(pointGps.lat, pointGps.lon, terrainTile.zoomLevel.value, terrainTile.mapPosition) * tileSize.value;
        }

        Vector3 CalculateCenter(Types.JsonOsmGeometry[] nodes)
        {
            Vector3 center = new Vector3();
            for (int i = 0; i < nodes.Length; i++)
            {
                Vector3 pointMapPosition = Maps.Utils.GpsToMapRealRelative(nodes[i].lat, nodes[i].lon, terrainTile.zoomLevel.value, terrainTile.mapPosition) * tileSize.value;

                center += pointMapPosition;
            }

            center /= nodes.Length;
            return center;
        }
    }
}
