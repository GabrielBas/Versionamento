using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreeCameraScreenshotManager : MonoBehaviour
{
    public Camera screenshotCamera; // Câmera secundária para modo screenshot
    public EraserCursor eraserCursor; // Ativação do EraserCursor
    public string[] tagsToHide; // Tags dos objetos a serem escondidos
    public GameObject levelEndPanel; // Painel de fim de nível
    public Button screenshotButton; // Botão para iniciar o sistema de captura
    public TMP_Text instructionsText; // Texto de instruções para o jogador
    public float cameraSpeed = 10f; // Velocidade de movimento da câmera
    public float zoomSpeed = 5f; // Velocidade de zoom

    private bool isScreenshotMode = false; // Se o modo de screenshot foi ativado
    private Dictionary<GameObject, bool> hiddenObjects = new Dictionary<GameObject, bool>();
    private string defaultScreenshotDirectory;

    void Start()
    {
        // Configura o diretório padrão para salvar as screenshots
        defaultScreenshotDirectory = Path.Combine(Application.dataPath, "Screenshots");

        // Cria o diretório padrão caso ele não exista
        if (!Directory.Exists(defaultScreenshotDirectory))
        {
            Directory.CreateDirectory(defaultScreenshotDirectory);
            Debug.Log("Pasta padrão criada: " + defaultScreenshotDirectory);
        }

        // Garante que o texto de instruções está desativado no início
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
        }

        // Busca automática do botão se não for atribuído no inspetor
        if (screenshotButton == null && levelEndPanel != null)
        {
            screenshotButton = levelEndPanel.GetComponentInChildren<Button>();
        }

        if (screenshotButton != null)
        {
            screenshotButton.onClick.AddListener(EnterScreenshotMode); // Associa o botão para ativar o modo de screenshot
        }
        else
        {
            Debug.LogError("Botão de Screenshot não encontrado! Verifique o painel LevelEndPanel.");
        }

        // Garante que a câmera secundária está desativada no início
        if (screenshotCamera != null && eraserCursor != null)
        {
            screenshotCamera.gameObject.SetActive(false);
            eraserCursor.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Câmera secundária para o modo Screenshot e Borracha não atribuída!");
        }
    }

    void Update()
    {
        if (isScreenshotMode)
        {
            HandleCameraMovement();
            HandleZoom();

            // Captura a screenshot ao pressionar P
            if (Input.GetKeyDown(KeyCode.P))
            {
                StartCoroutine(CaptureScreenshotCoroutine());
            }

            // Sai do modo screenshot ao pressionar O
            if (Input.GetKeyDown(KeyCode.O))
            {
                ExitScreenshotMode();
            }
        }
    }

    public void EnterScreenshotMode()
    {
        if (levelEndPanel != null && levelEndPanel.activeSelf)
        {
            isScreenshotMode = true; // Ativa o modo Screenshot
            levelEndPanel.SetActive(false); // Esconde o painel
            HideObjectsWithTag(); // Esconde objetos com tags

            // Ativa a câmera secundária para o modo screenshot
            if (screenshotCamera != null && eraserCursor != null)
            {
                screenshotCamera.gameObject.SetActive(true);
                eraserCursor.gameObject.SetActive(true);
            }

            // Exibe o texto de instruções
            if (instructionsText != null)
            {
                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "Pressione 'P' para capturar uma screenshot.\nPressione 'O' para sair do modo screenshot.\nPressione 'E' para ativar a Borracha";
            }

            Debug.Log("Modo Screenshot e Borracha ativado. Use as teclas de movimentação ou o mouse para ajustar a posição.");
        }
    }

    private void ExitScreenshotMode()
    {
        isScreenshotMode = false;
        ShowObjectsWithTag(); // Restaura os objetos escondidos

        // Desativa a câmera secundária/Borracha e retorna ao painel
        if (screenshotCamera != null && eraserCursor != null)
        {
            screenshotCamera.gameObject.SetActive(false);
            eraserCursor.gameObject.SetActive(false);
        }

        if (levelEndPanel != null)
        {
            levelEndPanel.SetActive(true); // Mostra o painel novamente
        }

        // Esconde o texto de instruções
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
        }

        Debug.Log("Modo Screenshot desativado. Retornando ao painel.");
    }

    private void HandleCameraMovement()
    {
        // Movimentação pelo teclado
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horizontal, 0, vertical) * cameraSpeed * Time.deltaTime;

        if (screenshotCamera != null)
        {
            screenshotCamera.transform.position += move;
        }

        // Movimentação pelo mouse (arrastar)
        if (Input.GetMouseButton(0)) // Botão esquerdo do mouse para arrastar
        {
            Vector3 mouseDelta = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0);
            if (screenshotCamera != null)
            {
                screenshotCamera.transform.position += mouseDelta * cameraSpeed * 0.1f;
            }
        }
    }

    private void HandleZoom()
    {
        // Zoom usando a roda do mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && screenshotCamera != null)
        {
            if (screenshotCamera.orthographic)
            {
                screenshotCamera.orthographicSize = Mathf.Clamp(screenshotCamera.orthographicSize - scroll * zoomSpeed, 5f, 50f);
            }
            else
            {
                screenshotCamera.transform.position += screenshotCamera.transform.forward * scroll * zoomSpeed;
            }
        }
    }

    private void HideObjectsWithTag()
    {
        foreach (string tag in tagsToHide)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                hiddenObjects[obj] = obj.activeSelf; // Salva o estado original
                obj.SetActive(false);
            }
        }
    }

    private void ShowObjectsWithTag()
    {
        foreach (var kvp in hiddenObjects)
        {
            kvp.Key.SetActive(kvp.Value); // Restaura o estado original
        }
        hiddenObjects.Clear();
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false); // Esconde o texto antes da captura
        }

        yield return new WaitForEndOfFrame();

        // Usa o diretório padrão para salvar screenshots
        string directory = defaultScreenshotDirectory;

        // Define o nome do arquivo
        string filePath = Path.Combine(directory, $"screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");

        // Captura a screenshot
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot salva em: " + filePath);

        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(true); // Restaura o texto após a captura
        }
    }
}
