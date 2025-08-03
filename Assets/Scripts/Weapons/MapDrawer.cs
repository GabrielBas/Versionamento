using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Novo sistema de Input

public class MapDrawer : MonoBehaviour
{
    private TrailRenderer trail;
    private bool isDrawing = false;

    private static List<MapDrawer> allMapDrawers = new List<MapDrawer>();

    // Ação de input combinada (mouse e controle)
    private InputAction drawAction;

    void Awake()
    {
        allMapDrawers.Add(this);

        // Criar ação de input
        drawAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/rightButton");
        drawAction.AddBinding("<Gamepad>/buttonSouth"); // botão X (Xbox) ou bolinha (PS)
        drawAction.Enable();
    }

    void OnDestroy()
    {
        allMapDrawers.Remove(this);
        drawAction.Disable();
    }

    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false;
    }

    void Update()
    {
        // Se pressionou botão do mouse ou botão do controle
        if (drawAction.WasPressedThisFrame())
        {
            isDrawing = !isDrawing;
            ToggleDrawingForAll(isDrawing);
        }
    }

    private static void ToggleDrawingForAll(bool shouldDraw)
    {
        foreach (var mapDrawer in allMapDrawers)
        {
            if (mapDrawer != null && mapDrawer.trail != null)
            {
                mapDrawer.trail.emitting = shouldDraw;
            }
        }
    }
}
