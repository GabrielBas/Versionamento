using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    private const string SelectedCharacterKey = "SelectedCharacter"; // Chave para salvar o índice do personagem

    void Start()
    {
        // Carrega o índice do personagem selecionado
        int selectedCharacterIndex = PlayerPrefs.GetInt(SelectedCharacterKey, 0);

        // Desativa todos os personagens filhos
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // Ativa o personagem correspondente
        if (selectedCharacterIndex >= 0 && selectedCharacterIndex < transform.childCount)
        {
            Transform selectedCharacter = transform.GetChild(selectedCharacterIndex);
            selectedCharacter.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Índice de personagem inválido ou não configurado corretamente.");
        }
    }
}
