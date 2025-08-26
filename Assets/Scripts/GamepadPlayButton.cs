using UnityEngine;
using UnityEngine.UI;

public class GamepadPlayButton : MonoBehaviour
{
    public Button playButton; // arraste o botão PLAY no inspector

    void Update()
    {
        // Botão A (Xbox) ou Cross (PS) -> Joystick1Button0
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            if (playButton != null && playButton.gameObject == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
            {
                playButton.onClick.Invoke();
            }
        }
    }
}
