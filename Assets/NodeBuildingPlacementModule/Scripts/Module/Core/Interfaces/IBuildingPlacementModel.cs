using System.Collections.Generic;
using System;
using UnityEngine;

namespace NodeBuildingPlacementModule
{
    public interface IBuildingPlacementModel
    {
        event Action<Vector2> OnTileClicked;
        event Action<BuildingData> OnBuildingPlaced;
        event Action<BuildingData> OnBuildingUpgraded;
        event Action<BuildingData> OnBuildingDestroyed;

        bool CanPlaceBuilding(Vector2 position, BuildingType type);
        bool PlaceBuilding(Vector2 position, BuildingType type);
        bool UpgradeBuilding(Vector2 position);
        bool DestroyBuilding(Vector2 position);
        BuildingData GetBuilding(Vector2 position);
        List<BuildingData> GetAllBuildings();
    }
}