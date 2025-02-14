using System.Collections.Generic;
using UnityEngine;

public class MapDrawer : MonoBehaviour
{
    private TrailRenderer trail;               // Referência ao Trail Renderer
    private bool isDrawing = false;            // Indica se está desenhando ou não

    private static List<MapDrawer> allMapDrawers = new List<MapDrawer>(); // Lista de todas as instâncias de MapDrawer

    void Awake()
    {
        // Adiciona esta instância à lista ao ser criada
        allMapDrawers.Add(this);
    }

    void OnDestroy()
    {
        // Remove esta instância da lista quando destruída
        allMapDrawers.Remove(this);
    }

    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false; // Inicializa o Trail Renderer desativado
    }

    void Update()
    {
        // Detecta o clique do botão direito para alternar o desenho em todas as instâncias
        if (Input.GetMouseButtonDown(1))  // Botão direito do mouse
        {
            isDrawing = !isDrawing;  // Alterna o estado de desenho
            ToggleDrawingForAll(isDrawing); // Ativa/desativa todos os clones
        }
    }

    // Método para ativar/desativar o Trail Renderer para todas as instâncias
    private static void ToggleDrawingForAll(bool shouldDraw)
    {
        foreach (var mapDrawer in allMapDrawers)
        {
            if (shouldDraw)
            {
                // Inicia o desenho em todas as instâncias
                //mapDrawer.trail.Clear();  // Limpa o rastro anterior para iniciar um novo
                mapDrawer.trail.emitting = true;
            }
            else
            {
                // Para o desenho em todas as instâncias
                mapDrawer.trail.emitting = false;
            }
        }
    }
}
