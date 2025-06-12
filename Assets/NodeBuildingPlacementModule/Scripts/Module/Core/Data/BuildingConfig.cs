using System;
using UnityEngine;

namespace NodeBuildingPlacementModule
{
    [Serializable]
    public class BuildingConfig
    {
        public BuildingType type;
        public GameObject prefab;
        public Sprite icon;
    }
}