using System;
using UnityEngine;

namespace NodeBuildingPlacementModule
{
    [Serializable]
    public class BuildingData
    {
        public BuildingType type;
        public Vector2 position;
        public int level;
        public float damage;
        public float range;
        public float attackSpeed;
        public int cost;
        public int upgradeCost;
        public GameObject visual;
        /// Additional properties can be added as needed

        public BuildingData(BuildingType type, Vector2 position)
        {
            this.type = type;
            this.position = position;
            level = 1;

            switch (type)
            {
                case BuildingType.BasicTower:
                    damage = 10f;
                    range = 3f;
                    attackSpeed = 1f;
                    cost = 50;
                    upgradeCost = 75;
                    break;
                case BuildingType.AdvancedTower:
                    damage = 25f;
                    range = 4f;
                    attackSpeed = 0.8f;
                    cost = 100;
                    upgradeCost = 150;
                    break;
                case BuildingType.SlowTower:
                    damage = 5f;
                    range = 3.5f;
                    attackSpeed = 2f;
                    cost = 75;
                    upgradeCost = 100;
                    break;
                case BuildingType.SplashTower:
                    damage = 15f;
                    range = 2.5f;
                    attackSpeed = 1.5f;
                    cost = 125;
                    upgradeCost = 200;
                    break;
            }
        }

        public void Upgrade()
        {
            level++;
            damage *= 1.5f;
            range *= 1.1f;
            attackSpeed *= 0.9f;
            upgradeCost = Mathf.RoundToInt(upgradeCost * 1.5f);
        }
    }
}