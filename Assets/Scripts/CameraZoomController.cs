using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    public float zoomSpeed = 2f;        // Velocidade de zoom ao usar o scroll
    public float minZoom = 2f;          // Zoom mínimo permitido
    public float maxZoom = 10f;         // Zoom máximo permitido

    private Camera cam;

    void Start()
    {
        cam = Camera.main; // Obtém a câmera principal
    }

    void Update()
    {
        HandleZoom();
    }

    private void HandleZoom()
    {
        // Obtém o valor do scroll do mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Ajusta o tamanho ortográfico da câmera com base no valor de scroll
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
