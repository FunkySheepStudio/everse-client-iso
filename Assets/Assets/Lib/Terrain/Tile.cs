using UnityEngine;


namespace FunkySheep.Terrain
{
    [AddComponentMenu("FunkySheep/Terrain/Tile")]
    public class Tile : MonoBehaviour
    {
        public Vector2Int position;
        public FunkySheep.Types.Vector2Int initialMapPositionRounded;
        public FunkySheep.Types.Int32 zoomLevel;
        public FunkySheep.Types.String osmUrl;
        public FunkySheep.Types.String satelliteUrl;
        public Material material;
        public FunkySheep.Events.GameObjectEvent onTileRefreshed;
        public Vector2Int mapPosition;
        UnityEngine.Terrain terrain;
        
        private void Awake()
        {
            terrain = GetComponent<UnityEngine.Terrain>();
            terrain.materialTemplate = Instantiate<Material>(material);
            terrain.terrainData = new TerrainData();
            GetComponent<TerrainCollider>().terrainData = terrain.terrainData;
            terrain.allowAutoConnect = true;
        }
        
        public void Refresh()
        {
            mapPosition = new Vector2Int(
                initialMapPositionRounded.value.x + position.x,
                initialMapPositionRounded.value.y - position.y
            );

            DownLoadOsm();
            DownLoadSatelite();
            onTileRefreshed.Raise(gameObject);
        }

        void DownLoadOsm()
        {
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };

            string[] values = new string[3] {
                zoomLevel.value.ToString(),
                mapPosition.x.ToString(),
                mapPosition.y.ToString()
            };

            string url = osmUrl.Interpolate(values, variables);
            StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(url, (fileID, texture) =>
            {
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Point;
                //GetComponent<MeshRenderer>().material.SetTexture("_Osm", texture);
                terrain.materialTemplate.SetTexture("_Osm", texture);
            }));
        }

        void DownLoadSatelite()
        {
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };

            string[] values = new string[3] {
                zoomLevel.value.ToString(),
                mapPosition.x.ToString(),
                mapPosition.y.ToString()
            };

            string url = satelliteUrl.Interpolate(values, variables);
            StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(url, (fileID, texture) =>
            {
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Point;
                //GetComponent<MeshRenderer>().material.SetTexture("_Satelite", texture);
                terrain.materialTemplate.SetTexture("_Satelite", texture);
            }));
        }
    }
}
