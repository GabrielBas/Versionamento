using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private bool gameActive;
    public float timer;

    public float waitToShowEndScreen = 3f;

    // Start is called before the first frame update
    void Start()
    {
        gameActive = true;  
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive == true)
        {
            timer += Time.deltaTime;
            UIController.instance.UpdateTimer(timer);
        }
    }

    public void EndLevel()
    {
        gameActive = false;


        StartCoroutine(EndLevelCo());
    }

    IEnumerator EndLevelCo()
    {
        
        yield return new WaitForSeconds(waitToShowEndScreen);
        
        float minutes = Mathf.FloorToInt(timer / 60f);
        float seconds = Mathf.FloorToInt(timer % 60);

        UIController.instance.endTimeText.text = minutes.ToString() + " mins " + seconds.ToString("00" + " secs");
        UIController.instance.levelEndScreen.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Jogo pausado ap�s o fim do n�vel.");
    }

}
