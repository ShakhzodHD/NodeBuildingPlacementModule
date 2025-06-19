using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeBuildingPlacementModule
{
    public class BuildingPlacementController : IBuildingPlacementController, IDisposable
    {
        private readonly IBuildingPlacementModel _model;
        private readonly IBuildingPlacementView _view;
        private readonly BuildingDatabase _buildingDatabase;

        private Vector2? _currentSelectedTile;
        private bool _isSelectionMode;

        public BuildingPlacementController(
            IBuildingPlacementModel model,
            IBuildingPlacementView view,
            BuildingDatabase buildingDatabase)
        {
            _model = model;
            _view = view;
            _buildingDatabase = buildingDatabase;
        }

        public void Initialize()
        {
            _view.OnTileClicked += HandleTileClicked;
            _view.OnBuildingTypeSelected += HandleBuildingTypeSelected;
            _view.OnUpgradeButtonClicked += HandleUpgradeButtonClicked;
            _view.OnDeleteButtonClicked += HandleDeleteButtonClicked;

            _model.OnBuildingPlaced += HandleBuildingPlaced;
            _model.OnBuildingUpgraded += HandleBuildingUpgraded;
            _model.OnBuildingDestroyed += HandleBuildingDestroyed;
        }

        public void Dispose()
        {
            _view.OnTileClicked -= HandleTileClicked;
            _view.OnBuildingTypeSelected -= HandleBuildingTypeSelected;
            _view.OnUpgradeButtonClicked -= HandleUpgradeButtonClicked;
            _view.OnDeleteButtonClicked -= HandleDeleteButtonClicked;

            _model.OnBuildingPlaced -= HandleBuildingPlaced;
            _model.OnBuildingUpgraded -= HandleBuildingUpgraded;
            _model.OnBuildingDestroyed -= HandleBuildingDestroyed;
        }

        private void HandleTileClicked(Vector2 position)
        {
            var existingBuilding = _model.GetBuilding(position);

            if (existingBuilding != null)
            {
                _view.HideBuildingSelection();
                _view.ShowUpgradeButton(position, existingBuilding);
                _view.ShowDeleteButton(position, existingBuilding);
                _currentSelectedTile = position;
                _isSelectionMode = false;
            }
            else
            {
                _view.HideUpgradeButton();
                _view.HideDeleteButton();
                var availableTypes = GetAvailableBuildingTypes();
                _view.ShowBuildingSelection(position, availableTypes);
                _currentSelectedTile = position;
                _isSelectionMode = true;
            }
        }

        private void HandleBuildingTypeSelected(BuildingType type)
        {
            if (!_currentSelectedTile.HasValue || !_isSelectionMode)
                return;

            var position = _currentSelectedTile.Value;

            if (_model.CanPlaceBuilding(position, type))
            {
                _model.PlaceBuilding(position, type);
                _view.HideBuildingSelection();
                _currentSelectedTile = null;
                _isSelectionMode = false;
            }
        }

        private void HandleUpgradeButtonClicked()
        {
            if (!_currentSelectedTile.HasValue)
                return;

            var position = _currentSelectedTile.Value;

            if (_model.UpgradeBuilding(position))
            {
                _view.HideUpgradeButton();
                _currentSelectedTile = null;
            }
        }

        private void HandleDeleteButtonClicked()
        {
            if (!_currentSelectedTile.HasValue)
                return;

            var position = _currentSelectedTile.Value;

            if (_model.DestroyBuilding(position))
            {
                _view.HideUpgradeButton();
                _view.HideDeleteButton();
                _currentSelectedTile = null;
            }
        }

        private void HandleBuildingPlaced(BuildingData building)
        {
            _view.CreateBuildingVisual(building);
        }

        private void HandleBuildingUpgraded(BuildingData building)
        {
            _view.UpdateBuildingVisual(building);
        }

        private void HandleBuildingDestroyed(BuildingData building)
        {
            _view.DestroyBuildingVisual(building.position);
        }

        private List<BuildingType> GetAvailableBuildingTypes()
        {
            // Here you can add logic to filter available types based on player resources, level, etc.
            return new List<BuildingType>
        {
            BuildingType.BasicTower,
            BuildingType.AdvancedTower,
            BuildingType.SlowTower,
            BuildingType.SplashTower
        };
        }
    }
}