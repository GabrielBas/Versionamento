using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneController : MonoBehaviour
{
    public GameObject airplanePrefab; // Prefab do avião
    public GameObject bombPrefab; // Prefab da bomba
    public float speed = 5f; // Velocidade do avião
    public float heightAbovePlayer = 10f; // Altura do avião acima do jogador
    public float bombInterval = 3f; // Intervalo de tempo entre lançamentos de bombas
    public float flyInterval = 300f; // Intervalo de tempo entre ativações do botão
    public float bombDropHeight = -2f; // Altura de lançamento das bombas abaixo do avião
    public float airplaneLifetime = 10f; // Tempo de vida do avião

    public Button activateAirplaneButton; // Botão da UI para ativar o avião

    private GameObject player;
    private bool isAirplaneAvailable = false; // Controla se o avião pode ser ativado

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        activateAirplaneButton.gameObject.SetActive(false); // Botão inicialmente desativado
        activateAirplaneButton.onClick.AddListener(ActivateAirplane); // Adiciona o evento ao botão
        StartCoroutine(ManageAirplaneButtonRoutine());
    }

    private IEnumerator ManageAirplaneButtonRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(flyInterval);
            isAirplaneAvailable = true; // Habilita a disponibilidade do avião
            activateAirplaneButton.gameObject.SetActive(true); // Mostra o botão na UI
        }
    }

    private void ActivateAirplane()
    {
        if (!isAirplaneAvailable || player == null || airplanePrefab == null) return;

        // Desativa o botão e reseta a disponibilidade do avião
        activateAirplaneButton.gameObject.SetActive(false);
        isAirplaneAvailable = false;

        // Instancia o avião acima do jogador com rotação de -90° no eixo Z
        Vector3 spawnPosition = new Vector3(player.transform.position.x - 50f, player.transform.position.y + heightAbovePlayer, 0);
        Quaternion rotation = Quaternion.Euler(0f, 0f, -90f); // Define a rotação no eixo Z
        GameObject airplaneInstance = Instantiate(airplanePrefab, spawnPosition, rotation);

        // Ativa o avião e inicia o movimento
        airplaneInstance.SetActive(true);
        Rigidbody2D rb = airplaneInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(speed, 0);
        }

        // Inicia o lançamento de bombas e o timer para desativar o avião
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
