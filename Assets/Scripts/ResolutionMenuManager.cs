using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ResolutionMenuManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private ScrollRect activeScroll;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;

        // Ajusta o scroll quando o dropdown ABRIR (mouse e gamepad)
        AddOpenListeners();

        // Mantém o scroll sincronizado se mudar o valor com o dropdown aberto
        resolutionDropdown.onValueChanged.AddListener(_ => StartCoroutine(AdjustScrollNextFrame()));
    }

    // ---- Detecta abertura do dropdown (mouse e gamepad) ----
    private void AddOpenListeners()
    {
        var et = resolutionDropdown.gameObject.GetComponent<EventTrigger>();
        if (et == null) et = resolutionDropdown.gameObject.AddComponent<EventTrigger>();

        AddEntry(et, EventTriggerType.PointerClick, _ => StartCoroutine(AdjustScrollNextFrame()));
        AddEntry(et, EventTriggerType.Submit, _ => StartCoroutine(AdjustScrollNextFrame())); // A/Enter no gamepad/teclado
    }

    private void AddEntry(EventTrigger et, EventTriggerType type, System.Action<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(action));
        et.triggers.Add(entry);
    }

    // Espera 1 frame (a lista é criada em Show()) e então ajusta
    private System.Collections.IEnumerator AdjustScrollNextFrame()
    {
        yield return null; // garante que "Dropdown List" já existe
        activeScroll = FindActiveDropdownScrollRect();
        AdjustScrollToCurrent();
    }

    private void AdjustScrollToCurrent()
    {
        if (activeScroll != null)
        {
            int index = Mathf.Clamp(resolutionDropdown.value, 0, Mathf.Max(0, resolutionDropdown.options.Count - 1));
            float denom = Mathf.Max(1, resolutionDropdown.options.Count - 1);
            float normalizedPos = 1f - (index / denom); // topo = 1, fundo = 0
            activeScroll.verticalNormalizedPosition = normalizedPos;
        }
    }

    void Update()
    {
        // Quando a lista estiver aberta, acompanha o item selecionado pelo Gamepad/Teclado
        if (activeScroll != null && activeScroll.gameObject.activeInHierarchy)
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null && selected.transform.IsChildOf(activeScroll.content))
            {
                int childIndex = selected.transform.GetSiblingIndex();
                int childCount = activeScroll.content.childCount - 1;
                float normalizedPos = 1f - ((float)childIndex / Mathf.Max(1, childCount));
                activeScroll.verticalNormalizedPosition = normalizedPos;
            }
        }
    }

    // Procura o ScrollRect da lista aberta (funciona para TMP e uGUI)
    private ScrollRect FindActiveDropdownScrollRect()
    {
        Transform root = resolutionDropdown.transform.root;

        // uGUI Dropdown padrão
        var t = resolutionDropdown.transform.parent.Find("Dropdown List/Viewport/Content");
        if (t != null && t.gameObject.activeInHierarchy)
            return t.GetComponentInParent<ScrollRect>();

        // TMP (nomes variam entre versões)
        var t1 = root.Find("TMP Dropdown/Viewport/Content");
        if (t1 != null && t1.gameObject.activeInHierarchy)
            return t1.GetComponentInParent<ScrollRect>();

        var t2 = root.Find("TMP Dropdown List/Viewport/Content");
        if (t2 != null && t2.gameObject.activeInHierarchy)
            return t2.GetComponentInParent<ScrollRect>();

        // Fallback: procura qualquer ScrollRect ativo com "Dropdown" no nome
        var allScrolls = root.GetComponentsInChildren<ScrollRect>(true);
        foreach (var sr in allScrolls)
        {
            if (!sr.gameObject.activeInHierarchy) continue;
            string n = sr.transform.name;
            string pn = sr.transform.parent ? sr.transform.parent.name : "";
            if (n.Contains("Dropdown") || pn.Contains("Dropdown"))
                return sr;
        }

        return null;
    }
}
