using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeapon : MonoBehaviour
{
    public float throwPower;
    public Rigidbody2D theRB;
    public float rotateSpeed;

    private TrailRenderer trail;       // Refer�ncia ao Trail Renderer
    private bool isDrawing = false;    // Indica se est� desenhando ou n�o

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

        // Detecta o clique do bot�o direito para alternar o desenho
        if (Input.GetMouseButtonDown(1))  // Bot�o direito do mouse
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
