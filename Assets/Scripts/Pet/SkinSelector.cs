using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkinSelector : MonoBehaviour
{
    public Sprite[] skinSprites; // Lista de sprites das skins
    public GameObject[] skinPrefabs; // Lista de prefabs das skins
    public int[] skinCosts; // Custo de cada skin
    public string[] skinDescriptions; // Descrição de cada skin

    public Image previewImage; // UI para exibir a pré-visualização
    public Button selectButton; // Botão para selecionar a skin
    public Button unlockButton; // Botão para desbloquear skin
    public Text priceText; // Texto para mostrar preço ou "Desbloqueado"
    public Text descriptionText; // Texto para exibir a descrição do pet

    public Color selectedColor = Color.green; // Cor do botão quando selecionado
    private Color defaultColor; // Cor padrão do botão

    private int selectedSkinIndex = 0;
    private int currentSelectedSkin = -1; // Armazena qual skin está atualmente selecionada
    private const string SelectedSkinKey = "SelectedSkin";

    void Start()
    {
        defaultColor = selectButton.image.color; // Salva a cor original do botão
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
        descriptionText.text = skinDescriptions[selectedSkinIndex]; // Atualiza a descrição do pet

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

        // Atualiza a cor do botão se for a skin selecionada
        UpdateButtonColor();
    }

    public void UnlockSkin()
    {
        if (CoinController.instance == null)
        {
            Debug.LogError("CoinController.instance está nulo! Verifique se CoinController está presente na cena.");
            return;
        }

        int cost = skinCosts[selectedSkinIndex];

        if (CoinController.instance.currentCoins >= cost && !IsSkinUnlocked(selectedSkinIndex))
        {
            CoinController.instance.SpendCoins(cost);
            PlayerPrefs.SetInt("SkinUnlocked_" + selectedSkinIndex, 1);
            PlayerPrefs.Save();

            UpdateSkinPreview();

            // 🔹 Força foco no botão SELECT depois de desbloquear
            EventSystem.current.SetSelectedGameObject(selectButton.gameObject);

        }
        else
        {
            Debug.Log("Coins insuficientes ou skin já desbloqueada.");
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
        currentSelectedSkin = selectedSkinIndex; // Define qual skin está ativa
        Debug.Log("Skin " + selectedSkinIndex + " selecionada!");

        UpdateButtonColor(); // Atualiza a cor do botão
    }

    private void UpdateButtonColor()
    {
        if (selectedSkinIndex == currentSelectedSkin)
        {
            selectButton.image.color = selectedColor; // Altera a cor para selecionado
        }
        else
        {
            selectButton.image.color = defaultColor; // Retorna à cor padrão
        }
    }
}
