using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserCursor : MonoBehaviour
{
    public Camera mainCamera;         // Câmera para detectar posição
    public GameObject trailPrefab;    // Prefab com TrailRenderer
    public float followSpeed = 10f;   // Velocidade para seguir
    public float analogSensitivity = 5f; // Sensibilidade do analógico

    private bool isDrawing = false;
    private GameObject currentTrail;
    private Vector3 cursorPos;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Começa na posição inicial do cursor
        cursorPos = transform.position;
    }

    void Update()
    {
        // Alterna desenho - E (teclado) ou botão Y (gamepad)
        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("buttonSouth"))
        {
            isDrawing = !isDrawing;

            if (isDrawing)
                StartNewTrail();
            else
                StopDrawing();
        }

        if (isDrawing)
        {
            Vector3 worldPos = cursorPos;

            // Movimento pelo mouse
            if (Input.mousePresent)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
                worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            }

            // Movimento pelo analógico direito do controle
            float moveX = Input.GetAxis("RightStickHorizontal");
            float moveY = Input.GetAxis("RightStickVertical");

            if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f)
            {
                worldPos = cursorPos + new Vector3(moveX, moveY, 0) * analogSensitivity * Time.deltaTime;
            }

            // Suaviza movimento
            cursorPos = Vector3.Lerp(cursorPos, worldPos, followSpeed * Time.deltaTime);
            transform.position = cursorPos;

            if (currentTrail != null)
                currentTrail.transform.position = transform.position;
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
    }

    private void StopDrawing()
    {
        if (currentTrail != null)
        {
            TrailRenderer trail = currentTrail.GetComponent<TrailRenderer>();
            if (trail != null)
                trail.emitting = false;
        }
    }

    void OnDisable()
    {
        if (currentTrail != null)
            Destroy(currentTrail);
    }
}
