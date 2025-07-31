using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SceneTransition : MonoBehaviour
{
    public Image fadePanel;
    public float fadeSpeed = 1f;

    private string sceneToLoad;

    private static List<SceneTransition> instances = new List<SceneTransition>();
    private static int maxInstances = 5;

    public GameObject firstOptionButton;

    void Awake()
    {
        instances.Add(this);

        if (instances.Count > maxInstances)
        {
            // Índice do penúltimo instanciado = count - 2
            int penultimateIndex = instances.Count - 2;

            if (penultimateIndex >= 0 && penultimateIndex < instances.Count)
            {
                SceneTransition toDestroy = instances[penultimateIndex];
                instances.RemoveAt(penultimateIndex);
                if (toDestroy != this)
                {
                    Destroy(toDestroy.gameObject);
                }
                else
                {
                    // Se por acaso essa instância for o penúltimo, destrói o anterior
                    int previousIndex = penultimateIndex - 1;
                    if (previousIndex >= 0 && previousIndex < instances.Count)
                    {
                        Destroy(instances[previousIndex].gameObject);
                        instances.RemoveAt(previousIndex);
                    }
                }
            }
            else
            {
                // Caso algo estranho, destrói o primeiro
                SceneTransition oldest = instances[0];
                instances.RemoveAt(0);
                if (oldest != this)
                    Destroy(oldest.gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        instances.Remove(this);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
            EventSystem.current.SetSelectedGameObject(firstOptionButton);
        }
        else
        {
            Debug.LogError("fadePanel não está atribuído no SceneTransition");
        }
    }

    public void FadeToScene(string sceneName)
    {
        sceneToLoad = sceneName;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        while (fadePanel.color.a > 0)
        {
            fadePanel.color = new Color(0, 0, 0, fadePanel.color.a - Time.deltaTime * fadeSpeed);
            yield return null;
        }
        fadePanel.gameObject.SetActive(false);
    }

    IEnumerator FadeOut()
    {
        fadePanel.gameObject.SetActive(true);
        while (fadePanel.color.a < 1)
        {
            fadePanel.color = new Color(0, 0, 0, fadePanel.color.a + Time.deltaTime * fadeSpeed);
            yield return null;
        }
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }
}
