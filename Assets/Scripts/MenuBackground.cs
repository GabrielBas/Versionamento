using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuBackground : MonoBehaviour
{
    public Image backgroundImage;

    void Start()
    {
        LoadBackground();
    }

    void OnEnable()
    {
        BackgroundManager.OnBackgroundChanged += LoadBackground;
        LoadBackground();
    }

    void OnDisable()
    {
        BackgroundManager.OnBackgroundChanged -= LoadBackground;
    }

    void LoadBackground()
    {
        string path = PlayerPrefs.GetString("MenuBackgroundPath", "");

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            backgroundImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            Debug.Log("Background atualizado!");
        }
        else
        {
            Debug.LogWarning("Nenhuma imagem de background encontrada!");
        }
    }
}
