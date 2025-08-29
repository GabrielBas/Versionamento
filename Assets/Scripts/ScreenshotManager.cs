using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem; // <-- Necessário para o Input System

public class FreeCameraScreenshotManager : MonoBehaviour
{
    public Camera screenshotCamera;
    //public EraserCursor eraserCursor;
    public string[] tagsToHide;
    public GameObject levelEndPanel;
    public Button screenshotButton;
    public TMP_Text instructionsText;
    public float cameraSpeed = 10f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 50f;
    public float minCameraSpeed = 2f;
    public float maxCameraSpeed = 10f;

    private bool isScreenshotMode = false;
    private Dictionary<GameObject, bool> hiddenObjects = new Dictionary<GameObject, bool>();
    private string defaultScreenshotDirectory;

    // 🔹 Referência para Input Actions
    public InputActionReference screenshotAction; // P ou Y
    public InputActionReference exitAction;       // O ou B
    //public InputActionReference eraserAction;     // E ou A
    public InputActionReference moveAction;       // WASD / Left Stick
    public InputActionReference zoomAction;       // Scroll / RT-LT

    void OnEnable()
    {
        screenshotAction.action.performed += OnScreenshot;
        exitAction.action.performed += OnExit;
        //eraserAction.action.performed += OnEraser;
    }

    void OnDisable()
    {
        screenshotAction.action.performed -= OnScreenshot;
        exitAction.action.performed -= OnExit;
        //eraserAction.action.performed -= OnEraser;
    }

    void Start()
    {
        defaultScreenshotDirectory = Path.Combine(Application.dataPath, "Screenshots");

        if (!Directory.Exists(defaultScreenshotDirectory))
        {
            Directory.CreateDirectory(defaultScreenshotDirectory);
            Debug.Log("Pasta padrão criada: " + defaultScreenshotDirectory);
        }

        if (instructionsText != null)
            instructionsText.gameObject.SetActive(false);

        if (screenshotButton == null && levelEndPanel != null)
            screenshotButton = levelEndPanel.GetComponentInChildren<Button>();

        if (screenshotButton != null)
            screenshotButton.onClick.AddListener(EnterScreenshotMode);
        else
            Debug.LogError("Botão de Screenshot não encontrado!");

        //if (screenshotCamera != null && eraserCursor != null)
        //{
        //    screenshotCamera.gameObject.SetActive(false);
        //    eraserCursor.gameObject.SetActive(false);
        //}

        if (screenshotCamera != null)
        {
            screenshotCamera.gameObject.SetActive(false);
            
        }
    }

    void Update()
    {
        if (isScreenshotMode)
        {
            HandleCameraMovement();
            HandleZoom();
        }
    }

    public void EnterScreenshotMode()
    {
        if (levelEndPanel != null && levelEndPanel.activeSelf)
        {
            isScreenshotMode = true;
            levelEndPanel.SetActive(false);
            HideObjectsWithTag();

            if (screenshotCamera != null)
            {
                screenshotCamera.gameObject.SetActive(true);
                
            }

            //if (screenshotCamera != null && eraserCursor != null)
            //{
            //    screenshotCamera.gameObject.SetActive(true);
            //    eraserCursor.gameObject.SetActive(true);
            //}

            if (instructionsText != null)
            {
                instructionsText.gameObject.SetActive(true);
                instructionsText.text =
                    "Press 'P' or [Y] to take a screenshot.\n" +
                    "Press 'O' or [B] to exit print mode.\n";
                    
            }
        }
    }

    private void ExitScreenshotMode()
    {
        isScreenshotMode = false;
        ShowObjectsWithTag();

        if (screenshotCamera != null)
        {
            screenshotCamera.gameObject.SetActive(false);
            
        }

        //if (screenshotCamera != null && eraserCursor != null)
        //{
        //    screenshotCamera.gameObject.SetActive(false);
        //    eraserCursor.gameObject.SetActive(false);
        //}

        if (levelEndPanel != null)
            levelEndPanel.SetActive(true);

        if (instructionsText != null)
            instructionsText.gameObject.SetActive(false);
    }

    private void HandleCameraMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>(); // WASD ou Left Stick
        Vector3 move = new Vector3(input.x, 0, input.y) * cameraSpeed * Time.deltaTime;

        if (screenshotCamera != null)
            screenshotCamera.transform.position += move;
    }

    private void HandleZoom()
    {
        float scroll = zoomAction.action.ReadValue<float>(); // Scroll ou RT/LT

        if (scroll != 0 && screenshotCamera != null)
        {
            if (screenshotCamera.orthographic)
            {
                float oldSize = screenshotCamera.orthographicSize;
                screenshotCamera.orthographicSize = Mathf.Clamp(
                    oldSize - scroll * zoomSpeed,
                    minZoom,
                    maxZoom
                );

                float t = Mathf.InverseLerp(minZoom, maxZoom, screenshotCamera.orthographicSize);
                cameraSpeed = Mathf.Lerp(minCameraSpeed, maxCameraSpeed, t);
            }
            else
            {
                screenshotCamera.transform.position += screenshotCamera.transform.forward * scroll * zoomSpeed;
            }
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

    //private void OnEraser(InputAction.CallbackContext ctx)
    //{
    //    if (isScreenshotMode && eraserCursor != null)
    //    {
    //        eraserCursor.gameObject.SetActive(!eraserCursor.gameObject.activeSelf);
    //        Debug.Log("Eraser " + (eraserCursor.gameObject.activeSelf ? "Ativado" : "Desativado"));
    //    }
    //}

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

        string directory = defaultScreenshotDirectory;
        string filePath = Path.Combine(directory, $"screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");

        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot salva em: " + filePath);

        if (instructionsText != null)
            instructionsText.gameObject.SetActive(true);
    }
}
