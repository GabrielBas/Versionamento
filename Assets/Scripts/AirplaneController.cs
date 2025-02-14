using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneController : MonoBehaviour
{
    public GameObject airplanePrefab; // Prefab do avi�o
    public GameObject bombPrefab; // Prefab da bomba
    public float speed = 5f; // Velocidade do avi�o
    public float heightAbovePlayer = 10f; // Altura do avi�o acima do jogador
    public float bombInterval = 3f; // Intervalo de tempo entre lan�amentos de bombas
    public float flyInterval = 300f; // Intervalo de tempo entre ativa��es do bot�o
    public float bombDropHeight = -2f; // Altura de lan�amento das bombas abaixo do avi�o
    public float airplaneLifetime = 10f; // Tempo de vida do avi�o

    public Button activateAirplaneButton; // Bot�o da UI para ativar o avi�o

    private GameObject player;
    private bool isAirplaneAvailable = false; // Controla se o avi�o pode ser ativado

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        activateAirplaneButton.gameObject.SetActive(false); // Bot�o inicialmente desativado
        activateAirplaneButton.onClick.AddListener(ActivateAirplane); // Adiciona o evento ao bot�o
        StartCoroutine(ManageAirplaneButtonRoutine());
    }

    private IEnumerator ManageAirplaneButtonRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(flyInterval);
            isAirplaneAvailable = true; // Habilita a disponibilidade do avi�o
            activateAirplaneButton.gameObject.SetActive(true); // Mostra o bot�o na UI
        }
    }

    private void ActivateAirplane()
    {
        if (!isAirplaneAvailable || player == null || airplanePrefab == null) return;

        // Desativa o bot�o e reseta a disponibilidade do avi�o
        activateAirplaneButton.gameObject.SetActive(false);
        isAirplaneAvailable = false;

        // Instancia o avi�o acima do jogador com rota��o de -90� no eixo Z
        Vector3 spawnPosition = new Vector3(player.transform.position.x - 50f, player.transform.position.y + heightAbovePlayer, 0);
        Quaternion rotation = Quaternion.Euler(0f, 0f, -90f); // Define a rota��o no eixo Z
        GameObject airplaneInstance = Instantiate(airplanePrefab, spawnPosition, rotation);

        // Ativa o avi�o e inicia o movimento
        airplaneInstance.SetActive(true);
        Rigidbody2D rb = airplaneInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(speed, 0);
        }

        // Inicia o lan�amento de bombas e o timer para desativar o avi�o
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
