using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public static CoinController instance;

    public int currentCoins;

    public CoinPickup coin;

    private void Awake()
    {
        instance = this;

        // Carregar o número de moedas ao iniciar o jogo
        LoadCoins();
    }

    private void OnApplicationQuit()
    {
        // Salvar o número de moedas quando o jogo é fechado
        SaveCoins();
    }

    public void AddCoins(int coinsToAdd)
    {
        currentCoins += coinsToAdd;

        // Atualizar a interface do usuário, se possível
        if (UIController.instance != null)
        {
            UIController.instance.UpdateCoins();
        }

        FindObjectOfType<CoinUIUpdater>()?.UpdateCoinsUI(); // Atualiza a UI de moedas

        // Salvar o novo total de moedas
        SaveCoins();
    }

    public void DropCoin(Vector3 position, int value)
    {
        CoinPickup newCoin = Instantiate(coin, position + new Vector3(.2f, .1f, 0f), Quaternion.identity);
        newCoin.coinAmount = value;
        newCoin.gameObject.SetActive(true);
    }

    public void SpendCoins(int coinsToSpend)
    {
        currentCoins -= coinsToSpend;

        // Atualizar a interface do usuário, se possível
        if (UIController.instance != null)
        {
            UIController.instance.UpdateCoins();
        }

        FindObjectOfType<CoinUIUpdater>()?.UpdateCoinsUI(); // Atualiza a UI de moedas

        // Salvar o novo total de moedas
        SaveCoins();
    }

    private void SaveCoins()
    {
        // Salvar o número de moedas usando PlayerPrefs
        PlayerPrefs.SetInt("PlayerCoins", currentCoins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        // Carregar o número de moedas usando PlayerPrefs
        currentCoins = PlayerPrefs.GetInt("PlayerCoins", 0); // 0 é o valor padrão se não houver dados salvos

        // Atualizar a interface do usuário, se possível
        if (UIController.instance != null)
        {
            UIController.instance.UpdateCoins();
        }

        Debug.Log(PlayerPrefs.GetInt("PlayerCoins"));
    }
}
