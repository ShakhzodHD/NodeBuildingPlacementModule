using UnityEngine;

namespace NodeBuildingPlacementModule
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BuildingDatabase buildingDatabase;

        private BuildingPlacementModel buildingModel;
        private void Start()
        {
            // ��������� ��������� ����� (������)
            buildingModel?.AddBlockedTile(new Vector2(0, 0));
            buildingModel?.AddBlockedTile(new Vector2(1, 0));

            // ������������� �� �������
            if (buildingModel != null)
            {
                buildingModel.OnBuildingPlaced += OnBuildingPlaced;
                buildingModel.OnBuildingUpgraded += OnBuildingUpgraded;
            }
        }
        private void OnBuildingPlaced(BuildingData building)
        {
            Debug.Log($"����� {building.type} ��������� � {building.position}");
        }

        private void OnBuildingUpgraded(BuildingData building)
        {
            Debug.Log($"����� �������� �� ������ {building.level}");
        }
    }
}