using UnityEngine;

namespace NodeBuildingPlacementModule
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BuildingDatabase buildingDatabase;

        private BuildingPlacementModel buildingModel;
        private void Start()
        {
            // Блокируем некоторые тайлы (пример)
            buildingModel?.AddBlockedTile(new Vector2(0, 0));
            buildingModel?.AddBlockedTile(new Vector2(1, 0));

            // Подписываемся на события
            if (buildingModel != null)
            {
                buildingModel.OnBuildingPlaced += OnBuildingPlaced;
                buildingModel.OnBuildingUpgraded += OnBuildingUpgraded;
            }
        }
        private void OnBuildingPlaced(BuildingData building)
        {
            Debug.Log($"Башня {building.type} размещена в {building.position}");
        }

        private void OnBuildingUpgraded(BuildingData building)
        {
            Debug.Log($"Башня улучшена до уровня {building.level}");
        }
    }
}