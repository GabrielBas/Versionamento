using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ScreenshotGallery : MonoBehaviour
{
    [Header("UI")]
    public Transform galleryContainer;
    public GameObject imagePrefab;
    public Image fullScreenImage;
    public GameObject fullScreenPanel;
    public GameObject scrollViewPanel;
    public Button setAsBackgroundButton;
    public GameObject backButton;
    public ScrollRect scrollRect;

    [Header("Navegação")]
    public GalleryScrollHybrid galleryNav; // arraste no inspetor

    private string screenshotPath;
    private string selectedImagePath;
    private PlayerInput playerInput;
    private readonly List<GameObject> thumbnails = new List<GameObject>();

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        screenshotPath = Path.Combine(Application.dataPath, "Screenshots/");

        if (galleryNav == null) galleryNav = FindObjectOfType<GalleryScrollHybrid>();
        if (galleryNav != null) galleryNav.skipInitialSelection = true;

        if (!Directory.Exists(screenshotPath))
        {
            Debug.LogError("Pasta de screenshots não encontrada: " + screenshotPath);
            return;
        }

        LoadGallery();

        if (thumbnails.Count > 0)
            StartCoroutine(SelectFirstThumbnailNextFrame());
    }

    private IEnumerator SelectFirstThumbnailNextFrame()
    {
        // espera 2 frames para garantir que o Layout/ContentSizeFitter atualizou
        yield return null;
        yield return null;

        if (galleryNav != null)
            galleryNav.SelectIndexPublic(0, centerInView: true);
        else
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

    private void CreateThumbnail(string filePath)
    {
        if (!File.Exists(filePath)) return;

        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (!texture.LoadImage(imageBytes)) return;

        GameObject thumbnail = Instantiate(imagePrefab, galleryContainer);
        var imageComponent = thumbnail.GetComponent<Image>();
        if (imageComponent != null)
            imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        var button = thumbnail.GetComponent<Button>();
        if (button != null)
        {
            // Abre a imagem correta
            button.onClick.AddListener(() => ShowFullScreen(filePath, texture));

            // Desliga navegação automática — nosso script controla o D-Pad
            var nav = button.navigation;
            nav.mode = Navigation.Mode.None;
            button.navigation = nav;

            // Garante que comece sem outline ligado
            var outline = thumbnail.GetComponent<Outline>();
            if (outline != null) outline.enabled = false;
        }

        thumbnails.Add(thumbnail);
    }

    public void ShowFullScreen(string filePath, Texture2D texture)
    {
        if (scrollViewPanel != null) scrollViewPanel.SetActive(false);

        fullScreenImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        fullScreenPanel.SetActive(true);
        selectedImagePath = filePath;

        if (backButton != null) backButton.SetActive(false);

        if (setAsBackgroundButton != null)
        {
            setAsBackgroundButton.gameObject.SetActive(true);
            setAsBackgroundButton.onClick.RemoveAllListeners();
            setAsBackgroundButton.onClick.AddListener(SetAsBackground);
            EventSystem.current.SetSelectedGameObject(setAsBackgroundButton.gameObject);
        }
    }

    public void CloseFullScreen()
    {
        fullScreenPanel.SetActive(false);
        if (scrollViewPanel != null) scrollViewPanel.SetActive(true);

        if (backButton != null) backButton.SetActive(true);
        if (setAsBackgroundButton != null) setAsBackgroundButton.gameObject.SetActive(false);

        if (galleryNav != null && galleryNav.CurrentIndex >= 0)
            galleryNav.SelectIndexPublic(galleryNav.CurrentIndex, centerInView: true);
        else if (thumbnails.Count > 0)
            EventSystem.current.SetSelectedGameObject(thumbnails[0]);
    }

    public void SetAsBackground()
    {
        if (string.IsNullOrEmpty(selectedImagePath)) return;

        PlayerPrefs.SetString("MenuBackgroundPath", selectedImagePath);
        PlayerPrefs.Save();
        BackgroundManager.ChangeBackground();
    }

    private void Update()
    {
        // Botão voltar (B do Gamepad)
        if (playerInput != null && playerInput.currentControlScheme == "Gamepad")
        {
            if (Gamepad.current != null && Gamepad.current.bButton.wasPressedThisFrame)
            {
                if (fullScreenPanel.activeSelf) CloseFullScreen();
                else backButton?.GetComponent<Button>()?.onClick.Invoke();
            }
        }
    }
}
