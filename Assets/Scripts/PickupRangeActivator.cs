using System.Collections;
using UnityEngine;

public class PickupRangeActivator : MonoBehaviour
{
    public float activationDuration = 10f; // Duração do efeito de atração (em segundos)
    public float cooldownTime = 20f; // Tempo para reativar o pickup range (em segundos)
    public float pickupRange = 100f; // Raio de atração dos drops
    private float originalPickupRange; // Armazena o valor original do pickup range
    private string cooldownID; // ID para o CooldownManager
    private SpriteRenderer spriteRenderer; // Referência ao SpriteRenderer do objeto

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Start()
    {
        // Gerar um ID único para este objeto
        cooldownID = "PickupRangeActivator_" + GetInstanceID();

        // Obter referência ao SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Obtém os limites do mapa
        BoxCollider2D boundsCollider = GameObject.FindGameObjectWithTag("MapBounds").GetComponent<BoxCollider2D>();
        minBounds = boundsCollider.bounds.min;
        maxBounds = boundsCollider.bounds.max;

        // Define uma posição aleatória dentro dos limites do mapa
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
        // Inicia o cooldown no CooldownManager
        CooldownManager.instance.StartCooldown(cooldownID, cooldownTime);

        // Armazena o valor original do pickup range
        originalPickupRange = PlayerStatController.instance.pickupRange[0].value;

        // Aumenta o raio de atração dos drops
        PlayerStatController.instance.pickupRange[0].value = pickupRange;

        // Atraia os drops ao redor do mapa
        Collider2D[] drops = Physics2D.OverlapCircleAll(player.transform.position, pickupRange, LayerMask.GetMask("Drop"));
        foreach (Collider2D drop in drops)
        {
            if (drop != null)
            {
                StartCoroutine(MoveTowardsPlayer(drop.gameObject, player));
            }
        }

        // Tornar o sprite invisível durante o cooldown
        spriteRenderer.enabled = false;

        // Aguarde a duração do efeito
        yield return new WaitForSeconds(activationDuration);

        // Restaura o pickup range original
        PlayerStatController.instance.pickupRange[0].value = originalPickupRange;

        // Aguarde até o cooldown acabar
        while (CooldownManager.instance.IsOnCooldown(cooldownID))
        {
            yield return null;
        }

        // Reativar o sprite após o cooldown
        spriteRenderer.enabled = true;

        Debug.Log("Pickup Range Machine reactivated");
    }

    private IEnumerator MoveTowardsPlayer(GameObject drop, GameObject player)
    {
        while (drop != null && Vector2.Distance(drop.transform.position, player.transform.position) > 0.1f)
        {
            drop.transform.position = Vector2.MoveTowards(drop.transform.position, player.transform.position, Time.deltaTime * 10f);
            yield return null;
        }
    }

    private Vector2 GetRandomPositionWithinBounds()
    {
        Vector2 spawnPosition;
        int attempts = 10; // Número máximo de tentativas para encontrar uma posição válida

        do
        {
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            float randomY = Random.Range(minBounds.y, maxBounds.y);
            spawnPosition = new Vector2(randomX, randomY);

            // Evita spawnar em obstáculos
            attempts--;
        }
        while (attempts > 0 && !IsPositionValid(spawnPosition));

        return spawnPosition;
    }

    private bool IsPositionValid(Vector2 position)
    {
        // Verifica se a posição está livre de obstáculos
        Collider2D hit = Physics2D.OverlapCircle(position, 1f, LayerMask.GetMask("Obstacles"));
        return hit == null;
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha um gizmo para visualizar o raio de pickup range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
