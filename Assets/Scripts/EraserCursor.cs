using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserCursor : MonoBehaviour
{
    public Camera mainCamera;         // A c�mera para detectar a posi��o do mouse
    public GameObject trailPrefab;    // Prefab que cont�m o TrailRenderer
    public float followSpeed = 10f;   // Velocidade para o cursor acompanhar o mouse

    private bool isDrawing = false;   // Indica se est� desenhando ou n�o
    private GameObject currentTrail;  // Refer�ncia ao objeto instanciado do trail

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Alterna o estado do desenho ao pressionar E
        if (Input.GetKeyDown(KeyCode.E))
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
        if (isDrawing)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(mainCamera.transform.position.z); // Ajusta a profundidade para c�mera 2D
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            transform.position = worldPos;

            // Move o objeto suavemente em dire��o ao cursor
            transform.position = Vector3.Lerp(transform.position, worldPos, followSpeed * Time.deltaTime);

            // Atualiza a posi��o do rastro atual
            if (currentTrail != null)
            {
                currentTrail.transform.position = transform.position;
            }
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
    void OnDisable()
    {
        if (currentTrail != null)
        {
            Destroy(currentTrail);
        }
    }

}



