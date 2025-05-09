using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CursorManager : MonoBehaviour
{
    public Texture2D customCursor; // A textura do cursor personalizada
    public Vector2 hotSpot = Vector2.zero; // Ponto quente (posição clicável no cursor)
    public CursorMode cursorMode = CursorMode.Auto; // Modo do cursor (Auto ou ForceSoftware)

    void Start()
    {
        // Verifica se o cursor foi atribuído
        if (customCursor != null)
        {
            Cursor.SetCursor(customCursor, hotSpot, cursorMode);
        }
        else
        {
            Debug.LogWarning("Cursor personalizado não atribuído no Inspetor!");
        }
    }
}