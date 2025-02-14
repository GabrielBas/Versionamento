using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeapon : MonoBehaviour
{
    public float throwPower;
    public Rigidbody2D theRB;
    public float rotateSpeed;

    private TrailRenderer trail;       // Referência ao Trail Renderer
    private bool isDrawing = false;    // Indica se está desenhando ou não

    // Start is called before the first frame update
    void Start()
    {
        theRB.velocity = new Vector2(Random.Range(-throwPower, throwPower), throwPower);

        trail = GetComponent<TrailRenderer>();
        trail.emitting = false; // Inicializa o Trail Renderer desativado

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f,0f, transform.rotation.eulerAngles.z + (rotateSpeed * 360f * Time.deltaTime * Mathf.Sign(theRB.velocity.x)));

        // Detecta o clique do botão direito para alternar o desenho
        if (Input.GetMouseButtonDown(1))  // Botão direito do mouse
        {
            isDrawing = !isDrawing;  // Alterna o estado de desenho

            if (isDrawing)
            {
                //trail.Clear();  // Limpa o rastro anterior
                trail.emitting = true;  // Inicia o desenho
            }
            else
            {
                trail.emitting = false;  // Para o desenho
            }
        }

    }
}
