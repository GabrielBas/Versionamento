using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
// Script separado para controlar o efeito de pós-processamento
public class GrayscaleEffectController : MonoBehaviour
{
    public static GrayscaleEffectController Instance;
    private Volume volume;
    private ColorAdjustments colorAdjustments;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        volume = GetComponent<Volume>();
        if (volume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.saturation.value = 0f;
        }
    }

    public void SetGrayscale(bool active)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = active ? -100f : 0f;
        }
    }
}
