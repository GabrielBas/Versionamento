using UnityEngine;

public class PersistentInputManager : MonoBehaviour
{
    private static PersistentInputManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
