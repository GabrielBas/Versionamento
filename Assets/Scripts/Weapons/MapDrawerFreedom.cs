using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawerFreedom : MonoBehaviour
{
    public List<Camera> cameras;       // Lista de câmeras disponíveis
    public GameObject trailPrefab;     // Prefab que contém o TrailRenderer
    public float followSpeed = 10f;    // Velocidade para o cursor acompanhar o mouse

    private bool isDrawing = false;    // Indica se está desenhando ou não
    private GameObject currentTrail;   // Referência ao objeto instanciado do trail

    private Camera activeCamera;       // A câmera atualmente ativa
    public Camera secondaryCamera;     // Câmera secundária para usar ao desativar o jogador

    public GameObject player;          // Referência ao jogador

    void Start()
    {
        if (cameras == null || cameras.Count == 0)
        {
            Debug.LogError("Nenhuma câmera atribuída! Adicione câmeras à lista no inspector.");
            return;
        }

        // Define a câmera ativa como a primeira da lista
        activeCamera = cameras[0];
    }

    void Update()
    {
        // Alterna o estado do desenho ao pressionar o botão direito do mouse
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

        // Atualiza a posição do objeto de desenho se estiver em modo de desenho
        if (isDrawing && activeCamera != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(activeCamera.transform.position.z); // Ajusta a profundidade para câmera 2D
            Vector3 worldPos = activeCamera.ScreenToWorldPoint(mousePos);
            transform.position = worldPos;

            // Move o objeto suavemente em direção ao cursor
            transform.position = Vector3.Lerp(transform.position, worldPos, followSpeed * Time.deltaTime);

            // Atualiza a posição do rastro atual
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
            Debug.LogError("Trail Prefab não está atribuído!");
            return;
        }

        // Instancia o Prefab na posição atual do cursor
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
            //Debug.Log("Câmera trocada para a câmera secundária!");
        }
        else
        {
            Debug.LogError("Câmera secundária não foi atribuída!");
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

            // Força a atualização do rastro
            // trailRenderer.Clear();
            Debug.Log("Espessura aplicada ao novo trail: " + trailRenderer.name);
        }
    }
}
