using System.Collections;
using UnityEngine;

public class PickupRangeActivator : MonoBehaviour
{
    [Header("Configuração do Pickup Range")]
    public float activationDuration = 10f; // Duração do efeito
    public float cooldownTime = 20f; // Tempo de cooldown
    public float pickupRange = 100f; // Alcance de atração
    public float pickupMoveSpeed = 10f; // ✅ VELOCIDADE DE ATRAÇÃO!

    private float originalPickupRange;
    private string cooldownID;
    private SpriteRenderer spriteRenderer;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Start()
    {
        cooldownID = "PickupRangeActivator_" + GetInstanceID();
        spriteRenderer = GetComponent<SpriteRenderer>();

        BoxCollider2D boundsCollider = GameObject.FindGameObjectWithTag("MapBounds").GetComponent<BoxCollider2D>();
        minBounds = boundsCollider.bounds.min;
        maxBounds = boundsCollider.bounds.max;

        transform.position = GetRandomPositionWithinBounds();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !CooldownManager.instance.IsOnCooldown(cooldownID))
        {
            StartCoroutine(ActivatePickupRange(other.gameObject));
        }
    }

    private IEnumerator ActivatePickupRange(GameObject player)
    {
        CooldownManager.instance.StartCooldown(cooldownID, cooldownTime);

        originalPickupRange = PlayerStatController.instance.pickupRange[0].value;
        PlayerStatController.instance.pickupRange[0].value = pickupRange;

        Collider2D[] drops = Physics2D.OverlapCircleAll(player.transform.position, pickupRange, LayerMask.GetMask("Drop"));
        foreach (Collider2D drop in drops)
        {
            if (drop != null)
            {
                StartCoroutine(MoveTowardsPlayer(drop.gameObject, player));
            }
        }

        SetSpriteRenderersEnabled(false);

        yield return new WaitForSeconds(activationDuration);

        PlayerStatController.instance.pickupRange[0].value = originalPickupRange;

        while (CooldownManager.instance.IsOnCooldown(cooldownID))
        {
            yield return null;
        }

        transform.position = GetRandomPositionWithinBounds();
        SetSpriteRenderersEnabled(true);

        Debug.Log("Pickup Range Machine reactivated at new position!");
    }

    private IEnumerator MoveTowardsPlayer(GameObject drop, GameObject player)
    {
        while (drop != null && Vector2.Distance(drop.transform.position, player.transform.position) > 0.1f)
        {
            drop.transform.position = Vector2.MoveTowards(
                drop.transform.position,
                player.transform.position,
                pickupMoveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private Vector2 GetRandomPositionWithinBounds()
    {
        Vector2 spawnPosition;
        int attempts = 10;

        do
        {
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            float randomY = Random.Range(minBounds.y, maxBounds.y);
            spawnPosition = new Vector2(randomX, randomY);

            attempts--;
        }
        while (attempts > 0 && !IsPositionValid(spawnPosition));

        return spawnPosition;
    }

    private bool IsPositionValid(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 1f, LayerMask.GetMask("Obstacles"));
        return hit == null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }

    private void SetSpriteRenderersEnabled(bool enabled)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in renderers)
        {
            sr.enabled = enabled;
        }
    }
}
