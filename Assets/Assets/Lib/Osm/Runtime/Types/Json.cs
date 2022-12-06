using System;
using UnityEngine;
using Unity.Entities;
using FunkySheep.Maps.Components;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;

namespace FunkySheep.Osm.Types
{
    [Serializable]
    public struct Root
    {
        public Way[] elements;

        public void CreateEntities(EntityCommandBuffer buffer)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].CreateEntity(buffer);
            }
        }
    }

    [Serializable]
    public struct Way
    {
        public long id;
        public Geometry[] geometry;
        public Tags tags;

        public void CreateEntity(EntityCommandBuffer buffer)
        {
            if (tags.IsPresent() && geometry != null)
            {
                Entity entity = buffer.CreateEntity();

                DynamicBuffer<GPSCoordinates> coordinates = buffer.AddBuffer<GPSCoordinates>(entity);

                for (int j = 0; j < geometry.Length; j++)
                {
                    coordinates.Add(new GPSCoordinates
                    {
                        Value = new double2
                        {
                            x = geometry[j].lat,
                            y = geometry[j].lon
                        }
                    });
                }

                if (tags.highway != null)
                {

                }
                else if (tags.area != null)
                {

                }
                else if (tags.building != null)
                {
                    buffer.AddComponent<FunkySheep.Buildings.Components.Building>(entity, new FunkySheep.Buildings.Components.Building { });
                }
                else if (tags.landuse != null)
                {

                }
                else if (tags.tourism != null)
                {

                }
                else if (tags.amenity != null)
                {

                }
                else if (tags.barrier != null)
                {

                }
                else if (tags.leisure != null)
                {

                }
            }    
            else
            {
                Debug.Log("No type found for Osm Way: " + id);
            }
        }
    }

    [Serializable]
    public struct Geometry
    {
        public double lat;
        public double lon;
    }

    [Serializable]
    public struct Tags
    {
        public string highway;
        public string area;
        public string building;
        public string landuse;
        public string tourism;
        public string amenity;
        public string barrier;
        public string leisure;

        public bool IsPresent()
        {
            return (highway != null || area != null || building != null || landuse != null || tourism != null || amenity != null || barrier != null || leisure != null);
        }
    }
}