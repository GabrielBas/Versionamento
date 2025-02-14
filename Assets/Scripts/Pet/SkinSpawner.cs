using UnityEngine;

public class SkinSpawner : MonoBehaviour
{
    private const string SelectedSkinKey = "SelectedSkin"; // Chave para salvar o índice da skin

    void Start()
    {
        // Carrega o índice da skin selecionada
        int selectedSkinIndex = PlayerPrefs.GetInt(SelectedSkinKey, 0);

        // Busca todos os prefabs na cena e desativa todos
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // Ativa a skin correspondente
        if (selectedSkinIndex >= 0 && selectedSkinIndex < transform.childCount)
        {
            Transform selectedSkin = transform.GetChild(selectedSkinIndex);
            selectedSkin.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Índice de skin inválido ou não configurado corretamente.");
        }
    }
}
