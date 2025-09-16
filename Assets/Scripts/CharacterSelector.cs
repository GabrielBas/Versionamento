using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    public Sprite[] characterSprites; // Lista de sprites dos personagens
    public GameObject[] characterPrefabs; // Lista de prefabs (opcional, caso queira instanciar)
    public int[] characterCosts; // Custo de cada personagem
    public string[] characterDescriptions; // Descrição de cada personagem

    public Image previewImage; // UI para exibir a pré-visualização
    public Button selectButton; // Botão para selecionar
    public Button unlockButton; // Botão para desbloquear
    public Text priceText; // Texto para preço ou "Desbloqueado"
    public Text descriptionText; // Texto de descrição

    public Color selectedColor = Color.green;
    private Color defaultColor;

    private int selectedCharacterIndex = 0;
    private int currentSelectedCharacter = -1; // Personagem atualmente selecionado
    private const string SelectedCharacterKey = "SelectedCharacter";

    void Start()
    {
        defaultColor = selectButton.image.color;
        UpdateCharacterPreview();
    }

    public void NextCharacter()
    {
        selectedCharacterIndex = (selectedCharacterIndex + 1) % characterSprites.Length;
        UpdateCharacterPreview();
    }

    public void PreviousCharacter()
    {
        selectedCharacterIndex = (selectedCharacterIndex - 1 + characterSprites.Length) % characterSprites.Length;
        UpdateCharacterPreview();
    }

    private void UpdateCharacterPreview()
    {
        previewImage.sprite = characterSprites[selectedCharacterIndex];
        descriptionText.text = characterDescriptions[selectedCharacterIndex];

        if (IsCharacterUnlocked(selectedCharacterIndex))
        {
            selectButton.interactable = true;
            unlockButton.gameObject.SetActive(false);
            priceText.text = "Unlocked";
        }
        else
        {
            selectButton.interactable = false;
            unlockButton.gameObject.SetActive(true);
            priceText.text = characterCosts[selectedCharacterIndex] + " Coins";
        }

        UpdateButtonColor();
    }

    public void UnlockCharacter()
    {
        if (CoinController.instance == null)
        {
            Debug.LogError("CoinController.instance está nulo! Verifique se CoinController está na cena.");
            return;
        }

        int cost = characterCosts[selectedCharacterIndex];

        if (CoinController.instance.currentCoins >= cost && !IsCharacterUnlocked(selectedCharacterIndex))
        {
            CoinController.instance.SpendCoins(cost);
            PlayerPrefs.SetInt("CharacterUnlocked_" + selectedCharacterIndex, 1);
            PlayerPrefs.Save();

            UpdateCharacterPreview();

            // 🔹 Força foco no botão SELECT depois de desbloquear
            EventSystem.current.SetSelectedGameObject(selectButton.gameObject);

        }
        else
        {
            Debug.Log("Coins insuficientes ou personagem já desbloqueado.");
        }
    }

    private bool IsCharacterUnlocked(int index)
    {
        return PlayerPrefs.GetInt("CharacterUnlocked_" + index, 0) == 1;
    }

    public void SelectCharacter()
    {
        PlayerPrefs.SetInt(SelectedCharacterKey, selectedCharacterIndex);
        PlayerPrefs.Save();
        currentSelectedCharacter = selectedCharacterIndex;
        Debug.Log("Personagem " + selectedCharacterIndex + " selecionado!");

        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        if (selectedCharacterIndex == currentSelectedCharacter)
        {
            selectButton.image.color = selectedColor;
        }
        else
        {
            selectButton.image.color = defaultColor;
        }
    }
}
