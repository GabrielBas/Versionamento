using System.Collections.Generic;
using UnityEngine;

public class MapDrawer : MonoBehaviour
{
    private TrailRenderer trail;               // Refer�ncia ao Trail Renderer
    private bool isDrawing = false;            // Indica se est� desenhando ou n�o

    private static List<MapDrawer> allMapDrawers = new List<MapDrawer>(); // Lista de todas as inst�ncias de MapDrawer

    void Awake()
    {
        // Adiciona esta inst�ncia � lista ao ser criada
        allMapDrawers.Add(this);
    }

    void OnDestroy()
    {
        // Remove esta inst�ncia da lista quando destru�da
        allMapDrawers.Remove(this);
    }

    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false; // Inicializa o Trail Renderer desativado
    }

    void Update()
    {
        // Detecta o clique do bot�o direito para alternar o desenho em todas as inst�ncias
        if (Input.GetMouseButtonDown(1))  // Bot�o direito do mouse
        {
            isDrawing = !isDrawing;  // Alterna o estado de desenho
            ToggleDrawingForAll(isDrawing); // Ativa/desativa todos os clones
        }
    }

    // M�todo para ativar/desativar o Trail Renderer para todas as inst�ncias
    private static void ToggleDrawingForAll(bool shouldDraw)
    {
        foreach (var mapDrawer in allMapDrawers)
        {
            if (shouldDraw)
            {
                // Inicia o desenho em todas as inst�ncias
                //mapDrawer.trail.Clear();  // Limpa o rastro anterior para iniciar um novo
                mapDrawer.trail.emitting = true;
            }
            else
            {
                // Para o desenho em todas as inst�ncias
                mapDrawer.trail.emitting = false;
            }
        }
    }
}
