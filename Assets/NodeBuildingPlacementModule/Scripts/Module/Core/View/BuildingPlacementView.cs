using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace NodeBuildingPlacementModule
{
    public class BuildingPlacementView : MonoBehaviour, IBuildingPlacementView
    {
        [Header("UI References")]
        [SerializeField] private GameObject _buildingSelectionPanel;
        [SerializeField] private Transform _buildingButtonsParent;
        [SerializeField] private GameObject _buildingButtonPrefab;
        [SerializeField] private GameObject _upgradeButton;

        [Header("Grid Settings")]
        [SerializeField] private float _gridSize = 1f;
        [SerializeField] private LayerMask _groundLayer = 1;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject _tileHighlightPrefab;
        [SerializeField] private Material _validPlacementMaterial;
        [SerializeField] private Material _invalidPlacementMaterial;

        [Header("Input")]
        [SerializeField] private InputActionReference _clickAction;
        [SerializeField] private InputActionReference _pointerPositionAction;

        public event Action<Vector2> OnTileClicked;
        public event Action<BuildingType> OnBuildingTypeSelected;
        public event Action OnUpgradeButtonClicked;

        private Camera _camera;
        private BuildingDatabase _buildingDatabase;
        private Vector2 _currentSelectedTile;
        private GameObject _currentHighlight;
        private readonly Dictionary<Vector2, GameObject> _buildingVisuals = new();

        private PlayerInput _playerInput;
        private InputAction _clickInputAction;
        private InputAction _pointerPositionInputAction;

        public void Initialize(BuildingDatabase buildingDatabase)
        {
            _buildingDatabase = buildingDatabase;
        }

        private void Awake()
        {
            _camera = Camera.main;
            if (_upgradeButton != null)
            {
                _upgradeButton.GetComponent<UnityEngine.UI.Button>()?.onClick
                    .AddListener(() => OnUpgradeButtonClicked?.Invoke());
            }

            SetupInputActions();
        }

        private void OnEnable()
        {
            EnableInputActions();
        }

        private void OnDisable()
        {
            DisableInputActions();
        }

        private void SetupInputActions()
        {
            if (_clickAction != null && _pointerPositionAction != null)
            {
                _clickInputAction = _clickAction.action;
                _pointerPositionInputAction = _pointerPositionAction.action;
            }
            else
            {
                _clickInputAction = new InputAction("Click", InputActionType.Button);
                _clickInputAction.AddBinding("<Mouse>/leftButton");
                _clickInputAction.AddBinding("<Touchscreen>/primaryTouch/tap");

                _pointerPositionInputAction = new InputAction("PointerPosition", InputActionType.Value);
                _pointerPositionInputAction.AddBinding("<Mouse>/position");
                _pointerPositionInputAction.AddBinding("<Touchscreen>/primaryTouch/position");
            }

            _clickInputAction.performed += OnClickPerformed;
        }

        private void EnableInputActions()
        {
            _clickInputAction?.Enable();
            _pointerPositionInputAction?.Enable();
        }

        private void DisableInputActions()
        {
            _clickInputAction?.Disable();
            _pointerPositionInputAction?.Disable();
        }

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            Vector2 screenPosition = _pointerPositionInputAction.ReadValue<Vector2>();

            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 worldPos = _camera.ScreenToWorldPoint(screenPosition);
            Vector2 gridPos = WorldToGridPosition(worldPos);

            if (IsValidGridPosition(gridPos))
            {
                OnTileClicked?.Invoke(gridPos);
            }
            else
            {
                HideBuildingSelection();
            }
        }

        private Vector2 WorldToGridPosition(Vector2 worldPos)
        {
            return new Vector2(
                Mathf.Round(worldPos.x / _gridSize) * _gridSize,
                Mathf.Round(worldPos.y / _gridSize) * _gridSize
            );
        }

        private bool IsValidGridPosition(Vector2 gridPos)
        {
            RaycastHit2D hit = Physics2D.Raycast(gridPos, Vector2.zero, 0f, _groundLayer);
            return hit.collider != null;
        }

        public void ShowBuildingSelection(Vector2 position, List<BuildingType> availableTypes)
        {
            _currentSelectedTile = position;
            HideBuildingSelection();

            if (_buildingSelectionPanel != null)
            {
                _buildingSelectionPanel.SetActive(true);
                _buildingSelectionPanel.transform.position = _camera.WorldToScreenPoint(position);

                CreateBuildingButtons(availableTypes);
            }

            ShowTileHighlight(position, true);
        }

        public void HideBuildingSelection()
        {
            if (_buildingSelectionPanel != null)
                _buildingSelectionPanel.SetActive(false);

            ClearBuildingButtons();
            HideTileHighlight();
        }

        public void ShowUpgradeButton(Vector2 position, BuildingData building)
        {
            _currentSelectedTile = position;

            if (_upgradeButton != null)
            {
                _upgradeButton.SetActive(true);
                _upgradeButton.transform.position = _camera.WorldToScreenPoint(position + Vector2.up * 0.5f);
            }

            ShowTileHighlight(position, true);
        }

        public void HideUpgradeButton()
        {
            if (_upgradeButton != null)
                _upgradeButton.SetActive(false);

            HideTileHighlight();
        }

        public void CreateBuildingVisual(BuildingData building)
        {
            var config = _buildingDatabase.GetConfig(building.type);
            if (config?.prefab != null)
            {
                Vector3 worldPos = new Vector3(building.position.x, building.position.y, 0);
                var visual = Instantiate(config.prefab, worldPos, Quaternion.identity);
                building.visual = visual;
                _buildingVisuals[building.position] = visual;
            }
        }

        public void DestroyBuildingVisual(Vector2 position)
        {
            if (_buildingVisuals.TryGetValue(position, out var visual))
            {
                if (visual != null)
                    Destroy(visual);
                _buildingVisuals.Remove(position);
            }
        }

        public void UpdateBuildingVisual(BuildingData building)
        {
            if (building.visual != null)
            {
                // Example: change scale or material to show upgrade
                building.visual.transform.localScale = Vector3.one * (1f + building.level * 0.1f);
            }
        }

        private void CreateBuildingButtons(List<BuildingType> availableTypes)
        {
            if (_buildingButtonPrefab == null || _buildingButtonsParent == null)
                return;

            foreach (var type in availableTypes)
            {
                var buttonObj = Instantiate(_buildingButtonPrefab, _buildingButtonsParent);
                var button = buttonObj.GetComponent<UnityEngine.UI.Button>();
                var image = buttonObj.GetComponent<UnityEngine.UI.Image>();

                var config = _buildingDatabase.GetConfig(type);
                if (config?.icon != null && image != null)
                    image.sprite = config.icon;

                if (button != null)
                {
                    BuildingType capturedType = type;
                    button.onClick.AddListener(() => OnBuildingTypeSelected?.Invoke(capturedType));
                }
            }
        }

        private void ClearBuildingButtons()
        {
            if (_buildingButtonsParent == null)
                return;

            for (int i = _buildingButtonsParent.childCount - 1; i >= 0; i--)
            {
                Destroy(_buildingButtonsParent.GetChild(i).gameObject);
            }
        }

        private void ShowTileHighlight(Vector2 position, bool isValid)
        {
            HideTileHighlight();

            if (_tileHighlightPrefab != null)
            {
                Vector3 worldPos = new Vector3(position.x, position.y, -0.1f);
                _currentHighlight = Instantiate(_tileHighlightPrefab, worldPos, Quaternion.identity);

                var renderer = _currentHighlight.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = isValid ? _validPlacementMaterial : _invalidPlacementMaterial;
                }
            }
        }

        private void HideTileHighlight()
        {
            if (_currentHighlight != null)
            {
                Destroy(_currentHighlight);
                _currentHighlight = null;
            }
        }

        private void OnDestroy()
        {
            if (_clickInputAction != null)
            {
                _clickInputAction.performed -= OnClickPerformed;
            }

            if (_clickAction == null)
            {
                _clickInputAction?.Dispose();
                _pointerPositionInputAction?.Dispose();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            for (float x = -10; x <= 10; x += _gridSize)
            {
                for (float y = -10; y <= 10; y += _gridSize)
                {
                    Gizmos.DrawWireCube(new Vector3(x, y, 0), new Vector3(_gridSize, _gridSize, 0));
                }
            }
        }
    }
}