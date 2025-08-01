using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelGallery;
    [SerializeField] private GameObject panelControls;

    private PlayerInput playerInput;

    public GameObject firstOptionButton, firstGalleryButton;

    private void Awake()
    {
        // Busca o PlayerInput existente na cena
        playerInput = FindObjectOfType<PlayerInput>();

        // Garante que o mapa de input inicial � o de UI
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("UI");
        }
    }

    public void Options()
    {
        panelMainMenu.SetActive(false);
        panelOptions.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOptionButton);
    }

    public void CloseOptions()
    {
        panelOptions.SetActive(false);
        panelMainMenu.SetActive(true);
        
    }

    public void Gallery()
    {
        panelMainMenu.SetActive(false);
        panelGallery.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstGalleryButton);
    }

    public void CloseGallery()
    {
        panelGallery.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    public void Controls()
    {
        panelMainMenu.SetActive(false);
        panelControls.SetActive(true);
    }

    public void CloseControls()
    {
        panelControls.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("I'm Quitting");
    }
}
