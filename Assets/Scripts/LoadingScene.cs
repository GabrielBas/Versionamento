using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Image LoadingBarFill;

    [Header("Pain�is para ativar/desativar")]
    [SerializeField] private List<GameObject> panels;

    [Header("Tempo m�nimo de loading (segundos)")]
    [SerializeField] private float minLoadingTime = 5f;

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false;  // Segura a ativa��o da cena

        foreach (GameObject panel in panels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        LoadingScreen.SetActive(true);

        float elapsedTime = 0f;

        while (operation.progress < 0.9f)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarFill.fillAmount = progressValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // A cena est� carregada at� 90%, Unity segura os 10% finais at� dar allowSceneActivation
        // Agora espera o tempo m�nimo
        while (elapsedTime < minLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Libera a cena para ativar
        operation.allowSceneActivation = true;
    }
}

