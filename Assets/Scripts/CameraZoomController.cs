using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoomController : MonoBehaviour
{
    public float zoomStep = 0.5f;   // Valor fixo por "tick" de scroll ou por botão
    public float zoomSpeedGamepad = 1.5f; // Velocidade progressiva para gamepad (segura o botão)
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private Camera cam;

    private InputAction scrollAction;
    private InputAction zoomInAction;   // RB
    private InputAction zoomOutAction;  // LB

    void Awake()
    {
        scrollAction = new InputAction(type: InputActionType.Value, binding: "<Mouse>/scroll");
        scrollAction.Enable();

        zoomInAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/rightShoulder");
        zoomInAction.Enable();

        zoomOutAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/leftShoulder");
        zoomOutAction.Enable();
    }

    void OnDestroy()
    {
        scrollAction.Disable();
        zoomInAction.Disable();
        zoomOutAction.Disable();
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleZoom();
    }

    private void HandleZoom()
    {
        float zoomDelta = 0f;

        // Scroll do mouse: tratar como evento de passo fixo
        Vector2 scroll = scrollAction.ReadValue<Vector2>();
        if (scroll.y > 0.01f)
            zoomDelta -= zoomStep;
        else if (scroll.y < -0.01f)
            zoomDelta += zoomStep;

        // Gamepad (segurar = zoom contínuo)
        if (zoomInAction.IsPressed())
            zoomDelta -= zoomSpeedGamepad * Time.deltaTime;

        if (zoomOutAction.IsPressed())
            zoomDelta += zoomSpeedGamepad * Time.deltaTime;

        if (Mathf.Abs(zoomDelta) > 0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomDelta, minZoom, maxZoom);
        }
    }
}
