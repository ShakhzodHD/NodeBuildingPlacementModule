using UnityEngine;

namespace NodeBuildingPlacementModule
{
    [CreateAssetMenu(fileName = "BuildingDatabase", menuName = "Building Database")]
    public class BuildingDatabase : ScriptableObject
    {
        [SerializeField] private BuildingConfig[] buildings;

        public BuildingConfig GetConfig(BuildingType type)
        {
            foreach (var config in buildings)
            {
                if (config.type == type)
                    return config;
            }
            return null;
        }

        public BuildingConfig[] GetAllConfigs() => buildings;
    }
}