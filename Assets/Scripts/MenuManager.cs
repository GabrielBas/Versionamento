using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

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

    [Header("UI Buttons")]
    public Button selectButton; // botão "SELECT" da imagem

    private bool isMapOpen = false;

    private void Start()
    {
        // pega ou adiciona CanvasGroup automaticamente
        mapGroup = selectMapPanel.GetComponent<CanvasGroup>();
        if (mapGroup == null) mapGroup = selectMapPanel.AddComponent<CanvasGroup>();

        characterGroup = selectCharacterPanel.GetComponent<CanvasGroup>();
        if (characterGroup == null) characterGroup = selectCharacterPanel.AddComponent<CanvasGroup>();

        // liga evento do botão SELECT
        if (selectButton != null)
            selectButton.onClick.AddListener(OpenSelectMap);

        // começa no Character
        OpenSelectCharacter();
    }

    private void Update()
    {
        // ESC no teclado ou Botão B do controle
        if (isMapOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button1)))
        {
            OpenSelectCharacter();
        }
    }

    public void OpenSelectMap()
    {
        isMapOpen = true;
        SetPanelState(mapGroup, true);
        SetPanelState(characterGroup, false);

        // 🔹 Garante que o botão PLAY receba o foco
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SetFirstSelected(firstMapButton));
    }


    public void OpenSelectCharacter()
    {
        isMapOpen = false;
        SetPanelState(mapGroup, false);
        SetPanelState(characterGroup, true);

        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SetFirstSelected(firstCharacterButton));
    }

    private void SetPanelState(CanvasGroup group, bool active)
    {
        group.alpha = active ? 1f : 0f;   // 0 = invisível, 1 = visível
        group.interactable = active;
        group.blocksRaycasts = active;
    }

    private IEnumerator SetFirstSelected(GameObject button)
    {
        yield return null; // espera 1 frame
        EventSystem.current.SetSelectedGameObject(button);
    }
}
