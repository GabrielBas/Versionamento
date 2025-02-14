using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreeCameraScreenshotManager : MonoBehaviour
{
    public Camera screenshotCamera; // C�mera secund�ria para modo screenshot
    public EraserCursor eraserCursor; // Ativa��o do EraserCursor
    public string[] tagsToHide; // Tags dos objetos a serem escondidos
    public GameObject levelEndPanel; // Painel de fim de n�vel
    public Button screenshotButton; // Bot�o para iniciar o sistema de captura
    public TMP_Text instructionsText; // Texto de instru��es para o jogador
    public float cameraSpeed = 10f; // Velocidade de movimento da c�mera
    public float zoomSpeed = 5f; // Velocidade de zoom

    private bool isScreenshotMode = false; // Se o modo de screenshot foi ativado
    private Dictionary<GameObject, bool> hiddenObjects = new Dictionary<GameObject, bool>();
    private string defaultScreenshotDirectory;

    void Start()
    {
        // Configura o diret�rio padr�o para salvar as screenshots
        defaultScreenshotDirectory = Path.Combine(Application.dataPath, "Screenshots");

        // Cria o diret�rio padr�o caso ele n�o exista
        if (!Directory.Exists(defaultScreenshotDirectory))
        {
            Directory.CreateDirectory(defaultScreenshotDirectory);
            Debug.Log("Pasta padr�o criada: " + defaultScreenshotDirectory);
        }

        // Garante que o texto de instru��es est� desativado no in�cio
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
        }

        // Busca autom�tica do bot�o se n�o for atribu�do no inspetor
        if (screenshotButton == null && levelEndPanel != null)
        {
            screenshotButton = levelEndPanel.GetComponentInChildren<Button>();
        }

        if (screenshotButton != null)
        {
            screenshotButton.onClick.AddListener(EnterScreenshotMode); // Associa o bot�o para ativar o modo de screenshot
        }
        else
        {
            Debug.LogError("Bot�o de Screenshot n�o encontrado! Verifique o painel LevelEndPanel.");
        }

        // Garante que a c�mera secund�ria est� desativada no in�cio
        if (screenshotCamera != null && eraserCursor != null)
        {
            screenshotCamera.gameObject.SetActive(false);
            eraserCursor.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("C�mera secund�ria para o modo Screenshot e Borracha n�o atribu�da!");
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

            // Ativa a c�mera secund�ria para o modo screenshot
            if (screenshotCamera != null && eraserCursor != null)
            {
                screenshotCamera.gameObject.SetActive(true);
                eraserCursor.gameObject.SetActive(true);
            }

            // Exibe o texto de instru��es
            if (instructionsText != null)
            {
                instructionsText.gameObject.SetActive(true);
                instructionsText.text = "Pressione 'P' para capturar uma screenshot.\nPressione 'O' para sair do modo screenshot.\nPressione 'E' para ativar a Borracha";
            }

            Debug.Log("Modo Screenshot e Borracha ativado. Use as teclas de movimenta��o ou o mouse para ajustar a posi��o.");
        }
    }

    private void ExitScreenshotMode()
    {
        isScreenshotMode = false;
        ShowObjectsWithTag(); // Restaura os objetos escondidos

        // Desativa a c�mera secund�ria/Borracha e retorna ao painel
        if (screenshotCamera != null && eraserCursor != null)
        {
            screenshotCamera.gameObject.SetActive(false);
            eraserCursor.gameObject.SetActive(false);
        }

        if (levelEndPanel != null)
        {
            levelEndPanel.SetActive(true); // Mostra o painel novamente
        }

        // Esconde o texto de instru��es
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
        }

        Debug.Log("Modo Screenshot desativado. Retornando ao painel.");
    }

    private void HandleCameraMovement()
    {
        // Movimenta��o pelo teclado
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horizontal, 0, vertical) * cameraSpeed * Time.deltaTime;

        if (screenshotCamera != null)
        {
            screenshotCamera.transform.position += move;
        }

        // Movimenta��o pelo mouse (arrastar)
        if (Input.GetMouseButton(0)) // Bot�o esquerdo do mouse para arrastar
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

        // Usa o diret�rio padr�o para salvar screenshots
        string directory = defaultScreenshotDirectory;

        // Define o nome do arquivo
        string filePath = Path.Combine(directory, $"screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");

        // Captura a screenshot
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot salva em: " + filePath);

        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(true); // Restaura o texto ap�s a captura
        }
    }
}
