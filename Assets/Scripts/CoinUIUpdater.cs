using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUIUpdater : MonoBehaviour
{
    public Text coinsText; // Referência ao texto da UI

    void Start()
    {
        UpdateCoinsUI(); // Atualiza a UI ao iniciar
    }

    void OnEnable()
    {
        UpdateCoinsUI(); // Atualiza sempre que o objeto for ativado
    }

    public void UpdateCoinsUI()
    {
        if (CoinController.instance != null)
        {
            coinsText.text = "Coins: " + CoinController.instance.currentCoins.ToString();
        }
        else
        {
            Debug.LogError("CoinController.instance está nulo! Verifique se CoinController está na cena.");
        }
    }
}
