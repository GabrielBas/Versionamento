using UnityEngine;
using UnityEngine.UI;

public class TrailWidthSlider : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();

        // Define o valor inicial do slider com base na configuração salva
        slider.value = TrailWidthManager.Instance.TrailWidth;

        // Adiciona o listener para atualizar a largura do TrailRenderer
        slider.onValueChanged.AddListener(UpdateTrailWidth);
    }

    private void UpdateTrailWidth(float value)
    {
        TrailWidthManager.Instance.SetTrailWidth(value);
    }
}
