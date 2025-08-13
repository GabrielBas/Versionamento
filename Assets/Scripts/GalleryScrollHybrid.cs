using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class GalleryScrollHybrid : MonoBehaviour
{
    [HideInInspector] public bool skipInitialSelection = false; // <<< NOVO
    public ScrollRect scrollRect;
    public GameObject outlinePrefab;

    private List<Button> items = new List<Button>();
    private int currentIndex = 0;
    private GameObject currentOutline;

    private void OnEnable()
    {
        //RebuildItems();

        // Só seleciona automaticamente se não for controlado por outro script
        if (!skipInitialSelection && items.Count > 0)
        {
            SelectIndex(0);
            EventSystem.current.SetSelectedGameObject(items[0].gameObject);
        }
    }

    //private void RebuildItems()
    //{
    //    items.Clear();
    //    foreach (Transform child in scrollRect.content)
    //    {
    //        Button btn = child.GetComponent<Button>();
    //        if (btn != null)
    //            items.Add(btn);
    //    }
    //}

    public void SelectIndex(int index)
    {
        if (items.Count == 0) return;

        currentIndex = Mathf.Clamp(index, 0, items.Count - 1);
        EventSystem.current.SetSelectedGameObject(items[currentIndex].gameObject);
        UpdateOutline();
    }

    private void UpdateOutline()
    {
        if (currentOutline != null) Destroy(currentOutline);

        if (outlinePrefab != null && items.Count > 0 && currentIndex >= 0)
        {
            currentOutline = Instantiate(outlinePrefab, items[currentIndex].transform);
            currentOutline.transform.SetAsFirstSibling();
        }
    }
}
