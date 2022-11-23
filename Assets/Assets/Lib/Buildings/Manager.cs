using UnityEngine;
using Unity.Mathematics;

namespace FunkySheep.Buildings
{
    [AddComponentMenu("FunkySheep/Buildings/Manager")]
    public class Manager : MonoBehaviour
    {
        public FunkySheep.Types.String waysUrlTemplate;
        public FunkySheep.Types.String relationsUrlTemplate;
        public FunkySheep.Types.Int32 zoomLevel;
        public FunkySheep.Types.Vector2Int initialMapPositionRounded;

        public void Download(GameObject tileGo)
        {
            Terrain.Tile tile = tileGo.GetComponent<Terrain.Tile>();
            TileManager tileManager = tileGo.GetComponent<TileManager>();
            tileManager.Clear();

            string waysUrl = InterpolatedUrl(tile.mapPosition, waysUrlTemplate);
            StartCoroutine(FunkySheep.Network.Downloader.Download(waysUrl, (fileID, file) =>
            {
                string fileStr = System.Text.Encoding.Default.GetString(file);
                Types.JsonOsmWays waysRoot = JsonUtility.FromJson<Types.JsonOsmWays>(fileStr);
                tileManager.Spawn(waysRoot.elements);
            }));

            string relationsUrl = InterpolatedUrl(tile.mapPosition, relationsUrlTemplate);
            StartCoroutine(FunkySheep.Network.Downloader.Download(relationsUrl, (fileID, file) =>
            {
                string fileStr = System.Text.Encoding.Default.GetString(file);
                Types.JsonOsmRelations relationsRoot = JsonUtility.FromJson<Types.JsonOsmRelations>(fileStr);

                for (int i = 0; i < relationsRoot.elements.Length; i++)
                {
                    tileManager.Spawn(relationsRoot.elements[i].members);
                }
            }));
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
