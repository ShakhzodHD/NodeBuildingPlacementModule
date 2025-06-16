using NodeBuildingPlacementModule;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BuildingDatabase buildingDatabase;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singleton = new(typeof(GameManager).Name);
                instance = singleton.AddComponent<GameManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(BuildingPlacementModel buildingModel)
    {
        if (buildingModel != null)
        {
            buildingModel.OnBuildingPlaced += OnBuildingPlaced;
            buildingModel.OnBuildingUpgraded += OnBuildingUpgraded;
        }
    }

    private void OnBuildingPlaced(BuildingData building)
    {
        Debug.Log($"Tower {building.type} placed at {building.position}");
    }

    private void OnBuildingUpgraded(BuildingData building)
    {
        Debug.Log($"Tower upgraded to level {building.level}");
    }
}