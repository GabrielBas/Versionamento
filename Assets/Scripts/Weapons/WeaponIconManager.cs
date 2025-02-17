using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIconsManager : MonoBehaviour
{
    public static WeaponIconsManager instance;

    [Header("Configurações dos Ícones")]
    public GameObject weaponIconPrefab; // Prefab do ícone de arma
    public Transform iconsContainer; // Container onde os ícones serão exibidos
    public int maxIcons = 11; // Máximo de ícones na tela

    private List<GameObject> activeIcons = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateWeaponIcons();
    }

    public void UpdateWeaponIcons()
    {
        // Limpa os ícones antigos
        foreach (GameObject icon in activeIcons)
        {
            Destroy(icon);
        }
        activeIcons.Clear();

        // Cria ícones para cada arma atribuída, limitando a quantidade máxima
        for (int i = 0; i < Player.instance.assignedWeapons.Count && i < maxIcons; i++)
        {
            Weapon weapon = Player.instance.assignedWeapons[i];
            if (weapon.icon != null)
            {
                GameObject newIcon = Instantiate(weaponIconPrefab, iconsContainer);
                newIcon.GetComponent<Image>().sprite = weapon.icon;
                newIcon.SetActive(true);

                activeIcons.Add(newIcon);
            }
        }
    }
}
