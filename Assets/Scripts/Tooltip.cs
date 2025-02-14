using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public GameObject tooltipPanel; // O painel do tooltip
    public TMP_Text tooltipText; // O texto do tooltip
    public Vector3 offset; // Deslocamento para evitar sobreposição do cursor
    public GameObject levelUpPanel; // O painel de Level UP

    private void Start()
    {
        if (tooltipPanel == null || tooltipText == null)
        {
            Debug.LogError("Tooltip: tooltipPanel ou tooltipText não foi configurado no Inspetor!");
            return;
        }
        HideTooltip();
    }

    private void Update()
    {
        // Ocultar o tooltip se o painel de Level UP estiver desativado
        if (levelUpPanel != null && !levelUpPanel.activeSelf)
        {
            HideTooltip();
        }

        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            // Seguir o cursor do mouse
            tooltipPanel.transform.position = Input.mousePosition + offset;
        }
    }

    public void ShowTooltip(string message)
    {
        if (tooltipPanel == null || tooltipText == null)
        {
            Debug.LogError("Tooltip: tooltipPanel ou tooltipText não foi configurado no Inspetor!");
            return;
        }

        tooltipText.text = message;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}
