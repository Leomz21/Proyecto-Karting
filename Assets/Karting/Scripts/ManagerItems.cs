using UnityEngine;
using KartGame.KartSystems;

public class ManagerItems : MonoBehaviour
{
    [Header("Puntos de aparicion")]
    public Transform[] spawnPoints;

    [Header("Objetos que pueden aparecer")]
    public GameObject powerupPrefab;
    public GameObject obstaclePrefab;

    [Header("Deteccion")]
    public float detectionRadius = 15f;

    GameObject[] activeItems;
    ArcadeKart playerKart;

    void Start()
    {
        activeItems = new GameObject[spawnPoints.Length];
        playerKart = FindObjectOfType<ArcadeKart>();
    }

    void Update()
    {
        if (playerKart == null) return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (activeItems[i] != null) continue;

            float d = Vector3.Distance(
                playerKart.transform.position,
                spawnPoints[i].position);

            if (d <= detectionRadius)
                SpawnRandomItem(i);
        }
    }

    void SpawnRandomItem(int index)
    {
        GameObject chosen = Random.value < 0.5f
            ? powerupPrefab : obstaclePrefab;

        GameObject item = Instantiate(
            chosen,
            spawnPoints[index].position,
            spawnPoints[index].rotation);

        activeItems[index] = item;

        var powerup = item.GetComponent<ArcadeKartPowerup>();
        int captured = index;

        if (powerup != null)
            powerup.onPowerupActivated.AddListener(
                () => OnItemCollected(captured));
    }

    void OnItemCollected(int index)
    {
        if (activeItems[index] != null)
        {
            Destroy(activeItems[index]);
            activeItems[index] = null;
        }
    }
}