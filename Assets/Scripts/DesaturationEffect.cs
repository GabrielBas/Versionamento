using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class DesaturationEffect : MonoBehaviour
{
    public Volume postProcessVolume;
    private ColorGrading colorGrading;
    private float originalSaturation;
    public float desaturationAmount = -100f; // Satura��o reduzida ao m�ximo

    void Start()
    {
        if (postProcessVolume == null)
        {
            Debug.LogError("Post-process volume n�o atribu�do no inspector.");
            return;
        }

        if (!postProcessVolume.profile.TryGet(out colorGrading))
        {
            Debug.LogError("Color Grading n�o encontrado no Post-process volume.");
            return;
        }

        originalSaturation = colorGrading.saturation.value;
        ActivateDesaturation();
    }

    void OnDestroy()
    {
        ResetSaturation();
    }

    public void ActivateDesaturation()
    {
        colorGrading.saturation.value = desaturationAmount;
    }

    public void ResetSaturation()
    {
        colorGrading.saturation.value = originalSaturation;
    }
}
