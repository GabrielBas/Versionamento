using UnityEngine;
using UnityEngine.EventSystems;

public class ItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string itemDescription; // Descrição do item
    private Tooltip tooltip;

    private void Start()
    {
        tooltip = FindObjectOfType<Tooltip>();

        if (tooltip == null)
        {
            Debug.LogError("ItemTooltip: Nenhum Tooltip foi encontrado na cena!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.ShowTooltip(itemDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }
}
