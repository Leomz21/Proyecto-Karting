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
    bool[] waitingForPlayerToLeave;
    ArcadeKart playerKart;

    void Start()
    {
        activeItems = new GameObject[spawnPoints.Length];
        waitingForPlayerToLeave = new bool[spawnPoints.Length];
        playerKart = FindObjectOfType<ArcadeKart>();
    }

    void Update()
    {
        if (playerKart == null) return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            float distance = Vector3.Distance(
                playerKart.transform.position,
                spawnPoints[i].position);

            // Después de recoger una bola, espera a que el kart
            // salga del área antes de permitir otra.
            if (waitingForPlayerToLeave[i])
            {
                if (distance > detectionRadius)
                    waitingForPlayerToLeave[i] = false;

                continue;
            }

            if (activeItems[i] != null) continue;

            if (distance <= detectionRadius)
                SpawnRandomItem(i);
        }
    }

    void SpawnRandomItem(int index)
    {
        GameObject chosen = Random.value < 0.5f
            ? powerupPrefab
            : obstaclePrefab;

        GameObject item = Instantiate(
            chosen,
            spawnPoints[index].position,
            spawnPoints[index].rotation);

        activeItems[index] = item;

        ArcadeKartPowerup powerup =
            item.GetComponent<ArcadeKartPowerup>();

        int capturedIndex = index;

        if (powerup != null)
        {
            powerup.onPowerupActivated.AddListener(
                () => OnItemCollected(capturedIndex));
        }
    }

    void OnItemCollected(int index)
    {
        if (activeItems[index] != null)
        {
            Destroy(activeItems[index]);
            activeItems[index] = null;
        }

        waitingForPlayerToLeave[index] = true;
    }
}