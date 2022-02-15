using System;
using System.Collections.Generic;
using Mechanics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    [CreateAssetMenu(fileName = "MapSettings", menuName = "Geekbrains/Settings/Map Settings")]
    public class MapSettings : ScriptableObject
    {
        [SerializeField]
        private List<PlanetInfo> _planetsInfo;

        [SerializeField] 
        private Star _star;

        [SerializeField, Range(1, 100)]
        private int _starsCount = 50;

        [SerializeField] 
        private float _starsGenerationRadius = 1500;

        [Serializable]
        private class PlanetInfo
        {
            public PlanetOrbit planet;
            public float radius;
            public float angle;
            public float circleInSecond;
            public float rotationSpeed;
        }


        public void CreatePlanets()
        {
            var rootObject = new GameObject("Planets");
            
            foreach (var planetInfo in _planetsInfo)
            {
                var point = 
                    new Vector3(Mathf.Sin(planetInfo.angle), 0, Mathf.Cos(planetInfo.angle)) * planetInfo.radius;
                
                var planet = (PlanetOrbit) PrefabUtility.InstantiatePrefab(planetInfo.planet, rootObject.transform);
                planet.transform.position = point;
                planet.CircleInSecond = planetInfo.circleInSecond;
                planet.RotationSpeed = planetInfo.rotationSpeed;
            }

        }

        public void CreateStars()
        {
            var rootObject = new GameObject("Stars");

            for (var i = 0; i < _starsCount; i++)
            {
                var position = Random.insideUnitSphere * _starsGenerationRadius;
                
                var star = (Star) PrefabUtility.InstantiatePrefab(_star, rootObject.transform);
                star.transform.position = position;
            }
        }
    }
}