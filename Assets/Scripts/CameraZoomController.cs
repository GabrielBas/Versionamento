using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    public float zoomSpeed = 2f;        // Velocidade de zoom ao usar o scroll
    public float minZoom = 2f;          // Zoom m�nimo permitido
    public float maxZoom = 10f;         // Zoom m�ximo permitido

    private Camera cam;

    void Start()
    {
        cam = Camera.main; // Obt�m a c�mera principal
    }

    void Update()
    {
        HandleZoom();
    }

    private void HandleZoom()
    {
        // Obt�m o valor do scroll do mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Ajusta o tamanho ortogr�fico da c�mera com base no valor de scroll
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
