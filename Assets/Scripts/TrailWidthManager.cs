using UnityEngine;
using System.Collections.Generic;

public class TrailWidthManager : MonoBehaviour
{
    public static TrailWidthManager Instance;

    public float TrailWidth { get; private set; } = 0.5f; // Espessura padrão
    private List<TrailRenderer> activeTrails = new List<TrailRenderer>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTrailWidth();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTrailWidth(float newWidth)
    {
        TrailWidth = newWidth;
        SaveTrailWidth();
        UpdateAllTrails();
        Debug.Log("TrailWidth atualizado para: " + TrailWidth);
    }

    private void SaveTrailWidth()
    {
        PlayerPrefs.SetFloat("TrailWidth", TrailWidth);
        PlayerPrefs.Save();
    }

    private void LoadTrailWidth()
    {
        if (PlayerPrefs.HasKey("TrailWidth"))
        {
            TrailWidth = PlayerPrefs.GetFloat("TrailWidth");
        }
    }

    public void RegisterTrail(TrailRenderer trail)
    {
        if (!activeTrails.Contains(trail))
        {
            activeTrails.Add(trail);
        }

        // Atualiza a largura ao registrar
        trail.startWidth = TrailWidth;
        trail.endWidth = TrailWidth;
        Debug.Log("Trail registrado: " + trail.name + " com largura: " + TrailWidth);
    }

    public void UpdateAllTrails()
    {
        foreach (var trail in activeTrails)
        {
            if (trail != null)
            {
                trail.startWidth = TrailWidth;
                trail.endWidth = TrailWidth;

                // Força a reconstrução do trail
                trail.Clear();
            }
        }
        Debug.Log("Todos os trails atualizados com largura: " + TrailWidth);
    }

}
