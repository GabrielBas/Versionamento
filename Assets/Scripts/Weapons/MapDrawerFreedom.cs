using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawerFreedom : MonoBehaviour
{
    public List<Camera> cameras;
    public GameObject trailPrefab;
    public float followSpeed = 10f;

    public GameObject levelUpPanel; // 🔥 Painel de Level Up (ou qualquer UI que pause)

    private bool isDrawing = false;
    private GameObject currentTrail;

    private Camera activeCamera;
    public Camera secondaryCamera;

    public GameObject player;

    void Start()
    {
        if (cameras == null || cameras.Count == 0)
        {
            Debug.LogError("Nenhuma câmera atribuída! Adicione câmeras à lista no inspector.");
            return;
        }

        activeCamera = cameras[0];
    }

    void Update()
    {
        // 🔥 Verifica se o painel está ativo e força desativar o desenho
        if (levelUpPanel != null && levelUpPanel.activeSelf)
        {
            if (isDrawing)
            {
                StopDrawing();
                isDrawing = false;
            }
            return; // Bloqueia qualquer tentativa de desenhar enquanto o painel está ativo
        }

        // Ativar ou desativar o desenho com o botão direito
        if (Input.GetMouseButtonDown(1))
        {
            isDrawing = !isDrawing;

            if (isDrawing)
            {
                StartNewTrail();
            }
            else
            {
                StopDrawing();
            }
        }

        // Atualiza a posição do cursor
        if (isDrawing && activeCamera != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(activeCamera.transform.position.z);
            Vector3 worldPos = activeCamera.ScreenToWorldPoint(mousePos);

            transform.position = Vector3.Lerp(transform.position, worldPos, followSpeed * Time.deltaTime);

            if (currentTrail != null)
            {
                currentTrail.transform.position = transform.position;
            }
        }

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

        currentTrail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
        ApplyTrailWidthToNewTrail();
    }

    private void StopDrawing()
    {
        if (currentTrail != null)
        {
            TrailRenderer trail = currentTrail.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.emitting = false;
            }
        }
    }

    private void SwitchToSecondaryCamera()
    {
        if (secondaryCamera != null)
        {
            activeCamera = secondaryCamera;
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
        }
    }
}
