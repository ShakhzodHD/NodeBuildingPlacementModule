using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeBuildingPlacementModule
{
    public class BuildingPlacementModel : IBuildingPlacementModel
    {
        public event Action<BuildingData> OnBuildingPlaced;
        public event Action<BuildingData> OnBuildingUpgraded;
        public event Action<BuildingData> OnBuildingDestroyed;

        private readonly Dictionary<Vector2, BuildingData> _buildings = new();

        public bool CanPlaceBuilding(Vector2 position, BuildingType type)
        {
            return !_buildings.ContainsKey(position);
        }

        public bool PlaceBuilding(Vector2 position, BuildingType type)
        {
            if (!CanPlaceBuilding(position, type))
                return false;

            var building = new BuildingData(type, position);
            _buildings[position] = building;
            OnBuildingPlaced?.Invoke(building);
            return true;
        }

        public bool UpgradeBuilding(Vector2 position)
        {
            if (!_buildings.TryGetValue(position, out var building))
                return false;

            building.Upgrade();
            OnBuildingUpgraded?.Invoke(building);
            return true;
        }

        public bool DestroyBuilding(Vector2 position)
        {
            if (!_buildings.TryGetValue(position, out var building))
                return false;

            _buildings.Remove(position);
            OnBuildingDestroyed?.Invoke(building);
            return true;
        }

        public BuildingData GetBuilding(Vector2 position)
        {
            _buildings.TryGetValue(position, out var building);
            return building;
        }

        public List<BuildingData> GetAllBuildings()
        {
            return new List<BuildingData>(_buildings.Values);
        }
    }
}