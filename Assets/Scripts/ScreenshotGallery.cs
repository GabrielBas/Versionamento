using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ScreenshotGallery : MonoBehaviour
{
    public Transform galleryContainer;
    public GameObject imagePrefab;
    public Image fullScreenImage;
    public GameObject fullScreenPanel;
    public GameObject scrollViewPanel;
    public Button setAsBackgroundButton;
    public GameObject backButton;
    public ScrollRect scrollRect;

    private string screenshotPath;
    private string selectedImagePath;
    private PlayerInput playerInput;
    private List<GameObject> thumbnails = new List<GameObject>();

    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        screenshotPath = Path.Combine(Application.dataPath, "Screenshots/");

        if (!Directory.Exists(screenshotPath))
        {
            Debug.LogError("Pasta de screenshots não encontrada: " + screenshotPath);
            return;
        }

        LoadGallery();

        // Seleciona a primeira imagem no final do frame
        if (thumbnails.Count > 0)
            StartCoroutine(SelectFirstThumbnailNextFrame());
    }

    IEnumerator SelectFirstThumbnailNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(thumbnails[0]);
    }

    public void LoadGallery()
    {
        string[] files = Directory.GetFiles(screenshotPath, "*.png");

        if (files.Length == 0)
        {
            Debug.LogWarning("Nenhuma screenshot encontrada na pasta: " + screenshotPath);
            return;
        }

        foreach (string file in files)
            CreateThumbnail(file);
    }

    void CreateThumbnail(string filePath)
    {
        if (!File.Exists(filePath)) return;

        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (!texture.LoadImage(imageBytes)) return;

        GameObject thumbnail = Instantiate(imagePrefab, galleryContainer);
        Image imageComponent = thumbnail.GetComponent<Image>();
        imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        Button button = thumbnail.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => ShowFullScreen(filePath, texture));

            // Garantir que seja navegável
            Navigation nav = button.navigation;
            nav.mode = Navigation.Mode.Automatic;
            button.navigation = nav;
        }

        thumbnails.Add(thumbnail);
    }

    public void ShowFullScreen(string filePath, Texture2D texture)
    {
        fullScreenImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        fullScreenPanel.SetActive(true);
        selectedImagePath = filePath;

        scrollViewPanel.SetActive(false);
        if (backButton != null) backButton.SetActive(false);

        if (setAsBackgroundButton != null)
        {
            setAsBackgroundButton.gameObject.SetActive(true);
            setAsBackgroundButton.onClick.RemoveAllListeners();
            setAsBackgroundButton.onClick.AddListener(SetAsBackground);
        }

        EventSystem.current.SetSelectedGameObject(setAsBackgroundButton.gameObject);
    }

    public void CloseFullScreen()
    {
        fullScreenPanel.SetActive(false);
        scrollViewPanel.SetActive(true);

        if (backButton != null) backButton.SetActive(true);
        if (setAsBackgroundButton != null) setAsBackgroundButton.gameObject.SetActive(false);

        if (thumbnails.Count > 0)
            EventSystem.current.SetSelectedGameObject(thumbnails[0]);
    }

    public void SetAsBackground()
    {
        if (string.IsNullOrEmpty(selectedImagePath)) return;

        PlayerPrefs.SetString("MenuBackgroundPath", selectedImagePath);
        PlayerPrefs.Save();
        BackgroundManager.ChangeBackground();
    }

    void Update()
    {
        // Botão voltar
        if (playerInput != null && playerInput.currentControlScheme == "Gamepad")
        {
            if (Gamepad.current != null && Gamepad.current.bButton.wasPressedThisFrame)
            {
                if (fullScreenPanel.activeSelf)
                    CloseFullScreen();
                else
                    backButton?.GetComponent<Button>()?.onClick.Invoke();
            }
        }

        // Scroll automático conforme seleção
        if (scrollRect != null && EventSystem.current.currentSelectedGameObject != null)
        {
            RectTransform selected = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
            if (selected != null && selected.transform.IsChildOf(galleryContainer))
            {
                float contentHeight = scrollRect.content.rect.height;
                float viewportHeight = scrollRect.viewport.rect.height;

                float normalizedY = Mathf.Clamp01(1 - ((selected.anchoredPosition.y + scrollRect.content.anchoredPosition.y) / (contentHeight - viewportHeight)));
                scrollRect.normalizedPosition = new Vector2(0, normalizedY);
            }
        }
    }
}
