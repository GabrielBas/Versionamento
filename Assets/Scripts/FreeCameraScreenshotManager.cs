using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class FreeCameraScreenshotManager : MonoBehaviour
{
    public Camera screenshotCamera;
    public string[] tagsToHide;
    public GameObject levelEndPanel;
    public Button screenshotButton;
    public TMP_Text instructionsText;

    [Header("Movimentação")]
    public float cameraSpeed = 10f;     // WASD / Left Stick
    public float mouseDragSpeed = 0.5f; // Arraste com botão direito

    [Header("Zoom (estilo CameraZoomController)")]
    public float zoomStep = 0.5f;          // passo por “tick” do scroll
    public float zoomSpeedGamepad = 1.5f;  // RB/LB segurados
    public float minZoom = 2f;
    public float maxZoom = 50f;

    private bool isScreenshotMode = false;
    private Dictionary<GameObject, bool> hiddenObjects = new Dictionary<GameObject, bool>();
    private string defaultScreenshotDirectory;

    // 🔹 Input Actions (referências do seu asset para Screenshot/Exit/Move)
    public InputActionReference screenshotAction; // P ou Y
    public InputActionReference exitAction;       // O ou B
    public InputActionReference moveAction;       // WASD / Left Stick

    // 🔹 Actions locais para replicar o CameraZoomController
    private InputAction scrollAction;   // <Mouse>/scroll (Vector2)
    private InputAction zoomInAction;   // <Gamepad>/rightShoulder
    private InputAction zoomOutAction;  // <Gamepad>/leftShoulder

    // ATENÇÃO: agora é Vector2 (antes era Vector3)
    private Vector2 lastMousePosition;

    public Collider2D areaLimit; // arraste no inspector

    void Awake()
    {
        // Cria as actions de zoom no estilo do CameraZoomController
        scrollAction = new InputAction(type: InputActionType.Value, binding: "<Mouse>/scroll");
        zoomInAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/rightShoulder");
        zoomOutAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/leftShoulder");
    }

    void OnEnable()
    {
        screenshotAction.action.performed += OnScreenshot;
        exitAction.action.performed += OnExit;

        scrollAction.Enable();
        zoomInAction.Enable();
        zoomOutAction.Enable();
    }

    void OnDisable()
    {
        screenshotAction.action.performed -= OnScreenshot;
        exitAction.action.performed -= OnExit;

        scrollAction.Disable();
        zoomInAction.Disable();
        zoomOutAction.Disable();
    }

    void Start()
    {
        defaultScreenshotDirectory = Path.Combine(Application.dataPath, "Screenshots");
        if (!Directory.Exists(defaultScreenshotDirectory))
            Directory.CreateDirectory(defaultScreenshotDirectory);

        if (instructionsText != null)
            instructionsText.gameObject.SetActive(false);

        if (screenshotButton == null && levelEndPanel != null)
            screenshotButton = levelEndPanel.GetComponentInChildren<Button>();

        if (screenshotButton != null)
            screenshotButton.onClick.AddListener(EnterScreenshotMode);

        if (screenshotCamera != null)
            screenshotCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isScreenshotMode) return;

        HandleCameraMovement();
        HandleZoom();
        ClampToBounds(); // 🔹 Mantém a câmera dentro do collider
    }

    public void EnterScreenshotMode()
    {
        if (levelEndPanel != null && levelEndPanel.activeSelf)
        {
            isScreenshotMode = true;
            levelEndPanel.SetActive(false);
            HideObjectsWithTag();

            if (screenshotCamera != null)
                screenshotCamera.gameObject.SetActive(true);

            // Garante mouse visível e destravado
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (instructionsText != null)
            {
                instructionsText.gameObject.SetActive(true);
                instructionsText.text =
                    "Press 'P' or [Y] to take a screenshot.\n" +
                    "Press 'O' or [B] to exit print mode.\n" +
                    "Drag with Right Mouse to move camera.\n";
            }
        }
    }

    private void ExitScreenshotMode()
    {
        isScreenshotMode = false;
        ShowObjectsWithTag();

        if (screenshotCamera != null)
            screenshotCamera.gameObject.SetActive(false);

        // Mantém o cursor normal quando volta ao painel
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (levelEndPanel != null)
            levelEndPanel.SetActive(true);

        if (instructionsText != null)
            instructionsText.gameObject.SetActive(false);
    }

    private void HandleCameraMovement()
    {
        if (screenshotCamera == null) return;

        Vector3 move = Vector3.zero;

        // 🔸 Teclado/Analógico esquerdo
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        if (input.sqrMagnitude > 0.001f)
        {
            move += new Vector3(input.x, input.y, 0f) * cameraSpeed * Time.unscaledDeltaTime;
        }


        // 🔸 Arraste com botão direito do mouse
        if (Mouse.current != null)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                // captura a posição inicial quando começa a segurar
                lastMousePosition = Mouse.current.position.ReadValue();
            }

            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 current = Mouse.current.position.ReadValue();
                Vector2 mouseDelta = current - lastMousePosition;
                lastMousePosition = current;

                // Opcional: escalar pela aproximação para manter “sensação” proporcional ao zoom
                float zoomScale = screenshotCamera.orthographic ? Mathf.Max(0.25f, screenshotCamera.orthographicSize / 10f) : 1f;

                Vector3 drag = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f) * mouseDragSpeed * zoomScale * Time.unscaledDeltaTime;
                move += drag;
            }
        }

        screenshotCamera.transform.position += move;
    }

    private void HandleZoom()
    {
        if (screenshotCamera == null) return;

        float zoomDelta = 0f;

        // Scroll do mouse (passo fixo)
        Vector2 scroll = scrollAction.ReadValue<Vector2>();
        if (scroll.y > 0.01f)
            zoomDelta -= zoomStep;
        else if (scroll.y < -0.01f)
            zoomDelta += zoomStep;

        // Gamepad (segurar RB/LB = zoom contínuo)
        if (zoomInAction.IsPressed())
            zoomDelta -= zoomSpeedGamepad * Time.unscaledDeltaTime;

        if (zoomOutAction.IsPressed())
            zoomDelta += zoomSpeedGamepad * Time.unscaledDeltaTime;

        if (Mathf.Abs(zoomDelta) > 0f)
        {
            screenshotCamera.orthographicSize = Mathf.Clamp(
                screenshotCamera.orthographicSize + zoomDelta,
                minZoom,
                maxZoom
            );
        }
    }

    private void OnScreenshot(InputAction.CallbackContext ctx)
    {
        if (isScreenshotMode)
            StartCoroutine(CaptureScreenshotCoroutine());
    }

    private void OnExit(InputAction.CallbackContext ctx)
    {
        if (isScreenshotMode)
            ExitScreenshotMode();
    }

    private void HideObjectsWithTag()
    {
        foreach (string tag in tagsToHide)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                hiddenObjects[obj] = obj.activeSelf;
                obj.SetActive(false);
            }
        }
    }

    private void ShowObjectsWithTag()
    {
        foreach (var kvp in hiddenObjects)
            kvp.Key.SetActive(kvp.Value);

        hiddenObjects.Clear();
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        if (instructionsText != null)
            instructionsText.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();

        string filePath = Path.Combine(defaultScreenshotDirectory, $"screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot salva em: " + filePath);

        if (instructionsText != null)
            instructionsText.gameObject.SetActive(true);
    }
    private void ClampToBounds()
    {
        if (areaLimit == null) return;

        Bounds bounds = areaLimit.bounds;
        Vector3 pos = screenshotCamera.transform.position;

        pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
        pos.y = Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y);

        screenshotCamera.transform.position = pos;
    }
}
