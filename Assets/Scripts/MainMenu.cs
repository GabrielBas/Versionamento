using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelGallery;


    public void StartGame()
    {
        SceneManager.LoadScene(1);
        panelMainMenu.SetActive(false);
    }

    public void Options()
    {
        panelMainMenu.SetActive(false);
        panelOptions.SetActive(true);
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
    }

    public void CloseGallery()
    {
        panelGallery.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("I'm Quitting");
    }


}
