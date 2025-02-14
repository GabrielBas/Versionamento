using System.Collections;
using UnityEngine;

public class PickupRangeActivator : MonoBehaviour
{
    public float activationDuration = 10f; // Dura��o do efeito de atra��o (em segundos)
    public float cooldownTime = 20f; // Tempo para reativar o pickup range (em segundos)
    public float pickupRange = 100f; // Raio de atra��o dos drops
    private float originalPickupRange; // Armazena o valor original do pickup range
    private string cooldownID; // ID para o CooldownManager
    private SpriteRenderer spriteRenderer; // Refer�ncia ao SpriteRenderer do objeto

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Start()
    {
        // Gerar um ID �nico para este objeto
        cooldownID = "PickupRangeActivator_" + GetInstanceID();

        // Obter refer�ncia ao SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Obt�m os limites do mapa
        BoxCollider2D boundsCollider = GameObject.FindGameObjectWithTag("MapBounds").GetComponent<BoxCollider2D>();
        minBounds = boundsCollider.bounds.min;
        maxBounds = boundsCollider.bounds.max;

        // Define uma posi��o aleat�ria dentro dos limites do mapa
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

        // Aumenta o raio de atra��o dos drops
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

        // Tornar o sprite invis�vel durante o cooldown
        spriteRenderer.enabled = false;

        // Aguarde a dura��o do efeito
        yield return new WaitForSeconds(activationDuration);

        // Restaura o pickup range original
        PlayerStatController.instance.pickupRange[0].value = originalPickupRange;

        // Aguarde at� o cooldown acabar
        while (CooldownManager.instance.IsOnCooldown(cooldownID))
        {
            yield return null;
        }

        // Reativar o sprite ap�s o cooldown
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
        int attempts = 10; // N�mero m�ximo de tentativas para encontrar uma posi��o v�lida

        do
        {
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            float randomY = Random.Range(minBounds.y, maxBounds.y);
            spawnPosition = new Vector2(randomX, randomY);

            // Evita spawnar em obst�culos
            attempts--;
        }
        while (attempts > 0 && !IsPositionValid(spawnPosition));

        return spawnPosition;
    }

    private bool IsPositionValid(Vector2 position)
    {
        // Verifica se a posi��o est� livre de obst�culos
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
