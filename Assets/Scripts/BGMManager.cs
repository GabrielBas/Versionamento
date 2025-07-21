using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    private AudioSource audioSource;

    [Header("Cenas em que a BGM deve parar")]
    [SerializeField] private string[] stopBgmScenes;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentScene = scene.name;

        bool shouldStop = false;

        foreach (string stopScene in stopBgmScenes)
        {
            if (stopScene == currentScene)
            {
                shouldStop = true;
                break;
            }
        }

        if (shouldStop)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                Debug.Log($"BGM pausada na cena: {currentScene}");
            }
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                Debug.Log($"BGM retomada na cena: {currentScene}");
            }
        }
    }
}
