using UnityEngine;


namespace FunkySheep.Terrain
{
    [AddComponentMenu("FunkySheep/Terrain/Tile")]
    public class Tile : MonoBehaviour
    {
        public Vector2Int position;
        public FunkySheep.Types.Vector2Int initialMapPositionRounded;
        public FunkySheep.Types.Int32 zoomLevel;
        public FunkySheep.Types.String diffuseUrl;
        public Material material;
        UnityEngine.Terrain terrain;
        
        private void Awake()
        {
            terrain = GetComponent<UnityEngine.Terrain>();
            terrain.materialTemplate = Instantiate<Material>(material);
            terrain.terrainData = new TerrainData();
            GetComponent<TerrainCollider>().terrainData = terrain.terrainData;
            terrain.allowAutoConnect = true;
        }

        public void DownLoadDiffuse()
        {
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };

            string[] values = new string[3] {
                zoomLevel.value.ToString(),
                (initialMapPositionRounded.value.x + position.x).ToString(),
                (initialMapPositionRounded.value.y - position.y).ToString()
            };

            string url = diffuseUrl.Interpolate(values, variables);
            StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(url, (fileID, texture) =>
            {
                ProcessDiffuse(texture);
            }));
        }

        public void ProcessDiffuse(Texture2D texture)
        {
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            terrain.materialTemplate.SetTexture("_MainTex", texture);
        }
    }
}
