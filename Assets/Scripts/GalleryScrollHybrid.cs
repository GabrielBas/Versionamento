using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GalleryScrollHybrid : MonoBehaviour
{
    [Header("Referências")]
    public ScrollRect scrollRect;               // ScrollRect vertical
    public Transform content;                   // Content com os thumbnails (cada filho tem Button)

    [Header("D-Pad Repeat")]
    public float dpadInitialDelay = 0.35f;      // tempo antes do primeiro repeat
    public float dpadRepeatRate = 0.12f;        // intervalo entre repeats

    [HideInInspector] public bool skipInitialSelection = false;

    // Estado interno
    private readonly List<Button> items = new List<Button>();
    private int currentIndex = -1;
    private GameObject lastSelectedGO;

    // Repeat
    private float repeatTimer = 0f;
    private int heldDir = 0; // -1 = up, +1 = down

    // Controle do EventSystem
    private bool prevSendNav = true;

    public int CurrentIndex => currentIndex;

    private void Awake()
    {
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
        if (content == null && scrollRect != null) content = scrollRect.content;
    }

    private void OnEnable()
    {
        // Desliga navegação automática do módulo de UI enquanto a galeria está ativa
        if (EventSystem.current != null)
        {
            prevSendNav = EventSystem.current.sendNavigationEvents;
            EventSystem.current.sendNavigationEvents = false;
        }

        RebuildItems();

        if (!skipInitialSelection && items.Count > 0)
            SelectIndexInternal(0, centerInView: true);
    }

    private void OnDisable()
    {
        // Restaura navegação do EventSystem
        if (EventSystem.current != null)
            EventSystem.current.sendNavigationEvents = prevSendNav;
    }

    private void Update()
    {
        // Recarrega lista se mudou a quantidade de filhos
        if (content != null && items.Count != content.childCount)
        {
            RebuildItems();
            if (currentIndex < 0 && items.Count > 0)
                SelectIndexInternal(0, centerInView: true);
        }

        HandleDPad();

        // Clique do mouse muda o selecionado via EventSystem; sincroniza índice
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            var esGO = EventSystem.current.currentSelectedGameObject;
            int idx = IndexOfItem(esGO);
            if (idx >= 0 && idx != currentIndex)
                SelectIndexInternal(idx, centerInView: false);
        }
    }

    // ---------- API pública ----------
    public void SelectIndexPublic(int index, bool centerInView = true)
    {
        SelectIndexInternal(index, centerInView);
    }

    // ---------- Navegação D-Pad ----------
    private void HandleDPad()
    {
        int dir = 0;

        bool upPressed =
            (Gamepad.current != null && Gamepad.current.dpad.up.wasPressedThisFrame) ||
            (Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame);
        bool downPressed =
            (Gamepad.current != null && Gamepad.current.dpad.down.wasPressedThisFrame) ||
            (Keyboard.current != null && Keyboard.current.downArrowKey.wasPressedThisFrame);

        if (upPressed) dir = -1;
        else if (downPressed) dir = +1;

        // Primeiro passo instantâneo
        if (dir != 0)
        {
            MoveSelection(dir);
            heldDir = dir;
            repeatTimer = dpadInitialDelay;
        }
        else
        {
            // Hold para repetir
            float heldY = 0f;
            if (Gamepad.current != null)
            {
                if (Gamepad.current.dpad.up.isPressed) heldY = -1f;
                else if (Gamepad.current.dpad.down.isPressed) heldY = +1f;
            }
            if (Keyboard.current != null)
            {
                if (Keyboard.current.upArrowKey.isPressed) heldY = -1f;
                else if (Keyboard.current.downArrowKey.isPressed) heldY = +1f;
            }

            if (Mathf.Abs(heldY) > 0.5f)
            {
                if (heldDir == 0) heldDir = heldY > 0 ? +1 : -1;

                if (repeatTimer > 0f) repeatTimer -= Time.unscaledDeltaTime;
                else
                {
                    MoveSelection(heldDir);
                    repeatTimer = dpadRepeatRate;
                }
            }
            else
            {
                heldDir = 0;
                repeatTimer = 0f;
            }
        }

        // Confirm (A / Enter / Espaço)
        bool confirm =
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) ||
            (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame));

        if (confirm && currentIndex >= 0 && currentIndex < items.Count)
        {
            var btn = items[currentIndex];
            if (btn != null && btn.interactable) btn.onClick.Invoke();
        }
    }

    private void MoveSelection(int delta)
    {
        if (items.Count == 0) return;

        int next = currentIndex < 0 ? 0 : currentIndex + delta;
        next = Mathf.Clamp(next, 0, items.Count - 1);

        SelectIndexInternal(next, centerInView: true);
    }

    // ---------- Seleção ----------
    private void SelectIndexInternal(int index, bool centerInView)
    {
        if (items.Count == 0) return;
        index = Mathf.Clamp(index, 0, items.Count - 1);

        currentIndex = index;
        var go = items[currentIndex].gameObject;

        // manda pro EventSystem
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);

        // 🔥 Destacar usando Canvas (não mexe no sibling index)
        foreach (var btn in items)
        {
            var canvas = btn.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = false;
            }
        }

        var selectedCanvas = go.GetComponent<Canvas>();
        if (selectedCanvas == null)
            selectedCanvas = go.AddComponent<Canvas>();

        selectedCanvas.overrideSorting = true;
        selectedCanvas.sortingOrder = 999; // bem alto

        var rt = go.GetComponent<RectTransform>();
        if (rt != null) EnsureVisible(rt, centerInView);
    }


    // ---------- Scroll robusto ----------
    private void EnsureVisible(RectTransform target, bool centerInView)
    {
        if (scrollRect == null || target == null) return;

        RectTransform viewport = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();
        RectTransform contentRT = scrollRect.content;

        float viewportH = viewport.rect.height;
        float contentH = contentRT.rect.height;
        if (contentH <= viewportH) return;

        // posição do item relativa ao conteúdo
        float itemTop = Mathf.Abs(target.anchoredPosition.y);
        float itemBottom = itemTop + target.rect.height;

        float viewTop = Mathf.Abs(contentRT.anchoredPosition.y);
        float viewBottom = viewTop + viewportH;

        float newY = contentRT.anchoredPosition.y;

        if (centerInView)
        {
            // centraliza item
            newY = itemTop + target.rect.height / 2f - viewportH / 2f;
        }
        else
        {
            if (itemTop < viewTop) newY = itemTop;          // item acima → desce conteúdo
            else if (itemBottom > viewBottom) newY = itemBottom - viewportH; // item abaixo → sobe conteúdo
        }

        newY = Mathf.Clamp(newY, 0f, Mathf.Max(0f, contentH - viewportH));

        contentRT.anchoredPosition = new Vector2(contentRT.anchoredPosition.x, newY);

        // atualiza ScrollRect normalizado
        scrollRect.verticalNormalizedPosition = (contentH <= viewportH) ? 1f : 1f - (newY / (contentH - viewportH));
    }

    // ---------- Util ----------
    private void RebuildItems()
    {
        items.Clear();
        if (content == null) return;

        for (int i = 0; i < content.childCount; i++)
        {
            var child = content.GetChild(i);
            var btn = child.GetComponent<Button>();
            if (btn != null && btn.gameObject.activeInHierarchy)
                items.Add(btn);
        }
    }

    private int IndexOfItem(GameObject go)
    {
        if (go == null) return -1;
        for (int i = 0; i < items.Count; i++)
            if (items[i] != null && items[i].gameObject == go) return i;
        return -1;
    }
}
