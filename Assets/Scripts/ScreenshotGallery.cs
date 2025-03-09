using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotGallery : MonoBehaviour
{
    public Transform galleryContainer; // O Content do ScrollView
    public GameObject imagePrefab; // O botão de miniatura
    public Image fullScreenImage; // Para exibir em tela cheia
    public GameObject fullScreenPanel; // Painel de visualização
    public GameObject scrollViewPanel; // O ScrollView que será desativado ao abrir fullscreen

    private string screenshotPath; // Caminho onde as screenshots estão salvas

    void Start()
    {
        screenshotPath = Path.Combine(Application.dataPath, "Screenshots/");

        if (!Directory.Exists(screenshotPath))
        {
            Debug.LogError("Pasta de screenshots não encontrada: " + screenshotPath);
            return;
        }

        LoadGallery();
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
        {
            CreateThumbnail(file);
        }
    }

    void CreateThumbnail(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Arquivo não encontrado: " + filePath);
            return;
        }

        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        bool loaded = texture.LoadImage(imageBytes);

        if (!loaded)
        {
            Debug.LogError("Falha ao carregar imagem: " + filePath);
            return;
        }

        GameObject thumbnail = Instantiate(imagePrefab, galleryContainer);
        Image imageComponent = thumbnail.GetComponent<Image>();

        if (imageComponent == null)
        {
            Debug.LogError("Prefab de miniatura não tem um componente Image!");
            return;
        }

        imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        Button button = thumbnail.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => ShowFullScreen(texture));
        }
        else
        {
            Debug.LogError("Prefab de miniatura não tem um componente Button!");
        }
    }

    public void ShowFullScreen(Texture2D texture)
    {
        if (fullScreenImage == null || fullScreenPanel == null)
        {
            Debug.LogError("UI de tela cheia não está configurada corretamente!");
            return;
        }

        fullScreenImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        fullScreenPanel.SetActive(true);

        // 🔴 Desativar o ScrollView ao abrir fullscreen
        if (scrollViewPanel != null)
        {
            scrollViewPanel.SetActive(false);
        }
    }

    public void CloseFullScreen()
    {
        if (fullScreenPanel != null)
        {
            fullScreenPanel.SetActive(false);
        }

        // ✅ Ativar o ScrollView ao fechar fullscreen
        if (scrollViewPanel != null)
        {
            scrollViewPanel.SetActive(true);
        }
    }
}
