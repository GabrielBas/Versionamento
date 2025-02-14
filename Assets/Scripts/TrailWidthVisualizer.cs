using UnityEngine;
using UnityEngine.UI;

public class TrailWidthVisualizer : MonoBehaviour
{
    [SerializeField] private Slider trailWidthSlider; // Refer�ncia ao Slider
    [SerializeField] private RectTransform visualFeedback; // Refer�ncia ao RectTransform da imagem de feedback
    [SerializeField] private float maxWidth = 200f; // Largura m�xima da imagem
    [SerializeField] private float minWidth = 10f; // Largura m�nima da imagem

    private void Start()
    {
        if (trailWidthSlider != null)
        {
            // Atualiza o ret�ngulo ao inicializar
            UpdateVisualFeedback(trailWidthSlider.value);

            // Inscreve no evento OnValueChanged do Slider
            trailWidthSlider.onValueChanged.AddListener(UpdateVisualFeedback);
        }
    }

    private void UpdateVisualFeedback(float value)
    {
        if (visualFeedback != null)
        {
            // Mapeia o valor do Slider para a largura do ret�ngulo
            float newWidth = Mathf.Lerp(minWidth, maxWidth, value);
            visualFeedback.sizeDelta = new Vector2(newWidth, visualFeedback.sizeDelta.y);
        }
    }

    private void OnDestroy()
    {
        // Remove o listener para evitar erros
        if (trailWidthSlider != null)
        {
            trailWidthSlider.onValueChanged.RemoveListener(UpdateVisualFeedback);
        }
    }
}
