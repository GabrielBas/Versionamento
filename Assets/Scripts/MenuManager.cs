using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigation : MonoBehaviour
{
    [Header("Panels")]
    public GameObject selectMapPanel;
    public GameObject selectCharacterPanel;

    private CanvasGroup mapGroup;
    private CanvasGroup characterGroup;

    [Header("First Selected")]
    public GameObject firstMapButton;
    public GameObject firstCharacterButton;

    private void Start()
    {
        // pega ou adiciona CanvasGroup automaticamente
        mapGroup = selectMapPanel.GetComponent<CanvasGroup>();
        if (mapGroup == null) mapGroup = selectMapPanel.AddComponent<CanvasGroup>();

        characterGroup = selectCharacterPanel.GetComponent<CanvasGroup>();
        if (characterGroup == null) characterGroup = selectCharacterPanel.AddComponent<CanvasGroup>();

        OpenSelectMap();
    }

    public void OpenSelectMap()
    {
        SetPanelState(mapGroup, true);
        SetPanelState(characterGroup, false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstMapButton);
    }

    public void OpenSelectCharacter()
    {
        SetPanelState(mapGroup, false);
        SetPanelState(characterGroup, true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstCharacterButton);
    }

    private void SetPanelState(CanvasGroup group, bool active)
    {
        group.alpha = active ? 1f : 0.5f;   // pode usar 0f se quiser invisível
        group.interactable = active;
        group.blocksRaycasts = active;
    }
}
