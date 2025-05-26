using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SkinSelector : MonoBehaviour
{
    public Sprite[] skinSprites; // Lista de sprites das skins
    public GameObject[] skinPrefabs; // Lista de prefabs das skins
    public int[] skinCosts; // Custo de cada skin
    public string[] skinDescriptions; // Descri��o de cada skin

    public Image previewImage; // UI para exibir a pr�-visualiza��o
    public Button selectButton; // Bot�o para selecionar a skin
    public Button unlockButton; // Bot�o para desbloquear skin
    public Text priceText; // Texto para mostrar pre�o ou "Desbloqueado"
    public Text descriptionText; // Texto para exibir a descri��o do pet

    public Color selectedColor = Color.green; // Cor do bot�o quando selecionado
    private Color defaultColor; // Cor padr�o do bot�o

    private int selectedSkinIndex = 0;
    private int currentSelectedSkin = -1; // Armazena qual skin est� atualmente selecionada
    private const string SelectedSkinKey = "SelectedSkin";

    void Start()
    {
        defaultColor = selectButton.image.color; // Salva a cor original do bot�o
        UpdateSkinPreview();
    }

    public void NextSkin()
    {
        selectedSkinIndex = (selectedSkinIndex + 1) % skinSprites.Length;
        UpdateSkinPreview();
    }

    public void PreviousSkin()
    {
        selectedSkinIndex = (selectedSkinIndex - 1 + skinSprites.Length) % skinSprites.Length;
        UpdateSkinPreview();
    }

    private void UpdateSkinPreview()
    {
        previewImage.sprite = skinSprites[selectedSkinIndex];
        descriptionText.text = skinDescriptions[selectedSkinIndex]; // Atualiza a descri��o do pet

        if (IsSkinUnlocked(selectedSkinIndex))
        {
            selectButton.interactable = true;
            unlockButton.gameObject.SetActive(false);
            priceText.text = "Unlocked";
        }
        else
        {
            selectButton.interactable = false;
            unlockButton.gameObject.SetActive(true);
            priceText.text = skinCosts[selectedSkinIndex] + " Coins";
        }

        // Atualiza a cor do bot�o se for a skin selecionada
        UpdateButtonColor();
    }

    public void UnlockSkin()
    {
        if (CoinController.instance == null)
        {
            Debug.LogError("CoinController.instance est� nulo! Verifique se CoinController est� presente na cena.");
            return;
        }

        int cost = skinCosts[selectedSkinIndex];

        if (CoinController.instance.currentCoins >= cost && !IsSkinUnlocked(selectedSkinIndex))
        {
            CoinController.instance.SpendCoins(cost);
            PlayerPrefs.SetInt("SkinUnlocked_" + selectedSkinIndex, 1);
            PlayerPrefs.Save();

            UpdateSkinPreview();
        }
        else
        {
            Debug.Log("Coins insuficientes ou skin j� desbloqueada.");
        }
    }

    private bool IsSkinUnlocked(int skinIndex)
    {
        return PlayerPrefs.GetInt("SkinUnlocked_" + skinIndex, 0) == 1;
    }

    public void SelectSkin()
    {
        PlayerPrefs.SetInt(SelectedSkinKey, selectedSkinIndex);
        PlayerPrefs.Save();
        currentSelectedSkin = selectedSkinIndex; // Define qual skin est� ativa
        Debug.Log("Skin " + selectedSkinIndex + " selecionada!");

        UpdateButtonColor(); // Atualiza a cor do bot�o
    }

    private void UpdateButtonColor()
    {
        if (selectedSkinIndex == currentSelectedSkin)
        {
            selectButton.image.color = selectedColor; // Altera a cor para selecionado
        }
        else
        {
            selectButton.image.color = defaultColor; // Retorna � cor padr�o
        }
    }
}
