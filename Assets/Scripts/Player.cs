using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Armas do jogador")]
    public List<Weapon> unassignedWeapons, assignedWeapons;
    public int maxWeapons;

    [Tooltip("Defina aqui a arma inicial do jogador (opcional)")]
    public Weapon startingWeapon;

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

    public bool levelUpPanel = false;

    [Header("Som de passos")]
    public AudioClip footstepSound;
    public float footstepInterval = 0.4f;

    private float footstepTimer = 0f;
    private AudioSource audioSource;

    [Header("Som de dash")]
    public AudioClip dashSound;

    private PlayerInput playerInput;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        CleanWeaponLists();

        // 🔹 Se tiver arma inicial definida no Inspector → adiciona
        if (startingWeapon != null && unassignedWeapons.Contains(startingWeapon))
        {
            AddWeapon(startingWeapon);
        }
        // 🔹 Caso contrário, escolhe aleatória
        else if (assignedWeapons.Count == 0 && unassignedWeapons.Count > 0)
        {
            AddWeapon(Random.Range(0, unassignedWeapons.Count));
        }

        speed = PlayerStatController.instance.moveSpeed[0].value;
        pickupRange = PlayerStatController.instance.pickupRange[0].value;
        maxWeapons = Mathf.RoundToInt(PlayerStatController.instance.maxWeapons[0].value);

        originalLayer = gameObject.layer;
    }

    void Update()
    {
        if (!isDashing)
        {
            if (playerDirection.x > 0)
                sprite.flipX = false;
            else if (playerDirection.x < 0)
                sprite.flipX = true;

            anim.SetBool("walking", playerDirection.sqrMagnitude > 0);
        }

        if (playerDirection.sqrMagnitude > 0 && !isDashing)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                if (footstepSound != null && audioSource != null)
                    audioSource.PlayOneShot(footstepSound);

                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.MovePosition(rb.position + playerDirection.normalized * speed * Time.deltaTime);
        }
    }

    private IEnumerator Dash(Vector2 dashDirection)
    {
        canDash = false;
        isDashing = true;
        dashTime = dashDuration;

        gameObject.layer = LayerMask.NameToLayer("PlayerDash");

        if (dashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        while (dashTime > 0)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.deltaTime);
            dashTime -= Time.deltaTime;
            yield return null;
        }

        gameObject.layer = originalLayer;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerDirection = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.started || !canDash || levelUpPanel || isDashing)
            return;

        if (StaminaBar.instance.staminaBar.value >= dashStaminaCost)
        {
            StaminaBar.instance.UseStamina(dashStaminaCost);
            StartCoroutine(Dash(playerDirection.normalized));
        }
        else
        {
            Debug.Log("Not enough stamina to dash");
        }
    }

    private void CleanWeaponLists()
    {
        if (unassignedWeapons != null)
            unassignedWeapons.RemoveAll(weapon => weapon == null);

        if (assignedWeapons != null)
            assignedWeapons.RemoveAll(weapon => weapon == null);
    }

    public void AddWeapon(int weaponNumber)
    {
        CleanWeaponLists();

        if (weaponNumber < unassignedWeapons.Count && unassignedWeapons[weaponNumber] != null)
        {
            Weapon weaponToAdd = unassignedWeapons[weaponNumber];
            if (weaponToAdd != null)
            {
                assignedWeapons.Add(weaponToAdd);
                weaponToAdd.gameObject.SetActive(true);
                UpdateWeaponIconsUI();
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
            UpdateWeaponIconsUI();
            unassignedWeapons.Remove(weaponToAdd);
        }
    }

    public void RemoveWeapon(Weapon weaponToRemove)
    {
        if (weaponToRemove != null)
        {
            if (unassignedWeapons.Contains(weaponToRemove))
                unassignedWeapons.Remove(weaponToRemove);

            if (assignedWeapons.Contains(weaponToRemove))
            {
                UpdateWeaponIconsUI();
                assignedWeapons.Remove(weaponToRemove);
            }
        }
    }

    private void OnValidate()
    {
        CleanWeaponLists();
    }

    public void UpdateWeaponIconsUI()
    {
        if (WeaponIconsManager.instance != null)
            WeaponIconsManager.instance.UpdateWeaponIcons();
    }
}
