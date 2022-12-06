using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Entities;

namespace FunkySheep.Osm
{
    public struct Area
    {
        public double latitude;
        public double longitude;
    }

    [Serializable]
    public struct Tags
    {
        public string name;
        public string value;
    }

    [AddComponentMenu("FunkySheep/Osm/Downloader")]
    public class Downloader : MonoBehaviour
    {
        public FunkySheep.Types.String overpassApiUrl;
        public float searchRadius;
        public FunkySheep.Types.Double currentLatitude;
        public FunkySheep.Types.Double currentLongitude;
        public List<Tags> exludedTags;

        List<Area> areaExclusiontList = new List<Area>();

        string ConstructUrl()
        {
            string[] parametersNames = new string[1] {"request"};
            string[] parameters = new string[1];
            string request = "way";

            // Tags exclusion
            for (int i = 0; i < exludedTags.Count; i++)
            {
                request += @"[""" + exludedTags[i].name + @"""!~""" + exludedTags[i].value + @"""]";
            }

            // Current location
            request += "(around:" + searchRadius + "," + currentLatitude.value.ToString().Replace(",", ".") + "," + currentLongitude.value.ToString().Replace(",", ".") + ");";

            // Exclude passed locations
            for (int i = 0; i < areaExclusiontList.Count; i++)
            {
                request += "-way(around:" + searchRadius + "," + areaExclusiontList[i].latitude.ToString().Replace(",", ".") + "," + areaExclusiontList[i].longitude.ToString().Replace(",", ".") + ");";
            }

            areaExclusiontList.Add(new Area
            {
                latitude = currentLatitude.value,
                longitude = currentLongitude.value,
            });

            parameters[0] = request;

            string url = overpassApiUrl.Interpolate(parameters, parametersNames);
            return url;
        }

        public void GetOsmDataAround()
        {
            string url = ConstructUrl();

            StartCoroutine(Download(url, (file) =>
            {
                ConvertOsmData(file);
            }));
        }

        static IEnumerator Download(string url, Action<byte[]> Callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                Debug.Log(url);
            }
            else
            {
                Callback(request.downloadHandler.data);
                yield break;
            }
        }

        void ConvertOsmData(byte[] osmData)
        {
            string fileStr = System.Text.Encoding.Default.GetString(osmData);
            Types.Root root = JsonUtility.FromJson<Types.Root>(fileStr);

            EntityCommandBufferSystem ecbSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer buffer = ecbSystem.CreateCommandBuffer();

            root.CreateEntities(buffer);
        }
    }
}
