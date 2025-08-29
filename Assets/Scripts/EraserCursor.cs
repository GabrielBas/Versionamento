using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // <- Novo Input System

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
        if (Keyboard.current.eKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
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
            if (Mouse.current != null && Mouse.current.position.IsActuated())
            {
                Vector3 mousePos = Mouse.current.position.ReadValue();
                mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
                worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            }

            // Movimento pelo analógico direito do controle (novo input system)
            if (Gamepad.current != null)
            {
                Vector2 rightStick = Gamepad.current.rightStick.ReadValue();

                if (rightStick.sqrMagnitude > 0.01f)
                {
                    worldPos = cursorPos + new Vector3(rightStick.x, rightStick.y, 0) * analogSensitivity * Time.deltaTime;
                }
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
