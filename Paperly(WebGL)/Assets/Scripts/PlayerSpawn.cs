using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [Header("All Truck Prefabs")]
    public GameObject[] rocketPrefabs;

    private void Awake()
    {
        if (rocketPrefabs == null || rocketPrefabs.Length == 0)
        {
            Debug.LogError("[PlayerSpawn] No truckPrefabs assigned!");
            return;
        }

        int selectedrocket = 0;

        if (PlayerPrefs.HasKey("selectedrocket"))
        {
            selectedrocket = PlayerPrefs.GetInt("selectedrocket", 0);
        }
        else if (PlayerPrefs.HasKey("car"))
        {
            selectedrocket = PlayerPrefs.GetInt("car", 0);
        }

        if (selectedrocket < 0 || selectedrocket >= rocketPrefabs.Length)
        {
            selectedrocket = 0;
        }

        GameObject truckToSpawn = rocketPrefabs[selectedrocket];
        if (truckToSpawn == null)
        {
            Debug.LogError($"[PlayerSpawn] truckPrefabs[{selectedrocket}] is NULL.");
            return;
        }

        // 🔹 Spawn EXACTLY the same way
        GameObject spawnedTruck =
            Instantiate(truckToSpawn, transform.position, transform.rotation);

        // 🔹 ONLY THIS: put inside Level-1
        GameObject level1 = GameObject.Find("Level");
        if (level1 != null)
        {
            spawnedTruck.transform.SetParent(level1.transform);
        }
        else
        {
            Debug.LogError("Level-1 GameObject not found in scene!");
        }

        Debug.Log($"[PlayerSpawn] Spawned truck index {selectedrocket}: {truckToSpawn.name}");
    }
}