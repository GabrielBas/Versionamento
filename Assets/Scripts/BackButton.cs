using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    [Header("Controlador de transição")]
    public SceneTransition sceneTransition;

    [Header("Nome fixo da cena para voltar (opcional)")]
    public string fixedSceneName;

    private string previousScene;

    void Start()
    {
        // Salva a cena atual como "anterior" para a próxima
        previousScene = PlayerPrefs.GetString("CurrentScene", "");
        PlayerPrefs.SetString("CurrentScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    public void GoBack()
    {
        string targetScene;

        if (!string.IsNullOrEmpty(fixedSceneName))
        {
            // Se tiver um nome fixo, usa ele
            targetScene = fixedSceneName;
        }
        else if (!string.IsNullOrEmpty(previousScene))
        {
            // Se não tiver nome fixo, usa a cena anterior salva
            targetScene = previousScene;
        }
        else
        {
            Debug.LogWarning("Nenhuma cena definida ou anterior encontrada!");
            return;
        }

        sceneTransition.FadeToScene(targetScene);
    }
}
