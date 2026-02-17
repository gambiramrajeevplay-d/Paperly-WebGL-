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
            Debug.LogError("[PlayerSpawn] No rocketPrefabs assigned!");
            return;
        }

        // 🔹 Use SAME key as CarControllerUI
        int selectedRocket = PlayerPrefs.GetInt("selectedCar", 0);

        if (selectedRocket < 0 || selectedRocket >= rocketPrefabs.Length)
        {
            selectedRocket = 0;
        }

        GameObject rocketToSpawn = rocketPrefabs[selectedRocket];

        if (rocketToSpawn == null)
        {
            Debug.LogError($"[PlayerSpawn] rocketPrefabs[{selectedRocket}] is NULL.");
            return;
        }

        GameObject spawnedRocket =
            Instantiate(rocketToSpawn, transform.position, transform.rotation);

        GameObject level = GameObject.Find("Level");
        if (level != null)
        {
            spawnedRocket.transform.SetParent(level.transform);
        }

        Debug.Log($"[PlayerSpawn] Spawned rocket index {selectedRocket}: {rocketToSpawn.name}");
    }
}
