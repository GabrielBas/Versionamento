using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DesaturationEffect : MonoBehaviour
{
    public float desaturationAmount = -100f;  // N�vel de satura��o (-100 � preto e branco)
    public float effectDuration = 5f;         // Dura��o do efeito em segundos

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
            Debug.LogError("Color Grading n�o encontrado no Post Process Volume!");
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
