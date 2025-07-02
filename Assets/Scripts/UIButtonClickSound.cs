using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonClickSound : MonoBehaviour
{
    public AudioClip clickSound; // Arraste o som do clique no inspetor
    [Range(0f, 1f)]
    public float volume = 1f; // Volume do som

    private AudioSource audioSource;

    void Awake()
    {
        // Garante que o botão tem um AudioSource separado
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Adiciona o evento OnClick do botão
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, volume);
        }
        else
        {
            Debug.LogWarning("Nenhum som de clique atribuído no UIButtonClickSound!");
        }
    }
}
