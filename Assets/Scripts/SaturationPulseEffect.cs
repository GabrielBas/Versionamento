using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SaturationPulseEffect : MonoBehaviour
{
    public float pulseSpeed = 2f;  // Velocidade da pulsa��o
    public float effectDuration = 5f;  // Dura��o total do efeito

    public PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;
    private bool isPulsing = false;

    void OnEnable()
    {
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<PostProcessVolume>();
        }

        if (postProcessVolume != null && postProcessVolume.profile.TryGetSettings(out colorGrading))
        {
            // Cancela qualquer pulso anterior e inicia o efeito
            CancelInvoke("StopPulse");
            StartCoroutine(SaturationPulse());
            Invoke("StopPulse", effectDuration);
        }
        else
        {
            Debug.LogError("Color Grading n�o encontrado no Post Process Volume!");
        }
    }

    IEnumerator SaturationPulse()
    {
        isPulsing = true;
        float time = 0f;

        while (isPulsing)
        {
            // Faz a satura��o oscilar de 0 a 100 com base no tempo
            colorGrading.saturation.value = Mathf.Lerp(0f, 100f, Mathf.PingPong(time * pulseSpeed, 1f));
            time += Time.deltaTime;
            yield return null;
        }
    }

    void StopPulse()
    {
        isPulsing = false;
        if (colorGrading != null)
        {
            colorGrading.saturation.value = 0f;  // Restaura a satura��o normal
        }
    }
}
