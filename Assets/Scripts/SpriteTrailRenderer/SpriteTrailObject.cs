using System.Collections.Generic;
using UnityEngine;

namespace SpriteTrailRenderer
{
    public class SpriteTrailObject : MonoBehaviour, IPoolable
    {
        public ReturnObjectToPool _returnToPool;

        public SpriteTrailRenderer _spriteTrailValues;
        private SpriteRenderer _spriteRenderer;
        private float _timeAlive;
        private bool _active;

        // Color palette
        public List<Color32> _colorsToUse;

        [Header("Layer Config")]
        [SerializeField] private string physicsLayerName = "Player";

        [Header("Sorting Layer Config")]
        [SerializeField] private string sortingLayerName = "Player";
        [SerializeField] private int sortingOrder = 0;

        private void Awake()
        {
            _returnToPool = (x) => x.SetActive(false);
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _timeAlive = 0f;
            _active = false;

            // Physics layer
            ApplyPhysicsLayer();

            // Sorting Layer
            ApplySortingLayer();
        }

        private void Update()
        {
            if (_active)
            {
                _timeAlive += Time.deltaTime;
                float percentDone = Mathf.Min(_timeAlive / _spriteTrailValues._trailLifetime, .999f);
                Vector3 newZPosition = transform.position;
                newZPosition.z = (float)(_spriteTrailValues.transform.position.z + (.01f * percentDone));
                transform.position = newZPosition;

                Color newColor = _spriteRenderer.color;

                float newXScale = Mathf.Lerp(_spriteTrailValues._startScale.x, _spriteTrailValues._endScale.x, percentDone);
                newXScale = transform.localScale.x < 0 ? newXScale * -1 : newXScale;

                float newYScale = Mathf.Lerp(_spriteTrailValues._startScale.y, _spriteTrailValues._endScale.y, percentDone);
                newYScale = transform.localScale.y < 0 ? newYScale * -1 : newYScale;

                transform.localScale = new Vector2(newXScale, newYScale);

                if (_spriteTrailValues._useSolidColors)
                {
                    newColor = _colorsToUse[Mathf.FloorToInt(_colorsToUse.Count * percentDone)];
                }

                if (_spriteTrailValues._alphaUpdateOn)
                {
                    newColor.a = ((1f - percentDone) * (_spriteTrailValues._maxAlpha - _spriteTrailValues._minAlpha)) + _spriteTrailValues._minAlpha;
                }

                _spriteRenderer.color = newColor;

                if (_timeAlive >= _spriteTrailValues._trailLifetime)
                {
                    _active = false;
                    _returnToPool(gameObject);
                }
            }
        }

        public void SetSpawnValues(SpriteRenderer spriteRenderer, SpriteTrailRenderer spriteTrailRenderer, List<Color32> colors)
        {
            _spriteTrailValues = spriteTrailRenderer;

            _spriteRenderer.flipX = spriteRenderer.flipX;
            _spriteRenderer.flipY = spriteRenderer.flipY;
            _spriteRenderer.sprite = spriteRenderer.sprite;

            _colorsToUse = colors;

            if (_spriteTrailValues._useSolidColors)
            {
                Shader shaderGUItext = Shader.Find("GUI/Text Shader");
                _spriteRenderer.material.shader = shaderGUItext;
                _spriteRenderer.color = Color.clear;
            }
            else
            {
                Shader spriteShader = Shader.Find("Sprites/Default");
                _spriteRenderer.material.shader = spriteShader;
                _spriteRenderer.color = spriteRenderer.color;
            }

            _timeAlive = 0f;
            _active = true;

            // Physics Layer
            ApplyPhysicsLayer();

            // Sorting Layer
            ApplySortingLayer();
        }

        public void SetReturnToPool(ReturnObjectToPool returnDelegate)
        {
            _returnToPool = returnDelegate;
        }

        /// <summary>
        /// Aplica a Layer de Physics em si mesmo e nos filhos.
        /// </summary>
        private void ApplyPhysicsLayer()
        {
            int targetLayer = LayerMask.NameToLayer(physicsLayerName);
            if (targetLayer == -1)
            {
                //Debug.LogWarning($"A physics layer '{physicsLayerName}' não existe! Verifique em Tags & Layers.");
                return;
            }
            SetLayerRecursively(gameObject, targetLayer);
        }

        /// <summary>
        /// Aplica a Sorting Layer no SpriteRenderer.
        /// </summary>
        private void ApplySortingLayer()
        {
            if (!string.IsNullOrEmpty(sortingLayerName))
            {
                _spriteRenderer.sortingLayerName = sortingLayerName;
                _spriteRenderer.sortingOrder = sortingOrder;
            }
            else
            {
                Debug.LogWarning("Sorting Layer name está vazio! Configure no Inspector.");
            }
        }

        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}
