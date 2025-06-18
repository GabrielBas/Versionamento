using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float lifeTime = 1f;
    public float floatSpeed;

    public TMP_Text textMesh;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }
    }
}
