using System.Collections;
using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public GameObject airplanePrefab;
    public GameObject bombPrefab;
    public GameObject airplaneActivatorPrefab;

    public float speed = 5f;
    public float heightAbovePlayer = 10f;
    public float bombInterval = 3f;
    public float flyInterval = 300f;
    public float bombDropHeight = -2f;
    public float airplaneLifetime = 10f;

    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    private GameObject player;
    private bool isAirplaneAvailable = false;
    private GameObject currentActivator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnAirplaneActivatorRoutine());
    }

    private IEnumerator SpawnAirplaneActivatorRoutine()
    {
        while (true)
        {
            if (currentActivator == null)
            {
                yield return new WaitForSeconds(flyInterval);
                isAirplaneAvailable = true;
                SpawnActivatorObject();
            }
            yield return null;
        }
    }

    private void SpawnActivatorObject()
    {
        Vector2 randomPos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        currentActivator = Instantiate(airplaneActivatorPrefab, randomPos, Quaternion.identity);
    }

    public void ActivateAirplane()
    {
        if (!isAirplaneAvailable || player == null || airplanePrefab == null) return;

        isAirplaneAvailable = false;

        if (currentActivator != null)
        {
            Destroy(currentActivator);
            currentActivator = null;
        }

        Vector3 spawnPosition = new Vector3(
            player.transform.position.x - 50f,
            player.transform.position.y + heightAbovePlayer,
            0
        );

        Quaternion rotation = Quaternion.Euler(0f, 0f, -90f);
        GameObject airplaneInstance = Instantiate(airplanePrefab, spawnPosition, rotation);
        airplaneInstance.SetActive(true);

        Rigidbody2D rb = airplaneInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(speed, 0);
        }

        Coroutine dropBombRoutine = StartCoroutine(DropBombRoutine(airplaneInstance));
        StartCoroutine(DeactivateAirplaneAfterTime(airplaneInstance, dropBombRoutine));
    }

    private IEnumerator DropBombRoutine(GameObject airplane)
    {
        while (airplane != null && airplane.activeInHierarchy)
        {
            DropBomb(airplane.transform);
            yield return new WaitForSeconds(bombInterval);
        }
    }

    private void DropBomb(Transform airplaneTransform)
    {
        Vector3 bombOffset = new Vector3(0, bombDropHeight, 0);
        Vector3 bombPosition = airplaneTransform.position + bombOffset;

        GameObject bombInstance = Instantiate(bombPrefab, bombPosition, Quaternion.identity);

        Rigidbody2D bombRb = bombInstance.GetComponent<Rigidbody2D>();
        if (bombRb != null)
        {
            bombRb.velocity = new Vector2(0, -5f);
        }
    }

    private IEnumerator DeactivateAirplaneAfterTime(GameObject airplane, Coroutine dropBombRoutine)
    {
        yield return new WaitForSeconds(airplaneLifetime);

        if (airplane != null)
        {
            if (dropBombRoutine != null)
            {
                StopCoroutine(dropBombRoutine);
            }

            airplane.SetActive(false);
        }
    }
}
