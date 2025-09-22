﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelGallery;
    [SerializeField] private GameObject panelControls;

    [Header("First Buttons")]
    [SerializeField] private GameObject firstMainMenuButton;
    [SerializeField] private GameObject firstOptionsButton;
    [SerializeField] private GameObject firstGalleryButton;
    [SerializeField] private GameObject firstControlsButton;

    private PlayerInput playerInput;

    private void Awake()
    {
        // Garante que há um PlayerInput na cena
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
            playerInput = gameObject.AddComponent<PlayerInput>();

        playerInput.SwitchCurrentActionMap("UI");

        // 🔹 Só ativa o foco inicial se houver um controle conectado
        if (Gamepad.current != null)
        {
            SetSelectedButton(firstMainMenuButton);
        }
    }

    private void Update()
    {
        bool usingGamepad = Gamepad.current != null;

        // Detecta se está usando gamepad (qualquer botão de navegação)
        if (usingGamepad &&
            (Gamepad.current.dpad.up.wasPressedThisFrame ||
             Gamepad.current.dpad.down.wasPressedThisFrame ||
             Gamepad.current.leftStick.up.wasPressedThisFrame ||
             Gamepad.current.leftStick.down.wasPressedThisFrame ||
             Gamepad.current.buttonNorth.wasPressedThisFrame ||
             Gamepad.current.buttonSouth.wasPressedThisFrame))
        {
            // Se não houver botão selecionado, restaura o último válido
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (panelMainMenu.activeSelf) SetSelectedButton(firstMainMenuButton);
                else if (panelOptions.activeSelf) SetSelectedButton(firstOptionsButton);
                else if (panelGallery.activeSelf) SetSelectedButton(firstGalleryButton);
                else if (panelControls.activeSelf) SetSelectedButton(firstControlsButton);
            }
        }

        // --- Fechar painéis com ESC ou Botão B ---
        if ((Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
            (usingGamepad && Gamepad.current.buttonEast.wasPressedThisFrame))
        {
            if (panelOptions.activeSelf) CloseOptions();
            else if (panelGallery.activeSelf) CloseGallery();
            else if (panelControls.activeSelf) CloseControls();
        }

        // 🔹 Se desconectar o controle, limpa o foco
        if (!usingGamepad && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void SetSelectedButton(GameObject uiElement)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(uiElement);
    }

    public void Options()
    {
        panelMainMenu.SetActive(false);
        panelOptions.SetActive(true);

        if (Gamepad.current != null) // 🔹 só define foco se controle conectado
            SetSelectedButton(firstOptionsButton);
    }

    public void CloseOptions()
    {
        panelOptions.SetActive(false);
        panelMainMenu.SetActive(true);

        if (Gamepad.current != null)
            SetSelectedButton(firstMainMenuButton);
    }

    public void Gallery()
    {
        panelMainMenu.SetActive(false);
        panelGallery.SetActive(true);

        if (Gamepad.current != null)
            SetSelectedButton(firstGalleryButton);
    }

    public void CloseGallery()
    {
        panelGallery.SetActive(false);
        panelMainMenu.SetActive(true);

        if (Gamepad.current != null)
            SetSelectedButton(firstMainMenuButton);
    }

    public void Controls()
    {
        panelMainMenu.SetActive(false);
        panelControls.SetActive(true);

        if (Gamepad.current != null)
            SetSelectedButton(firstControlsButton);
    }

    public void CloseControls()
    {
        panelControls.SetActive(false);
        panelMainMenu.SetActive(true);

        if (Gamepad.current != null)
            SetSelectedButton(firstMainMenuButton);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("I'm Quitting");
    }
}
