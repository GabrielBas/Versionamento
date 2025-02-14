using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMapManager : MonoBehaviour
{
    //[SerializeField] private GameObject panelSelectMap1;
    

    public void SelectMap1()
    {
        SceneManager.LoadScene(2);
        //panelSelectMap1.SetActive(false);
    }


    //public void QuitGame()
    //{
    //    Application.Quit();

    //    Debug.Log("I'm Quitting");
    //}
}
