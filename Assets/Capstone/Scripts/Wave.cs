using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Wave
{

    [Serializable]
    public struct EnemySpawn
    {
        [Serializable]
        public struct SpawningInformation
        {
            public float Wait;
            public GameObject EnemyPrefab;
            public float NumberOfEnemies;
            public float TimeBetweenEnemySpawn;
        }
        
        public Transform spawnPoint;
        public List<SpawningInformation> spawning;
    }

    public List<EnemySpawn> EnemySpawns = new List<EnemySpawn>();


    private List<GameObject> Enemies = new List<GameObject>();
}
