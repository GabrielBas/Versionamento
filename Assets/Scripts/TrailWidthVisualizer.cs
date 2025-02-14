using UnityEngine;
using UnityEngine.UI;

public class TrailWidthVisualizer : MonoBehaviour
{
    [SerializeField] private Slider trailWidthSlider; // Referência ao Slider
    [SerializeField] private RectTransform visualFeedback; // Referência ao RectTransform da imagem de feedback
    [SerializeField] private float maxWidth = 200f; // Largura máxima da imagem
    [SerializeField] private float minWidth = 10f; // Largura mínima da imagem

    private void Start()
    {
        if (trailWidthSlider != null)
        {
            // Atualiza o retângulo ao inicializar
            UpdateVisualFeedback(trailWidthSlider.value);

            // Inscreve no evento OnValueChanged do Slider
            trailWidthSlider.onValueChanged.AddListener(UpdateVisualFeedback);
        }
    }

    private void UpdateVisualFeedback(float value)
    {
        if (visualFeedback != null)
        {
            // Mapeia o valor do Slider para a largura do retângulo
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
