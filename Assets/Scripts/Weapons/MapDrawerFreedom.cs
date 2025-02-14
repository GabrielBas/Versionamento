using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawerFreedom : MonoBehaviour
{
    public List<Camera> cameras;       // Lista de c�meras dispon�veis
    public GameObject trailPrefab;     // Prefab que cont�m o TrailRenderer
    public float followSpeed = 10f;    // Velocidade para o cursor acompanhar o mouse

    private bool isDrawing = false;    // Indica se est� desenhando ou n�o
    private GameObject currentTrail;   // Refer�ncia ao objeto instanciado do trail

    private Camera activeCamera;       // A c�mera atualmente ativa
    public Camera secondaryCamera;     // C�mera secund�ria para usar ao desativar o jogador

    public GameObject player;          // Refer�ncia ao jogador

    void Start()
    {
        if (cameras == null || cameras.Count == 0)
        {
            Debug.LogError("Nenhuma c�mera atribu�da! Adicione c�meras � lista no inspector.");
            return;
        }

        // Define a c�mera ativa como a primeira da lista
        activeCamera = cameras[0];
    }

    void Update()
    {
        // Alterna o estado do desenho ao pressionar o bot�o direito do mouse
        if (Input.GetMouseButtonDown(1))
        {
            isDrawing = !isDrawing; // Alterna entre ativar e desativar o desenho

            if (isDrawing)
            {
                StartNewTrail(); // Inicia um novo Trail
            }
            else
            {
                StopDrawing(); // Para de desenhar
            }
        }

        // Atualiza a posi��o do objeto de desenho se estiver em modo de desenho
        if (isDrawing && activeCamera != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(activeCamera.transform.position.z); // Ajusta a profundidade para c�mera 2D
            Vector3 worldPos = activeCamera.ScreenToWorldPoint(mousePos);
            transform.position = worldPos;

            // Move o objeto suavemente em dire��o ao cursor
            transform.position = Vector3.Lerp(transform.position, worldPos, followSpeed * Time.deltaTime);

            // Atualiza a posi��o do rastro atual
            if (currentTrail != null)
            {
                currentTrail.transform.position = transform.position;
            }
        }

        // Verifica se o jogador foi desativado
        if (player != null && !player.activeSelf)
        {
            SwitchToSecondaryCamera();
        }
    }

    private void StartNewTrail()
    {
        if (trailPrefab == null)
        {
            Debug.LogError("Trail Prefab n�o est� atribu�do!");
            return;
        }

        // Instancia o Prefab na posi��o atual do cursor
        currentTrail = Instantiate(trailPrefab, transform.position, Quaternion.identity);

        // Aplica o valor de largura do TrailWidthManager ao novo TrailRenderer
        ApplyTrailWidthToNewTrail();
    }

    private void StopDrawing()
    {
        // Para de desenhar no rastro atual
        if (currentTrail != null)
        {
            TrailRenderer trail = currentTrail.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.emitting = false; // Para de emitir o rastro
            }
        }
    }

    private void SwitchToSecondaryCamera()
    {
        if (secondaryCamera != null)
        {
            activeCamera = secondaryCamera;
            //Debug.Log("C�mera trocada para a c�mera secund�ria!");
        }
        else
        {
            Debug.LogError("C�mera secund�ria n�o foi atribu�da!");
        }
    }

    private void ApplyTrailWidthToNewTrail()
    {
        if (TrailWidthManager.Instance == null || currentTrail == null) return;

        TrailRenderer trailRenderer = currentTrail.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.startWidth = TrailWidthManager.Instance.TrailWidth;
            trailRenderer.endWidth = TrailWidthManager.Instance.TrailWidth;

            // For�a a atualiza��o do rastro
            // trailRenderer.Clear();
            Debug.Log("Espessura aplicada ao novo trail: " + trailRenderer.name);
        }
    }
}
