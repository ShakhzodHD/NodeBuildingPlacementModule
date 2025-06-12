using System.Collections.Generic;
using System;
using UnityEngine;

namespace NodeBuildingPlacementModule
{
    public interface IBuildingPlacementView
    {
        event Action<Vector2> OnTileClicked;
        event Action<BuildingType> OnBuildingTypeSelected;
        event Action OnUpgradeButtonClicked;

        void ShowBuildingSelection(Vector2 position, List<BuildingType> availableTypes);
        void HideBuildingSelection();
        void ShowUpgradeButton(Vector2 position, BuildingData building);
        void HideUpgradeButton();
        void CreateBuildingVisual(BuildingData building);
        void DestroyBuildingVisual(Vector2 position);
        void UpdateBuildingVisual(BuildingData building);
    }
}