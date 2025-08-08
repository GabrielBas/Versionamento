using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapDrawerFreedom : MonoBehaviour
{
    public List<Camera> cameras;
    public GameObject trailPrefab;
    public float followSpeed = 10f;

    public GameObject levelUpPanel;
    public GameObject player;
    public Camera secondaryCamera;

    private bool isDrawing = false;
    private GameObject currentTrail;
    private Camera activeCamera;

    private InputAction drawAction;
    private InputAction rightStickAction;

    private Vector2 virtualPointerPos;
    public float rightStickSensitivity = 1000f;

    private enum InputMode { Mouse, Gamepad }
    private InputMode currentInputMode = InputMode.Mouse;

    void Awake()
    {
        drawAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/rightButton");
        drawAction.AddBinding("<Gamepad>/buttonSouth");
        drawAction.Enable();

        rightStickAction = new InputAction(type: InputActionType.Value, binding: "<Gamepad>/rightStick");
        rightStickAction.Enable();
    }

    void OnDestroy()
    {
        drawAction.Disable();
        rightStickAction.Disable();
    }

    void Start()
    {
        if (cameras == null || cameras.Count == 0)
        {
            Debug.LogError("Nenhuma câmera atribuída!");
            return;
        }

        activeCamera = cameras[0];
        virtualPointerPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        if (levelUpPanel != null && levelUpPanel.activeSelf)
        {
            if (isDrawing)
            {
                StopDrawing();
                isDrawing = false;
            }
            return;
        }

        if (drawAction.WasPressedThisFrame())
        {
            isDrawing = !isDrawing;
            if (isDrawing) StartNewTrail();
            else StopDrawing();
        }

        // Detecta tipo de entrada atual
        Vector2 stickInput = rightStickAction.ReadValue<Vector2>();
        bool isUsingGamepad = stickInput.magnitude > 0.1f;

        if (isUsingGamepad)
            currentInputMode = InputMode.Gamepad;
        else if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0f)
            currentInputMode = InputMode.Mouse;

        // Atualiza posição do cursor virtual
        if (isDrawing && activeCamera != null)
        {
            if (currentInputMode == InputMode.Gamepad)
            {
                virtualPointerPos += stickInput * rightStickSensitivity * Time.deltaTime;
                virtualPointerPos = ClampToScreenBounds(virtualPointerPos);
            }
            else if (currentInputMode == InputMode.Mouse)
            {
                virtualPointerPos = Mouse.current.position.ReadValue();
            }

            Vector3 pointerScreen = new Vector3(virtualPointerPos.x, virtualPointerPos.y, Mathf.Abs(activeCamera.transform.position.z));
            Vector3 pointerWorld = activeCamera.ScreenToWorldPoint(pointerScreen);

            transform.position = Vector3.Lerp(transform.position, pointerWorld, followSpeed * Time.deltaTime);

            if (currentTrail != null)
                currentTrail.transform.position = transform.position;
        }

        if (player != null && !player.activeSelf)
        {
            SwitchToSecondaryCamera();
        }
    }

    private Vector2 ClampToScreenBounds(Vector2 pos)
    {
        return new Vector2(
            Mathf.Clamp(pos.x, 0, Screen.width),
            Mathf.Clamp(pos.y, 0, Screen.height)
        );
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
                trail.emitting = false;
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
            Debug.LogError("Câmera secundária não atribuída!");
        }
    }

    private void ApplyTrailWidthToNewTrail()
    {
        if (TrailWidthManager.Instance == null || currentTrail == null) return;

        TrailRenderer trailRenderer = currentTrail.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            float width = TrailWidthManager.Instance.TrailWidth;
            trailRenderer.startWidth = width;
            trailRenderer.endWidth = width;
        }
    }
}