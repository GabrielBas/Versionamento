using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DesaturationEffect : MonoBehaviour
{
    public float desaturationAmount = -100f;  // Nível de saturação (-100 é preto e branco)
    public float effectDuration = 5f;         // Duração do efeito em segundos

    public PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;

    void OnEnable()
    {
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<PostProcessVolume>();
        }

        if (postProcessVolume != null && postProcessVolume.profile.TryGetSettings(out colorGrading))
        {
            colorGrading.saturation.value = desaturationAmount;
            CancelInvoke("ResetSaturation");
            Invoke("ResetSaturation", effectDuration);
        }
        else
        {
            Debug.LogError("Color Grading não encontrado no Post Process Volume!");
        }
    }

    void ResetSaturation()
    {
        if (colorGrading != null)
        {
            colorGrading.saturation.value = 0f;
        }
    }
}
