using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenTargetIndicator : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public float proximityThreshold = 2f; // Distância em que a seta será destruída

    private SpriteRenderer _spriteRenderer;
    private float _spriteWidth;
    private float _spriteHeight;

    private Camera _camera;
    private GameObject player;

    private static List<TargetIndicatorPair> targets = new List<TargetIndicatorPair>();

    private void Start()
    {
        _camera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");

        if (indicatorPrefab != null)
        {
            _spriteRenderer = indicatorPrefab.GetComponent<SpriteRenderer>();
            var bounds = _spriteRenderer.bounds;
            _spriteHeight = bounds.size.y / 2f;
            _spriteWidth = bounds.size.x / 2f;
        }
        else
        {
            Debug.LogError("Indicator Prefab is not set!");
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return; // Exit if player is still not found
        }

        for (int i = targets.Count - 1; i >= 0; i--)
        {
            var targetPair = targets[i];
            var target = targetPair.Target;
            var indicator = targetPair.Indicator;

            if (target == null || indicator == null)
            {
                if (indicator != null)
                {
                    Destroy(indicator);
                }
                targets.RemoveAt(i);
                continue;
            }

            if (Vector2.Distance(player.transform.position, target.transform.position) < proximityThreshold)
            {
                Destroy(indicator);
                targets.RemoveAt(i);
            }
            else
            {
                UpdateTarget(target, indicator);
            }
        }
    }

    private void UpdateTarget(GameObject target, GameObject indicator)
    {
        var screenPos = _camera.WorldToViewportPoint(target.transform.position);
        bool isOffScreen = screenPos.x <= 0 || screenPos.x >= 1 || screenPos.y <= 0 || screenPos.y >= 1;
        if (isOffScreen)
        {
            indicator.SetActive(true);
            var spriteSizeInViewPort = _camera.WorldToViewportPoint(new Vector3(_spriteWidth, _spriteHeight, 0))
              - _camera.WorldToViewportPoint(Vector3.zero);

            screenPos.x = Mathf.Clamp(screenPos.x, spriteSizeInViewPort.x, 1 - spriteSizeInViewPort.x);
            screenPos.y = Mathf.Clamp(screenPos.y, spriteSizeInViewPort.y, 1 - spriteSizeInViewPort.y);

            var worldPosition = _camera.ViewportToWorldPoint(screenPos);
            worldPosition.z = 0;
            indicator.transform.position = worldPosition;

            Vector3 direction = target.transform.position - indicator.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            indicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else
        {
            indicator.SetActive(false);
        }
    }

    public static void RegisterTarget(GameObject target, GameObject indicator)
    {
        if (target == null || indicator == null) return;

        targets.Add(new TargetIndicatorPair(target, indicator));
    }

    public static void UnregisterTarget(GameObject target)
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i].Target == target)
            {
                if (targets[i].Indicator != null)
                {
                    Destroy(targets[i].Indicator);
                }
                targets.RemoveAt(i);
            }
        }
    }

    private class TargetIndicatorPair
    {
        public GameObject Target { get; private set; }
        public GameObject Indicator { get; private set; }

        public TargetIndicatorPair(GameObject target, GameObject indicator)
        {
            Target = target;
            Indicator = indicator;
        }
    }
}
