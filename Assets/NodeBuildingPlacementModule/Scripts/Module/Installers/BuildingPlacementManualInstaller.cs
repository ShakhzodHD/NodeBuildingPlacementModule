using UnityEngine;

namespace NodeBuildingPlacementModule
{
    public class BuildingPlacementManualInstaller : MonoBehaviour
    {
        [SerializeField] private BuildingDatabase _buildingDatabase;
        [SerializeField] private BuildingPlacementView _view;

        private IBuildingPlacementController _controller;

        private void Awake()
        {
            // Create instances
            var model = new BuildingPlacementModel();
            var view = _view;

            // Manual injection for view
            if (view.GetComponent<BuildingPlacementView>() is BuildingPlacementView viewComponent)
            {
                viewComponent.Initialize(_buildingDatabase);
            }

            var controller = new BuildingPlacementController(model, view, _buildingDatabase);

            _controller = controller;
            _controller.Initialize();

            GameManager.Instance.Initialize(model);
        }

        private void OnDestroy()
        {
            _controller?.Dispose();
        }
    }
}