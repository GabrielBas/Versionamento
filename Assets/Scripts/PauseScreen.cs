using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{

    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject panelControls;

    public void Controls()
    {
        panelPause.SetActive(false);
        panelControls.SetActive(true);
    }

    public void CloseControls()
    {
        panelControls.SetActive(false);
        panelPause.SetActive(true);
    }
}
