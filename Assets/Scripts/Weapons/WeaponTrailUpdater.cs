using UnityEngine;

public class WeaponTrailUpdater : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            TrailWidthManager.Instance.RegisterTrail(trailRenderer);
            ApplyTrailWidth();
        }
    }

    private void ApplyTrailWidth()
    {
        if (TrailWidthManager.Instance != null)
        {
            trailRenderer.startWidth = TrailWidthManager.Instance.TrailWidth;
            trailRenderer.endWidth = TrailWidthManager.Instance.TrailWidth;

            // Força a limpeza e atualização do mesh para aplicar os novos valores
            trailRenderer.Clear();
            Debug.Log("Espessura aplicada ao trail: " + trailRenderer.name + " com largura: " + TrailWidthManager.Instance.TrailWidth);
        }
    }

}
