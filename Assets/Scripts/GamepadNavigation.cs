using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // novo sistema de input

public class GamepadNavigation : MonoBehaviour
{
    public SwipeController swipeController; // arraste no inspetor
    public GameObject firstSelected;        // bot�o inicial (ex: Play)

    private void OnEnable()
    {
        // For�a sele��o inicial
        if (firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    void Update()
    {
        if (Gamepad.current == null) return;

        // Vai para a pr�xima p�gina
        if (Gamepad.current.dpad.right.wasPressedThisFrame || Gamepad.current.rightShoulder.wasPressedThisFrame)
        {
            swipeController.Next();
        }

        // Vai para a p�gina anterior
        if (Gamepad.current.dpad.left.wasPressedThisFrame || Gamepad.current.leftShoulder.wasPressedThisFrame)
        {
            swipeController.Previous();
        }
    }
}
