using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MenuNavigation : MonoBehaviour
{
    [Header("Panels")]
    public GameObject selectSkinCharacterPanel; // 🔹 Painel de seleção de Skin
    public GameObject selectCharacterPanel;     // Painel de seleção de personagem (Pets)

    [Header("First Selected")]
    public GameObject firstSkinCharacterButton;
    public GameObject firstCharacterButton;

    [Header("UI Buttons")]
    public Button selectButton; // botão "SELECT" da imagem

    private bool isCharacterOpen = false;

    private void Start()
    {
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

        // apenas muda o foco
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SetFirstSelected(firstCharacterButton));
    }

    public void OpenSelectSkinCharacter()
    {
        isCharacterOpen = false;

        // apenas muda o foco
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SetFirstSelected(firstSkinCharacterButton));
    }

    private IEnumerator SetFirstSelected(GameObject button)
    {
        yield return null; // espera 1 frame
        EventSystem.current.SetSelectedGameObject(button);
    }
}
