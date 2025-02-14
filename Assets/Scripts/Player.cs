using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    private void Awake()
    {
        instance = this;
    }

    public Animator anim;
    public SpriteRenderer sprite;
    public Rigidbody2D rb;
    public float speed;

    Vector2 playerDirection;

    public float pickupRange = 1.5f;

    public List<Weapon> unassignedWeapons, assignedWeapons;

    public int maxWeapons;

    [HideInInspector]
    public List<Weapon> fullyLevelledWeapons = new List<Weapon>();

    // Dash variables
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;
    private float dashTime;
    public int dashStaminaCost = 15;

    private int originalLayer;

    // Assumption: This variable checks if the upgrade menu is active
    public bool levelUpPanel = false;

    void Start()
    {
        CleanWeaponLists();

        if (assignedWeapons.Count == 0)
        {
            AddWeapon(Random.Range(0, unassignedWeapons.Count));
        }

        speed = PlayerStatController.instance.moveSpeed[0].value;
        pickupRange = PlayerStatController.instance.pickupRange[0].value;
        maxWeapons = Mathf.RoundToInt(PlayerStatController.instance.maxWeapons[0].value);

        // Save the player's original layer
        originalLayer = gameObject.layer;
    }

    void Update()
    {
        playerDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Handle dash input
        if (canDash && Input.GetKeyDown(KeyCode.Space) && !levelUpPanel)
        {
            if (StaminaBar.instance.staminaBar.value >= dashStaminaCost)
            {
                StaminaBar.instance.UseStamina(dashStaminaCost);
                StartCoroutine(Dash());
            }
            else
            {
                Debug.Log("Not enough stamina to dash");
            }
        }

        if (!isDashing)
        {
            // Handle player direction and animation
            if (playerDirection.x > 0)
            {
                sprite.flipX = false;
            }
            else if (playerDirection.x < 0)
            {
                sprite.flipX = true;
            }

            anim.SetBool("walking", playerDirection.sqrMagnitude > 0);
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.MovePosition(rb.position + playerDirection.normalized * speed * Time.deltaTime);
        }

        SFXManager.instance.PlaySFX(0);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        dashTime = dashDuration;

        // Temporarily change the player's layer to ignore enemy collisions
        gameObject.layer = LayerMask.NameToLayer("PlayerDash");

        Vector2 dashDirection = playerDirection.normalized;

        while (dashTime > 0)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.deltaTime);
            dashTime -= Time.deltaTime;
            yield return null;
        }

        // Revert the player's layer back to the original layer
        gameObject.layer = originalLayer;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void CleanWeaponLists()
    {
        if (unassignedWeapons != null)
        {
            unassignedWeapons.RemoveAll(weapon => weapon == null);
        }

        if (assignedWeapons != null)
        {
            assignedWeapons.RemoveAll(weapon => weapon == null);
        }
    }


    public void AddWeapon(int weaponNumber)
    {
        CleanWeaponLists(); // Garante que as listas estejam limpas

        if (weaponNumber < unassignedWeapons.Count && unassignedWeapons[weaponNumber] != null)
        {
            Weapon weaponToAdd = unassignedWeapons[weaponNumber];
            if (weaponToAdd != null)
            {
                assignedWeapons.Add(weaponToAdd);
                weaponToAdd.gameObject.SetActive(true);
                unassignedWeapons.RemoveAt(weaponNumber);
            }
        }
    }


    public void AddWeapon(Weapon weaponToAdd)
    {
        if (weaponToAdd != null && unassignedWeapons.Contains(weaponToAdd))
        {
            assignedWeapons.Add(weaponToAdd);
            weaponToAdd.gameObject.SetActive(true);
            unassignedWeapons.Remove(weaponToAdd);
        }
    }

    public void RemoveWeapon(Weapon weaponToRemove)
    {
        if (weaponToRemove != null)
        {
            if (unassignedWeapons.Contains(weaponToRemove))
            {
                unassignedWeapons.Remove(weaponToRemove);
            }
            if (assignedWeapons.Contains(weaponToRemove))
            {
                assignedWeapons.Remove(weaponToRemove);
            }
        }
    }

    private void OnValidate()
    {
        CleanWeaponLists();
    }

}
