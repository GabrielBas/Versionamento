using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SnapScrollView : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 10f;
    private int currentIndex = 0;
    private int totalItems;
    private float[] itemPositions;

    private bool canMove = true;
    public float moveCooldown = 0.2f;

    private bool initialized = false;



    void Start()
    {
        // Não inicializar ainda, será chamado após carregar thumbnails
    }

    public void Initialize()
    {
        totalItems = scrollRect.content.childCount;

        if (totalItems == 0) return;

        itemPositions = new float[totalItems];
        for (int i = 0; i < totalItems; i++)
        {
            if (totalItems == 1)
                itemPositions[i] = 1f;
            else
                itemPositions[i] = 1f - ((float)i / (totalItems - 1)); // vertical
        }

        currentIndex = 0;

        if (scrollRect.content.childCount > 0)
            EventSystem.current.SetSelectedGameObject(scrollRect.content.GetChild(currentIndex).gameObject);

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;
        if (totalItems == 0) return;

        float vertical = Input.GetAxis("Vertical");

        if (canMove)
        {
            if (vertical > 0.1f) { MoveUp(); StartCoroutine(MoveCooldown()); }
            else if (vertical < -0.1f) { MoveDown(); StartCoroutine(MoveCooldown()); }
        }

        scrollRect.verticalNormalizedPosition = Mathf.Lerp(
            scrollRect.verticalNormalizedPosition,
            itemPositions[currentIndex],
            Time.deltaTime * scrollSpeed
        );
    }

    void MoveUp()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            EventSystem.current.SetSelectedGameObject(scrollRect.content.GetChild(currentIndex).gameObject);
        }
    }

    void MoveDown()
    {
        if (currentIndex < totalItems - 1)
        {
            currentIndex++;
            EventSystem.current.SetSelectedGameObject(scrollRect.content.GetChild(currentIndex).gameObject);
        }
    }

    private IEnumerator MoveCooldown()
    {
        canMove = false;
        yield return new WaitForSeconds(moveCooldown);
        canMove = true;
    }
}
