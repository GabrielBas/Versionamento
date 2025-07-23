using System.Collections;
using UnityEngine;

public class AirplaneActivator : MonoBehaviour
{
    [Header("Objeto com Animator que toca a animação")]
    [SerializeField] private GameObject airPlaneActivePrefab;

    [Header("Nome do Trigger da animação (opcional)")]
    [SerializeField] private string animationTriggerName = "Activate";

    [Header("Nome exato do AnimationClip")]
    [SerializeField] private string animationClipName = "AirplaneActivate";

    [Header("Usar Play direto (ignora Trigger)")]
    [SerializeField] private bool forcePlayClipDirectly = false;

    [Header("Duração fallback (segundos)")]
    [SerializeField] private float fallbackDuration = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AirplaneController controller = FindObjectOfType<AirplaneController>();
            if (controller != null)
            {
                controller.ActivateAirplane();
                StartCoroutine(PlayAnimationAndDestroy());
            }
        }
    }

    private IEnumerator PlayAnimationAndDestroy()
    {
        float clipLength = fallbackDuration;

        if (airPlaneActivePrefab != null)
        {
            if (!airPlaneActivePrefab.activeInHierarchy)
                airPlaneActivePrefab.SetActive(true);

            Animator animator = airPlaneActivePrefab.GetComponent<Animator>();
            if (animator != null)
            {
                RuntimeAnimatorController controller = animator.runtimeAnimatorController;

                if (controller != null && controller.animationClips.Length > 0)
                {
                    foreach (AnimationClip clip in controller.animationClips)
                    {
                        if (clip.name == animationClipName)
                        {
                            clipLength = clip.length;
                            Debug.Log($"Duração da animação '{clip.name}' = {clipLength}");
                            break;
                        }
                    }
                }

                if (forcePlayClipDirectly)
                {
                    animator.Play(animationClipName, 0, 0f);
                    Debug.Log($"Forçando Play direto do Clip '{animationClipName}'.");
                }
                else
                {
                    animator.ResetTrigger(animationTriggerName);
                    animator.SetTrigger(animationTriggerName);
                    Debug.Log($"Trigger '{animationTriggerName}' disparado.");
                }
            }
            else
            {
                Debug.LogWarning("Nenhum Animator encontrado no airPlaneActivePrefab.");
            }

            if (airPlaneActivePrefab.transform.parent == transform)
            {
                airPlaneActivePrefab.transform.parent = null;
            }
        }
        else
        {
            Debug.LogWarning("airPlaneActivePrefab não atribuído no Inspector!");
        }

        Debug.Log($"Aguardando {clipLength} segundos para destruir o Activator.");
        yield return new WaitForSeconds(clipLength);

        Destroy(gameObject);
    }
}
