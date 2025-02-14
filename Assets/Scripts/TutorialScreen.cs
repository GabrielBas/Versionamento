using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScreen : MonoBehaviour
{
    public string gameSceneName = "GameScene"; // Nome da cena normal do jogo
    public string skipTutorialSceneName = "MainMenu"; // Cena que ser� carregada se o tutorial j� foi visto
    private const string TutorialKey = "HasSeenTutorial"; // Chave para verificar se o tutorial j� foi visto

    void Start()
    {
        // Se o jogador j� viu o tutorial, carrega a cena alternativa
        if (PlayerPrefs.GetInt(TutorialKey, 0) == 1)
        {
            SceneManager.LoadScene(skipTutorialSceneName);
        }
    }

    void Update()
    {
        // Se o jogador pressionar qualquer tecla, inicia o jogo e marca o tutorial como visto
        if (Input.anyKeyDown)
        {
            PlayerPrefs.SetInt(TutorialKey, 1); // Salva que o jogador j� viu o tutorial
            PlayerPrefs.Save();
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
