using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MenuNavigation : MonoBehaviour
{
    [Header("Panels")]
    public GameObject selectSkinCharacterPanel; // 🔹 Painel de seleção de Skin
    public GameObject selectCharacterPanel;     // Painel de seleção de personagem (Pets)

    private CanvasGroup skinCharacterGroup;
    private CanvasGroup characterGroup;

    [Header("First Selected")]
    public GameObject firstSkinCharacterButton;
    public GameObject firstCharacterButton;

    [Header("UI Buttons")]
    public Button selectButton; // botão "SELECT" da imagem

    private bool isCharacterOpen = false;

    private void Start()
    {
        // pega ou adiciona CanvasGroup automaticamente
        skinCharacterGroup = selectSkinCharacterPanel.GetComponent<CanvasGroup>();
        if (skinCharacterGroup == null)
            skinCharacterGroup = selectSkinCharacterPanel.AddComponent<CanvasGroup>();

        characterGroup = selectCharacterPanel.GetComponent<CanvasGroup>();
        if (characterGroup == null)
            characterGroup = selectCharacterPanel.AddComponent<CanvasGroup>();

        // liga evento do botão SELECT → abre painel de Character (Pets)
        if (selectButton != null)
            selectButton.onClick.AddListener(OpenSelectCharacter);

        // 🔹 começa no painel de SkinCharacter
        OpenSelectSkinCharacter();
    }

    private void Update()
    {
        // ESC no teclado ou Botão B do controle → volta para SkinCharacter
        if (isCharacterOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button1)))
        {
            OpenSelectSkinCharacter();
        }
    }

    public void OpenSelectCharacter()
    {
        isCharacterOpen = true;
        SetPanelState(characterGroup, true);
        SetPanelState(skinCharacterGroup, false);

        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SetFirstSelected(firstCharacterButton));
    }

    public void OpenSelectSkinCharacter()
    {
        isCharacterOpen = false;
        SetPanelState(characterGroup, false);
        SetPanelState(skinCharacterGroup, true);

        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SetFirstSelected(firstSkinCharacterButton));
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
