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

        // Carregar o n�mero de moedas ao iniciar o jogo
        LoadCoins();
    }

    private void OnApplicationQuit()
    {
        // Salvar o n�mero de moedas quando o jogo � fechado
        SaveCoins();
    }

    public void AddCoins(int coinsToAdd)
    {
        currentCoins += coinsToAdd;

        // Atualizar a interface do usu�rio, se poss�vel
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

        // Atualizar a interface do usu�rio, se poss�vel
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
        // Salvar o n�mero de moedas usando PlayerPrefs
        PlayerPrefs.SetInt("PlayerCoins", currentCoins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        // Carregar o n�mero de moedas usando PlayerPrefs
        currentCoins = PlayerPrefs.GetInt("PlayerCoins", 0); // 0 � o valor padr�o se n�o houver dados salvos

        // Atualizar a interface do usu�rio, se poss�vel
        if (UIController.instance != null)
        {
            UIController.instance.UpdateCoins();
        }

        Debug.Log(PlayerPrefs.GetInt("PlayerCoins"));
    }
}
