using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GalleryScrollHybrid : MonoBehaviour
{
    [Header("Referências")]
    public ScrollRect scrollRect;
    public GameObject scrollbarVertical;
    public Transform content; // Onde ficam as imagens (com Button ou Selectable)

    [Header("Configuração")]
    public string verticalAxis = "Vertical";
    public float scrollSpeed = 2f;
    public float snapDelay = 0.2f; // tempo para travar o foco após parar de rolar

    private float scrollTimer;

    private void OnEnable()
    {
        SelectFirstItem();
    }

    private void Update()
    {
        float move = Input.GetAxis(verticalAxis);

        if (Mathf.Abs(move) > 0.1f)
        {
            // Movimento do analógico → rola manualmente
            scrollRect.verticalNormalizedPosition += move * scrollSpeed * Time.unscaledDeltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);

            scrollTimer = snapDelay; // reinicia o timer de foco
        }
        else
        {
            // Quando parar de mover, tenta focar item visível mais centralizado
            if (scrollTimer > 0)
            {
                scrollTimer -= Time.unscaledDeltaTime;
                if (scrollTimer <= 0)
                {
                    SnapToNearestItem();
                }
            }
        }

        // Navegação direta por setas/d-pad
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("DPadY") < -0.5f)
        {
            SelectNextItem();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("DPadY") > 0.5f)
        {
            SelectPreviousItem();
        }
    }

    private void SelectFirstItem()
    {
        GameObject firstSelectable = GetFirstSelectable();
        EventSystem.current.SetSelectedGameObject(null);

        if (firstSelectable != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectable);
        }
        else if (scrollbarVertical != null)
        {
            EventSystem.current.SetSelectedGameObject(scrollbarVertical);
        }
    }

    private void SnapToNearestItem()
    {
        GameObject nearest = GetNearestCenteredSelectable();
        if (nearest != null)
        {
            EventSystem.current.SetSelectedGameObject(nearest);
        }
    }

    private GameObject GetFirstSelectable()
    {
        foreach (Transform child in content)
        {
            if (child.GetComponent<Selectable>() != null)
                return child.gameObject;
        }
        return null;
    }

    private GameObject GetNearestCenteredSelectable()
    {
        float closestDist = Mathf.Infinity;
        GameObject closestObj = null;

        foreach (Transform child in content)
        {
            if (child.GetComponent<Selectable>() != null)
            {
                Vector3 viewportPos = scrollRect.viewport.InverseTransformPoint(child.position);

                // Calcula a distância vertical do centro exato da viewport
                float dist = Mathf.Abs(viewportPos.y);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestObj = child.gameObject;
                }
            }
        }
        return closestObj;
    }

    private void SelectNextItem()
    {
        if (EventSystem.current.currentSelectedGameObject == null) return;

        Selectable current = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        if (current != null)
        {
            Selectable next = current.FindSelectableOnDown();
            if (next != null)
            {
                EventSystem.current.SetSelectedGameObject(next.gameObject);
                ScrollToItem(next.transform);
            }
        }
    }

    private void SelectPreviousItem()
    {
        if (EventSystem.current.currentSelectedGameObject == null) return;

        Selectable current = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        if (current != null)
        {
            Selectable prev = current.FindSelectableOnUp();
            if (prev != null)
            {
                EventSystem.current.SetSelectedGameObject(prev.gameObject);
                ScrollToItem(prev.transform);
            }
        }
    }

    private void ScrollToItem(Transform item)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
        Vector2 childLocalPosition = item.localPosition;

        float normalizedPos = 1 - Mathf.InverseLerp(
            content.GetChild(content.childCount - 1).localPosition.y,
            content.GetChild(0).localPosition.y,
            childLocalPosition.y
        );

        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPos);
    }
}
